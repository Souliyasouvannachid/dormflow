using System.Net;
using DormFlow.Api.Configuration;
using DormFlow.Api.Middleware;
using DormFlow.Api.Options;
using DormFlow.Application;
using DormFlow.Infrastructure;
using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

// Logger ຊົ່ວຄາວເພື່ອຈັບຄວາມຜິດພາດທີ່ເກີດຂຶ້ນກ່ອນທີ່ຈະອ່ານ configuration ສຳເລັດແລ້ວ
SerilogConfiguration.CreateBootstrapLogger();

try
{
    Log.Information("ກຳລັງເລີ່ມຕົ້ນ DormFlow API");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.AddDormFlowSerilog();

    // ຊັ້ນ API
    builder.Services.AddDormFlowApi(builder.Configuration);
    builder.Services.AddDormFlowCors(builder.Configuration, builder.Environment);
    builder.Services.AddDormFlowRateLimiting(builder.Configuration);
    builder.Services.AddDormFlowOpenApi();

    // ຊັ້ນ Application ແລະ Infrastructure
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

    builder.Services.AddDormFlowHealthChecks();

    builder.Services.AddDormFlowAuthentication(builder.Configuration);

    builder.Services.AddDormFlowBackgroundJobs(builder.Configuration, builder.Environment);

    // ໄດ້ຮັບມູນຄ່າ IP ແລະ scheme ເປັນຈິງໃນເວລາທີ່ແລ່ນຫຼັງ reverse proxy ຫຼື ingress
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        // ຮັບ forwarded headers ຈາກ proxy ທີ່ລະບຸໄວ້ເທົ່ານັ້ນ. ຢ່າລ້າງ KnownProxies/Networks
        // ເພາະຈະເຮັດໃຫ້ client ປອມ X-Forwarded-For ໄດ້ ແລະຫຼົບ rate limit/audit IP.
        string[] configuredProxies = builder.Configuration
            .GetSection("ForwardedHeaders:KnownProxies")
            .Get<string[]>() ?? [];

        foreach (string proxy in configuredProxies.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            if (!IPAddress.TryParse(proxy, out IPAddress? address))
            {
                throw new InvalidOperationException(
                    $"ForwardedHeaders:KnownProxies ມີ IP address ບໍ່ຖືກຕ້ອງ: '{proxy}'.");
            }

            options.KnownProxies.Add(address);
        }
    });

    WebApplication app = builder.Build();

    app.UseForwardedHeaders();

    // ຢູ່ດ້ານນອກ GlobalExceptionHandling ເພື່ອໃຫ້ log ເຫັນ HTTP status ສຸດທ້າຍຫຼັງຈາກ exception ຖືກ map ແລ້ວ.
    app.UseDormFlowRequestLogging();
    // ຄຸ້ມຄອງ exception ຈາກ middleware ແລະ endpoint ທັງໝົດທີ່ຢູ່ດ້ານໃນ.
    app.UseGlobalExceptionHandling();

    // Docker builds place the React bundle here; hash routes share the API origin.
    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    // ປິດ redirect ໄປ HTTPS ໃນເວລາທີ່ແລ່ນຢູ່ໃນຖັງທີ່ມີ reverse proxy ຈັດການ TLS ມອບໃຫ້ແລ້ວ
    if (app.Configuration.GetValue("Https:EnableRedirection", defaultValue: true))
    {
        app.UseHttpsRedirection();
    }

    app.UseCors(CorsOptions.PolicyName);

    app.UseAuthentication();
    // ໃຫ້ rate limiter ເຫັນ user principal ກ່ອນ ເພື່ອແບ່ງ quota ຕາມ user ໄດ້ຖືກຕ້ອງ.
    app.UseRateLimiter();
    app.UseAuthorization();

    app.MapControllers();
    app.MapDormFlowHealthChecks();
    app.UseDormFlowOpenApi();
    app.UseDormFlowBackgroundJobs();

    // ຂ້າມການປັບຕົວ schema ແລະ Seed ອັດຕະໂນມັດໃນສະພາບແວດລ້ອມ Testing
    // ເນື່ອງຈາກວ່າ Integration Test ກະກຽມຖານຂໍ້ມູນຂອງທ່ານເອງໂດຍຜ່ານ DormFlowApiFactory ແລ້ວ
    if (!app.Environment.IsEnvironment("Testing"))
    {
        await using AsyncServiceScope initializationScope = app.Services.CreateAsyncScope();
        DatabaseInitializer initializer = initializationScope.ServiceProvider
            .GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    Log.Information(
        "DormFlow API ພ້ອມເຮັດວຽກໃນສະພາບແວດລ້ອມ {Environment}",
        app.Environment.EnvironmentName);

    await app.RunAsync();

    return 0;
}
catch (Exception exception)
{
    Log.Fatal(exception, "DormFlow API ຢຸດເຮັດວຽກເພາະຂໍ້ຜິດພາດທີ່ບໍ່ຄາດຄິດລະຫວ່າງເລີ່ມຕົ້ນລະບົບ");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

/// <summary>
/// ຈຸດເລີ່ມຕົ້ນຂອງຄໍາຮ້ອງສະຝັກໄດ້ຖືກປະກາດເປັນ partial ດັ່ງນັ້ນ Integration Test ອ້າງເຖິງມັນໂດຍຜ່ານ WebApplicationFactory
/// </summary>
public partial class Program
{
    /// <summary>ປ້ອງກັນບໍ່ໃຫ້ເກີດທັນທີ ຫ້ອງຮຽນນີ້ແມ່ນມີຈຸດປະສົງເປັນຈຸດອ້າງອີງ Test Host ເທົ່ານັ້ນ</summary>
    protected Program()
    {
    }
}
