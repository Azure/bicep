namespace Bicep.Parser
{
    public enum TokenType
    {
        Unrecognized,
        LeftBrace,
        RightBrace,
        LeftParen,
        RightParen,
        LeftSquare,
        RightSquare,
        Comma,
        Dot,
        Question,
        Colon,
        Semicolon,
        Plus,
        Minus,
        Asterisk,
        Slash,
        Modulus,
        Exclamation,
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equals,
        NotEquals,
        EqualsInsensitive,
        NotEqualsInsensitive,
        BinaryAnd,
        BinaryOr,
        Identifier,
        String,
        Number,
        InputKeyword,
        OutputKeyword,
        VariableKeyword,
        ResourceKeyword,
        ModuleKeyword,
        TrueKeyword,
        FalseKeyword,
        NullKeyword,
        EndOfFile,
    }

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