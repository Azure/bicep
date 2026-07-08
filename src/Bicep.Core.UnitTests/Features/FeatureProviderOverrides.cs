// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Features;

public record FeatureProviderOverrides(
    IDirectoryHandle? CacheRootDirectory = null,
    bool? RegistryEnabled = default,
    bool? OciEnabled = default,
    bool? SymbolicNameCodegenEnabled = default,
    bool? AdvancedListComprehensionEnabled = default,
    bool? ResourceTypedParamsAndOutputsEnabled = default,
    bool? SourceMappingEnabled = default,
    bool? LegacyFormatterEnabled = default,
    bool? TestFrameworkEnabled = default,
    bool? AssertsEnabled = default,
    bool? WaitUntilEnabled = default,
    bool? LocalDeployEnabled = default,
    bool? ResourceInfoCodegenEnabled = default,
    string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion,
    bool? ModuleExtensionConfigsEnabled = default,
    bool? UserDefinedConstraintsEnabled = default,
    bool? DeployCommandsEnabled = default)
{
    public FeatureProviderOverrides(
        TestContext testContext,
        bool? RegistryEnabled = default,
        bool? OciEnabled = default,
        bool? SymbolicNameCodegenEnabled = default,
        bool? AdvancedListComprehensionEnabled = default,
        bool? ResourceTypedParamsAndOutputsEnabled = default,
        bool? SourceMappingEnabled = default,
        bool? LegacyFormatterEnabled = default,
        bool? TestFrameworkEnabled = default,
        bool? AssertsEnabled = default,
        bool? WaitUntilEnabled = default,
        bool? LocalDeployEnabled = default,
        bool? ResourceInfoCodegenEnabled = default,
        string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion,
        bool? ModuleExtensionConfigsEnabled = default,
        bool? UserDefinedConstraintsEnabled = default,
        bool? DeployCommandsEnabled = default) : this(
            FileHelper.GetCacheRootDirectory(testContext),
            RegistryEnabled,
            OciEnabled,
            SymbolicNameCodegenEnabled,
            AdvancedListComprehensionEnabled,
            ResourceTypedParamsAndOutputsEnabled,
            SourceMappingEnabled,
            LegacyFormatterEnabled,
            TestFrameworkEnabled,
            AssertsEnabled,
            WaitUntilEnabled,
            LocalDeployEnabled,
            ResourceInfoCodegenEnabled,
            AssemblyVersion,
            ModuleExtensionConfigsEnabled,
            UserDefinedConstraintsEnabled,
            DeployCommandsEnabled)
    { }
}
