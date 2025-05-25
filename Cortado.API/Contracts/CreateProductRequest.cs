namespace Cortado.API.Contracts;

public record CreateProductRequest(string Code, string Name, string Description, string Address, DateOnly StartDate, DateOnly EndDate, TimeOnly StartTime, TimeOnly EndTime, IFormFile Image);

