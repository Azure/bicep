// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Bicep.ExtensionHost.Configuration;
using Bicep.ExtensionHost.Oci;

namespace Bicep.ExtensionHost.Extensions;

/// <summary>
/// Manages multiple extension instances - downloading, starting, and providing access to them.
/// </summary>
public class ExtensionManager : IAsyncDisposable
{
    private readonly ILogger<ExtensionManager> _logger;
    private readonly OciArtifactClient _ociClient;
    private readonly ConcurrentDictionary<string, ExtensionInstance> _extensions = new();
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _disposed;

    public ExtensionManager(ILogger<ExtensionManager> logger, OciArtifactClient ociClient)
    {
        _logger = logger;
        _ociClient = ociClient;
    }

    /// <summary>
    /// Initializes all configured extensions by downloading and starting them.
    /// </summary>
    public async Task InitializeExtensionsAsync(IEnumerable<ExtensionConfig> extensions, CancellationToken cancellationToken)
    {
        await _initLock.WaitAsync(cancellationToken);
        try
        {
            foreach (var config in extensions)
            {
                if (_extensions.ContainsKey(config.Name))
                {
                    _logger.LogWarning("Extension '{Name}' is already initialized, skipping", config.Name);
                    continue;
                }

                try
                {
                    var binaryInfo = await _ociClient.DownloadExtensionAsync(config, cancellationToken);
                    var instance = await ExtensionInstance.StartAsync(
                        binaryInfo.ExtensionName,
                        binaryInfo.BinaryPath,
                        _logger,
                        cancellationToken);

                    _extensions[config.Name] = instance;
                    _logger.LogInformation("Extension '{Name}' initialized successfully", config.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize extension '{Name}'", config.Name);
                    throw;
                }
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// Gets an extension instance by name.
    /// </summary>
    public ExtensionInstance? GetExtension(string name)
    {
        return _extensions.TryGetValue(name, out var instance) ? instance : null;
    }

    /// <summary>
    /// Gets all registered extension names.
    /// </summary>
    public IEnumerable<string> GetExtensionNames()
    {
        return _extensions.Keys;
    }

    /// <summary>
    /// Gets the health status of all extensions.
    /// </summary>
    public async Task<Dictionary<string, ExtensionHealthStatus>> GetHealthStatusAsync(CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, ExtensionHealthStatus>();

        foreach (var (name, instance) in _extensions)
        {
            try
            {
                var isRunning = instance.IsRunning;
                var pingSuccess = isRunning && await instance.PingAsync(cancellationToken);

                results[name] = new ExtensionHealthStatus(
                    IsRunning: isRunning,
                    IsPingSuccessful: pingSuccess,
                    Status: pingSuccess ? "Healthy" : (isRunning ? "Unhealthy" : "Stopped"));
            }
            catch (Exception ex)
            {
                results[name] = new ExtensionHealthStatus(
                    IsRunning: false,
                    IsPingSuccessful: false,
                    Status: $"Error: {ex.Message}");
            }
        }

        return results;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (var (name, instance) in _extensions)
        {
            try
            {
                await instance.DisposeAsync();
                _logger.LogInformation("Extension '{Name}' disposed", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing extension '{Name}'", name);
            }
        }

        _extensions.Clear();
        _initLock.Dispose();
    }
}

/// <summary>
/// Represents the health status of an extension.
/// </summary>
public record ExtensionHealthStatus(
    bool IsRunning,
    bool IsPingSuccessful,
    string Status);
