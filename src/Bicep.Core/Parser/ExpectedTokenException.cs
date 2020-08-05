using System;
using Bicep.Core.Errors;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(Token unexpectedToken, ErrorBuilder.BuildDelegate errorFunc)
            : base($"Unexpected token '{unexpectedToken.Text}'")
        {
            UnexpectedToken = unexpectedToken;
            Error = errorFunc(ErrorBuilder.ForPosition(unexpectedToken));
        }

        public Error Error { get; }

        public Token UnexpectedToken { get; }
    }
}