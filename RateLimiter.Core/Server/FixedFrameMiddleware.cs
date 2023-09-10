using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateLimiter.Core.Server;

public class FixedFrameMiddleware : IMiddleware, IDisposable
{
    private readonly ConcurrentDictionary<string, FixedFramePolicy> _polices = new(StringComparer.OrdinalIgnoreCase);
    private readonly FixedFramePolicyOptions _policySetting;
    private readonly ILogger<FixedFrameMiddleware> _logger;

    public FixedFrameMiddleware(
        IOptions<FixedFramePolicyOptions> policySetting, 
        ILogger<FixedFrameMiddleware> logger)
    {
        _policySetting = policySetting.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isClientIdExists = context.Request.Query.TryGetValue("clientId", out var clientId);

        if (!isClientIdExists)
        {
            await WriteFailed(context, $"ClientId not provided.");
            return;
        }

        var policy = _polices.GetOrAdd($"client.{clientId}", 
            _ => new FixedFramePolicy(_policySetting));

        if (policy.IsAcquire())
        {
            _logger.LogInformation("Success: {ClientId}.", clientId);
            await next(context);
            return;
        }

        _logger.LogError("Forbidden: {ClientId}.", clientId);
        await WriteFailed(context, $"Forbidden: {clientId}");
    }

    private static async Task WriteFailed(HttpContext context, string message)
    {
        context.Response.StatusCode = (int ) HttpStatusCode.ServiceUnavailable;
        await context.Response.WriteAsync(message);       
    }

    public void Dispose()
    {
        foreach (var policy in _polices.Values)
        {
            policy.Dispose();
        }
        
        _polices.Clear();
        
        _logger.LogInformation("FixedFrameMiddleware disposed");
    }
} 