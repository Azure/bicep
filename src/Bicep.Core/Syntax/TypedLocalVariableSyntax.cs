// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TypedLocalVariableSyntax : SyntaxBase, INamedDeclarationSyntax
{
    public TypedLocalVariableSyntax(IdentifierSyntax name, SyntaxBase type)
    {
        this.Name = name;
        this.Type = type;
    }

    public IdentifierSyntax Name { get; }

    public SyntaxBase Type { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypedLocalVariableSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Name, this.Type);
}
