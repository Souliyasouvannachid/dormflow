using System.Reflection;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Features.Announcements;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Application.Features.Authentication;
using DormFlow.Application.Features.Buildings;
using DormFlow.Application.Features.Contracts;
using DormFlow.Application.Features.Dashboard;
using DormFlow.Application.Features.Deposits;
using DormFlow.Application.Features.Expenses;
using DormFlow.Application.Features.Organizations;
using DormFlow.Application.Features.Parcels;
using DormFlow.Application.Features.Parking;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Application.Features.Floors;
using DormFlow.Application.Features.Invoices;
using DormFlow.Application.Features.Maintenance;
using DormFlow.Application.Features.MeterReadings;
using DormFlow.Application.Features.Notifications;
using DormFlow.Application.Features.Payments;
using DormFlow.Application.Features.Permissions;
using DormFlow.Application.Features.Receipts;
using DormFlow.Application.Features.Reports;
using DormFlow.Application.Features.Roles;
using DormFlow.Application.Features.Reservations;
using DormFlow.Application.Features.Settings;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Application.Features.Visitors;
using DormFlow.Application.Features.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DormFlow.Application;

/// <summary>
/// ຈຸດລົງທະບຽນ Dependency Injection ຂອງຊັ້ນ Application
/// </summary>
public static class DependencyInjection
{
    /// <summary>Assembly ຂອງຊັ້ນ Application ໃຊ້ເພື່ອສະແກນ Validator ແລະໃຊ້ໃນ Architecture Test</summary>
    public static Assembly ApplicationAssembly => typeof(DependencyInjection).Assembly;

    /// <summary>
    /// ລົງທະບຽນບໍລິການຊັ້ນທັງຝົດ Application
    /// </summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // ສະແກນຫາ Validator ທັງຝົດໃນ Assembly ນີ້ Use Case ອັນໃໝ່ທີ່ເພີ່ມໃນພາຍຫຼັງຈະຖືກລົງທະບຽນໂດຍອັດຕະໂນມັດ
        services.AddValidatorsFromAssembly(ApplicationAssembly, includeInternalTypes: true);

        // ໃຊ້ຊື່ Property ດັ່ງທີ່ປະກາດຕົວຈິງແລ້ວປ່ຽນເປັນ camelCase ເມື່ອມັນຖືກສ້າງຂຶ້ນ Response
        ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name;

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IDormitoryService, DormitoryService>();
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<IFloorService, FloorService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IMeterReadingService, MeterReadingService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IDepositService, DepositService>();
        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IParcelService, ParcelService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<IParkingRegistrationService, ParkingRegistrationService>();
        services.AddScoped<IMaintenanceRequestService, MaintenanceRequestService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuditLogQueryService, AuditLogQueryService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
