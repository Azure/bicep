// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics.Metadata
{
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
                                return new(
                                    resourceType,
                                    symbol,
                                    new(parentMetadata, null, true),
                                    symbol.DeclaringResource.IsExistingResource());
                            }
                        }
                        else if (symbol.TryGetBodyPropertyValue(LanguageConstants.ResourceParentPropertyName) is { } referenceParentSyntax)
                        {
                            SyntaxBase? indexExpression = null;
                            if (referenceParentSyntax is ArrayAccessSyntax arrayAccess)
                            {
                                referenceParentSyntax = arrayAccess.BaseExpression;
                                indexExpression = arrayAccess.IndexExpression;
                            }

                            // parent property reference syntax
                            if (TryLookup(referenceParentSyntax) is { } parentMetadata)
                            {
                                return new(
                                    resourceType,
                                    symbol,
                                    new(parentMetadata, indexExpression, false),
                                    symbol.DeclaringResource.IsExistingResource());
                            }
                        }
                        else
                        {
                            return new(
                                resourceType,
                                symbol,
                                null,
                                symbol.DeclaringResource.IsExistingResource());
                        }

                        break;
                    }
            }

            return null;
        }
    }
}
