// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Expression.Extensions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Exceptions;
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

            _ => GetType((ITemplateSchemaNode)parameter, allowLooseAssignment: true),
        };

        private TypeSymbol GetType(ITemplateSchemaNode schemaNode, bool allowLooseAssignment = false)
        {
            try
            {
                var resolved = TemplateEngine.ResolveSchemaReferences(SourceFile.Template, schemaNode);

                var bicepType = resolved.Type.Value switch
                {
                    TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(resolved.Metadata?.Value, out var resourceType) => resourceType,
                    TemplateParameterType.String => GetStringType(resolved, allowLooseAssignment ? TypeSymbolValidationFlags.AllowLooseAssignment : default),
                    TemplateParameterType.Int => GetIntegerType(resolved, allowLooseAssignment),
                    TemplateParameterType.Bool => GetPrimitiveType(resolved, t => t.Type == JTokenType.Boolean, LanguageConstants.TypeNameBool, allowLooseAssignment ? LanguageConstants.LooseBool : LanguageConstants.Bool),
                    TemplateParameterType.Array => GetArrayType(resolved),
                    TemplateParameterType.Object => GetObjectType(SourceFile.Template!, resolved),
                    TemplateParameterType.SecureString => GetStringType(resolved,
                        TypeSymbolValidationFlags.IsSecure | (allowLooseAssignment ? TypeSymbolValidationFlags.AllowLooseAssignment : TypeSymbolValidationFlags.Default)),
                    TemplateParameterType.SecureObject => GetObjectType(SourceFile.Template!, resolved, TypeSymbolValidationFlags.IsSecure),
                    _ => ErrorType.Empty(),
                };

                if (resolved.Nullable?.Value == true)
                {
                    bicepType = TypeHelper.CreateTypeUnion(bicepType, LanguageConstants.Null);
                }

                return bicepType;
            }
            catch (TemplateValidationException tve)
            {
                return ErrorType.Create(DiagnosticBuilder.ForDocumentStart().UnresolvableArmJsonType(tve.TemplateErrorAdditionalInfo.Path ?? "<unknown location>", tve.Message));
            }
        }

        /// <summary>
        /// Metadata may be attached to $ref nodes, and the appropriate description for a given parameter or property will be the first one (if any) encountered while following $ref pointers to a concrete type.
        /// </summary>
        /// <param name="schemaNode">The starting point for the search</param>
        /// <returns></returns>
        private string? GetMostSpecificDescription(ITemplateSchemaNode schemaNode)
        {
            if (GetMetadata(schemaNode) is JObject metadataObject &&
                metadataObject.TryGetValue(LanguageConstants.MetadataDescriptionPropertyName, out var descriptionToken) &&
                descriptionToken is JValue { Value: string description })
            {
                return description;
            }

            return null;
        }
        private JToken? GetMetadata(ITemplateSchemaNode schemaNode)
        {
            try
            {
                return TemplateEngine.ResolveSchemaReferences(SourceFile.Template, schemaNode).Metadata?.Value;
            }
            catch (TemplateValidationException)
            {
                return null;
            }
        }

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

        private static TypeSymbol GetIntegerType(ITemplateSchemaNode schemaNode, bool allowLooseAssignment)
        {
            if (schemaNode.AllowedValues?.Value is JArray jArray)
            {
                return TryGetLiteralUnionType(jArray, t => t.Type == JTokenType.Integer, b => b.InvalidUnionTypeMember(LanguageConstants.TypeNameInt));
            }

            return TypeFactory.CreateIntegerType(schemaNode.MinValue?.Value,
                schemaNode.MaxValue?.Value,
                allowLooseAssignment ? TypeSymbolValidationFlags.AllowLooseAssignment : TypeSymbolValidationFlags.Default);
        }

        private static TypeSymbol GetStringType(ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags flags)
        {
            if (schemaNode.AllowedValues?.Value is JArray jArray)
            {
                return TryGetLiteralUnionType(jArray, t => t.IsTextBasedJTokenType(), b => b.InvalidUnionTypeMember(LanguageConstants.TypeNameString));
            }

            return TypeFactory.CreateStringType(schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value, flags);
        }

        private TypeSymbol GetArrayType(ITemplateSchemaNode schemaNode)
        {
            if (schemaNode.AllowedValues?.Value is JArray allowedValues)
            {
                return GetArrayLiteralType(allowedValues, schemaNode);
            }

            if (schemaNode.PrefixItems is { } prefixItems)
            {
                TupleTypeNameBuilder nameBuilder = new();
                List<ITypeReference> tupleMembers = new();
                foreach (var prefixItem in prefixItems)
                {
                    var (type, typeName) = GetDeferrableTypeInfo(prefixItem);
                    nameBuilder.AppendItem(typeName);
                    tupleMembers.Add(type);
                }

                return new TupleType(nameBuilder.ToString(), tupleMembers.ToImmutableArray(), default);
            }

            if (schemaNode.Items?.SchemaNode is { } items)
            {
                if (items.Ref?.Value is { } @ref)
                {
                    var (type, typeName) = GetDeferrableTypeInfo(items);
                    return new TypedArrayType($"{typeName}[]", type, default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
                }

                return new TypedArrayType(GetType(items), default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
            }

            // TODO it's possible to encounter an array with a defined prefix and either a schema or a boolean for "items."
            // TupleType does not support an "AdditionalItemsType" for items after the tuple, but when it does, update this type reader to handle the combination of "items" and "prefixItems"

            return TypeFactory.CreateArrayType(schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
        }

        private static TypeSymbol GetArrayLiteralType(JArray allowedValues, ITemplateSchemaNode schemaNode)
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

            return new TypedArrayType(TypeHelper.CreateTypeUnion(elements), default, schemaNode.MinLength?.Value, schemaNode.MaxLength?.Value);
        }

        private TypeSymbol GetObjectType(Template template, ITemplateSchemaNode schemaNode, TypeSymbolValidationFlags symbolValidationFlags = TypeSymbolValidationFlags.Default)
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
                    // depending on the language version, either only properties included in schemaNode.Required are required,
                    // or all of them are (but some may be nullable)
                    var required = template.GetLanguageVersion().HasFeature(TemplateLanguageFeature.NullableParameters)
                        ? true
                        : schemaNode.Required?.Value.Contains(propertyName) ?? false;
                    var flags = required ? TypePropertyFlags.Required : TypePropertyFlags.None;
                    var description = GetMostSpecificDescription(schema);

                    var (type, typeName) = GetDeferrableTypeInfo(schema);

                    properties.Add(new(propertyName, type, flags, description));
                    nameBuilder.AppendProperty(propertyName, typeName);
                }
            }

            if (schemaNode.AdditionalProperties is { } addlProps)
            {
                additionalPropertiesFlags = TypePropertyFlags.None;

                if (addlProps.SchemaNode is { } additionalPropertiesSchema)
                {
                    var typeInfo = GetDeferrableTypeInfo(additionalPropertiesSchema);
                    additionalPropertiesType = typeInfo.type;
                    nameBuilder.AppendPropertyMatcher("*", typeInfo.typeName);
                }
                else if (addlProps.BooleanValue == false)
                {
                    additionalPropertiesType = null;
                }
            }

            if (properties.Count == 0 && schemaNode.AdditionalProperties is null)
            {
                return symbolValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) ? LanguageConstants.SecureObject : LanguageConstants.Object;
            }

            return new ObjectType(nameBuilder.ToString(), symbolValidationFlags, properties, additionalPropertiesType, additionalPropertiesFlags);
        }

        private (ITypeReference type, string typeName) GetDeferrableTypeInfo(ITemplateSchemaNode schemaNode) => schemaNode.Ref?.Value switch
        {
            string @ref => (new DeferredTypeReference(() => GetType(schemaNode)), @ref.Replace("#/definitions/", "")),
            _ => GetType(schemaNode) switch { TypeSymbol concreteType => (concreteType, concreteType.Name) },
        };

        private TypeSymbol GetType(TemplateOutputParameter output)
        {
            return output.Type?.Value switch
            {
                TemplateParameterType.String when TryCreateUnboundResourceTypeParameter(resolved.Metadata?.Value, out var resourceType) =>
                    resourceType,

                _ => GetType((ITemplateSchemaNode)output),
            };
        }

        private static bool TryCreateUnboundResourceTypeParameter(JToken? metadataToken, [NotNullWhen(true)] out TypeSymbol? type)
        {
            if (metadataToken is JObject metadata &&
                metadata.TryGetValue(LanguageConstants.MetadataResourceTypePropertyName, out var obj) &&
                obj.Value<string>() is string resourceTypeRaw)
            {
                if (ResourceTypeReference.TryParse(resourceTypeRaw) is { } parsed)
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
