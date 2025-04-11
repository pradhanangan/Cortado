using Bookings.Domain.Common;

namespace Bookings.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public string OrderNumber { get; set; }
    public Guid ProductId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsPaid { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsVerified { get; set; }
    public bool IsConfirmed { get; set; }
    public string PaymentId { get; set; }

    // Navigation property
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

}
