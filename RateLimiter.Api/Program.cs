using Microsoft.AspNetCore.Mvc;
using RateLimiter.Core.Server;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddMemoryCache();
builder.Services.Configure<FixedFramePolicyOptions>(config.GetSection(nameof(FixedFramePolicyOptions)));
builder.Services.AddTransient<ClientRequestHandler>();
builder.Services.AddSingleton<FixedFrameMiddleware>();

var app = builder.Build();
app.UseMiddleware<FixedFrameMiddleware>();

app.MapGet("/", 
    ([FromQuery] string clientId, ClientRequestHandler handler, CancellationToken token) 
        => handler.HandleAsync(clientId, token));

app.Run();