using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parser
{
    [DebuggerDisplay("{Type} = {Text}")]
    public class Token : IPositionable
    {
        public Token(TokenType type, TextSpan span, string text, ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia)
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

        public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }

        public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }
    }
}