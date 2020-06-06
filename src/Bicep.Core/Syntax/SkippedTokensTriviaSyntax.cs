using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class SkippedTokensTriviaSyntax : SyntaxBase
    {
        public SkippedTokensTriviaSyntax(IEnumerable<Token> tokens, string errorMessage, Token errorCause)
        {
            this.Tokens = tokens.ToList().AsReadOnly();
            this.ErrorMessage = errorMessage;
            this.ErrorCause = errorCause;
        }

        public IReadOnlyList<Token> Tokens { get; }

        public string ErrorMessage { get; }

        public Token ErrorCause { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitSkippedTokensTriviaSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(Tokens.First(), Tokens.Last());
    }
}