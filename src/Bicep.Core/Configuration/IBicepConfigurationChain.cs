// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration;

/// <summary>
/// Represents a chain of Bicep configuration files linked via "extends".
/// The chain goes from the leaf (most specific) file up to the built-in defaults.
/// </summary>
public interface IBicepConfigurationChain
{
    /// <summary>
    /// Returns the effective configuration produced by merging all layers in the chain,
    /// with leaf settings winning over base settings.
    /// </summary>
    IBicepConfiguration GetEffectiveConfiguration();

    /// <summary>
    /// Returns diagnostics grouped by the config file URI they originated from.
    /// Only user-defined config files are included (built-in defaults are excluded).
    /// </summary>
    DiagnosticsPerFile GetDiagnosticsByConfigFile();

    /// <summary>
    /// The number of configuration files in the chain.
    /// </summary>
    int LayerCount { get; }
}
