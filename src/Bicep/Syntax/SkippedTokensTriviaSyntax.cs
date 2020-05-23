using System.Collections.Generic;
using System.Linq;
using Bicep.Parser;

namespace Bicep.Syntax
{
    public class SkippedTokensTriviaSyntax : SyntaxBase
    {
        public SkippedTokensTriviaSyntax(IEnumerable<Token> tokens)
        {
            Tokens = tokens.ToList();
        }

        public IReadOnlyList<Token> Tokens { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitSkippedTokensTriviaSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Tokens.First(), Tokens.Last());
    }
}