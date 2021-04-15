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
    internal class BCPL1070 : LinterRule
    {
        internal BCPL1070() : base(
            code: "BCPL1070",
            ruleName: "Unecessary dependsOn",
            description: "Best Practice: remove unnecessary dependsOn.",
            docUri: "https://bicep/linter/rules/BCPL1070") // TODO: setup up doc pages
        { }

        override public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel model)
        {
            var spanDiagnostics = new List<TextSpan>();
            var visitor = new BCPL1070Visitor(model, spanDiagnostics);
            visitor.Visit(model.SyntaxTree.ProgramSyntax);
            return spanDiagnostics.Select(span => CreateFixableDiagnosticForSpan(span, GetCodeFix(span)));
        }

        private CodeFix GetCodeFix(TextSpan span) =>
            new CodeFix(this.Description, true, new CodeReplacement(span, string.Empty));

        private sealed class BCPL1070Visitor : SyntaxVisitor
        {
            private readonly SemanticModel semanticModel;
            private readonly List<TextSpan> diagnostics;

            public BCPL1070Visitor(SemanticModel semanticModel, List<TextSpan> diagnostics)
            {
                this.semanticModel = semanticModel;
                this.diagnostics = diagnostics;
            }

            public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
            {
                if (syntax.NameEquals("dependsOn"))
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
                }
                base.VisitObjectPropertySyntax(syntax);
            }
        }
    }
}
