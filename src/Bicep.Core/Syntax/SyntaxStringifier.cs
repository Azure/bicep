// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class SyntaxStringifier : CstVisitor
    {
        private readonly TextWriter writer;

        private readonly string? newlineReplacement;

        private SyntaxStringifier(TextWriter writer, string? newlineReplacement)
        {
            this.writer = writer;
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

            if (token.Type is TokenType.NewLine or TokenType.StringComplete or TokenType.StringLeftPiece or TokenType.StringMiddlePiece or TokenType.StringRightPiece &&
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
                if (!string.IsNullOrEmpty(this.newlineReplacement))
                {
                    writer.Write(trivia.Text.ReplaceLineEndings(this.newlineReplacement));
                }
                else
                {
                    writer.Write(trivia.Text);
                }
            }
        }
    }
}
