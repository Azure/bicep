// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class LambdaSyntax : ExpressionSyntax
    {
        public LambdaSyntax(SyntaxBase variableSection, Token arrow, SyntaxBase body)
        {
            AssertTokenType(arrow, nameof(arrow), TokenType.Arrow);

            this.VariableSection = variableSection;
            this.Arrow = arrow;
            this.Body = body;
        }

        public SyntaxBase VariableSection { get; }

        public Token Arrow { get; }

        public SyntaxBase Body { get; }

        public IEnumerable<LocalVariableSyntax> GetLocalVariables()
            => VariableSection switch {
                LocalVariableSyntax var => var.AsEnumerable(),
                VariableBlockSyntax vars => vars.Arguments,
                _ => Enumerable.Empty<LocalVariableSyntax>(),
            };

        public override TextSpan Span => TextSpan.Between(this.VariableSection, this.Body);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitLambdaSyntax(this);
    }
}
