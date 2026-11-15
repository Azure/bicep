// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using Bicep.Orchestration.Local;

var workspace = Path.GetFullPath(Value("--workspace") ?? Directory.GetCurrentDirectory());
var entry = Path.GetFullPath(Value("--entry") ?? "docs/experimental/orchestration-samples/basic/main.bicepparam", workspace);
var state = Path.GetFullPath(Value("--state") ?? ".azure/local-demo-state.json", workspace);
var port = int.Parse(Value("--port") ?? "5088", System.Globalization.CultureInfo.InvariantCulture);
if (args.Contains("--real")) { throw new ArgumentException("The local orchestration viewer supports JSON simulation only. Remove --real."); }
if (port is < 1024 or > 65535) { throw new ArgumentException("Port must be between 1024 and 65535."); }
if (!File.Exists(entry) || !entry.EndsWith(".bicepparam", StringComparison.Ordinal)) { throw new ArgumentException("Entry must be an existing .bicepparam file."); }
Directory.CreateDirectory(Path.GetDirectoryName(state) ?? workspace);
var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [], WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot") });
builder.Logging.ClearProviders(); // Do not log request headers, source, resource properties or provider failures.
builder.WebHost.ConfigureKestrel(server => { server.Listen(IPAddress.Loopback, port); server.Limits.MaxRequestBodySize = 64 * 1024; });
builder.Services.AddBicepCore().AddBicepLocalDeploy();
builder.Services.AddSingleton(new LaunchOptions(workspace, entry, state));
builder.Services.AddSingleton<OrchestrationService>();
builder.Services.AddMcpServer().WithHttpTransport().WithTools<OrchestrationTools>();
var app = builder.Build();
var origin = $"http://127.0.0.1:{port}";
app.Use(async (context, next) =>
{
    context.Response.Headers.CacheControl = "no-store";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self'; connect-src 'self'; frame-ancestors 'none'";
    if (context.Request.Host.Value != $"127.0.0.1:{port}" ||
        (context.Request.Headers.TryGetValue("Origin", out var suppliedOrigin) && suppliedOrigin.ToString() != origin))
    {
        context.Response.StatusCode = 403;
        return;
    }
    try { await next(context); }
    catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
    catch (Exception exception) when (!context.Response.HasStarted)
    {
        context.Response.StatusCode = exception is KeyNotFoundException ? 404 : exception is InvalidOperationException ? 409 : 422;
        // Only our fixed domain error prefixes are public; SDK errors can include sensitive bodies.
        var message = exception.Message;
        var safe = new[] { "Stale plan:", "Explicit approval", "Plan not found", "An operation", "Compilation failed", "Operation not found" }.Any(message.StartsWith);
        await context.Response.WriteAsJsonAsync(new { error = safe ? message : "Operation could not be completed. Check source/configuration and provider access, then retry." });
    }
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapMcp("/mcp");
await Console.Out.WriteLineAsync($"Local orchestration viewer: {origin}\nTarget: JSON SIMULATION");
await app.RunAsync();

string? Value(string option)
{
    var index = Array.IndexOf(args, option);
    if (index < 0) { return null; }
    return index + 1 < args.Length ? args[index + 1] : throw new ArgumentException($"Missing value for {option}.");
}
