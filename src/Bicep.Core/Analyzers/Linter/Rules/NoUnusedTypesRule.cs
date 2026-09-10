// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedTypesRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-types";

        public NoUnusedTypesRule() : base(
            code: Code,
            description: CoreResources.UnusedTypeRuleDescription,
            diagnosticStyling: DiagnosticStyling.ShowCodeAsUnused)
        { }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.UnusedTypeRuleMessageFormat, values);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var invertedBindings = model.Binder.Bindings.ToLookup(kvp => kvp.Value, kvp => kvp.Key);

            var unreferencedTypes = model.Root.TypeDeclarations
                // exported types may be referenced by other files that this single-file analysis can't see
                .Where(sym => !IsExported(model, sym.DeclaringType))
                .Where(sym => sym.NameSource.IsValid)
                // a declaration always binds to itself, so a type is unused unless something else references it
                .Where(sym => !invertedBindings[sym].Any(x => x != sym.DeclaringSyntax));

            foreach (var sym in unreferencedTypes)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(
                    diagnosticLevel, sym.Name, sym.NameSource.Span, sym.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }
        }

        protected override string GetCodeFixDescription(string name) => $"Remove unused type {name}";
    }
}
