// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

[method: JsonConstructor]
public record ExperimentalFeaturesEnabled(
    bool OciEnabled,
    bool SymbolicNameCodegen,
    bool ResourceTypedParamsAndOutputs,
    bool SourceMapping,
    bool LegacyFormatter,
    bool TestFramework,
    bool Assertions,
    bool WaitUntil,
    bool LocalDeploy,
    bool ResourceInfoCodegen,
    bool ModuleExtensionConfigs,
    bool UserDefinedConstraints,
    bool DeployCommands,
    bool Patch,
    bool RuntimeValuesInTagsAndSku,
    bool AzExtensionConfig,
    bool DocsGeneration)
{
    public ExperimentalFeaturesEnabled(
        bool OciEnabled,
        bool SymbolicNameCodegen,
        bool ResourceTypedParamsAndOutputs,
        bool SourceMapping,
        bool LegacyFormatter,
        bool TestFramework,
        bool Assertions,
        bool WaitUntil,
        bool LocalDeploy,
        bool ResourceInfoCodegen,
        bool ModuleExtensionConfigs,
        bool UserDefinedConstraints,
        bool DeployCommands,
        bool Patch,
        bool RuntimeValuesInTagsAndSku,
        bool AzExtensionConfig)
        : this(
            OciEnabled,
            SymbolicNameCodegen,
            ResourceTypedParamsAndOutputs,
            SourceMapping,
            LegacyFormatter,
            TestFramework,
            Assertions,
            WaitUntil,
            LocalDeploy,
            ResourceInfoCodegen,
            ModuleExtensionConfigs,
            UserDefinedConstraints,
            DeployCommands,
            Patch,
            RuntimeValuesInTagsAndSku,
            AzExtensionConfig,
            DocsGeneration: false)
    {
    }

    public static ExperimentalFeaturesEnabled Bind(JsonElement element)
        => element.ToNonNullObject<ExperimentalFeaturesEnabled>();

    public void WriteTo(Utf8JsonWriter writer) => JsonElementFactory.CreateElement(this).WriteTo(writer);

    public static readonly ExperimentalFeaturesEnabled AllDisabled = new(
        OciEnabled: false,
        SymbolicNameCodegen: false,
        ResourceTypedParamsAndOutputs: false,
        SourceMapping: false,
        LegacyFormatter: false,
        TestFramework: false,
        Assertions: false,
        WaitUntil: false,
        LocalDeploy: false,
        ResourceInfoCodegen: false,
        ModuleExtensionConfigs: false,
        UserDefinedConstraints: false,
        DeployCommands: false,
        Patch: false,
        RuntimeValuesInTagsAndSku: false,
        AzExtensionConfig: false,
        DocsGeneration: false);
}
