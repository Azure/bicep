// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    /// <summary>
    /// Represents a local variable (such as the item or index variable in loops).
    /// </summary>
    public class LocalVariableSyntax(IdentifierSyntax name) : SyntaxBase, INamedDeclarationSyntax
    {
        public IdentifierSyntax Name { get; } = name;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitLocalVariableSyntax(this);

        public override TextSpan Span => this.Name.Span;
    }
}
