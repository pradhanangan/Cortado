using Bookings.Application.Common.Interfaces;
using Bookings.Application.Common.Models;
using Bookings.Application.Configurations;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bookings.Infrastructure.Services;

public class SendGridEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SendGridEmailService(IOptions<EmailSettings> emailSettingsOptions)
    {
        _emailSettings = emailSettingsOptions.Value;    
    }

    public async Task SendEmailAsync(string to, string subject, string body, List<EmailAttachment>? attachments = null)
    {
        var client = new SendGridClient(_emailSettings.SendGridApiKey);
        var from_email = new EmailAddress(_emailSettings.FromEmail);
        var to_email = new EmailAddress(to);

        var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, null, body);
        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                msg.AddAttachment(attachment.Filename, attachment.Content, attachment.Type, attachment.Disposition, attachment.ContentId);
            }
        }
        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
    }
}
