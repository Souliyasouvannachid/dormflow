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

/// <summary>Use Case ໂມດູນການຈອງທັງໝົດ</summary>
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
    private const int ExpirationBatchSize = 500;
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
    public Task<int> GetPendingExpiringCountAsync(
        DateTimeOffset expiresBeforeOrAt,
        CancellationToken cancellationToken = default)
    {
        ReservationListFilter filter = new(
            TenantScope.GetOrganizationRestrictionForListing(_currentUser),
            DormitoryId: null,
            RoomId: null,
            TenantId: null,
            Status: ReservationStatus.Pending,
            TenantScope.GetDormitoryRestrictionForListing(_currentUser));

        return _repository.GetPendingExpiringCountAsync(filter, expiresBeforeOrAt, cancellationToken);
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
            ?? throw new NotFoundException("ບໍ່ພົບຫ້ອງທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຫ້ອງທີ່ລະບຸ");

        if (room.Status != RoomStatus.Available)
        {
            throw new ConflictException("ຫ້ອງນີ້ບໍ່ວ່າງ ບໍ່ສາມາດຈອງໄດ້");
        }

        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຜູ້ເຊົ່າທີ່ລະບຸ");

        if (tenant.OrganizationId != room.OrganizationId)
        {
            throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຜູ້ເຊົ່າທີ່ລະບຸ");
        }

        if (tenant.Status == TenantStatus.Blacklisted)
        {
            throw new ConflictException("ຜູ້ເຊົ່າລາຍນີ້ຖືກຂຶ້ນບັນຊີດຳ ບໍ່ສາມາດເຮັດການຈອງໄດ້");
        }

        // ປ້ອງກັນການຈອງສອງຄັ້ງເມື່ອມີສອງຄໍາຮ້ອງຂໍເຂົ້າມາໃນເວລາດຽວກັນສໍາລັບຫ້ອງດຽວກັນ
        // ພຽງແຕ່ກວດເບິ່ງສະຖານະຂອງຫ້ອງຂ້າງເທິງມີຊ່ອງຫວ່າງ race condition ລະຫວ່າງສອງຄໍາຮ້ອງຂໍເກືອບພ້ອມໆກັນ
        if (await _repository.HasPendingReservationAsync(room.Id, cancellationToken))
        {
            throw new ConflictException("ຫ້ອງນີ້ມີການຈອງຄ້າງຢູ່ແລ້ວ ບໍ່ສາມາດຈອງຊ້ຳໄດ້");
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

        // ໂທຫາຄັ້ງດຽວເພື່ອບັນທຶກການຈອງໃຝ່ແລະການປ່ຽນແປງສະຖານະຫ້ອງໃນທຸລະກໍາດຽວກັນ
        // ເນື່ອງຈາກວ່າ Room ແລະ Reservation ຖືກຕິດຕາມໃນ DbContext ດຽວກັນຢູ່ໃນຄໍາຮ້ອງຂໍນີ້
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
            throw new ConflictException("ຍົກເລີກໄດ້ສະເພາະການຈອງທີ່ຍັງລໍຖ້າດຳເນີນການເທົ່ານັ້ນ");
        }

        reservation.Status = ReservationStatus.Cancelled;

        // ພຽງແຕ່ປ່ອຍຫ້ອງຄືນຖ້າຫາກວ່າມັນຍັງຢູ່ໃນສະຖານະສະຫງວນ. ຖ້າມັນຖືກປ່ຽນເປັນລັດອື່ນ, ມັນຈະບໍ່ຖືກແຕະຕ້ອງ
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
        int expiredCount = 0;

        while (true)
        {
            IReadOnlyList<Reservation> expiredReservations = await _repository.GetExpiredPendingAsync(
                now,
                ExpirationBatchSize,
                cancellationToken);

            if (expiredReservations.Count == 0)
            {
                break;
            }

            IReadOnlyDictionary<Guid, Room> rooms = await _roomRepository.GetByIdsAsync(
                expiredReservations.Select(reservation => reservation.RoomId).Distinct().ToArray(),
                cancellationToken);

            foreach (Reservation reservation in expiredReservations)
            {
                reservation.Status = ReservationStatus.Expired;

                // ພຽງແຕ່ປ່ອຍຫ້ອງຄືນຖ້າຫາກວ່າມັນຍັງຢູ່ໃນສະຖານະສະຫງວນ. ຖ້າມັນຖືກປ່ຽນເປັນລັດອື່ນ, ມັນຈະບໍ່ຖືກແຕະຕ້ອງ
                // ຕາມເຫດຜົນດຽວກັນໃນ CancelAsync
                if (rooms.TryGetValue(reservation.RoomId, out Room? room)
                    && room.Status == RoomStatus.Reserved)
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

            await _repository.SaveChangesAsync(cancellationToken);
            expiredCount += expiredReservations.Count;

            if (expiredReservations.Count < ExpirationBatchSize)
            {
                break;
            }
        }

        return expiredCount;
    }

    private async Task<Reservation> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Reservation reservation = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນການຈອງທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            reservation.OrganizationId,
            reservation.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນການຈອງທີ່ຕ້ອງການ");

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
