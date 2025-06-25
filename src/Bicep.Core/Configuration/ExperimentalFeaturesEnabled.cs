// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

public record ExperimentalFeaturesEnabled(
    bool SymbolicNameCodegen,
    bool ExtendableParamFiles,
    bool ResourceTypedParamsAndOutputs,
    bool SourceMapping,
    bool LegacyFormatter,
    bool TestFramework,
    bool Assertions,
    bool WaitAndRetry,
    bool LocalDeploy,
    bool ResourceInfoCodegen,
    bool ModuleExtensionConfigs,
    bool DesiredStateConfiguration,
    bool ExternalInputFunction,
    bool OnlyIfNotExists,
    bool ModuleIdentity)
{
    public static ExperimentalFeaturesEnabled Bind(JsonElement element)
        => element.ToNonNullObject<ExperimentalFeaturesEnabled>();

    public void WriteTo(Utf8JsonWriter writer) => JsonElementFactory.CreateElement(this).WriteTo(writer);

    public static readonly ExperimentalFeaturesEnabled AllDisabled = new(
        SymbolicNameCodegen: false,
        ExtendableParamFiles: false,
        ResourceTypedParamsAndOutputs: false,
        SourceMapping: false,
        LegacyFormatter: false,
        TestFramework: false,
        Assertions: false,
        WaitAndRetry: false,
        LocalDeploy: false,
        ResourceInfoCodegen: false,
        ModuleExtensionConfigs: false,
        DesiredStateConfiguration: false,
        ExternalInputFunction: false,
        OnlyIfNotExists: false,
        ModuleIdentity: false);

    public IFeatureProvider ToFeatureProvider() => new FeatureProviderAdapter(this);

    private class FeatureProviderAdapter : IFeatureProvider
    {
        private readonly ExperimentalFeaturesEnabled features;

        public FeatureProviderAdapter(ExperimentalFeaturesEnabled features)
        {
            this.features = features;
        }

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
