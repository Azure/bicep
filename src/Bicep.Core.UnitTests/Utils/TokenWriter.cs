using System.Collections.Generic;
using System.Text;
using Bicep.Core.Parser;

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
            this.buffer.Append(token.LeadingTrivia);
            this.buffer.Append(token.Text);
            this.buffer.Append(token.TrailingTrivia);
        }
    }
}
