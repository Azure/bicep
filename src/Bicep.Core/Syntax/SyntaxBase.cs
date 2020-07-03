using System;
using System.Collections.Generic;
using Bicep.Core.Parser;

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

            throw new ArgumentException($"{parameterName} must be of type {expectedTypeIfNotNull} but provided token type was {token.Type}.");
        }

        protected void AssertTokenTypeList(IEnumerable<Token> tokens, string parameterName, TokenType expectedType, int minimumCount)
        {
            int index = 0;
            foreach (Token token in tokens)
            {
                if (token.Type != expectedType)
                {
                    throw new ArgumentException($"{parameterName} must contain tokens of type {expectedType}, but the token at index {index} is of type {token.Type}.");
                }

                ++index;
            }

            if (index < minimumCount)
            {
                throw new ArgumentException($"{parameterName} must contain at least {minimumCount}, but the list contains {index} token(s).");
            }
        }
    }
}