using System;
using Azure.ResourceManager.Deployments.Expression.Configuration;
using Azure.ResourceManager.Deployments.Expression.Expressions;
using Azure.ResourceManager.Deployments.Expression.Serializers;
using Bicep.Core.Syntax;
using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public static class ExpressionEmitter
    {
        private static readonly ExpressionSerializer expressionSerializer = new ExpressionSerializer(new ExpressionSerializerSettings
        {
            IncludeOuterSquareBrackets = true,
            SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
        });

        public static void EmitExpression(JsonTextWriter writer, SyntaxBase syntax)
        {
            switch (syntax)
            {
                case BooleanLiteralSyntax boolSyntax:
                    writer.WriteValue(boolSyntax.Value);
                    break;

                case NumericLiteralSyntax numericSyntax:
                    writer.WriteValue(numericSyntax.Value);
                    break;

                case StringSyntax stringSyntax:
                    // using the throwing method to get semantic value of the string because
                    // error checking should have caught any errors by now
                    writer.WriteValue(stringSyntax.GetValue());
                    break;

                case NullLiteralSyntax _:
                    writer.WriteNull();

                    break;

                case ObjectSyntax objectSyntax:
                    writer.WriteStartObject();
                    EmitObjectProperties(writer, objectSyntax);
                    writer.WriteEndObject();

                    break;

                case ArraySyntax arraySyntax:
                    writer.WriteStartArray();

                    foreach (ArrayItemSyntax itemSyntax in arraySyntax.Items)
                    {
                        EmitExpression(writer, itemSyntax.Value);
                    }

                    writer.WriteEndArray();

                    break;

                case ParenthesizedExpressionSyntax _:
                case UnaryOperationSyntax _:
                case BinaryOperationSyntax _:
                case TernaryOperationSyntax _:
                    EmitLanguageExpression(writer, syntax);
                    
                    break;

                case ArrayAccessSyntax _:
                case FunctionArgumentSyntax _:
                case FunctionCallSyntax _:
                case PropertyAccessSyntax _:
                case VariableAccessSyntax _:
                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }

        public static void EmitLanguageExpression(JsonTextWriter writer, SyntaxBase syntax)
        {
            LanguageExpression converted = syntax.ToTemplateExpression();
            var serialized = expressionSerializer.SerializeExpression(converted);

            writer.WriteValue(serialized);
        }

        public static void EmitObjectProperties(JsonTextWriter writer, ObjectSyntax objectSyntax)
        {
            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                EmitPropertyExpression(writer, propertySyntax.Identifier.IdentifierName, propertySyntax.Value);
            }
        }

        public static void EmitPropertyValue(JsonTextWriter writer, string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void EmitPropertyExpression(JsonTextWriter writer, string name, SyntaxBase expression)
        {
            writer.WritePropertyName(name);
            EmitExpression(writer, expression);
        }

        public static void EmitOptionalPropertyExpression(JsonTextWriter writer, string name, SyntaxBase? expression)
        {
            if (expression != null)
            {
                EmitPropertyExpression(writer, name, expression);
            }
        }
    }
}
