// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class DecoratorSyntax : SyntaxBase
    {
        public DecoratorSyntax(Token at, SyntaxBase expression)
        {
            AssertTokenType(at, nameof(at), TokenType.At);
            AssertSyntaxType(
                expression,
                nameof(expression),
                typeof(SkippedTriviaSyntax),
                typeof(VariableAccessSyntax),
                typeof(PropertyAccessSyntax),
                typeof(FunctionCallSyntax),
                typeof(InstanceFunctionCallSyntax));

            this.At = at;
            this.Expression = expression;
        }

        public Token At { get; }

        public SyntaxBase Expression { get; }

        public IEnumerable<FunctionArgumentSyntax> Arguments => this.Expression is FunctionCallSyntaxBase functionCall
            ? functionCall.Arguments
            : Enumerable.Empty<FunctionArgumentSyntax>();

        public override TextSpan Span => TextSpan.Between(this.At, this.Expression);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitDecoratorSyntax(this);
    }
}
