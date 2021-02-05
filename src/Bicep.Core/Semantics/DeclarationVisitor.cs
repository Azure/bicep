// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public class DeclarationVisitor: SyntaxVisitor
    {
        private readonly ISymbolContext context;

        private readonly IList<DeclaredSymbol> declaredSymbols;

        private readonly IList<LocalScopeSymbol> outermostScopes;

        private readonly Stack<LocalScopeSymbol> activeScopes = new();

        public DeclarationVisitor(ISymbolContext context, IList<DeclaredSymbol> declaredSymbols, IList<LocalScopeSymbol> outermostScopes)
        {
            this.context = context;
            this.declaredSymbols = declaredSymbols;
            this.outermostScopes = outermostScopes;
        }

        public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            base.VisitParameterDeclarationSyntax(syntax);

            var symbol = new ParameterSymbol(this.context, syntax.Name.IdentifierName, syntax, syntax.Modifier);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            base.VisitVariableDeclarationSyntax(syntax);

            var symbol = new VariableSymbol(this.context, syntax.Name.IdentifierName, syntax, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            base.VisitResourceDeclarationSyntax(syntax);

            var symbol = new ResourceSymbol(this.context, syntax.Name.IdentifierName, syntax);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            base.VisitModuleDeclarationSyntax(syntax);

            var symbol = new ModuleSymbol(this.context, syntax.Name.IdentifierName, syntax);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            base.VisitOutputDeclarationSyntax(syntax);

            var symbol = new OutputSymbol(this.context, syntax.Name.IdentifierName, syntax, syntax.Value);
            this.declaredSymbols.Add(symbol);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            /*
             * We cannot add the local symbol to the list of declarations because it will
             * break name binding at the global namespace level
             */
            var itemVariable = new LocalSymbol(this.context, syntax.ItemVariable.Name.IdentifierName, syntax.ItemVariable);

            // create new scope without any descendants
            var scope = new LocalScopeSymbol(string.Empty, syntax, itemVariable.AsEnumerable(), ImmutableArray<LocalScopeSymbol>.Empty);

            // potentially swap out the top of the stack to append this child (unless we're just starting)
            AppendChildScope(scope);

            // push it
            this.activeScopes.Push(scope);

            // visit the children
            base.VisitForSyntax(syntax);

            // remove from stack
            var lastPopped = this.activeScopes.Pop();

            if (this.activeScopes.Count <= 0)
            {
                // the stack is empty
                // we must add this scope to the list of outermost scopes
                // to keep the whole chain reachable
                // (we also must use what was popped instead of what was pushed since it may have been replaced)
                this.outermostScopes.Add(lastPopped);
            }
        }

        private void AppendChildScope(LocalScopeSymbol newChildScope)
        {
            if (this.activeScopes.Count <= 0)
            {
                // this is the outermost local scope - no descendants to append
                return;
            }

            // pop the parent and append the new child to it
            var parent = this.activeScopes.Pop().AppendChild(newChildScope);

            // push the parent back
            this.activeScopes.Push(parent);
        }
    }
}

