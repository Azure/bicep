// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedVariablesRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-vars";

        public NoUnusedVariablesRule() : base(
            code: Code,
            description: CoreResources.UnusedVariableRuleDescription,
            diagnosticStyling: Diagnostics.DiagnosticStyling.ShowCodeAsUnused)
        { }


        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UnusedVariableRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var invertedBindings = model.Binder.Bindings.ToLookup(kvp => kvp.Value, kvp => kvp.Key);

            // variables must have a reference of type VariableAccessSyntax
            var unreferencedVariables = model.Root.VariableDeclarations
                .Where(sym => !IsExported(model, sym.DeclaringVariable))
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var sym in unreferencedVariables)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, sym.Name, sym.NameSource.Span, sym.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }

            // TODO: This will not find local variables because they are not in the top-level scope.
            // Therefore this will not find scenarios such as a loop variable that is not used within the loop

            // local variables must have a reference of type VariableAccessSyntax
            var unreferencedLocalVariables = model.Root.Declarations.OfType<LocalVariableSymbol>()
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var sym in unreferencedLocalVariables)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, sym.Name, sym.NameSource.Span, sym.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }
        }

        override protected string GetCodeFixDescription(string name)
        {
            return $"Remove unused variable {name}";
        }
    }
}
