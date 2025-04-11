namespace Products.Application.Products.Dtos;

public sealed record ProductItemDto(Guid Id, string Name, string Description, string Variants, decimal UnitPrice);
