// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Expression.Serializers;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Newtonsoft.Json;
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

                case IntegerLiteralSyntax integerSyntax:
                    writer.WriteValue(integerSyntax.Value);
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
                case ResourceAccessSyntax _:
                case VariableAccessSyntax _:
                    EmitLanguageExpression(syntax);

                    break;

                default:
                    throw new NotImplementedException($"Cannot emit unexpected expression of type {syntax.GetType().Name}");
            }
        }

        public void EmitExpression(SyntaxBase resourceNameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = converter.CreateConverterForIndexReplacement(resourceNameSyntax, indexExpression, newContext);

            var expression = converterForContext.ConvertExpression(resourceNameSyntax);
            var serialized = ExpressionSerializer.SerializeExpression(expression);

            writer.WriteValue(serialized);
        }

        public void EmitUnqualifiedResourceId(ResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = converter.CreateConverterForIndexReplacement(resource.NameSyntax, indexExpression, newContext);

            var unqualifiedResourceId = converterForContext.GetUnqualifiedResourceId(resource);
            var serialized = ExpressionSerializer.SerializeExpression(unqualifiedResourceId);

            writer.WriteValue(serialized);
        }

        public void EmitIndexedSymbolReference(ResourceMetadata resource, SyntaxBase indexExpression, SyntaxBase newContext)
        {
            var expression = converter.CreateConverterForIndexReplacement(resource.NameSyntax, indexExpression, newContext)
                .GenerateSymbolicReference(resource.Symbol.Name, indexExpression);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public void EmitIndexedSymbolReference(ModuleSymbol moduleSymbol, SyntaxBase indexExpression, SyntaxBase newContext)
        {
            var expression = converter.CreateConverterForIndexReplacement(ExpressionConverter.GetModuleNameSyntax(moduleSymbol), indexExpression, newContext)
                .GenerateSymbolicReference(moduleSymbol.Name, indexExpression);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public void EmitResourceIdReference(ResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = this.converter.CreateConverterForIndexReplacement(resource.NameSyntax, indexExpression, newContext);

            var resourceIdExpression = converterForContext.GetFullyQualifiedResourceId(resource);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public void EmitResourceIdReference(ModuleSymbol moduleSymbol, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = this.converter.CreateConverterForIndexReplacement(ExpressionConverter.GetModuleNameSyntax(moduleSymbol), indexExpression, newContext);

            var resourceIdExpression = converterForContext.GetFullyQualifiedResourceId(moduleSymbol);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public LanguageExpression GetFullyQualifiedResourceName(ResourceMetadata resource)
        {
            return converter.GetFullyQualifiedResourceName(resource);
        }

        public LanguageExpression GetManagementGroupResourceId(SyntaxBase managementGroupNameProperty, SyntaxBase? indexExpression, SyntaxBase newContext, bool fullyQualified)
        {
            var converterForContext = converter.CreateConverterForIndexReplacement(managementGroupNameProperty, indexExpression, newContext);
            return converterForContext.GenerateManagementGroupResourceId(managementGroupNameProperty, fullyQualified);
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
                string.Equals(functionSymbol.Name, LanguageConstants.AnyFunction, LanguageConstants.IdentifierComparison))
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
        public void EmitCopyObject(string? name, ForSyntax syntax, SyntaxBase? input, string? copyIndexOverride = null, long? batchSize = null)
        {
            // local function
            static bool CanEmitAsInputDirectly(SyntaxBase input)
            {
                // the deployment engine only allows JTokenType of String or Object in the copy loop "input" property
                // everything else must be converted into an expression
                return input switch
                {
                    // objects should be emitted as is
                    ObjectSyntax => true,

                    // non-interpolated strings should be emitted as-is
                    StringSyntax @string when !@string.IsInterpolated() => true,

                    // all other expressions should be converted into a language expression before emitting
                    // which will have the resulting JTokenType of String
                    _ => false
                };
            }

            writer.WriteStartObject();

            if (name is not null)
            {
                this.EmitProperty("name", name);
            }

            // construct the length ARM expression from the Bicep array expression
            // type check has already ensured that the array expression is an array
            this.EmitPropertyWithTransform(
                "count",
                syntax.Expression,
                arrayExpression => new FunctionExpression("length", new[] { arrayExpression }, Array.Empty<LanguageExpression>()));

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
                                    function.Parameters = new LanguageExpression[] { new JTokenExpression("value") };
                                }
                            }
                        };

                        // mutate the expression
                        expression.Accept(visitor);

                        return expression;
                    });
                }
            }

            writer.WriteEndObject();
        }

        public void EmitObjectProperties(ObjectSyntax objectSyntax, ISet<string>? propertiesToOmit = null)
        {
            var propertyLookup = objectSyntax.Properties.ToLookup(property => property.Value is ForSyntax);

            // emit loop properties first (if any)
            if (propertyLookup.Contains(true))
            {
                // we have properties whose value is a for-expression
                this.EmitProperty("copy", () =>
                {
                    this.writer.WriteStartArray();

                    foreach (var property in propertyLookup[true])
                    {
                        var key = property.TryGetKeyText();
                        if (key is null || property.Value is not ForSyntax @for)
                        {
                            // should be caught by loop emit limitation checks
                            throw new InvalidOperationException("Encountered a property with an expression-based key whose value is a for-expression.");
                        }

                        this.EmitCopyObject(key, @for, @for.Body);
                    }

                    this.writer.WriteEndArray();
                });
            }

            // emit non-loop properties
            foreach (ObjectPropertySyntax propertySyntax in propertyLookup[false])
            {
                // property whose value is not a for-expression

                if (propertySyntax.TryGetKeyText() is string keyName)
                {
                    if (propertiesToOmit?.Contains(keyName) == true)
                    {
                        continue;
                    }

                    EmitProperty(keyName, propertySyntax.Value);
                }
                else
                {
                    EmitProperty(propertySyntax.Key, propertySyntax.Value);
                }
            }
        }

        public void EmitModuleParameterValue(SyntaxBase syntax)
        {
            if (syntax is InstanceFunctionCallSyntax instanceFunctionCall && string.Equals(instanceFunctionCall.Name.IdentifierName, "getSecret", LanguageConstants.IdentifierComparison))
            {
                var baseSyntax = instanceFunctionCall.BaseExpression switch
                {
                    ArrayAccessSyntax arrayAccessSyntax => arrayAccessSyntax.BaseExpression,
                    _ => instanceFunctionCall.BaseExpression,
                };

                if (context.SemanticModel.ResourceMetadata.TryLookup(baseSyntax) is not {} resource ||
                    !string.Equals(resource.TypeReference.FullyQualifiedType, "microsoft.keyvault/vaults", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Cannot emit parameter's KeyVault secret reference.");
                }

                var keyVaultId = instanceFunctionCall.BaseExpression switch
                {
                    ArrayAccessSyntax arrayAccessSyntax => converter.CreateConverterForIndexReplacement(resource.NameSyntax, arrayAccessSyntax.IndexExpression, instanceFunctionCall)
                                                                    .GetFullyQualifiedResourceId(resource),
                    _ => converter.GetFullyQualifiedResourceId(resource)
                };

                writer.WritePropertyName("reference");
                writer.WriteStartObject();
                writer.WritePropertyName("keyVault");
                writer.WriteStartObject();

                writer.WritePropertyName("id");

                var keyVaultIdSerialised = ExpressionSerializer.SerializeExpression(keyVaultId);
                writer.WriteValue(keyVaultIdSerialised);

                writer.WriteEndObject(); // keyVault

                writer.WritePropertyName("secretName");
                var secretName = converter.ConvertExpression(instanceFunctionCall.Arguments[0].Expression);
                var secretNameSerialised = ExpressionSerializer.SerializeExpression(secretName);
                writer.WriteValue(secretNameSerialised);

                if (instanceFunctionCall.Arguments.Length > 1)
                {
                    writer.WritePropertyName("secretVersion");
                    var secretVersion = converter.ConvertExpression(instanceFunctionCall.Arguments[1].Expression);
                    var secretVersionSerialised = ExpressionSerializer.SerializeExpression(secretVersion);
                    writer.WriteValue(secretVersionSerialised);
                }
                writer.WriteEndObject(); // reference

                return;
            }
            EmitProperty("value", syntax);
        }

        public void EmitProperty(string name, LanguageExpression expressionValue)
            => EmitPropertyInternal(new JTokenExpression(name), () =>
            {
                var propertyValue = ExpressionSerializer.SerializeExpression(expressionValue);
                writer.WriteValue(propertyValue);
            });

        public void EmitPropertyWithTransform(string name, SyntaxBase value, Func<LanguageExpression, LanguageExpression> convertedValueTransform)
            => EmitPropertyInternal(new JTokenExpression(name), () =>
            {
                var converted = converter.ConvertExpression(value);
                var transformed = convertedValueTransform(converted);
                var serialized = ExpressionSerializer.SerializeExpression(transformed);

                this.writer.WriteValue(serialized);
            });

        public void EmitProperty(string name, Action valueFunc)
            => EmitPropertyInternal(new JTokenExpression(name), valueFunc);

        public void EmitProperty(string name, string value)
            => EmitPropertyInternal(new JTokenExpression(name), value);

        public void EmitProperty(string name, SyntaxBase expressionValue)
            => EmitPropertyInternal(new JTokenExpression(name), expressionValue);

        public void EmitProperty(SyntaxBase syntaxKey, SyntaxBase syntaxValue)
            => EmitPropertyInternal(converter.ConvertExpression(syntaxKey), syntaxValue);

        private void EmitPropertyInternal(LanguageExpression expressionKey, Action valueFunc)
        {
            var serializedName = ExpressionSerializer.SerializeExpression(expressionKey);
            writer.WritePropertyName(serializedName);

            valueFunc();
        }

        private void EmitPropertyInternal(LanguageExpression expressionKey, string value)
            => EmitPropertyInternal(expressionKey, () =>
            {
                var propertyValue = ExpressionSerializer.SerializeExpression(new JTokenExpression(value));
                writer.WriteValue(propertyValue);
            });

        private void EmitPropertyInternal(LanguageExpression expressionKey, SyntaxBase syntaxValue)
            => EmitPropertyInternal(expressionKey, () => EmitExpression(syntaxValue));

        public void EmitOptionalPropertyExpression(string name, SyntaxBase? expression)
        {
            if (expression != null)
            {
                EmitProperty(name, expression);
            }
        }
    }
}

