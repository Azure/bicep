// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;
using Bicep.Local.Deploy.Azure;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Helpers;

public class ArmDeploymentProviderFactory(
    IArmDeploymentProvider armDeploymentProvider,
    IEnvironment environment,
    ILogger logger)
{
    public Session Create()
    {
        var path = environment.GetVariable(SimulatedArmDeploymentProvider.EnvironmentVariable);
        if (path is null)
        {
            return new(armDeploymentProvider, null);
        }
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new CommandLineException($"{SimulatedArmDeploymentProvider.EnvironmentVariable} must contain a JSON state file path.");
        }
        path = Path.GetFullPath(path, environment.CurrentDirectory);
        logger.LogWarning("SIMULATION: Azure operations use local state at {Path}.", path);
        var provider = new SimulatedArmDeploymentProvider(path);
        return new(provider, provider);
    }

    public sealed class Session(IArmDeploymentProvider provider, IAsyncDisposable? ownedProvider) : IAsyncDisposable
    {
        public IArmDeploymentProvider Provider { get; } = provider;

        public ValueTask DisposeAsync() => ownedProvider?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
