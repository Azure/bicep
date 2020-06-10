using System.Diagnostics;

namespace Bicep.Core.Parser
{
    [DebuggerDisplay("{Type} = {Text}")]
    public class Token : IPositionable
    {
        public Token(TokenType type, TextSpan span, string text, string leadingTrivia, string trailingTrivia)
        {
            Type = type;
            Span = span;
            Text = text;
            LeadingTrivia = leadingTrivia;
            TrailingTrivia = trailingTrivia;
        }

        public TokenType Type { get; }

        public TextSpan Span { get; }

        public string Text { get; }

        public string LeadingTrivia { get; }

        public string TrailingTrivia { get; }
    }
}