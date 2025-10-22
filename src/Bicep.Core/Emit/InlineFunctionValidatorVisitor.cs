// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.Emit;

public class InlineFunctionValidatorVisitor : AstVisitor
{
    private enum ContextType
    {
        ParameterAssignment,
        VariableAssignment,
        StringInterpolation,
        Other
    }

    private readonly SemanticModel semanticModel;
    private readonly IDiagnosticWriter diagnosticWriter;
    private readonly VisitorRecorder<ContextType> contextRecorder = new();

    private InlineFunctionValidatorVisitor(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
    {
        this.semanticModel = semanticModel;
        this.diagnosticWriter = diagnosticWriter;
    }

    public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
    {
        var visitor = new InlineFunctionValidatorVisitor(semanticModel, diagnosticWriter);
        visitor.Visit(semanticModel.SourceFile.ProgramSyntax);
    }

    public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
    {
        using var _ = contextRecorder.Scope(ContextType.ParameterAssignment);
        base.VisitParameterAssignmentSyntax(syntax);
    }

    public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
    {
        using var _ = contextRecorder.Scope(ContextType.VariableAssignment);
        base.VisitVariableDeclarationSyntax(syntax);
    }

    public override void VisitStringSyntax(StringSyntax syntax)
    {
        using var _ = contextRecorder.Scope(ContextType.StringInterpolation);
        base.VisitStringSyntax(syntax);
    }

    public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
    {
        ValidateInlineFunctionUsage(syntax);
        base.VisitFunctionCallSyntax(syntax);
    }

    public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
    {
        ValidateInlineFunctionUsage(syntax);
        base.VisitInstanceFunctionCallSyntax(syntax);
    }

    private void ValidateInlineFunctionUsage(FunctionCallSyntaxBase syntax)
    {
        if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, SystemNamespaceType.BuiltInName, syntax) is not { } functionCall)
        {
            return;
        }

        if (functionCall.Name.NameEquals(LanguageConstants.InlineFunctionName))
        {
            var currentContext = contextRecorder.TryPeek(out var context) ? context : ContextType.Other;

            if (currentContext != ContextType.ParameterAssignment)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).InlineFunctionOnlyValidInParameterAssignments());
            }
        }
    }
}
