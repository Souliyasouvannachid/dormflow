using DormFlow.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// เตรียมฐานข้อมูลให้พร้อมใช้งานตอนแอปพลิเคชันเริ่มทำงาน
/// </summary>
/// <remarks>
/// การรัน Migration อัตโนมัติถูกปิดไว้เป็นค่าเริ่มต้น
/// เพราะบน Production ควรรัน Migration เป็นขั้นตอนแยกก่อน deploy
/// เพื่อไม่ให้หลายอินสแตนซ์แย่งกันแก้สคีมาพร้อมกัน
/// </remarks>
public sealed class DatabaseInitializer(
    DormFlowDbContext dbContext,
    DatabaseSeeder seeder,
    IOptions<DatabaseOptions> databaseOptions,
    ILogger<DatabaseInitializer> logger)
{
    private static readonly Action<ILogger, Exception?> MigratingCallback =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(4100, "ApplyingMigrations"),
            "กำลังปรับสคีมาฐานข้อมูลให้ตรงกับ Migration ล่าสุด");

    private static readonly Action<ILogger, Exception?> PendingMigrationsCallback =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(4101, "PendingMigrations"),
            "ฐานข้อมูลมี Migration ที่ยังไม่ได้รัน แต่ Database:AutoMigrate ถูกปิดไว้ กรุณารัน dotnet ef database update ก่อนใช้งาน");

    private readonly DormFlowDbContext _dbContext = dbContext;
    private readonly DatabaseSeeder _seeder = seeder;
    private readonly DatabaseOptions _databaseOptions = databaseOptions.Value;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    /// <summary>ปรับสคีมาและสร้างข้อมูลตั้งต้นตามค่าที่ตั้งไว้</summary>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_databaseOptions.AutoMigrate)
        {
            MigratingCallback(_logger, null);
            await _dbContext.Database.MigrateAsync(cancellationToken);
        }
        else if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            PendingMigrationsCallback(_logger, null);
        }

        if (_databaseOptions.SeedOnStartup)
        {
            await _seeder.SeedAsync(cancellationToken);
        }
    }
}
