// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Testing;

public static class TestFeatureProviderFactory
{
    public static IFeatureProviderFactory WithAssemblyVersion(IFeatureProviderFactory factory, string assemblyVersion) =>
        new AssemblyVersionFeatureProviderFactory(factory, assemblyVersion);

    private sealed class AssemblyVersionFeatureProviderFactory(IFeatureProviderFactory factory, string assemblyVersion) : IFeatureProviderFactory
    {
        public IFeatureProvider GetFeatureProvider(IOUri sourceFileUri) =>
            new AssemblyVersionFeatureProvider(factory.GetFeatureProvider(sourceFileUri), assemblyVersion);
    }

    private sealed class AssemblyVersionFeatureProvider(IFeatureProvider features, string assemblyVersion) : IFeatureProvider
    {
        public string AssemblyVersion => assemblyVersion;

        public IDirectoryHandle CacheRootDirectory => features.CacheRootDirectory;

        public bool OciEnabled => features.OciEnabled;

        public bool SymbolicNameCodegenEnabled => features.SymbolicNameCodegenEnabled;

        public bool ResourceTypedParamsAndOutputsEnabled => features.ResourceTypedParamsAndOutputsEnabled;

        public bool SourceMappingEnabled => features.SourceMappingEnabled;

        public bool LegacyFormatterEnabled => features.LegacyFormatterEnabled;

        public bool TestFrameworkEnabled => features.TestFrameworkEnabled;

        public bool AssertsEnabled => features.AssertsEnabled;

        public bool WaitUntilEnabled => features.WaitUntilEnabled;

        public bool LocalDeployEnabled => features.LocalDeployEnabled;

        public bool ResourceInfoCodegenEnabled => features.ResourceInfoCodegenEnabled;

        public bool ModuleExtensionConfigsEnabled => features.ModuleExtensionConfigsEnabled;

        public bool UserDefinedConstraintsEnabled => features.UserDefinedConstraintsEnabled;

        public bool DeployCommandsEnabled => features.DeployCommandsEnabled;

        public bool PatchEnabled => features.PatchEnabled;

        public bool RuntimeValuesInTagsAndSkuEnabled => features.RuntimeValuesInTagsAndSkuEnabled;

        public bool AzExtensionConfigEnabled => features.AzExtensionConfigEnabled;

    }
}
