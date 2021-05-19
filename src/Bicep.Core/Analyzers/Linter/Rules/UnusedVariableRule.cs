// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UnusedVariableRule : LinterRuleBase
    {
        public new const string Code = "no-unused-vars";

        public UnusedVariableRule() : base(
            code: Code,
            description: CoreResources.UnusedVariableRuleDescription,
            docUri: "https://aka.ms/bicep/linter/no-unused-vars",
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // variables must have a reference of type VariableAccessSyntax
            var unreferencedVariables = model.Root.Declarations.OfType<VariableSymbol>()
                                    .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());

            foreach (var sym in unreferencedVariables)
            {
                yield return CreateDiagnosticForSpan(sym.NameSyntax.Span);
            }

            // local variables must have a reference of type VariableAccessSyntax
            var unreferencedLocalVariables = model.Root.Declarations.OfType<LocalVariableSymbol>()
                        .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any());


            foreach (var sym in unreferencedLocalVariables)
            {
                yield return CreateDiagnosticForSpan(sym.NameSyntax.Span);
            }
        }
    }
}
