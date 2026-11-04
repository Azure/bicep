// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoSecureValuesInInsecureInputsRule : LinterRuleBase
{
    public new const string Code = "no-secure-values-in-insecure-inputs";

    public NoSecureValuesInInsecureInputsRule() : base(
        code: Code,
        description: CoreResources.NoSecureValuesInInsecureInputsRule_Description,
        category: LinterRuleCategory.Security)
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoSecureValuesInInsecureInputsRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        => ModuleParameterSecurity.EnumerateAssignments(model).SelectMany(assignment =>
            ModuleParameterSecurity.EnumerateMismatches(model, assignment, secureInput: false)
                .Select(mismatch => CreateDiagnosticForSpan(diagnosticLevel, mismatch.Syntax.Span, mismatch.Path, mismatch.FoundMessage)));
}
