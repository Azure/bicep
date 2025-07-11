// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public class StacksExtensibilityCompatibilityRule : LinterRuleBase
    {
        public new const string Code = "stacks-extensibility-compat";

        public StacksExtensibilityCompatibilityRule() : base(
            code: Code,
            description: CoreResources.StacksExtensibilityCompatibilityRuleDescription,
            category: LinterRuleCategory.PotentialCodeIssues,
            overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Info)
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.StacksExtensibilityCompatibilityRuleMessageFormat, values);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (!model.Features.ModuleExtensionConfigsEnabled) // This rule is being released with this experimental flag.
            {
                return [];
            }

            var visitor = new RuleVisitor(this, model, diagnosticLevel);

            visitor.Visit(model.SourceFile.ProgramSyntax);

            return visitor.Diagnostics;
        }

        private sealed class RuleVisitor : AstVisitor
        {
            public List<IDiagnostic> Diagnostics { get; } = new();

            private StacksExtensibilityCompatibilityRule Rule { get; }

            private SemanticModel Model { get; }

            private DiagnosticLevel DiagnosticLevel { get; }

            public RuleVisitor(StacksExtensibilityCompatibilityRule rule, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                Rule = rule;
                Model = model;
                DiagnosticLevel = diagnosticLevel;
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                base.VisitModuleDeclarationSyntax(syntax);
                // TODO(kylealbert): Track when we're in extensionConfigs: { [extAlias]: { [configKey]: [value] } }. [value] must be a keyvault reference for secure properties. [value] must not be a reference for non-secure properties.
            }

            public override void VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax)
            {
                base.VisitExtensionWithClauseSyntax(syntax);
                // TODO(kylealbert): [value] must be a keyvault reference for secure properties. [value] must not be a reference for non-secure properties.
            }
        }
    }
}
