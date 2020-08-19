using System;
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
            SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
        });

        private readonly JsonTextWriter writer;
        private readonly SemanticModel.SemanticModel model;
        private readonly ExpressionConverter converter;

        public ExpressionEmitter(JsonTextWriter writer, SemanticModel.SemanticModel model)
        {
            this.writer = writer;
            this.model = model;
            this.converter = new ExpressionConverter(this.model);
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

        public void EmitLanguageExpression(SyntaxBase syntax)
        {
            var symbol = model.GetSymbolInfo(syntax);
            if (model.GetSymbolInfo(syntax) is VariableSymbol variableSymbol && model.RequiresInlining(variableSymbol))
            {
                EmitExpression(variableSymbol.Value);
                return;
            }

            LanguageExpression converted = converter.ConvertExpression(syntax);

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

        public void EmitObjectProperties(ObjectSyntax objectSyntax)
        {
            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                EmitPropertyExpression(propertySyntax.Identifier.IdentifierName, propertySyntax.Value);
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

        public void EmitOptionalPropertyExpression(string name, SyntaxBase? expression)
        {
            if (expression != null)
            {
                EmitPropertyExpression(name, expression);
            }
        }
    }
}
