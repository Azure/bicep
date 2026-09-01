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
///
/// The primary purpose of keeping the full chain — rather than just the merged result —
/// is to support accurate diagnostics: each <see cref="IBicepConfiguration"/> in the chain
/// carries its own diagnostics, so errors can be
/// attributed to the exact file that caused them.
///
/// </summary>
public class BicepConfigurationChain : IBicepConfigurationChain
{
    // The fully merged effective configuration constructed by BicepConfigurationManager.
    private readonly IBicepConfiguration effectiveConfiguration;

    // All individual configurations in the chain, ordered leaf-first.
    // Each carries its own diagnostics, enabling per-file error attribution.
    private readonly ImmutableArray<IBicepConfiguration> layers;

    // Lazily computed aggregation of diagnostics from all layers.
    private ImmutableArray<IDiagnostic>? aggregatedDiagnostics;

    /// <param name="effectiveConfiguration">
    ///   The pre-merged configuration representing the combined settings of all layers.
    ///   Produced by <c>BicepConfigurationManager</c>.
    /// </param>
    /// <param name="layers">
    ///   Ordered collection of individual configurations, leaf first, built-in last.
    ///   Used for diagnostic provenance tracking.
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
    /// The returned configuration's <see cref="IBicepConfiguration.GetDiagnostics"/>
    /// aggregates diagnostics from every layer in the chain.
    /// </summary>
    public IBicepConfiguration GetEffectiveConfiguration() => this.effectiveConfiguration;

    /// <summary>
    /// Returns all diagnostics from every configuration file in the chain.
    /// Each diagnostic already carries its source file URI via its message context,
    /// so callers can attribute errors to the exact file that caused them.
    /// </summary>
    public IEnumerable<IDiagnostic> GetAllDiagnostics()
    {
        if (this.aggregatedDiagnostics is null)
        {
            this.aggregatedDiagnostics = this.layers
                .SelectMany(layer => layer.GetDiagnostics())
                .ToImmutableArray();
        }

        return this.aggregatedDiagnostics.Value;
    }

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
