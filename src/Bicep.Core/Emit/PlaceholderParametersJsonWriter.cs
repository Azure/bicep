// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit.Options;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

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

            // Template is used for calcualting template hash, template jtoken is used for writing to file.
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
            using var jsonWriter = new PositionTrackingJsonTextWriter(this.Context.SemanticModel.FileResolver, stringWriter);
            var emitter = new ExpressionEmitter(jsonWriter, this.Context);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#");

            emitter.EmitProperty("contentVersion", contentVersion);

            var allParameterDeclarations = this.Context.SemanticModel.Root.ParameterDeclarations;

            var filteredParameterDeclarations = this.IncludeParams == IncludeParamsOption.All ? allParameterDeclarations : allParameterDeclarations.Where(e => e.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax);

            if (filteredParameterDeclarations.Count() > 0)
            {
                jsonWriter.WritePropertyName("parameters");
                jsonWriter.WriteStartObject();

                foreach (var parameterSymbol in filteredParameterDeclarations)
                {
                    if (parameterSymbol.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax)
                    {
                        jsonWriter.WritePropertyName(parameterSymbol.Name);

                        jsonWriter.WriteStartObject();
                        switch (parameterSymbol.Type.Name)
                        {
                            case "string":
                                emitter.EmitProperty("value", "");
                                break;
                            case "int":
                                emitter.EmitProperty("value", () => jsonWriter.WriteValue(0));
                                break;
                            case "bool":
                                emitter.EmitProperty("value", () => jsonWriter.WriteValue(false));
                                break;
                            case "object":
                                emitter.EmitProperty("value", () => { jsonWriter.WriteStartObject(); jsonWriter.WriteEndObject(); });
                                break;
                            case "array":
                                emitter.EmitProperty("value", () => { jsonWriter.WriteStartArray(); jsonWriter.WriteEndArray(); });
                                break;
                        }
                        jsonWriter.WriteEndObject();
                    }
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();

            return content.FromJson<JToken>();
        }
    }
}
