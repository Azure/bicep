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

    public bool SymbolicNameCodegenEnabled => overrides.SymbolicNameCodegenEnabled ?? features.SymbolicNameCodegenEnabled;

    public bool ExtensibilityEnabled => overrides.ExtensibilityEnabled ?? features.ExtensibilityEnabled;

    public bool ResourceTypedParamsAndOutputsEnabled => overrides.ResourceTypedParamsAndOutputsEnabled ?? features.ResourceTypedParamsAndOutputsEnabled;

    public bool SourceMappingEnabled => overrides.SourceMappingEnabled ?? features.SourceMappingEnabled;

    public bool UserDefinedFunctionsEnabled => overrides.UserDefinedFunctionsEnabled ?? features.UserDefinedFunctionsEnabled;

    public bool PrettyPrintingEnabled => overrides.PrettyPrintingEnabled ?? features.PrettyPrintingEnabled;

    public bool TestFrameworkEnabled => overrides.TestFrameworkEnabled ?? features.TestFrameworkEnabled;

    public bool AssertsEnabled => overrides.AssertsEnabled ?? features.AssertsEnabled;

    public bool DynamicTypeLoadingEnabled => overrides.DynamicTypeLoading ?? features.DynamicTypeLoadingEnabled;

    public bool CompileTimeImportsEnabled => overrides.CompileTimeImportsEnabled ?? features.CompileTimeImportsEnabled;

    public bool MicrosoftGraphPreviewEnabled => overrides.MicrosoftGraphPreviewEnabled ?? features.MicrosoftGraphPreviewEnabled;

    public bool PublishSourceEnabled => overrides.PublishSourceEnabled ?? features.PublishSourceEnabled;
}
