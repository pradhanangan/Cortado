using System.ComponentModel.DataAnnotations;

namespace Shared.Common.Abstraction;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

}
