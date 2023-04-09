// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TypedLocalVariableSyntax : SyntaxBase, INamedDeclarationSyntax
{
    public TypedLocalVariableSyntax(SyntaxBase type, IdentifierSyntax name)
    {
        this.Type = type;
        this.Name = name;
    }

    public SyntaxBase Type { get; }

    public IdentifierSyntax Name { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypedLocalVariableSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Type, this.Name);
}