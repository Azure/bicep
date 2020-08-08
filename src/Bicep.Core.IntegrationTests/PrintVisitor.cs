using System.Collections.Generic;
using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.IntegrationTests
{
    public class PrintVisitor : SyntaxVisitor
    {
        private readonly StringBuilder buffer;

        public PrintVisitor(StringBuilder buffer)
        {
            this.buffer = buffer;
        }

        public override void VisitToken(Token token)
        {
            WriteTrivia(token.LeadingTrivia);
            buffer.Append(token.Text);
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