// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Expression.Serializers;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers.Az;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ExpressionEmitter
    {
        private static readonly ExpressionSerializer ExpressionSerializer = new(new ExpressionSerializerSettings
        {
            IncludeOuterSquareBrackets = true,

            // this setting will ensure that we emit strings instead of a string literal expressions
            SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
        });

        private readonly PositionTrackingJsonTextWriter writer;
        private readonly EmitterContext context;
        private readonly ExpressionConverter converter;

        public ExpressionEmitter(PositionTrackingJsonTextWriter writer, EmitterContext context)
        {
            this.writer = writer;
            this.context = context;
            this.converter = new ExpressionConverter(context);
        }

        public void EmitExpression(Expression expression)
        {
            switch (expression)
            {
                case BooleanLiteralExpression @bool:
                    writer.WriteValue(@bool.Value);
                    break;

                case IntegerLiteralExpression @int:
                    writer.WriteValue(@int.Value);
                    break;

                case NullLiteralExpression _:
                    writer.WriteNull();

                    break;

                case ObjectExpression @object:
                    writer.WriteStartObject();
                    EmitObjectProperties(@object);
                    writer.WriteEndObject();

                    break;

                case ArrayExpression @array:
                    writer.WriteStartArray();

                    foreach (var item in @array.Items)
                    {
                        writer.WriteExpressionWithPosition(
                            item.SourceSyntax,
                            () => EmitExpression(item));
                    }

                    writer.WriteEndArray();

                    break;

                default:
                    EmitLanguageExpression(expression);
                    break;
            }
        }

        public void EmitExpression(SyntaxBase resourceNameSyntax, IndexReplacementContext? indexContext)
        {
            var converterForContext = converter.GetConverter(indexContext);

            var expression = converterForContext.ConvertExpression(resourceNameSyntax);
            var serialized = ExpressionSerializer.SerializeExpression(expression);

            writer.WriteValue(serialized);
        }

        public void EmitUnqualifiedResourceId(DeclaredResourceMetadata resource, IndexReplacementContext? indexContext)
        {
            var converterForContext = converter.GetConverter(indexContext);

            var unqualifiedResourceId = converterForContext.GetUnqualifiedResourceId(resource);
            var serialized = ExpressionSerializer.SerializeExpression(unqualifiedResourceId);

            writer.WriteValue(serialized);
        }

        public void EmitIndexedSymbolReference(DeclaredResourceMetadata resource, IndexReplacementContext? indexContext)
        {
            var expression = converter.GetConverter(indexContext).GenerateSymbolicReference(resource, indexContext);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public void EmitSymbolReference(DeclaredResourceMetadata resource)
        {
            var expression = converter.GenerateSymbolicReference(resource, null);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public string GetSymbolicName(DeclaredResourceMetadata resource)
            => converter.GetSymbolicName(resource);

        public void EmitIndexedSymbolReference(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext)
        {
            var expression = converter.GetConverter(indexContext).GenerateSymbolicReference(moduleSymbol, indexContext);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public void EmitResourceIdReference(DeclaredResourceMetadata resource, IndexReplacementContext? indexContext)
        {
            var converterForContext = this.converter.GetConverter(indexContext);

            var resourceIdExpression = converterForContext.GetFullyQualifiedResourceId(resource);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public void EmitResourceIdReference(ModuleSymbol moduleSymbol, IndexReplacementContext? indexContext)
        {
            var converterForContext = this.converter.GetConverter(indexContext);

            var resourceIdExpression = converterForContext.GetFullyQualifiedResourceId(moduleSymbol);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public LanguageExpression GetFullyQualifiedResourceName(DeclaredResourceMetadata resource)
        {
            return converter.GetFullyQualifiedResourceName(resource);
        }

        public LanguageExpression GetManagementGroupResourceId(SyntaxBase managementGroupNameProperty, IndexReplacementContext? indexContext, bool fullyQualified)
        {
            var converterForContext = converter.GetConverter(indexContext);
            return converterForContext.GenerateManagementGroupResourceId(managementGroupNameProperty, fullyQualified);
        }

        public void EmitLanguageExpression(Expression expression)
        {
            if (expression is FunctionCallExpression functionCall &&
                string.Equals(functionCall.Name, LanguageConstants.AnyFunction, LanguageConstants.IdentifierComparison))
            {
                // the outermost function in the current syntax node is the "any" function
                // we should emit its argument directly
                // otherwise, they'd get wrapped in a json() template function call in the converted expression

                // we have checks for function parameter count mismatch, which should prevent an exception from being thrown
                EmitExpression(functionCall.Parameters.Single());
                return;
            }

            var converted = converter.ConvertExpression(expression);

            if (converted is JTokenExpression valueExpression && valueExpression.Value.Type == JTokenType.Integer)
            {
                // for integer literals the expression will look like "[42]" or "[-12]"
                // while it's still a valid template expression that works in ARM, it looks weird
                // and is also not recognized by the template language service in VS code
                // let's serialize it as a proper integer instead
                writer.WriteValue(valueExpression.Value);
            }
            else
            {
                // strings literals and other expressions must be processed with the serializer to ensure correct conversion and escaping
                var serialized = ExpressionSerializer.SerializeExpression(converted);

                writer.WriteValue(serialized);
            }
        }

        public void EmitCopyObject(string? name, Expression forExpression, Expression? input, string? copyIndexOverride = null, ulong? batchSize = null)
        {
            // local function
            static bool CanEmitAsInputDirectly(Expression input)
            {
                // the deployment engine only allows JTokenType of String or Object in the copy loop "input" property
                // everything else must be converted into an expression
                return input switch
                {
                    // objects should be emitted as is
                    ObjectExpression => true,

                    // string literal values should be emitted as-is
                    StringLiteralExpression => true,

                    // all other expressions should be converted into a language expression before emitting
                    // which will have the resulting JTokenType of String
                    _ => false
                };
            }

            writer.WriteObjectWithPosition(forExpression.SourceSyntax, () =>
            {
                if (name is not null)
                {
                    this.EmitProperty("name", name);
                }

                // construct the length ARM expression from the Bicep array expression
                // type check has already ensured that the array expression is an array
                this.EmitProperty("count", new FunctionCallExpression(forExpression.SourceSyntax, "length", [forExpression]));

                if (batchSize.HasValue)
                {
                    this.EmitProperty("mode", "serial");
                    this.EmitProperty("batchSize", () => writer.WriteValue(batchSize.Value));
                }

                if (input != null)
                {
                    if (copyIndexOverride == null)
                    {
                        if (CanEmitAsInputDirectly(input))
                        {
                            this.EmitProperty("input", input);
                        }
                        else
                        {
                            this.EmitPropertyWithTransform("input", input, converted => ExpressionConverter.ToFunctionExpression(converted));
                        }
                    }
                    else
                    {
                        this.EmitPropertyWithTransform("input", input, expression =>
                        {
                            if (!CanEmitAsInputDirectly(input))
                            {
                                expression = ExpressionConverter.ToFunctionExpression(expression);
                            }

                            // the named copy index in the serialized expression is incorrect
                            // because the object syntax here does not match the JSON equivalent due to the presence of { "value": ... } wrappers
                            // for now, we will manually replace the copy index in the converted expression
                            // this approach will not work for nested property loops
                            var visitor = new LanguageExpressionVisitor
                            {
                                OnFunctionExpression = function =>
                                {
                                    if (string.Equals(function.Function, "copyIndex") &&
                                        function.Parameters.Length == 1 &&
                                        function.Parameters[0] is JTokenExpression)
                                    {
                                        // it's an invocation of the copyIndex function with 1 argument with a literal value
                                        // replace the argument with the correct value
                                        function.Parameters = [new JTokenExpression("value")];
                                    }
                                }
                            };

                            // mutate the expression
                            expression.Accept(visitor);

                            return expression;
                        });
                    }
                }
            });
        }

        public void EmitObjectProperties(ObjectExpression @object)
        {
            var propertyLookup = @object.Properties.OfType<ObjectPropertyExpression>().ToLookup(property => property.Value is ForLoopExpression);

            // emit loop properties first (if any)
            if (propertyLookup.Contains(true))
            {
                // we have properties whose value is a for-expression
                this.EmitCopyProperty(() =>
                {
                    this.writer.WriteStartArray();

                    foreach (var property in propertyLookup[true])
                    {
                        if (property.Key is not StringLiteralExpression stringKeyExpression ||
                            property.Value is not ForLoopExpression forLoop)
                        {
                            // should be caught by loop emit limitation checks
                            throw new InvalidOperationException("Encountered a property with an expression-based key whose value is a for-expression.");
                        }

                        this.EmitCopyObject(stringKeyExpression.Value, forLoop.Expression, forLoop.Body);
                    }

                    this.writer.WriteEndArray();
                });
            }

            // emit non-loop properties
            foreach (var property in propertyLookup[false])
            {
                // property whose value is not a for-expression
                if (property.Key is StringLiteralExpression stringKeyExpression)
                {
                    EmitProperty(stringKeyExpression.Value, property.Value);
                }
                else
                {
                    EmitProperty(property.Key, property.Value);
                }
            }
        }

        public static Expression ConvertModuleParameter(Expression parameter)
        {
            switch (parameter)
            {
                case ResourceFunctionCallExpression functionCall when
                    LanguageConstants.IdentifierComparer.Equals(functionCall.Name, AzResourceTypeProvider.GetSecretFunctionName):
                    return ConvertModuleParameterGetSecret(functionCall);
                case TernaryExpression ternary:
                    return new TernaryExpression(ternary.SourceSyntax, ternary.Condition, ConvertModuleParameter(ternary.True), ConvertModuleParameter(ternary.False));
                default:
                    return ExpressionFactory.CreateObject(new[] {
                        ExpressionFactory.CreateObjectProperty("value", parameter)
                    }, parameter.SourceSyntax);
            }
        }

        private static Expression ConvertModuleParameterGetSecret(ResourceFunctionCallExpression functionCall)
        {
            var properties = new List<ObjectPropertyExpression>();
            properties.Add(ExpressionFactory.CreateObjectProperty("keyVault", ExpressionFactory.CreateObject(new[] {
                ExpressionFactory.CreateObjectProperty("id", new PropertyAccessExpression(functionCall.Resource.SourceSyntax, functionCall.Resource, "id", AccessExpressionFlags.None)),
            }, functionCall.SourceSyntax)));
            properties.Add(ExpressionFactory.CreateObjectProperty("secretName", functionCall.Parameters[0]));
            if (functionCall.Parameters.Length > 1)
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("secretVersion", functionCall.Parameters[1]));
            }

            return ExpressionFactory.CreateObject(new[] {
                ExpressionFactory.CreateObjectProperty("reference", ExpressionFactory.CreateObject(properties))
            }, functionCall.SourceSyntax);
        }

        public void EmitProperty(ObjectPropertyExpression property)
            => EmitPropertyInternal(converter.ConvertExpression(property.Key), () => EmitExpression(property.Value), property.SourceSyntax ?? property.Key.SourceSyntax);

        public void EmitProperty(Expression name, Expression expression)
            => EmitPropertyInternal(converter.ConvertExpression(name), () => EmitExpression(expression), expression.SourceSyntax);

        public void EmitProperty(string name, Expression expression)
            => EmitProperty(new StringLiteralExpression(expression.SourceSyntax, name), expression);

        public void EmitPropertyWithTransform(string name, Expression expression, Func<LanguageExpression, LanguageExpression> convertedValueTransform)
            => EmitPropertyInternal(new JTokenExpression(name), () =>
            {
                var converted = converter.ConvertExpression(expression);
                var transformed = convertedValueTransform(converted);
                var serialized = ExpressionSerializer.SerializeExpression(transformed);

                this.writer.WriteValue(serialized);
            });

        public void EmitProperty(string name, LanguageExpression expressionValue)
            => EmitPropertyInternal(new JTokenExpression(name), () =>
            {
                var propertyValue = ExpressionSerializer.SerializeExpression(expressionValue);
                writer.WriteValue(propertyValue);
            });

        public void EmitProperty(string name, string value)
            => EmitProperty(name, new StringLiteralExpression(null, value));

        public void EmitProperty(string name, SyntaxBase expressionValue)
            => EmitProperty(name, converter.ConvertToIntermediateExpression(expressionValue));

        private void EmitPropertyInternal(LanguageExpression expressionKey, Action valueFunc, IPositionable? location = null, bool skipCopyCheck = false)
        {
            var serializedName = ExpressionSerializer.SerializeExpression(expressionKey);
            if (!skipCopyCheck && serializedName.Equals(LanguageConstants.CopyLoopIdentifier, StringComparison.OrdinalIgnoreCase))
            {
                // we escape "copy" property name with a ARM expression to avoid it being interpreted by ARM as a copy instruction
                serializedName = $"[string('{serializedName}')]";
            }
            writer.WritePropertyWithPosition(location, serializedName, valueFunc);
        }

        public void EmitProperty(string name, Action valueFunc)
            => EmitPropertyInternal(new JTokenExpression(name), valueFunc);

        public void EmitCopyProperty(Action valueFunc)
            => EmitPropertyInternal(new JTokenExpression(LanguageConstants.CopyLoopIdentifier), valueFunc, skipCopyCheck: true);

        public void EmitProperty(string propertyName, Action writeValueFunc, IPositionable? position = null)
            => writer.WritePropertyWithPosition(
                position,
                propertyName,
                writeValueFunc);

        public void EmitObjectProperty(string propertyName, Action writePropertiesFunc, IPositionable? position = null)
            => EmitProperty(propertyName, () => EmitObject(writePropertiesFunc, position), position);

        public void EmitArrayProperty(string propertyName, Action writeItemsFunc, IPositionable? position = null)
            => EmitProperty(propertyName, () => EmitArray(writeItemsFunc, position), position);

        public void EmitObject(Action writePropertiesFunc, IPositionable? position = null)
            => writer.WriteObjectWithPosition(position, writePropertiesFunc);

        public void EmitArray(Action writeItemsFunc, IPositionable? position = null)
            => writer.WriteArrayWithPosition(position, writeItemsFunc);
    }
}
