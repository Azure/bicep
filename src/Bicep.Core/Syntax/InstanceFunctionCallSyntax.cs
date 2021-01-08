// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class InstanceFunctionCallSyntax : FunctionCallSyntaxBase, ISymbolReference
    {
        public InstanceFunctionCallSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax name, Token openParen, IEnumerable<FunctionArgumentSyntax> arguments, Token closeParen)
            : base(name, openParen, arguments, closeParen)
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
