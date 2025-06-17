using Customers.Application.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cortado.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class ApiControllerBase<T> : ControllerBase where T : ControllerBase
{
    private ILogger<T>? _logger;
    private ISender? _mediator;

    // Lazy initialization of Logger and Mediator
    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();



    /// <summary>
    /// Retrieves the CustomerId associated with the authenticated user.
    /// </summary>
    /// <returns>The CustomerId as a Guid.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the 'sub' claim is missing from the token.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no customer is found for the given 'sub'.</exception>
    protected async Task<Guid?> GetCustomerIdAsync()
    {
       
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(sub))
        {
            //throw new UnauthorizedAccessException("User ID (sub) is missing from the token.");
            Logger.LogWarning("User ID (sub) is missing from the token.");
            return null;
        }

        var response = await Mediator.Send(new GetCustomerByIdentityIdQuery(Guid.Parse(sub)));


        if (response.IsFailure)
        {
            Logger.LogWarning("Customer not found for the given user ID.");
            return null;
        }

        return response.Value.Id;
    }
}
