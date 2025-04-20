using Shared.Common.Abstraction;

namespace Bookings.Domain.Entities;

public class Ticket : BaseAuditableEntity
{
    public Guid OrderItemId { get; set; }
    public string TicketNumber { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedDate { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public byte[]? QrCode { get; set; }
}
