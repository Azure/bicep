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

        private readonly bool includeTrivia;

        private SyntaxStringifier(string? newlineReplacement, bool includeTrivia)
        {
            this.buffer = new StringBuilder();
            this.newlineReplacement = newlineReplacement;
            this.includeTrivia = includeTrivia;
        }

        public static string Stringify(SyntaxBase syntax, string? newlineReplacement = null, bool includeTrvia = true)
        {
            var stringifier = new SyntaxStringifier(newlineReplacement, includeTrvia);

            stringifier.Visit(syntax);

            return stringifier.buffer.ToString();
        }

        public override void VisitToken(Token token)
        {
            if (this.includeTrivia)
            {
                WriteTrivia(token.LeadingTrivia);
            }

            if (token.Type is TokenType.NewLine or TokenType.MultilineString &&
                !string.IsNullOrEmpty(this.newlineReplacement))
            {
                buffer.Append(token.Text.ReplaceLineEndings(this.newlineReplacement));
            }
            else
            {
                buffer.Append(token.Text);
            }

            if (this.includeTrivia)
            {
                WriteTrivia(token.TrailingTrivia);
            }
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
