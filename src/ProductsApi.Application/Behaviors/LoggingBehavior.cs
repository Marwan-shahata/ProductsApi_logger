using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ProductsApi.Application.Behaviors;

/// <summary>Logs every command/query: start, duration, and failures. Request payloads are not logged.</summary>
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var response = await next();
            logger.LogInformation("Handled {RequestName} in {ElapsedMs} ms", requestName, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            // The API's exception handler logs unhandled errors with the full stack trace; keep this one short.
            logger.LogWarning("{RequestName} failed after {ElapsedMs} ms: {ExceptionType}",
                requestName, stopwatch.ElapsedMilliseconds, ex.GetType().Name);
            throw;
        }
    }
}
