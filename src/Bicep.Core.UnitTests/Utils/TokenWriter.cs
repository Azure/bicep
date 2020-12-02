// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Text;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public class TokenWriter
    {
        private readonly StringBuilder buffer;

        public TokenWriter(StringBuilder buffer)
        {
            this.buffer = buffer;
        }

        public void WriteTokens(IEnumerable<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                this.WriteToken(token);
            }
        }

        public void WriteToken(Token token)
        {
            WriteTrivia(token.LeadingTrivia);
            this.buffer.Append(token.Text);
            WriteTrivia(token.TrailingTrivia);
        }

        private void WriteTrivia(IEnumerable<SyntaxTrivia> triviaList)
        {
            foreach (var trivia in triviaList)
            {
                this.buffer.Append(trivia.Text);
            }
        }
    }
}

