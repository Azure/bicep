// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class FunctionCallSyntax : FunctionCallSyntaxBase, ISymbolReference
    {
        public FunctionCallSyntax(IdentifierSyntax name, Token openParen, IEnumerable<FunctionArgumentSyntax> arguments, Token closeParen)
            : base(name, openParen, arguments, closeParen)
        {
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(Name, CloseParen);
    }
}
