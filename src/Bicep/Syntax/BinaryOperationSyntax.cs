using Bicep.Parser;

namespace Bicep.Syntax
{
    public enum BinaryOperation
    {
        // booleanOr
        Or,
        // booleanAnd
        And,
        // equality
        Equals,
        NotEquals,
        EqualsInsensitive,
        NotEqualsInsensitive,
        // comparison
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        // addition
        Add,
        Subtract,
        // multiplication
        Multiply,
        Divide,
        Modulus,
    }

    public class BinaryOperationSyntax : SyntaxBase
    {
        public BinaryOperationSyntax(SyntaxBase leftExpression, Token operatorToken, SyntaxBase rightExpression, BinaryOperation operation)
        {
            LeftExpression = leftExpression;
            OperatorToken = operatorToken;
            RightExpression = rightExpression;
            Operation = operation;
        }

        public SyntaxBase LeftExpression { get; }

        public Token OperatorToken { get; }

        public SyntaxBase RightExpression { get; }

        public BinaryOperation Operation { get; }

        public override void Accept(SyntaxVisitor visitor)
            => visitor.VisitBinaryOperationSyntax(this);

        public override TextSpan Span
            => TextSpan.Between(LeftExpression, RightExpression);
    }
}