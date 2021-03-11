// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Parsing
{
    public sealed class ExpressionTestVisitor : SyntaxVisitor
    {
        private readonly StringBuilder buffer;

        public ExpressionTestVisitor(StringBuilder buffer)
        {
            this.buffer = buffer;
        }

        public override void VisitToken(Token token) => this.buffer.Append(token.Text);

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            this.buffer.Append('(');
            base.VisitTernaryOperationSyntax(syntax);
            this.buffer.Append(')');
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            this.buffer.Append('(');
            base.VisitBinaryOperationSyntax(syntax);
            this.buffer.Append(')');
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            this.buffer.Append('(');
            base.VisitUnaryOperationSyntax(syntax);
            this.buffer.Append(')');
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            this.buffer.Append('(');
            base.VisitPropertyAccessSyntax(syntax);
            this.buffer.Append(')');
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            this.buffer.Append('(');
            base.VisitResourceAccessSyntax(syntax);
            this.buffer.Append(')');
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            this.buffer.Append('(');
            base.VisitArrayAccessSyntax(syntax);
            this.buffer.Append(')');
        }
    }
}

