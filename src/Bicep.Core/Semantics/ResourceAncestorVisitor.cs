// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public sealed class ResourceAncestorVisitor : SyntaxVisitor
    {
        private readonly IBinder binder;
        private readonly Stack<ResourceSymbol> ancestorResources;
        private readonly ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceSymbol>>.Builder ancestry;

        public ResourceAncestorVisitor(IBinder binder)
        {
            this.binder = binder;
            this.ancestorResources = new Stack<ResourceSymbol>();
            this.ancestry = ImmutableDictionary.CreateBuilder<ResourceSymbol, ImmutableArray<ResourceSymbol>>();
        }

        public ImmutableDictionary<ResourceSymbol, ImmutableArray<ResourceSymbol>> Ancestry 
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

            // We don't need to do anything here to validate types and their relationships, that was handled during type assignment.
            this.ancestry.Add(symbol, ImmutableArray.CreateRange(this.ancestorResources.Reverse()));

            try
            {
                // This will recursively process the resource body - capture the 'current' declaration's declared resource
                // type so we can validate nesting.
                this.ancestorResources.Push(symbol);
                base.VisitResourceDeclarationSyntax(syntax);
            }
            finally
            {
                this.ancestorResources.Pop();
            }
        }
    }
}