﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common.Exceptions;

namespace Cortado.API.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(RequestValidationException), HandleValidationException },
            //{ typeof(NotFoundException), HandleNotFoundException },
            //{ typeof(BadRequestException), HandleBadRequestException },
            //{ typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            //{ typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            //{ typeof(DatabaseException), HandleDatabaseException },
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        //if (!context.ModelState.IsValid)
        //{
        //    HandleInvalidModelStateException(context);
        //    return;
        //}

        HandleInternalServerErrorException(context);
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (RequestValidationException)context.Exception;

        var details = new ValidationProblemDetails(exception.Errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);

        context.ExceptionHandled = true;
    }

    //private void HandleBadRequestException(ExceptionContext context)
    //{
    //    var exceptions = (BadRequestException)context.Exception;

    //    var details = new ProblemDetails()
    //    {
    //        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    //        Title = "Bad Request"
    //    };

    //    context.Result = new BadRequestObjectResult(details);

    //    context.ExceptionHandled = true;
    //}

    //private void HandleInvalidModelStateException(ExceptionContext context)
    //{
    //    var details = new ValidationProblemDetails(context.ModelState)
    //    {
    //        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
    //    };

    //    context.Result = new BadRequestObjectResult(details);

    //    context.ExceptionHandled = true;
    //}

    //private void HandleNotFoundException(ExceptionContext context)
    //{
    //    var exception = (NotFoundException)context.Exception;

    //    var details = new ProblemDetails()
    //    {
    //        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    //        Title = "The specified resource was not found.",
    //        Detail = exception.Message
    //    };

    //    context.Result = new NotFoundObjectResult(details);

    //    context.ExceptionHandled = true;
    //}

    //private void HandleUnauthorizedAccessException(ExceptionContext context)
    //{
    //    var details = new ProblemDetails
    //    {
    //        Status = StatusCodes.Status401Unauthorized,
    //        Title = "Unauthorized",
    //        Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
    //    };

    //    context.Result = new ObjectResult(details)
    //    {
    //        StatusCode = StatusCodes.Status401Unauthorized
    //    };

    //    context.ExceptionHandled = true;
    //}

    //private void HandleForbiddenAccessException(ExceptionContext context)
    //{
    //    var details = new ProblemDetails
    //    {
    //        Status = StatusCodes.Status403Forbidden,
    //        Title = "Forbidden",
    //        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
    //    };

    //    context.Result = new ObjectResult(details)
    //    {
    //        StatusCode = StatusCodes.Status403Forbidden
    //    };

    //    context.ExceptionHandled = true;
    //}

    private void HandleInternalServerErrorException(ExceptionContext context)
    {
        var details = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = context.Exception.Message is null ? context.Exception.InnerException?.Message : context.Exception.Message,
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }

    //private void HandleDatabaseException(ExceptionContext context)
    //{
    //    var exception = (DatabaseException)context.Exception;

    //    var details = new ProblemDetails()
    //    {
    //        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    //        Title = "Bad Request",
    //        Detail = exception.Message
    //    };

    //    context.Result = new BadRequestObjectResult(details);

    //    context.ExceptionHandled = true;

    //}
}

