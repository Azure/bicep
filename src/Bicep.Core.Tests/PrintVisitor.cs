using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Tests
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
            buffer.Append(token.LeadingTrivia);
            buffer.Append(token.Text);
            buffer.Append(token.TrailingTrivia);
        }
    }
}