// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class VariableAccessSyntax: ExpressionSyntax, ISymbolReference
    {
        public VariableAccessSyntax(IdentifierSyntax name)
        {
            this.Name = name;
        }

        public IdentifierSyntax Name { get; }


        public override void Accept(SyntaxVisitor visitor) => visitor.VisitVariableAccessSyntax(this);

        public override TextSpan Span => this.Name.Span;
    }
}

