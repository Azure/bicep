// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class FunctionArgumentSyntax : ExpressionSyntax
    {
        public FunctionArgumentSyntax(SyntaxBase expression)
        {
            this.Expression = expression;
        }

        public SyntaxBase Expression { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionArgumentSyntax(this);

        public override TextSpan Span => this.Expression.Span;
    }
}
