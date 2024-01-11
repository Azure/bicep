// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ObjectTypeAdditionalPropertiesAccessSyntax : TypeSyntax
{
    public ObjectTypeAdditionalPropertiesAccessSyntax(SyntaxBase baseExpression, Token dot, Token asterisk)
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

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitObjectTypeAdditionalPropertiesAccessSyntax(this);

    public override TextSpan Span => TextSpan.Between(BaseExpression, Asterisk);
}
