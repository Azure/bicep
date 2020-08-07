using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by expression assignment to a compile-time constant property.
    /// </summary>
    public sealed class CompileTimeConstantVisitor : SyntaxVisitor
    {
        private readonly IList<Diagnostic> errors;

        public CompileTimeConstantVisitor(IList<Diagnostic> errors)
        {
            this.errors = errors;
        }

        /*
         * Overrides below correspond to nodes whose presence guarantees that
         * the expression is not a compile-time constant (assuming constant folding is not performed)
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
            this.AppendError(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.AppendError(syntax);
        }

        private void AppendError(SyntaxBase syntax)
        {
            this.errors.Add(DiagnosticBuilder.ForPosition(syntax).CompileTimeConstantRequired());
        }
    }
}
