using System;
using Bicep.Core.Parser;
using Bicep.Core.Visitors;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxBase : IPositionable
    {
        public abstract void Accept(SyntaxVisitor visitor);

        public abstract TextSpan Span { get; }

        protected void Assert(bool predicate, string message)
        {
            if (predicate == false)
            {
                // we have a code defect - use the exception stack to debug
                throw new ArgumentException(message);
            }
        }

        protected void AssertTokenType(Token? token, string parameterName, TokenType expectedTypeIfNotNull)
        {
            if (token == null || token.Type == expectedTypeIfNotNull)
            {
                return;
            }

            throw new ArgumentException($"{parameterName} must be of type {expectedTypeIfNotNull} but provided token type was {token.Type}");
        }
    }
}