// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Extensions;
using Bicep.Core.Json;

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
    bool OnlyIfNotExists,
    bool ModuleIdentity)
{
    public static ExperimentalFeaturesEnabled Bind(JsonElement element)
        => element.ToNonNullObject<ExperimentalFeaturesEnabled>();

    public void WriteTo(Utf8JsonWriter writer) => JsonElementFactory.CreateElement(this).WriteTo(writer);
}
