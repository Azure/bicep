using System;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(Token unexpectedToken, DiagnosticBuilder.BuildDelegate errorFunc)
            : base()
        {
            UnexpectedToken = unexpectedToken;
            Error = errorFunc(DiagnosticBuilder.ForPosition(unexpectedToken));
        }

        public ErrorDiagnostic Error { get; }

        public override string Message => Error.Message;

        public Token UnexpectedToken { get; }
    }
}