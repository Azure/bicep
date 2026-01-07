// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
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

    public bool ResourceTypedParamsAndOutputsEnabled => overrides.ResourceTypedParamsAndOutputsEnabled ?? features.ResourceTypedParamsAndOutputsEnabled;

    public bool SourceMappingEnabled => overrides.SourceMappingEnabled ?? features.SourceMappingEnabled;

    public bool LegacyFormatterEnabled => overrides.LegacyFormatterEnabled ?? features.LegacyFormatterEnabled;

    public bool TestFrameworkEnabled => overrides.TestFrameworkEnabled ?? features.TestFrameworkEnabled;

    public bool AssertsEnabled => overrides.AssertsEnabled ?? features.AssertsEnabled;

    public bool WaitAndRetryEnabled => overrides.WaitAndRetryEnabled ?? features.WaitAndRetryEnabled;

    public bool LocalDeployEnabled => overrides.LocalDeployEnabled ?? features.LocalDeployEnabled;

    public bool ResourceInfoCodegenEnabled => overrides.ResourceInfoCodegenEnabled ?? features.ResourceInfoCodegenEnabled;

    public bool ExtendableParamFilesEnabled => overrides.ExtendableParamFilesEnabled ?? features.ExtendableParamFilesEnabled;

    public bool ModuleExtensionConfigsEnabled => overrides.ModuleExtensionConfigsEnabled ?? features.ModuleExtensionConfigsEnabled;

    public bool DesiredStateConfigurationEnabled => overrides.DesiredStateConfigurationEnabled ?? features.DesiredStateConfigurationEnabled;

    public bool UserDefinedConstraintsEnabled => overrides.UserDefinedConstraintsEnabled ?? features.UserDefinedConstraintsEnabled;

    public bool DeployCommandsEnabled => overrides.DeployCommandsEnabled ?? features.DeployCommandsEnabled;

    public bool ThisNamespaceEnabled => overrides.ThisNamespaceEnabled ?? features.ThisNamespaceEnabled;
}
