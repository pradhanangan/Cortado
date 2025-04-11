using System.ComponentModel.DataAnnotations;

namespace Bookings.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

}
