// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TypedVariableBlockSyntax : SyntaxBase
{
    public TypedVariableBlockSyntax(Token openParen, IEnumerable<SyntaxBase> children, SyntaxBase closeParen)
    {
        AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);

        this.OpenParen = openParen;
        this.Children = [.. children];
        this.CloseParen = closeParen;
        this.Arguments = [.. this.Children.OfType<TypedLocalVariableSyntax>()];
    }

    public Token OpenParen { get; }

    public ImmutableArray<SyntaxBase> Children { get; }

    public ImmutableArray<TypedLocalVariableSyntax> Arguments { get; }

    public SyntaxBase CloseParen { get; }

    public override TextSpan Span => TextSpan.Between(this.OpenParen, this.CloseParen);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypedVariableBlockSyntax(this);
}
