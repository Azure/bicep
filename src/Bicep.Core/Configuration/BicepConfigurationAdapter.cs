// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Adapts the existing <see cref="RootConfiguration"/> to the <see cref="IBicepConfiguration"/> interface.
/// This allows <see cref="BicepConfigurationManager"/> to return <see cref="IBicepConfiguration"/> instances
/// backed by the existing configuration loading infrastructure, without requiring <see cref="RootConfiguration"/>
/// to directly implement the interface.
/// </summary>
internal sealed class BicepConfigurationAdapter : IBicepConfiguration
{
    private readonly RootConfiguration inner;

    public BicepConfigurationAdapter(RootConfiguration inner)
    {
        this.inner = inner;
    }

    internal RootConfiguration InnerConfiguration => this.inner;

    public IBicepCloudConfiguration Cloud => this.inner.Cloud;

    public IBicepModuleAliasesConfiguration ModuleAliases => this.inner.ModuleAliases;

    public IBicepModuleAliasesMockConfiguration ModuleAliasesMock => this.inner.ModuleAliasesMock;

    public IBicepExtensionsConfiguration Extensions => this.inner.Extensions;

    public IBicepImplicitExtensionsConfiguration ImplicitExtensions => this.inner.ImplicitExtensions;

    public IBicepAnalyzersConfiguration Analyzers => this.inner.Analyzers;

    public IBicepFormattingConfiguration Formatting => this.inner.Formatting;

    public ExperimentalFeaturesEnabled ExperimentalFeaturesEnabled => this.inner.ExperimentalFeaturesEnabled;

    public string? CacheRootDirectory => this.inner.CacheRootDirectory;

    public bool ExperimentalFeaturesWarning => this.inner.ExperimentalFeaturesWarning;

    public IOUri? ConfigFileUri => this.inner.ConfigFileUri;

    public bool IsBuiltIn => this.inner.IsBuiltIn;

    public IEnumerable<IDiagnostic> GetDiagnostics() => this.inner.Diagnostics;
}
