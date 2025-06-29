// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    /// <summary>
    /// Represents a language scope that declares local symbols. (For example the item or index variables in loops are local symbols.)
    /// </summary>
    public class LocalScope : Symbol, ILanguageScope
    {
        public LocalScope(string name, SyntaxBase declaringSyntax, SyntaxBase bindingSyntax, IEnumerable<DeclaredSymbol> locals, IEnumerable<LocalScope> childScopes, ScopeResolution scopeResolution)
            : base(name)
        {
            this.DeclaringSyntax = declaringSyntax;
            this.BindingSyntax = bindingSyntax;
            this.ScopeResolution = scopeResolution;
            this.Locals = [.. locals];
            this.ChildScopes = [.. childScopes];
        }

        /// <summary>
        /// The syntax node that declares the scope, but may not have effect on name binding. Most commonly this will be a ForSyntax object.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }

        /// <summary>
        /// The syntax node within which this scope will affect binding. This will typically be the Body of a ForSyntax node.
        /// </summary>
        /// <remarks>Identifiers within this node will first bind to symbols in this scope. Identifiers above this node will bind to the parent scope.</remarks>
        public SyntaxBase BindingSyntax { get; }

        public ImmutableArray<DeclaredSymbol> Locals { get; }

        public ImmutableArray<LocalScope> ChildScopes { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitLocalScope(this);

        public override SymbolKind Kind => SymbolKind.Scope;

        public override IEnumerable<Symbol> Descendants => this.ChildScopes.Concat<Symbol>(this.Locals);

        public LocalScope ReplaceLocals(IEnumerable<DeclaredSymbol> newLocals) => new(this.Name, this.DeclaringSyntax, this.BindingSyntax, newLocals, this.ChildScopes, this.ScopeResolution);

        public LocalScope ReplaceChildren(IEnumerable<LocalScope> newChildren) => new(this.Name, this.DeclaringSyntax, this.BindingSyntax, this.Locals, newChildren, this.ScopeResolution);

        public IEnumerable<DeclaredSymbol> GetDeclarationsByName(string name) => this.Locals.Where(symbol => symbol.NameSource.IsValid && string.Equals(symbol.Name, name, LanguageConstants.IdentifierComparison)).ToList();

        public IEnumerable<DeclaredSymbol> Declarations => this.Locals;

        public ScopeResolution ScopeResolution { get; }
    }
}
