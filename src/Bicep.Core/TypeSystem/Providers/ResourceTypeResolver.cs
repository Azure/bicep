// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Providers
{
    public class ResourceTypeResolver
    {
        private readonly SemanticModel semanticModel;

        private readonly ImmutableDictionary<ResourceSymbol, ObjectType> existingResourceBodyTypeOverrides;

        private ResourceTypeResolver(SemanticModel semanticModel, IReadOnlyDictionary<ResourceSymbol, ObjectType> existingResourceBodyTypeOverrides)
        {
            this.semanticModel = semanticModel;
            this.existingResourceBodyTypeOverrides = existingResourceBodyTypeOverrides.ToImmutableDictionary();
        }

        public static ResourceTypeResolver Create(SemanticModel semanticModel)
        {
            var existingResourceBodyTypeOverrides = CreateExistingResourceBodyTypeOverrides(semanticModel);

            return new(semanticModel, existingResourceBodyTypeOverrides);
        }

        public (ResourceSymbol?, ObjectType?) TryResolveRuntimeExistingResourceSymbolAndBodyType(SyntaxBase resourceOrModuleAccessSyntax)
        {
            var resolved = TryResolveResourceOrModuleSymbolAndBodyType(resourceOrModuleAccessSyntax);

            if (resolved is (ResourceSymbol resourceSymbol, { } bodyType) &&
                resourceSymbol.TryGetResourceType() is { } resourceType &&
                // this validation only applies to resources under the "az" extension
                resourceType.IsAzResource() &&
                resourceSymbol.DeclaringResource.IsExistingResource())
            {

                foreach (var identifierPropertyName in AzResourceTypeProvider.UniqueIdentifierProperties)
                {
                    if (bodyType.Properties.TryGetValue(identifierPropertyName, out var identifierPropertyType) &&
                        !identifierPropertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
                    {
                        // Found an existing resource whose identifier properties are not readable at deploy-time, for example,
                        // the name/scope/parent property references a module output).
                        return (resourceSymbol, bodyType);
                    }
                }
            }

            return (null, null);
        }

        public (DeclaredSymbol?, ObjectType?) TryResolveResourceOrModuleSymbolAndBodyType(SyntaxBase resourceOrModuleAccessSyntax)
        {
            if (resourceOrModuleAccessSyntax is not ArrayAccessSyntax { BaseExpression: var baseAccessSyntax, IndexExpression: var indexExpression })
            {
                return TryResolveResourceOrModuleSymbolAndBodyType(resourceOrModuleAccessSyntax, false);
            }

            var indexExprTypeInfo = semanticModel.GetTypeInfo(indexExpression);
            var isCollection = indexExprTypeInfo.TypeKind != TypeKind.StringLiteral;

            var syntaxToResolve = isCollection ? baseAccessSyntax : resourceOrModuleAccessSyntax;
            return TryResolveResourceOrModuleSymbolAndBodyType(syntaxToResolve, isCollection);
        }

        private (DeclaredSymbol?, ObjectType?) TryResolveResourceOrModuleSymbolAndBodyType(SyntaxBase syntax, bool isCollection) => semanticModel.GetSymbolInfo(syntax) switch
        {
            ResourceSymbol resourceSymbol when resourceSymbol.IsCollection == isCollection =>
                (resourceSymbol, existingResourceBodyTypeOverrides.GetValueOrDefault(resourceSymbol) ?? resourceSymbol.TryGetBodyObjectType()),
            ModuleSymbol moduleSymbol when moduleSymbol.IsCollection == isCollection =>
                (moduleSymbol, moduleSymbol.TryGetBodyObjectType()),
            ParameterSymbol parameterSymbol when parameterSymbol.IsCollection == isCollection =>
                    (parameterSymbol, parameterSymbol.TryGetBodyObjectType()),
            _ => (null, null),
        };

        private static IReadOnlyDictionary<ResourceSymbol, ObjectType> CreateExistingResourceBodyTypeOverrides(SemanticModel semanticModel)
        {
            var existingResourceBodyTypeOverrides = new Dictionary<ResourceSymbol, ObjectType>();

            // grab all existing resources
            var existingResourceSymbols = semanticModel.DeclaredResources
                .Where(x => x.IsExistingResource)
                .Select(x => x.Symbol)
                .ToArray();

            // compare existing resources to all other existing resources to find dependencies and see if it has a non-DTC value
            // and needs to remove the ReadableAtDeployTime flag from the "name" property -> O(n^2) time complexity with some optimizations
            foreach (var _ in existingResourceSymbols)
            {
                int count = existingResourceBodyTypeOverrides.Count;

                foreach (var existingResourceSymbol in existingResourceSymbols)
                {
                    // skip if the existing resource symbol is already in the dictionary because there is no need to check the DTC value again
                    if (existingResourceBodyTypeOverrides.ContainsKey(existingResourceSymbol))
                    {
                        continue;
                    }

                    // if "name" property has a non-DTC value, then make an entry for the corresponding existing ResourceSymbol to a modified ObjectType in the dictionary
                    if (existingResourceSymbol.DeclaringResource.TryGetBody() is { } existingResourceBody &&
                        existingResourceSymbol.TryGetBodyObjectType() is { } existingResourceBodyType)
                    {
                        var resourceTypeResolver = new ResourceTypeResolver(semanticModel, existingResourceBodyTypeOverrides);

                        foreach (var propertyName in AzResourceTypeProvider.UniqueIdentifierProperties)
                        {
                            if (existingResourceBody.TryGetPropertyByName(propertyName) is { } identifierProperty)
                            {
                                var diagnosticWriter = new SimpleDiagnosticWriter();

                                DeployTimeConstantValidator.Validate(identifierProperty, semanticModel, resourceTypeResolver, diagnosticWriter);

                                // If a DTC diagnostic was caught, the existing resource identifier property contains a runtime value.
                                // Remove ReadableAtDeployTime flag from the name property.
                                if (diagnosticWriter.HasDiagnostics())
                                {
                                    existingResourceBodyTypeOverrides[existingResourceSymbol] = ClearReadableAtDeployTimeFlags(propertyName, existingResourceBodyType); ;
                                }
                            }
                        }
                    }
                }

                // if no new entries have been added to existingResourceBodyObjectTypeOverrides, that means there are no further DTC checks needed
                if (count == existingResourceBodyTypeOverrides.Count)
                {
                    break;
                }
            }

            // now map every resourceSymbol in the dictionary to the same ObjectType but with the DeployTimeConstant flag removed this time
            foreach (var (existingResourceSymbol, existingResourceBodyType) in existingResourceBodyTypeOverrides)
            {
                existingResourceBodyTypeOverrides[existingResourceSymbol] = ClearDeployTimeConstantFlagIfNotReadableAtDeployTime(existingResourceBodyType);
            }

            return existingResourceBodyTypeOverrides;
        }

        private static ObjectType ClearReadableAtDeployTimeFlags(string propertyName, ObjectType existingResourceBodyType)
        {
            if (existingResourceBodyType.Properties.TryGetValue(propertyName, out var propertyType))
            {
                propertyType = propertyType with { Flags = propertyType.Flags & ~TypePropertyFlags.ReadableAtDeployTime };

                existingResourceBodyType = existingResourceBodyType.With(
                    properties: existingResourceBodyType.Properties.SetItem(propertyName, propertyType).Values);
            }

            return existingResourceBodyType;
        }

        private static ObjectType ClearDeployTimeConstantFlagIfNotReadableAtDeployTime(ObjectType existingResourceBodyType)
        {
            foreach (var propertyName in AzResourceTypeProvider.UniqueIdentifierProperties)
            {
                if (existingResourceBodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                    !propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
                {
                    propertyType = propertyType with { Flags = propertyType.Flags & ~TypePropertyFlags.DeployTimeConstant };

                    existingResourceBodyType = existingResourceBodyType.With(
                        properties: existingResourceBodyType.Properties.SetItem(propertyName, propertyType).Values);
                }
            }

            return existingResourceBodyType;
        }
    }
}
