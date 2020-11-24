// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class BinaryOperationSyntax : ExpressionSyntax
    {
        public BinaryOperationSyntax(SyntaxBase leftExpression, Token operatorToken, SyntaxBase rightExpression)
        {
            if (Operators.TokenTypeToBinaryOperator.ContainsKey(operatorToken.Type) == false)
            {
                throw new ArgumentException($"{nameof(operatorToken)} is of type '{operatorToken.Type}' which does not represent a valid binary operator.");
            }

            this.LeftExpression = leftExpression;
            this.OperatorToken = operatorToken;
            this.RightExpression = rightExpression;
        }

        public SyntaxBase LeftExpression { get; }

        public Token OperatorToken { get; }

        public SyntaxBase RightExpression { get; }

        public BinaryOperator Operator => Operators.TokenTypeToBinaryOperator[this.OperatorToken.Type];

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitBinaryOperationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.LeftExpression, this.RightExpression);
    }
}
