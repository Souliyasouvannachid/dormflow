using System.Reflection;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Application.Features.Authentication;
using DormFlow.Application.Features.Buildings;
using DormFlow.Application.Features.Contracts;
using DormFlow.Application.Features.Deposits;
using DormFlow.Application.Features.Organizations;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Application.Features.Floors;
using DormFlow.Application.Features.Invoices;
using DormFlow.Application.Features.Maintenance;
using DormFlow.Application.Features.MeterReadings;
using DormFlow.Application.Features.Payments;
using DormFlow.Application.Features.Permissions;
using DormFlow.Application.Features.Reports;
using DormFlow.Application.Features.Roles;
using DormFlow.Application.Features.Reservations;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Application.Features.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DormFlow.Application;

/// <summary>
/// จุดลงทะเบียน Dependency Injection ของชั้น Application
/// </summary>
public static class DependencyInjection
{
    /// <summary>Assembly ของชั้น Application ใช้สแกน Validator และใช้ใน Architecture Test</summary>
    public static Assembly ApplicationAssembly => typeof(DependencyInjection).Assembly;

    /// <summary>
    /// ลงทะเบียนบริการทั้งหมดของชั้น Application
    /// </summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // สแกนหา Validator ทั้งหมดใน Assembly นี้ Use Case ใหม่ที่เพิ่มภายหลังจะถูกลงทะเบียนอัตโนมัติ
        services.AddValidatorsFromAssembly(ApplicationAssembly, includeInternalTypes: true);

        // ใช้ชื่อ Property ตามที่ประกาศจริง แล้วค่อยแปลงเป็น camelCase ตอนสร้าง Response
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
        services.AddScoped<IMaintenanceRequestService, MaintenanceRequestService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuditLogQueryService, AuditLogQueryService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
