// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public abstract class FunctionCallSyntaxBase : ExpressionSyntax
    {
        protected FunctionCallSyntaxBase(IdentifierSyntax name, Token openParen, IEnumerable<SyntaxBase> children, Token closeParen)
        {
            AssertTokenType(openParen, nameof(openParen), TokenType.LeftParen);
            AssertTokenType(closeParen, nameof(closeParen), TokenType.RightParen);

            this.Name = name;
            this.OpenParen = openParen;
            this.Children = children.ToImmutableArray();
            this.CloseParen = closeParen;
        }

        public IdentifierSyntax Name { get; }

        public Token OpenParen { get; }

        public ImmutableArray<SyntaxBase> Children { get; }

        public IEnumerable<FunctionArgumentSyntax> Arguments => this.Children.OfType<FunctionArgumentSyntax>();

        public Token CloseParen { get; }

        public FunctionArgumentSyntax GetArgumentByPosition(int index) => Arguments.Skip(index).First();
    }
}
