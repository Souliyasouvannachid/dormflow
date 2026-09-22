using DormFlow.Api.Authorization;
using DormFlow.Application.Features.Invoices;
using DormFlow.Application.Features.Reservations;
using DormFlow.Infrastructure.Options;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DormFlow.Api.Configuration;

/// <summary>ຕັ້ງຄ່າ Background Job Server ແລະ Recurring Job ທັງໝົດ</summary>
public static class BackgroundJobConfiguration
{
    /// <summary>ລະຫັດຂອງ Recurring Job ທີ່ໝົດອາຍຸອັດຕະໂນມັດ</summary>
    private const string ExpireReservationsJobId = "expire-reservations";

    /// <summary>ລະຫັດຂອງ Recurring Job ທີ່ໝາຍວ່າໃບເກັບເງິນເກີນກຳນົດ ແລະຄຳນວນການລົງໂທດ</summary>
    private const string MarkOverdueInvoicesJobId = "mark-overdue-invoices";

    /// <summary>ທຸກໆ 15 ນາທີເພາະໝົດເວລາຈອງຫ້ອງ (ExpiresAt) ມີຄວາມລະອຽດນາທີ</summary>
    private const string ExpireReservationsCron = "*/15 * * * *";

    /// <summary>
    /// ລົງທະບຽນ Background Job Server
    /// </summary>
    /// <remarks>
    /// ຂ້າມການລົງທະບຽນໃນສະພາບແວດລ້ອມ Testing ຫຼືເມື່ອປິດຜ່ານການຕັ້ງຄ່າ BackgroundJobs:Enabled
    /// ດ້ວຍເງື່ອນໄຂດຽວກັນກັບ Infrastructure.AddBackgroundJobStorage ໃຊ້ການລົງທະບຽນ bypass Storage
    /// (ພວກເຂົາຕ້ອງຂ້າມກັນແລະກັນ, ຖ້າບໍ່ດັ່ງນັ້ນ Server ຈະພະຍາຍາມ resolve JobStorage ທີ່ບໍ່ໄດ້ລົງທະບຽນ)
    /// </remarks>
    /// <param name="services">Service collection ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="configuration">Configuration ຂອງຄໍາຮ້ອງສະຝັກ</param>
    /// <param name="environment">ສະພາບແວດລ້ອມເຈົ້າພາບແອັບພລິເຄຊັນ</param>
    public static IServiceCollection AddDormFlowBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        if (environment.IsEnvironment("Testing")
            || !configuration.GetValue("BackgroundJobs:Enabled", defaultValue: true))
        {
            return services;
        }

        services.AddHangfireServer();

        return services;
    }

    /// <summary>
    /// ຫັນຜ້າ Dashboard ແລະກໍານົດເວລາ Recurring Job ທັງໝົດ
    /// </summary>
    /// <remarks>
    /// ໃນທົ່ວສະພາບແວດລ້ອມ Testing (Integration Test ກະກຽມຖານຂໍ້ມູນຊົ່ວຄາວຂອງຂ້ອຍເອງແລ້ວ
    /// ບໍ່ຈໍາເປັນຕ້ອງມີ Hangfire Schema ຫຼື Worker ເຮັດວຽກຂະຜານລະຫວ່າງການທົດສອບ) ແລະເມື່ອປິດຜ່ານການຕັ້ງຄ່າ
    /// BackgroundJobs:Enabled ຕົນເອງ — Dashboard ສະພາບແວດລ້ອມເປີດເທົ່ານັ້ນທີ່ບໍ່ມີ Production ຄືກັນກັບ OpenAPI/Scalar
    /// ເພື່ອເປີດໃນ Production ປົກປ້ອງຕົນເອງສະເໝີໂດຍການຢັ້ງຢືນຕົວຕົນຂອງທ່ານກ່ອນ
    /// </remarks>
    /// <param name="app">ແອັບພລິເຄຊັນທີ່ສ້າງຂຶ້ນ</param>
    public static WebApplication UseDormFlowBackgroundJobs(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsEnvironment("Testing"))
        {
            return app;
        }

        BackgroundJobOptions options = app.Services.GetRequiredService<IOptions<BackgroundJobOptions>>().Value;
        if (!options.Enabled)
        {
            return app;
        }

        if (!app.Environment.IsProduction())
        {
            app.MapHangfireDashboard(new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthorizationFilter()],
            });
        }

        // Hangfire ໄດ້ລົງທະບຽນຜ່ານ `AddHangfire(...)` ທີ່ໃຊ້ DI (ເບິ່ງ AddBackgroundJobStorage)
        // ເຊິ່ງບໍ່ໄດ້ກຳນົດ singleton `JobStorage.Current` ແບບ static — ດັ່ງນັ້ນ recurring jobs ຕ້ອງ
        // ກຳນົດຜ່ານ `IRecurringJobManager` ທີ່ inject ເຂົ້າມາ ບໍ່ແມ່ນ helper `RecurringJob` ແບບ static.
        IRecurringJobManager recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<IReservationService>(
            ExpireReservationsJobId,
            service => service.ExpirePastDueReservationsAsync(CancellationToken.None),
            ExpireReservationsCron);

        recurringJobManager.AddOrUpdate<IInvoiceService>(
            MarkOverdueInvoicesJobId,
            service => service.MarkOverdueInvoicesAsync(CancellationToken.None),
            Cron.Daily(1));

        return app;
    }
}
