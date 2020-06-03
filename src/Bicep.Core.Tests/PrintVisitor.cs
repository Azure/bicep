using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Visitors;

namespace Bicep.Core.Tests
{
    public class PrintVisitor : TokenVisitor
    {
        private readonly StringBuilder buffer;

        public PrintVisitor(StringBuilder buffer)
        {
            this.buffer = buffer;
        }

        protected override void VisitToken(Token token)
        {
            buffer.Append(token.LeadingTrivia);
            buffer.Append(token.Text);
            buffer.Append(token.TrailingTrivia);
        }
    }
}