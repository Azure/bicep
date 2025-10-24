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
    bool UserDefinedConstraints,
    bool DeployCommands,
    bool ThisNamespace)
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
        UserDefinedConstraints: false,
        DeployCommands: false,
        ThisNamespace: false);
}
