// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoUnusedResourcesRule : NoUnusedRuleBase
    {
        public new const string Code = "no-unused-resources";

        public NoUnusedResourcesRule() : base(
            code: Code,
            description: CoreResources.UnusedResourceRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
            diagnosticStyling: Diagnostics.DiagnosticStyling.ShowCodeAsUnused)
        { }


        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UnusedResourceRuleMessageFormat, values);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            // TODO: Performance: Use a visitor to visit VariableAccesssyntax and collects the non-error symbols into a list.
            // Then do a symbol visitor to go through all the symbols that exist and compare.
            // Same issue for unused-params and unused-variables rule.

            var unreferencedResources = model.Root.Declarations.OfType<ResourceSymbol>()
                .Where(sym => sym.NameSyntax.IsValid)
                .Where(sym => sym.DeclaringResource.IsExistingResource())
                .Where(sym => !model.FindReferences(sym).Any(rf => rf != sym.DeclaringResource));
            foreach (var sym in unreferencedResources)
            {
                yield return CreateRemoveUnusedDiagnosticForSpan(sym.Name, sym.NameSyntax, sym.DeclaringSyntax, model.SourceFile.ProgramSyntax);
            }
        }

        override protected string GetCodeFixDescription(string name)
        {
            return $"Remove unused resource {name}";
        }
    }
}
