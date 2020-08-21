using Bicep.Core.Parser;

namespace Bicep.Core.Diagnostics
{
    // roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
    public class Diagnostic : IPositionable
    {
        public Diagnostic(TextSpan span, DiagnosticLevel level, string code, string message)
        {
            Span = span;
            Level = level;
            Code = code;
            Message = message;
        }

        public TextSpan Span { get; }

        public DiagnosticLevel Level { get; }

        public string Code { get; }

        public string Message { get; }
    }
}