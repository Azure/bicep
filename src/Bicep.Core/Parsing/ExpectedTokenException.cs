// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.Parsing
{
    public class ExpectedTokenException : Exception
    {
        public ExpectedTokenException(Token unexpectedToken, DiagnosticBuilder.DiagnosticBuilderDelegate errorFunc)
            : base()
        {
            // To avoid the squiggly spanning multiple lines, return a 0-length span at the end of the line for a newline token.
            var errorSpan = unexpectedToken.Type == TokenType.NewLine ?
                unexpectedToken.ToZeroLengthSpan() :
                unexpectedToken.Span;

            Error = errorFunc(DiagnosticBuilder.ForPosition(errorSpan));
        }

        public Diagnostic Error { get; }

        public override string Message => Error.Message;
    }
}
