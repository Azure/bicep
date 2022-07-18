// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.IO;
using System;
using Bicep.Core.Semantics;

namespace Bicep.Core.Emit
{
    public class ParametersJsonWriter
    {
        private readonly ParamsSemanticModel paramSemanticModel;
        public ParametersJsonWriter(ParamsSemanticModel paramSemanticModel)
        {
            this.paramSemanticModel = paramSemanticModel;
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

            //TODO: Update after param semantic model is complete
            var syntax = paramSemanticModel.BicepParamFile.ProgramSyntax;
            var parameters = syntax.Children.OfType<ParameterAssignmentSyntax>().ToImmutableList();

            if (parameters.Count > 0)
            {
                jsonWriter.WritePropertyName("parameters");
                jsonWriter.WriteStartObject();

                foreach (var parameter in parameters)
                {
                    jsonWriter.WritePropertyName(parameter.Name.IdentifierName);

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("value");
                    this.EmitExpression(parameter.Value, jsonWriter);
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();

            return content.FromJson<JToken>();
        }

        public void EmitExpression(SyntaxBase syntax, JsonTextWriter jsonWriter)
        {
            switch (syntax)
            {
                case BooleanLiteralSyntax booleanLiteralSyntax:
                {
                    jsonWriter.WriteValue(booleanLiteralSyntax.Value);
                    break;
                }
                case IntegerLiteralSyntax integerLiteralSyntax:
                {
                    jsonWriter.WriteValue(integerLiteralSyntax.Value);
                    break;
                }
                case StringSyntax stringSyntax:
                {
                    jsonWriter.WriteValue(extractString(stringSyntax));
                    break;
                }
                case ObjectSyntax objectSyntax:
                {
                    jsonWriter.WriteStartObject();
                    EmitObjectProperties(objectSyntax, jsonWriter);
                    jsonWriter.WriteEndObject();
                    break;
                }
                case ArraySyntax arraySyntax:
                {
                    jsonWriter.WriteStartArray();
                    foreach (ArrayItemSyntax itemSyntax in arraySyntax.Items)
                    {
                        EmitExpression(itemSyntax.Value, jsonWriter);
                    }
                    jsonWriter.WriteEndArray();
                    break;
                }
                case NullLiteralSyntax _:
                {
                    jsonWriter.WriteNull();
                    break;
                }
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}"); 
            }
        }

        public void EmitObjectProperties(ObjectSyntax objectSyntax, JsonTextWriter jsonWriter)
        {
            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                string key = propertySyntax.TryGetKeyText() ?? throw new InvalidOperationException($"Interpolation is not currently supported for object keys");
                jsonWriter.WritePropertyName(key);
                EmitExpression(propertySyntax.Value, jsonWriter);
            }
        }

        private string extractString(StringSyntax stringSyntax) => stringSyntax.TryGetLiteralValue() ?? throw new InvalidOperationException($"Interpolation is not currently supported for string values");
    }
}
