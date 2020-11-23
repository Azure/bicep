// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ArraySyntax : ExpressionSyntax
    {
        public ArraySyntax(Token openBracket, IEnumerable<SyntaxBase> children, Token closeBracket)
        {
            AssertTokenType(openBracket, nameof(openBracket), TokenType.LeftSquare);
            AssertTokenType(closeBracket, nameof(closeBracket), TokenType.RightSquare);

            this.OpenBracket = openBracket;
            this.Children = children.ToImmutableArray();
            this.CloseBracket = closeBracket;
        }

        public Token OpenBracket { get; }

        /// <summary>
        /// Gets the list of child nodes. Children may not necessarily be array item nodes.
        /// </summary>
        public ImmutableArray<SyntaxBase> Children { get; }

        public Token CloseBracket { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArraySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.OpenBracket, this.CloseBracket);

        public IEnumerable<ArrayItemSyntax> Items => this.Children.OfType<ArrayItemSyntax>();
    }
}
