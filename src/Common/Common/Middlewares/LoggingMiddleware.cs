using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
        
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
        
    public async Task Invoke(HttpContext context)
    {
        //get correlation id from request header
        // var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
        var stopwatch = Stopwatch.StartNew();
            
        // Log details about the incoming request.
        _logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
            
        await _next(context);
            
        stopwatch.Stop();
            
        // Log details about the response after processing.
        _logger.LogInformation("Finished handling request: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}