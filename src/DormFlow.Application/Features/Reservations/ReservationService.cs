using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Pagination;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Reservations;
using DormFlow.Domain.Entities;
using DormFlow.Domain.Enums;
using FluentValidation;

namespace DormFlow.Application.Features.Reservations;

/// <summary>Use Case ทั้งหมดของโมดูลการจอง</summary>
public sealed class ReservationService(
    IReservationRepository repository,
    IRoomRepository roomRepository,
    ITenantRepository tenantRepository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    IAuditLogWriter auditLogWriter,
    IValidator<CreateReservationRequest> createValidator) : IReservationService
{
    private const string AuditEntityName = "Reservation";
    private static readonly string[] AllowedSortFields = ["moveInDate", "expiresAt", "status", "createdAt"];

    private readonly IReservationRepository _repository = repository;
    private readonly IRoomRepository _roomRepository = roomRepository;
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IAuditLogWriter _auditLogWriter = auditLogWriter;
    private readonly IValidator<CreateReservationRequest> _createValidator = createValidator;

    /// <inheritdoc />
    public async Task<PagedResult<ReservationResponse>> GetPagedAsync(
        ReservationListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PagedQuery query = PagedQueryNormalizer.Normalize(request, AllowedSortFields, defaultSortBy: "createdAt");

        ReservationListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            request.DormitoryId,
            request.RoomId,
            request.TenantId,
            ParseStatusOrNull(request.Status),
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        PagedResult<Reservation> result = await _repository.GetPagedAsync(filter, query, cancellationToken);

        return PagedResult.Create(
            (IReadOnlyList<ReservationResponse>)[.. result.Items.Select(ToResponse)],
            result.Page,
            result.PageSize,
            result.TotalItems);
    }

    /// <inheritdoc />
    public async Task<ReservationResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => ToResponse(await FindAccessibleAsync(id, cancellationToken));

    /// <inheritdoc />
    public async Task<ReservationResponse> CreateAsync(
        CreateReservationRequest request,
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

        if (room.Status != RoomStatus.Available)
        {
            throw new ConflictException("ห้องนี้ไม่ว่าง ไม่สามารถจองได้");
        }

        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");

        if (tenant.OrganizationId != room.OrganizationId)
        {
            throw new NotFoundException("ไม่พบข้อมูลผู้เช่าที่ระบุ");
        }

        if (tenant.Status == TenantStatus.Blacklisted)
        {
            throw new ConflictException("ผู้เช่ารายนี้ถูกขึ้นบัญชีดำ ไม่สามารถทำการจองได้");
        }

        // ป้องกันการจองซ้ำซ้อนเมื่อมีคำขอสองคำขอเข้ามาพร้อมกันสำหรับห้องเดียวกัน
        // การตรวจสถานะห้องด้านบนอย่างเดียวมีช่องโหว่ race condition ระหว่างสองคำขอที่เกือบพร้อมกัน
        if (await _repository.HasPendingReservationAsync(room.Id, cancellationToken))
        {
            throw new ConflictException("ห้องนี้มีการจองค้างอยู่แล้ว ไม่สามารถจองซ้ำได้");
        }

        DateTimeOffset now = _dateTimeProvider.UtcNow;

        Reservation reservation = new()
        {
            OrganizationId = room.OrganizationId,
            DormitoryId = room.DormitoryId,
            RoomId = room.Id,
            TenantId = tenant.Id,
            ReservationDate = now,
            MoveInDate = request.MoveInDate,
            ExpiresAt = now.AddDays(request.HoldDays),
            ReservationFee = request.ReservationFee,
            Notes = request.Notes,
        };

        room.Status = RoomStatus.Reserved;

        await _repository.AddAsync(reservation, cancellationToken);

        // เรียกครั้งเดียวเพื่อบันทึกทั้งการจองใหม่และการเปลี่ยนสถานะห้องในธุรกรรมเดียวกัน
        // เพราะ Room และ Reservation ถูกติดตามอยู่ใน DbContext เดียวกันภายในคำขอนี้
        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.Create,
            AuditEntityName,
            reservation.Id.ToString(),
            organizationId: reservation.OrganizationId,
            cancellationToken: cancellationToken);

        return ToResponse(reservation);
    }

    /// <inheritdoc />
    public async Task<ReservationResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Reservation reservation = await FindAccessibleAsync(id, cancellationToken);

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ConflictException("ยกเลิกได้เฉพาะการจองที่ยังรอดำเนินการเท่านั้น");
        }

        reservation.Status = ReservationStatus.Cancelled;

        // ปล่อยห้องคืนเฉพาะเมื่อยังอยู่ในสถานะถูกจองอยู่ ถ้าถูกเปลี่ยนสถานะไปทางอื่นแล้วจะไม่แตะต้อง
        Room? room = await _roomRepository.GetByIdAsync(reservation.RoomId, cancellationToken);
        if (room is not null && room.Status == RoomStatus.Reserved)
        {
            room.Status = RoomStatus.Available;
        }

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditLogWriter.WriteAsync(
            AuditAction.ChangeStatus,
            AuditEntityName,
            reservation.Id.ToString(),
            organizationId: reservation.OrganizationId,
            newValues: $$"""{"status":"{{ReservationStatus.Cancelled}}"}""",
            cancellationToken: cancellationToken);

        return ToResponse(reservation);
    }

    /// <inheritdoc />
    public async Task<int> ExpirePastDueReservationsAsync(CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = _dateTimeProvider.UtcNow;

        IReadOnlyList<Reservation> expiredReservations = await _repository.GetExpiredPendingAsync(now, cancellationToken);

        foreach (Reservation reservation in expiredReservations)
        {
            reservation.Status = ReservationStatus.Expired;

            // ปล่อยห้องคืนเฉพาะเมื่อยังอยู่ในสถานะถูกจองอยู่ ถ้าถูกเปลี่ยนสถานะไปทางอื่นแล้วจะไม่แตะต้อง
            // เหมือนกับตรรกะเดียวกันใน CancelAsync
            Room? room = await _roomRepository.GetByIdAsync(reservation.RoomId, cancellationToken);
            if (room is not null && room.Status == RoomStatus.Reserved)
            {
                room.Status = RoomStatus.Available;
            }

            await _auditLogWriter.WriteAsync(
                AuditAction.ChangeStatus,
                AuditEntityName,
                reservation.Id.ToString(),
                organizationId: reservation.OrganizationId,
                newValues: $$"""{"status":"{{ReservationStatus.Expired}}"}""",
                cancellationToken: cancellationToken);
        }

        if (expiredReservations.Count > 0)
        {
            await _repository.SaveChangesAsync(cancellationToken);
        }

        return expiredReservations.Count;
    }

    private async Task<Reservation> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Reservation reservation = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ไม่พบข้อมูลการจองที่ต้องการ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            reservation.OrganizationId,
            reservation.DormitoryId.GetValueOrDefault(),
            "ไม่พบข้อมูลการจองที่ต้องการ");

        return reservation;
    }

    private static ReservationStatus? ParseStatusOrNull(string? status)
        => !string.IsNullOrWhiteSpace(status) && Enum.TryParse(status, ignoreCase: true, out ReservationStatus parsed)
            ? parsed
            : null;

    private static ReservationResponse ToResponse(Reservation reservation) => new()
    {
        Id = reservation.Id,
        OrganizationId = reservation.OrganizationId,
        DormitoryId = reservation.DormitoryId.GetValueOrDefault(),
        RoomId = reservation.RoomId,
        TenantId = reservation.TenantId,
        ReservationDate = reservation.ReservationDate,
        MoveInDate = reservation.MoveInDate,
        ExpiresAt = reservation.ExpiresAt,
        ReservationFee = reservation.ReservationFee,
        Status = reservation.Status.ToString(),
        Notes = reservation.Notes,
        CreatedAt = reservation.CreatedAt,
    };
}
