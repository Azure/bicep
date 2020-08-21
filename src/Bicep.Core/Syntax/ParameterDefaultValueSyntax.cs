using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class ParameterDefaultValueSyntax : SyntaxBase
    {
        public ParameterDefaultValueSyntax(Token assignmentToken, SyntaxBase defaultValue)
        {
            AssertTokenType(assignmentToken, nameof(assignmentToken), TokenType.Assignment);

            this.AssignmentToken = assignmentToken;
            this.DefaultValue = defaultValue;
        }

        public Token AssignmentToken { get; }

        public SyntaxBase DefaultValue { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitParameterDefaultValueSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.AssignmentToken, this.DefaultValue);
    }
}