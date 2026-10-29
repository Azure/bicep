// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter.Rules;

/// <summary>
/// Flags secure values passed to module parameters that are not secure. A module's parameter values are stored in
/// clear text in its deployment's inputs, so a secret passed to an insecure parameter can be read back by anyone who
/// can read that deployment.
/// </summary>
/// <remarks>
/// Parameter types come from Bicep's type system. A <c>resourceInput&lt;&gt;</c> type carries secure flags from the
/// resource type, but compiles to a plain <c>object</c> schema whose resource-type link is metadata ARM ignores, so
/// those nested values aren't protected at deployment time. This rule treats them as secure and can miss such leaks.
/// </remarks>
public sealed class NoSecureValuesInInsecureInputsRule : LinterRuleBase
{
    public new const string Code = "no-secure-values-in-insecure-inputs";

    public NoSecureValuesInInsecureInputsRule() : base(
        code: Code,
        description: CoreResources.NoSecureValuesInInsecureInputsRule_Description,
        LinterRuleCategory.Security)
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoSecureValuesInInsecureInputsRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var analyzer = new SecureValueSources.Analyzer(model);
        foreach (var module in model.Root.ModuleDeclarations)
        {
            if (module.DeclaringModule.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) is not { Value: ObjectSyntax paramsObject })
            {
                continue;
            }

            foreach (var property in paramsObject.Properties)
            {
                if (property.TryGetKeyText() is not { } name || model.GetDeclaredType(property) is not { } sinkType)
                {
                    continue;
                }

                foreach (var (syntax, input, found) in analyzer.GetInsecureAssignments(property.Value, name, sinkType))
                {
                    yield return CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, input, found);
                }
            }
        }
    }
}
