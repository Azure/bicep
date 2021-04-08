// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by expression assignment to a compile-time constant property.
    /// </summary>
    public sealed class CompileTimeConstantVisitor : SyntaxVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;

        public CompileTimeConstantVisitor(IDiagnosticWriter diagnosticWriter)
        {
            this.diagnosticWriter = diagnosticWriter;
        }

        /*
         * Overrides below correspond to nodes whose presence guarantees that
         * the expression is not a compile-time constant (assuming constant folding is not performed)
         * the only exception is the UnaryOperatorSyntax (see comment within)
         *
         * When the visitor logs an error, it should not visit child nodes as that could lead to redundant errors.
         */

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            this.AppendError(syntax);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            if(syntax.Operator == UnaryOperator.Minus)
            {
                // the unary minus operator is allowed in compile time constants
                // to support negative integer literals
                base.VisitUnaryOperationSyntax(syntax);

                return;
            }

            this.AppendError(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.AppendError(syntax);
        }

        private void AppendError(SyntaxBase syntax)
        {
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).CompileTimeConstantRequired());
        }
    }
}

