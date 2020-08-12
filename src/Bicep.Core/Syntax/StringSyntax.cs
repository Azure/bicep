using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class StringSyntax : SyntaxBase, IExpressionSyntax, ILiteralSyntax
    {
        public StringSyntax(IEnumerable<Token> stringTokens, IEnumerable<SyntaxBase> expressions)
        {
            this.StringTokens = stringTokens.ToImmutableArray();
            this.Expressions = expressions.ToImmutableArray();
        }

        public ImmutableArray<Token> StringTokens { get; }

        public ImmutableArray<SyntaxBase> Expressions { get; }

        public bool IsInterpolated() => Expressions.Any();

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitStringSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(StringTokens.First(), StringTokens.Last());
    }
}