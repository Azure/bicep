// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TupleTypeSyntax : TypeSyntax
{
    public TupleTypeSyntax(Token openBracket, IEnumerable<SyntaxBase> children, Token closeBracket)
    {
        AssertTokenType(openBracket, nameof(openBracket), TokenType.LeftSquare);
        AssertTokenType(closeBracket, nameof(closeBracket), TokenType.RightSquare);

        this.OpenBracket = openBracket;
        this.Children = children.ToImmutableArray();
        this.CloseBracket = closeBracket;
    }

    public Token OpenBracket { get; }

    /// <summary>
    /// Gets the list of child nodes. Children may not necessarily be tuple member nodes.
    /// </summary>
    public ImmutableArray<SyntaxBase> Children { get; }

    public Token CloseBracket { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTupleTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.OpenBracket, this.CloseBracket);

    public IEnumerable<TupleTypeItemSyntax> Items => this.Children.OfType<TupleTypeItemSyntax>();
}
