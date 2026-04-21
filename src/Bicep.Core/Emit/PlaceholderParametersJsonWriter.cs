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

                    EmitValue(parameterSymbol);

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            var content = stringWriter.ToString();
            return content.FromJson<JToken>();

            void EmitValue(ParameterSymbol parameterSymbol)
            {
                if (TryEmitDefaultValue(parameterSymbol))
                {
                    return;
                }

                var type = parameterSymbol.Type;
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

            bool TryEmitDefaultValue(ParameterSymbol parameterSymbol)
            {
                if (parameterSymbol.DeclaringParameter.Modifier is not ParameterDefaultValueSyntax defaultValueSyntax)
                {
                    return false;
                }

                if (!TryConvertSyntaxToJToken(defaultValueSyntax.DefaultValue, out var defaultValue))
                {
                    return false;
                }

                emitter.EmitProperty("value", () => defaultValue.WriteTo(jsonWriter));
                return true;
            }

            bool TryConvertSyntaxToJToken(SyntaxBase syntax, out JToken token)
            {
                switch (syntax)
                {
                    case ParenthesizedExpressionSyntax parenthesized:
                        return TryConvertSyntaxToJToken(parenthesized.Expression, out token);

                    case StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is string stringValue:
                        token = new JValue(stringValue);
                        return true;

                    case IntegerLiteralSyntax integerLiteral:
                        token = new JValue(integerLiteral.Value);
                        return true;

                    case UnaryOperationSyntax { Operator: UnaryOperator.Minus, Expression: IntegerLiteralSyntax integerLiteral }:
                        if (integerLiteral.Value <= (ulong)long.MaxValue)
                        {
                            token = new JValue(-(long)integerLiteral.Value);
                            return true;
                        }

                        if (integerLiteral.Value == (ulong)long.MaxValue + 1)
                        {
                            token = new JValue(long.MinValue);
                            return true;
                        }

                        token = JValue.CreateNull();
                        return false;

                    case BooleanLiteralSyntax booleanLiteral:
                        token = new JValue(booleanLiteral.Value);
                        return true;

                    case NullLiteralSyntax:
                        token = JValue.CreateNull();
                        return true;

                    case ArraySyntax arraySyntax:
                        {
                            var array = new JArray();
                            foreach (var item in arraySyntax.Items)
                            {
                                if (!TryConvertSyntaxToJToken(item.Value, out var itemToken))
                                {
                                    token = JValue.CreateNull();
                                    return false;
                                }

                                array.Add(itemToken);
                            }

                            token = array;
                            return true;
                        }

                    case ObjectSyntax objectSyntax:
                        {
                            var obj = new JObject();
                            foreach (var property in objectSyntax.Properties)
                            {
                                if (property.TryGetKeyText() is not string key || !TryConvertSyntaxToJToken(property.Value, out var propertyValue))
                                {
                                    token = JValue.CreateNull();
                                    return false;
                                }

                                obj.Add(key, propertyValue);
                            }

                            token = obj;
                            return true;
                        }

                    default:
                        token = JValue.CreateNull();
                        return false;
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
