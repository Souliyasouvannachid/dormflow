using System.Net;
using System.Net.Mail;
using DormFlow.Application.Common.Abstractions;
using DormFlow.Application.Common.Models;
using DormFlow.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace DormFlow.Infrastructure.Services;

/// <summary>ສົ່ງອີເມວຜ່ານ SMTP</summary>
public sealed class SmtpEmailSender(IOptions<EmailOptions> emailOptions) : IEmailSender
{
    private readonly EmailOptions _emailOptions = emailOptions.Value;

    /// <inheritdoc />
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        using MailMessage mailMessage = new()
        {
            From = new MailAddress(_emailOptions.FromAddress, _emailOptions.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = false,
        };
        mailMessage.To.Add(new MailAddress(message.To));

        using SmtpClient client = new(_emailOptions.SmtpHost!, _emailOptions.SmtpPort)
        {
            EnableSsl = _emailOptions.SmtpUseSsl,
        };

        if (!string.IsNullOrWhiteSpace(_emailOptions.SmtpUsername))
        {
            client.Credentials = new NetworkCredential(
                _emailOptions.SmtpUsername,
                _emailOptions.SmtpPassword);
        }

        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}
