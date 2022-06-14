// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Expression.Serializers;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json;

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

        public void EmitOperation(Operation operation)
        {
            switch (operation)
            {
                case ConstantValueOperation constantValueOperation when constantValueOperation.Value is bool boolValue:
                    writer.WriteValue(boolValue);
                    return;

                case ConstantValueOperation constantValueOperation when constantValueOperation.Value is long intValue:
                    writer.WriteValue(intValue);
                    return;

                case NullValueOperation _:
                    writer.WriteNull();
                    return;

                case ObjectOperation objectOperation:
                    writer.WriteStartObject();
                    EmitObjectProperties(objectOperation);
                    writer.WriteEndObject();
                    return;

                case ObjectPropertyOperation objectPropertyOperation:
                    if (objectPropertyOperation.Key is ConstantValueOperation constantKey &&
                        constantKey.Value is string keyValue)
                    {
                        EmitProperty(keyValue, objectPropertyOperation.Value);
                    }
                    else
                    {
                        EmitProperty(objectPropertyOperation.Key, objectPropertyOperation.Value);
                    }
                    return;

                case ArrayOperation arrayOperation:
                    writer.WriteStartArray();
                    foreach (var item in arrayOperation.Items)
                    {
                        EmitOperation(item);
                    }
                    writer.WriteEndArray();
                    return;

                case GetKeyVaultSecretOperation getKeyVaultSecret:
                    EmitProperty("reference", () =>
                    {
                        writer.WriteStartObject();

                        EmitProperty("keyVault", () =>
                        {
                            writer.WriteStartObject();
                            EmitProperty("id", getKeyVaultSecret.KeyVaultId);
                            writer.WriteEndObject();
                        });

                        EmitProperty("secretName", getKeyVaultSecret.SecretName);
                        if (getKeyVaultSecret.SecretVersion is not null)
                        {
                            EmitProperty("secretVersion", getKeyVaultSecret.SecretVersion);
                        }

                        writer.WriteEndObject();
                    });
                    return;

                default:
                    EmitLanguageOperation(operation);
                    return;
            }
        }

        public void EmitExpression(SyntaxBase resourceNameSyntax, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = converter.CreateConverterForIndexReplacement(resourceNameSyntax, indexExpression, newContext);

            var expression = converterForContext.ConvertExpression(resourceNameSyntax);
            var serialized = ExpressionSerializer.SerializeExpression(expression);

            writer.WriteValue(serialized);
        }

        public void EmitUnqualifiedResourceId(DeclaredResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = converter.CreateConverterForIndexReplacement(resource.NameSyntax, indexExpression, newContext);

            var unqualifiedResourceId = converterForContext.GetUnqualifiedResourceId(resource);
            var serialized = ExpressionSerializer.SerializeExpression(unqualifiedResourceId);

            writer.WriteValue(serialized);
        }

        public void EmitIndexedSymbolReference(DeclaredResourceMetadata resource, SyntaxBase indexExpression, SyntaxBase newContext)
        {
            var replacementContext = converter.TryGetReplacementContext(resource, indexExpression, newContext);
            var expression = converter.GenerateSymbolicReference(resource.Symbol.Name, replacementContext);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public void EmitIndexedSymbolReference(ModuleSymbol moduleSymbol, SyntaxBase indexExpression, SyntaxBase newContext)
        {
            var replacementContext = converter.TryGetReplacementContext(OperationConverter.GetModuleNameSyntax(moduleSymbol), indexExpression, newContext);
            var expression = converter.GenerateSymbolicReference(moduleSymbol.Name, replacementContext);

            writer.WriteValue(ExpressionSerializer.SerializeExpression(expression));
        }

        public void EmitResourceIdReference(DeclaredResourceMetadata resource, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var nameComponents = SyntaxFactory.CreateArray(this.converter.GetResourceNameSyntaxSegments(resource));

            var converterForContext = this.converter.CreateConverterForIndexReplacement(nameComponents, indexExpression, newContext);

            var resourceIdExpression = converterForContext.GetFullyQualifiedResourceId(resource);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public void EmitResourceIdReference(ModuleSymbol moduleSymbol, SyntaxBase? indexExpression, SyntaxBase newContext)
        {
            var converterForContext = this.converter.CreateConverterForIndexReplacement(OperationConverter.GetModuleNameSyntax(moduleSymbol), indexExpression, newContext);

            var resourceIdExpression = converterForContext.GetFullyQualifiedResourceId(moduleSymbol);
            var serialized = ExpressionSerializer.SerializeExpression(resourceIdExpression);

            writer.WriteValue(serialized);
        }

        public LanguageExpression GetFullyQualifiedResourceName(DeclaredResourceMetadata resource)
        {
            return converter.GetFullyQualifiedResourceName(resource);
        }

        public LanguageExpression GetManagementGroupResourceId(SyntaxBase managementGroupNameProperty, SyntaxBase? indexExpression, SyntaxBase newContext, bool fullyQualified)
        {
            var converterForContext = converter.CreateConverterForIndexReplacement(managementGroupNameProperty, indexExpression, newContext);
            return converterForContext.GenerateManagementGroupResourceId(managementGroupNameProperty, fullyQualified);
        }

        private void EmitLanguageOperation(Operation operation)
        {
            if (operation is ConstantValueOperation constantValueOperation && constantValueOperation.Value is long intValue)
            {
                // the converted expression is an integer literal

                // for integer literals the expression will look like "[42]" or "[-12]"
                // while it's still a valid template expression that works in ARM, it looks weird
                // and is also not recognized by the template language service in VS code
                // let's serialize it as a proper integer instead
                writer.WriteValue(intValue);

                return;
            }

            // strings literals and other expressions must be processed with the serializer to ensure correct conversion and escaping
            var converted = converter.ConvertOperation(operation);
            var serialized = ExpressionSerializer.SerializeExpression(converted);

            writer.WriteValue(serialized);
        }

        public void EmitCopyObject(string? name, Operation forExpression, Operation? input, string? copyIndexOverride = null, long? batchSize = null)
        {
            // local function
            static bool CanEmitAsInputDirectly(Operation input)
            {
                // the deployment engine only allows JTokenType of String or Object in the copy loop "input" property
                // everything else must be converted into an expression
                return input switch
                {
                    // objects should be emitted as is
                    ObjectOperation => true,

                    // constant values should be emitted as-is
                    ConstantValueOperation => true,

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
            this.EmitProperty("count", new FunctionCallOperation("length", new [] { forExpression }.ToImmutableArray()));

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
            var properties = objectSyntax.Properties
                .Where(p => p.TryGetKeyText() is not { } keyName || propertiesToOmit is null || !propertiesToOmit.Contains(keyName))
                .Select(p => new ObjectPropertyOperation(
                    p.TryGetKeyText() is { } keyName ? new ConstantValueOperation(keyName) : converter.GetExpressionOperation(p.Key),
                    converter.GetExpressionOperation(p.Value)));

            var operation = new ObjectOperation(properties.ToImmutableArray());

            EmitObjectProperties(operation);
        }

        private void EmitObjectProperties(ObjectOperation objectOperation)
        {
            var propertyLookup = objectOperation.Properties.ToLookup(property => property.Value is ForLoopOperation);

            // emit loop properties first (if any)
            if (propertyLookup.Contains(true))
            {
                // we have properties whose value is a for-expression
                this.EmitProperty("copy", () =>
                {
                    this.writer.WriteStartArray();

                    foreach (var property in propertyLookup[true])
                    {
                        if (property.Key is not ConstantValueOperation keyValue ||
                            keyValue.Value is not string keyName ||
                            property.Value is not ForLoopOperation forLoop)
                        {
                            // should be caught by loop emit limitation checks
                            throw new InvalidOperationException("Encountered a property with an expression-based key whose value is a for-expression.");
                        }

                        this.EmitCopyObject(keyName, forLoop.Expression, forLoop.Body);
                    }

                    this.writer.WriteEndArray();
                });
            }

            // emit non-loop properties
            foreach (var property in propertyLookup[false])
            {
                // property whose value is not a for-expression
                if (property.Key is ConstantValueOperation constantValueOperation &&
                    constantValueOperation.Value is string keyName)
                {
                    EmitProperty(keyName, () => EmitOperation(property.Value));
                }
                else
                {
                    var keyExpression = converter.ConvertOperation(property.Key);
                    EmitPropertyInternal(keyExpression, () => EmitOperation(property.Value));
                }
            }
        }

        public void EmitModuleParameterValue(Operation value)
        {
            if (value is not GetKeyVaultSecretOperation)
            {
                value = new ObjectPropertyOperation(
                    new ConstantValueOperation("value"),
                    value);
            }

            EmitOperation(value);
        }

        public Operation GetExpressionOperation(SyntaxBase syntax)
            => converter.GetExpressionOperation(syntax);

        public ProgramOperation ConvertProgram(FileSymbol fileSymbol)
            => converter.ConvertProgram(fileSymbol);

        public IEnumerable<ObjectPropertyOperation> GetDecorators(StatementSyntax statement, TypeSymbol targetType)
            => converter.GetDecorators(statement, targetType);

        public void EmitProperty(Operation name, Operation operation)
            => EmitPropertyInternal(converter.ConvertOperation(name), () => EmitOperation(operation));

        public void EmitProperty(string name, Operation operation)
            => EmitProperty(new ConstantValueOperation(name), operation);

        public void EmitPropertyWithTransform(string name, Operation operation, Func<LanguageExpression, LanguageExpression> convertedValueTransform)
            => EmitPropertyInternal(new JTokenExpression(name), () =>
            {
                var converted = converter.ConvertOperation(operation);
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

        public void EmitProperty(string name, Action valueFunc)
            => EmitPropertyInternal(new JTokenExpression(name), valueFunc);

        public void EmitProperty(string name, string value)
            => EmitProperty(name, new ConstantValueOperation(value));

        public void EmitProperty(string name, SyntaxBase syntax)
            => EmitProperty(name, converter.GetExpressionOperation(syntax));

        private void EmitPropertyInternal(LanguageExpression expressionKey, Action valueFunc)
        {
            var serializedName = ExpressionSerializer.SerializeExpression(expressionKey);
            writer.WritePropertyName(serializedName);

            valueFunc();
        }

        public void EmitObjectProperty(string propertyName, Action writePropertiesFunc)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteStartObject();

            writePropertiesFunc();

            writer.WriteEndObject();
        }

        public void EmitArrayProperty(string propertyName, Action writeItemsFunc)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteStartArray();

            writeItemsFunc();

            writer.WriteEndArray();
        }
    }
}

