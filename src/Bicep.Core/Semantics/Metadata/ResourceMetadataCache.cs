// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
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

        public ResourceMetadataCache(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.symbolLookup = new();
        }

        protected override ResourceMetadata? Calculate(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case VariableDeclarationSyntax variableDeclarationSyntax:
                {
                    return this.TryLookup(variableDeclarationSyntax.Value);
                }
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
                    var symbol = semanticModel.GetSymbolInfo(syntax) as ResourceSymbol;
                    if (symbol is null || 
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
