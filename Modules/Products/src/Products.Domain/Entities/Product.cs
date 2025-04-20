using Shared.Common.Abstraction;

namespace Products.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Address { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
   

    public ICollection<ProductItem> ProductItems = new List<ProductItem>();
}
