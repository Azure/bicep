// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.Analyzers.Linter.Rules;


public abstract class UseDescriptionRuleBase : LinterRuleBase
{
    protected UseDescriptionRuleBase(string code, string description) : base(
        code,
        description,
        LinterRuleCategory.BestPractice,
        overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off)
    { }
    protected readonly record struct DescriptionTarget(DecorableSyntax Decorable, string Name, TextSpan NameSpan);

    protected abstract IEnumerable<DescriptionTarget> GetTargets(SemanticModel model);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var target in GetTargets(model))
        {
            var descriptionDecorator = SemanticModelHelper.TryGetDecoratorInNamespace(
                model,
                target.Decorable,
                SystemNamespaceType.BuiltInName,
                LanguageConstants.MetadataDescriptionPropertyName);

            if (descriptionDecorator is null)
            {
                yield return CreateDiagnosticForSpan(diagnosticLevel, target.NameSpan, target.Name);
                continue;
            }

            if (DescriptionHelper.TryGetFromDecorator(model, target.Decorable) is { } description &&
                string.IsNullOrWhiteSpace(description))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    descriptionDecorator.Arguments.First().Expression.Span,
                    target.Name);
            }
        }
    }
}
