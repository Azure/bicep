// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Arm.Expression.Configuration;
using Arm.Expression.Expressions;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionEmitter
    {
        private static readonly ExpressionSerializer ExpressionSerializer = new ExpressionSerializer(new ExpressionSerializerSettings
        {
            IncludeOuterSquareBrackets = true,

            // this setting will ensure that we emit strings instead of a string literal expressions
            SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
        });

        private readonly JsonTextWriter writer;
        private readonly EmitterContext context;
        private readonly ExpressionConverter converter;

        public ExpressionEmitter(JsonTextWriter writer, EmitterContext context)
        {
            this.writer = writer;
            this.context = context;
            this.converter = new ExpressionConverter(context);
        }

        public void EmitExpression(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case BooleanLiteralSyntax boolSyntax:
                    writer.WriteValue(boolSyntax.Value);
                    break;

                case NumericLiteralSyntax numericSyntax:
                    writer.WriteValue(numericSyntax.Value);
                    break;

                case NullLiteralSyntax _:
                    writer.WriteNull();

                    break;

                case ObjectSyntax objectSyntax:
                    writer.WriteStartObject();
                    EmitObjectProperties(objectSyntax);
                    writer.WriteEndObject();

                    break;

                case ArraySyntax arraySyntax:
                    writer.WriteStartArray();

                    foreach (ArrayItemSyntax itemSyntax in arraySyntax.Items)
                    {
                        EmitExpression(itemSyntax.Value);
                    }

                    writer.WriteEndArray();

                    break;

                case ParenthesizedExpressionSyntax _:
                case UnaryOperationSyntax _:
                case BinaryOperationSyntax _:
                case TernaryOperationSyntax _:
                case StringSyntax _:
                case InstanceFunctionCallSyntax _:
                case FunctionCallSyntax _:
                case ArrayAccessSyntax _:
                case PropertyAccessSyntax _:
                case VariableAccessSyntax _:
                    EmitLanguageExpression(syntax);
                    
                    break;

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }

        public void EmitResourceIdReference(ResourceDeclarationSyntax resourceSyntax, ResourceTypeReference typeReference)
        {
            var resourceIdExpression = converter.GetResourceIdExpression(resourceSyntax, typeReference);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public void EmitModuleResourceIdExpression(ModuleSymbol moduleSymbol)
        {
            var resourceIdExpression = converter.GetModuleResourceIdExpression(moduleSymbol);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public void EmitLanguageExpression(SyntaxBase syntax)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(syntax);
            if (symbol is VariableSymbol variableSymbol && context.VariablesToInline.Contains(variableSymbol))
            {
                EmitExpression(variableSymbol.Value);
                return;
            }

            if (syntax is FunctionCallSyntax functionCall && 
                symbol is FunctionSymbol functionSymbol && 
                string.Equals(functionSymbol.Name, "any", LanguageConstants.IdentifierComparison))
            {
                // the outermost function in the current syntax node is the "any" function
                // we should emit its argument directly
                // otherwise, they'd get wrapped in a json() template function call in the converted expression

                // we have checks for function parameter count mismatch, which should prevent an exception from being thrown
                EmitExpression(functionCall.Arguments.Single().Expression);
                return;
            }

            LanguageExpression converted = converter.ConvertExpression(syntax);

            if (converted is JTokenExpression valueExpression && valueExpression.Value.Type == JTokenType.Integer)
            {
                // the converted expression is an integer literal
                JToken value = valueExpression.Value;

                // for integer literals the expression will look like "[42]" or "[-12]"
                // while it's still a valid template expression that works in ARM, it looks weird
                // and is also not recognized by the template language service in VS code
                // let's serialize it as a proper integer instead
                writer.WriteValue(value);

                return;
            }

            // strings literals and other expressions must be processed with the serializer to ensure correct conversion and escaping
            var serialized = ExpressionSerializer.SerializeExpression(converted);

            writer.WriteValue(serialized);
        }

        public void EmitObjectProperties(ObjectSyntax objectSyntax, ISet<string>? propertiesToOmit = null)
        {
            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                if (propertySyntax.TryGetKeyText() is string keyName)
                {
                    if (propertiesToOmit?.Contains(keyName) == true)
                    {
                        continue;
                    }

                    EmitPropertyExpression(keyName, propertySyntax.Value);
                }
                else
                {
                    EmitPropertyExpressionWithExpressionKey(propertySyntax.Key, propertySyntax.Value);
                }
            }
        }

        public void EmitPropertyValue(string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public void EmitPropertyExpression(string name, SyntaxBase expression)
        {
            writer.WritePropertyName(name);
            EmitExpression(expression);
        }

        public void EmitPropertyExpressionWithExpressionKey(SyntaxBase expressionKey, SyntaxBase expressionValue)
        {
            var keyExpression = converter.ConvertExpression(expressionKey);
            var keyText = ExpressionSerializer.SerializeExpression(keyExpression);
            EmitPropertyExpression(keyText, expressionValue);
        }

        public void EmitOptionalPropertyExpression(string name, SyntaxBase? expression)
        {
            if (expression != null)
            {
                EmitPropertyExpression(name, expression);
            }
        }
    }
}

