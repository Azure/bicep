// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoLocationExprOutsideParamsRule : LocationRuleBase
    {
        // Functions resourceGroup().location and deployment().location should only be used as the default value of a parameter.

        public new const string Code = "no-loc-expr-outside-params";

        public NoLocationExprOutsideParamsRule() : base(
            code: Code,
            description: CoreResources.NoLocExprOutsideParamsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format((string)values[0]);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            RuleVisitor visitor = new(this, GetDiagnosticLevel(model));
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private sealed class RuleVisitor : SyntaxVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly NoLocationExprOutsideParamsRule parent;
            private readonly DiagnosticLevel diagnosticLevel;

            public RuleVisitor(NoLocationExprOutsideParamsRule parent, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
            {
                // {deployment,resourceroup}().location are acceptable inside of the "location" parameter's
                //   default value, so don't traverse into the parameter's default value any further because we'll flag
                //   any other uses of those expressions.
                return;
            }

            public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            {
                if (IsCallToRgOrDeploymentLocation(syntax, out string? actualExpression))
                {
                    string msg = String.Format(
                        CoreResources.NoLocExprOutsideParamsRuleError,
                        actualExpression);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, msg));
                }

                base.VisitPropertyAccessSyntax(syntax);
            }
        }
    }
}
