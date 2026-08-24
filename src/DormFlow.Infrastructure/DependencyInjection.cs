using System.Reflection;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Application.Features.Authentication.Validators;
using DormFlow.Application.Features.Buildings;
using DormFlow.Application.Features.Contracts;
using DormFlow.Application.Features.Organizations;
using DormFlow.Application.Features.Dormitories;
using DormFlow.Application.Features.Floors;
using DormFlow.Application.Features.Permissions;
using DormFlow.Application.Features.Roles;
using DormFlow.Application.Features.Reservations;
using DormFlow.Application.Features.RoomTypes;
using DormFlow.Application.Features.Rooms;
using DormFlow.Application.Features.Tenants;
using DormFlow.Application.Features.Users;
using DormFlow.Infrastructure.Authentication;
using DormFlow.Infrastructure.Identity;
using DormFlow.Infrastructure.Options;
using DormFlow.Infrastructure.Persistence;
using DormFlow.Infrastructure.Persistence.Interceptors;
using DormFlow.Infrastructure.Persistence.Repositories;
using DormFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DormFlow.Infrastructure;

/// <summary>
/// จุดลงทะเบียน Dependency Injection ของชั้น Infrastructure
/// </summary>
public static class DependencyInjection
{
    /// <summary>ชื่อ Connection String หลักของระบบ</summary>
    public const string DefaultConnectionName = "DefaultConnection";

    /// <summary>Assembly ของชั้น Infrastructure ใช้ใน Architecture Test</summary>
    public static Assembly InfrastructureAssembly => typeof(DependencyInjection).Assembly;

    /// <summary>
    /// ลงทะเบียนบริการทั้งหมดของชั้น Infrastructure
    /// </summary>
    /// <param name="services">Service collection ของแอปพลิเคชัน</param>
    /// <param name="configuration">Configuration ของแอปพลิเคชัน</param>
    /// <exception cref="InvalidOperationException">
    /// เมื่อไม่พบ Connection String ชื่อ DefaultConnection
    /// ระบบเลือกที่จะหยุดทำงานทันทีแทนที่จะไปพังตอนมีคำขอเข้ามาแล้ว
    /// </exception>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddPersistence(configuration);
        services.AddDormFlowIdentity();
        services.AddTokenServices(configuration);
        services.AddSupportingServices(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = ResolveConnectionString(configuration);

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        DatabaseOptions databaseOptions = new();
        configuration.GetSection(DatabaseOptions.SectionName).Bind(databaseOptions);

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddDbContext<DormFlowDbContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder
                .UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(DormFlowDbContext.InfrastructureAssembly.FullName);
                    npgsql.MigrationsHistoryTable(DormFlowDbContext.MigrationsHistoryTableName);
                    npgsql.CommandTimeout(databaseOptions.CommandTimeoutSeconds);
                    npgsql.EnableRetryOnFailure(
                        databaseOptions.MaxRetryCount,
                        TimeSpan.FromSeconds(databaseOptions.MaxRetryDelaySeconds),
                        errorCodesToAdd: null);
                })
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

            // ทั้งสองค่านี้เปิดได้เฉพาะตอนพัฒนา เพราะทำให้ค่าพารามิเตอร์จริงปรากฏใน Log
            optionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
            optionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
        });

        services.AddScoped<IAuditLogWriter, AuditLogWriter>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IDormitoryRepository, DormitoryRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<IFloorRepository, FloorRepository>();
        services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }

    /// <summary>
    /// ลงทะเบียน ASP.NET Core Identity
    /// </summary>
    /// <remarks>
    /// ใช้ AddIdentityCore แทน AddIdentity เพราะระบบยืนยันตัวตนด้วย JWT ไม่ใช้ cookie
    /// AddIdentity จะลงทะเบียน authentication scheme แบบ cookie มาให้ด้วยซึ่งไม่จำเป็นและทำให้สับสน
    /// </remarks>
    private static IServiceCollection AddDormFlowIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                // ข้อกำหนดรหัสผ่านต้องตรงกับ PasswordRules ในชั้น Application
                options.Password.RequiredLength = PasswordRules.MinimumLength;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 4;

                // ล็อกบัญชี 15 นาทีเมื่อกรอกรหัสผ่านผิดครบ 5 ครั้ง
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<DormFlowDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IUserAdministrationRepository, UserAdministrationRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }

    private static IServiceCollection AddTokenServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        return services;
    }

    private static IServiceCollection AddSupportingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // ข้อมูลผู้ดูแลระบบเริ่มต้นอ่านจาก environment variable ตรง ๆ ตามชื่อที่กำหนดไว้ในเอกสารระบบ
        services.Configure<SeedOptions>(options =>
        {
            options.AdminEmail = configuration[SeedOptions.AdminEmailVariable];
            options.AdminPassword = configuration[SeedOptions.AdminPasswordVariable];
            options.AdminFirstName = configuration[SeedOptions.AdminFirstNameVariable];
            options.AdminLastName = configuration[SeedOptions.AdminLastNameVariable];
        });

        services.AddSingleton<IEmailSender, PickupDirectoryEmailSender>();

        return services;
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString(DefaultConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"ไม่พบ Connection String ชื่อ '{DefaultConnectionName}' " +
                "กำหนดค่าผ่าน environment variable ชื่อ ConnectionStrings__DefaultConnection " +
                "หรือใน appsettings.Development.json สำหรับการพัฒนาบนเครื่อง");
        }

        return connectionString;
    }
}
