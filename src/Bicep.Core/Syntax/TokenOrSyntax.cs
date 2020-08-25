// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class TokenOrSyntax : IPositionable
    {
        public TokenOrSyntax(SyntaxBase syntax)
        {
            Syntax = syntax;
            Span = syntax.Span;
        }

        public TokenOrSyntax(Token token)
        {
            Token = token;
            Span = token.Span;
        }

        public void Visit(Action<SyntaxBase> syntaxFunc, Action<Token> tokenFunc)
        {
            if (Syntax != null)
            {
                syntaxFunc(Syntax);
            }

            if (Token != null)
            {
                tokenFunc(Token);
            }
        }

        public SyntaxBase? Syntax { get; }

        public Token? Token { get; }

        public TextSpan Span { get;}
    }
}