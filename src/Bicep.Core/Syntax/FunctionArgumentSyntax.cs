// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class FunctionArgumentSyntax : SyntaxBase
    {
        public FunctionArgumentSyntax(SyntaxBase expression, Token? comma)
        {
            AssertTokenType(comma, nameof(comma), TokenType.Comma);

            this.Expression = expression;
            this.Comma = comma;
        }

        public SyntaxBase Expression { get; }

        // will be null on the last argument in a function call
        public Token? Comma { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitFunctionArgumentSyntax(this);

        public override TextSpan Span => this.Comma == null 
            ? this.Expression.Span
            : TextSpan.Between(this.Expression, this.Comma);
    }
}
