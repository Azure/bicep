// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Emit.Options;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class PlaceholderParametersJsonWriter
    {
        public PlaceholderParametersJsonWriter(SemanticModel semanticModel, IncludeParamsOption includeParams)
        {
            this.Context = new EmitterContext(semanticModel);
            this.IncludeParams = includeParams;
        }

        private EmitterContext Context { get; }

        private IncludeParamsOption IncludeParams { get; }

        public void Write(JsonTextWriter writer, string existingContent)
        {
            var existingParamsContent = null as JToken;
            var existingContentVersion = "1.0.0.0";
            if (!string.IsNullOrWhiteSpace(existingContent))
            {
                try
                {
                    existingParamsContent = JToken.Parse(existingContent);
                    existingContentVersion = existingParamsContent.Value<string>("contentVersion") ?? existingContentVersion;
                }
                catch
                {
                    // content of the existing file is not valid json, ignore merging it
                }
            }

            // Template is used for calculating template hash, template jtoken is used for writing to file.
            var templateJToken = GenerateTemplate(existingContentVersion);

            if (existingParamsContent != null)
            {
                var existingParameters = existingParamsContent?.SelectToken("parameters")?.ToObject<JObject>();
                if (existingParameters is not null)
                {
                    var mergeSettings = new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union };
                    foreach (var existingParameter in existingParameters)
                    {
                        var existingParameterName = existingParameter.Key;
                        var existingParameterValue = existingParameter.Value;

                        if (existingParameterValue is not null)
                        {
                            if (templateJToken.SelectToken($"parameters.{existingParameterName}") is JObject existingParameterObject)
                            {
                                existingParameterObject.Merge(existingParameterValue, mergeSettings);
                            }
                        }
                    }
                }
            }

            templateJToken.WriteTo(writer);
        }

        private JToken GenerateTemplate(string contentVersion)
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new PositionTrackingJsonTextWriter(stringWriter);
            var emitter = new ExpressionEmitter(jsonWriter, this.Context);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#");
            emitter.EmitProperty("contentVersion", contentVersion);

            var allParameterDeclarations = this.Context.SemanticModel.Root.ParameterDeclarations;

            var filteredParameterDeclarations = this.IncludeParams == IncludeParamsOption.All
                ? allParameterDeclarations
                : [.. allParameterDeclarations.Where(e => e.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax)];

            if (filteredParameterDeclarations.Length > 0)
            {
                jsonWriter.WritePropertyName("parameters");
                jsonWriter.WriteStartObject();

                foreach (var parameterSymbol in filteredParameterDeclarations)
                {
                    jsonWriter.WritePropertyName(parameterSymbol.Name);
                    jsonWriter.WriteStartObject();

                    EmitValue(parameterSymbol.Type);

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            var content = stringWriter.ToString();
            return content.FromJson<JToken>();

            void EmitValue(TypeSymbol type)
            {
                if (!TryEmitPrimitive(type.Name))
                {
                    if (type is ObjectType objType && objType.Properties.Count > 0)
                    {
                        emitter.EmitProperty("value", () =>
                        {
                            jsonWriter.WriteStartObject();
                            foreach (var property in objType.Properties)
                            {
                                jsonWriter.WritePropertyName(property.Key);
                                EmitPrimitiveValue(property.Value.TypeReference.Type.Name);
                            }
                            jsonWriter.WriteEndObject();
                        });
                    }
                }
            }

            bool TryEmitPrimitive(string typeName)
            {
                switch (typeName)
                {
                    case "string":
                        emitter.EmitProperty("value", () => EmitPrimitiveValue("string"));
                        return true;
                    case "int":
                        emitter.EmitProperty("value", () => EmitPrimitiveValue("int"));
                        return true;
                    case "bool":
                        emitter.EmitProperty("value", () => EmitPrimitiveValue("bool"));
                        return true;
                    case "object":
                        emitter.EmitProperty("value", () => EmitPrimitiveValue("object"));
                        return true;
                    case "array":
                        emitter.EmitProperty("value", () => EmitPrimitiveValue("array"));
                        return true;
                    default:
                        return false;
                }
            }

            void EmitPrimitiveValue(string typeName)
            {
                switch (typeName)
                {
                    case "string":
                        jsonWriter.WriteValue(string.Empty);
                        break;
                    case "int":
                        jsonWriter.WriteValue(0);
                        break;
                    case "bool":
                        jsonWriter.WriteValue(false);
                        break;
                    case "object":
                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                        break;
                    case "array":
                        jsonWriter.WriteStartArray();
                        jsonWriter.WriteEndArray();
                        break;
                    default:
                        jsonWriter.WriteValue(string.Empty);
                        break;
                }
            }
        }
    }
}
