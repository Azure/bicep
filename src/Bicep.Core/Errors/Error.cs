using Bicep.Core.Parser;

namespace Bicep.Core.Errors
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class Error : IPositionable
    {
        public Error(TextSpan span, string errorCode, string message)
        {
            Span = span;
            ErrorCode = errorCode;
            Message = message;
        }

        public TextSpan Span { get; }

        public string ErrorCode { get; }

        public string Message { get; }

        public Error WithSpan(TextSpan newSpan)
            => new Error(newSpan, ErrorCode, Message);
    }
}