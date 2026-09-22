using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// ຜູ້ກໍ່ສ້າງ DbContext ສໍາລັບເຄື່ອງມື dotnet ef ຕອນ design time
/// </summary>
/// <remarks>
/// ໃຊ້ພຽງແຕ່ໃນເວລາທີ່ການກໍ່ສ້າງແລະແລ່ນ Migration ພຽງແຕ່, ມັນບໍ່ໄດ້ຖືກເອີ້ນວ່າໃນເວລາທີ່ app ກໍາລັງເຮັດວຽກຕົວຈິງ
/// ອ່ານ Connection String ຈາກ environment variable ເພື່ອບໍ່ຈໍາເປັນຕ້ອງຜູກມັດກັບໄຟລ໌ appsettings ຂອງໂຄງການ API
/// </remarks>
public sealed class DormFlowDbContextFactory : IDesignTimeDbContextFactory<DormFlowDbContext>
{
    /// <summary>ຊື່ environment variable ໃຊ້ເພື່ອກໍານົດ Connection String ຕອນ design time</summary>
    public const string ConnectionStringVariable = "ConnectionStrings__DefaultConnection";

    /// <summary>Connection String ສະຫຼັບຊີ້ໄປ PostgreSQL ຂອງ docker compose ໃນເຄື່ອງພັດທະນາ</summary>
    private const string LocalDevelopmentConnectionString =
        "Host=localhost;Port=5432;Database=dormflow;Username=dormflow;Password=dormflow;Include Error Detail=true";

    /// <inheritdoc />
    public DormFlowDbContext CreateDbContext(string[] args)
    {
        string? connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = LocalDevelopmentConnectionString;
        }

        DbContextOptions<DormFlowDbContext> options = new DbContextOptionsBuilder<DormFlowDbContext>()
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(DormFlowDbContext.InfrastructureAssembly.FullName);

                // ຕ້ອງກົງກັບຊື່ຕາຕະລາງ DependencyInjection.AddPersistence ໃຊ້ມັນສະເໝີເມື່ອແອັບກຳລັງເຮັດວຽກຢູ່
                npgsql.MigrationsHistoryTable(DormFlowDbContext.MigrationsHistoryTableName);
            })
            .UseSnakeCaseNamingConvention()
            .Options;

        return new DormFlowDbContext(options);
    }
}
