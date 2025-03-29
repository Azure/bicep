// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Core.Emit;

public class ParametersJsonWriter
{
    private readonly SemanticModel model;

    public ParametersJsonWriter(SemanticModel model)
    {
        this.model = model;
    }

    public void Write(JsonTextWriter jsonWriter)
    {
        jsonWriter.WriteStartObject();

        jsonWriter.WritePropertyName("$schema");
        jsonWriter.WriteValue("https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");

        jsonWriter.WritePropertyName("contentVersion");
        jsonWriter.WriteValue("1.0.0.0");

        jsonWriter.WritePropertyName("parameters");
        jsonWriter.WriteStartObject();

        foreach (var assignment in model.Root.ParameterAssignments)
        {
            jsonWriter.WritePropertyName(assignment.Name);
            jsonWriter.WriteStartObject();

            var parameter = model.EmitLimitationInfo.ParameterAssignments[assignment];

            if (parameter.KeyVaultReferenceExpression is { } keyVaultReference)
            {
                WriteKeyVaultReference(jsonWriter, keyVaultReference, "reference");
            }
            else if (parameter.Value is { } value)
            {
                jsonWriter.WritePropertyName("value");
                value.WriteTo(jsonWriter);
            }
            else
            {
                throw new InvalidOperationException($"The '{assignment.Name}' parameter assignment defined neither a concrete value nor a key vault reference");
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();

        if (model.Features is { ExtensibilityEnabled: true, ModuleExtensionConfigsEnabled: true })
        {
            WriteExtensionConfigs(jsonWriter);
        }

        jsonWriter.WriteEndObject();
    }

    private void WriteExtensionConfigs(JsonTextWriter jsonWriter)
    {
        jsonWriter.WritePropertyName("extensionConfigs");
        jsonWriter.WriteStartObject();

        foreach (var extension in model.Root.ExtensionConfigAssignments)
        {
            jsonWriter.WritePropertyName(extension.Name);
            jsonWriter.WriteStartObject();

            var configProperties = model.EmitLimitationInfo.ExtensionConfigAssignments[extension];

            foreach (var configProperty in configProperties)
            {
                jsonWriter.WritePropertyName(configProperty.Key);

                if (configProperty.Value.KeyVaultReferenceExpression is { } keyVaultReference)
                {
                    WriteKeyVaultReference(jsonWriter, keyVaultReference, "keyVaultReference");
                }
                else if (configProperty.Value.Value is { } value)
                {
                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("value");
                    value.WriteTo(jsonWriter);

                    jsonWriter.WriteEndObject();
                }
                else
                {
                    throw new InvalidOperationException($"The '{configProperty.Key}' property of the '{extension.Name}' extension config assignment defined neither a concrete value nor a key vault reference");
                }
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
    }

    private static void WriteKeyVaultReference(JsonWriter jsonWriter, ParameterKeyVaultReferenceExpression expression, string referencePropertyName)
    {
        jsonWriter.WritePropertyName(referencePropertyName);
        jsonWriter.WriteStartObject();

        jsonWriter.WritePropertyName("keyVault");
        jsonWriter.WriteStartObject();

        jsonWriter.WritePropertyName("id");
        jsonWriter.WriteValue(expression.KeyVaultId);

        jsonWriter.WriteEndObject();

        jsonWriter.WritePropertyName("secretName");
        jsonWriter.WriteValue(expression.SecretName);

        if (expression.SecretVersion is string secretVersion)
        {
            jsonWriter.WritePropertyName("secretVersion");
            jsonWriter.WriteValue(secretVersion);
        }

        jsonWriter.WriteEndObject();
    }
}
