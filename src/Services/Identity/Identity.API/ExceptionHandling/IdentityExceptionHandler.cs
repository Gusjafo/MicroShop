using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MicroShop.Services.Identity.Application.Exceptions;

namespace MicroShop.Services.Identity.API.ExceptionHandling;

internal sealed class IdentityExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not IdentityException identityException)
        {
            return false;
        }

        var statusCode = identityException switch
        {
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            UserAlreadyExistsException => StatusCodes.Status409Conflict,
            RoleNotFoundException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        var problem = new ProblemDetails
        {
            Title = "Identity error",
            Detail = identityException.Message,
            Status = statusCode
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
