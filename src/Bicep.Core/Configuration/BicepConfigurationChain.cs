// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Represents a chain of Bicep configuration files linked via "extends".
/// The chain is ordered from the leaf (most specific, e.g. closest bicepconfig.json)
/// to the built-in defaults (last).
/// </summary>
public class BicepConfigurationChain : IBicepConfigurationChain
{
    // The fully merged effective configuration constructed by BicepConfigurationManager.
    private readonly IBicepConfiguration effectiveConfiguration;

    // One entry per configuration file in the chain, ordered leaf-first.
    private readonly ImmutableArray<IBicepConfiguration> layers;

    /// <param name="effectiveConfiguration">
    ///   The pre-merged configuration representing the combined settings of all layers.
    ///   Produced by <c>BicepConfigurationManager</c>.
    /// </param>
    /// <param name="layers">
    ///   One entry per configuration file in the chain, leaf first. Each entry identifies its file through
    ///   <see cref="IBicepConfiguration.ConfigFileUri"/>.
    /// </param>
    public BicepConfigurationChain(
        IBicepConfiguration effectiveConfiguration,
        IEnumerable<IBicepConfiguration> layers)
    {
        this.effectiveConfiguration = effectiveConfiguration;
        this.layers = [.. layers];
    }

    /// <summary>
    /// Returns the fully merged effective configuration.
    /// Errors that prevented the chain from loading are reported by its <see cref="IBicepConfiguration.GetDiagnostics"/>.
    /// </summary>
    public IBicepConfiguration GetEffectiveConfiguration() => this.effectiveConfiguration;

    /// <summary>
    /// Returns diagnostics grouped by the config file URI they originated from.
    /// Built-in default layers (no <see cref="IBicepConfiguration.ConfigFileUri"/>) are excluded.
    /// </summary>
    public DiagnosticsPerFile EnumerateDiagnosticsPerFile()
        => this.layers
            .Where(layer => !layer.IsBuiltIn)
            .Select(layer => new KeyValuePair<IOUri, ImmutableArray<IDiagnostic>>(
                layer.ConfigFileUri!,
                layer.GetDiagnostics().ToImmutableArray()));

    /// <summary>
    /// The number of configuration files in the chain, including the built-in defaults.
    /// </summary>
    public int LayerCount => this.layers.Length;
}
