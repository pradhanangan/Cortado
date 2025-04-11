namespace Cortado.API.Contracts;

public record CreateProductRequest(string Code, string Name, string Description, DateOnly StartDate, DateOnly EndDate);

