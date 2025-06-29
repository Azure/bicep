// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class InstanceFunctionCallSyntax : FunctionCallSyntaxBase
    {
        public InstanceFunctionCallSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax name, Token openParen, IEnumerable<SyntaxBase> children, SyntaxBase closeParen)
            : base(name, openParen, children, closeParen)
        {
            AssertTokenType(dot, nameof(dot), TokenType.Dot);

            this.BaseExpression = baseExpression;
            this.Dot = dot;
        }

        public SyntaxBase BaseExpression { get; }

        public Token Dot { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitInstanceFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, CloseParen);
    }
}
