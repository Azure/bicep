// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class OutputsShouldNotContainSecretsRule : LinterRuleBase
    {
        public new const string Code = "outputs-should-not-contain-secrets";

        public OutputsShouldNotContainSecretsRule() : base(
            code: Code,
            description: CoreResources.OutputsShouldNotContainSecretsRuleDescription,
            LinterRuleCategory.Security
        )
        {
        }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(
                CoreResources.OutputsShouldNotContainSecretsMessageFormat,
                this.Description,
                values.First());
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var visitor = new OutputVisitor(this, model, diagnosticLevel);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class OutputVisitor : AstVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public OutputVisitor(OutputsShouldNotContainSecretsRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            {
                // If the output has a secure decorator and contains secure values, don't emit a diagnostic.
                // If the output contains secure values but doesn't have a secure decorator, emit a warning diagnostic.
                if (syntax.HasSecureDecorator(model.Binder, model.TypeManager))
                {
                    return;
                }

                // Does the output name contain 'password' (suggesting it contains an actual password)?
                if (syntax.Name.IdentifierName.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsOutputName, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                }

                diagnostics.AddRange(FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(model, syntax)
                    .Select(possibleSecret => parent.CreateDiagnosticForSpan(diagnosticLevel, possibleSecret.Syntax.Span, possibleSecret.FoundMessage)));

                // Note: No need to navigate deeper, don't call base
            }
        }
    }
}
