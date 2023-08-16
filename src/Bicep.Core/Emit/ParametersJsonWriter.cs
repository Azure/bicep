// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            if (parameter.KeyVaultReferenceExpression is {} keyVaultReference)
            {
                WriteKeyVaultReference(jsonWriter, keyVaultReference);
            }
            else if (parameter.Value is {} value)
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

        jsonWriter.WriteEndObject();
    }

    private static void WriteKeyVaultReference(JsonWriter jsonWriter, ParameterKeyVaultReferenceExpression expression)
    {
        jsonWriter.WritePropertyName("reference");
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
