using Microsoft.Extensions.Logging;

namespace RateLimiter.Core.Server;

public class ClientRequestHandler
{
    private readonly ILogger<ClientRequestHandler> _logger;

    public ClientRequestHandler(ILogger<ClientRequestHandler> logger)
    {
        _logger = logger;
    }
    
    public Task<string> HandleAsync(string clientId, CancellationToken token = default)
    {
        _logger.LogInformation("Client request: {ClientId}", clientId);

        return Task.FromResult($"Success: {clientId}");
    }
}