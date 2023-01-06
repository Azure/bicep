// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Extensions;
using Azure.Deployments.Templates.Extensions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public class ArmTemplateSemanticModel : ISemanticModel
    {
        private readonly Lazy<ResourceScope> targetScopeLazy;

        private readonly Lazy<ImmutableDictionary<string, ParameterMetadata>> parametersLazy;

        private readonly Lazy<ImmutableArray<OutputMetadata>> outputsLazy;

        private readonly ConcurrentDictionary<string, TypeSymbol> templateTypeDefinitions = new();

        public ArmTemplateSemanticModel(ArmTemplateFile sourceFile)
        {
            Trace.WriteLine($"Building semantic model for {sourceFile.FileUri}");

            this.SourceFile = sourceFile;

            this.targetScopeLazy = new(() =>
            {
                if (sourceFile.Template?.Schema is not { Value: var schema })
                {
                    return ResourceScope.None;
                }

                if (!Uri.TryCreate(schema, UriKind.Absolute, out var schemaUri))
                {
                    return ResourceScope.None;
                }

                return (schemaUri.AbsolutePath) switch
                {
                    "/schemas/2019-08-01/tenantDeploymentTemplate.json" => ResourceScope.Tenant,
                    "/schemas/2019-08-01/managementGroupDeploymentTemplate.json" => ResourceScope.ManagementGroup,
                    "/schemas/2018-05-01/subscriptionDeploymentTemplate.json" => ResourceScope.Subscription,
                    "/schemas/2014-04-01-preview/deploymentTemplate.json" or
                    "/schemas/2015-01-01/deploymentTemplate.json" or
                    "/schemas/2019-04-01/deploymentTemplate.json" => ResourceScope.ResourceGroup,
                    _ => ResourceScope.None,
                };
            });

            this.parametersLazy = new(() =>
            {
                if (this.SourceFile.Template?.Parameters is null)
                {
                    return ImmutableDictionary<string, ParameterMetadata>.Empty;
                }

                // the source here is of type InsensitiveDictionary<TemplateInputParameter>,
                // which is basically a Dictionary<string, TemplateInputParameter> that uses an
                // InvariantCultureIgnoreCase key comparer
                // there should be no possibility of clashes since the target dictionary is using
                // Ordinal key comparer, which looks purely at the key string code points
                // without applying any additional rules
                return this.SourceFile.Template.Parameters
                    .ToImmutableDictionary(
                        parameterProperty => parameterProperty.Key,
                        parameterProperty => new ParameterMetadata(
                            parameterProperty.Key,
                            GetType(parameterProperty.Value),
                            parameterProperty.Value.DefaultValue is null,
                            GetMostSpecificDescription(parameterProperty.Value)),
                        LanguageConstants.IdentifierComparer);
            });

            this.outputsLazy = new(() =>
            {
                if (this.SourceFile.Template?.Outputs is null)
                {
                    return ImmutableArray<OutputMetadata>.Empty;
                }

                return this.SourceFile.Template.Outputs
                    .Select(outputProperty => new OutputMetadata(
                        outputProperty.Key,
                        GetType(outputProperty.Value),
                        TryGetMetadataDescription(outputProperty.Value.Metadata)))
                    .ToImmutableArray();
            });
        }

        public ArmTemplateFile SourceFile { get; }

        public ResourceScope TargetScope => this.targetScopeLazy.Value == ResourceScope.None
            ? ResourceScope.ResourceGroup
            : this.targetScopeLazy.Value;

        public ImmutableDictionary<string, ParameterMetadata> Parameters => this.parametersLazy.Value;

        public ImmutableArray<OutputMetadata> Outputs => this.outputsLazy.Value;

        public bool HasErrors()
        {
            if (this.SourceFile.HasErrors())
            {
                return true;
            }

            if (this.targetScopeLazy.Value == ResourceScope.None)
            {
                return true;
            }

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var visitor = new SemanticDiagnosticVisitor(diagnosticWriter);

            foreach (var parameter in this.Parameters.Values)
            {
                visitor.Visit(parameter.TypeReference.Type);
            }

            foreach (var output in this.Outputs)
            {
                visitor.Visit(output.TypeReference.Type);
            }

            return diagnosticWriter.GetDiagnostics().Count > 0;
        }

        private TypeSymbol GetType(TemplateInputParameter parameter) => parameter.Type?.Value switch
        {
            TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(GetMetadata(parameter), out var resourceType) =>
                resourceType,

            _ => GetType((ITemplateSchemaNode) parameter),
        };

        private TypeSymbol GetType(ITemplateSchemaNode schemaNode)
        {
            if (schemaNode.Ref?.Value is string @ref)
            {

                return templateTypeDefinitions.GetOrAdd(@ref, ResolveTypeReference);
            }

            return schemaNode.Type.Value switch
            {
                TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(GetMetadata(schemaNode), out var resourceType) => resourceType,
                TemplateParameterType.String => GetPrimitiveType(schemaNode, t => t.IsTextBasedJTokenType(), LanguageConstants.TypeNameString, LanguageConstants.LooseString),
                TemplateParameterType.Int => GetPrimitiveType(schemaNode, t => t.Type == JTokenType.Integer, LanguageConstants.TypeNameInt, LanguageConstants.LooseInt),
                TemplateParameterType.Bool => GetPrimitiveType(schemaNode, t => t.Type == JTokenType.Boolean, LanguageConstants.TypeNameBool, LanguageConstants.LooseBool),
                TemplateParameterType.Array => GetArrayType(schemaNode),
                TemplateParameterType.Object => GetObjectType(schemaNode),
                TemplateParameterType.SecureString => LanguageConstants.SecureString,
                TemplateParameterType.SecureObject => GetObjectType(schemaNode, TypeSymbolValidationFlags.IsSecure),
                _ => ErrorType.Empty(),
            };
        }

        private TypeSymbol ResolveTypeReference(string reference)
        {
            if (!FollowRefsToConcreteTypeDefinition(reference, out var concreteType, out var errorBuilder))
            {
                return ErrorType.Create(errorBuilder(DiagnosticBuilder.ForDocumentStart()));
            }

            return GetType(concreteType);
        }

        private bool FollowRefsToConcreteTypeDefinition(string reference, [NotNullWhen(true)] out ITemplateSchemaNode? concreteType, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? errorBuilder)
            => FollowRefsUntil(new TemplateTypeDefinition { Ref = reference.ToTemplateGenericProperty() }, node => node.Type != null, out concreteType, out errorBuilder);

        /// <summary>
        /// Metadata may be attached to $ref nodes, and the appropriate description for a given parameter or property will be the first one (if any) encountered while following $ref pointers to a concrete type.
        /// </summary>
        /// <param name="schemaNode">The starting point for the search</param>
        /// <returns></returns>
        private string? GetMostSpecificDescription(ITemplateSchemaNode schemaNode) => FollowRefsUntil(schemaNode,
            n => (GetMetadata(n) as JObject)?.ContainsKey(LanguageConstants.MetadataDescriptionPropertyName) == true,
            out var nodeWithDescriptionOrEndOfRefTrail,
            out _)
            ? (GetMetadata(nodeWithDescriptionOrEndOfRefTrail) as JObject)?[LanguageConstants.MetadataDescriptionPropertyName]?.ToString() : null;

        private bool FollowRefsUntil(
            ITemplateSchemaNode startingPoint,
            Func<ITemplateSchemaNode, bool> shouldStopTraversing,
            [NotNullWhen(true)] out ITemplateSchemaNode? cursorOnStop,
            [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? errorBuilder)
        {
            ITemplateSchemaNode current = startingPoint;
            LinkedList<string> visited = new();

            while (!shouldStopTraversing(current) && current.Ref?.Value is string @ref)
            {
                if (visited.Contains(@ref))
                {
                    errorBuilder = b => b.CyclicArmTypeRefs(visited.Append(@ref));
                    cursorOnStop = null;
                    return false;
                }
                visited.AddLast(@ref);


                if (SourceFile.Template?.Definitions?.TryGetValue(@ref.Replace("#/definitions/", string.Empty), out var dereferenced) == true)
                {
                    current = dereferenced;
                    continue;
                }

                errorBuilder = b => b.ArmTypeRefTargetNotFound(@ref);
                cursorOnStop = null;
                return false;
            }

            errorBuilder = null;
            cursorOnStop = current;
            return true;
        }

        private static JToken? GetMetadata(ITemplateSchemaNode schemaNode) => schemaNode switch
        {
            TemplateInputParameter param => param.Metadata?.Value,
            TemplateTypeDefinition type => type.Metadata?.Value,
            _ => null,
        };

        private static TypeSymbol GetPrimitiveType(ITemplateSchemaNode schemaNode, Func<JToken, bool> isValidLiteralPredicate, string typeName, TypeSymbol type)
        {
            if (schemaNode.AllowedValues?.Value is JArray jArray)
            {
                return TryGetLiteralUnionType(jArray, isValidLiteralPredicate, b => b.InvalidUnionTypeMember(typeName));
            }

            return type;
        }

        private static TypeSymbol TryGetLiteralUnionType(JArray allowedValues, Func<JToken, bool> validator, DiagnosticBuilder.ErrorBuilderDelegate diagnosticOnMismatch)
        {
            List<TypeSymbol> literalTypeTargets = new();
            foreach (var element in allowedValues)
            {
                if (!validator(element) || TypeHelper.TryCreateTypeLiteral(element) is not { } literal)
                {
                    return ErrorType.Create(diagnosticOnMismatch(DiagnosticBuilder.ForDocumentStart()));
                }

                literalTypeTargets.Add(literal);
            }

            return TypeHelper.CreateTypeUnion(literalTypeTargets);
        }

        private TypeSymbol GetArrayType(ITemplateSchemaNode schemaNode)
        {
            if (schemaNode.AllowedValues?.Value is JArray allowedValues)
            {
                return GetArrayLiteralType(allowedValues);
            }

            if (schemaNode.PrefixItems is { } prefixItems)
            {
                TupleTypeNameBuilder nameBuilder = new();
                List<ITypeReference> tupleMembers = new();
                foreach (var prefixItem in prefixItems)
                {
                    if (prefixItem.Ref?.Value is { } @ref)
                    {
                        nameBuilder.AppendItem(@ref.Replace("#/definitions", ""));
                        tupleMembers.Add(new DeferredTypeReference(() => templateTypeDefinitions.GetOrAdd(@ref, ResolveTypeReference)));
                    }
                    else
                    {
                        var itemType = GetType(prefixItem);
                        nameBuilder.AppendItem(itemType.Name);
                        tupleMembers.Add(itemType);
                    }
                }

                return new TupleType(nameBuilder.ToString(), tupleMembers.ToImmutableArray(), default);
            }

            if (schemaNode.Items?.SchemaNode is { } items)
            {
                if (items.Ref?.Value is { } @ref)
                {
                    return new TypedArrayType($"{@ref.Replace("#/definitions", "")}[]",
                        new DeferredTypeReference(() => templateTypeDefinitions.GetOrAdd(@ref, ResolveTypeReference)),
                        default);
                }

                return new TypedArrayType(GetType(items), default);
            }

            // TODO it's possible to encounter an array with a defined prefix and either a schema or a boolean for "items."
            // TupleType does not support an "AdditionalItemsType" for items after the tuple, but when it does, update this type reader to handle the combination of "items" and "prefixItems"

            return LanguageConstants.Array;
        }

        private static TypeSymbol GetArrayLiteralType(JArray allowedValues)
        {
            // For allowedValues on an array, either all or none of the allowed values need to be arrays.
            if (allowedValues.Any(t => t.Type == JTokenType.Array))
            {
                // If any of the allowed values are arrays, it's a regular union of literals
                return TryGetLiteralUnionType(allowedValues, t => t.Type == JTokenType.Array, b => b.InvalidUnionTypeMember(LanguageConstants.ArrayType));
            }

            // If no allowed values are arrays, the each element in the array must be one of the allowed values provided
            List<TypeSymbol> elements = new();
            foreach (var element in allowedValues)
            {
                // Arrays with constrained but mixed-type literal elements are the only place where `null` is a valid type literal
                if (element.Type == JTokenType.Null)
                {
                    elements.Add(LanguageConstants.Null);
                }
                else if (element.Type == JTokenType.Comment)
                {
                    continue;
                }
                else if (TypeHelper.TryCreateTypeLiteral(element) is { } literal)
                {
                    elements.Add(literal);
                }
                else
                {
                    // TryCreateTypeLiteral is exhaustive, so this should never be reached
                    return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().TypeExpressionLiteralConversionFailed());
                }
            }

            return new TypedArrayType(TypeHelper.CreateTypeUnion(elements), default);
        }

        private TypeSymbol GetObjectType(ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags symbolValidationFlags = TypeSymbolValidationFlags.Default)
        {
            if (schemaNode.AllowedValues?.Value is JArray jArray)
            {
                return TryGetLiteralUnionType(jArray, t => t.Type == JTokenType.Object, b => b.InvalidUnionTypeMember(LanguageConstants.ObjectType));
            }

            ObjectTypeNameBuilder nameBuilder = new();
            List<TypeProperty> properties = new();
            ITypeReference? additionalPropertiesType = LanguageConstants.Any;
            TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.FallbackProperty;

            if (schemaNode.Properties is { } propertySchemata)
            {
                foreach (var (propertyName, schema) in propertySchemata)
                {
                    var flags = TypePropertyFlags.Required;
                    var description = GetMostSpecificDescription(schema);

                    if (schema.Ref?.Value is { } @ref)
                    {
                        var type = new DeferredTypeReference(() => templateTypeDefinitions.GetOrAdd(@ref, ResolveTypeReference));
                        properties.Add(new(propertyName, type, flags, description));
                        nameBuilder.AppendProperty(propertyName, @ref.Replace("#/definitions", ""));
                    }
                    else
                    {
                        var type = GetType(schema);
                        properties.Add(new(propertyName, type, flags, description));
                        nameBuilder.AppendProperty(propertyName, type.Name);
                    }
                }
            }

            if (schemaNode.AdditionalProperties is { } addlProps)
            {
                additionalPropertiesFlags = TypePropertyFlags.None;

                if (addlProps.SchemaNode is { } additionalPropertiesSchema)
                {
                    additionalPropertiesType = additionalPropertiesSchema.Ref?.Value is { } @ref
                        ? new DeferredTypeReference(() => templateTypeDefinitions.GetOrAdd(@ref, ResolveTypeReference))
                        : GetType(additionalPropertiesSchema);
                }
                else if (addlProps.BooleanValue == false)
                {
                    additionalPropertiesType = null;
                }
            }

            if (properties.Count == 0 && additionalPropertiesType == LanguageConstants.Any && additionalPropertiesFlags == TypePropertyFlags.FallbackProperty)
            {
                return symbolValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) ? LanguageConstants.SecureObject : LanguageConstants.Object;
            }

            return new ObjectType(nameBuilder.ToString(), symbolValidationFlags, properties, additionalPropertiesType, additionalPropertiesFlags);
        }

        private static TypeSymbol GetType(TemplateOutputParameter output)
        {
            return output.Type.Value switch
            {
                TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(output.Metadata?.Value, out var resourceType) =>
                    resourceType,

                _ => GetType((TemplateParameter)output),
            };
        }

        private static TypeSymbol GetType(TemplateParameter parameterOrOutput) => parameterOrOutput.Type.Value switch
        {
            TemplateParameterType.String => LanguageConstants.LooseString,
            TemplateParameterType.Int => LanguageConstants.Int,
            TemplateParameterType.Bool => LanguageConstants.Bool,
            TemplateParameterType.Array => LanguageConstants.Array,
            TemplateParameterType.Object => LanguageConstants.Object,
            TemplateParameterType.SecureString => LanguageConstants.SecureString,
            TemplateParameterType.SecureObject => LanguageConstants.SecureObject,
            _ => ErrorType.Empty(),
        };

        private static bool TryCreateUnboundResourceTypeParameter(JToken? metadataToken, [NotNullWhen(true)] out TypeSymbol? type)
        {
            if (metadataToken is JObject metadata &&
                metadata.TryGetValue(LanguageConstants.MetadataResourceTypePropertyName, out var obj) &&
                obj.Value<string>() is string resourceTypeRaw)
            {
                if (ResourceTypeReference.TryParse(resourceTypeRaw) is {} parsed)
                {
                    type = new UnboundResourceType(parsed);
                    return true;
                }

                // Return true here because it *was* a resource type, just a wrong one.
                type = ErrorType.Create(DiagnosticBuilder.ForDocumentStart().InvalidResourceType());
                return true;
            }

            type = null;
            return false;
        }

        private static string? TryGetMetadataDescription(TemplateGenericProperty<JToken>? metadata)
        {
            if (metadata?.Value?.SelectToken(LanguageConstants.MetadataDescriptionPropertyName) is { } descriptionToken
            && descriptionToken.Type is JTokenType.String)
            {
                return descriptionToken.ToString();
            }
            return null;
        }
    }
}
