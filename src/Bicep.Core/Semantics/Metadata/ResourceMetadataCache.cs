// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Emit;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

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
                case FunctionCallSyntaxBase functionCall:
                {
                    if (this.semanticModel.GetTypeInfo(functionCall) is not ResourceType resourceType)
                    {
                        break;
                    }

                    if (functionCall.Name.IdentifierName == "child" &&
                        functionCall is InstanceFunctionCallSyntax ifc &&
                        this.Calculate(ifc.BaseExpression) is {} parentResource)
                    {
                        return new ResourceMetadata(
                            resourceType,
                            functionCall.Arguments.Skip(1).Select(x => x.Expression).ToImmutableArray(),
                            null,
                            functionCall,
                            new(parentResource, null, false),
                            null,
                            true);
                    }

                    var scopeSyntax = (functionCall as InstanceFunctionCallSyntax)?.BaseExpression;

                    return new ResourceMetadata(
                        resourceType,
                        functionCall.Arguments.Skip(1).Select(x => x.Expression).ToImmutableArray(),
                        null,
                        functionCall,
                        null,
                        scopeSyntax,
                        true);
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
                                ImmutableArray<SyntaxBase>.Empty.Add(nameSyntax),
                                symbol,
                                symbol.DeclaringResource.Value,
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
                                ImmutableArray<SyntaxBase>.Empty.Add(nameSyntax),
                                symbol,
                                symbol.DeclaringResource.Value,
                                new(parentMetadata, indexExpression, false),
                                symbol.SafeGetBodyPropertyValue(LanguageConstants.ResourceScopePropertyName),
                                symbol.DeclaringResource.IsExistingResource());
                        }
                    }
                    else
                    {
                        return new(
                            resourceType,
                            ImmutableArray<SyntaxBase>.Empty.Add(nameSyntax),
                            symbol,
                            symbol.DeclaringResource.Value,
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
