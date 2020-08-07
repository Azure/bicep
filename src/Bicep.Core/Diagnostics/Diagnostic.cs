using Bicep.Core.Parser;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class Diagnostic : IPositionable
    {
        public Diagnostic(TextSpan span, string code, string message)
        {
            Span = span;
            Code = code;
            Message = message;
        }

        public TextSpan Span { get; }

        public string Code { get; }

        public string Message { get; }

        public Diagnostic WithSpan(TextSpan newSpan)
            => new Diagnostic(newSpan, Code, Message);
    }
}