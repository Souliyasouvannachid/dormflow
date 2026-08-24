using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// ตัวสร้าง DbContext สำหรับเครื่องมือ dotnet ef ตอน design time
/// </summary>
/// <remarks>
/// ใช้เฉพาะตอนสร้างและรัน Migration เท่านั้น ไม่ถูกเรียกตอนแอปทำงานจริง
/// อ่าน Connection String จาก environment variable เพื่อไม่ต้องผูกกับไฟล์ appsettings ของโปรเจกต์ API
/// </remarks>
public sealed class DormFlowDbContextFactory : IDesignTimeDbContextFactory<DormFlowDbContext>
{
    /// <summary>ชื่อ environment variable ที่ใช้กำหนด Connection String ตอน design time</summary>
    public const string ConnectionStringVariable = "ConnectionStrings__DefaultConnection";

    /// <summary>Connection String สำรองที่ชี้ไปยัง PostgreSQL ของ docker compose บนเครื่องนักพัฒนา</summary>
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

                // ต้องตรงกับชื่อตารางที่ DependencyInjection.AddPersistence ใช้ตอนแอปทำงานจริงเสมอ
                npgsql.MigrationsHistoryTable(DormFlowDbContext.MigrationsHistoryTableName);
            })
            .UseSnakeCaseNamingConvention()
            .Options;

        return new DormFlowDbContext(options);
    }
}
