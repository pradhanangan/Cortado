namespace Bookings.Application.Configurations;

public sealed class EmailSettings
{
    public string SendGridApiKey { get; set; }
    public string FromEmail { get; set; }
}
