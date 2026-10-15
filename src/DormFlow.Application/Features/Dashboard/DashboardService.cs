using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Authorization;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Application.Features.Maintenance;
using DormFlow.Application.Features.Parcels;
using DormFlow.Application.Features.Payments;
using DormFlow.Application.Features.Reports;
using DormFlow.Application.Features.Reservations;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Visitors;
using DormFlow.Contracts.AuditLogs;
using DormFlow.Contracts.Common;
using DormFlow.Contracts.Dashboard;
using DormFlow.Contracts.Maintenance;
using DormFlow.Contracts.Reports;
using DormFlow.Contracts.Rooms;
using DormFlow.Domain.Enums;

namespace DormFlow.Application.Features.Dashboard;

/// <summary>
/// Use Case ຂອງ dashboards, ສົມທົບຂໍ້ມູນຈາກຫຼາຍໂມດູນໂດຍຜ່ານ Service ຂອງແຕ່ລະໂມດູນໂດຍກົງ
/// (ບໍ່ມີ Repository ຂອງຕົນເອງ) ເພື່ອອະນຸຍາດໃຫ້ການກັ່ນຕອງໂດຍອົງກອນ/ຫໍພັກຂອງຜູ້ໃຊ້ປະຈຸບັນຖືກໃຊ້ຄືນຈາກ
/// ຈຸດກວດກາ (ລວມທັງ Regression Test ຊ່ອງໂຫວ່ຂອງຂໍ້ມູນຮົ່ວໄຫຼໃນທົ່ວອົງກອນ) ແລ້ວໃນແຕ່ລະໂມດູນ
/// ແທນທີ່ຈະຄັດລອກແລະຂຽນຄືນເຫດຜົນການກັ່ນຕອງ
/// </summary>
/// <remarks>
/// ໂທຫາແຕ່ລະໂມດູນຕາມລໍາດັບ (ຢ່າໃຊ້ Task.WhenAll) ເນື່ອງຈາກວ່າ Service ທັງຝົດທີ່ເອີ້ນວ່າການນໍາໃຊ້
/// DbContext ອັນດຽວກັນທີ່ຜູກມັດກັບຂອບເຂດຂອງການຮ້ອງຂໍນີ້. ເຊິ່ງບໍ່ຮອງຮັບການປະຕິບັດຄຳສັ່ງຫຼາຍອັນພ້ອມກັນ
/// </remarks>
public sealed class DashboardService(
    DormFlow.Application.Features.Operations.IAnalyticsService analyticsService,
    IRoomService roomService,
    IMaintenanceRequestService maintenanceService,
    IReservationService reservationService,
    IParcelService parcelService,
    IVisitorService visitorService,
    IPaymentService paymentService,
    IAuditLogQueryService auditLogQueryService,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider) : IDashboardService
{
    private const int RevenueTrendMonths = 6;
    private const int RecentActivityCount = 8;
    private const int ExpiringReservationWindowHours = 24;

    private readonly IRoomService _roomService = roomService;
    private readonly IMaintenanceRequestService _maintenanceService = maintenanceService;
    private readonly IReservationService _reservationService = reservationService;
    private readonly IParcelService _parcelService = parcelService;
    private readonly IVisitorService _visitorService = visitorService;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IAuditLogQueryService _auditLogQueryService = auditLogQueryService;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    /// <inheritdoc />
    public async Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        DateTimeOffset now = _dateTimeProvider.UtcNow;
        DateOnly today = DateOnly.FromDateTime(now.ToOffset(TimeSpan.FromHours(7)).DateTime);
        DateOnly currentMonth = new(today.Year, today.Month, 1);

        RoomSummaryResponse roomSummary = await _roomService.GetSummaryAsync(
            new RoomSummaryRequest(), cancellationToken);

        int openMaintenanceCount = await GetOpenMaintenanceCountAsync(cancellationToken);

        bool isTechnician = _currentUser.Roles.Contains(DormFlowRoles.Technician, StringComparer.Ordinal);
        IReadOnlyList<MaintenanceRequestResponse> myAssignedMaintenance = isTechnician
            ? await GetMyAssignedMaintenanceAsync(cancellationToken)
            : [];

        int expiringReservationsCount = await GetExpiringReservationsCountAsync(now, cancellationToken);

        int awaitingParcels = await _parcelService.GetCountAsync(ParcelStatus.Received, cancellationToken);
        int activeVisitors = await _visitorService.GetCountAsync(VisitorStatus.CheckedIn, cancellationToken);
        int pendingPayments = await _paymentService.GetCountAsync(PaymentStatus.Pending, cancellationToken);

        IReadOnlyList<DashboardActivityItemResponse> recentActivity = await GetRecentActivityAsync(cancellationToken);

        var analytics = await analyticsService.GetAsync(new DormFlow.Contracts.Operations.AnalyticsRequest(currentMonth, today), cancellationToken);
        bool canViewFinance = _currentUser.HasPermission(PermissionCodes.Reports.View);
        var trendAnalytics = canViewFinance ? await analyticsService.GetAsync(new DormFlow.Contracts.Operations.AnalyticsRequest(currentMonth.AddMonths(-(RevenueTrendMonths - 1)), today), cancellationToken) : null;
        return new DashboardSummaryResponse
        {
            RoomStatusCounts = BuildRoomStatusCounts(roomSummary),
            TotalRooms = roomSummary.TotalRooms,
            OccupancyRate = roomSummary.TotalRooms > 0
                ? Math.Round(roomSummary.Occupied * 100d / roomSummary.TotalRooms, 1)
                : null,
            ExpensesThisMonth = canViewFinance ? analytics.Expenses : null,
            NetOperatingCashThisMonth = canViewFinance ? analytics.NetOperatingCash : null,
            ExpiringContractsCount = analytics.ExpiringContracts,
            OutstandingInvoicesTotal = canViewFinance ? analytics.Outstanding : null,
            OutstandingInvoicesCount = canViewFinance ? analytics.OutstandingCount : null,
            RevenueThisMonth = canViewFinance ? analytics.Revenue : null,
            RevenueTrend = trendAnalytics?.MonthlyRevenue.Select(x => new RevenueTrendPointResponse { Label = x.Label, Amount = x.Amount }).ToArray(),
            PendingPaymentConfirmations = pendingPayments,
            OpenMaintenanceCount = openMaintenanceCount,
            MyAssignedMaintenance = myAssignedMaintenance,
            ExpiringReservationsCount = expiringReservationsCount,
            ParcelsAwaitingPickup = awaitingParcels,
            ActiveVisitorsCount = activeVisitors,
            RecentActivity = recentActivity,
        };
    }

    private async Task<int> GetOpenMaintenanceCountAsync(CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<MaintenanceStatus, int> counts =
            await _maintenanceService.GetStatusCountsAsync(cancellationToken);

        return counts.Values.Sum();
    }

    private async Task<IReadOnlyList<MaintenanceRequestResponse>> GetMyAssignedMaintenanceAsync(
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } technicianId)
        {
            return [];
        }

        PagedResult<MaintenanceRequestResponse> page = await _maintenanceService.GetPagedAsync(
            new MaintenanceRequestListRequest
            {
                AssignedTechnicianId = technicianId,
                PageSize = PaginationDefaults.MaxPageSize,
            },
            cancellationToken);

        return [.. page.Items
            .Where(request => request.Status != nameof(MaintenanceStatus.Completed)
                && request.Status != nameof(MaintenanceStatus.Cancelled))
            .OrderByDescending(request => request.CreatedAt)
            .Take(10)];
    }

    /// <remarks>ນັບຈາກຖານຂໍ້ມູນໂດຍກົງ ເພື່ອບໍ່ໃຫ້ຈຳນວນຖືກຈຳກັດດ້ວຍ page size</remarks>
    private async Task<int> GetExpiringReservationsCountAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        DateTimeOffset threshold = now.AddHours(ExpiringReservationWindowHours);

        return await _reservationService.GetPendingExpiringCountAsync(threshold, cancellationToken);
    }

    private async Task<IReadOnlyList<DashboardActivityItemResponse>> GetRecentActivityAsync(
        CancellationToken cancellationToken)
    {
        PagedResult<AuditLogResponse> page = await _auditLogQueryService.GetPagedAsync(
            new AuditLogListRequest { PageSize = RecentActivityCount }, cancellationToken);

        return [.. page.Items.Select(ToActivityItem)];
    }

    private static Dictionary<string, int> BuildRoomStatusCounts(RoomSummaryResponse summary) => new(StringComparer.Ordinal)
    {
        [nameof(RoomStatus.Available)] = summary.Available,
        [nameof(RoomStatus.Reserved)] = summary.Reserved,
        [nameof(RoomStatus.Occupied)] = summary.Occupied,
        [nameof(RoomStatus.Cleaning)] = summary.Cleaning,
        [nameof(RoomStatus.Maintenance)] = summary.Maintenance,
        [nameof(RoomStatus.Inactive)] = summary.Inactive,
    };

    private static DashboardActivityItemResponse ToActivityItem(AuditLogResponse log) => new()
    {
        Id = log.Id.ToString(),
        Message = $"{TranslateAction(log.Action)}{TranslateEntity(log.EntityName)}",
        OccurredAt = log.CreatedAt,
    };

    /// <summary>AuditLog ບໍ່ມີຊ່ອງຂໍ້ຄວາມທີ່ກຽມພ້ອມ. ເພາະສະນັ້ນຈຶ່ງປະກອບຂໍ້ຄວາມທີ່ງ່າຍດາຍຈາກ Action + EntityName ຕົນເອງ</summary>
    private static string TranslateAction(string action) => action switch
    {
        nameof(AuditAction.Create) => "ສ້າງ ",
        nameof(AuditAction.Update) => "ແກ້ໄຂ ",
        nameof(AuditAction.Delete) => "ລຶບ ",
        nameof(AuditAction.ChangeStatus) => "ປ່ຽນສະຖານະ ",
        nameof(AuditAction.AssignRole) => "ກຳນົດບົດບາດໃຫ້ ",
        nameof(AuditAction.AssignDormitory) => "ມອບໝາຍຫໍພັກໃຫ້ ",
        nameof(AuditAction.Login) => "ເຂົ້າສູ່ລະບົບ ",
        nameof(AuditAction.Logout) => "ອອກຈາກລະບົບ ",
        _ => action + " ",
    };

    private static string TranslateEntity(string entityName) => entityName switch
    {
        "Organization" => "ອົງກອນ",
        "Dormitory" => "ຫໍພັກ",
        "Building" => "ອາຄານ",
        "Floor" => "ຊັ້ນ",
        "RoomType" => "ປະເພດຫ້ອງ",
        "Room" => "ຫ້ອງ",
        "Tenant" => "ຜູ້ເຊົ່າ",
        "Reservation" => "ການຈອງ",
        "Contract" => "ສັນຍາເຊົ່າ",
        "MeterReading" => "ຄ່າມິເຕີ",
        "Invoice" => "ໃບແຈ້ງໜີ້",
        "Payment" => "ການຊຳລະ",
        "Deposit" => "ເງິນມັດຈຳ",
        "Receipt" => "ໃບຮັບເງິນ",
        "Expense" => "ຄ່າໃຊ້ຈ່າຍ",
        "Parcel" => "ພັດສະດຸ",
        "Visitor" => "ຜູ້ມາຢ້ຽມຢາມ",
        "ParkingRegistration" => "ການລົງທະບຽນບ່ອນຈອດລົດ",
        "MaintenanceRequest" => "ງານສ້ອມແປງ",
        "Announcement" => "ປະກາດ",
        "Notification" => "ການແຈ້ງເຕືອນ",
        "Authentication" => "ການຢັ້ງຢືນຕົວຕົນ",
        "User" => "ຜູ້ໃຊ້",
        "Role" => "ບົດບາດ",
        "Setting" => "ການຕັ້ງຄ່າ",
        _ => entityName,
    };
}
