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
using Bicep.Core.Emit.CompileTimeImports;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    // TODO: Are there discrepancies between parameter, variable, and output names between bicep and ARM?
    public class TemplateWriter : ITemplateWriter
    {
        public const string GeneratorMetadataPath = "metadata._generator";
        public const string NestedDeploymentResourceType = AzResourceTypeProvider.ResourceTypeDeployments;
        public const string TemplateHashPropertyName = "templateHash";
        public const string LanguageVersionPropertyName = "languageVersion";
        private const string TypePropertyName = "type";

        // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
        public const string NestedDeploymentResourceApiVersion = "2022-09-01";

        private static ISemanticModel GetModuleSemanticModel(ModuleSymbol moduleSymbol)
        {
            if (!moduleSymbol.TryGetSemanticModel().IsSuccess(out var moduleSemanticModel))
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
        private ImportClosureInfo ImportClosureInfo { get; }
        private ImmutableDictionary<string, DeclaredTypeExpression> declaredTypesByName;

        public TemplateWriter(SemanticModel semanticModel)
        {
            ExpressionBuilder = new ExpressionBuilder(new EmitterContext(semanticModel));
            ImportClosureInfo = ImportClosureInfo.Calculate(semanticModel);
            declaredTypesByName = ImmutableDictionary<string, DeclaredTypeExpression>.Empty;
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

        public (Template, JToken) GetTemplate(SourceAwareJsonTextWriter writer)
        {
            return GenerateTemplateWithoutHash(writer.TrackingJsonWriter);
        }


        private (Template, JToken) GenerateTemplateWithoutHash(PositionTrackingJsonTextWriter jsonWriter)
        {
            var emitter = new ExpressionEmitter(jsonWriter, this.Context);
            var program = (ProgramExpression)ExpressionBuilder.Convert(Context.SemanticModel.Root.Syntax);

            var programTypes = program.Types.Concat(ImportClosureInfo.ImportedTypesInClosure);
            declaredTypesByName = programTypes.ToImmutableDictionary(t => t.Name);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(Context.SemanticModel.TargetScope));

            if (Context.Settings.UseExperimentalTemplateLanguageVersion)
            {
                emitter.EmitProperty(LanguageVersionPropertyName, "2.1-experimental");
            }
            else if (Context.Settings.EnableSymbolicNames)
            {
                emitter.EmitProperty(LanguageVersionPropertyName, "2.0");
            }

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitMetadata(emitter, program.Metadata);

            this.EmitTypeDefinitionsIfPresent(emitter, programTypes);

            this.EmitUserDefinedFunctions(emitter, program.Functions);

            this.EmitParametersIfPresent(emitter, program.Parameters);

            this.EmitVariablesIfPresent(emitter, program.Variables);

            this.EmitProviders(emitter, program.Providers);

            this.EmitResources(jsonWriter, emitter, program.Resources, program.Modules);

            this.EmitOutputsIfPresent(emitter, program.Outputs);

            this.EmitAssertsIfPresent(emitter, program.Asserts);

            jsonWriter.WriteEndObject();

            var content = jsonWriter.ToString();
            return (Template.FromJson<Template>(content), content.FromJson<JToken>());
        }

        private void EmitTypeDefinitionsIfPresent(ExpressionEmitter emitter, IEnumerable<DeclaredTypeExpression> types)
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

        private static ObjectExpression ApplyTypeModifiers(TypeDeclaringExpression expression, ObjectExpression input)
        {
            var result = input;

            if (expression.Secure is {} secure)
            {
                result = result.Properties.Where(p => p.Key is StringLiteralExpression { Value: string name} && name == "type").Single().Value switch
                {
                    StringLiteralExpression { Value: string typeName } when typeName == LanguageConstants.TypeNameString
                        => result.MergeProperty("type", ExpressionFactory.CreateStringLiteral("securestring", secure.SourceSyntax)),
                    StringLiteralExpression { Value: string typeName } when typeName == LanguageConstants.ObjectType
                        => result.MergeProperty("type", ExpressionFactory.CreateStringLiteral("secureObject", secure.SourceSyntax)),
                    _ => result,
                };
            }

            if (expression.Sealed is {} @sealed)
            {
                result = result.MergeProperty("additionalProperties", ExpressionFactory.CreateBooleanLiteral(false, @sealed.SourceSyntax));
            }

            if (expression is DeclaredTypeExpression declaredTypeExpression && declaredTypeExpression.Exported is {} exported)
            {
                result = ApplyMetadataProperty(result, LanguageConstants.MetadataExportedPropertyName, ExpressionFactory.CreateBooleanLiteral(true, exported.SourceSyntax));
            }

            foreach (var (modifier, propertyName) in new[]
            {
                (expression.Metadata, LanguageConstants.ParameterMetadataPropertyName),
                (expression.MinLength, LanguageConstants.ParameterMinLengthPropertyName),
                (expression.MaxLength, LanguageConstants.ParameterMaxLengthPropertyName),
                (expression.MinValue, LanguageConstants.ParameterMinValuePropertyName),
                (expression.MaxValue, LanguageConstants.ParameterMaxValuePropertyName),
            }) {
                if (modifier is not null)
                {
                    result = result.MergeProperty(propertyName, modifier);
                }
            }

            return ApplyDescription(expression, result);
        }

        private static ObjectExpression ApplyDescription(DescribableExpression expression, ObjectExpression input)
            => ApplyMetadataProperty(input, LanguageConstants.MetadataDescriptionPropertyName, expression.Description);

        private static ObjectExpression ApplyMetadataProperty(ObjectExpression input, string propertyName, Expression? propertyValue) => propertyValue is not null
            ? input.MergeProperty(LanguageConstants.ParameterMetadataPropertyName, ExpressionFactory.CreateObject(
                ExpressionFactory.CreateObjectProperty(propertyName, propertyValue, propertyValue.SourceSyntax).AsEnumerable(),
                propertyValue.SourceSyntax))
            : input;

        private void EmitParameter(ExpressionEmitter emitter, DeclaredParameterExpression parameter)
        {
            emitter.EmitObjectProperty(parameter.Name, () =>
            {
                var parameterObject = TypePropertiesForTypeExpression(parameter.Type);

                if (parameter.DefaultValue is not null)
                {
                    parameterObject = parameterObject.MergeProperty("defaultValue", parameter.DefaultValue);
                }

                if (parameter.AllowedValues is not null)
                {
                    parameterObject = parameterObject.MergeProperty("allowedValues", parameter.AllowedValues);
                }

                EmitProperties(emitter, ApplyTypeModifiers(parameter, parameterObject));
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
                    var declaredTypeObject = ApplyTypeModifiers(declaredType, TypePropertiesForTypeExpression(declaredType.Value));
                    if (ImportClosureInfo.ImportedSymbolOriginMetadata.TryGetValue(declaredType.Name, out var originMetadata))
                    {
                        var importedFromProperties = ExpressionFactory.CreateObjectProperty(LanguageConstants.ImportMetadataSourceTemplatePropertyName,
                            ExpressionFactory.CreateStringLiteral(originMetadata.SourceTemplateIdentifier)).AsEnumerable();
                        if (!declaredType.Name.EndsWith(originMetadata.OriginalName))
                        {
                            importedFromProperties = importedFromProperties.Append(ExpressionFactory.CreateObjectProperty(LanguageConstants.ImportMetadataOriginalIdentifierPropertyName,
                                ExpressionFactory.CreateStringLiteral(originMetadata.OriginalName)));
                        }
                        declaredTypeObject = ApplyMetadataProperty(declaredTypeObject, LanguageConstants.MetadataImportedFromPropertyName, ExpressionFactory.CreateObject(importedFromProperties));
                    }
                    EmitProperties(emitter, declaredTypeObject);
                },
                declaredType.SourceSyntax);
        }

        private ObjectExpression TypePropertiesForTypeExpression(TypeExpression typeExpression) => typeExpression switch
        {
            // references
            AmbientTypeReferenceExpression ambientTypeReference
                => ExpressionFactory.CreateObject(TypeProperty(ambientTypeReference.Name, ambientTypeReference.SourceSyntax).AsEnumerable(),
                    ambientTypeReference.SourceSyntax),
            FullyQualifiedAmbientTypeReferenceExpression fullyQualifiedAmbientTypeReference
                => TypePropertiesForQualifiedReference(fullyQualifiedAmbientTypeReference),
            TypeAliasReferenceExpression typeAliasReference => CreateRefSchemaNode(typeAliasReference.Name, typeAliasReference.SourceSyntax),
            ImportedTypeReferenceExpression importedTypeReference => CreateRefSchemaNode(importedTypeReference.Symbol.Name, importedTypeReference.SourceSyntax),
            WildcardImportPropertyReferenceExpression wildcardProperty => CreateRefSchemaNode(
                ImportClosureInfo.WildcardPropertyReferenceToImportedTypeName[new(wildcardProperty.ImportSymbol, wildcardProperty.PropertyName)],
                wildcardProperty.SourceSyntax),

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
            DiscriminatedObjectTypeExpression discriminatedObject => GetTypePropertiesForDiscriminatedObjectExpression(discriminatedObject),
            // this should have been caught by the parser
            _ => throw new ArgumentException("Invalid type expression encountered."),
        };

        private static ObjectExpression TypePropertiesForQualifiedReference(FullyQualifiedAmbientTypeReferenceExpression qualifiedAmbientType)
        {
            if (qualifiedAmbientType.ProviderName != SystemNamespaceType.BuiltInName)
            {
                throw new ArgumentException("Property access base expression did not resolve to the 'sys' namespace.");
            }

            return ExpressionFactory.CreateObject(TypeProperty(qualifiedAmbientType.Name, qualifiedAmbientType.SourceSyntax).AsEnumerable(),
                qualifiedAmbientType.SourceSyntax);
        }

        private static ObjectExpression CreateRefSchemaNode(string typeName, SyntaxBase? sourceSyntax) => ExpressionFactory.CreateObject(
            ExpressionFactory.CreateObjectProperty("$ref", ExpressionFactory.CreateStringLiteral($"#/definitions/{typeName}", sourceSyntax), sourceSyntax).AsEnumerable(),
            sourceSyntax);

        private static ObjectPropertyExpression TypeProperty(string typeName, SyntaxBase? sourceSyntax)
            => Property(TypePropertyName, new StringLiteralExpression(sourceSyntax, typeName), sourceSyntax);

        private static ObjectPropertyExpression AllowedValuesProperty(ArrayExpression allowedValues, SyntaxBase? sourceSyntax)
            => Property("allowedValues", allowedValues, sourceSyntax);

        private static ObjectPropertyExpression Property(string name, Expression value, SyntaxBase? sourceSyntax)
            => ExpressionFactory.CreateObjectProperty(name, value, sourceSyntax);

        private static ObjectExpression GetTypePropertiesForResourceType(ResourceTypeExpression expression)
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
                properties.Add(AllowedValuesProperty(allowedValues, expression.BaseExpression.SourceSyntax));
            }
            else
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("items", TypePropertiesForTypeExpression(expression.BaseExpression)));
            }

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private static ArrayExpression? TryGetAllowedValues(TypeExpression expression) => expression switch
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

        private static ArrayExpression SingleElementArray(Expression expression) => ExpressionFactory.CreateArray(expression.AsEnumerable());

        private static ArrayExpression GetAllowedValuesForUnionType(UnionType unionType, SyntaxBase? sourceSyntax)
            => ExpressionFactory.CreateArray(unionType.Members.Select(ToLiteralValue), sourceSyntax);

        private ObjectExpression GetTypePropertiesForObjectType(ObjectTypeExpression expression)
        {
            var properties = new List<ObjectPropertyExpression> { TypeProperty(LanguageConstants.ObjectType, expression.SourceSyntax) };
            List<ObjectPropertyExpression> propertySchemata = new();

            foreach (var property in expression.PropertyExpressions)
            {
                var propertySchema = ApplyTypeModifiers(property, TypePropertiesForTypeExpression(property.Value));
                propertySchemata.Add(ExpressionFactory.CreateObjectProperty(property.PropertyName, propertySchema, property.SourceSyntax));
            }

            if (propertySchemata.Any())
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("properties", ExpressionFactory.CreateObject(propertySchemata, expression.SourceSyntax)));
            }

            if (expression.AdditionalPropertiesExpression is { } addlPropsType)
            {
                var addlPropertiesSchema = ApplyTypeModifiers(addlPropsType, TypePropertiesForTypeExpression(addlPropsType.Value));

                properties.Add(ExpressionFactory.CreateObjectProperty("additionalProperties", addlPropertiesSchema, addlPropsType.SourceSyntax));
            }

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private ObjectExpression GetTypePropertiesForTupleType(TupleTypeExpression expression) => ExpressionFactory.CreateObject(new[]
        {
            TypeProperty(LanguageConstants.ArrayType, expression.SourceSyntax),
            ExpressionFactory.CreateObjectProperty("prefixItems",
                ExpressionFactory.CreateArray(
                    expression.ItemExpressions.Select(item => ApplyTypeModifiers(item, TypePropertiesForTypeExpression(item.Value))),
                    expression.SourceSyntax),
                expression.SourceSyntax),
            ExpressionFactory.CreateObjectProperty("items", ExpressionFactory.CreateBooleanLiteral(false), expression.SourceSyntax),
        });

        private ObjectExpression GetTypePropertiesForUnionTypeExpression(UnionTypeExpression expression)
        {
            var (nullable, nonLiteralTypeName, allowedValues) = TypeHelper.TryRemoveNullability(expression.ExpressedUnionType) switch
            {
                UnionType nonNullableUnion => (true, GetNonLiteralTypeName(nonNullableUnion.Members.First().Type), GetAllowedValuesForUnionType(nonNullableUnion, expression.SourceSyntax)),
                TypeSymbol nonNullable => (true, GetNonLiteralTypeName(nonNullable), SingleElementArray(ToLiteralValue(nonNullable))),
                _ => (false, GetNonLiteralTypeName(expression.ExpressedUnionType.Members.First().Type), GetAllowedValuesForUnionType(expression.ExpressedUnionType, expression.SourceSyntax)),
            };

            var properties = new List<ObjectPropertyExpression>
            {
                TypeProperty(nonLiteralTypeName, expression.SourceSyntax),
                AllowedValuesProperty(allowedValues, expression.SourceSyntax),
            };

            if (nullable)
            {
                properties.Add(ExpressionFactory.CreateObjectProperty("nullable", ExpressionFactory.CreateBooleanLiteral(true), expression.SourceSyntax));
            }

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private ObjectExpression GetTypePropertiesForDiscriminatedObjectExpression(DiscriminatedObjectTypeExpression expression)
        {
            var properties = new List<ObjectPropertyExpression>
            {
                TypeProperty("object", expression.SourceSyntax),
                ExpressionFactory.CreateObjectProperty(
                    "discriminator",
                    GetTypePropertiesForDiscriminator(expression),
                    expression.SourceSyntax),
            };

            return ExpressionFactory.CreateObject(properties, expression.SourceSyntax);
        }

        private ObjectExpression GetTypePropertiesForDiscriminator(
            DiscriminatedObjectTypeExpression discriminatedObjectTypeExpr)
        {
            var objectProperties = new List<ObjectPropertyExpression>();

            var discriminatorPropertyName = discriminatedObjectTypeExpr.ExpressedDiscriminatedObjectType.DiscriminatorProperty.Name;
            objectProperties.Add(ExpressionFactory.CreateObjectProperty("propertyName", ExpressionFactory.CreateStringLiteral(discriminatorPropertyName)));
            objectProperties.Add(
                ExpressionFactory.CreateObjectProperty(
                    "mapping",
                    ExpressionFactory.CreateObject(
                        GetDiscriminatedUnionMappingEntries(discriminatedObjectTypeExpr))));

            return ExpressionFactory.CreateObject(properties: objectProperties);
        }

        private IEnumerable<ObjectPropertyExpression> GetDiscriminatedUnionMappingEntries(DiscriminatedObjectTypeExpression discriminatedObjectTypeExpr)
        {
            var discriminatorPropertyName = discriminatedObjectTypeExpr.ExpressedDiscriminatedObjectType.DiscriminatorProperty.Name;

            foreach (var memberExpression in discriminatedObjectTypeExpr.MemberExpressions)
            {
                var resolvedMemberExpression = memberExpression;
                TypeAliasReferenceExpression? typeAliasReferenceExpr = null;
                if (memberExpression is TypeAliasReferenceExpression memberTypeAliasExpr
                    && declaredTypesByName.TryGetValue(memberTypeAliasExpr.Name, out var declaredTypeExpression))
                {
                    resolvedMemberExpression = declaredTypeExpression.Value;
                    typeAliasReferenceExpr = memberTypeAliasExpr;
                }

                if (resolvedMemberExpression is ObjectTypeExpression objectUnionMemberExpr)
                {
                    var memberObjectType = objectUnionMemberExpr.ExpressedObjectType;

                    if (!memberObjectType.Properties.TryGetValue(discriminatorPropertyName, out var discriminatorTypeProperty)
                        || discriminatorTypeProperty.TypeReference.Type is not StringLiteralType discriminatorStringLiteral)
                    {
                        // This should have been caught during type checking
                        throw new ArgumentException("Invalid discriminated union type encountered during serialization.");
                    }

                    ObjectExpression objectExpression;

                    if (typeAliasReferenceExpr != null)
                    {
                        objectExpression = TypePropertiesForTypeExpression(typeAliasReferenceExpr);
                    }
                    else
                    {
                        objectExpression = GetTypePropertiesForObjectType(objectUnionMemberExpr);
                    }

                    yield return ExpressionFactory.CreateObjectProperty(discriminatorStringLiteral.RawStringValue, objectExpression);
                }
                else if (resolvedMemberExpression is DiscriminatedObjectTypeExpression nestedDiscriminatedMemberExpr)
                {
                    var nestedDiscriminatorPropertyName = nestedDiscriminatedMemberExpr.ExpressedDiscriminatedObjectType.DiscriminatorProperty.Name;

                    if (nestedDiscriminatorPropertyName != discriminatorPropertyName)
                    {
                        // This should have been caught during type checking
                        throw new ArgumentException("Invalid discriminated union type encountered during serialization.");
                    }

                    foreach (var nestedPropertyExpr in GetDiscriminatedUnionMappingEntries(nestedDiscriminatedMemberExpr))
                    {
                        yield return nestedPropertyExpr;
                    }
                }
                else
                {
                    // This should have been caught during type checking
                    throw new ArgumentException("Invalid discriminated union type encountered during serialization.");
                }
            }
        }

        private static Expression ToLiteralValue(ITypeReference literalType) => literalType.Type switch
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

        private static string GetNonLiteralTypeName(TypeSymbol? type) => type switch
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

        private void EmitProviders(ExpressionEmitter emitter, ImmutableArray<DeclaredProviderExpression> providers)
        {
            if (!providers.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("imports", () => {
                foreach (var provider in providers)
                {
                    var settings = provider.NamespaceType.Settings;

                    emitter.EmitObjectProperty(provider.Name, () =>
                    {
                        emitter.EmitProperty("provider", settings.ArmTemplateProviderName);
                        emitter.EmitProperty("version", settings.ArmTemplateProviderVersion);
                        if (provider.Config is not null)
                        {
                            emitter.EmitProperty("config", provider.Config);
                        }
                    }, provider.SourceSyntax);
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
                        if (resource.ResourceMetadata.IsExistingResource)
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
                            emitter.GetSymbolicName(resource.ResourceMetadata),
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
            var metadata = resource.ResourceMetadata;

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

                var importSymbol = Context.SemanticModel.Root.ProviderDeclarations.FirstOrDefault(i => metadata.Type.DeclaringNamespace.AliasNameEquals(i.Name));
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
                foreach (var property in ApplyDescription(resource, ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty)).Properties)
                {
                    emitter.EmitProperty(property);
                }
            }, resource.SourceSyntax);
        }

        public void EmitTestParameters(ExpressionEmitter emitter, Expression parameters)
        {
            if (parameters is not ObjectExpression paramsObject)
            {
                // 'params' is optional if the module has no required params
                return;
            }

            emitter.EmitObject(() => {
                foreach (var property in paramsObject.Properties)
                {
                    if (property.TryGetKeyText() is not {} keyName)
                    {
                        // should have been caught by earlier validation
                        throw new ArgumentException("Disallowed interpolation in test parameter");
                    }

                    emitter.EmitProperty(keyName, property.Value);
                }
            }, paramsObject.SourceSyntax);
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
                foreach (var property in ApplyDescription(module, ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty)).Properties)
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
                EmitProperties(emitter, ApplyTypeModifiers(output, TypePropertiesForTypeExpression(output.Type)));

                if (output.Value is ForLoopExpression @for)
                {
                    emitter.EmitCopyProperty(() => emitter.EmitCopyObject(name: null, @for.Expression, @for.Body));
                }
                else if (output.Type.ExpressedType is ResourceType)
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

        private void EmitAssertsIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredAssertExpression> asserts)
        {
            if (!asserts.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("asserts", () =>
            {
                foreach (var assert in asserts)
                {
                    emitter.EmitProperty(assert.Name, assert.Value);
                }
            });
        }

        private void EmitMetadata(ExpressionEmitter emitter, ImmutableArray<DeclaredMetadataExpression> metadata)
        {
            emitter.EmitObjectProperty("metadata", () => {
                if (Context.Settings.UseExperimentalTemplateLanguageVersion)
                {
                    emitter.EmitProperty("_EXPERIMENTAL_WARNING", "This template uses ARM features that are experimental and should be enabled for testing purposes only. Do not enable these settings for any production usage, or you may be unexpectedly broken at any time!");
                    emitter.EmitArrayProperty("_EXPERIMENTAL_FEATURES_ENABLED", () =>
                    {
                        foreach (var (featureName, _, _) in this.Context.SemanticModel.Features.EnabledFeatureMetadata.Where(f => f.usesExperimentalArmEngineFeature))
                        {
                            emitter.EmitExpression(ExpressionFactory.CreateStringLiteral(featureName));
                        }
                    });
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
