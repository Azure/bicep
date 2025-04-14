// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a reference to a variable or parameter
    /// </summary>
    public class TypeVariableAccessSyntax : TypeSyntax, ISymbolReference
    {
        public TypeVariableAccessSyntax(IdentifierSyntax name)
        {
            this.Name = name;
        }

        public IdentifierSyntax Name { get; }


        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypeVariableAccessSyntax(this);

        public override TextSpan Span => this.Name.Span;
    }
}
