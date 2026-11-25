// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
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
        LinterRuleCategory.Security)
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
        foreach (var assignment in SecureAssignmentChecker.EnumerateModuleParameterAssignments(model))
        {
            foreach (var mismatch in SecureAssignmentChecker.EnumerateInsecureValuesForSecureParameters(model, assignment))
            {
                var diagnostic = CreateDiagnostic(mismatch.Syntax.Span,
                    string.Format(CoreResources.UseSecureValueForSecureInputsRule_ModuleParameterMessageFormat, mismatch.Path)) with
                { Level = diagnosticLevel };
                yield return ReferenceEquals(mismatch.Syntax, assignment.Value)
                    ? WithSecureParameterFix(model, assignment.Value, diagnostic)
                    : diagnostic;
            }
        }

        var resourceTypeResolver = ResourceTypeResolver.Create(model);
        foreach (var property in GetSecureObjectPropertiesFromTypeInformation(model))
        {
            if (!DeployTimeConstantValidator.IsDeployTimeConstant(property, property.Value, model, resourceTypeResolver))
            {
                // Let's ignore the case where insecure runtime values are used - this is complicated due to lack of accurate type information.
                // The main thing we are trying to block is hard-coded values, insecure parameters, and value calculated from these.
                continue;
            }

            if (model.GetTypeInfo(property.Value) is { } type && IsPotentiallyInsecure(type))
            {
                var diagnostic = CreateDiagnosticForSpan(
                    diagnosticLevel,
                    property.Value.Span,
                    [string.Join('.', property.TryGetKeyText() ?? string.Empty), type.Name]);
                yield return WithSecureParameterFix(model, property.Value, diagnostic);
            }
        }
    }

    private static Diagnostic WithSecureParameterFix(SemanticModel model, SyntaxBase value, Diagnostic diagnostic)
    {
        if (value is not VariableAccessSyntax ||
            model.GetSymbolInfo(value) is not ParameterSymbol
            {
                DeclaringParameter: { Type: TypeVariableAccessSyntax typeSyntax, Modifier: null }
            } parameter ||
            model.GetSymbolInfo(typeSyntax) is not AmbientTypeSymbol { Name: LanguageConstants.TypeNameString or LanguageConstants.ObjectType } ||
            parameter.IsSecure())
        {
            return diagnostic;
        }

        var replacement = DecoratorCodeFixProvider.CreateCodeReplacement(model, parameter.DeclaringParameter, SyntaxFactory.CreateDecorator("secure"));
        return diagnostic with
        {
            Fixes = [new CodeFix($"Mark parameter '{parameter.Name}' as secure", isPreferred: false, CodeFixKind.QuickFix, replacement)]
        };
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
}
