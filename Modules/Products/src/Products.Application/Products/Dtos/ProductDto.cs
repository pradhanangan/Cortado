namespace Products.Application.Products.Dtos;

public sealed record ProductDto(
    Guid Id,
    Guid CustomerId,
    string Code,
    string Name,
    string Description,
    string ImageUrl,
    string Address,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    List<ProductItemDto> ProductItems,
    string RegistrationUrl);
