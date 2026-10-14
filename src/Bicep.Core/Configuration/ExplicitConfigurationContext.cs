// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Tracks the explicit configuration file, if any, for the current logical operation
/// (for example, a CLI invocation or a single JSON-RPC request).
/// When set, the explicit configuration replaces bicepconfig.json discovery and is used as the
/// leaf configuration for every source file loaded by the operation.
/// </summary>
/// <remarks>
/// The value flows with the async execution context, so concurrent operations can each use
/// a different explicit configuration without interfering with one another.
/// </remarks>
public sealed class ExplicitConfigurationContext
{
    private readonly AsyncLocal<IOUri?> configFileUri = new();

    /// <summary>
    /// The URI of the explicit configuration file for the current operation,
    /// or <see langword="null"/> if configuration should be discovered.
    /// </summary>
    public IOUri? ConfigFileUri => this.configFileUri.Value;

    /// <summary>
    /// Uses <paramref name="configFileUri"/> as the explicit configuration until the returned scope is disposed.
    /// </summary>
    public IDisposable Enter(IOUri configFileUri)
    {
        var scope = new Scope(this, this.configFileUri.Value);
        this.configFileUri.Value = configFileUri;

        return scope;
    }

    private sealed class Scope(ExplicitConfigurationContext context, IOUri? previousConfigFileUri) : IDisposable
    {
        public void Dispose() => context.configFileUri.Value = previousConfigFileUri;
    }
}
