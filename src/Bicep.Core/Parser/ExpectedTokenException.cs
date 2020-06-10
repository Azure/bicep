using System;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(string message, Token unexpectedToken)
            : base(message)
        {
            this.UnexpectedToken = unexpectedToken;
        }

        public Token UnexpectedToken { get; }
    }
}