// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedVariablesRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-vars";

        public NoUnusedVariablesRule() : base(
            "variable",
            code: Code,
            description: CoreResources.UnusedVariableRuleDescription,
            
            diagnosticStyling: Diagnostics.DiagnosticStyling.ShowCodeAsUnused, docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }


        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UnusedVariableRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // TODO: Performance: Use a visitor to visit VariableAccesssyntax and collects the non-error symbols into a list.
            // Then do a symbol visitor to go through all the symbols that exist and compare.
            // Same issue for unused-params rule.

            // variables must have a reference of type VariableAccessSyntax
            var unreferencedVariables = model.Root.Declarations.OfType<VariableSymbol>()
                .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any())
                .Where(sym => sym.Name != MissingName);
            foreach (var sym in unreferencedVariables)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(sym.Name, sym.NameSyntax, sym.DeclaringSyntax, model.SourceFile.LineStarts, model.SourceFile.ProgramSyntax);
            }

            // TODO: This will not find local variables because they are not in the top-level scope.
            // Therefore this will not find scenarios such as a loop variable that is not used within the loop

            // local variables must have a reference of type VariableAccessSyntax
            var unreferencedLocalVariables = model.Root.Declarations.OfType<LocalVariableSymbol>()
                        .Where(sym => !model.FindReferences(sym).OfType<VariableAccessSyntax>().Any())
                        .Where(sym => sym.Name != MissingName);

            foreach (var sym in unreferencedLocalVariables)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(sym.Name, sym.NameSyntax, sym.DeclaringSyntax, model.SourceFile.LineStarts, model.SourceFile.ProgramSyntax);
            }
        }
    }
}
