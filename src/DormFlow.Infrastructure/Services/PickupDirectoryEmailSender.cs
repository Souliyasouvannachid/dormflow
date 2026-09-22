using System.Globalization;
using System.Text;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Services;

/// <summary>
/// ຂຽນອີເມວຂາອອກໄປຫາໄຟລ໌ໃນໂຟນເດີທີ່ກໍານົດແທນທີ່ຈະສົ່ງຜ່ານ SMTP
/// </summary>
/// <remarks>
/// ເລືອກວິທີນີ້ເພາະວ່າລະບົບຍັງບໍ່ໄດ້ເຊື່ອມຕໍ່ກັບຜູ້ໃຫ້ບໍລິການອີເມວໃດໆ
/// ການຂຽນໄຟລ໌ຊ່ວຍໃຫ້ທ່ານສາມາດທົດສອບເສັ້ນທາງລະຫັດຜ່ານທີ່ລືມຂອງເຈົ້າໄດ້ໃນຕອນນີ້
/// ແລະບໍ່ໄດ້ເຮັດໃຫ້ token ການຕັ້ງລະຫັດຜ່ານໃຝ່ສູນເສຍໄປໃນ Log ຂອງຄໍາຮ້ອງສະຝັກ
/// ເມື່ອເຊື່ອມຕໍ່ SMTP ຫຼັງຈາກນັ້ນ, ມີການປ່ຽນແປງການລົງທະບຽນໃນ DependencyInjection ມັນແມ່ນຜູ້ສົ່ງຕົວຈິງແທນ
/// </remarks>
public sealed class PickupDirectoryEmailSender(
    IOptions<EmailOptions> emailOptions,
    ILogger<PickupDirectoryEmailSender> logger) : IEmailSender
{
    private static readonly Action<ILogger, string, Exception?> EmailWrittenCallback =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(3000, "EmailWritten"),
            "ຂຽນອີເມວຂາອອກລົງໄຟລ໌ {FilePath} ແລ້ວ");

    private readonly EmailOptions _emailOptions = emailOptions.Value;
    private readonly ILogger<PickupDirectoryEmailSender> _logger = logger;

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        string directory = Path.IsPathRooted(_emailOptions.PickupDirectory)
            ? _emailOptions.PickupDirectory
            : Path.Combine(AppContext.BaseDirectory, _emailOptions.PickupDirectory);

        Directory.CreateDirectory(directory);

        string fileName = string.Create(
            CultureInfo.InvariantCulture,
            $"{DateTimeOffset.UtcNow:yyyyMMdd-HHmmssfff}-{Guid.CreateVersion7():N}.eml");

        string filePath = Path.Combine(directory, fileName);

        StringBuilder content = new();
        content.AppendLine(CultureInfo.InvariantCulture, $"From: {_emailOptions.FromName} <{_emailOptions.FromAddress}>");
        content.AppendLine(CultureInfo.InvariantCulture, $"To: {message.To}");
        content.AppendLine(CultureInfo.InvariantCulture, $"Subject: {message.Subject}");
        content.AppendLine(CultureInfo.InvariantCulture, $"Date: {DateTimeOffset.UtcNow:R}");
        content.AppendLine("Content-Type: text/plain; charset=utf-8");
        content.AppendLine();
        content.AppendLine(message.Body);

        await File.WriteAllTextAsync(filePath, content.ToString(), Encoding.UTF8, cancellationToken);

        // ບັນທຶກພຽງແຕ່ທີ່ຢູ່ໄຟລ໌. ເນື້ອຫາບໍ່ໄດ້ຖືກບັນທຶກເນື່ອງຈາກມີລະຫັດປ່ຽນລະຫັດລັບພາຍໃນ
        EmailWrittenCallback(_logger, filePath, null);
    }
}
