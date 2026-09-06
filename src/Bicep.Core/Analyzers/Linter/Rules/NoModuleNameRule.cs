// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoModuleNameRule : LinterRuleBase
{
    public new const string Code = "no-module-name";

    public NoModuleNameRule() : base(
        code: Code,
        description: CoreResources.NoModuleNameRule_Description,
        LinterRuleCategory.BestPractice,
        overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off)
    { }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var module in model.Root.ModuleDeclarations)
        {
            if (module.DeclaringModule.TryGetBody() is not { } body ||
                body.TryGetPropertyByName(LanguageConstants.ModuleNamePropertyName) is not { } nameProperty)
            {
                continue;
            }

            var updatedBody = SyntaxModifier.TryRemoveProperty(body, nameProperty, model.ParsingErrorLookup);
            if (updatedBody is null)
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, nameProperty.Span);
                continue;
            }

            var codeReplacement = new CodeReplacement(body.Span, updatedBody.ToString());
            yield return CreateFixableDiagnosticForSpan(
                diagnosticLevel,
                nameProperty.Span,
                new CodeFix(CoreResources.NoModuleNameRule_CodeFix, isPreferred: true, CodeFixKind.QuickFix, codeReplacement));
        }
    }
}
