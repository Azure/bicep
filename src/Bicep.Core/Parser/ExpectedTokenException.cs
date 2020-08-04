using System;
using Bicep.Core.Errors;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(Token unexpectedToken, ErrorCode errorCode, params object[] formatArguments)
            : base($"Unexpected token '{unexpectedToken.Text}'")
        {
            UnexpectedToken = unexpectedToken;
            Error = new Error(unexpectedToken, errorCode, formatArguments);
        }

        public Error Error { get; }

        public Token UnexpectedToken { get; }
    }
}