// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    /// <summary>
    /// Represents a language scope that declares local symbols. (For example the item or index variables in loops are local symbols.)
    /// </summary>
    public class LocalScopeSymbol : Symbol, ILanguageScope
    {
        public LocalScopeSymbol(string name, SyntaxBase enclosingSyntax, IEnumerable<DeclaredSymbol> declaredSymbols, IEnumerable<LocalScopeSymbol> childScopes)
            : base(name)
        {
            this.EnclosingSyntax = enclosingSyntax;
            this.DeclaredSymbols = declaredSymbols.ToImmutableArray();
            this.ChildScopes = childScopes.ToImmutableArray();
        }

        public SyntaxBase EnclosingSyntax { get; }

        public ImmutableArray<DeclaredSymbol> DeclaredSymbols { get; }

        public ImmutableArray<LocalScopeSymbol> ChildScopes { get; }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitLocalScopeSymbol(this);

        public override SymbolKind Kind => SymbolKind.Scope;

        public override IEnumerable<Symbol> Descendants => this.ChildScopes.Concat<Symbol>(this.DeclaredSymbols);

        public LocalScopeSymbol AppendChild(LocalScopeSymbol newChild) => new(this.Name, this.EnclosingSyntax, this.DeclaredSymbols, this.ChildScopes.Append(newChild));

        public IEnumerable<DeclaredSymbol> GetDeclarationsByName(string name) => this.DeclaredSymbols.Where(symbol => symbol.NameSyntax.IsValid && string.Equals(symbol.Name, name, LanguageConstants.IdentifierComparison)).ToList();

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            // TODO: Remove when loops codegen is done.
            yield return DiagnosticBuilder.ForPosition(((ForSyntax) this.EnclosingSyntax).ForKeyword).LoopsNotSupported();
        }
    }
}