// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class LambdaSyntax : ExpressionSyntax
    {
        public LambdaSyntax(SyntaxBase variableSection, Token arrow, ImmutableArray<Token> newlinesBeforeBody, SyntaxBase body)
        {
            AssertTokenType(arrow, nameof(arrow), TokenType.Arrow);

            this.VariableSection = variableSection;
            this.Arrow = arrow;
            this.NewlinesBeforeBody = newlinesBeforeBody;
            this.Body = body;
        }

        public SyntaxBase VariableSection { get; }

        public Token Arrow { get; }

        public ImmutableArray<Token> NewlinesBeforeBody { get; }

        public SyntaxBase Body { get; }

        public ImmutableArray<LocalVariableSyntax> GetLocalVariables()
            => VariableSection switch
            {
                LocalVariableSyntax var => [var],
                VariableBlockSyntax vars => vars.Arguments,
                _ => [],
            };

        public override TextSpan Span => TextSpan.Between(this.VariableSection, this.Body);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitLambdaSyntax(this);
    }
}
