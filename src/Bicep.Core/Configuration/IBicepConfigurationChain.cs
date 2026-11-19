// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration;

/// <summary>
/// Represents a chain of Bicep configuration files linked via "extends",
/// from the leaf (most specific) file to its most distant base.
/// </summary>
public interface IBicepConfigurationChain
{
    /// <summary>
    /// Returns the effective configuration produced by merging all layers in the chain,
    /// with leaf settings winning over base settings. Its diagnostics include every diagnostic for the chain.
    /// </summary>
    IBicepConfiguration GetEffectiveConfiguration();

    /// <summary>
    /// Returns the diagnostics for each configuration file in the chain, keyed by the file they belong to.
    /// Every file in the chain has an entry, even without diagnostics, so callers can clear stale ones.
    /// </summary>
    DiagnosticsPerFile EnumerateDiagnosticsPerFile();


    /// <summary>
    /// The number of configuration files in the chain, not counting the built-in configuration.
    /// </summary>
    int LayerCount { get; }
}
