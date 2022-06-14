// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Semantics.Metadata
{
    public class ResourceMetadataCache : SyntaxMetadataCacheBase<ResourceMetadata?>
    {
        private readonly SemanticModel semanticModel;
        private readonly ConcurrentDictionary<ResourceSymbol, ResourceMetadata> symbolLookup;
        private readonly Lazy<ImmutableDictionary<ResourceDeclarationSyntax, ResourceSymbol>> resourceSymbols;
        private readonly ConcurrentDictionary<(ModuleSymbol module, string output), ResourceMetadata> moduleOutputLookup;

        public ResourceMetadataCache(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.symbolLookup = new();
            this.resourceSymbols = new(() => ResourceSymbolVisitor.GetAllResources(semanticModel.Root)
                .ToImmutableDictionary(x => x.DeclaringResource));

            this.moduleOutputLookup = new();
        }

        // NOTE: modules can declare outputs with resource types. This means one piece of syntax (the module)
        // can declare multiple ResourceMetadata. We have this separate code path to 'load' these because
        // the discovery is driven by semantics not syntax.
        public ResourceMetadata? TryAdd(ModuleSymbol module, string output)
        {
            if (module.TryGetBodyPropertyValue(AzResourceTypeProvider.ResourceNamePropertyName) is {} nameSyntax &&
                module.TryGetBodyObjectType() is ObjectType objectType &&
                objectType.Properties.TryGetValue(LanguageConstants.ModuleOutputsPropertyName, out var outputsProperty) &&
                outputsProperty.TypeReference.Type is ObjectType outputsType &&
                outputsType.Properties.TryGetValue(output, out var property))
            {
                if (property.TypeReference.Type is ResourceType resourceType)
                {
                    var metadata = new ModuleOutputResourceMetadata(resourceType, module, nameSyntax, output);
                    moduleOutputLookup.TryAdd((module, output), metadata);
                    return metadata;
                }
            }

            return null;
        }

        protected override ResourceMetadata? Calculate(SyntaxBase syntax)
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();

            switch (syntax)
            {
                case ResourceAccessSyntax _:
                case VariableAccessSyntax _:
                    {
                        var symbol = semanticModel.GetSymbolInfo(syntax);
                        if (symbol is DeclaredSymbol declaredSymbol && semanticModel.Binder.TryGetCycle(declaredSymbol) is null)
                        {
                            return this.TryLookup(declaredSymbol.DeclaringSyntax);
                        }

                    break;
                }
                case ParameterDeclarationSyntax parameterDeclarationSyntax:
                {
                    var symbol = semanticModel.GetSymbolInfo(parameterDeclarationSyntax);
                    if (symbol is ParameterSymbol parameterSymbol && parameterSymbol.Type is ResourceType resourceType)
                    {
                        return new ParameterResourceMetadata(resourceType, parameterSymbol);
                    }
                    break;
                }
                case PropertyAccessSyntax propertyAccessSyntax when IsModuleScalarOutputAccess(propertyAccessSyntax, out var module):
                {
                    // Access to a module output might be a resource metadata.
                    return this.TryAdd(module, propertyAccessSyntax.PropertyName.IdentifierName);
                }
                case PropertyAccessSyntax propertyAccessSyntax when IsModuleArrayOutputAccess(propertyAccessSyntax, out var module):
                {
                    // Access to a module array output might be a resource metadata.
                    return this.TryAdd(module, propertyAccessSyntax.PropertyName.IdentifierName);
                }
                case ResourceDeclarationSyntax resourceDeclarationSyntax:
                    {
                        // Skip analysis for ErrorSymbol and similar cases, these are invalid cases, and won't be emitted.
                        if (!resourceSymbols.Value.TryGetValue(resourceDeclarationSyntax, out var symbol) ||
                            symbol.TryGetResourceType() is not { } resourceType)
                        {
                            break;
                        }

                        if (semanticModel.Binder.TryGetCycle(symbol) is not null)
                        {
                            break;
                        }

                        if (semanticModel.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax) is { } nestedParentSyntax)
                        {
                            // nested resource parent syntax
                            if (TryLookup(nestedParentSyntax) is { } parentMetadata)
                            {
                                return new DeclaredResourceMetadata(
                                    resourceType,
                                    symbol.DeclaringResource.IsExistingResource(),
                                    symbol,
                                    new(parentMetadata, null, true));
                            }
                        }
                        else if (symbol.TryGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is { } referenceParentSyntax)
                        {
                            var (baseSyntax, indexExpression) = SyntaxHelper.UnwrapArrayAccessSyntax(referenceParentSyntax);

                            // parent property reference syntax
                            if (TryLookup(baseSyntax) is { } parentMetadata)
                            {
                                return new DeclaredResourceMetadata(
                                    resourceType,
                                    symbol.DeclaringResource.IsExistingResource(),
                                    symbol,
                                    new(parentMetadata, indexExpression, false));
                            }
                        }
                        else
                        {
                            return new DeclaredResourceMetadata(
                                resourceType,
                                symbol.DeclaringResource.IsExistingResource(),
                                symbol,
                                null);
                        }

                        break;
                    }
                case VariableDeclarationSyntax variableDeclarationSyntax:
                    return this.TryLookup(variableDeclarationSyntax.Value);
            }

            return null;
        }

        private bool IsModuleScalarOutputAccess(PropertyAccessSyntax propertyAccessSyntax, [NotNullWhen(true)] out ModuleSymbol? symbol)
        {
            if (propertyAccessSyntax.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
                childPropertyAccess.PropertyName.IdentifierName == LanguageConstants.ModuleOutputsPropertyName &&
                childPropertyAccess.BaseExpression is VariableAccessSyntax grandChildAccess &&
                this.semanticModel.GetSymbolInfo(grandChildAccess) is ModuleSymbol module &&
                module.TryGetBodyPropertyValue(AzResourceTypeProvider.ResourceNamePropertyName) is {} name)
            {
                symbol = module;
                return true;
            }

            symbol = null;
            return false;
        }

        private bool IsModuleArrayOutputAccess(PropertyAccessSyntax propertyAccessSyntax, [NotNullWhen(true)] out ModuleSymbol? symbol)
        {
            if (propertyAccessSyntax.BaseExpression is PropertyAccessSyntax childPropertyAccess &&
                childPropertyAccess.PropertyName.IdentifierName == LanguageConstants.ModuleOutputsPropertyName &&
                childPropertyAccess.BaseExpression is ArrayAccessSyntax arrayAccessSyntax &&
                arrayAccessSyntax.BaseExpression is  VariableAccessSyntax grandChildAccess &&
                this.semanticModel.GetSymbolInfo(grandChildAccess) is ModuleSymbol module &&
                module.TryGetBodyPropertyValue(AzResourceTypeProvider.ResourceNamePropertyName) is {} name)
            {
                symbol = module;
                return true;
            }

            symbol = null;
            return false;
        }
    }
}
