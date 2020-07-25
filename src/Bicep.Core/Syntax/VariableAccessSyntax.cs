using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class VariableAccessSyntax: SyntaxBase
    {
        public VariableAccessSyntax(IdentifierSyntax name)
        {
            this.Name = name;
        }

        public IdentifierSyntax Name { get; }


        public override void Accept(SyntaxVisitor visitor) => visitor.VisitVariableAccessSyntax(this);

        public override TextSpan Span => this.Name.Span;
    }
}
