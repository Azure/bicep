// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    // Virtual machine scale sets should set automatic OS image upgrades
    public sealed class AutomaticOSImageUpgradeRule : LinterRuleBase
    {
        public new const string Code = "automatic-os-image-upgrade";

        public AutomaticOSImageUpgradeRule() : base(
            code: Code,
            description: CoreResources.AutomaticOSImageUpgrade,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
        { }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(CoreResources.AutomaticOSImageUpgrade, values);
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            List<IDiagnostic> diagnostics = new();


            foreach (DeclaredResourceMetadata resource in model.DeclaredResources)
            {
                if (resource.TypeReference.TypeSegments[0].Equals("Microsoft.Compute") &&
                    resource.TypeReference.TypeSegments[1].Equals("virtualMachineScaleSets"))
                {
                    var resourceSyntax = resource.Symbol.DeclaringResource;
                    var enableAutomaticOSUpgradeSyntax = resourceSyntax.TryGetBody()?
                        .TryGetPropertyByNameRecursive("properties",
                        "upgradePolicy",
                        "automaticOSUpgradePolicy",
                        "enableAutomaticOSUpgrade")?.Value;
                    if (enableAutomaticOSUpgradeSyntax is not BooleanLiteralSyntax enableAutomaticOSUpgrade ||
                        enableAutomaticOSUpgrade.Value == false)
                    {
                        //var fix = new CodeFix(
                        //    "Enable automatic OS image upgrade",
                        //    true,
                        //    CodeFixKind.QuickFix,
                        //    new CodeReplacement(
                        //        )
                        diagnostics.Add(CreateDiagnosticForSpan(DiagnosticLevel.Error, resourceSyntax.Span));
                    }

                }
            }

            return diagnostics;
        }
    }
}
