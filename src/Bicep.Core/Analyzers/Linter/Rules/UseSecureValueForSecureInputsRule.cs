// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseSecureValueForSecureInputsRule : LinterRuleBase
{
    public new const string Code = "use-secure-value-for-secure-inputs";

    public UseSecureValueForSecureInputsRule() : base(
        code: Code,
        description: CoreResources.UseSecureValueForSecureInputsRule_Description,
        LinterRuleCategory.Security,
        docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"))
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseSecureValueForSecureInputsRule_MessageFormat, values);

    private static bool ExpectsSecureType(SemanticModel model, SyntaxBase syntax)
    {
        if (model.GetDeclaredType(syntax) is not { } type)
        {
            return false;
        }

        return type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);
    }

    private static IEnumerable<ObjectPropertySyntax> GetSecureObjectPropertiesFromTypeInformation(SemanticModel model)
    {
        foreach (var resource in model.DeclaredResources)
        {
            if (resource.Symbol.DeclaringResource.TryGetBody() is not { } resourceBody)
            {
                continue;
            }

            var properties = SyntaxAggregator.AggregateByType<ObjectPropertySyntax>(resourceBody)
                .Where(x => ExpectsSecureType(model, x));

            foreach (var property in properties)
            {
                yield return property;
            }
        }
    }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var property in GetSecureObjectPropertiesFromTypeInformation(model))
        {
            if (model.GetTypeInfo(property.Value) is { } type &&
                type is not ErrorType &&
                !type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    property.Value.Span,
                    [string.Join('.', property.TryGetKeyText() ?? string.Empty), type.Name]);
            }
        }
    }
}
