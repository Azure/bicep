// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Services;

/// <summary>
/// Applies a user-specified configuration file (the <c>--config</c> option or the JSON-RPC <c>configPath</c> field)
/// as the explicit configuration of the current operation.
/// </summary>
public class ExplicitConfigurationScopeFactory(
    InputOutputArgumentsResolver inputOutputArgumentsResolver,
    IFileExplorer fileExplorer,
    ExplicitConfigurationContext explicitConfigurationContext)
{
    /// <summary>
    /// Uses the configuration file at <paramref name="configPath"/> in place of bicepconfig.json discovery
    /// until the returned scope is disposed. Returns <see langword="null"/> if <paramref name="configPath"/> is not specified.
    /// </summary>
    /// <param name="configPath">A local file path, absolute or relative to the current working directory.</param>
    /// <exception cref="CommandLineException">The configuration file does not exist.</exception>
    public IDisposable? CreateScope(string? configPath)
    {
        if (configPath is null)
        {
            return null;
        }

        var configFileUri = inputOutputArgumentsResolver.PathToUri(configPath);

        if (!fileExplorer.GetFile(configFileUri).Exists())
        {
            throw new CommandLineException($"Could not find configuration file \"{configPath}\".");
        }

        return explicitConfigurationContext.Enter(configFileUri);
    }
}
