namespace Products.Application.Products.Dtos;

public sealed record ProductDto(Guid Id, string Code, string Name, string Description, DateOnly StartDate, DateOnly EndDate, List<ProductItemDto> ProductItems);
