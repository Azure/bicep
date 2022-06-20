// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ParameterLiteralValueSyntax : SyntaxBase
    {
        public ParameterLiteralValueSyntax(Token assignmentToken, SyntaxBase defaultValue)
        {
            AssertTokenType(assignmentToken, nameof(assignmentToken), TokenType.Assignment);

            this.AssignmentToken = assignmentToken;
            this.DefaultValue = defaultValue;
        }

        public Token AssignmentToken { get; }

        public SyntaxBase DefaultValue { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitParameterLiteralValueSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.AssignmentToken, this.DefaultValue);
    }
}
