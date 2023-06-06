// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Bicep.Core.Semantics;

namespace Bicep.Core.Emit;

public class ParametersJsonWriter
{
    private readonly SemanticModel model;

    public ParametersJsonWriter(SemanticModel model)
    {
        this.model = model;
    }

    public void Write(JsonTextWriter writer) => GenerateTemplate().WriteTo(writer);

    public JToken GenerateTemplate()
    {
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);

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

            var parameter = model.EmitLimitationInfo.ParameterAssignments[assignment];

            var propertyName = (parameter.First as JProperty)?.Name;
            var isReferenceProperty = propertyName == "reference";
            var isObjectType = assignment.Type.TypeKind == TypeSystem.TypeKind.Object;

            if (parameter.Type == JTokenType.Object)
            {
                if (!isReferenceProperty || (isReferenceProperty && isObjectType))
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("value");
                }
            }
            else
            {
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("value");
            }

            parameter.WriteTo(jsonWriter);

            if (parameter.Type == JTokenType.Object)
            {
                if (!isReferenceProperty || (isReferenceProperty && isObjectType))
                {
                    jsonWriter.WriteEndObject();
                }
            }
            else
            {
                jsonWriter.WriteEndObject();
            }
        }

        jsonWriter.WriteEndObject();

        jsonWriter.WriteEndObject();

        var content = stringWriter.ToString();

        return content.FromJson<JToken>();
    }
}
