// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrintV2
{
    public class SyntaxStringifier : CstVisitor
    {
        private readonly TextWriter writer;

        private readonly string? newlineReplacement;

        private SyntaxStringifier(TextWriter buffer, string? newlineReplacement)
        {
            this.writer = buffer;
            this.newlineReplacement = newlineReplacement;
        }

        public static string Stringify(SyntaxBase syntax, string? newlineReplacement = null)
        {
            var writer = new StringWriter();
            var stringifier = new SyntaxStringifier(writer, newlineReplacement);

            stringifier.Visit(syntax);

            return writer.ToString();
        }

        public static void StringifyTo(TextWriter writer, SyntaxBase syntax, string? newlineReplacement = null)
        {
            var stringifier = new SyntaxStringifier(writer, newlineReplacement);

            stringifier.Visit(syntax);
        }

        public override void VisitToken(Token token)
        {
            WriteTrivia(token.LeadingTrivia);

            if (token.Type is TokenType.NewLine or TokenType.MultilineString &&
                !string.IsNullOrEmpty(this.newlineReplacement))
            {
                writer.Write(token.Text.ReplaceLineEndings(this.newlineReplacement));
            }
            else
            {
                writer.Write(token.Text);
            }

            WriteTrivia(token.TrailingTrivia);
        }

        private void WriteTrivia(IEnumerable<SyntaxTrivia> triviaList)
        {
            foreach (var trivia in triviaList)
            {
                writer.Write(trivia.Text);
            }
        }
    }
}
