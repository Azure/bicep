// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Helpers;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
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
        private const string InternalTypeRefStart = "#";
        private const string TypeDefinitionsProperty = "definitions";

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
        private ImmutableDictionary<string, DeclaredTypeExpression> declaredTypesByName;

        public TemplateWriter(SemanticModel semanticModel)
        {
            ExpressionBuilder = new ExpressionBuilder(new EmitterContext(semanticModel));
            declaredTypesByName = ImmutableDictionary<string, DeclaredTypeExpression>.Empty;
        }

        public void Write(SourceAwareJsonTextWriter writer)
        {
            // Template is used for calculating template hash, template jtoken is used for writing to file.
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

            var programTypes = program.Types.Concat(Context.SemanticModel.ImportClosureInfo.ImportedTypesInClosure);
            declaredTypesByName = programTypes.ToImmutableDictionary(t => t.Name);

            jsonWriter.WriteStartObject();

            emitter.EmitProperty("$schema", GetSchema(Context.SemanticModel.TargetScope));

            if (Context.Settings.UseExperimentalTemplateLanguageVersion)
            {
                if (Context.SemanticModel.Features.LocalDeployEnabled ||
                    Context.SemanticModel.Features.ExtensibilityV2EmittingEnabled)
                {
                    emitter.EmitProperty(LanguageVersionPropertyName, "2.2-experimental");
                }
                else
                {
                    emitter.EmitProperty(LanguageVersionPropertyName, "2.1-experimental");
                }
            }
            else if (Context.Settings.EnableSymbolicNames)
            {
                emitter.EmitProperty(LanguageVersionPropertyName, "2.0");
            }

            emitter.EmitProperty("contentVersion", "1.0.0.0");

            this.EmitMetadata(emitter, program.Metadata);

            this.EmitTypeDefinitionsIfPresent(emitter, programTypes);

            this.EmitUserDefinedFunctions(emitter, program.Functions.Concat(Context.SemanticModel.ImportClosureInfo.ImportedFunctionsInClosure));

            this.EmitParametersIfPresent(emitter, program.Parameters);

            this.EmitVariablesIfPresent(emitter, program.Variables.Concat(Context.SemanticModel.ImportClosureInfo.ImportedVariablesInClosure));

            this.EmitExtensionsIfPresent(emitter, program.Extensions);

            this.EmitResources(jsonWriter, emitter, program.Extensions, program.Resources, program.Modules);

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

            emitter.EmitObjectProperty(TypeDefinitionsProperty, () =>
            {
                foreach (var type in types)
                {
                    EmitTypeDeclaration(emitter, type);
                }
            });
        }

        private void EmitUserDefinedFunctions(ExpressionEmitter emitter, IEnumerable<DeclaredFunctionExpression> functions)
        {
            if (!functions.Any())
            {
                return;
            }

            emitter.EmitArrayProperty("functions", () =>
            {
                foreach (var ns in functions.GroupBy(f => f.Namespace))
                {
                    emitter.EmitObject(() =>
                    {
                        emitter.EmitProperty("namespace", ns.Key);

                        emitter.EmitObjectProperty("members", () =>
                        {
                            foreach (var function in ns)
                            {
                                EmitUserDefinedFunction(emitter, function);
                            }
                        });
                    });
                }
            });
        }

        private void EmitParametersIfPresent(ExpressionEmitter emitter, ImmutableArray<DeclaredParameterExpression> parameters)
        {
            if (!parameters.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("parameters", () =>
            {
                foreach (var parameter in parameters)
                {
                    EmitParameter(emitter, parameter);
                }
            });
        }

        private static ObjectExpression ApplyTypeModifiers(TypeDeclaringExpression expression, ObjectExpression input)
        {
            var result = input;

            if (expression.Secure is { } secure)
            {
                result = result.Properties.Where(p => p.Key is StringLiteralExpression { Value: string name } && name == "type").Single().Value switch
                {
                    StringLiteralExpression { Value: string typeName } when typeName == LanguageConstants.TypeNameString
                        => result.MergeProperty("type", ExpressionFactory.CreateStringLiteral("securestring", secure.SourceSyntax)),
                    StringLiteralExpression { Value: string typeName } when typeName == LanguageConstants.ObjectType
                        => result.MergeProperty("type", ExpressionFactory.CreateStringLiteral("secureObject", secure.SourceSyntax)),
                    _ => result,
                };
            }

            if (expression.Sealed is { } @sealed)
            {
                result = result.MergeProperty("additionalProperties", ExpressionFactory.CreateBooleanLiteral(false, @sealed.SourceSyntax));
            }

            if (expression is DeclaredTypeExpression declaredTypeExpression && declaredTypeExpression.Exported is { } exported)
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
            })
            {
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
                emitter.EmitArrayProperty("parameters", () =>
                {
                    for (var i = 0; i < lambda.Parameters.Length; i++)
                    {
                        var parameterObject = TypePropertiesForTypeExpression(lambda.ParameterTypes[i]!);
                        parameterObject = parameterObject.MergeProperty("name", new StringLiteralExpression(null, lambda.Parameters[i]));

                        emitter.EmitObject(() =>
                        {
                            EmitProperties(emitter, parameterObject);
                        });
                    }
                });

                emitter.EmitObjectProperty("output", () =>
                {
                    var outputObject = TypePropertiesForTypeExpression(lambda.OutputType!);
                    outputObject = outputObject.MergeProperty("value", lambda.Body);

                    EmitProperties(emitter, outputObject);
                });

                var originMetadataLookupKey = function.Namespace == EmitConstants.UserDefinedFunctionsNamespace
                    ? function.Name
                    : $"{function.Namespace}.{function.Name}";

                if (function.Description is not null || function.Exported is not null || Context.SemanticModel.ImportClosureInfo.ImportedSymbolOriginMetadata.ContainsKey(originMetadataLookupKey))
                {
                    emitter.EmitObjectProperty(LanguageConstants.ParameterMetadataPropertyName, () =>
                    {
                        if (function.Description is not null)
                        {
                            emitter.EmitProperty(LanguageConstants.MetadataDescriptionPropertyName, function.Description);
                        }

                        if (function.Exported is not null)
                        {
                            emitter.EmitProperty(LanguageConstants.MetadataExportedPropertyName, ExpressionFactory.CreateBooleanLiteral(true, function.Exported.SourceSyntax));
                        }

                        if (Context.SemanticModel.ImportClosureInfo.ImportedSymbolOriginMetadata.TryGetValue(originMetadataLookupKey, out var originMetadata))
                        {
                            emitter.EmitObjectProperty(LanguageConstants.MetadataImportedFromPropertyName, () =>
                            {
                                emitter.EmitProperty(LanguageConstants.ImportMetadataSourceTemplatePropertyName, originMetadata.SourceTemplateIdentifier);

                                if (!function.Name.Equals(originMetadata.OriginalName))
                                {
                                    emitter.EmitProperty(LanguageConstants.ImportMetadataOriginalIdentifierPropertyName, originMetadata.OriginalName);
                                }
                            });
                        }
                    });
                }
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
                    if (Context.SemanticModel.ImportClosureInfo.ImportedSymbolOriginMetadata.TryGetValue(declaredType.Name, out var originMetadata))
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
            // ARM primitive types
            AmbientTypeReferenceExpression ambientTypeReference
                => ExpressionFactory.CreateObject(TypeProperty(ambientTypeReference.Name, ambientTypeReference.SourceSyntax).AsEnumerable(),
                    ambientTypeReference.SourceSyntax),
            FullyQualifiedAmbientTypeReferenceExpression fullyQualifiedAmbientTypeReference
                => TypePropertiesForQualifiedReference(fullyQualifiedAmbientTypeReference),

            // references
            TypeAliasReferenceExpression or
            SynthesizedTypeAliasReferenceExpression or
            ImportedTypeReferenceExpression or
            WildcardImportTypePropertyReferenceExpression or
            ResourceDerivedTypeExpression or
            ITypeReferenceAccessExpression => GetTypePropertiesForReferenceExpression(typeExpression),

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

        private ObjectExpression GetTypePropertiesForReferenceExpression(TypeExpression typeExpression)
            => ResolveTypeReferenceExpression(typeExpression)
                .GetTypePropertiesForResolvedReferenceExpression(typeExpression.SourceSyntax);

        private interface ITypeReferenceExpressionResolution
        {
            ImmutableArray<string> PointerSegments { get; }

            ObjectExpression GetTypePropertiesForResolvedReferenceExpression(SyntaxBase? sourceSyntax);

            ImmutableArray<string> SegmentsForProperty(string propertyName)
                => PointerSegments.AddRange("properties", propertyName);

            ImmutableArray<string> SegmentsForAdditionalProperties()
                => PointerSegments.Add("additionalProperties");

            ImmutableArray<string> SegmentsForIndex(long index)
                => PointerSegments.AddRange("prefixItems", index.ToString(CultureInfo.InvariantCulture));

            ImmutableArray<string> SegmentsForItems()
                => PointerSegments.Add("items");

            ImmutableArray<string> SegmentsForVariant(string discriminatorValue)
                => PointerSegments.AddRange("discriminator", "mapping", discriminatorValue);
        }

        private record ResourceDerivedTypeResolution(
            ResourceTypeReference RootResourceTypeReference,
            ImmutableArray<string> PointerSegments,
            TypeSymbol DerivedType,
            ResourceDerivedTypeVariant Variant) : ITypeReferenceExpressionResolution
        {
            internal ResourceDerivedTypeResolution(ResourceDerivedTypeExpression expression)
                : this(expression.RootResourceType.TypeReference, [], expression.ExpressedType, expression.Variant) { }

            public ObjectExpression GetTypePropertiesForResolvedReferenceExpression(SyntaxBase? sourceSyntax)
            {
                var typePointerProperty = ExpressionFactory.CreateObjectProperty(
                    LanguageConstants.MetadataResourceDerivedTypePointerPropertyName,
                    ExpressionFactory.CreateStringLiteral(string.Concat(
                        RootResourceTypeReference.FormatName(),
                        PointerSegments.IsEmpty ? string.Empty : '#',
                        string.Join('/', PointerSegments.Select(StringExtensions.Rfc6901Encode)))));
                var metadataValue = Variant switch
                {
                    ResourceDerivedTypeVariant.Input => ExpressionFactory.CreateObject(typePointerProperty.AsEnumerable()),
                    ResourceDerivedTypeVariant.Output => ExpressionFactory.CreateObject(new[]
                    {
                        typePointerProperty,
                        ExpressionFactory.CreateObjectProperty(
                            LanguageConstants.MetadataResourceDerivedTypeOutputFlagName,
                            ExpressionFactory.CreateBooleanLiteral(true))
                    }),
                    // The legacy representation uses a string (the type pointer), not an object
                    _ => typePointerProperty.Value,
                };

                return ExpressionFactory.CreateObject(new[]
                {
                    TypeProperty(GetNonLiteralTypeName(DerivedType), sourceSyntax),
                    ExpressionFactory.CreateObjectProperty(LanguageConstants.ParameterMetadataPropertyName,
                        ExpressionFactory.CreateObject(
                            ExpressionFactory.CreateObjectProperty(
                                LanguageConstants.MetadataResourceDerivedTypePropertyName,
                                metadataValue,
                                sourceSyntax).AsEnumerable(),
                            sourceSyntax),
                        sourceSyntax),
                });
            }
        }

        private record ResolvedInternalReference(ImmutableArray<string> PointerSegments, TypeExpression Declaration) : ITypeReferenceExpressionResolution
        {
            public ObjectExpression GetTypePropertiesForResolvedReferenceExpression(SyntaxBase? sourceSyntax)
                => ExpressionFactory.CreateObject(
                    ExpressionFactory.CreateObjectProperty("$ref",
                        ExpressionFactory.CreateStringLiteral(
                            string.Join('/', InternalTypeRefStart.AsEnumerable().Concat(PointerSegments.Select(StringExtensions.Rfc6901Encode))),
                            sourceSyntax),
                        sourceSyntax).AsEnumerable(),
                    sourceSyntax);
        }

        private ResolvedInternalReference ForNamedRoot(string rootName)
            => new([TypeDefinitionsProperty, rootName], declaredTypesByName[rootName].Value);

        private ITypeReferenceExpressionResolution ResolveTypeReferenceExpression(TypeExpression expression)
        {
            Stack<ITypeReferenceAccessExpression> typeReferencesStack = new();
            TypeExpression root = expression;

            while (root is ITypeReferenceAccessExpression accessExpression)
            {
                typeReferencesStack.Push(accessExpression);
                root = accessExpression.BaseExpression;
            }

            ITypeReferenceExpressionResolution currentResolution = root switch
            {
                TypeAliasReferenceExpression typeAliasReference => ForNamedRoot(typeAliasReference.Symbol.Name),
                SynthesizedTypeAliasReferenceExpression typeAliasReference => ForNamedRoot(typeAliasReference.Name),
                ImportedTypeReferenceExpression importedTypeReference => ForNamedRoot(
                    Context.SemanticModel.ImportClosureInfo.ImportedSymbolNames[importedTypeReference.Symbol]),
                WildcardImportTypePropertyReferenceExpression importedTypeReference => ForNamedRoot(
                    Context.SemanticModel.ImportClosureInfo.WildcardImportPropertyNames[new(importedTypeReference.ImportSymbol, importedTypeReference.PropertyName)]),
                ResourceDerivedTypeExpression resourceDerived => new ResourceDerivedTypeResolution(resourceDerived),
                _ => throw new ArgumentException($"Cannot resolve type reference access expression with a root of type '{root.GetType().Name}'."),
            };

            while (typeReferencesStack.TryPop(out var currentAccessExpression))
            {
                currentResolution = currentAccessExpression switch
                {
                    TypeReferencePropertyAccessExpression propertyAccess
                        => ResolveTypeReferenceExpressionProperty(currentResolution, propertyAccess),
                    TypeReferenceAdditionalPropertiesAccessExpression additionalPropertiesAccess
                        => ResolveTypeReferenceExpressionAdditionalProperties(currentResolution, additionalPropertiesAccess),
                    TypeReferenceIndexAccessExpression indexAccess
                        => ResolveTypeReferenceExpressionIndex(currentResolution, indexAccess),
                    TypeReferenceItemsAccessExpression itemsAccess
                        => ResolveTypeReferenceExpressionItems(currentResolution, itemsAccess),
                    _ => throw new UnreachableException(
                        $"This switch expression should be exhaustive, but received an unexpected reference access expression of type {currentAccessExpression.GetType().Name}."),
                };
            }

            return currentResolution;
        }

        private ITypeReferenceExpressionResolution ResolveTypeReferenceExpressionProperty(
            ITypeReferenceExpressionResolution containerResolution,
            TypeReferencePropertyAccessExpression propertyAccess)
        {
            var (resolution, declaration) = TraceToObjectDeclaration(containerResolution);

            if (declaration is not null)
            {
                return new ResolvedInternalReference(
                    resolution.SegmentsForProperty(propertyAccess.PropertyName),
                    declaration.PropertyExpressions
                        .Single(p => StringComparer.OrdinalIgnoreCase.Equals(p.PropertyName, propertyAccess.PropertyName))
                        .Value);
            }

            if (resolution is ResourceDerivedTypeResolution resourceDerived)
            {
                return new ResourceDerivedTypeResolution(
                    resourceDerived.RootResourceTypeReference,
                    resolution.SegmentsForProperty(propertyAccess.PropertyName),
                    propertyAccess.ExpressedType,
                    resourceDerived.Variant);
            }

            throw new ArgumentException($"Unable to handle resolution of type {resolution.GetType().Name}.");
        }

        private ITypeReferenceExpressionResolution ResolveTypeReferenceExpressionAdditionalProperties(
            ITypeReferenceExpressionResolution containerResolution,
            TypeReferenceAdditionalPropertiesAccessExpression additionalPropertiesAccess)
        {
            var (resolution, declaration) = TraceToObjectDeclaration(containerResolution);

            if (declaration is not null)
            {
                if (declaration.AdditionalPropertiesExpression is null)
                {
                    throw new ArgumentException("This should have been caught by the type checker.");
                }

                return new ResolvedInternalReference(
                    resolution.SegmentsForAdditionalProperties(),
                    declaration.AdditionalPropertiesExpression.Value);
            }

            if (resolution is ResourceDerivedTypeResolution resourceDerived)
            {
                return new ResourceDerivedTypeResolution(
                    resourceDerived.RootResourceTypeReference,
                    resolution.SegmentsForAdditionalProperties(),
                    additionalPropertiesAccess.ExpressedType,
                    resourceDerived.Variant);
            }

            throw new ArgumentException($"Unable to handle resolution of type {resolution.GetType().Name}.");
        }

        private (ITypeReferenceExpressionResolution resolution, ObjectTypeExpression? declaration) TraceToObjectDeclaration(ITypeReferenceExpressionResolution original)
        {
            var currentResolution = original;
            while (currentResolution is ResolvedInternalReference @ref)
            {
                if (@ref.Declaration is ObjectTypeExpression @object)
                {
                    return (currentResolution, @object);
                }

                currentResolution = ResolveTypeReferenceExpression(@ref.Declaration);
            }

            return (currentResolution, null);
        }

        private ITypeReferenceExpressionResolution ResolveTypeReferenceExpressionIndex(
            ITypeReferenceExpressionResolution containerResolution,
            TypeReferenceIndexAccessExpression indexAccess)
        {
            var currentResolution = containerResolution;
            while (currentResolution is ResolvedInternalReference @ref)
            {
                if (@ref.Declaration is TupleTypeExpression tuple)
                {
                    return new ResolvedInternalReference(
                        currentResolution.SegmentsForIndex(indexAccess.Index),
                        tuple.ItemExpressions[(int)indexAccess.Index].Value);
                }

                currentResolution = ResolveTypeReferenceExpression(@ref.Declaration);
            }

            if (currentResolution is ResourceDerivedTypeResolution resourceDerived)
            {
                return new ResourceDerivedTypeResolution(
                    resourceDerived.RootResourceTypeReference,
                    currentResolution.SegmentsForIndex(indexAccess.Index),
                    indexAccess.ExpressedType,
                    resourceDerived.Variant);
            }

            throw new ArgumentException($"Unable to handle resolution of type {currentResolution.GetType().Name}.");
        }

        private ITypeReferenceExpressionResolution ResolveTypeReferenceExpressionItems(
            ITypeReferenceExpressionResolution containerResolution,
            TypeReferenceItemsAccessExpression itemsAccess)
        {
            var currentResolution = containerResolution;
            while (currentResolution is ResolvedInternalReference @ref)
            {
                if (@ref.Declaration is ArrayTypeExpression @array)
                {
                    return new ResolvedInternalReference(
                        currentResolution.SegmentsForItems(),
                        @array.BaseExpression);
                }

                currentResolution = ResolveTypeReferenceExpression(@ref.Declaration);
            }

            if (currentResolution is ResourceDerivedTypeResolution resourceDerived)
            {
                return new ResourceDerivedTypeResolution(
                    resourceDerived.RootResourceTypeReference,
                    currentResolution.SegmentsForItems(),
                    itemsAccess.ExpressedType,
                    resourceDerived.Variant);
            }

            throw new ArgumentException($"Unable to handle resolution of type {currentResolution.GetType().Name}.");
        }

        private static ObjectExpression TypePropertiesForQualifiedReference(FullyQualifiedAmbientTypeReferenceExpression qualifiedAmbientType)
        {
            if (qualifiedAmbientType.ProviderName != SystemNamespaceType.BuiltInName)
            {
                throw new ArgumentException("Property access base expression did not resolve to the 'sys' namespace.");
            }

            return ExpressionFactory.CreateObject(TypeProperty(qualifiedAmbientType.Name, qualifiedAmbientType.SourceSyntax).AsEnumerable(),
                qualifiedAmbientType.SourceSyntax);
        }

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

            if (TryGetAllowedValues(expression.BaseExpression) is { } allowedValues)
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

            properties.Add(ExpressionFactory.CreateObjectProperty("properties", ExpressionFactory.CreateObject(propertySchemata, expression.SourceSyntax)));

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
                var mappingsEnumerable = memberExpression switch
                {
                    TypeAliasReferenceExpression or
                    SynthesizedTypeAliasReferenceExpression or
                    ImportedTypeReferenceExpression or
                    WildcardImportTypePropertyReferenceExpression or
                    ITypeReferenceAccessExpression => ResolveTypeReferenceExpression(memberExpression) switch
                    {
                        ResolvedInternalReference udtRef => GetDiscriminatedUnionMappingEntries(discriminatorPropertyName, udtRef.Declaration, udtRef),
                        ResourceDerivedTypeResolution rdtRef => GetDiscriminatedUnionMappingEntries(discriminatorPropertyName, rdtRef),
                        _ => throw new UnreachableException(),
                    },
                    _ => GetDiscriminatedUnionMappingEntries(discriminatorPropertyName, memberExpression, null),
                };

                foreach (var mapping in mappingsEnumerable)
                {
                    yield return mapping;
                }
            }
        }

        private IEnumerable<ObjectPropertyExpression> GetDiscriminatedUnionMappingEntries(
            string discriminatorPropertyName,
            TypeExpression memberDeclaration,
            ITypeReferenceExpressionResolution? memberResolution)
        {
            if (memberDeclaration is ObjectTypeExpression objectUnionMemberExpr)
            {
                yield return ExpressionFactory.CreateObjectProperty(
                    DiscriminatorValue(discriminatorPropertyName, objectUnionMemberExpr.ExpressedObjectType),
                    memberResolution?.GetTypePropertiesForResolvedReferenceExpression(sourceSyntax: null)
                        ?? GetTypePropertiesForObjectType(objectUnionMemberExpr));
            }
            else if (memberDeclaration is DiscriminatedObjectTypeExpression nestedDiscriminatedMemberExpr)
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

        private static IEnumerable<ObjectPropertyExpression> GetDiscriminatedUnionMappingEntries(
            string discriminatorPropertyName,
            ResourceDerivedTypeResolution resolution)
        {
            if (resolution.DerivedType is ObjectType @object)
            {
                yield return ExpressionFactory.CreateObjectProperty(
                    DiscriminatorValue(discriminatorPropertyName, @object),
                    resolution.GetTypePropertiesForResolvedReferenceExpression(sourceSyntax: null));
            }
            else if (resolution.DerivedType is DiscriminatedObjectType discriminatedObject)
            {
                foreach (var variant in discriminatedObject.UnionMembersByKey.Values)
                {
                    var discriminatorValue = DiscriminatorValue(discriminatorPropertyName, variant);
                    var variantResolution = new ResourceDerivedTypeResolution(
                        resolution.RootResourceTypeReference,
                        (resolution as ITypeReferenceExpressionResolution).SegmentsForVariant(discriminatorValue),
                        variant,
                        resolution.Variant);

                    yield return ExpressionFactory.CreateObjectProperty(
                        discriminatorValue,
                        variantResolution.GetTypePropertiesForResolvedReferenceExpression(sourceSyntax: null));
                }
            }
            else
            {
                // This should have been caught during type checking
                throw new ArgumentException("Invalid discriminated union type encountered during serialization.");
            }
        }

        private static string DiscriminatorValue(string discriminatorPropertyName, ObjectType @object)
        {
            if (!@object.Properties.TryGetValue(discriminatorPropertyName, out var discriminatorTypeProperty)
                || discriminatorTypeProperty.TypeReference.Type is not StringLiteralType discriminatorStringLiteral)
            {
                // This should have been caught during type checking
                throw new ArgumentException("Invalid discriminated union type encountered during serialization.");
            }

            return discriminatorStringLiteral.RawStringValue;
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
            ObjectType or DiscriminatedObjectType => "object",
            ArrayType => "array",
            UnionType @union => @union.Members.Select(m => GetNonLiteralTypeName(m.Type)).Distinct().Single(),
            // This would have been caught by the DeclaredTypeManager during initial type assignment
            _ => throw new ArgumentException($"Cannot resolve nonliteral type name of type {type?.GetType().Name}"),
        };

        private void EmitVariablesIfPresent(ExpressionEmitter emitter, IEnumerable<DeclaredVariableExpression> variables)
        {
            if (!variables.Any())
            {
                return;
            }

            emitter.EmitObjectProperty("variables", () =>
            {
                var loopVariables = variables.Where(x => x is { Value: ForLoopExpression });
                var nonLoopVariables = variables.Where(x => x is { Value: not ForLoopExpression });

                if (loopVariables.Any())
                {
                    // we have variables whose values are loops
                    emitter.EmitArrayProperty("copy", () =>
                    {
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

        private void EmitExtensionsIfPresent(ExpressionEmitter emitter, ImmutableArray<ExtensionExpression> extensions)
        {
            if (!extensions.Any())
            {
                return;
            }

            // TODO: Remove the EmitExtensions if conditions once ARM w37 is deployed to all regions.
            if (Context.SemanticModel.Features.LocalDeployEnabled)
            {
                EmitExtensions(emitter, extensions.Add(GetExtensionForLocalDeploy()));
            }
            else if (Context.SemanticModel.Features.ExtensibilityV2EmittingEnabled)
            {
                EmitExtensions(emitter, extensions);
            }
            else
            {
                EmitProviders(emitter, extensions);
            }
        }

        private static void EmitProviders(ExpressionEmitter emitter, ImmutableArray<ExtensionExpression> extensions)
        {
            emitter.EmitObjectProperty("imports", () =>
            {
                foreach (var extension in extensions)
                {
                    var settings = extension.Settings;

                    emitter.EmitObjectProperty(extension.Name, () =>
                    {
                        emitter.EmitProperty("provider", settings.TemplateExtensionName);
                        emitter.EmitProperty("version", settings.TemplateExtensionVersion);
                        if (extension.Config is not null)
                        {
                            emitter.EmitProperty("config", extension.Config);
                        }
                    }, extension.SourceSyntax);
                }
            });
        }

        private static void EmitExtensions(ExpressionEmitter emitter, ImmutableArray<ExtensionExpression> extensions)
        {
            emitter.EmitObjectProperty("extensions", () =>
            {
                foreach (var extension in extensions)
                {
                    var settings = extension.Settings;

                    emitter.EmitObjectProperty(extension.Name, () =>
                    {
                        emitter.EmitProperty("name", settings.TemplateExtensionName);
                        emitter.EmitProperty("version", settings.TemplateExtensionVersion);

                        EmitExtensionConfig(extension, emitter);
                    },
                    extension.SourceSyntax);
                }
            });
        }

        private static void EmitExtensionConfig(ExtensionExpression extension, ExpressionEmitter emitter)
        {
            if (extension.Config is null)
            {
                return;
            }

            if (extension.Config is not ObjectExpression config)
            {
                throw new UnreachableException($"Extension config type expected to be of type: '{nameof(ObjectExpression)}' but received: '{extension.Config.GetType()}'");
            }

            emitter.EmitObjectProperty("config", () =>
            {
                foreach (var configProperty in config.Properties)
                {
                    // Type checking should have validated that the config name is not an expression (e.g. string interpolation), if we get a null value it means something
                    // was wrong with type checking validation.
                    var extensionConfigName = configProperty.TryGetKeyText() ?? throw new UnreachableException("Expressions are not allowed as config names.");
                    var configType = extension.Settings.ConfigurationType ?? throw new UnreachableException("Config type must be specified.");
                    var extensionConfigType = GetExtensionConfigType(extensionConfigName, configType);

                    emitter.EmitObjectProperty(extensionConfigName, () =>
                    {
                        switch (extensionConfigType)
                        {
                            case StringType:
                                if (extensionConfigType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                                {
                                    emitter.EmitProperty("type", "secureString");
                                }
                                else
                                {
                                    emitter.EmitProperty("type", "string");
                                }
                                break;
                            case IntegerType:
                                emitter.EmitProperty("type", "int");
                                break;
                            case BooleanType:
                                emitter.EmitProperty("type", "bool");
                                break;
                            case ArrayType:
                                emitter.EmitProperty("type", "array");
                                break;
                            case ObjectType:
                                if (extensionConfigType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                                {
                                    emitter.EmitProperty("type", "secureObject");
                                }
                                else
                                {
                                    emitter.EmitProperty("type", "object");
                                }
                                break;
                            default:
                                throw new ArgumentException($"Config name: '{extensionConfigName}' specified an unsupported type: '{extensionConfigType}'. Supported types are: 'string', 'secureString', 'int', 'bool', 'array', 'secureObject', 'object'.");
                        }

                        emitter.EmitProperty("defaultValue", configProperty.Value);
                    });
                }
            });
        }

        private static TypeSymbol GetExtensionConfigType(string configName, ObjectType configType)
        {
            if (configType.Properties.TryGetValue(configName) is { } configItem)
            {
                return configItem.TypeReference.Type;
            }

            throw new UnreachableException($"Configuration name: '{configName}' does not exist as part of extension configuration.");
        }

        private ExtensionExpression GetExtensionForLocalDeploy()
        {
            return new(
                null,
                "az0synthesized",
                new(false, "__az", null, "LocalNested", "0.0.0"),
                null,
                null);
        }

        private void EmitResources(
            PositionTrackingJsonTextWriter jsonWriter,
            ExpressionEmitter emitter,
            ImmutableArray<ExtensionExpression> extensions,
            ImmutableArray<DeclaredResourceExpression> resources,
            ImmutableArray<DeclaredModuleExpression> modules)
        {
            if (!Context.Settings.EnableSymbolicNames)
            {
                emitter.EmitArrayProperty("resources", () =>
                {
                    foreach (var resource in resources)
                    {
                        if (resource.ResourceMetadata.IsExistingResource)
                        {
                            continue;
                        }

                        this.EmitResource(emitter, extensions, resource);
                    }

                    foreach (var module in modules)
                    {
                        this.EmitModule(jsonWriter, module, emitter);
                    }
                });
            }
            else
            {
                emitter.EmitObjectProperty("resources", () =>
                {
                    foreach (var resource in resources)
                    {
                        emitter.EmitProperty(
                            emitter.GetSymbolicName(resource.ResourceMetadata),
                            () => EmitResource(emitter, extensions, resource),
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

        private void EmitResource(ExpressionEmitter emitter, ImmutableArray<ExtensionExpression> extensions, DeclaredResourceExpression resource)
        {
            var metadata = resource.ResourceMetadata;

            emitter.EmitObject(() =>
            {
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

                var extensionSymbol = extensions.FirstOrDefault(i => metadata.Type.DeclaringNamespace.AliasNameEquals(i.Name));
                if (extensionSymbol is not null)
                {
                    if (this.Context.SemanticModel.Features.LocalDeployEnabled ||
                        this.Context.SemanticModel.Features.ExtensibilityV2EmittingEnabled)
                    {
                        emitter.EmitProperty("extension", extensionSymbol.Name);
                    }
                    else
                    {
                        emitter.EmitProperty("import", extensionSymbol.Name);
                    }
                }

                // Emit the options property 
                if (resource.RetryOn is not null || resource.WaitUntil is not null)
                {
                    emitter.EmitObjectProperty("options", () =>
                    {
                        if (resource.RetryOn is not null)
                        {
                            emitter.EmitObjectProperty("retryOn", () =>
                            {
                                emitter.EmitObjectProperties(resource.RetryOn);
                            });
                        }

                        if (resource.WaitUntil is not null)
                        {
                            emitter.EmitObjectProperty("waitUntil", () =>
                            {
                                emitter.EmitObjectProperties(resource.WaitUntil);
                            });
                        }

                    });

                }

                if (metadata.IsAzResource ||
                    this.Context.SemanticModel.Features.LocalDeployEnabled ||
                    this.Context.SemanticModel.Features.ExtensibilityV2EmittingEnabled)
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
                    emitter.EmitObjectProperty("properties", () =>
                    {
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

            emitter.EmitObject(() =>
            {
                foreach (var property in paramsObject.Properties)
                {
                    if (property.TryGetKeyText() is not { } keyName)
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

            emitter.EmitObjectProperty("parameters", () =>
            {
                foreach (var property in paramsObject.Properties)
                {
                    if (property.TryGetKeyText() is not { } keyName)
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
                        emitter.EmitObjectProperty(keyName, () =>
                        {
                            emitter.EmitCopyProperty(() =>
                            {
                                emitter.EmitArray(() =>
                                {
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

        private void EmitModuleForLocalDeploy(PositionTrackingJsonTextWriter jsonWriter, DeclaredModuleExpression module, ExpressionEmitter emitter)
        {
            emitter.EmitObject(() =>
            {
                emitter.EmitProperty("extension", "az0synthesized");

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

                emitter.EmitObjectProperty("properties", () =>
                {
                    EmitModuleParameters(emitter, module);

                    var moduleSemanticModel = GetModuleSemanticModel(module.Symbol);

                    var moduleWriter = TemplateWriterFactory.CreateTemplateWriter(moduleSemanticModel);
                    var moduleBicepFile = (moduleSemanticModel as SemanticModel)?.SourceFile;
                    var moduleTextWriter = new StringWriter();
                    var moduleJsonWriter = new SourceAwareJsonTextWriter(moduleTextWriter, moduleBicepFile);

                    moduleWriter.Write(moduleJsonWriter);
                    jsonWriter.AddNestedSourceMap(moduleJsonWriter.TrackingJsonWriter);
                    emitter.EmitProperty("template", moduleTextWriter.ToString());
                });

                this.EmitDependsOn(emitter, module.DependsOn);

                // Since we don't want to be mutating the body of the original ObjectSyntax, we create a placeholder body in place
                // and emit its properties to merge decorator properties.
                foreach (var property in ApplyDescription(module, ExpressionFactory.CreateObject(ImmutableArray<ObjectPropertyExpression>.Empty)).Properties)
                {
                    emitter.EmitProperty(property);
                }
            }, module.SourceSyntax);
        }

        private void EmitModule(PositionTrackingJsonTextWriter jsonWriter, DeclaredModuleExpression module, ExpressionEmitter emitter)
        {
            if (this.Context.SemanticModel.Features.LocalDeployEnabled)
            {
                EmitModuleForLocalDeploy(jsonWriter, module, emitter);
                return;
            }

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
                emitter.EmitProperty("apiVersion", EmitConstants.NestedDeploymentResourceApiVersion);

                // emit all properties apart from 'params'. In practice, this currently only allows 'name', but we may choose to allow other top-level resource properties in future.
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
                            new FunctionCallExpression(null, "resourceGroup", []),
                            "location",
                            AccessExpressionFlags.None));
                    }
                    else
                    {
                        // at all other scopes we can just use deployment().location
                        emitter.EmitProperty("location", new PropertyAccessExpression(
                            null,
                            new FunctionCallExpression(null, "deployment", []),
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
                        var moduleJsonWriter = new SourceAwareJsonTextWriter(moduleTextWriter, moduleBicepFile);

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

        private static void EmitSymbolicNameDependsOnEntry(ExpressionEmitter emitter, ResourceDependencyExpression dependency)
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

        private static void EmitClassicDependsOnEntry(ExpressionEmitter emitter, ResourceDependencyExpression dependency)
        {
            switch (dependency.Reference)
            {
                case ResourceReferenceExpression { Metadata: DeclaredResourceMetadata resource } reference:
                    if (resource.Symbol.IsCollection && reference.IndexContext?.Index is null)
                    {
                        // dependency is on the entire resource collection
                        // write the fully qualified name of the resource collection (this is the name of the copy loop) as the dependency
                        emitter.EmitSymbolReference(resource);

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

            emitter.EmitArrayProperty("dependsOn", () =>
            {
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

            emitter.EmitObjectProperty("outputs", () =>
            {
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
            emitter.EmitObjectProperty("metadata", () =>
            {
                if (Context.Settings.UseExperimentalTemplateLanguageVersion)
                {
                    emitter.EmitProperty("_EXPERIMENTAL_WARNING", "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.");
                    emitter.EmitArrayProperty("_EXPERIMENTAL_FEATURES_ENABLED", () =>
                    {
                        foreach (var (featureName, _, _) in this.Context.SemanticModel.Features.EnabledFeatureMetadata.Where(f => f.usesExperimentalArmEngineFeature))
                        {
                            emitter.EmitExpression(ExpressionFactory.CreateStringLiteral(featureName));
                        }
                    });
                }

                emitter.EmitObjectProperty("_generator", () =>
                {
                    emitter.EmitProperty("name", LanguageConstants.LanguageId);
                    emitter.EmitProperty("version", this.Context.SemanticModel.Features.AssemblyVersion);
                });

                var exportedVariables = Context.SemanticModel.Exports.Values.OfType<ExportedVariableMetadata>().ToImmutableArray();

                if (exportedVariables.Length > 0)
                {
                    emitter.EmitArrayProperty(LanguageConstants.TemplateMetadataExportedVariablesName, () =>
                    {
                        foreach (var exportedVariable in exportedVariables)
                        {
                            emitter.EmitObject(() =>
                            {
                                emitter.EmitProperty("name", exportedVariable.Name);
                                if (exportedVariable.Description is string description)
                                {
                                    emitter.EmitProperty(LanguageConstants.MetadataDescriptionPropertyName, description);
                                }
                            });
                        }
                    });
                }

                foreach (var item in metadata)
                {
                    emitter.EmitProperty(item.Name, item.Value);
                }
            });
        }
    }
}
