// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Features;

public record FeatureProviderOverrides(
    IDirectoryHandle? CacheRootDirectory = null,
    bool? RegistryEnabled = default,
    bool? SymbolicNameCodegenEnabled = default,
    bool? ExtensibilityEnabled = default,
    bool? AdvancedListComprehensionEnabled = default,
    bool? ResourceTypedParamsAndOutputsEnabled = default,
    bool? SourceMappingEnabled = default,
    bool? LegacyFormatterEnabled = default,
    bool? TestFrameworkEnabled = default,
    bool? AssertsEnabled = default,
    bool? OptionalModuleNamesEnabled = default,
    bool? LocalDeployEnabled = default,
    bool? ResourceDerivedTypesEnabled = default,
    bool? SecureOutputsEnabled = default,
    bool? ExtendableParamFilesEnabled = default,
    string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion,
    bool? ExtensibilityV2EmittingEnabled = default)
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
        bool? OptionalModuleNamesEnabled = default,
        bool? LocalDeployEnabled = default,
        bool? ResourceDerivedTypesEnabled = default,
        bool? SecureOutputsEnabled = default,
        bool? ExtendableParamFilesEnabled = default,
        string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion,
        bool? ExtensibilityV2EmittingEnabled = default
    ) : this(
        FileHelper.GetCacheRootDirectory(testContext),
        RegistryEnabled,
        SymbolicNameCodegenEnabled,
        ExtensibilityEnabled,
        AdvancedListComprehensionEnabled,
        ResourceTypedParamsAndOutputsEnabled,
        SourceMappingEnabled,
        LegacyFormatterEnabled,
        TestFrameworkEnabled,
        AssertsEnabled,
        OptionalModuleNamesEnabled,
        LocalDeployEnabled,
        ResourceDerivedTypesEnabled,
        SecureOutputsEnabled,
        ExtendableParamFilesEnabled,
        AssemblyVersion,
        ExtensibilityV2EmittingEnabled)
    { }
}
