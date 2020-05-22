namespace Bicep.Parser
{
    public class Error : IPositionable
    {
        public Error(string message, TextSpan span)
        {
            Message = message;
            Span = span;
        }

        public string Message { get; }

        public TextSpan Span { get; }
    }
}