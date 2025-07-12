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
    public sealed class UseRecentAzPowerShellVersionRule : LinterRuleBase
    {
        public new const string Code = "use-recent-az-powershell-version";

        public UseRecentAzPowerShellVersionRule() : base(
            code: Code,
            description: CoreResources.UseRecentAzPowerShellVersionRuleDescription,
            LinterRuleCategory.BestPractice)
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.UseRecentAzPowerShellVersionRuleMessageFormat, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var minimumVersionString = (GetConfigurationValue(model.Configuration.Analyzers, "minimumVersion", "11.0") as string) ?? "11.0";
            if (!Version.TryParse(minimumVersionString, out var minimumVersion))
            {
                // If the configuration value is invalid, use the default
                minimumVersion = new Version("11.0");
            }
            var visitor = new DeploymentScriptVisitor(this, model, diagnosticLevel, minimumVersion);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.Diagnostics;
        }

        private class DeploymentScriptVisitor : AstVisitor
        {
            private readonly UseRecentAzPowerShellVersionRule rule;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;
            private readonly Version minimumVersion;
            private readonly List<IDiagnostic> diagnostics = new();

            public IEnumerable<IDiagnostic> Diagnostics => diagnostics;

            public DeploymentScriptVisitor(UseRecentAzPowerShellVersionRule rule, SemanticModel model, DiagnosticLevel diagnosticLevel, Version minimumVersion)
            {
                this.rule = rule;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
                this.minimumVersion = minimumVersion;
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                if (IsDeploymentScriptResource(syntax))
                {
                    // Check if kind property is 'AzurePowerShell'
                    var kind = GetKindProperty(syntax);
                    if (string.Equals(kind, "AzurePowerShell", StringComparison.OrdinalIgnoreCase))
                    {
                        var azPowerShellVersion = GetAzPowerShellVersion(syntax);
                        if (azPowerShellVersion != null && IsVersionBelowMinimum(azPowerShellVersion))
                        {
                            diagnostics.Add(rule.CreateDiagnosticForSpan(
                                diagnosticLevel,
                                syntax.Span,
                                azPowerShellVersion,
                                minimumVersion.ToString()));
                        }
                    }
                }
                base.VisitResourceDeclarationSyntax(syntax);
            }

            private string? GetKindProperty(ResourceDeclarationSyntax syntax)
            {
                if (syntax.Value is ObjectSyntax objectSyntax)
                {
                    var properties = objectSyntax.Properties;
                    var propertiesProperty = properties.FirstOrDefault(p =>
                        p.Key is IdentifierSyntax identifier &&
                        identifier.IdentifierName == "properties");

                    if (propertiesProperty?.Value is ObjectSyntax propertiesObject)
                    {
                        var kindProperty = propertiesObject.Properties.FirstOrDefault(p =>
                            p.Key is IdentifierSyntax identifier &&
                            string.Equals(identifier.IdentifierName, "kind", StringComparison.OrdinalIgnoreCase));

                        if (kindProperty?.Value is StringSyntax stringSyntax)
                        {
                            return stringSyntax.TryGetLiteralValue();
                        }
                    }
                }
                return null;
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
                            string.Equals(identifier.IdentifierName, "azPowerShellVersion", StringComparison.OrdinalIgnoreCase));

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
                if (Version.TryParse(version, out var parsedVersion))
                {
                    return parsedVersion < minimumVersion;
                }
                return false;
            }
        }
    }
} 