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
/// Use Case ໂມດູນການເຊົ່າທັງໝົດ
/// </summary>
/// <remarks>
/// ການເຊັກອິນແລະເຊັກເອົາແມ່ນລວມເຂົ້າໃນການດໍາເນີນການໃນສັນຍາ (<see cref="ActivateAsync"/> ແລະ <see cref="TerminateAsync"/>)
/// ແທນທີ່ຈະແຍກມັນເຂົ້າໄປໃນໂມດູນຂອງຕົນເອງ ເບິ່ງເຫດຜົນເພີ່ມເຕີມຢູ່ທີ່ <see cref="Domain.Entities.Contract"/>
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

    /// <summary>ຈຳນວນຄັ້ງສູງສຸດເພື່ອທົດລອງໃຊ້ໝາຍເລກສັນຍາແບບສຸ່ມໃໝ່ເມື່ອຂັດກັບອັນທີ່ມີຢູ່ແລ້ວ</summary>
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
            ?? throw new NotFoundException("ບໍ່ພົບຫ້ອງທີ່ລະບຸ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            room.OrganizationId,
            room.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຫ້ອງທີ່ລະບຸ");

        Tenant tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຜູ້ເຊົ່າທີ່ລະບຸ");

        if (tenant.OrganizationId != room.OrganizationId)
        {
            throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນຜູ້ເຊົ່າທີ່ລະບຸ");
        }

        if (tenant.Status == TenantStatus.Blacklisted)
        {
            throw new ConflictException("ຜູ້ເຊົ່າລາຍນີ້ຖືກຂຶ້ນບັນຊີດຳ ບໍ່ສາມາດເຮັດສັນຍາເຊົ່າໄດ້");
        }

        if (await _repository.HasOpenContractAsync(room.Id, cancellationToken))
        {
            throw new ConflictException("ຫ້ອງນີ້ມີສັນຍາເຊົ່າທີ່ຍັງບໍ່ສິ້ນສຸດຢູ່ແລ້ວ");
        }

        Reservation? reservation = await ResolveReservationAsync(request, room, tenant, cancellationToken);

        if (reservation is null)
        {
            // ເຮັດສັນຍາໂດຍບໍ່ມີການຈອງລ່ວງຜ້າ. ຕົວຈິງແລ້ວຫ້ອງຈະຕ້ອງຫວ່າງເປົ່າ
            if (room.Status != RoomStatus.Available)
            {
                throw new ConflictException("ຫ້ອງນີ້ບໍ່ວ່າງ ບໍ່ສາມາດເຮັດສັນຍາໄດ້");
            }

            room.Status = RoomStatus.Reserved;
        }

        if (request.WaterRatePerUnit < 0 || request.ElectricityRatePerUnit < 0)
            throw new ValidationFailedException("Utility rates must not be negative.");
        if (request.CoTenantIds.Distinct().Count() + 1 > room.MaximumOccupants)
            throw new ValidationFailedException("Occupants exceed room capacity.");
        foreach (Guid coTenantId in request.CoTenantIds.Distinct())
        {
            Tenant? coTenant = await _tenantRepository.GetByIdAsync(coTenantId, cancellationToken);
            if (coTenant is null || coTenant.OrganizationId != room.OrganizationId)
                throw new NotFoundException("Co-tenant not found.");
            if (coTenant.Status == TenantStatus.Blacklisted || coTenantId == tenant.Id)
                throw new ValidationFailedException("Invalid co-tenant.");
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
            WaterRatePerUnit = request.WaterRatePerUnit,
            ElectricityRatePerUnit = request.ElectricityRatePerUnit,
            CoTenantIds = request.CoTenantIds.Distinct().ToArray(),
            Notes = request.Notes,
        };

        await _repository.AddAsync(contract, cancellationToken);

        // ໂທຫາຄັ້ງດຽວເພື່ອບັນທຶກສັນຍາໃຝ່ພ້ອມກັບການປ່ຽນແປງຫ້ອງແລະການຈອງ (ຖ້າມີ) ໃນທຸລະກໍາດຽວກັນ
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
            throw new ConflictException("ເປີດໃຊ້ງານໄດ້ສະເພາະສັນຍາທີ່ຍັງເປັນຮ່າງເທົ່ານັ້ນ");
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
            throw new ConflictException("ສິ້ນສຸດໄດ້ສະເພາະສັນຍາທີ່ເປີດໃຊ້ງານຢູ່ເທົ່ານັ້ນ");
        }

        Room? room = await _roomRepository.GetByIdAsync(contract.RoomId, cancellationToken);
        if (room is not null)
        {
            // ຫ້ອງເຂົ້າໄປໃນສະຖານະລໍຖ້າການເຮັດຄວາມສະອາດກ່ອນ. ຈາກນັ້ນເຈົ້າໜ້າທີ່ກໍຄ່ອຍໆປ່ຽນໃຫ້ເປັນອິດສະລະດ້ວຍຕົນເອງ Endpoint ຂອງຫ້ອງ
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
            ?? throw new NotFoundException("ບໍ່ພົບການຈອງທີ່ລະບຸ");

        if (reservation.RoomId != room.Id || reservation.TenantId != tenant.Id)
        {
            throw new ValidationFailedException("ການຈອງທີ່ລະບຸບໍ່ກົງກັບຫ້ອງຫຼືຜູ້ເຊົ່າທີ່ເລືອກ");
        }

        if (reservation.Status != ReservationStatus.Pending)
        {
            throw new ConflictException("ການຈອງນີ້ຖືກໃຊ້ເຮັດສັນຍາໄປແລ້ວ ຫຼືຖືກຍົກເລີກໄປແລ້ວ");
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

        throw new InvalidOperationException("ບໍ່ສາມາດສຸ່ມເລກທີ່ສັນຍາທີ່ບໍ່ຊ້ຳກັນໄດ້ ກະລຸນາລອງໃໝ່ອີກຄັ້ງ");
    }

    private async Task<Contract> FindAccessibleAsync(Guid id, CancellationToken cancellationToken)
    {
        Contract contract = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("ບໍ່ພົບຂໍ້ມູນສັນຍາເຊົ່າທີ່ຕ້ອງການ");

        TenantScope.EnsureDormitoryAccess(
            _currentUser,
            contract.OrganizationId,
            contract.DormitoryId.GetValueOrDefault(),
            "ບໍ່ພົບຂໍ້ມູນສັນຍາເຊົ່າທີ່ຕ້ອງການ");

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
        WaterRatePerUnit = contract.WaterRatePerUnit,
        ElectricityRatePerUnit = contract.ElectricityRatePerUnit,
        CoTenantIds = contract.CoTenantIds,
        Status = contract.Status.ToString(),
        SignedAt = contract.SignedAt,
        TerminatedAt = contract.TerminatedAt,
        TerminationReason = contract.TerminationReason,
        Notes = contract.Notes,
        CreatedAt = contract.CreatedAt,
    };
}
