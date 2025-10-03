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

        private readonly IReadOnlyDictionary<ResourceSymbol, ObjectType> existingResourceBodyTypeOverrides;

        private ResourceTypeResolver(SemanticModel semanticModel, IReadOnlyDictionary<ResourceSymbol, ObjectType> existingResourceBodyTypeOverrides)
        {
            this.semanticModel = semanticModel;
            this.existingResourceBodyTypeOverrides = existingResourceBodyTypeOverrides;
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

        public ObjectType? TryGetBodyObjectType(ResourceSymbol resource)
            => existingResourceBodyTypeOverrides.GetValueOrDefault(resource) ?? resource.TryGetBodyObjectType();

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
            static IEnumerable<ResourceSymbol> TopoSort(IBinder binder, IReadOnlySet<ResourceSymbol> resources)
            {
                HashSet<ResourceSymbol> processed = new();
                IEnumerable<ResourceSymbol> YieldResourceAndUnprocessedPredecessors(ResourceSymbol resource)
                {
                    if (!processed.Add(resource))
                    {
                        yield break;
                    }

                    foreach (var predecessor in binder.GetSymbolsReferencedInDeclarationOf(resource)
                        .OfType<ResourceSymbol>()
                        .SelectMany(YieldResourceAndUnprocessedPredecessors))
                    {
                        if (resources.Contains(predecessor))
                        {
                            yield return predecessor;
                        }
                    }

                    yield return resource;
                }

                return resources.SelectMany(YieldResourceAndUnprocessedPredecessors);
            }

            var existingResourceBodyTypeOverrides = new Dictionary<ResourceSymbol, ObjectType>();
            var resourceTypeResolver = new ResourceTypeResolver(semanticModel, existingResourceBodyTypeOverrides);

            foreach (var existingResource in TopoSort(semanticModel.Binder, semanticModel.SymbolsToInline.ExistingResourcesToInline))
            {
                if (existingResource.TryGetBodyObjectType() is { } existingResourceBodyType)
                {
                    List<string> notReadableAtDeployTime = new() { AzResourceTypeProvider.ResourceIdPropertyName };
                    foreach (var propertyName in AzResourceTypeProvider.UniqueIdentifierProperties)
                    {
                        if (existingResource.TryGetBodyProperty(propertyName) is { } identifierProperty)
                        {
                            var diagnosticWriter = new SimpleDiagnosticWriter();

                            DeployTimeConstantValidator.Validate(identifierProperty, semanticModel, resourceTypeResolver, diagnosticWriter);

                            // If a DTC diagnostic was caught, the existing resource identifier property contains a runtime value.
                            // Remove ReadableAtDeployTime flag from the name property.
                            if (diagnosticWriter.HasDiagnostics())
                            {
                                notReadableAtDeployTime.Add(propertyName);
                            }
                        }
                    }

                    foreach (var propertyName in notReadableAtDeployTime)
                    {
                        existingResourceBodyType = ClearReadableAtDeployTimeFlags(propertyName, existingResourceBodyType);
                    }

                    existingResourceBodyTypeOverrides[existingResource] = ClearDeployTimeConstantFlagIfNotReadableAtDeployTime(existingResourceBodyType);
                }
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
            foreach (var propertyName in AzResourceTypeProvider.UniqueIdentifierProperties
                .Append(AzResourceTypeProvider.ResourceIdPropertyName))
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
