// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class WildcardImportSymbol : DeclaredSymbol, INamespaceSymbol
{
    public WildcardImportSymbol(ISymbolContext context, WildcardImportSyntax declaringSyntax)
        : base(context, declaringSyntax.Name.IdentifierName, declaringSyntax, declaringSyntax.AliasAsClause.Alias)
    {
    }

    public override void Accept(SymbolVisitor visitor)
    {
        visitor.VisitWildcardImportSymbol(this);
    }

    public override SymbolKind Kind => SymbolKind.ImportedNamespace;

    public NamespaceType? TryGetNamespaceType() => this.Type as NamespaceType;
}
