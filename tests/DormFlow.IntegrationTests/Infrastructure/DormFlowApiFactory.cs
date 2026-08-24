using DormFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace DormFlow.IntegrationTests.Infrastructure;

/// <summary>
/// สร้าง Test Host ของ DormFlow API พร้อม PostgreSQL จริงที่รันในคอนเทนเนอร์
/// </summary>
/// <remarks>
/// ใช้ PostgreSQL จริงผ่าน Testcontainers แทน InMemory Database
/// เพราะพฤติกรรมสำคัญหลายอย่างของระบบ เช่น Global Query Filter, timestamptz,
/// jsonb และ Optimistic Concurrency ด้วยคอลัมน์ xmin ไม่มีอยู่ใน InMemory Provider
/// </remarks>
public sealed class DormFlowApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("dormflow_test")
        .WithUsername("dormflow")
        .WithPassword("dormflow")
        .WithCleanUp(true)
        .Build();

    /// <summary>Connection String ของฐานข้อมูลที่ใช้ในเทสต์</summary>
    public string ConnectionString => _postgres.GetConnectionString();

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        // การเข้าถึง Services เป็นจุดที่ Test Host ถูกสร้างขึ้นจริง
        // จึงต้องสตาร์ตคอนเทนเนอร์ให้เสร็จก่อนบรรทัดนี้เสมอ
        using IServiceScope scope = Services.CreateScope();
        DormFlowDbContext dbContext = scope.ServiceProvider.GetRequiredService<DormFlowDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Program.cs ข้ามการ Seed อัตโนมัติในสภาพแวดล้อม Testing
        // จึงต้อง Seed สิทธิ์และบทบาทที่นี่เพื่อให้เทสต์การเข้าสู่ระบบมีข้อมูลให้ทดสอบ
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

        // ต้องใช้ UseSetting ไม่ใช่ ConfigureAppConfiguration
        // เพราะ Program.cs อ่านค่า configuration ตั้งแต่ตอนลงทะเบียนบริการ
        // ซึ่งเกิดก่อนที่ callback ของ ConfigureAppConfiguration จะถูกเรียก
        builder.UseSetting("ConnectionStrings:DefaultConnection", _postgres.GetConnectionString());

        // TestServer ไม่มีพอร์ต HTTPS จริง จึงต้องปิดการ redirect ไม่งั้นทุกคำขอจะได้ 307
        builder.UseSetting("Https:EnableRedirection", "false");

        // ปิด Rate Limiting เพื่อไม่ให้เทสต์ที่ยิงหลายคำขอชนโควตาแล้วล้มแบบสุ่ม
        builder.UseSetting("RateLimiting:Enabled", "false");

        builder.UseSetting("Cors:AllowedOrigins:0", "http://localhost");

        // กุญแจสำหรับเทสต์เท่านั้น ไม่ใช่ความลับจริง ต้องยาวอย่างน้อย 32 ตัวอักษรตาม JwtOptions
        builder.UseSetting("Jwt:SigningKey", "dormflow-integration-test-signing-key-not-a-real-secret");
    }
}
