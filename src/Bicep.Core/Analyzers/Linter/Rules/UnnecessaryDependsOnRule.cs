// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UnnecessaryDependsOnRule : LinterRuleBase
    {
        public new const string Code = "no-unnecessary-dependson";

        public UnnecessaryDependsOnRule() : base(
            code: Code,
            description: CoreResources.UnnecessaryDependsOnRuleDescription,
            docUri: "https://aka.ms/bicep/linter/no-unnecessary-dependson",
            diagnosticLabel: Diagnostics.DiagnosticLabel.Unnecessary)
        { }

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spanDiagnostics = new List<TextSpan>();

            var allResources = model.Root.GetAllResourceDeclarations();
            var visitor = new dependsOnPropertyVisitor(model, spanDiagnostics);
            foreach (var resource in allResources)
            {
                var dependsOnProperty = resource.SafeGetBodyPropertyValue(LanguageConstants.ResourceLocationPropertyName);
                if (dependsOnProperty != null)
                {
                    visitor.Visit(dependsOnProperty);
                }
            }

            return spanDiagnostics.Select(span => CreateDiagnosticForSpan(span));
        }

        private sealed class dependsOnPropertyVisitor : SyntaxVisitor
        {
            private readonly SemanticModel semanticModel;
            private readonly List<TextSpan> diagnostics;

            public dependsOnPropertyVisitor(SemanticModel semanticModel, List<TextSpan> diagnostics)
            {
                this.semanticModel = semanticModel;
                this.diagnostics = diagnostics;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                if (syntax.Value is ArraySyntax dependencies)
                {
                    var inferredDependencies = ResourceDependencyFinderVisitor.GetResourceDependencies(this.semanticModel, syntax);
                    foreach (var dependency in dependencies.Items)
                    {
                        if (dependency.Value is VariableAccessSyntax item
                            && inferredDependencies.Any(d => d.DeclaringSyntax is ResourceDeclarationSyntax resource
                               && item.ReferencesResource(resource)))
                        {
                            this.diagnostics.Add(dependency.Span);
                        }
                    }
                }

                base.VisitObjectPropertySyntax(syntax);
            }
        }
    }
}
