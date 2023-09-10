using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateLimiter.Core.Client;
public class RateLimiterHandler : IDisposable
{
    private readonly IRateLimiterClient _client;
    private readonly ILogger<RateLimiterHandler> _logger;
    private readonly RateLimiterClientOptions _options;

    private readonly Random _random = new();

    public RateLimiterHandler(
        IOptions<RateLimiterClientOptions> options,
        IRateLimiterClient client, 
        ILogger<RateLimiterHandler> logger)
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
    }

    public async Task ExecuteAsync(CancellationToken token)
    {
        _logger.LogInformation("RateLimiterClientHandler started");
        
        while (!token.IsCancellationRequested)
        {
            var delay = _random.Next(100, _options.MaxIntervalSeconds * 1000);

            await ExecuteInternalAsync(token);
            await Task.Delay(TimeSpan.FromMilliseconds(delay), token);
        }
        
        _logger.LogInformation("RateLimiterClientHandler finished");
    }

    private async Task ExecuteInternalAsync(CancellationToken token)
    {
        try
        {
            var clientId = _random.Next(1, _options.ClientCount);
            var result = await _client.GetAsync(clientId.ToString());
            var content = await result.Content.ReadAsStringAsync(token);
            
            _logger.LogInformation("Client: {ClientId}, {StatusCode}: {Content}", 
                clientId, result.StatusCode, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed");
        }
        
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}