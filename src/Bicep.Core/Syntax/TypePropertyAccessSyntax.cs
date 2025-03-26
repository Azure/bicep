// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TypePropertyAccessSyntax : TypeSyntax
{
    public TypePropertyAccessSyntax(SyntaxBase baseExpression, Token dot, IdentifierSyntax propertyName)
    {
        AssertTokenType(dot, nameof(dot), TokenType.Dot);

        BaseExpression = baseExpression;
        Dot = dot;
        PropertyName = propertyName;
    }

    public SyntaxBase BaseExpression { get; }

    public Token Dot { get; }

    public IdentifierSyntax PropertyName { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypePropertyAccessSyntax(this);

    public override TextSpan Span => TextSpan.Between(BaseExpression, PropertyName);
}
