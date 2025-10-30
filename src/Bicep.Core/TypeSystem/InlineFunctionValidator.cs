// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// InlineFunctionValidator is used to validate that the inline() function is only used in valid contexts.
    /// The inline() function is only allowed in parameter assignments in .bicepparam files,
    /// and is not permitted in variables, string interpolation, or decorators.
    /// </summary>
    public sealed class InlineFunctionValidator : AstVisitor
    {
        private enum ValidationContext
        {
            ParameterValue,
            VariableValue,
            StringInterpolation,
            Other
        }

        private readonly SemanticModel semanticModel;
        private readonly IDiagnosticWriter diagnosticWriter;
        private ValidationContext currentContext = ValidationContext.Other;
        private string? currentParameterName;
        private string? currentPropertyName;

        private InlineFunctionValidator(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            this.semanticModel = semanticModel;
            this.diagnosticWriter = diagnosticWriter;
        }

        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            if (semanticModel.SourceFile is not BicepParamFile)
            {
                return;
            }

            var visitor = new InlineFunctionValidator(semanticModel, diagnosticWriter);
            visitor.Visit(semanticModel.SourceFile.ProgramSyntax);
        }

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            var previousContext = currentContext;
            var previousParameterName = currentParameterName;

            currentContext = ValidationContext.ParameterValue;
            currentParameterName = syntax.Name.IdentifierName;

            base.VisitParameterAssignmentSyntax(syntax);

            currentContext = previousContext;
            currentParameterName = previousParameterName;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            var previousContext = currentContext;
            currentContext = ValidationContext.VariableValue;

            base.VisitVariableDeclarationSyntax(syntax);

            currentContext = previousContext;
        }

        public override void VisitStringSyntax(StringSyntax syntax)
        {
            if (syntax.IsInterpolated())
            {
                var previousContext = currentContext;
                currentContext = ValidationContext.StringInterpolation;

                foreach (var expression in syntax.Expressions)
                {
                    Visit(expression);
                }

                currentContext = previousContext;
            }
            else
            {
                base.VisitStringSyntax(syntax);
            }
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var previousPropertyName = currentPropertyName;

            // Extract property name from the key
            currentPropertyName = syntax.Key switch
            {
                StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is string literal => literal,
                IdentifierSyntax identifier => identifier.IdentifierName,
                _ => null
            };

            base.VisitObjectPropertySyntax(syntax);

            currentPropertyName = previousPropertyName;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            if (syntax.Name.IdentifierName == LanguageConstants.InlineKeyword)
            {
                ValidateInlineUsage(syntax);
            }

            base.VisitFunctionCallSyntax(syntax);
        }

        private void ValidateInlineUsage(FunctionCallSyntax syntax)
        {
            switch (currentContext)
            {
                case ValidationContext.VariableValue:
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InlineFunctionNotValidInVariables());
                    break;

                case ValidationContext.StringInterpolation:
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InlineFunctionNotValidInStringInterpolation());
                    break;

                case ValidationContext.ParameterValue:
                    var overrideKey = currentPropertyName ?? currentParameterName ?? "parameter";
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).MissingInlineParameterOverride(overrideKey));
                    break;

                case ValidationContext.Other:
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InlineFunctionNotValidInVariables());
                    break;
            }
        }
    }
}
