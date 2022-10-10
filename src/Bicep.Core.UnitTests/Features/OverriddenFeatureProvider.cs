// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;

namespace Bicep.Core.UnitTests.Features;

public class OverriddenFeatureProvider : IFeatureProvider
{
    private readonly IFeatureProvider features;
    private readonly FeatureProviderOverrides overrides;

    public OverriddenFeatureProvider(IFeatureProvider features, FeatureProviderOverrides overrides)
    {
        this.features = features;
        this.overrides = overrides;
    }

    public string AssemblyVersion => overrides.AssemblyVersion ?? features.AssemblyVersion;

    public string CacheRootDirectory => overrides.CacheRootDirectory ?? features.CacheRootDirectory;

    public bool RegistryEnabled => overrides.RegistryEnabled ?? features.RegistryEnabled;

    public bool SymbolicNameCodegenEnabled => overrides.SymbolicNameCodegenEnabled ?? features.SymbolicNameCodegenEnabled;

    public bool ImportsEnabled => overrides.ImportsEnabled ?? features.ImportsEnabled;

    public bool ResourceTypedParamsAndOutputsEnabled => overrides.ResourceTypedParamsAndOutputsEnabled ?? features.ResourceTypedParamsAndOutputsEnabled;

    public bool SourceMappingEnabled => overrides.SourceMappingEnabled ?? features.SourceMappingEnabled;

    public bool ParamsFilesEnabled => overrides.ParamsFilesEnabled ?? features.ParamsFilesEnabled;
}