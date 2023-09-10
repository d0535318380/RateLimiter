using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateLimiter.Core.Server;

public class FixedFrameMiddleware  : IMiddleware
{
    private readonly ConcurrentDictionary<string, FixedFramePolicy> _polices = new(StringComparer.OrdinalIgnoreCase);
    private readonly IOptions<FixedFramePolicyOptions> _policySetting;
    private readonly ILogger<FixedFrameMiddleware> _logger;

    public FixedFrameMiddleware(
        IOptions<FixedFramePolicyOptions> policySetting, 
        ILogger<FixedFrameMiddleware> logger)
    {
        _policySetting = policySetting;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isClientIdExists = context.Request.Query.TryGetValue("clientId", out var clientId);

        if (!isClientIdExists)
        {
            await WriteFailed(context, $"ClientId not provided: {clientId}");
            return;
        }

        var policy = _polices.GetOrAdd($"client.{clientId}", 
            _ => new FixedFramePolicy(_policySetting.Value));

        if (policy.IsAcquire())
        {
            _logger.LogInformation("{ClientId} - success..", clientId);
            await next(context);
            return;
        }

        _logger.LogWarning("{ClientId} - forbidden..", clientId);
        await WriteFailed(context, $"Forbidden: {clientId}");
    }

    private static async Task WriteFailed(HttpContext context, string message)
    {
        context.Response.StatusCode = (int ) HttpStatusCode.ServiceUnavailable;
        await context.Response.WriteAsync(message);       
    }
}