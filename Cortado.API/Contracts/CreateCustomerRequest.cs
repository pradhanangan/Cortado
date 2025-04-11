namespace Cortado.API.Contracts;

public sealed record CreateCustomerRequest(string Username, string Email, Guid IdentityId);
