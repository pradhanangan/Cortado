using System.ComponentModel.DataAnnotations;

namespace Customers.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

}
