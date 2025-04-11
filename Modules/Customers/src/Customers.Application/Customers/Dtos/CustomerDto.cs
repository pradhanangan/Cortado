namespace Customers.Application.Customers.Dtos;

public sealed record CustomerDto(Guid Id, string Username, string Email, Guid IdentityId);
