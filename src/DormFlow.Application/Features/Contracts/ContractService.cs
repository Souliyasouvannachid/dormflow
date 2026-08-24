using System.Globalization;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Reservations;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Leases;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Contracts;

/// <summary>
/// Use Case ทั้งหมดของโมดูลสัญญาเช่า
/// </summary>
/// <remarks>
/// เช็คอินและเช็คเอาท์ถูกรวมไว้เป็นการกระทำบนสัญญา (<see cref="ActivateAsync"/> และ <see cref="TerminateAsync"/>)
/// แทนการแยกเป็นโมดูลของตัวเอง ดูเหตุผลเพิ่มเติมที่ <see cref="Domain.Entities.Contract"/>
/// </remarks>
public sealed class ContractService(
    IContractRepository repository,
    IRoomRepository roomRepository,
    ITenantRepository tenantRepository,
    IReservationRepository reservationRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateContractRequest> createValidator,
    IValidator<TerminateContractRequest> terminateValidator) : IContractService
{
    private const string AuditEntityName = "Contract";

    /// <summary>จำนวนครั้งสูงสุดที่ลองสุ่มเลขที่สัญญาใหม่เมื่อชนกับที่มีอยู่แล้ว</summary>
    private const int MaxContractNumberAttempts = 5;

    private static readonly string[] AllowedSortFields = ["contractNumber", "startDate", "endDate", "status", "createdAt"];

    private readonly IContractRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateContractRequest> _createValidator = createValidator;
    private readonly IValidator<TerminateContractRequest> _terminateValidator = terminateValidator;

    /// <inheritdoc />
    public async Task<PagedResult<ContractResponse>> GetPagedAsync(
        ContractListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        ContractListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.RoomId,
            request.TenantId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Contract> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<ContractResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<ContractResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<ContractResponse> CreateAsync(
        CreateContractRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        Room room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบห้องที่ระบุ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ไม่พบห้องที่ระบุ");

        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");

        if (tenant.OrganizationId != room.OrganizationId)
        {
            throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");
        }

        if (tenant.Status == TenantStatus.Blacklisted)
        {
            throw new ConflictException("ผู้เช่ารายนี้ถูกขึ้นบัญชีดำ ไม่สามารถทำสัญญาเช่าได้");
        }

        if (await _repository.HasOpenContractAsync(room.Id, cancellationToken))
        {
            throw new ConflictException("ห้องนี้มีสัญญาเช่าที่ยังไม่สิ้นสุดอยู่แล้ว");
        }

        Reservation? reservation = await ResolveReservationAsync(request, room, tenant, cancellationToken);

        if (reservation is null)
        {
            // ทำสัญญาโดยไม่ผ่านการจองล่วงหน้า ห้องต้องว่างอยู่จริงเท่านั้น
            if (room.Status != RoomStatus.Available)
            {
                throw new ConflictException("ห้องนี้ไม่ว่าง ไม่สามารถทำสัญญาได้");
            }

            room.Status = RoomStatus.Reserved;
        }

        string contractNumber = await GenerateUniqueContractNumberAsync(room.OrganizationId, cancellationToken);

        Contract contract = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            TenantId = tenant.Id,
            ReservationId = reservation?.Id,
            ContractNumber = contractNumber,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MonthlyRent = request.MonthlyRent ?? room.MonthlyRent,
            DepositAmount = request.DepositAmount ?? room.DepositAmount,
            Notes = request.Notes,
        };

        await _repository.AddAsync(contract, cancellationToken);

        // เรียกครั้งเดียวเพื่อบันทึกสัญญาใหม่พร้อมกับการเปลี่ยนแปลงของห้องและการจอง (ถ้ามี) ในธุรกรรมเดียวกัน
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            contract.Id.ToString(),
            organizationId: contract.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(contract);
    }

    /// <inheritdoc />
    public async Task<ContractResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Contract contract = await FindAccessibleAsync(id, cancellationToken);

        if (contract.Status != ContractStatus.Draft)
        {
            throw new ConflictException("เปิดใช้งานได้เฉพาะสัญญาที่ยังเป็นร่างเท่านั้น");
        }

        Room? room = await _roomRepository.GetByIdAsync(contract.RoomId, cancellationToken);
        if (room is not null)
        {
            room.Status = RoomStatus.Occupied;
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        contract.Status = ContractStatus.Active;
        contract.SignedAt = now;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            contract.Id.ToString(),
            organizationId: contract.OrganizationId,
            newValues: $$"""{"status":"{{ContractStatus.Active}}","checkInAt":"{{now:O}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(contract);
    }

    /// <inheritdoc />
    public async Task<ContractResponse> TerminateAsync(
        Guid id,
        TerminateContractRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _terminateValidator.ValidateAndThrowAsync(request, cancellationToken);

        Contract contract = await FindAccessibleAsync(id, cancellationToken);

        if (contract.Status != ContractStatus.Active)
        {
            throw new ConflictException("สิ้นสุดได้เฉพาะสัญญาที่เปิดใช้งานอยู่เท่านั้น");
        }

        Room? room = await _roomRepository.GetByIdAsync(contract.RoomId, cancellationToken);
        if (room is not null)
        {
            // ห้องเข้าสู่สถานะรอทำความสะอาดก่อน แล้วเจ้าหน้าที่ค่อยเปลี่ยนเป็นว่างเองผ่าน Endpoint ของห้อง
            room.Status = RoomStatus.Cleaning;
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;
        contract.Status = ContractStatus.Terminated;
        contract.TerminatedAt = now;
        contract.TerminationReason = request.Reason;

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            contract.Id.ToString(),
            organizationId: contract.OrganizationId,
            newValues: $$"""{"status":"{{ContractStatus.Terminated}}","checkOutAt":"{{now:O}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(contract);
    }

    private async Task<Reservation?> ResolveReservationAsync(
        CreateContractRequest request,
        Room room,
        Tenant tenant,
        CancellationToken cancellationToken)
    {
        if (request.ReservationId is null)
        {
            return null;
        }

        Reservation reservation = await _reservationRepository.GetByIdAsync(request.ReservationId.Value, cancellationToken)
            ?? throw new NotFoundException("ไม่พบการจองที่ระบุ");

        if (reservation.RoomId != room.Id || reservation.TenantId != tenant.Id)
        {
            throw new ValidationFailedException("การจองที่ระบุไม่ตรงกับห้องหรือผู้เช่าที่เลือก");
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ConflictException("การจองนี้ถูกใช้ทำสัญญาไปแล้วหรือถูกยกเลิกไปแล้ว");
        }

        reservation.Status = ReservationStatus.ConvertedToContract;

        return reservation;
    }

    private async Task<string> GenerateUniqueContractNumberAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        string today = _dateTimeProvider.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        for (int attempt = 0; attempt < MaxContractNumberAttempts; attempt++)
        {
            string candidate = $"CT-{today}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

            if (!await _repository.ExistsByContractNumberAsync(organizationId, candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("ไม่สามารถสุ่มเลขที่สัญญาที่ไม่ซ้ำกันได้ กรุณาลองใหม่อีกครั้ง");
    }

    private async Task<Contract> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Contract contract = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลสัญญาเช่าที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            contract.OrganizationId,
            contract.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลสัญญาเช่าที่ต้องการ");

        return contract;
    }

    private static ContractStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out ContractStatus parsed)
            ? parsed
            : null;

    private static ContractResponse ToResponse(Contract contract) => new()
    {
        Id = contract.Id,
        OrganizationId = contract.OrganizationId,
        DormitoryId = contract.DormitoryId.GetValueOrDefault(),
        RoomId = contract.RoomId,
        TenantId = contract.TenantId,
        ReservationId = contract.ReservationId,
        ContractNumber = contract.ContractNumber,
        StartDate = contract.StartDate,
        EndDate = contract.EndDate,
        MonthlyRent = contract.MonthlyRent,
        DepositAmount = contract.DepositAmount,
        Status = contract.Status.ToString(),
        SignedAt = contract.SignedAt,
        TerminatedAt = contract.TerminatedAt,
        TerminationReason = contract.TerminationReason,
        Notes = contract.Notes,
        CreatedAt = contract.CreatedAt,
    };
}
