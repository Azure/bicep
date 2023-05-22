// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2
{
    public class SyntaxStringifier : CstVisitor
    {
        private readonly StringBuilder buffer;

        private readonly string? newlineReplacement;

        private SyntaxStringifier(string? newlineReplacement)
        {
            this.buffer = new StringBuilder();
            this.newlineReplacement = newlineReplacement;
        }

        public static string Stringify(SyntaxBase syntax, string? newlineReplacement = null)
        {
            var stringifier = new SyntaxStringifier(newlineReplacement);

            stringifier.Visit(syntax);

            return stringifier.buffer.ToString();
        }

        public override void VisitToken(Token token)
        {
            WriteTrivia(token.LeadingTrivia);

            if (token.IsOneOf(TokenType.NewLine, TokenType.MultilineString) &&
                !string.IsNullOrEmpty(this.newlineReplacement))
            {
                buffer.Append(token.Text.ReplaceLineEndings(this.newlineReplacement));
            }
            else
            {
                buffer.Append(token.Text);
            }

            WriteTrivia(token.TrailingTrivia);
        }

        private void WriteTrivia(IEnumerable<SyntaxTrivia> triviaList)
        {
            foreach (var trivia in triviaList)
            {
                buffer.Append(trivia.Text);
            }
        }
    }
}
