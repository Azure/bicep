// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Syntax;
using static Bicep.Core.Semantics.ResourceAncestorGraph;

namespace Bicep.Core.Semantics
{
    public sealed class ResourceAncestorVisitor : SyntaxVisitor
    {
        private readonly IBinder binder;
        private readonly ImmutableDictionary<ResourceSymbol, ResourceAncestor>.Builder ancestry;

        public ResourceAncestorVisitor(IBinder binder)
        {
            this.binder = binder;
            this.ancestry = ImmutableDictionary.CreateBuilder<ResourceSymbol, ResourceAncestor>();
        }

        public ImmutableDictionary<ResourceSymbol, ResourceAncestor> Ancestry 
            => this.ancestry.ToImmutableDictionary();

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // Skip analysis for ErrorSymbol and similar cases, these are invalid cases, and won't be emitted.
            var symbol = this.binder.GetSymbolInfo(syntax) as ResourceSymbol;
            if (symbol is null)
            {
                base.VisitResourceDeclarationSyntax(syntax);
                return;
            }
            
            if (this.binder.GetNearestAncestor<ResourceDeclarationSyntax>(syntax) is {} nestedParentSyntax)
            {
                // nested resource parent syntax
                if (this.binder.GetSymbolInfo(nestedParentSyntax) is ResourceSymbol parentSymbol)
                {
                    this.ancestry.Add(symbol, new ResourceAncestor(ResourceAncestorType.Nested, parentSymbol, null));
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
                if (this.binder.GetSymbolInfo(referenceParentSyntax) is ResourceSymbol parentSymbol)
                {
                    this.ancestry.Add(symbol, new ResourceAncestor(ResourceAncestorType.ParentProperty, parentSymbol, indexExpression));
                }
            }

            base.VisitResourceDeclarationSyntax(syntax);
            return;
        }
    }
}