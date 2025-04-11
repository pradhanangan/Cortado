namespace Cortado.API.Contracts;

public record CreateProductItemRequest(Guid ProductId, string Name, string Description, string Variants, decimal UnitPrice);