// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public abstract class FunctionCallSyntaxBase : ExpressionSyntax
    {
        protected FunctionCallSyntaxBase(IdentifierSyntax name, Token openParen, IEnumerable<FunctionArgumentSyntax> arguments, Token closeParen)
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
    }
}