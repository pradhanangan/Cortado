using Microsoft.AspNetCore.Mvc;
using Shared.Common.Abstraction;

namespace Cortado.API.Extensions;

public static class ResultExtension
{
    public static ProblemDetails ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a successful result to ProblemDetails.");
        }
        
        var problemDetails = new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", new[] { result.Error } }
            }
        };
        return problemDetails;
    }
}
