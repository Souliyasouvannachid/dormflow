using System.Reflection;
using DormFlow.Application.Common.Abstractions;
using Hangfire;
using Hangfire.PostgreSql;
using DormFlow.Application.Features.Announcements;
using DormFlow.Application.Features.AuditLogs;
using DormFlow.Application.Features.Authentication.Validators;
using DormFlow.Application.Features.Buildings;
using DormFlow.Application.Features.Contracts;
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
using Microsoft.Extensions.Hosting;

namespace DormFlow.Infrastructure;

/// <summary>
/// ຈຸດລົງທະບຽນ Dependency Injection ຂອງຊັ້ນ Infrastructure
/// </summary>
public static class DependencyInjection
{
    /// <summary>ຊື່ Connection String ຫຼັກຂອງລະບົບ</summary>
    public const string DefaultConnectionName = "DefaultConnection";

    /// <summary>Assembly ຂອງຊັ້ນ Infrastructure ໃຊ້ໃນ Architecture Test</summary>
    public static Assembly InfrastructureAssembly => typeof(DependencyInjection).Assembly;

    /// <summary>
    /// ລົງທະບຽນບໍລິການຊັ້ນທັງຝົດ Infrastructure
    /// </summary>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="configuration">Configuration ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <exception cref="InvalidOperationException">
    /// ເມື່ອບໍ່ພົບ Connection String ຊື່ DefaultConnection
    /// ລະບົບເລືອກທີ່ຈະຢຸດການເຮັດວຽກທັນທີແທນທີ່ຈະ crashing ເມື່ອໄດ້ຮັບການຮ້ອງຂໍ
    /// </exception>
    /// <param name="environment">ສະພາບແວດລ້ອມໂຮດແອັບພລິເຄຊັນຖືກໃຊ້ເພື່ອຂ້າມຜ່ານ Background Job ໃນສະພາບແວດລ້ອມ Testing</param>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddPersistence(configuration);
        services.AddDormFlowIdentity();
        services.AddTokenServices(configuration);
        services.AddSupportingServices(configuration, environment);
        services.AddBackgroundJobStorage(configuration, environment);

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

            // ທັງສອງການຕັ້ງຄ່າເຫຼົ່ານີ້ສາມາດເປີດໄດ້ພຽງແຕ່ໃນລະຫວ່າງການພັດທະນາ. ເນື່ອງຈາກວ່າຄ່າພາລາມິເຕີທີ່ແທ້ຈິງປາກົດຢູ່ໃນ Log
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
        services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IDepositRepository, DepositRepository>();
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IParcelRepository, ParcelRepository>();
        services.AddScoped<IVisitorRepository, VisitorRepository>();
        services.AddScoped<IParkingRegistrationRepository, ParkingRegistrationRepository>();
        services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }

    /// <summary>
    /// ລົງທະບຽນ ASP.NET Core Identity
    /// </summary>
    /// <remarks>
    /// ໃຊ້ AddIdentityCore ແທນ AddIdentity ເນື່ອງຈາກວ່າລະບົບການຢັ້ງຢືນຕົວຕົນເຊັ່ນດຽວກັນ JWT ຢ່າໃຊ້ cookie
    /// AddIdentity ຈະລົງທະບຽນ authentication scheme ຕົວແບບ cookie ສະຜອງໃຫ້, ທີ່ບໍ່ຈໍາເປັນແລະສັບສົນ
    /// </remarks>
    private static IServiceCollection AddDormFlowIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                // ຄວາມຕ້ອງການລະຫັດຜ່ານຕ້ອງກົງກັນ PasswordRules ໃນຫ້ອງຮຽນ Application
                options.Password.RequiredLength = PasswordRules.MinimumLength;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 4;

                // ບັນຊີຖືກລັອກ 15 ນາທີຫຼັງຈາກໃສ່ລະຫັດຜ່ານຜິດ 5 ເທື່ອ
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
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => !string.Equals(options.Provider, "Smtp", StringComparison.OrdinalIgnoreCase)
                    || !string.IsNullOrWhiteSpace(options.SmtpHost),
                "ຕ້ອງກຳນົດ Email:SmtpHost ເມື່ອ Email:Provider ເປັນ Smtp.")
            .ValidateOnStart();

        // ຂໍ້ມູນເລີ່ມຕົ້ນຂອງຜູ້ເບິ່ງແຍງລະບົບອ່ານຈາກ environment variable ກົງກັບຊື່ທີ່ລະບຸໄວ້ໃນເອກະສານລະບົບ
        services.Configure<SeedOptions>(options =>
        {
            options.AdminEmail = configuration[SeedOptions.AdminEmailVariable];
            options.AdminPassword = configuration[SeedOptions.AdminPasswordVariable];
            options.AdminFirstName = configuration[SeedOptions.AdminFirstNameVariable];
            options.AdminLastName = configuration[SeedOptions.AdminLastNameVariable];
        });

        string emailProvider = configuration["Email:Provider"] ?? "PickupDirectory";
        if (environment.IsProduction()
            && string.Equals(emailProvider, "PickupDirectory", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Email:Provider ບໍ່ສາມາດເປັນ PickupDirectory ໃນ Production ໄດ້. ກະລຸນາຕັ້ງຄ່າ SMTP ແທນ.");
        }

        if (string.Equals(emailProvider, "Smtp", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
        }
        else if (string.Equals(emailProvider, "PickupDirectory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IEmailSender, PickupDirectoryEmailSender>();
        }
        else
        {
            throw new InvalidOperationException(
                $"ບໍ່ຮອງຮັບ Email:Provider '{emailProvider}'. ກະລຸນາໃຊ້ 'Smtp' ຫຼື 'PickupDirectory'.");
        }

        return services;
    }

    /// <summary>
    /// ລົງທະບຽນບ່ອນເກັບຂໍ້ມູນຂອງທ່ານ Background Job (Hangfire) ສຸດ PostgreSQL ຄືກັນກັບຖານຂໍ້ມູນຫຼັກ
    /// </summary>
    /// <remarks>
    /// ລົງທະບຽນເທົ່ານັ້ນ Storage ເທົ່ານັ້ນ, ບໍ່ລວມ Server/Dashboard/ກໍານົດເວລາ Recurring Job
    /// ຊຶ່ງເປັນເລື່ອງຂອງ Runtime/Hosting ແລະຢູ່ເທິງພື້ນ Api ແທນ (ສັງເກດເບິ່ງ BackgroundJobConfiguration)
    /// ແຍກ "ລົງທະບຽນສໍາລັບການບໍລິການ" ກັບ "ຄໍາສັ່ງທີ່ຈະເຮັດວຽກຕົວຈິງ" ແຍກອອກຈາກກັນຢ່າງຈະແຈ້ງ, ເປັນ AddPersistence/DatabaseInitializer
    /// ຂ້າມການລົງທະບຽນທັງໝົດໃນສະພາບແວດລ້ອມ Testing ຫຼືເມື່ອປິດຜ່ານການຕັ້ງຄ່າ BackgroundJobs:Enabled ເນື່ອງຈາກວ່າ
    /// Hangfire.PostgreSql ຕິດຕັ້ງ Schema ເປັນເຈົ້າຂອງທັນທີເມື່ອໂທຫາ UsePostgreSqlStorage (ບໍ່ແມ່ນໃນຕອນເລີ່ມຕົ້ນ Server)
    /// ຖ້າອະນຸຍາດໃຫ້ເຮັດວຽກຂະຜານກັບ dbContext.Database.EnsureCreatedAsync() ຂອງ Integration Test
    /// EF Core ມັນເຮັດໃຫ້ຄວາມຜິດພາດຂອງການສົມມຸດວ່າຖານຂໍ້ມູນມີຕາຕະລາງແລ້ວແລະຂ້າມການສ້າງຕາຕະລາງຂອງແອັບພລິເຄຊັນທັງຝົດ (ພົບແມງໄມ້ທີ່ແທ້ຈິງໃນລະຫວ່າງການພັດທະນາ)
    /// </remarks>
    private static IServiceCollection AddBackgroundJobStorage(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddOptions<BackgroundJobOptions>()
            .Bind(configuration.GetSection(BackgroundJobOptions.SectionName))
            .ValidateOnStart();

        if (environment.IsEnvironment("Testing")
            || !configuration.GetValue("BackgroundJobs:Enabled", defaultValue: true))
        {
            return services;
        }

        string connectionString = ResolveConnectionString(configuration);

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        return services;
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString(DefaultConnectionName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"ບໍ່ພົບ Connection String ຊື່ '{DefaultConnectionName}' " +
                "ກຳນົດຄ່າຜ່ານ environment variable ຊື່ ConnectionStrings__DefaultConnection " +
                "ຫຼືໃນ appsettings.Development.json ສຳລັບການພັດທະນາເທິງເຄື່ອງ");
        }

        return connectionString;
    }
}
