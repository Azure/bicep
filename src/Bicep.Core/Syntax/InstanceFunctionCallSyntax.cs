// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class InstanceFunctionCallSyntax : ExpressionSyntax, ISymbolReference
    {
        public InstanceFunctionCallSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax name, Token openParen, IEnumerable<FunctionArgumentSyntax> arguments, Token closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            AssertTokenType(closeParen, nameof(closeParen), TokenType.RightParen);
            AssertTokenType(dot, nameof(dot), TokenType.Dot);

            this.BaseExpression = baseExpression;
            this.Dot = dot;
            this.Name = name;
            this.OpenParen = openParen;
            this.Arguments = arguments.ToImmutableArray();
            this.CloseParen = closeParen;
        }

        public SyntaxBase BaseExpression { get; }

        public Token Dot { get; }

        public IdentifierSyntax Name { get; }

        public Token OpenParen { get; }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public Token CloseParen { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitInstanceFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, CloseParen);
    }
}
