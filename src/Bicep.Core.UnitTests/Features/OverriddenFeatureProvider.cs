// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.IO.Abstraction;

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

    public IDirectoryHandle CacheRootDirectory => overrides.CacheRootDirectory ?? features.CacheRootDirectory;

    public bool SymbolicNameCodegenEnabled => overrides.SymbolicNameCodegenEnabled ?? features.SymbolicNameCodegenEnabled;

    public bool ExtensibilityEnabled => overrides.ExtensibilityEnabled ?? features.ExtensibilityEnabled;

    public bool ResourceTypedParamsAndOutputsEnabled => overrides.ResourceTypedParamsAndOutputsEnabled ?? features.ResourceTypedParamsAndOutputsEnabled;

    public bool SourceMappingEnabled => overrides.SourceMappingEnabled ?? features.SourceMappingEnabled;

    public bool LegacyFormatterEnabled => overrides.LegacyFormatterEnabled ?? features.LegacyFormatterEnabled;

    public bool TestFrameworkEnabled => overrides.TestFrameworkEnabled ?? features.TestFrameworkEnabled;

    public bool AssertsEnabled => overrides.AssertsEnabled ?? features.AssertsEnabled;

    public bool OptionalModuleNamesEnabled => overrides.OptionalModuleNamesEnabled ?? features.OptionalModuleNamesEnabled;

    public bool WaitAndRetryEnabled => overrides.WaitAndRetryEnabled ?? features.WaitAndRetryEnabled;

    public bool LocalDeployEnabled => overrides.LocalDeployEnabled ?? features.LocalDeployEnabled;

    public bool ResourceDerivedTypesEnabled => overrides.ResourceDerivedTypesEnabled ?? features.ResourceDerivedTypesEnabled;

    public bool SecureOutputsEnabled => overrides.SecureOutputsEnabled ?? features.SecureOutputsEnabled;

    public bool ResourceInfoCodegenEnabled => overrides.ResourceInfoCodegenEnabled ?? features.ResourceInfoCodegenEnabled;

    public bool ExtendableParamFilesEnabled => overrides.ExtendableParamFilesEnabled ?? features.ExtendableParamFilesEnabled;

    public bool ExtensibilityV2EmittingEnabled => overrides.ExtensibilityV2EmittingEnabled ?? features.ExtensibilityV2EmittingEnabled;
}
