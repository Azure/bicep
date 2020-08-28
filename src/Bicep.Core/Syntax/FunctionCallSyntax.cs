// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class FunctionCallSyntax : SyntaxBase, IExpressionSyntax, ISymbolReference
    {
        public FunctionCallSyntax(IdentifierSyntax name, Token openParen, IEnumerable<FunctionArgumentSyntax> arguments, Token closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            AssertTokenType(closeParen, nameof(closeParen), TokenType.RightParen);

            this.Name = name;
            this.OpenParen = openParen;
            this.Arguments = arguments.ToImmutableArray();
            this.CloseParen = closeParen;
        }

        public IdentifierSyntax Name { get; }

        public Token OpenParen { get; }

        public ImmutableArray<FunctionArgumentSyntax> Arguments { get; }

        public Token CloseParen { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(Name, CloseParen);

        public ExpressionKind ExpressionKind => ExpressionKind.Operator;
    }
}
