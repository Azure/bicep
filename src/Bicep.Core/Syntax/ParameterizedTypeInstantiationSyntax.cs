// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ParameterizedTypeInstantiationSyntax : ParameterizedTypeInstantiationSyntaxBase
{
    public ParameterizedTypeInstantiationSyntax(IdentifierSyntax name, Token openChevron, IEnumerable<SyntaxBase> children, Token closeChevron)
        : base(name, openChevron, children, closeChevron) { }

    public override TextSpan Span => TextSpan.Between(Name, CloseChevron);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitParameterizedTypeInstantiationSyntax(this);
}
