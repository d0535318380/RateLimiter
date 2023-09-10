// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateLimiter.Core.Client;
using Refit;

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;

builder.Services.AddLogging();
builder.Services.Configure<RateLimiterClientOptions>(config.GetSection(nameof(RateLimiterClientOptions)));

builder.Services
    .AddRefitClient<IRateLimiterClient>()
    .ConfigureHttpClient(x => x.BaseAddress = new Uri("http://localhost:8080"));

builder.Services
    .AddTransient<RateLimiterHandler>()
    .AddHostedService<RateLimiterService>();

var host = builder.Build();
host.Run();