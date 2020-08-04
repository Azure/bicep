using Bicep.Core.Parser;

namespace Bicep.Core.Errors
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class Error : IPositionable
    {
        public TextSpan Span { get; }

        public ErrorCode Code { get; }

        private object[] formatArguments { get; }

        public Error(TextSpan span, ErrorCode code, params object[] formatArguments)
        {
            Span = span;
            Code = code;
            this.formatArguments = formatArguments;
        }

        public Error(IPositionable positionable, ErrorCode code, params object[] args)
            : this(positionable.Span, code, args)
        {
        }

        public string GetMessage()
            => ErrorFormatter.Format(Code, formatArguments);

        public Error WithSpan(TextSpan newSpan)
            => new Error(newSpan, Code, formatArguments);
    }
}