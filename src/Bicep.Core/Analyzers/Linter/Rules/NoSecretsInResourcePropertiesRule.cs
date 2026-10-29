// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoSecretsInResourcePropertiesRule : LinterRuleBase
{
    public new const string Code = "no-secrets-in-resource-properties";

    public NoSecretsInResourcePropertiesRule() : base(
        code: Code,
        description: CoreResources.NoSecretsInResourcePropertiesRule_Description,
        LinterRuleCategory.Security,
        overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off) // Resource schemas do not consistently mark sensitive properties secure.
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoSecretsInResourcePropertiesRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var analyzer = new SecureValueSources.Analyzer(model);
        foreach (var resource in model.DeclaredResources)
        {
            if (resource.IsExistingResource ||
                !resource.Type.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resource.Type.TypeReference) ||
                resource.Symbol.DeclaringResource.TryGetBody() is not { } body)
            {
                continue;
            }

            foreach (var (syntax, property, found) in analyzer.GetInsecureAssignments(body, resource.Symbol.Name, resource.Type.Body.Type))
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, property, found);
            }
        }
    }
}
