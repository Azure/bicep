// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class StrictModeValidationRule : LinterRuleBase
{
    public new const string Code = "strict-mode";

    public StrictModeValidationRule() : base(
        code: Code,
        description: CoreResources.StrictMode_Description,
        docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
        diagnosticLevel: DiagnosticLevel.Warning)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.StrictMode_MessageFormat, values);

    record ParameterInfo(
        bool IsCalculatedFromRuntimeValues,
        bool HasDefaultRuntimeValue);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        // TODO flag all usage of reference() as unsafe

        foreach (var module in model.Root.ModuleDeclarations)
        {
            if (!module.TryGetSemanticModel(out var moduleGenericModel, out _) ||
                moduleGenericModel is not SemanticModel moduleModel)
            {
                continue;
            }

            var moduleSm = moduleModel.StrictModeAnalysis;
            var fileSm = model.StrictModeAnalysis;
            var paramsObject = module.TryGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName) as ObjectSyntax;

            foreach (var param in moduleModel.Root.ParameterDeclarations)
            {
                if (!moduleSm.RuntimeParameterUsages.Contains(param))
                {
                    // doesn't matter whether or not it's set to a runtime value
                    continue;
                }

                if (paramsObject?.TryGetPropertyByName(param.Name) is {} paramProperty)
                {
                    var references = StrictModeAnalyzer.ReferencesFinder.Analyze(model, paramProperty.Value);

                    if (references.ModuleRuntimeDependencies.Any() || references.ResourceRuntimeDependencies.Any())
                    {
                        yield return this.CreateDiagnosticForSpan(diagnosticLevel, paramProperty.Value.Span, module.Name, param.Name);
                    }
                }
            }
        }
    }
}