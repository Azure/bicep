// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using System.Globalization;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class UseLatestAzPowerShellVersionRule : LinterRuleBase
    {
        public new const string Code = "use-latest-az-powershell-version";
        private const string MinimumAzPowerShellVersion = "11.0";

        public UseLatestAzPowerShellVersionRule() : base(
            code: Code,
            description: CoreResources.UseLatestAzPowerShellVersionRuleDescription,
            LinterRuleCategory.BestPractice)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UseLatestAzPowerShellVersionRuleMessageFormat, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var visitor = new DeploymentScriptVisitor(this, model, diagnosticLevel);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.Diagnostics;
        }

        private class DeploymentScriptVisitor : AstVisitor
        {
            private readonly UseLatestAzPowerShellVersionRule rule;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;
            private readonly List<IDiagnostic> diagnostics = new();

            public IEnumerable<IDiagnostic> Diagnostics => diagnostics;

            public DeploymentScriptVisitor(UseLatestAzPowerShellVersionRule rule, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.rule = rule;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                if (IsDeploymentScriptResource(syntax))
                {
                    var azPowerShellVersion = GetAzPowerShellVersion(syntax);
                    if (azPowerShellVersion != null && IsVersionBelowMinimum(azPowerShellVersion))
                    {
                        diagnostics.Add(rule.CreateDiagnosticForSpan(
                            diagnosticLevel,
                            syntax.Span,
                            azPowerShellVersion,
                            MinimumAzPowerShellVersion));
                    }
                }
                base.VisitResourceDeclarationSyntax(syntax);
            }

            private bool IsDeploymentScriptResource(ResourceDeclarationSyntax syntax)
            {
                return syntax.TypeString?.TryGetLiteralValue()?.Contains("Microsoft.Resources/deploymentScripts") == true;
            }

            private string? GetAzPowerShellVersion(ResourceDeclarationSyntax syntax)
            {
                if (syntax.Value is ObjectSyntax objectSyntax)
                {
                    var properties = objectSyntax.Properties;
                    var propertiesProperty = properties.FirstOrDefault(p =>
                        p.Key is IdentifierSyntax identifier &&
                        identifier.IdentifierName == "properties");

                    if (propertiesProperty?.Value is ObjectSyntax propertiesObject)
                    {
                        var azPowerShellVersionProperty = propertiesObject.Properties.FirstOrDefault(p =>
                            p.Key is IdentifierSyntax identifier &&
                            identifier.IdentifierName == "azPowerShellVersion");

                        if (azPowerShellVersionProperty?.Value is StringSyntax stringSyntax)
                        {
                            return stringSyntax.TryGetLiteralValue();
                        }
                    }
                }
                return null;
            }

            private bool IsVersionBelowMinimum(string version)
            {
                if (string.IsNullOrWhiteSpace(version))
                {
                    return false;
                }
                if (Version.TryParse(version, out var parsedVersion) &&
                    Version.TryParse(MinimumAzPowerShellVersion, out var minimumVersion))
                {
                    return parsedVersion < minimumVersion;
                }
                return false;
            }
        }
    }
} 