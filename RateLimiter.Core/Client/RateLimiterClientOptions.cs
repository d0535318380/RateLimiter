
namespace RateLimiter.Core.Client;
public class RateLimiterClientOptions
{
    public int ClientCount { get; set; } = 10;
    public int MaxIntervalSeconds { get; set; } = 10;
}