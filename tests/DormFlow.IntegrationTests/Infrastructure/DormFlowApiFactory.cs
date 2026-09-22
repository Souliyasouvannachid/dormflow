using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace DormFlow.IntegrationTests.Infrastructure;

/// <summary>
/// ສ້າງ Test Host ຂອງ DormFlow API ພ້ອມ PostgreSQL ຕົວຈິງແລ້ວແລ່ນຢູ່ໃນຖັງ
/// </summary>
/// <remarks>
/// ໃຊ້ PostgreSQL ຜ່ານຄວາມຈິງ Testcontainers ແທນ InMemory Database
/// ເນື່ອງຈາກວ່າພຶດຕິກໍາທີ່ສໍາຄັນຈໍານວນຫຼາຍຂອງລະບົບເຊັ່ນ: Global Query Filter, timestamptz,
/// jsonb ແລະ Optimistic Concurrency ກັບຖັນ xmin ບໍ່ມີຢູ່ໃນ InMemory Provider
/// </remarks>
public sealed class DormFlowApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("dormflow_test")
        .WithUsername("dormflow")
        .WithPassword("dormflow")
        .WithCleanUp(true)
        .Build();

    /// <summary>Connection String ຂອງຖານຂໍ້ມູນທີ່ໃຊ້ໃນການທົດສອບ</summary>
    public string ConnectionString => _postgres.GetConnectionString();

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        // ການເຂົ້າເຖິງ Services ແມ່ນຈຸດ Test Host ຕົວຈິງແລ້ວໄດ້ຖືກສ້າງຂຶ້ນ
        // ເພາະສະນັ້ນ, ຕູ້ຄອນເທນເນີຕ້ອງຖືກເລີ່ມຕົ້ນກ່ອນເສັ້ນນີ້
        using IServiceScope scope = Services.CreateScope();
        DormFlowDbContext dbContext = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Program.cs ຂ້າມ Seed ອັດຕະໂນມັດໃນສະພາບແວດລ້ອມ Testing
        // ສະນັ້ນຕ້ອງ Seed ການອະນຸຍາດ ແລະບົດບາດຢູ່ທີ່ນີ້ເພື່ອໃຫ້ຂໍ້ມູນການເຂົ້າສູ່ລະບົບທົດສອບ
        DatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Testing");

        // ຕ້ອງໃຊ້ UseSetting ບໍ່ ConfigureAppConfiguration
        // ເນື່ອງຈາກວ່າ Program.cs ອ່ານຄ່າ configuration ຈາກປັດຈຸບັນຂອງການລົງທະບຽນການບໍລິການ
        // ທີ່ເກີດຂຶ້ນກ່ອນ callback ຂອງ ConfigureAppConfiguration ຈະຖືກເອີ້ນ
        builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.GetConnectionString());

        // TestServer ບໍ່ມີພອດ HTTPS ແທ້ຈິງແລ້ວ, ສະນັ້ນຕ້ອງປິດ redirect ຖ້າບໍ່ດັ່ງນັ້ນ, ທຸກໆຄໍາຮ້ອງຂໍຈະຖືກອະນຸຍາດ 307
        builder.UseSetting("Https:EnableRedirection", "false");

        // ປິດ Rate Limiting ນີ້ແມ່ນເພື່ອປ້ອງກັນການທົດສອບທີ່ເຮັດໃຫ້ເກີດການຮ້ອງຂໍຫຼາຍຈາກການກົດດັນໂຄຕ້າຂອງພວກເຂົາແລະລົ້ມເຫຼວແບບສຸ່ມ
        builder.UseSetting("RateLimiting:Enabled", "false");

        builder.UseSetting("Cors:AllowedOrigins:0", "http://localhost");

        // ລະຫັດສໍາລັບການທົດສອບເທົ່ານັ້ນ ມັນບໍ່ແມ່ນຄວາມລັບແທ້ໆ. ຢ່າງໜ້ອຍຕ້ອງມີຄວາມຍາວ 32 ຕົວອັກສອນຕາມ JwtOptions
        builder.UseSetting("Jwt:SigningKey", "dormflow-integration-test-signing-key-not-a-real-secret");
    }
}
