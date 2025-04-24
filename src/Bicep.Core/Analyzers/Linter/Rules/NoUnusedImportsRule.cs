// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // Does not handle detection of unused exports when using wildcard imports
    public sealed class NoUnusedImportsRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-imports";

        public NoUnusedImportsRule() : base(
            code: Code,
            description: CoreResources.ImportMustBeUsedRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticStyling: DiagnosticStyling.ShowCodeAsUnused)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.ImportMustBeUsedRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var invertedBindings = model.Binder.Bindings.ToLookup(kvp => kvp.Value, kvp => kvp.Key);

            // ImportedSymbolsListItemSyntax indicates a reference to the import
            var unreferencedImports = model.Root.ImportedSymbols
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var import in unreferencedImports)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, import.Name, import.NameSource.Span, import.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }
        }

        override protected string GetCodeFixDescription(string name)
        {
            return $"Remove import parameter {name}";
        }
    }
}
