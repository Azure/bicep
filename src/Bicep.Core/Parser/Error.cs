namespace Bicep.Core.Parser
{
    public class Error : IPositionable
    {
        public Error(string message, TextSpan span)
        {
            this.Message = message;
            this.Span = span;
        }

        public Error(string message, IPositionable positionable)
        {
            this.Message = message;
            this.Span = positionable.Span;
        }

        public string Message { get; }

        public TextSpan Span { get; }
    }
}