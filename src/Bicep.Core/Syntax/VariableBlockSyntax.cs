// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class VariableBlockSyntax : SyntaxBase
    {
        public VariableBlockSyntax(Token openParen, IEnumerable<SyntaxBase> children, SyntaxBase closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);

            this.OpenParen = openParen;
            this.Children = children.ToImmutableArray();
            this.CloseParen = closeParen;
            this.Arguments = Children.OfType<LocalVariableSyntax>().ToImmutableArray();
        }

        public Token OpenParen { get; }

        public ImmutableArray<SyntaxBase> Children { get; }

        public ImmutableArray<LocalVariableSyntax> Arguments { get; }

        public SyntaxBase CloseParen { get; }

        public override TextSpan Span => TextSpan.Between(this.OpenParen, this.CloseParen);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitVariableBlockSyntax(this);
    }
}
