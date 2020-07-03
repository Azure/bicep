using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ArraySyntax : SyntaxBase
    {
        public ArraySyntax(Token openBracket, IEnumerable<Token> newLines, IEnumerable<ArrayItemSyntax> items, Token closeBracket)
        {
            AssertTokenType(openBracket, nameof(openBracket), TokenType.LeftSquare);
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 1);
            AssertTokenType(closeBracket, nameof(closeBracket), TokenType.RightSquare);

            this.OpenBracket = openBracket;
            this.NewLines = newLines.ToImmutableArray();
            this.Items = items.ToImmutableArray();
            this.CloseBracket = closeBracket;
        }

        public Token OpenBracket { get; }

        public ImmutableArray<Token> NewLines { get; }

        public ImmutableArray<ArrayItemSyntax> Items { get; }

        public Token CloseBracket { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitArraySyntax(this);

        public override TextSpan Span => TextSpan.Between(this.OpenBracket, this.CloseBracket);
    }
}