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
using Bicep.Core.TypeSystem.Providers;
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
        var resourceTypeResolver = ResourceTypeResolver.Create(model);
        foreach (var property in GetSecureObjectPropertiesFromTypeInformation(model))
        {
            if (!IsDeployTimeConstant(property, model, resourceTypeResolver))
            {
                // Let's ignore the case where insecure runtime values are used - this is complicated due to lack of accurate type information.
                // The main thing we are trying to block is hard-coded values, insecure parameters, and value calculated from these.
                continue;
            }

            if (model.GetTypeInfo(property.Value) is { } type && IsPotentiallyInsecure(type))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    property.Value.Span,
                    [string.Join('.', property.TryGetKeyText() ?? string.Empty), type.Name]);
            }
        }
    }

    private static bool IsPotentiallyInsecure(TypeSymbol type)
        => type switch
        {
            ErrorType => false,
            NullType => false,
            StringLiteralType { RawStringValue: "" } => false,
            UnionType unionType => unionType.Members.Any(x => IsPotentiallyInsecure(x.Type)),
            _ => !type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
        };

    private static bool IsDeployTimeConstant(ObjectPropertySyntax syntax, SemanticModel model, ResourceTypeResolver resolver)
    {
        var diagWriter = ToListDiagnosticWriter.Create();
        DeployTimeConstantValidator.CheckDeployTimeConstantViolations(syntax, syntax.Value, model, diagWriter, resolver);

        return diagWriter.GetDiagnostics().Count == 0;
    }
}
