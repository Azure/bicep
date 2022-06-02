// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Bicep.Core.Emit
{
    public class ParametersFileTemplateWriter : TemplateWriter
    {
        public ParametersFileTemplateWriter(SemanticModel semanticModel, EmitterSettings settings): base(semanticModel, settings)
        {
            this.context = new EmitterContext(semanticModel, settings);
            this.settings = settings;
        }

        private readonly EmitterContext context;
        private readonly EmitterSettings settings;

        public void Write(JsonTextWriter writer, string existingContent)
        {
            // Template is used for calcualting template hash, template jtoken is used for writing to file.
            var templateJToken = GenerateTemplate();
            if (templateJToken.SelectToken(GeneratorMetadataPath) is not JObject generatorObject)
            {
                throw new InvalidOperationException($"generated template doesn't contain a generator object at the path {GeneratorMetadataPath}");
            }

            if (!string.IsNullOrWhiteSpace(existingContent))
            {
                try
                {
                    var existingParamsContent = JToken.Parse(existingContent);
                    var existingParameters = existingParamsContent.SelectToken("parameters")?.ToObject<JObject>();
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
                catch
                {
                    // content of the existing file is not valid json, ignore merging it
                }
            }

            templateJToken.WriteTo(writer);
        }

        private JToken GenerateTemplate()
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            var emitter = new ExpressionEmitter(jsonWriter, this.context);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#");

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitMetadata(jsonWriter, emitter);

            if (this.context.SemanticModel.Root.ParameterDeclarations.Length > 0)
            {
                jsonWriter.WritePropertyName("parameters");
                jsonWriter.WriteStartObject();

                foreach (var parameterSymbol in this.context.SemanticModel.Root.ParameterDeclarations)
                {
                    jsonWriter.WritePropertyName(parameterSymbol.Name);

                    jsonWriter.WriteStartObject();

                    if (parameterSymbol.DeclaringParameter.Modifier is ParameterDefaultValueSyntax defaultValueSyntax)
                    {
                        emitter.EmitProperty("value", defaultValueSyntax.DefaultValue);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();

            return content.FromJson<JToken>();
        }
    }
}
