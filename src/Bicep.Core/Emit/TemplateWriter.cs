// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter : ITemplateWriter
    {
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
        public const string TemplateHashPropertyName = "templateHash";

        // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
        public const string NestedDeploymentResourceApiVersion = "2022-09-01";

        private static readonly ImmutableHashSet<string> DecoratorsToEmitAsResourceProperties = new[] {
            LanguageConstants.ParameterSecurePropertyName,
            LanguageConstants.ParameterAllowedPropertyName,
            LanguageConstants.ParameterMinValuePropertyName,
            LanguageConstants.ParameterMaxValuePropertyName,
            LanguageConstants.ParameterMinLengthPropertyName,
            LanguageConstants.ParameterMaxLengthPropertyName,
            LanguageConstants.ParameterMetadataPropertyName,
            LanguageConstants.MetadataDescriptionPropertyName,
            LanguageConstants.ParameterSealedPropertyName,
        }.ToImmutableHashSet();

        private static ISemanticModel GetModuleSemanticModel(ModuleSymbol moduleSymbol)
        {
            if (!moduleSymbol.TryGetSemanticModel(out var moduleSemanticModel, out _))
            {
                // this should have already been checked during type assignment
                throw new InvalidOperationException($"Unable to find referenced compilation for module {moduleSymbol.Name}");
            }

            return moduleSemanticModel;
        }
        private static string GetSchema(ResourceScope targetScope)
        {
            if (targetScope.HasFlag(ResourceScope.Tenant))
            {
                return "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#";
            }

            if (targetScope.HasFlag(ResourceScope.ManagementGroup))
            {
                return "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#";
            }

            if (targetScope.HasFlag(ResourceScope.Subscription))
            {
                return "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#";
            }

            return "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";
        }

        private EmitterContext Context => ExpressionBuilder.Context;
        private ExpressionBuilder ExpressionBuilder { get; }

        public TemplateWriter(SemanticModel semanticModel)
        {
            ExpressionBuilder = new ExpressionBuilder(new EmitterContext(semanticModel));
        }

        public void Write(SourceAwareJsonTextWriter writer)
        {
            // Template is used for calcualting template hash, template jtoken is used for writing to file.
            var (template, templateJToken) = GenerateTemplateWithoutHash(writer.TrackingJsonWriter);
            var templateHash = TemplateHelpers.ComputeTemplateHash(template.ToJToken());
            if (templateJToken.SelectToken(GeneratorMetadataPath) is not JObject generatorObject)
            {
                throw new InvalidOperationException($"generated template doesn't contain a generator object at the path {GeneratorMetadataPath}");
            }
            generatorObject.Add("templateHash", templateHash);
            templateJToken.WriteTo(writer);
            writer.ProcessSourceMap(templateJToken);
        }

        private (Template, JToken) GenerateTemplateWithoutHash(PositionTrackingJsonTextWriter jsonWriter)
        {
            var emitter = new ExpressionEmitter(jsonWriter, this.Context);
            var program = (ProgramExpression)ExpressionBuilder.Convert(Context.SemanticModel.Root.Syntax);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(Context.SemanticModel.TargetScope));

            if (Context.Settings.EnableSymbolicNames)
            {
                emitter.EmitProperty("languageVersion", "1.10-experimental");
            }

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitMetadata(emitter, program.Metadata);

            this.EmitTypeDefinitionsIfPresent(emitter, program.Types);

            this.EmitUserDefinedFunctions(emitter, program.Functions);

            this.EmitParametersIfPresent(emitter, program.Parameters);

            this.EmitVariablesIfPresent(emitter, program.Variables);

            this.EmitImports(emitter, program.Imports);

            this.EmitResources(jsonWriter, emitter, program.Resources, program.Modules);

            this.EmitOutputsIfPresent(emitter, program.Outputs);

            jsonWriter.WriteEndObject();

            var content = jsonWriter.ToString();
            return (Template.FromJson<Template>(content), content.FromJson<JToken>());
        }

        private void EmitTypeDefinitionsIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredTypeExpression> types)
        {
            if (!types.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("definitions", () => {
                foreach (var type in types)
                {
                    EmitTypeDeclaration(emitter, type);
                }
            });
        }

        private void EmitUserDefinedFunctions(ExpressionEmitter emitter, ImmutableArray<DeclaredFunctionExpression> functions)
        {
            if (!functions.Any())
            {
                return;
            }

            emitter.EmitArrayProperty("functions", () => {
                emitter.EmitObject(() => {
                    emitter.EmitProperty("namespace", EmitConstants.UserDefinedFunctionsNamespace);

                    emitter.EmitObjectProperty("members", () => {
                        foreach (var function in functions)
                        {
                            EmitUserDefinedFunction(emitter, function);
                        }
                    });
                });
            });
        }

        private void EmitParametersIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredParameterExpression> parameters)
        {
            if (!parameters.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("parameters", () => {
                foreach (var parameter in parameters)
                {
                    EmitParameter(emitter, parameter);
                }
            });
        }

        private ObjectExpression AddDecoratorsToBody(DecorableSyntax decorated, ObjectExpression input, TypeSymbol targetType)
        {
            var result = input;
            foreach (var decoratorSyntax in decorated.Decorators.Reverse())
            {
                var symbol = this.Context.SemanticModel.GetSymbolInfo(decoratorSyntax.Expression);

                if (symbol is FunctionSymbol decoratorSymbol &&
                    decoratorSymbol.DeclaringObject is NamespaceType namespaceType &&
                    DecoratorsToEmitAsResourceProperties.Contains(decoratorSymbol.Name))
                {
                    var argumentTypes = decoratorSyntax.Arguments
                        .Select(argument => this.Context.SemanticModel.TypeManager.GetTypeInfo(argument))
                        .ToArray();

                    // There should be exact one matching decorator since there's no errors.
                    var decorator = namespaceType.DecoratorResolver.GetMatches(decoratorSymbol, argumentTypes).Single();

                    var functionCall = ExpressionBuilder.Convert(decoratorSyntax.Expression) as FunctionCallExpression
                        ?? throw new InvalidOperationException($"Failed to convert decorator expression {decoratorSyntax.Expression.GetType()}");

                    var evaluated = decorator.Evaluate(functionCall, targetType, result);
                    if (evaluated is not null)
                    {
                        result = evaluated;
                    }
                }
            }

            return result;
        }

        private void EmitParameter(ExpressionEmitter emitter, DeclaredParameterExpression parameter)
        {
            emitter.EmitObjectProperty(parameter.Name, () =>
            {
                var parameterObject = TypePropertiesForTypeExpression(parameter.Type);

                if (parameter.DefaultValue is not null)
                {
                    parameterObject = parameterObject.MergeProperty("defaultValue", parameter.DefaultValue);
                }

                EmitProperties(emitter, AddDecoratorsToBody(parameter.Symbol.DeclaringParameter, parameterObject, parameter.Symbol.Type));
            }, parameter.SourceSyntax);
        }

        private void EmitUserDefinedFunction(ExpressionEmitter emitter, DeclaredFunctionExpression function)
        {
            if (function.Lambda is not LambdaExpression lambda)
            {
                throw new ArgumentException("Invalid function expression lambda encountered.");
            }

            emitter.EmitObjectProperty(function.Name, () =>
            {
                emitter.EmitArrayProperty("parameters", () => {
                    for (var i = 0; i < lambda.Parameters.Length; i++)
                    {
                        var parameterObject = TypePropertiesForTypeExpression(lambda.ParameterTypes[i]!);
                        parameterObject = parameterObject.MergeProperty("name", new StringLiteralExpression(null, lambda.Parameters[i]));

                        emitter.EmitObject(() => {
                            EmitProperties(emitter, parameterObject);
                        });
                    }
                });

                emitter.EmitObjectProperty("output", () => {
                    var outputObject = TypePropertiesForTypeExpression(lambda.OutputType!);
                    outputObject = outputObject.MergeProperty("value", lambda.Body);

                    EmitProperties(emitter, outputObject);
                });
            }, function.SourceSyntax);
        }

        private void EmitProperties(ExpressionEmitter emitter, ObjectExpression objectExpression)
        {
            foreach (var property in objectExpression.Properties)
            {
                if (property.TryGetKeyText() is string propertyName)
                {
                    emitter.EmitProperty(propertyName, property.Value);
                }
            }
        }

        private void EmitTypeDeclaration(ExpressionEmitter emitter, DeclaredTypeExpression declaredType)
        {
            emitter.EmitObjectProperty(declaredType.Name,
                () =>
                {
                    EmitProperties(emitter, AddDecoratorsToBody(declaredType.Symbol.DeclaringType, TypePropertiesForTypeExpression(declaredType.Value), declaredType.Symbol.Type));
                },
                declaredType.SourceSyntax);
        }

        private ObjectExpression TypePropertiesForTypeExpression(TypeExpression typeExpression) => typeExpression switch
        {
            // references
            AmbientTypeReferenceExpression ambientTypeReference
                => ExpressionFactory.CreateObject(TypeProperty(ambientTypeReference.Symbol.Name, ambientTypeReference.SourceSyntax).AsEnumerable(),
                    ambientTypeReference.SourceSyntax),
            FullyQualifiedAmbientTypeReferenceExpression fullyQualifiedAmbientTypeReference
                => TypePropertiesForQualifiedReference(fullyQualifiedAmbientTypeReference),
            TypeAliasReferenceExpression typeAliasReference=> ExpressionFactory.CreateObject(ExpressionFactory.CreateObjectProperty("$ref",
                ExpressionFactory.CreateStringLiteral($"#/definitions/{typeAliasReference.Symbol.Name}")).AsEnumerable(),
                    typeAliasReference.SourceSyntax),

            // literals
            StringLiteralTypeExpression @string => ExpressionFactory.CreateObject(
                new[]
                {
                    TypeProperty(LanguageConstants.TypeNameString, @string.SourceSyntax),
                    AllowedValuesProperty(SingleElementArray(ExpressionFactory.CreateStringLiteral(@string.Value, @string.SourceSyntax)),
                        @string.SourceSyntax),
                },
                @string.SourceSyntax),
            IntegerLiteralTypeExpression @int => ExpressionFactory.CreateObject(
                new[]
                {
                    TypeProperty(LanguageConstants.TypeNameInt, @int.SourceSyntax),
                    AllowedValuesProperty(SingleElementArray(ExpressionFactory.CreateIntegerLiteral(@int.Value, @int.SourceSyntax)),
                        @int.SourceSyntax),
                },
                @int.SourceSyntax),
            BooleanLiteralTypeExpression @bool => ExpressionFactory.CreateObject(
                new[]
                {
                    TypeProperty(LanguageConstants.TypeNameBool, @bool.SourceSyntax),
                    AllowedValuesProperty(SingleElementArray(ExpressionFactory.CreateBooleanLiteral(@bool.Value, @bool.SourceSyntax)),
                        @bool.SourceSyntax),
                },
                @bool.SourceSyntax),
            UnionTypeExpression unionType => GetTypePropertiesForUnionTypeExpression(unionType),

            // resource types
            ResourceTypeExpression resourceType => GetTypePropertiesForResourceType(resourceType),

            // aggregate types
            ArrayTypeExpression arrayType => GetTypePropertiesForArrayType(arrayType),
            ObjectTypeExpression objectType => GetTypePropertiesForObjectType(objectType),
            TupleTypeExpression tupleType => GetTypePropertiesForTupleType(tupleType),
            NullableTypeExpression nullableType => TypePropertiesForTypeExpression(nullableType.BaseExpression)
                .MergeProperty("nullable", ExpressionFactory.CreateBooleanLiteral(true, nullableType.SourceSyntax)),
            NonNullableTypeExpression nonNullableType => TypePropertiesForTypeExpression(nonNullableType.BaseExpression)
                .MergeProperty("nullable", ExpressionFactory.CreateBooleanLiteral(false, nonNullableType.SourceSyntax)),

            // this should have been caught by the parser
            _ => throw new ArgumentException("Invalid type expression encountered."),
        };

        private ObjectExpression TypePropertiesForQualifiedReference(FullyQualifiedAmbientTypeReferenceExpression qualifiedAmbientType)
        {
            if (qualifiedAmbientType.Namespace.Type.ProviderName != SystemNamespaceType.BuiltInName)
            {
                throw new ArgumentException("Property access base expression did not resolve to the 'sys' namespace.");
            }

            return ExpressionFactory.CreateObject(TypeProperty(qualifiedAmbientType.NamespaceProperty.Name, qualifiedAmbientType.SourceSyntax).AsEnumerable(),
                qualifiedAmbientType.SourceSyntax);
        }

        private ObjectPropertyExpression TypeProperty(string typeName, SyntaxBase? sourceSyntax)
            => Property("type", new StringLiteralExpression(sourceSyntax, typeName), sourceSyntax);

        private ObjectPropertyExpression AllowedValuesProperty(ArrayExpression allowedValues, SyntaxBase? sourceSyntax)
            => Property("allowedValues", allowedValues, sourceSyntax);

        private ObjectPropertyExpression Property(string name, Expression value, SyntaxBase? sourceSyntax)
            => ExpressionFactory.CreateObjectProperty(name, value, sourceSyntax);

        private ObjectExpression GetTypePropertiesForResourceType(ResourceTypeExpression expression)
        {
            var typeString = expression.ExpressedResourceType.TypeReference.FormatName();

            return ExpressionFactory.CreateObject(new[]
            {
                TypeProperty(LanguageConstants.TypeNameString, expression.SourceSyntax),
                ExpressionFactory.CreateObjectProperty(LanguageConstants.ParameterMetadataPropertyName,
                    ExpressionFactory.CreateObject(
                        ExpressionFactory.CreateObjectProperty(LanguageConstants.MetadataResourceTypePropertyName,
                            ExpressionFactory.CreateStringLiteral(typeString, expression.SourceSyntax),
                            expression.SourceSyntax).AsEnumerable(),
                        expression.SourceSyntax),
                    expression.SourceSyntax),
            });
        }

        private ObjectExpression GetTypePropertiesForArrayType(ArrayTypeExpression expression)
        {
            var properties = new List<ObjectPropertyExpression> { TypeProperty(LanguageConstants.ArrayType, expression.SourceSyntax) };

            if (TryGetAllowedValues(expression.BaseExpression) is {} allowedValues)
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("allowedValues", allowedValues, expression.BaseExpression.SourceSyntax));
            }
            else
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("items", TypePropertiesForTypeExpression(expression.BaseExpression)));
            }

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private ArrayExpression? TryGetAllowedValues(TypeExpression expression) => expression switch
        {
            StringLiteralTypeExpression @string => SingleElementArray(ExpressionFactory.CreateStringLiteral(@string.Value, @string.SourceSyntax)),
            IntegerLiteralTypeExpression @int => SingleElementArray(ExpressionFactory.CreateIntegerLiteral(@int.Value, @int.SourceSyntax)),
            BooleanLiteralTypeExpression @bool => SingleElementArray(ExpressionFactory.CreateBooleanLiteral(@bool.Value, @bool.SourceSyntax)),
            NullLiteralTypeExpression @null => SingleElementArray(new NullLiteralExpression(@null.SourceSyntax)),
            ObjectTypeExpression @object when TypeHelper.IsLiteralType(@object.ExpressedType) => SingleElementArray(ToLiteralValue(@object.ExpressedType)),
            TupleTypeExpression tuple when TypeHelper.IsLiteralType(tuple.ExpressedType) => SingleElementArray(ToLiteralValue(tuple.ExpressedType)),
            UnionTypeExpression union => GetAllowedValuesForUnionType(union.ExpressedUnionType, union.SourceSyntax),
            _ => null,
        };

        private ArrayExpression SingleElementArray(Expression expression) => ExpressionFactory.CreateArray(expression.AsEnumerable());

        private ArrayExpression GetAllowedValuesForUnionType(UnionType unionType, SyntaxBase? sourceSyntax)
            => ExpressionFactory.CreateArray(unionType.Members.Select(ToLiteralValue), sourceSyntax);

        private ObjectExpression GetTypePropertiesForObjectType(ObjectTypeExpression expression)
        {
            var properties = new List<ObjectPropertyExpression> { TypeProperty(LanguageConstants.ObjectType, expression.SourceSyntax) };
            List<ObjectPropertyExpression> propertySchemata = new();

            foreach (var property in expression.PropertyExpressions)
            {
                var propertySchema = TypePropertiesForTypeExpression(property.Value);
                if (property.SourceSyntax is DecorableSyntax decorable)
                {
                    propertySchema = AddDecoratorsToBody(decorable, propertySchema, property.Value.ExpressedType);
                }

                propertySchemata.Add(ExpressionFactory.CreateObjectProperty(property.PropertyName, propertySchema, property.SourceSyntax));
            }

            if (propertySchemata.Any())
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("properties", ExpressionFactory.CreateObject(propertySchemata, expression.SourceSyntax)));
            }

            if (expression.AdditionalPropertiesExpression is { } addlPropsType)
            {
                var addlPropertiesSchema = TypePropertiesForTypeExpression(addlPropsType.Value);
                if (addlPropsType.SourceSyntax is DecorableSyntax decorable)
                {
                    addlPropertiesSchema = AddDecoratorsToBody(decorable, addlPropertiesSchema, addlPropsType.Value.ExpressedType);
                }

                properties.Add(ExpressionFactory.CreateObjectProperty("additionalProperties", addlPropertiesSchema, addlPropsType.SourceSyntax));
            }

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private ObjectExpression GetTypePropertiesForTupleType(TupleTypeExpression expression) => ExpressionFactory.CreateObject(new[]
        {
            TypeProperty(LanguageConstants.ArrayType, expression.SourceSyntax),
            ExpressionFactory.CreateObjectProperty("prefixItems",
                ExpressionFactory.CreateArray(
                    expression.ItemExpressions.Select(item => (item, TypePropertiesForTypeExpression(item.Value)))
                        .Select(t => t.item.SourceSyntax is DecorableSyntax decorable
                            ? AddDecoratorsToBody(decorable, t.Item2, t.item.Value.ExpressedType)
                            : t.Item2),
                    expression.SourceSyntax),
                expression.SourceSyntax),
            ExpressionFactory.CreateObjectProperty("items", ExpressionFactory.CreateBooleanLiteral(false), expression.SourceSyntax),
        });

        private ObjectExpression GetTypePropertiesForUnionTypeExpression(UnionTypeExpression expression)
        {
            (var nullable, var nonLiteralTypeName) = TypeHelper.TryRemoveNullability(expression.ExpressedUnionType) switch
            {
                UnionType nonNullableUnion => (true, GetNonLiteralTypeName(nonNullableUnion.Members.First().Type)),
                TypeSymbol nonNullable => (true, GetNonLiteralTypeName(nonNullable)),
                _ => (false, GetNonLiteralTypeName(expression.ExpressedUnionType.Members.First().Type)),
            };

            var properties = new List<ObjectPropertyExpression>
            {
                TypeProperty(nonLiteralTypeName, expression.SourceSyntax),
                ExpressionFactory.CreateObjectProperty("allowedValues",
                    GetAllowedValuesForUnionType(expression.ExpressedUnionType, expression.SourceSyntax),
                    expression.SourceSyntax),
            };

            if (nullable)
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("nullable", ExpressionFactory.CreateBooleanLiteral(true), expression.SourceSyntax));
            }

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private Expression ToLiteralValue(ITypeReference literalType) => literalType.Type switch
        {
            StringLiteralType @string => ExpressionFactory.CreateStringLiteral(@string.RawStringValue),
            IntegerLiteralType @int => new IntegerLiteralExpression(null, @int.Value),
            BooleanLiteralType @bool => ExpressionFactory.CreateBooleanLiteral(@bool.Value),
            NullType => new NullLiteralExpression(null),
            ObjectType @object => ExpressionFactory.CreateObject(@object.Properties.Select(kvp => ExpressionFactory.CreateObjectProperty(kvp.Key, ToLiteralValue(kvp.Value.TypeReference)))),
            TupleType tuple => ExpressionFactory.CreateArray(tuple.Items.Select(ToLiteralValue)),
            // This would have been caught by the DeclaredTypeManager during initial type assignment
            _ => throw new ArgumentException("Union types used in ARM type checks must be composed entirely of literal types"),
        };

        private string GetNonLiteralTypeName(TypeSymbol? type) => type switch
        {
            StringLiteralType or StringType => "string",
            IntegerLiteralType or IntegerType => "int",
            BooleanLiteralType or BooleanType => "bool",
            ObjectType => "object",
            ArrayType => "array",
            // This would have been caught by the DeclaredTypeManager during initial type assignment
            _ => throw new ArgumentException("Unresolvable type name"),
        };

        private void EmitVariablesIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredVariableExpression> variables)
        {
            if (!variables.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("variables", () => {
                var loopVariables = variables.Where(x => x is { Value: ForLoopExpression });
                var nonLoopVariables = variables.Where(x => x is { Value: not ForLoopExpression });

                if (loopVariables.Any())
                {
                    // we have variables whose values are loops
                    emitter.EmitArrayProperty("copy", () => {
                        foreach (var loopVariable in loopVariables)
                        {
                            var forLoopVariable = (ForLoopExpression)loopVariable.Value;
                            emitter.EmitCopyObject(loopVariable.Name, forLoopVariable.Expression, forLoopVariable.Body);
                        }
                    });
                }

                // emit non-loop variables
                foreach (var variable in nonLoopVariables)
                {
                    emitter.EmitProperty(variable.Name, variable.Value);
                }
            });
        }

        private void EmitImports(ExpressionEmitter emitter, ImmutableArray<DeclaredImportExpression> imports)
        {
            if (!imports.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("imports", () => {
                foreach (var import in imports)
                {
                    var settings = import.NamespaceType.Settings;

                    emitter.EmitObjectProperty(import.Name, () =>
                    {
                        emitter.EmitProperty("provider", settings.ArmTemplateProviderName);
                        emitter.EmitProperty("version", settings.ArmTemplateProviderVersion);
                        if (import.Config is not null)
                        {
                            emitter.EmitProperty("config", import.Config);
                        }
                    }, import.SourceSyntax);
                }
            });
        }

        private void EmitResources(
            PositionTrackingJsonTextWriter jsonWriter,
            ExpressionEmitter emitter,
            ImmutableArray<DeclaredResourceExpression> resources,
            ImmutableArray<DeclaredModuleExpression> modules)
        {
            if (!Context.Settings.EnableSymbolicNames)
            {
                emitter.EmitArrayProperty("resources", () => {
                    foreach (var resource in resources)
                    {
                        if (resource.Metadata.IsExistingResource)
                        {
                            continue;
                        }

                        this.EmitResource(emitter, resource);
                    }

                    foreach (var module in modules)
                    {
                        this.EmitModule(jsonWriter, module, emitter);
                    }
                });
            }
            else
            {
                emitter.EmitObjectProperty("resources", () => {
                    foreach (var resource in resources)
                    {
                        emitter.EmitProperty(
                            emitter.GetSymbolicName(resource.Metadata),
                            () => EmitResource(emitter, resource),
                            resource.SourceSyntax);
                    }

                    foreach (var module in modules)
                    {
                        emitter.EmitProperty(
                            module.Symbol.Name,
                            () => EmitModule(jsonWriter, module, emitter),
                            module.SourceSyntax);
                    }
                });
            }
        }

        private void EmitResource(ExpressionEmitter emitter, DeclaredResourceExpression resource)
        {
            var metadata = resource.Metadata;

            emitter.EmitObject(() => {
                var body = resource.Body;
                if (body is ForLoopExpression forLoop)
                {
                    body = forLoop.Body;
                    emitter.EmitCopyProperty(() => emitter.EmitCopyObject(forLoop.Name, forLoop.Expression, input: null, batchSize: forLoop.BatchSize));
                }
                if (body is ConditionExpression condition)
                {
                    body = condition.Body;
                    emitter.EmitProperty("condition", condition.Expression);
                }

                if (Context.Settings.EnableSymbolicNames && metadata.IsExistingResource)
                {
                    emitter.EmitProperty("existing", new BooleanLiteralExpression(null, true));
                }

                var importSymbol = Context.SemanticModel.Root.ImportDeclarations.FirstOrDefault(i => metadata.Type.DeclaringNamespace.AliasNameEquals(i.Name));
                if (importSymbol is not null)
                {
                    emitter.EmitProperty("import", importSymbol.Name);
                }

                if (metadata.IsAzResource)
                {
                    emitter.EmitProperty("type", metadata.TypeReference.FormatType());
                    if (metadata.TypeReference.ApiVersion is not null)
                    {
                        emitter.EmitProperty("apiVersion", metadata.TypeReference.ApiVersion);
                    }
                }
                else
                {
                    emitter.EmitProperty("type", metadata.TypeReference.FormatName());
                }

                ExpressionBuilder.EmitResourceScopeProperties(emitter, resource);

                if (metadata.IsAzResource)
                {
                    emitter.EmitProperty(AzResourceTypeProvider.ResourceNamePropertyName, emitter.GetFullyQualifiedResourceName(metadata));
                    emitter.EmitObjectProperties((ObjectExpression)body);
                }
                else
                {
                    emitter.EmitObjectProperty("properties", () => {
                        emitter.EmitObjectProperties((ObjectExpression)body);
                    });
                }

                this.EmitDependsOn(emitter, resource.DependsOn);

                // Since we don't want to be mutating the body of the original ObjectSyntax, we create an placeholder body in place
                // and emit its properties to merge decorator properties.
                foreach (var property in AddDecoratorsToBody(
                    metadata.Symbol.DeclaringResource,
                    ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty),
                    metadata.Symbol.Type).Properties)
                {
                    emitter.EmitProperty(property);
                }
            }, resource.SourceSyntax);
        }

        private void EmitModuleParameters(ExpressionEmitter emitter, DeclaredModuleExpression module)
        {
            if (module.Parameters is not ObjectExpression paramsObject)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            emitter.EmitObjectProperty("parameters", () => {
                foreach (var property in paramsObject.Properties)
                {
                    if (property.TryGetKeyText() is not {} keyName)
                    {
                        // should have been caught by earlier validation
                        throw new ArgumentException("Disallowed interpolation in module parameter");
                    }

                    // we can't just call EmitObjectProperties here because the ObjectSyntax is flatter than the structure we're generating
                    // because nested deployment parameters are objects with a single value property
                    if (property.Value is ForLoopExpression @for)
                    {
                        // the value is a for-expression
                        // write a single property copy loop
                        emitter.EmitObjectProperty(keyName, () => {
                            emitter.EmitCopyProperty(() =>
                            {
                                emitter.EmitArray(() => {
                                    emitter.EmitCopyObject("value", @for.Expression, @for.Body, "value");
                                }, @for.SourceSyntax);
                            });
                        });
                    }
                    else if (property.Value is ResourceReferenceExpression resource &&
                        module.Symbol.TryGetModuleType() is ModuleType moduleType &&
                        moduleType.TryGetParameterType(keyName) is ResourceParameterType parameterType)
                    {
                        // This is a resource being passed into a module, we actually want to pass in its id
                        // rather than the whole resource.
                        var idExpression = new PropertyAccessExpression(resource.SourceSyntax, resource, "id", AccessExpressionFlags.None);
                        emitter.EmitProperty(keyName, ExpressionEmitter.ConvertModuleParameter(idExpression));
                    }
                    else
                    {
                        // the value is not a for-expression - can emit normally
                        emitter.EmitProperty(keyName, ExpressionEmitter.ConvertModuleParameter(property.Value));
                    }
                }
            }, paramsObject.SourceSyntax);
        }

        private void EmitModule(PositionTrackingJsonTextWriter jsonWriter, DeclaredModuleExpression module, ExpressionEmitter emitter)
        {
            var moduleSymbol = module.Symbol;
            emitter.EmitObject(() =>
            {
                var body = module.Body;
                if (body is ForLoopExpression forLoop)
                {
                    body = forLoop.Body;
                    emitter.EmitCopyProperty(() => emitter.EmitCopyObject(forLoop.Name, forLoop.Expression, input: null, batchSize: forLoop.BatchSize));
                }
                if (body is ConditionExpression condition)
                {
                    body = condition.Body;
                    emitter.EmitProperty("condition", condition.Expression);
                }

                emitter.EmitProperty("type", NestedDeploymentResourceType);
                emitter.EmitProperty("apiVersion", NestedDeploymentResourceApiVersion);

                // emit all properties apart from 'params'. In practice, this currrently only allows 'name', but we may choose to allow other top-level resource properties in future.
                // params requires special handling (see below).
                emitter.EmitObjectProperties((ObjectExpression)body);

                ExpressionBuilder.EmitModuleScopeProperties(emitter, module);

                if (module.ScopeData.RequestedScope != ResourceScope.ResourceGroup)
                {
                    // if we're deploying to a scope other than resource group, we need to supply a location
                    if (this.Context.SemanticModel.TargetScope == ResourceScope.ResourceGroup)
                    {
                        // the deployment() object at resource group scope does not contain a property named 'location', so we have to use resourceGroup().location
                        emitter.EmitProperty("location", new PropertyAccessExpression(
                            null,
                            new FunctionCallExpression(null, "resourceGroup", ImmutableArray<Expression>.Empty),
                            "location",
                            AccessExpressionFlags.None));
                    }
                    else
                    {
                        // at all other scopes we can just use deployment().location
                        emitter.EmitProperty("location", new PropertyAccessExpression(
                            null,
                            new FunctionCallExpression(null, "deployment", ImmutableArray<Expression>.Empty),
                            "location",
                            AccessExpressionFlags.None));
                    }
                }

                emitter.EmitObjectProperty("properties", () =>
                {
                    emitter.EmitObjectProperty("expressionEvaluationOptions", () =>
                    {
                        emitter.EmitProperty("scope", "inner");
                    });

                    emitter.EmitProperty("mode", "Incremental");

                    EmitModuleParameters(emitter, module);

                    var moduleSemanticModel = GetModuleSemanticModel(moduleSymbol);

                    // If it is a template spec module, emit templateLink instead of template contents.
                    jsonWriter.WritePropertyName(moduleSemanticModel is TemplateSpecSemanticModel ? "templateLink" : "template");
                    {
                        var moduleWriter = TemplateWriterFactory.CreateTemplateWriter(moduleSemanticModel);
                        var moduleBicepFile = (moduleSemanticModel as SemanticModel)?.SourceFile;
                        var moduleTextWriter = new StringWriter();
                        var moduleJsonWriter = new SourceAwareJsonTextWriter(this.Context.SemanticModel.FileResolver, moduleTextWriter, moduleBicepFile);

                        moduleWriter.Write(moduleJsonWriter);
                        jsonWriter.AddNestedSourceMap(moduleJsonWriter.TrackingJsonWriter);

                        var nestedTemplate = moduleTextWriter.ToString().FromJson<JToken>();
                        nestedTemplate.WriteTo(jsonWriter);
                    }
                });

                this.EmitDependsOn(emitter, module.DependsOn);

                // Since we don't want to be mutating the body of the original ObjectSyntax, we create an placeholder body in place
                // and emit its properties to merge decorator properties.
                foreach (var property in AddDecoratorsToBody(
                    moduleSymbol.DeclaringModule,
                    ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty),
                    moduleSymbol.Type).Properties)
                {
                    emitter.EmitProperty(property);
                }
            }, module.SourceSyntax);
        }

        private void EmitSymbolicNameDependsOnEntry(ExpressionEmitter emitter, ResourceDependencyExpression dependency)
        {
            switch (dependency.Reference)
            {
                case ResourceReferenceExpression { Metadata: DeclaredResourceMetadata resource } reference:
                    switch ((resource.Symbol.IsCollection, reference.IndexContext?.Index))
                    {
                        case (false, var index):
                            emitter.EmitSymbolReference(resource);
                            Debug.Assert(index is null);
                            break;
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        case (true, null):
                            emitter.EmitSymbolReference(resource);
                            break;
                        case (true, { } index):
                            emitter.EmitIndexedSymbolReference(resource, reference.IndexContext);
                            break;
                    }
                    break;
                case ModuleReferenceExpression { Module: ModuleSymbol module } reference:
                    switch ((module.IsCollection, reference.IndexContext?.Index))
                    {
                        case (false, var index):
                            emitter.EmitExpression(new StringLiteralExpression(null, module.Name));
                            Debug.Assert(index is null);
                            break;
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        case (true, null):
                            emitter.EmitExpression(new StringLiteralExpression(null, module.Name));
                            break;
                        case (true, { } index):
                            emitter.EmitIndexedSymbolReference(module, reference.IndexContext);
                            break;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Found dependency of unexpected type {dependency.GetType()}");
            }
        }

        private void EmitClassicDependsOnEntry(ExpressionEmitter emitter, ResourceDependencyExpression dependency)
        {
            switch (dependency.Reference)
            {
                case ResourceReferenceExpression { Metadata: DeclaredResourceMetadata resource } reference:
                    if (resource.Symbol.IsCollection && reference.IndexContext?.Index is null)
                    {
                        // dependency is on the entire resource collection
                        // write the name of the resource collection as the dependency
                        emitter.EmitExpression(new StringLiteralExpression(null, resource.Symbol.DeclaringResource.Name.IdentifierName));

                        break;
                    }

                    emitter.EmitResourceIdReference(resource, reference.IndexContext);
                    break;
                case ModuleReferenceExpression { Module: ModuleSymbol module } reference:
                    if (module.IsCollection && reference.IndexContext?.Index is null)
                    {
                        // dependency is on the entire module collection
                        // write the name of the module collection as the dependency
                        emitter.EmitExpression(new StringLiteralExpression(null, module.DeclaringModule.Name.IdentifierName));

                        break;
                    }

                    emitter.EmitResourceIdReference(module, reference.IndexContext);

                    break;
                default:
                    throw new InvalidOperationException($"Found dependency of unexpected type {dependency.GetType()}");
            }
        }

        private void EmitDependsOn(ExpressionEmitter emitter, ImmutableArray<ResourceDependencyExpression> dependencies)
        {
            if (!dependencies.Any())
            {
                return;
            }

            emitter.EmitArrayProperty("dependsOn", () => {
                foreach (var dependency in dependencies)
                {
                    if (Context.Settings.EnableSymbolicNames)
                    {
                        EmitSymbolicNameDependsOnEntry(emitter, dependency);
                    }
                    else
                    {
                        EmitClassicDependsOnEntry(emitter, dependency);
                    }
                }
            });
        }

        private void EmitOutputsIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredOutputExpression> outputs)
        {
            if (!outputs.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("outputs", () => {
                foreach (var output in outputs)
                {
                    EmitOutput(emitter, output);
                }
            });
        }

        private void EmitOutput(ExpressionEmitter emitter, DeclaredOutputExpression output)
        {
            emitter.EmitObjectProperty(output.Name, () =>
            {
                EmitProperties(emitter, AddDecoratorsToBody(output.Symbol.DeclaringOutput,
                    TypePropertiesForTypeExpression(output.Type),
                    output.Symbol.Type));

                if (output.Value is ForLoopExpression @for)
                {
                    emitter.EmitCopyProperty(() => emitter.EmitCopyObject(name: null, @for.Expression, @for.Body));
                }
                else if (output.Symbol.Type is ResourceType)
                {
                    // Resource-typed outputs are serialized using the resource id.
                    var value = new PropertyAccessExpression(output.SourceSyntax, output.Value, "id", AccessExpressionFlags.None);

                    emitter.EmitProperty("value", value);
                }
                else
                {
                    emitter.EmitProperty("value", output.Value);
                }
            }, output.SourceSyntax);
        }

        public void EmitMetadata(ExpressionEmitter emitter, ImmutableArray<DeclaredMetadataExpression> metadata)
        {
            emitter.EmitObjectProperty("metadata", () => {
                if (Context.Settings.EnableSymbolicNames)
                {
                    emitter.EmitProperty("_EXPERIMENTAL_WARNING", "Symbolic name support in ARM is experimental, and should be enabled for testing purposes only. Do not enable this setting for any production usage, or you may be unexpectedly broken at any time!");
                }

                emitter.EmitObjectProperty("_generator", () => {
                    emitter.EmitProperty("name", LanguageConstants.LanguageId);
                    emitter.EmitProperty("version", this.Context.SemanticModel.Features.AssemblyVersion);
                });

                foreach (var item in metadata)
                {
                    emitter.EmitProperty(item.Name, item.Value);
                }
            });
        }
    }
}
