using Bookings.Application.Common.Models;

namespace Bookings.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, List<EmailAttachment>? attachments = null);
}
