// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SimplifyJsonNullRule : LinterRuleBase
    {
        public new const string Code = "simplify-json-null";

        public SimplifyJsonNullRule() : base(
            code: Code,
            description: CoreResources.SimplifyJsonNullRuleDescription,
            LinterRuleCategory.BestPractice)
        { }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            foreach (var jsonFunction in SemanticModelHelper.GetFunctionsByName(model, SystemNamespaceType.BuiltInName, "json", model.Root.Syntax))
            {
                if (jsonFunction.Arguments.Length == 1 &&
                    jsonFunction.Arguments[0].Expression is StringSyntax argSyntax &&
                    argSyntax.TryGetLiteralValue()?.Trim() == "null")
                {
                    var codeReplacement = new CodeReplacement(jsonFunction.Span, LanguageConstants.NullKeyword);
                    var fix = new CodeFix(CoreResources.SimplifyJsonNullFixTitle, true, CodeFixKind.QuickFix, codeReplacement);

                    yield return CreateFixableDiagnosticForSpan(diagnosticLevel, jsonFunction.Span, fix);
                }
            }
        }
    }
}
