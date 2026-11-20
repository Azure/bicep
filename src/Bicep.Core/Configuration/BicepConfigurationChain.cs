// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Represents a chain of Bicep configuration files linked via "extends",
/// ordered from the leaf (the most specific file, e.g. the nearest bicepconfig.json) to its most distant base.
/// </summary>
public class BicepConfigurationChain : IBicepConfigurationChain
{
    private readonly IBicepConfiguration effectiveConfiguration;

    private readonly ImmutableArray<IOUri> configFileUris;

    private readonly ImmutableArray<ConfigurationDiagnostic> diagnostics;

    /// <param name="configuration">
    ///   The configuration produced by merging the chain, or the built-in configuration if the chain couldn't be loaded.
    /// </param>
    /// <param name="configFileUris">The configuration files in the chain that were read, leaf first.</param>
    /// <param name="diagnostics">Every diagnostic for the chain, with the file each belongs to.</param>
    public BicepConfigurationChain(
        IBicepConfiguration configuration,
        ImmutableArray<IOUri> configFileUris,
        ImmutableArray<ConfigurationDiagnostic> diagnostics)
    {
        this.effectiveConfiguration = diagnostics.IsEmpty
            ? configuration
            : configuration.With(diagnostics: diagnostics.Select(diagnostic => diagnostic.Diagnostic));
        this.configFileUris = configFileUris;
        this.diagnostics = diagnostics;
    }

    /// <summary>
    /// Returns the fully merged effective configuration. Its <see cref="IBicepConfiguration.GetDiagnostics"/> returns
    /// every diagnostic for the chain, whichever file it belongs to.
    /// </summary>
    public IBicepConfiguration GetEffectiveConfiguration() => this.effectiveConfiguration;

    /// <summary>
    /// Returns the diagnostics for each configuration file in the chain, including files without any,
    /// and files that couldn't be read. Diagnostics that don't belong to a file (for example, an invalid environment variable)
    /// are only returned by <see cref="GetEffectiveConfiguration"/>.
    /// </summary>
    public DiagnosticsPerFile EnumerateDiagnosticsPerFile()
        => this.configFileUris
            .Concat(this.diagnostics.Select(diagnostic => diagnostic.File).WhereNotNull())
            .Distinct()
            .Select(fileUri => new KeyValuePair<IOUri, ImmutableArray<IDiagnostic>>(
                fileUri,
                [.. this.diagnostics.Where(diagnostic => fileUri.Equals(diagnostic.File)).Select(diagnostic => diagnostic.Diagnostic)]));


    /// <summary>
    /// The number of configuration files in the chain that were read, not counting the built-in configuration.
    /// </summary>
    public int LayerCount => this.configFileUris.Length;
}
