// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class FunctionCallSyntax : FunctionCallSyntaxBase
    {
        public FunctionCallSyntax(IdentifierSyntax name, Token openParen, IEnumerable<SyntaxBase> children, Token closeParen)
            : base(name, openParen, children, closeParen)
        {
        }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitFunctionCallSyntax(this);

        public override TextSpan Span => TextSpan.Between(Name, CloseParen);
    }
}
