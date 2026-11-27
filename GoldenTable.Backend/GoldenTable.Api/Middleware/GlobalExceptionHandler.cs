using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GoldenTable.Api.Middleware;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // A request the endpoint couldn't read (malformed JSON, an unknown enum name...) is the client's error.
        // Minimal APIs only throw this in Development; elsewhere they answer 400 themselves.
        if (exception is BadHttpRequestException badRequest)
        {
            logger.LogWarning(exception, "Bad request");

            httpContext.Response.StatusCode = badRequest.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = badRequest.StatusCode,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad request",
                Detail = badRequest.Message
            }, cancellationToken);

            return true;
        }

        logger.LogError(exception, "Unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
