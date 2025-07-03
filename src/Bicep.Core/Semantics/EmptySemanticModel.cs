// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Semantics
{
    public class EmptySemanticModel : ISemanticModel
    {
        public BicepSourceFile SourceFile => throw new NotImplementedException();

        public ResourceScope TargetScope => ResourceScope.None;

        public ImmutableSortedDictionary<string, ParameterMetadata> Parameters => ImmutableSortedDictionary<string, ParameterMetadata>.Empty;

        public ImmutableSortedDictionary<string, ExtensionMetadata> Extensions => ImmutableSortedDictionary<string, ExtensionMetadata>.Empty;

        public ImmutableSortedDictionary<string, ExportMetadata> Exports => ImmutableSortedDictionary<string, ExportMetadata>.Empty;

        public ImmutableArray<OutputMetadata> Outputs => [];

        public bool HasErrors() => false;

        public IFeatureProvider Features { get; } = new EmptySemanticModelFeatureProvider(ExperimentalFeaturesEnabled.AllDisabled);

        private class EmptySemanticModelFeatureProvider(ExperimentalFeaturesEnabled features) : IFeatureProvider
        {
            public string AssemblyVersion => throw new NotImplementedException();
            public IDirectoryHandle CacheRootDirectory => throw new NotImplementedException();
            public bool SymbolicNameCodegenEnabled => features.SymbolicNameCodegen;
            public bool ResourceTypedParamsAndOutputsEnabled => features.ResourceTypedParamsAndOutputs;
            public bool SourceMappingEnabled => features.SourceMapping;
            public bool LegacyFormatterEnabled => features.LegacyFormatter;
            public bool TestFrameworkEnabled => features.TestFramework;
            public bool AssertsEnabled => features.Assertions;
            public bool WaitAndRetryEnabled => features.WaitAndRetry;
            public bool OnlyIfNotExistsEnabled => features.OnlyIfNotExists;
            public bool LocalDeployEnabled => features.LocalDeploy;
            public bool ExtendableParamFilesEnabled => features.ExtendableParamFiles;
            public bool ResourceInfoCodegenEnabled => features.ResourceInfoCodegen;
            public bool ModuleExtensionConfigsEnabled => features.ModuleExtensionConfigs;
            public bool DesiredStateConfigurationEnabled => features.DesiredStateConfiguration;
            public bool ExternalInputFunctionEnabled => features.ExternalInputFunction;
            public bool ModuleIdentityEnabled => features.ModuleIdentity;
        }
    }
}

