using Products.Domain.Common;

namespace Products.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
   

    public ICollection<ProductItem> ProductItems = new List<ProductItem>();
}
