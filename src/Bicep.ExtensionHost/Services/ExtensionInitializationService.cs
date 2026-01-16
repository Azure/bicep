// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.ExtensionHost.Configuration;
using Bicep.ExtensionHost.Extensions;

namespace Bicep.ExtensionHost.Services;

/// <summary>
/// Background service that initializes extensions on application startup.
/// </summary>
public class ExtensionInitializationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExtensionInitializationService> _logger;
    private readonly ExtensionHostConfig _config;

    public ExtensionInitializationService(
        IServiceProvider serviceProvider,
        ILogger<ExtensionInitializationService> logger,
        ExtensionHostConfig config)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting extension initialization service");

        if (_config.Extensions.Count == 0)
        {
            _logger.LogWarning("No extensions configured. Add extensions to the configuration to enable functionality.");
            return;
        }

        try
        {
            // Get the singleton ExtensionManager from DI
            var extensionManager = _serviceProvider.GetRequiredService<ExtensionManager>();

            _logger.LogInformation("Initializing {Count} extension(s)...", _config.Extensions.Count);

            await extensionManager.InitializeExtensionsAsync(_config.Extensions, stoppingToken);

            _logger.LogInformation("All extensions initialized successfully");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Extension initialization cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize extensions. Application may not function correctly.");
            throw;
        }
    }
}
