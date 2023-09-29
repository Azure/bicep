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
    bool? UserDefinedFunctionsEnabled = default,
    bool? PrettyPrintingEnabled = default,
    bool? TestFrameworkEnabled = default,
    bool? AssertsEnabled = default,
    bool? DynamicTypeLoading = default,
    bool? CompileTimeImportsEnabled = default,
    bool? MicrosoftGraphPreviewEnabled = default,
    bool? PublishSourceEnabled = default,
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
        bool? UserDefinedFunctionsEnabled = default,
        bool? PrettyPrintingEnabled = default,
        bool? TestFrameworkEnabled = default,
        bool? AssertsEnabled = default,
        bool? DynamicTypeLoading = default,
        bool? CompileTimeImportsEnabled = default,
        bool? MicrosoftGraphPreviewEnabled = default,
        bool? PublishSourceEnabled = default,
        string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion
    ) : this(
        FileHelper.GetCacheRootPath(testContext),
        RegistryEnabled,
        SymbolicNameCodegenEnabled,
        ExtensibilityEnabled,
        AdvancedListComprehensionEnabled,
        ResourceTypedParamsAndOutputsEnabled,
        SourceMappingEnabled,
        UserDefinedFunctionsEnabled,
        PrettyPrintingEnabled,
        TestFrameworkEnabled,
        AssertsEnabled,
        DynamicTypeLoading,
        CompileTimeImportsEnabled,
        MicrosoftGraphPreviewEnabled,
        PublishSourceEnabled,
        AssemblyVersion)
    { }
}
