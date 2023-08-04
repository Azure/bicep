// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ImportedSymbolsListItemSyntax : SyntaxBase, INamedDeclarationSyntax
{
    public ImportedSymbolsListItemSyntax(IdentifierSyntax originalSymbolName, SyntaxBase? asClause)
    {
        AssertSyntaxType(asClause, nameof(asClause), typeof(AliasAsClauseSyntax), typeof(SkippedTriviaSyntax));

        this.OriginalSymbolName = originalSymbolName;
        this.AsClause = asClause;
    }

    public IdentifierSyntax OriginalSymbolName { get; }

    public SyntaxBase? AsClause { get; }

    public IdentifierSyntax Name => (AsClause as AliasAsClauseSyntax)?.Alias ?? OriginalSymbolName;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportedSymbolsListItemSyntax(this);

    public override TextSpan Span => AsClause is {} nonNullAsClause
        ? TextSpan.Between(this.OriginalSymbolName, nonNullAsClause)
        : this.OriginalSymbolName.Span;
}
