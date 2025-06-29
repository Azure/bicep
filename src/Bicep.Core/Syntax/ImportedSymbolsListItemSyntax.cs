// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ImportedSymbolsListItemSyntax : SyntaxBase, INamedDeclarationSyntax
{
    public ImportedSymbolsListItemSyntax(SyntaxBase originalSymbolName, SyntaxBase? asClause)
    {
        AssertSyntaxType(originalSymbolName, nameof(originalSymbolName), typeof(StringSyntax), typeof(IdentifierSyntax), typeof(SkippedTriviaSyntax));
        AssertSyntaxType(asClause, nameof(asClause), typeof(AliasAsClauseSyntax), typeof(SkippedTriviaSyntax));

        this.OriginalSymbolName = originalSymbolName;
        this.AsClause = asClause;

        Name = AsClause switch
        {
            AliasAsClauseSyntax aliasAsClause => aliasAsClause.Alias,
            _ => OriginalSymbolName switch
            {
                IdentifierSyntax identifier => identifier,
                SkippedTriviaSyntax triviaSyntax => new IdentifierSyntax(triviaSyntax),
                _ => throw new ArgumentException(DiagnosticBuilder.ForDocumentStart().ImportListItemDoesNotIncludeDeclaredSymbolName().Message),
            },
        };
    }

    public string? TryGetOriginalSymbolNameText() => OriginalSymbolName switch
    {
        IdentifierSyntax identifier => identifier.IdentifierName,
        StringSyntax @string when @string.TryGetLiteralValue() is string literalValue => literalValue,
        _ => null,
    };

    public SyntaxBase OriginalSymbolName { get; }

    public SyntaxBase? AsClause { get; }

    public IdentifierSyntax Name { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportedSymbolsListItemSyntax(this);

    public override TextSpan Span => AsClause is { } nonNullAsClause
        ? TextSpan.Between(this.OriginalSymbolName, nonNullAsClause)
        : this.OriginalSymbolName.Span;
}
