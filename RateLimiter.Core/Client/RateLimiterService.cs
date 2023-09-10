using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimiter.Core.Server;

namespace RateLimiter.Core.Client;
public class RateLimiterService : BackgroundService 
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<RateLimiterService> _logger;
    private readonly RateLimiterClientOptions _options;

    public RateLimiterService(
        IOptions<RateLimiterClientOptions> options,
        IServiceProvider provider, 
        ILogger<RateLimiterService> logger)
    {
        _provider = provider;
        _logger = logger;
        _options = options.Value;
    }
    
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var handlers = CreateHandlers();
        var tasks = handlers.Select(x => x.ExecuteAsync(cancellationToken));

        _logger.LogInformation("RateLimiterService started.....");
        return Task.WhenAll(tasks);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RateLimiterService stopped......");
        return base.StopAsync(cancellationToken);
    }

    private IEnumerable<RateLimiterHandler> CreateHandlers()
    {
        for (int i = 0; i < _options.ClientCount; i++)
        {
            yield return _provider.GetRequiredService<RateLimiterHandler>();
        }
    }
}