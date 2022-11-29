// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class IntegerValidatorVisitor : AstVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;
        private readonly SemanticModel semanticModel;

        private IntegerValidatorVisitor(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            this.diagnosticWriter = diagnosticWriter;
            this.semanticModel = semanticModel;
        }

        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new IntegerValidatorVisitor(semanticModel, diagnosticWriter);
            // visiting writes diagnostics in some cases
            visitor.Visit(semanticModel.SourceFile.ProgramSyntax);
        }

        public override void VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
        {
            // syntax.Value is always positive and can't be greater than the greatest 64 bit integer
            if (syntax.Value > long.MaxValue)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InvalidInteger());
            }
            base.VisitIntegerLiteralSyntax(syntax);
        }

        public override void VisitUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            // a negative integer is parsed into a minus token and an integer token which is always positive
            // so for the most negative valid signed 64 bit integer -9,223,372,036,854,775,808, we need to compare its positive integer token (9,223,372,036,854,775,808) to long.MaxValue (9,223,372,036,854,775,807) + 1
            if (syntax.Operator == UnaryOperator.Minus && syntax.Expression is IntegerLiteralSyntax integerLiteral)
            {
                if (integerLiteral.Value > (ulong)long.MaxValue + 1)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InvalidInteger());
                }
            }
            else
            {
                base.VisitUnaryOperationSyntax(syntax);
            }
        }
    }
}
