using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Customers.Application.Customers;

namespace Cortado.API.Pages
{
    public abstract class PageModelBase<T> : PageModel where T : PageModel
    {
        private ILogger<T>? _logger;
        private ISender? _mediator;

        // Lazy initialization of Logger and Mediator
        protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        /// <summary>
        /// Retrieves the authenticated user's ID from the claims.
        /// </summary>
        /// <returns>The user ID as a Guid, or null if not found.</returns>
        protected Guid? GetUserId()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                Logger.LogWarning("User ID (sub) is missing from the claims.");
                return null;
            }

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            Logger.LogWarning("User ID (sub) is not a valid GUID.");
            return null;
        }

        /// <summary>
        /// Retrieves the CustomerId associated with the authenticated user.
        /// </summary>
        /// <returns>The CustomerId as a Guid, or null if not found.</returns>
        protected async Task<Guid?> GetCustomerIdAsync()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                Logger.LogWarning("Cannot retrieve CustomerId because UserId is null.");
                return null;
            }

            var response = await Mediator.Send(new GetCustomerByIdentityIdQuery(userId.Value));
            if (response.IsFailure)
            {
                Logger.LogWarning("Customer not found for UserId: {UserId}", userId);
                return null;
            }

            return response.Value.Id;
        }
    }
}
