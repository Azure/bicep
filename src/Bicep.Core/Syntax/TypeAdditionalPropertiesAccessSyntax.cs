// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TypeAdditionalPropertiesAccessSyntax : TypeSyntax
{
    public TypeAdditionalPropertiesAccessSyntax(SyntaxBase baseExpression, Token dot, Token asterisk)
    {
        AssertTokenType(dot, nameof(dot), TokenType.Dot);
        AssertTokenType(asterisk, nameof(asterisk), TokenType.Asterisk);

        BaseExpression = baseExpression;
        Dot = dot;
        Asterisk = asterisk;
    }

    public SyntaxBase BaseExpression { get; }

    public Token Dot { get; }

    public Token Asterisk { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypeAdditionalPropertiesAccessSyntax(this);

    public override TextSpan Span => TextSpan.Between(BaseExpression, Asterisk);
}
