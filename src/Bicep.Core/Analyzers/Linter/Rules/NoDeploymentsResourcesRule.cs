// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Providers.Az;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoDeploymentsResourcesRule : LinterRuleBase
{
    public new const string Code = "no-deployments-resources";

    public NoDeploymentsResourcesRule() : base(
        code: Code,
        description: CoreResources.NoDeploymentsResourcesRuleDescription,
        LinterRuleCategory.BestPractice)
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoDeploymentsResourcesRuleMessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var resource in model.DeclaredResources.Where(r => r.IsAzResource))
        {
            if (!LanguageConstants.ResourceTypeComparer.Equals(resource.TypeReference.FormatType(), AzResourceTypeProvider.ResourceTypeDeployments))
            {
                continue;
            }

            var typeSyntax = resource.Symbol.DeclaringResource.Type;
            yield return CreateDiagnosticForSpan(diagnosticLevel, typeSyntax.Span, resource.Symbol.Name, resource.TypeReference.FormatName());
        }
    }
}
