// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedExistingResourcesRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-existing-resources";

        public NoUnusedExistingResourcesRule() : base(
            code: Code,
            description: CoreResources.UnusedExistingResourceRuleDescription,
            diagnosticStyling: Diagnostics.DiagnosticStyling.ShowCodeAsUnused)
        { }


        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UnusedExistingResourceRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var invertedBindings = model.Binder.Bindings.ToLookup(kvp => kvp.Value, kvp => kvp.Key);

            var unreferencedResources = model.Root.ResourceDeclarations
                .Where(sym => sym.NameSource.IsValid)
                .Where(sym => sym.DeclaringResource.IsExistingResource())
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax))
                .Where(sym => !(sym.DeclaringResource.TryGetBody()?.Resources ?? []).Any());
            foreach (var sym in unreferencedResources)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(diagnosticLevel, sym.Name, sym.NameSource.Span, sym.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }
        }

        override protected string GetCodeFixDescription(string name)
        {
            return $"Remove unused existing resource {name}";
        }
    }
}
