using Shared.Common.Abstraction;

namespace Products.Domain.Entities;

public class ProductItem : BaseAuditableEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Variants { get; set; } // Kids, Adults, Seniors Or Color, Size, Material ect combination of all, later move to separate table and add relationship here.
    public decimal UnitPrice { get; set; }
    public Guid ProductId { get; set; }
}
