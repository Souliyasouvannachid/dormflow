using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.MeterReadings;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.MeterReadings;

/// <summary>
/// Use Case ทั้งหมดของโมดูลค่ามิเตอร์
/// </summary>
/// <remarks>
/// ไม่มี Use Case แก้ไขหรือลบโดยตั้งใจ ดูเหตุผลเพิ่มเติมที่ <see cref="Domain.Entities.MeterReading"/>
/// </remarks>
public sealed class MeterReadingService(
    IMeterReadingRepository repository,
    IRoomRepository roomRepository,
    ICurrentUser currentUser,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateMeterReadingRequest> createValidator) : IMeterReadingService
{
    private const string AuditEntityName = "MeterReading";
    private static readonly string[] AllowedSortFields = ["periodMonth", "readingDate", "meterType", "createdAt"];

    private readonly IMeterReadingRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateMeterReadingRequest> _createValidator = createValidator;

    /// <inheritdoc />
    public async Task<PagedResult<MeterReadingResponse>> GetPagedAsync(
        MeterReadingListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "periodMonth");

        MeterReadingListFilter filter = new(
            request.DormitoryId,
            request.RoomId,
            ParseMeterTypeOrNull(request.MeterType),
            request.PeriodMonth.HasValue ? NormalizeToFirstOfMonth(request.PeriodMonth.Value) : null,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<MeterReading> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<MeterReadingResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<MeterReadingResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<MeterReadingResponse> CreateAsync(
        CreateMeterReadingRequest request,
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

        MeterType meterType = Enum.Parse<MeterType>(request.MeterType, ignoreCase: true);
        DateOnly periodMonth = NormalizeToFirstOfMonth(request.PeriodMonth);

        if (await _repository.ExistsForPeriodAsync(room.Id, meterType, periodMonth, cancellationToken))
        {
            throw new ConflictException("ห้องนี้ถูกจดค่ามิเตอร์ประเภทนี้ของเดือนที่ระบุไปแล้ว");
        }

        MeterReading? previous = await _repository.GetLatestBeforePeriodAsync(
            room.Id, meterType, periodMonth, cancellationToken);
        decimal previousReading = previous?.CurrentReading ?? 0;

        if (request.CurrentReading < previousReading)
        {
            throw new ValidationFailedException(
                $"ค่ามิเตอร์ที่จดได้ ({request.CurrentReading}) ต้องไม่น้อยกว่าค่าครั้งก่อนหน้า ({previousReading})");
        }

        MeterReading meterReading = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            MeterType = meterType,
            PeriodMonth = periodMonth,
            ReadingDate = request.ReadingDate,
            PreviousReading = previousReading,
            CurrentReading = request.CurrentReading,
            UnitsUsed = request.CurrentReading - previousReading,
            Notes = request.Notes,
        };

        await _repository.AddAsync(meterReading, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            meterReading.Id.ToString(),
            organizationId: meterReading.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(meterReading);
    }

    private async Task<MeterReading> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        MeterReading meterReading = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลค่ามิเตอร์ที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            meterReading.OrganizationId,
            meterReading.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลค่ามิเตอร์ที่ต้องการ");

        return meterReading;
    }

    private static DateOnly NormalizeToFirstOfMonth(DateOnly date) => new(date.Year, date.Month, 1);

    private static MeterType? ParseMeterTypeOrNull(string? meterType)
        => !string.IsNullOrWhiteSpace(meterType) && Enum.TryParse(meterType, ignoreCase: true, out MeterType parsed)
            ? parsed
            : null;

    private static MeterReadingResponse ToResponse(MeterReading meterReading) => new()
    {
        Id = meterReading.Id,
        OrganizationId = meterReading.OrganizationId,
        DormitoryId = meterReading.DormitoryId.GetValueOrDefault(),
        RoomId = meterReading.RoomId,
        MeterType = meterReading.MeterType.ToString(),
        PeriodMonth = meterReading.PeriodMonth,
        ReadingDate = meterReading.ReadingDate,
        PreviousReading = meterReading.PreviousReading,
        CurrentReading = meterReading.CurrentReading,
        UnitsUsed = meterReading.UnitsUsed,
        Notes = meterReading.Notes,
        CreatedAt = meterReading.CreatedAt,
    };
}
