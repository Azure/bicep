using Bicep.Core.Parser;

namespace Bicep.Core.Errors
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class Error : IPositionable
    {
        public Error(TextSpan span, ErrorCode errorCode, string userVisibleCode, string message)
        {
            Span = span;
            ErrorCode = errorCode;
            UserVisibleCode = userVisibleCode;
            Message = message;
        }

        public TextSpan Span { get; }

        public ErrorCode ErrorCode { get; }

        public string UserVisibleCode { get; }

        public string Message { get; }

        public Error WithSpan(TextSpan newSpan)
            => new Error(newSpan, ErrorCode, UserVisibleCode, Message);
    }
}