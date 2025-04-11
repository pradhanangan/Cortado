using Bookings.Domain.Common;

namespace Bookings.Domain.Entities;

public class OrderItem : BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductItemId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    // Navigation property
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
