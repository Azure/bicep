using System;
using Arm.Expression.Configuration;
using Arm.Expression.Expressions;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public static class ExpressionEmitter
    {
        private static readonly ExpressionSerializer ExpressionSerializer = new ExpressionSerializer(new ExpressionSerializerSettings
        {
            IncludeOuterSquareBrackets = true,
            SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
        });

        public static void EmitExpression(JsonTextWriter writer, SyntaxBase syntax, SemanticModel.SemanticModel model)
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
                    EmitObjectProperties(writer, objectSyntax, model);
                    writer.WriteEndObject();

                    break;

                case ArraySyntax arraySyntax:
                    writer.WriteStartArray();

                    foreach (ArrayItemSyntax itemSyntax in arraySyntax.Items)
                    {
                        EmitExpression(writer, itemSyntax.Value, model);
                    }

                    writer.WriteEndArray();

                    break;

                case ParenthesizedExpressionSyntax _:
                case UnaryOperationSyntax _:
                case BinaryOperationSyntax _:
                case TernaryOperationSyntax _:
                case StringSyntax _:
                case FunctionCallSyntax _:
                case ArrayAccessSyntax _:
                case PropertyAccessSyntax _:
                case VariableAccessSyntax _:
                    EmitLanguageExpression(writer, syntax, model);
                    
                    break;

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }

        public static void EmitLanguageExpression(JsonTextWriter writer, SyntaxBase syntax, SemanticModel.SemanticModel model)
        {
            LanguageExpression converted = syntax.ToTemplateExpression(model);

            if (converted is JTokenExpression valueExpression)
            {
                // the converted expression is a literal
                JToken value = valueExpression.Value;

                // for integer literals the expression will look like "[42]" or "[-12]"
                // while it's still a valid template expression that works in ARM, it looks weird
                // and is also not recognized by the template language service in VS code
                // let's serialize it as a proper integer instead
                // string literals are actually handled by the expression serializer already, but
                // we can take care of them here as well
                writer.WriteValue(value);

                return;
            }

            var serialized = ExpressionSerializer.SerializeExpression(converted);

            writer.WriteValue(serialized);
        }

        public static void EmitObjectProperties(JsonTextWriter writer, ObjectSyntax objectSyntax, SemanticModel.SemanticModel model)
        {
            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                EmitPropertyExpression(writer, propertySyntax.Identifier.IdentifierName, propertySyntax.Value, model);
            }
        }

        public static void EmitPropertyValue(JsonTextWriter writer, string name, string value)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void EmitPropertyExpression(JsonTextWriter writer, string name, SyntaxBase expression, SemanticModel.SemanticModel model)
        {
            writer.WritePropertyName(name);
            EmitExpression(writer, expression, model);
        }

        public static void EmitOptionalPropertyExpression(JsonTextWriter writer, string name, SyntaxBase? expression, SemanticModel.SemanticModel model)
        {
            if (expression != null)
            {
                EmitPropertyExpression(writer, name, expression, model);
            }
        }
    }
}
