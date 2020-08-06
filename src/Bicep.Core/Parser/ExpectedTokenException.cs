using System;
using Bicep.Core.Errors;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(Token unexpectedToken, ErrorBuilder.BuildDelegate errorFunc)
            : base()
        {
            UnexpectedToken = unexpectedToken;
            Error = errorFunc(ErrorBuilder.ForPosition(unexpectedToken));
        }

        public Error Error { get; }

        public override string Message => Error.Message;

        public Token UnexpectedToken { get; }
    }
}