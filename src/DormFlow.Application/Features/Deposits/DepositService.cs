using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Contracts;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Deposits;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Deposits;

/// <summary>
/// Use Case ทั้งหมดของโมดูลเงินมัดจำ
/// </summary>
/// <remarks>
/// แยกจากโมดูล Payments โดยตั้งใจ ดูเหตุผลเพิ่มเติมที่ <see cref="Domain.Entities.Deposit"/>
/// </remarks>
public sealed class DepositService(
    IDepositRepository repository,
    IContractRepository contractRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CollectDepositRequest> collectValidator,
    IValidator<RefundDepositRequest> refundValidator) : IDepositService
{
    private const string AuditEntityName = "Deposit";
    private static readonly string[] AllowedSortFields = ["amount", "status", "collectedAt", "createdAt"];

    private readonly IDepositRepository _repository = repository;
    private readonly IContractRepository _contractRepository = contractRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CollectDepositRequest> _collectValidator = collectValidator;
    private readonly IValidator<RefundDepositRequest> _refundValidator = refundValidator;

    /// <inheritdoc />
    public async Task<PagedResult<DepositResponse>> GetPagedAsync(
        DepositListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        DepositListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.ContractId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Deposit> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<DepositResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<DepositResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<DepositResponse> CollectAsync(
        CollectDepositRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _collectValidator.ValidateAndThrowAsync(request, cancellationToken);

        Contract contract = await _contractRepository.GetByIdAsync(request.ContractId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบสัญญาเช่าที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            contract.OrganizationId,
            contract.DormitoryId.GetValueOrDefault(),
            "ไม่พบสัญญาเช่าที่ระบุ");

        if (contract.Status is ContractStatus.Terminated or ContractStatus.Expired)
        {
            throw new ConflictException("สัญญาเช่านี้สิ้นสุดไปแล้ว ไม่สามารถรับเงินมัดจำได้");
        }

        if (await _repository.ExistsByContractIdAsync(contract.Id, cancellationToken))
        {
            throw new ConflictException("สัญญาเช่านี้มีการรับเงินมัดจำไว้แล้ว");
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;

        Deposit deposit = new()
        {
            OrganizationId = contract.OrganizationId,
            DormitoryId = contract.DormitoryId,
            ContractId = contract.Id,
            Amount = request.Amount ?? contract.DepositAmount,
            CollectedAt = now,
            Notes = request.Notes,
        };

        await _repository.AddAsync(deposit, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            deposit.Id.ToString(),
            organizationId: deposit.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(deposit);
    }

    /// <inheritdoc />
    public async Task<DepositResponse> RefundAsync(
        Guid id,
        RefundDepositRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _refundValidator.ValidateAndThrowAsync(request, cancellationToken);

        Deposit deposit = await FindAccessibleAsync(id, cancellationToken);

        if (deposit.Status != DepositStatus.Held)
        {
            throw new ConflictException("เงินมัดจำนี้ถูกคืนหรือริบไปแล้ว คืนได้เพียงครั้งเดียวต่อรายการ");
        }

        Contract contract = await _contractRepository.GetByIdAsync(deposit.ContractId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบสัญญาเช่าของเงินมัดจำนี้");

        if (contract.Status != ContractStatus.Terminated)
        {
            throw new ConflictException("คืนเงินมัดจำได้เฉพาะสัญญาเช่าที่สิ้นสุดแล้วเท่านั้น");
        }

        if (request.RefundAmount > deposit.Amount)
        {
            throw new ValidationFailedException(
                $"จำนวนเงินที่คืน ({request.RefundAmount}) เกินจำนวนเงินมัดจำที่รับไว้ ({deposit.Amount})");
        }

        if (request.RefundAmount < deposit.Amount && string.IsNullOrWhiteSpace(request.DeductionReason))
        {
            throw new ValidationFailedException("กรุณาระบุเหตุผลของส่วนที่หักไว้ไม่คืน");
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        deposit.RefundedAmount = request.RefundAmount;
        deposit.RefundedAt = now;
        deposit.DeductionReason = request.DeductionReason;
        deposit.Status = request.RefundAmount switch
        {
            var amount when amount == deposit.Amount => DepositStatus.Refunded,
            0 => DepositStatus.Forfeited,
            _ => DepositStatus.PartiallyRefunded,
        };

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            deposit.Id.ToString(),
            organizationId: deposit.OrganizationId,
            newValues: $$"""{"status":"{{deposit.Status}}","refundedAmount":{{deposit.RefundedAmount}}}""",
            cancellationToken: cancellationToken);

        return ToResponse(deposit);
    }

    private async Task<Deposit> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Deposit deposit = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลเงินมัดจำที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            deposit.OrganizationId,
            deposit.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลเงินมัดจำที่ต้องการ");

        return deposit;
    }

    private static DepositStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out DepositStatus parsed)
            ? parsed
            : null;

    private static DepositResponse ToResponse(Deposit deposit) => new()
    {
        Id = deposit.Id,
        OrganizationId = deposit.OrganizationId,
        DormitoryId = deposit.DormitoryId.GetValueOrDefault(),
        ContractId = deposit.ContractId,
        Amount = deposit.Amount,
        Status = deposit.Status.ToString(),
        CollectedAt = deposit.CollectedAt,
        RefundedAmount = deposit.RefundedAmount,
        RefundedAt = deposit.RefundedAt,
        DeductionReason = deposit.DeductionReason,
        Notes = deposit.Notes,
        CreatedAt = deposit.CreatedAt,
    };
}
