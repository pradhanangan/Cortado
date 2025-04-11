using Customers.Domain.Common;

namespace Customers.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public Guid IdentityId { get; set; }
}
