// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class InstanceParameterizedTypeInstantiationSyntax : ParameterizedTypeInstantiationSyntaxBase
{
    public InstanceParameterizedTypeInstantiationSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax name, Token openChevron, IEnumerable<SyntaxBase> children, SyntaxBase closeChevron)
        : base(name, openChevron, children, closeChevron)
    {
        AssertTokenType(dot, nameof(dot), TokenType.Dot);

        this.BaseExpression = baseExpression;
        this.Dot = dot;
    }

    public SyntaxBase BaseExpression { get; }

    public Token Dot { get; }

    public IdentifierSyntax PropertyName => Name;

    public override TextSpan Span => TextSpan.Between(BaseExpression, CloseChevron);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitInstanceParameterizedTypeInstantiationSyntax(this);
}
