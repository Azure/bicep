using System;
using System.Linq;
using Azure.ResourceManager.Deployments.Expression.Configuration;
using Azure.ResourceManager.Deployments.Expression.Expressions;
using Azure.ResourceManager.Deployments.Expression.Serializers;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

                    // parse errors should have prevented code generation, so cast should be safe
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

            if (converted is JTokenExpression valueExpression)
            {
                // the converted expression is a literal
                JToken value = valueExpression.EvaluateExpression(null);

                // for integer literals the expression will look like "[42]" or "[-12]"
                // while it's still a valid template expression that works in ARM, it looks weird
                // and is also not recognized by the template language service in VS code
                // let's serialize it as a proper integer instead
                // string literals are actually handled by the expression serializer already, but
                // we can take care of them here as well
                writer.WriteValue(value);

                return;
            }

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
