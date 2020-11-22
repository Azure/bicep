// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using JetBrains.Annotations;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxBase : IPositionable
    {
        public abstract void Accept(ISyntaxVisitor visitor);

        public abstract TextSpan Span { get; }

        public virtual bool IsSkipped => false;

        protected static void Assert(bool predicate, string message)
        {
            if (predicate == false)
            {
                // we have a code defect - use the exception stack to debug
                throw new ArgumentException(message);
            }
        }

        protected static void AssertTokenType(Token? token, [InvokerParameterName] string parameterName, TokenType expectedTypeIfNotNull)
        {
            if (token == null || token.Type == expectedTypeIfNotNull)
            {
                return;
            }

            throw new ArgumentException($"{parameterName} must be of type {expectedTypeIfNotNull} but provided token type was {token.Type}.");
        }

        protected static void AssertKeyword(Token? token, [InvokerParameterName] string parameterName, string expectedKeywordNameIfNotNull)
        {
            AssertTokenType(token, parameterName, TokenType.Identifier);
            if (token == null || token.Text == expectedKeywordNameIfNotNull)
            {
                return;
            }

            throw new ArgumentException($"{parameterName} must match keyword {expectedKeywordNameIfNotNull} but provided token was {token.Text}.");
        }

        protected static void AssertTokenTypeList(IEnumerable<Token> tokens, [InvokerParameterName] string parameterName, TokenType expectedType, int minimumCount)
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

        protected static void AssertSyntaxType(SyntaxBase? syntax, [InvokerParameterName] string parameterName, params Type[] expectedTypes)
        {
            if (syntax == null)
            {
                return;
            }

            var syntaxType = syntax.GetType();
            if (expectedTypes.Any(expectedType => syntaxType == expectedType) == false)
            {
                throw new ArgumentException($"{parameterName} is of an unexpected type {syntaxType.Name}. Expected types: {expectedTypes.Select(t => t.Name).ConcatString(", ")}");
            }
        }
    }
}
