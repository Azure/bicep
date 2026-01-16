// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.ExtensionHost.Api;
using Bicep.ExtensionHost.Configuration;
using Bicep.ExtensionHost.Extensions;
using Bicep.ExtensionHost.Oci;
using Bicep.ExtensionHost.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var extensionHostConfig = builder.Configuration
    .GetSection(ExtensionHostConfig.SectionName)
    .Get<ExtensionHostConfig>() ?? new ExtensionHostConfig();

// Determine storage path
var storagePath = extensionHostConfig.ExtensionStoragePath
    ?? Path.Combine(Path.GetTempPath(), "bicep-extension-host", Guid.NewGuid().ToString());

// Register services
builder.Services.AddSingleton(extensionHostConfig);
builder.Services.AddSingleton(sp => new OciArtifactClient(
    sp.GetRequiredService<ILogger<OciArtifactClient>>(),
    storagePath));
builder.Services.AddSingleton<ExtensionManager>();
builder.Services.AddSingleton<ExtensionApiHandler>();
builder.Services.AddHostedService<ExtensionInitializationService>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Health endpoint - returns detailed health status of all extensions
app.MapGet("/health", async (ExtensionManager extensionManager, CancellationToken cancellationToken) =>
{
    var status = await extensionManager.GetHealthStatusAsync(cancellationToken);
    var allHealthy = status.Values.All(s => s.IsPingSuccessful);

    return Results.Json(new
    {
        Status = allHealthy ? "Healthy" : "Unhealthy",
        Extensions = status
    }, statusCode: allHealthy ? 200 : 503);
});

// List available extensions
app.MapGet("/extensions", (ExtensionManager extensionManager) =>
{
    var extensions = extensionManager.GetExtensionNames();
    return Results.Ok(new { Extensions = extensions });
});

// Extension API endpoint - proxies requests to gRPC servers
app.MapPost("/extensions/{extensionName}", async (
    string extensionName,
    ExtensionRequest request,
    ExtensionApiHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleRequestAsync(extensionName, request, cancellationToken);

    if (response.ErrorData is not null)
    {
        return Results.Json(response, statusCode: 400);
    }

    return Results.Ok(response);
});

// Root endpoint with basic info
app.MapGet("/", () => Results.Ok(new
{
    Service = "Bicep Extension Host",
    Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0",
    Endpoints = new[]
    {
        "GET /health - Health check for all extensions",
        "GET /extensions - List available extensions",
        "POST /extensions/{name} - Execute extension operation"
    }
}));

app.Run();
