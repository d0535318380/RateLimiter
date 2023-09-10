using Refit;

namespace RateLimiter.Core.Client;

public interface IRateLimiterClient
{
    [Get("/")]
    Task<HttpResponseMessage> GetAsync([Query] string clientId);
}