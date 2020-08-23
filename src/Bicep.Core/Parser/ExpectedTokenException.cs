using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.Parser
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(Token unexpectedToken, DiagnosticBuilder.ErrorBuilderDelegate errorFunc)
            : base()
        {
            // for errors caused by newlines, shorten the span to 1 character to avoid spilling the error over multiple lines
            // VSCode will put squiggles on the entire word at that location even for a 0-length span (coordinates in the problems view will be accurate though)
            var errorSpan = unexpectedToken.Type == TokenType.NewLine ?
                unexpectedToken.ToZeroLengthSpan() :
                unexpectedToken.Span;

            Error = errorFunc(DiagnosticBuilder.ForPosition(errorSpan));
        }

        public ErrorDiagnostic Error { get; }

        public override string Message => Error.Message;
    }
}