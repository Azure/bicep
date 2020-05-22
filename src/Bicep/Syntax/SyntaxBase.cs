using Bicep.Parser;

namespace Bicep.Syntax
{
    public abstract class SyntaxBase : IPositionable
    {
        public abstract void Accept(SyntaxVisitor visitor);

        public abstract TextSpan Span { get; }
    }
}