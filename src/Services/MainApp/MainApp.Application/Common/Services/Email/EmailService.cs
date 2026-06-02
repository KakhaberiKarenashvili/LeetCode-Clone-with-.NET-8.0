using System.Net;
using System.Net.Mail;
using MainApp.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace MainApp.Application.Common.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailOptions _options;

    public EmailService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient(_options.SmtpHost)
        {
            Port = _options.SmtpPort,
            Credentials = new NetworkCredential(_options.SmtpUserEmail, _options.SmtpPassword),
            EnableSsl = true,
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_options.SmtpUserEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        message.To.Add(to);

        await client.SendMailAsync(message, cancellationToken);
    }
}
