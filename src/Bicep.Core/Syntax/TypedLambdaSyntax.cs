// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TypedLambdaSyntax : ExpressionSyntax
{
    public TypedLambdaSyntax(SyntaxBase variableSection, SyntaxBase type, SyntaxBase arrow, SyntaxBase body)
    {
        this.VariableSection = variableSection;
        this.Type = type;
        this.Arrow = arrow;
        this.Body = body;
    }

    public SyntaxBase VariableSection { get; }

    public SyntaxBase Type { get; }

    public SyntaxBase Arrow { get; }

    public SyntaxBase Body { get; }

    public IEnumerable<TypedLocalVariableSyntax> GetLocalVariables()
        => VariableSection switch {
            TypedVariableBlockSyntax vars => vars.Arguments,
            _ => Enumerable.Empty<TypedLocalVariableSyntax>(),
        };

    public override TextSpan Span => TextSpan.Between(this.VariableSection, this.Body);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypedLambdaSyntax(this);
}