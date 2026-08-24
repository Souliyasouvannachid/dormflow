using DormFlow.Application.Common.Models;

namespace DormFlow.Application.Common.Abstractions;

/// <summary>ส่งอีเมลออกจากระบบ</summary>
public interface IEmailSender
{
    /// <summary>ส่งอีเมลหนึ่งฉบับ</summary>
    /// <param name="message">ข้อความที่ต้องการส่ง</param>
    /// <param name="cancellationToken">โทเคนสำหรับยกเลิกคำสั่ง</param>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
