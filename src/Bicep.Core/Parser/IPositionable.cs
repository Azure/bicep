namespace Bicep.Core.Parser
{
    public interface IPositionable
    {
        TextSpan Span { get; }
    }

    public class TextSpan
    {
        public TextSpan(int position, int length) 
        {
            Position = position;
            Length = length;
        }

        public int Position { get; }

        public int Length { get; }

        public override string ToString()
            => $"[{Position}:{Position + Length}]";

        public static TextSpan Between(IPositionable a, IPositionable b)
            => new TextSpan(a.Span.Position, b.Span.Position + b.Span.Length - a.Span.Position);

        public static TextSpan BetweenNonInclusive(IPositionable a, IPositionable b)
            => new TextSpan(a.Span.Position + a.Span.Length, b.Span.Position);
    }
}