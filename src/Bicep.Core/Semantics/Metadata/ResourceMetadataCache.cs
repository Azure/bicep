// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Metadata
{
    public class ResourceMetadataParent
    {
        public ResourceMetadataParent(ResourceMetadata metadata, SyntaxBase? indexExpression, bool isNested)
        {
            // TODO: turn this into a record when the target framework supports it
            Metadata = metadata;
            IndexExpression = indexExpression;
            IsNested = isNested;
        }

        public ResourceMetadata Metadata { get; }

        public SyntaxBase? IndexExpression { get; }

        public bool IsNested { get; }
    }

    public class ResourceMetadata
    {
        public ResourceMetadata(
            ResourceType type,
            SyntaxBase nameSyntax,
            ResourceSymbol symbol,
            ResourceMetadataParent? parent,
            SyntaxBase? scopeSyntax,
            bool isExistingResource)
        {
            // TODO: turn this into a record when the target framework supports it
            Type = type;
            NameSyntax = nameSyntax;
            Symbol = symbol;
            Parent = parent;
            ScopeSyntax = scopeSyntax;
            IsExistingResource = isExistingResource;
        }

        public ResourceSymbol Symbol { get; }

        public ResourceTypeReference TypeReference => Type.TypeReference;

        public ResourceType Type { get; }

        public ResourceMetadataParent? Parent { get; }

        public SyntaxBase NameSyntax { get; }

        public SyntaxBase? ScopeSyntax { get; }

        public bool IsExistingResource { get; }
    }

    public class ResourceMetadataCache : SyntaxMetadataCacheBase<ResourceMetadata?>
    {
        private readonly SemanticModel semanticModel;
        private readonly ConcurrentDictionary<ResourceSymbol, ResourceMetadata> symbolLookup;
        private readonly Lazy<ImmutableDictionary<ResourceDeclarationSyntax, ResourceSymbol>> resourceSymbols;

        public ResourceMetadataCache(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.symbolLookup = new();
            this.resourceSymbols = new(() => ResourceSymbolVisitor.GetAllResources(semanticModel.Root)
                .ToImmutableDictionary(x => x.DeclaringResource));
        }

        protected override ResourceMetadata? Calculate(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case ResourceAccessSyntax _:
                case VariableAccessSyntax _:
                {
                    var symbol = semanticModel.GetSymbolInfo(syntax);
                    if (symbol is DeclaredSymbol declaredSymbol)
                    {
                        return this.TryLookup(declaredSymbol.DeclaringSyntax);
                    }

                    break;
                }
                case ResourceDeclarationSyntax resourceDeclarationSyntax:
                {
                    // Skip analysis for ErrorSymbol and similar cases, these are invalid cases, and won't be emitted.
                    if (!resourceSymbols.Value.TryGetValue(resourceDeclarationSyntax, out var symbol) || 
                        symbol.TryGetResourceType() is not {} resourceType ||
                        symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceNamePropertyName) is not {} nameSyntax)
                    {
                        break;
                    }

                    if (semanticModel.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax) is {} nestedParentSyntax)
                    {
                        // nested resource parent syntax
                        if (TryLookup(nestedParentSyntax) is {} parentMetadata)
                        {
                            return new(
                                resourceType,
                                nameSyntax,
                                symbol,
                                new(parentMetadata,  null, true),
                                symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName),
                                symbol.DeclaringResource.IsExistingResource());
                        }
                    }
                    else if (symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is {} referenceParentSyntax)
                    {
                        SyntaxBase? indexExpression = null;
                        if (referenceParentSyntax is ArrayAccessSyntax arrayAccess)
                        {
                            referenceParentSyntax = arrayAccess.BaseExpression;
                            indexExpression = arrayAccess.IndexExpression;
                        }

                        // parent property reference syntax
                        if (TryLookup(referenceParentSyntax) is {} parentMetadata)
                        {
                            return new(
                                resourceType,
                                nameSyntax,
                                symbol,
                                new(parentMetadata, indexExpression, false),
                                symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName),
                                symbol.DeclaringResource.IsExistingResource());
                        }
                    }
                    else
                    {
                        return new(
                            resourceType,
                            nameSyntax,
                            symbol,
                            null,
                            symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName),
                            symbol.DeclaringResource.IsExistingResource());
                    }

                    break;
                }
            }

            return null;
        }
    }
}
