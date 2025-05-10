using Microsoft.AspNetCore.Diagnostics;

namespace Logging.API;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Exception occurred: {Message}",
            exception.Message
        );

        var (statusCode, title) = MapException(exception);

        await Results.Problem(
            title: title,
            statusCode: statusCode,
            detail: exception.Message
        ).ExecuteAsync(httpContext);

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad request"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Error Server: We are on it!")
        };
    }
}
