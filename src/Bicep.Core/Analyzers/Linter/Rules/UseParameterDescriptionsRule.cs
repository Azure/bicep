// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseParameterDescriptionsRule : LinterRuleBase
{
    public new const string Code = "use-parameter-descriptions";

    public UseParameterDescriptionsRule() : base(
        code: Code,
        description: CoreResources.UseParameterDescriptionsRuleDescription,
        LinterRuleCategory.BestPractice)
    { }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseParameterDescriptionsRuleMessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var parameter in model.Root.ParameterDeclarations.Where(parameter => parameter.NameSource.IsValid))
        {
            var descriptionDecorator = parameter.TryGetDecorator(
                model,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.MetadataDescriptionPropertyName);

            if (descriptionDecorator is null)
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, parameter.NameSource.Span, parameter.Name);
                continue;
            }

            if (DescriptionHelper.TryGetFromDecorator(model, parameter.DeclaringParameter) is { } description &&
                string.IsNullOrWhiteSpace(description))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    descriptionDecorator.Arguments.First().Expression.Span,
                    parameter.Name);
            }
        }
    }
}
