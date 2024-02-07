// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class TypedLambdaSyntax(SyntaxBase variableSection, SyntaxBase returnType, SyntaxBase arrow, ImmutableArray<Token> newlinesBeforeBody, SyntaxBase body) : ExpressionSyntax
{
    public SyntaxBase VariableSection { get; } = variableSection;

    public SyntaxBase ReturnType { get; } = returnType;

    public SyntaxBase Arrow { get; } = arrow;

    public ImmutableArray<Token> NewlinesBeforeBody { get; } = newlinesBeforeBody;

    public SyntaxBase Body { get; } = body;

    public IEnumerable<TypedLocalVariableSyntax> GetLocalVariables()
        => VariableSection switch
        {
            TypedVariableBlockSyntax vars => vars.Arguments,
            _ => Enumerable.Empty<TypedLocalVariableSyntax>(),
        };

    public override TextSpan Span => TextSpan.Between(this.VariableSection, this.Body);

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypedLambdaSyntax(this);
}
