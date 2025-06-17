namespace Cortado.API.Contracts;

public record CreateProductItemRequest(Guid ProductId, string Name, string Description, string Variants, bool IsFree, decimal UnitPrice);