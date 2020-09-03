// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ArraySyntax : SyntaxBase, IExpressionSyntax
    {
        public ArraySyntax(Token openBracket, IEnumerable<Token> newLines, IEnumerable<SyntaxBase> children, Token closeBracket)
        {
            AssertTokenType(openBracket, nameof(openBracket), TokenType.LeftSquare);
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 0);
            AssertTokenType(closeBracket, nameof(closeBracket), TokenType.RightSquare);

            this.OpenBracket = openBracket;
            this.NewLines = newLines.ToImmutableArray();
            this.Children = children.ToImmutableArray();
            this.CloseBracket = closeBracket;
        }

        public Token OpenBracket { get; }

        public ImmutableArray<Token> NewLines { get; }

        /// <summary>
        /// Gets the list of child nodes. Children may not necessarily be array item nodes.
        /// </summary>
        public ImmutableArray<SyntaxBase> Children { get; }

        public Token CloseBracket { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitArraySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.OpenBracket, this.CloseBracket);

        public IEnumerable<ArrayItemSyntax> Items => this.Children.OfType<ArrayItemSyntax>();
        
        public ExpressionKind ExpressionKind => ExpressionKind.ArrayLiteral;
    }
}
