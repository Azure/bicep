// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ImportedSymbolsListSyntax : SyntaxBase
{
    public ImportedSymbolsListSyntax(Token openBrace, IEnumerable<SyntaxBase> children, Token closeBrace)
    {
        AssertTokenType(openBrace, nameof(openBrace), TokenType.LeftBrace);
        AssertTokenType(closeBrace, nameof(closeBrace), TokenType.RightBrace);

        this.OpenBrace = openBrace;
        this.Children = [.. children];
        this.CloseBrace = closeBrace;
    }

    public Token OpenBrace { get; }

    /// <summary>
    /// Gets the child syntax nodes. May return nodes that aren't valid symbol imports.
    /// </summary>
    public ImmutableArray<SyntaxBase> Children { get; }

    public Token CloseBrace { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitImportedSymbolsListSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.OpenBrace, this.CloseBrace);

    public IEnumerable<ImportedSymbolsListItemSyntax> ImportedSymbols => Children.OfType<ImportedSymbolsListItemSyntax>();
}
