// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public sealed class DeclarationVisitor: SyntaxVisitor
    {
        private readonly ISymbolContext context;

        private readonly IList<DeclaredSymbol> declaredSymbols;

        private readonly IList<ScopeInfo> childScopes;

        private readonly Stack<ScopeInfo> activeScopes = new();

        private DeclarationVisitor(ISymbolContext context, IList<DeclaredSymbol> declaredSymbols, IList<ScopeInfo> childScopes)
        {
            this.context = context;
            this.declaredSymbols = declaredSymbols;
            this.childScopes = childScopes;
        }

        public static (ImmutableArray<DeclaredSymbol>, ImmutableArray<LocalScope>) GetAllDeclarations(SyntaxTree syntaxTree, ISymbolContext symbolContext)
        {
            // collect declarations
            var declarations = new List<DeclaredSymbol>();
            var childScopes = new List<ScopeInfo>();
            var declarationVisitor = new DeclarationVisitor(symbolContext, declarations, childScopes);
            declarationVisitor.Visit(syntaxTree.ProgramSyntax);

            return (declarations.ToImmutableArray(), childScopes.Select(MakeImmutable).ToImmutableArray());
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
            var itemVariable = new LocalVariableSymbol(this.context, syntax.ItemVariable.Name.IdentifierName, syntax.ItemVariable);

            // create new scope without any descendants
            var scope = new LocalScope(string.Empty, syntax, syntax.Body, itemVariable.AsEnumerable(), ImmutableArray<LocalScope>.Empty);

            this.PushScope(scope);

            // visit the children
            base.VisitForSyntax(syntax);

            this.PopScope();
        }

        private void PushScope(LocalScope scope)
        {
            var item = new ScopeInfo(scope);

            if (this.activeScopes.TryPeek(out var current))
            {
                // add this one to the parent
                current.Children.Add(item);
            }
            else
            {
                // add this to the root list
                this.childScopes.Add(item);
            }
            
            this.activeScopes.Push(item);
        }

        private void PopScope()
        {
            this.activeScopes.Pop();
        }

        private static LocalScope MakeImmutable(ScopeInfo info)
        {
            return info.Scope.ReplaceChildren(info.Children.Select(MakeImmutable));
        }

        /// <summary>
        /// Allows us to mutate child scopes without having to swap out items on the stack
        /// which is fragile.
        /// </summary>
        /// <remarks>This could be replaced with a record if we could target .net 5</remarks>
        private class ScopeInfo
        {
            public ScopeInfo(LocalScope scope)
            {
                this.Scope = scope;
            }

            public LocalScope Scope { get; }

            public IList<ScopeInfo> Children { get; } = new List<ScopeInfo>();
        }
    }
}

