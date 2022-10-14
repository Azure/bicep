// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Features;

public record FeatureProviderOverrides(
    string? CacheRootDirectory = null,
    bool? RegistryEnabled = default,
    bool? SymbolicNameCodegenEnabled = default,
    bool? ImportsEnabled = default,
    bool? AdvancedListComprehensionEnabled = default,
    bool? ResourceTypedParamsAndOutputsEnabled = default,
    bool? SourceMappingEnabled = default,
    bool? ParamsFilesEnabled = default,
    bool? AggregateTypesEnabled = default,
    string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion)
{
    public FeatureProviderOverrides(TestContext testContext,
        bool? RegistryEnabled = default,
        bool? SymbolicNameCodegenEnabled = default,
        bool? ImportsEnabled = default,
        bool? AdvancedListComprehensionEnabled = default,
        bool? ResourceTypedParamsAndOutputsEnabled = default,
        bool? SourceMappingEnabled = default,
        bool? ParamsFilesEnabled = default,
        bool? AggregateTypesEnabled = default,
        string? AssemblyVersion = BicepTestConstants.DevAssemblyFileVersion
    ) : this(FileHelper.GetCacheRootPath(testContext), RegistryEnabled, SymbolicNameCodegenEnabled, ImportsEnabled, AdvancedListComprehensionEnabled, ResourceTypedParamsAndOutputsEnabled, SourceMappingEnabled, ParamsFilesEnabled, AggregateTypesEnabled, AssemblyVersion) {}
}