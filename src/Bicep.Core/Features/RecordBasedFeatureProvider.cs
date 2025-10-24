// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Features
{
    public class RecordBasedFeatureProvider(ExperimentalFeaturesEnabled features) : IFeatureProvider
    {
        public static IFeatureProvider AllDisabled { get; } = new RecordBasedFeatureProvider(ExperimentalFeaturesEnabled.AllDisabled);

        public string AssemblyVersion => throw new NotImplementedException();
        public IDirectoryHandle CacheRootDirectory => throw new NotImplementedException();
        public bool SymbolicNameCodegenEnabled => features.SymbolicNameCodegen;
        public bool ResourceTypedParamsAndOutputsEnabled => features.ResourceTypedParamsAndOutputs;
        public bool SourceMappingEnabled => features.SourceMapping;
        public bool LegacyFormatterEnabled => features.LegacyFormatter;
        public bool TestFrameworkEnabled => features.TestFramework;
        public bool AssertsEnabled => features.Assertions;
        public bool WaitAndRetryEnabled => features.WaitAndRetry;
        public bool LocalDeployEnabled => features.LocalDeploy;
        public bool ExtendableParamFilesEnabled => features.ExtendableParamFiles;
        public bool ResourceInfoCodegenEnabled => features.ResourceInfoCodegen;
        public bool ModuleExtensionConfigsEnabled => features.ModuleExtensionConfigs;
        public bool DesiredStateConfigurationEnabled => features.DesiredStateConfiguration;
        public bool UserDefinedConstraintsEnabled => features.UserDefinedConstraints;
        public bool DeployCommandsEnabled => features.DeployCommands;
        public bool ThisNamespaceEnabled => features.ThisNamespace;
    }
}
