// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Features;

public record FeatureProviderOverrides(
    string? CacheRootDirectory = null,
    bool? RegistryEnabled = default,
    bool? SymbolicNameCodegenEnabled = default,
    bool? ExtensibilityEnabled = default,
    bool? AdvancedListComprehensionEnabled = default,
    bool? ResourceTypedParamsAndOutputsEnabled = default,
    bool? SourceMappingEnabled = default,
    bool? LegacyFormatterEnabled = default,
    bool? TestFrameworkEnabled = default,
    bool? AssertsEnabled = default,
    bool? DynamicTypeLoadingEnabled = default,
    bool? ProviderRegistry = default,
    bool? OptionalModuleNamesEnabled = default,
    bool? LocalDeployEnabled = default,
    bool? ResourceDerivedTypesEnabled = default,
    string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion)
{
    public FeatureProviderOverrides(
        TestContext testContext,
        bool? RegistryEnabled = default,
        bool? SymbolicNameCodegenEnabled = default,
        bool? ExtensibilityEnabled = default,
        bool? AdvancedListComprehensionEnabled = default,
        bool? ResourceTypedParamsAndOutputsEnabled = default,
        bool? SourceMappingEnabled = default,
        bool? LegacyFormatterEnabled = default,
        bool? TestFrameworkEnabled = default,
        bool? AssertsEnabled = default,
        bool? DynamicTypeLoadingEnabled = default,
        bool? ProviderRegistry = default,
        bool? OptionalModuleNamesEnabled = default,
        bool? LocalDeployEnabled = default,
        bool? ResourceDerivedTypesEnabled = default,
        string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion
    ) : this(
        FileHelper.GetCacheRootPath(testContext),
        RegistryEnabled,
        SymbolicNameCodegenEnabled,
        ExtensibilityEnabled,
        AdvancedListComprehensionEnabled,
        ResourceTypedParamsAndOutputsEnabled,
        SourceMappingEnabled,
        LegacyFormatterEnabled,
        TestFrameworkEnabled,
        AssertsEnabled,
        DynamicTypeLoadingEnabled,
        ProviderRegistry,
        OptionalModuleNamesEnabled,
        LocalDeployEnabled,
        ResourceDerivedTypesEnabled,
        AssemblyVersion)
    { }
}
