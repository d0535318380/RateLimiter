namespace RateLimiter.Core.Server;

public class FixedFramePolicy : IDisposable, IAsyncDisposable
{
    private readonly object _syncRoot = new();
    private int _requestCount = 0;
    private readonly FixedFramePolicyOptions _options;
    private readonly Timer _timer;
    private bool _disposed;

    public FixedFramePolicy(FixedFramePolicyOptions options)
    {
        var period = TimeSpan.FromSeconds(options.FrameInSeconds);
        _options = options;
        _timer = new Timer(Refresh, null, period, period);
    }

    private void Refresh(object? state)
    {
        if (_disposed)
        {
            return;
        }
        
        lock (_syncRoot)
        {
            _requestCount = 0;
        }
    }

    public bool IsAcquire()
    {
        var currentValue = Interlocked.Increment(ref _requestCount);
        return currentValue <= _options.PermitLimit;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        
        _timer.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await _timer.DisposeAsync();
        _disposed = true;
    }
}