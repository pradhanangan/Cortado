using System.ComponentModel.DataAnnotations;

namespace Products.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

}
