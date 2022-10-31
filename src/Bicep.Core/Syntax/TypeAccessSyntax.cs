// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

/// <summary>
/// Represents a reference to a declared type
/// </summary>
public class TypeAccessSyntax : TypeSyntax, ISymbolReference
{
    public TypeAccessSyntax(IdentifierSyntax name)
    {
        this.Name = name;
    }

    public IdentifierSyntax Name { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypeAccessSyntax(this);

    public override TextSpan Span => this.Name.Span;
}
