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
    bool ModuleIdentity) : IFeatureProvider
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

    public string AssemblyVersion => throw new NotImplementedException();
    public IDirectoryHandle CacheRootDirectory => throw new NotImplementedException();
    public bool SymbolicNameCodegenEnabled { get; } = SymbolicNameCodegen;
    public bool ResourceTypedParamsAndOutputsEnabled { get; } = ResourceTypedParamsAndOutputs;
    public bool SourceMappingEnabled { get; } = SourceMapping;
    public bool LegacyFormatterEnabled { get; } = LegacyFormatter;
    public bool TestFrameworkEnabled { get; } = TestFramework;
    public bool AssertsEnabled { get; } = Assertions;
    public bool WaitAndRetryEnabled { get; } = WaitAndRetry;
    public bool OnlyIfNotExistsEnabled { get; } = OnlyIfNotExists;
    public bool LocalDeployEnabled { get; } = LocalDeploy;
    public bool ExtendableParamFilesEnabled { get; } = ExtendableParamFiles;
    public bool ResourceInfoCodegenEnabled { get; } = ResourceInfoCodegen;
    public bool ModuleExtensionConfigsEnabled { get; } = ModuleExtensionConfigs;
    public bool DesiredStateConfigurationEnabled { get; } = DesiredStateConfiguration;
    public bool ExternalInputFunctionEnabled { get; } = ExternalInputFunction;
    public bool ModuleIdentityEnabled { get; } = ModuleIdentity;
}
