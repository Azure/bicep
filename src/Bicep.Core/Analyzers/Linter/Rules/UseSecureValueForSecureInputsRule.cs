// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseSecureValueForSecureInputsRule : LinterRuleBase
{
    public new const string Code = "use-secure-value-for-secure-inputs";

    private static readonly ImmutableDictionary<string, ImmutableArray<ImmutableArray<string>>> propertyLookup = new[] {
        ("Microsoft.Compute/virtualMachines", "properties.osProfile.adminPassword"),
        ("Microsoft.Compute/virtualMachineScaleSets", "properties.virtualMachineProfile.osProfile.adminPassword")
    }.GroupBy(x => x.Item1)
        .ToImmutableDictionary(
            x => x.Key,
            x => x.Select(y => y.Item2.Split(".").ToImmutableArray()).ToImmutableArray(),
            StringComparer.OrdinalIgnoreCase);

    public UseSecureValueForSecureInputsRule() : base(
        code: Code,
        description: CoreResources.UseSecureValueForSecureInputsRule_Description,
        LinterRuleCategory.Security,
        docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseSecureValueForSecureInputsRule_MessageFormat, values);

    private record SecureValuePropertyData(
        ObjectPropertySyntax ObjectProperty,
        ResourceTypeReference TypeReference,
        ImmutableArray<string> PropertyPath);

    private static IEnumerable<SecureValuePropertyData> GetPropertiesExpectingSecureValues(SemanticModel model)
    {
        foreach (var resource in model.DeclaredResources)
        {
            if (!resource.IsAzResource)
            {
                continue;
            }

            if (resource.Symbol.DeclaringResource.TryGetBody() is not { } resourceBody)
            {
                continue;
            }

            if (propertyLookup.TryGetValue(resource.TypeReference.Type, out var propertyPaths))
            {
                foreach (var propertyPath in propertyPaths)
                {
                    if (resourceBody.TryGetPropertyByNameRecursive(propertyPath) is ObjectPropertySyntax property)
                    {
                        yield return new(property, resource.TypeReference, propertyPath);
                        continue;
                    }
                }
            }
        }
    }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var data in GetPropertiesExpectingSecureValues(model))
        {
            if (model.GetTypeInfo(data.ObjectProperty.Value) is { } type &&
                type is not ErrorType &&
                !type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    data.ObjectProperty.Value.Span,
                    [string.Join('.', data.PropertyPath), data.TypeReference.Name]);
            }
        }
    }
}
