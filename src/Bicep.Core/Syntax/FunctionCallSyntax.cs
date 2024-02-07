// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class FunctionCallSyntax(IdentifierSyntax name, Token openParen, IEnumerable<SyntaxBase> children, Token closeParen) : FunctionCallSyntaxBase(name, openParen, children, closeParen), ISymbolReference
    {
        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(Name, CloseParen);
    }
}
