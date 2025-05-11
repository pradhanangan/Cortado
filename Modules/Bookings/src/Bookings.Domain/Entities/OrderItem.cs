using Shared.Common.Abstraction;

namespace Bookings.Domain.Entities;

public class OrderItem : BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    

    // Navigation property
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
