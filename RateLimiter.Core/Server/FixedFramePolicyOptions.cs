namespace RateLimiter.Core.Server;

public class FixedFramePolicyOptions
{
    public int PermitLimit { get; set; } = 5;
    public int FrameInSeconds { get; set; } = 5;
}