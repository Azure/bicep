using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxBase : IPositionable
    {
        public abstract void Accept(SyntaxVisitor visitor);

        public abstract TextSpan Span { get; }
    }
}