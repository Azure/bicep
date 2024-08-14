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
    bool? ExtensionRegistry = default,
    bool? OptionalModuleNamesEnabled = default,
    bool? LocalDeployEnabled = default,
    bool? ResourceDerivedTypesEnabled = default,
    bool? SecureOutputsEnabled = default,
    bool? ExtendableParamFilesEnabled = default,
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
        bool? ExtensionRegistry = default,
        bool? OptionalModuleNamesEnabled = default,
        bool? LocalDeployEnabled = default,
        bool? ResourceDerivedTypesEnabled = default,
        bool? SecureOutputsEnabled = default,
        bool? ExtendableParamFilesEnabled = default,
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
        ExtensionRegistry,
        OptionalModuleNamesEnabled,
        LocalDeployEnabled,
        ResourceDerivedTypesEnabled,
        SecureOutputsEnabled,
        ExtendableParamFilesEnabled,
        AssemblyVersion)
    { }
}
