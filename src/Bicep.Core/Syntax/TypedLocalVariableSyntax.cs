// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TypedLocalVariableSyntax(IdentifierSyntax name, SyntaxBase type) : SyntaxBase, INamedDeclarationSyntax
{
    public IdentifierSyntax Name { get; } = name;

    public SyntaxBase Type { get; } = type;

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypedLocalVariableSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Name, this.Type);
}
