using DormFlow.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Persistence;

/// <summary>
/// ກະກຽມຖານຂໍ້ມູນໃຫ້ພ້ອມສໍາລັບການນໍາໃຊ້ໃນເວລາທີ່ຄໍາຮ້ອງສະຝັກເລີ່ມຕົ້ນ
/// </summary>
/// <remarks>
/// ແລ່ນ Migration ອັດຕະໂນມັດຖືກປິດໄວ້ຕາມຄ່າເລີ່ມຕົ້ນ
/// ເນື່ອງຈາກວ່າ Production ຄວນດໍາເນີນການ Migration ມັນເປັນຂັ້ນຕອນແຍກຕ່າງຫາກທໍາອິດ deploy
/// ນີ້ແມ່ນເພື່ອປ້ອງກັນຫຼາຍໆຕົວຢ່າງຈາກການແຂ່ງຂັນເພື່ອແກ້ໄຂ schema ໃນເວລາດຽວກັນ
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
            "ກຳລັງປັບສະຄີມາຖານຂໍ້ມູນໃຫ້ຕົງກັບ Migration ລ່າສຸດ");

    private static readonly Action<ILogger, Exception?> PendingMigrationsCallback =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(4101, "PendingMigrations"),
            "ຖານຂໍ້ມູນມີ Migration ທີ່ຍັງບໍ່ໄດ້ຮັນ ແຕ່ Database:AutoMigrate ຖືກປິດໄວ້ ກະລຸນາຮັນ dotnet ef database update ກ່ອນໃຊ້ງານ");

    private readonly DormFlowDbContext _dbContext = dbContext;
    private readonly DatabaseSeeder _seeder = seeder;
    private readonly DatabaseOptions _databaseOptions = databaseOptions.Value;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    /// <summary>ປັບ schema ແລະສ້າງຂໍ້ມູນເລີ່ມຕົ້ນຕາມການຕັ້ງຄ່າ</summary>
    /// <param name="cancellationToken">Token ສໍາລັບການຍົກເລີກຄໍາສັ່ງ</param>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_databaseOptions.AutoMigrate)
        {
            MigratingCallback(_logger, null);
            await _dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            string[] pendingMigrations = [.. await _dbContext.Database
                .GetPendingMigrationsAsync(cancellationToken)];

            if (pendingMigrations.Length > 0)
            {
                PendingMigrationsCallback(_logger, null);

                if (_databaseOptions.FailOnPendingMigrations)
                {
                    throw new InvalidOperationException(
                        $"Database has {pendingMigrations.Length} pending migration(s). " +
                        "Run 'dotnet ef database update' before starting the application.");
                }
            }
        }

        if (_databaseOptions.SeedOnStartup)
        {
            await _seeder.SeedAsync(cancellationToken);
        }
    }
}
