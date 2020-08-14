using System;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class UnaryOperationSyntax : SyntaxBase, IExpressionSyntax
    {
        public UnaryOperationSyntax(Token operatorToken, SyntaxBase expression)
        {
            if (Operators.TokenTypeToUnaryOperator.ContainsKey(operatorToken.Type) == false)
            {
                throw new ArgumentException($"{nameof(operatorToken)} is of type '{operatorToken.Type}' which does not represent a valid unary operator.");
            }

            this.OperatorToken = operatorToken;
            this.Expression = expression;
        }

        public Token OperatorToken { get; }

        public SyntaxBase Expression { get; }

        public UnaryOperator Operator => Operators.TokenTypeToUnaryOperator[this.OperatorToken.Type];

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitUnaryOperationSyntax(this);

        public override TextSpan Span => TextSpan.Between(OperatorToken, Expression);
    }
}