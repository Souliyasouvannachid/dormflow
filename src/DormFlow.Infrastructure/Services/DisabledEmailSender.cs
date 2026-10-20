using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Exceptions;
using DormFlow.Application.Common.Models;

namespace DormFlow.Infrastructure.Services;

/// <summary>Explicitly disables outbound mail without recording messages or tokens.</summary>
public sealed class DisabledEmailSender : IEmailSender
{
    /// <inheritdoc />
    public bool IsEnabled => false;

    /// <inheritdoc />
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        throw new ServiceUnavailableException("ระบบส่งอีเมลยังไม่เปิดใช้งาน กรุณาติดต่อผู้ดูแลระบบ");
    }
}
