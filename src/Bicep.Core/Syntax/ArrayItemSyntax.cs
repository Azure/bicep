using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ArrayItemSyntax : SyntaxBase, IExpressionSyntax, ILiteralSyntax
    {
        public ArrayItemSyntax(SyntaxBase value, IEnumerable<Token> newLines)
        {
            AssertTokenTypeList(newLines, nameof(newLines), TokenType.NewLine, 1);

            this.Value = value;
            this.NewLines = newLines.ToImmutableArray();
        }

        public SyntaxBase Value { get; }

        public ImmutableArray<Token> NewLines { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitArrayItemSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Value, this.NewLines.Last());
    }
}