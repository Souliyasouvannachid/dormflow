using System.Globalization;
using System.Text;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Services;

/// <summary>
/// เขียนอีเมลขาออกลงไฟล์ในโฟลเดอร์ที่กำหนดแทนการส่งผ่าน SMTP
/// </summary>
/// <remarks>
/// เลือกวิธีนี้เพราะระบบยังไม่ได้ผูกกับผู้ให้บริการอีเมลใด
/// การเขียนลงไฟล์ทำให้ทดสอบเส้นทางลืมรหัสผ่านได้จริงตั้งแต่ตอนนี้
/// และไม่ทำให้โทเคนตั้งรหัสผ่านใหม่หลุดไปอยู่ใน Log ของแอปพลิเคชัน
/// เมื่อเชื่อมต่อ SMTP แล้วให้เปลี่ยนการลงทะเบียนใน DependencyInjection เป็นตัวส่งจริงแทน
/// </remarks>
public sealed class PickupDirectoryEmailSender(
    IOptions<EmailOptions> emailOptions,
    ILogger<PickupDirectoryEmailSender> logger) : IEmailSender
{
    private static readonly Action<ILogger, string, Exception?> EmailWrittenCallback =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(3000, "EmailWritten"),
            "เขียนอีเมลขาออกลงไฟล์ {FilePath} แล้ว");

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

        // บันทึกเฉพาะที่อยู่ของไฟล์ ไม่บันทึกเนื้อหาเพราะมีโทเคนตั้งรหัสผ่านใหม่อยู่ข้างใน
        EmailWrittenCallback(_logger, filePath, null);
    }
}
