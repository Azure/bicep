// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.TypeSystem
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

        public (DeclaredSymbol?, ObjectType?) TryResolveResourceOrModuleSymbolAndBodyType(SyntaxBase resourceOrModuleAccessSyntax) =>
            resourceOrModuleAccessSyntax is ArrayAccessSyntax { IndexExpression: IntegerLiteralSyntax, BaseExpression: var baseAccessSyntax }
                ? TryResolveResourceOrModuleSymbolAndBodyType(baseAccessSyntax, true)
                : TryResolveResourceOrModuleSymbolAndBodyType(resourceOrModuleAccessSyntax, false);

        private (DeclaredSymbol?, ObjectType?) TryResolveResourceOrModuleSymbolAndBodyType(SyntaxBase syntax, bool isCollection) => this.semanticModel.GetSymbolInfo(syntax) switch
        {
            ResourceSymbol resourceSymbol when resourceSymbol.IsCollection == isCollection =>
                (resourceSymbol, this.existingResourceBodyTypeOverrides.GetValueOrDefault(resourceSymbol) ?? resourceSymbol.TryGetBodyObjectType()),
            ModuleSymbol moduleSymbol when moduleSymbol.IsCollection == isCollection =>
                (moduleSymbol, moduleSymbol.TryGetBodyObjectType()),
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
                        existingResourceBody.TryGetPropertyByName(AzResourceTypeProvider.ResourceNamePropertyName) is { } nameProperty &&
                        existingResourceSymbol.TryGetBodyObjectType() is { } existingResourceBodyType)
                    {
                        var diagnosticWriter = new SimpleDiagnosticWriter();
                        var resourceTypeResolver = new ResourceTypeResolver(semanticModel, existingResourceBodyTypeOverrides);

                        DeployTimeConstantValidator.Validate(nameProperty, semanticModel, resourceTypeResolver, diagnosticWriter);

                        // If a DTC diagnostic was caught, the existing resource name property contains a runtime value.
                        // Remove ReadableAtDeployTime flag from the name property.
                        if (diagnosticWriter.HasDiagnostics())
                        {
                            existingResourceBodyTypeOverrides[existingResourceSymbol] = ClearNamePropertyFlags(existingResourceBodyType, TypePropertyFlags.ReadableAtDeployTime);;
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
                existingResourceBodyTypeOverrides[existingResourceSymbol] = ClearNamePropertyFlags(existingResourceBodyType, TypePropertyFlags.DeployTimeConstant);
            }

            return existingResourceBodyTypeOverrides;
        }

        private static ObjectType ClearNamePropertyFlags(ObjectType existingResourceBodyType, TypePropertyFlags flagsToClear)
        {
            var namePropertyType = existingResourceBodyType.Properties[AzResourceTypeProvider.ResourceNamePropertyName];
            namePropertyType = namePropertyType.With(namePropertyType.Flags & ~flagsToClear);

            return existingResourceBodyType.With(
                properties: existingResourceBodyType.Properties.SetItem(AzResourceTypeProvider.ResourceNamePropertyName, namePropertyType).Values);
        }
    }
}
