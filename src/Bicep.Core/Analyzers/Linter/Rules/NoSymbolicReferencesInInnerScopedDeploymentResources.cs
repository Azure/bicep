// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoSymbolicReferencesInInnerScopedDeploymentResources : LinterRuleBase
    {
        public new const string Code = "nested-deployment-template-scoping";

        public NoSymbolicReferencesInInnerScopedDeploymentResources() : base(
            code: Code,
            description: CoreResources.NoSymbolicReferencesInInnerScopedDeploymentResourcesDescription,
            LinterRuleCategory.DeploymentError)
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format(CoreResources.NoSymbolicReferencesInInnerScopedDeploymentResourcesMessageFormat, values);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var innerScopedEvaluationIsDefault = model.EmitterSettings.EnableSymbolicNames;
            var topLevelSymbols = model.Root.Declarations.ToHashSet();

            foreach (var resource in model.DeclaredResources.Where(r => r.IsAzResource))
            {
                if (!LanguageConstants.ResourceTypeComparer.Equals(resource.TypeReference.FormatType(), AzResourceTypeProvider.ResourceTypeDeployments) ||
                    resource.Symbol.DeclaringResource.TryGetBody()?.TryGetPropertyByName("properties", StringComparison.OrdinalIgnoreCase)?.Value is not ObjectSyntax propertiesObject ||
                    propertiesObject.TryGetPropertyByName("template", StringComparison.OrdinalIgnoreCase)?.Value is not SyntaxBase nestedTemplate)
                {
                    continue;
                }

                bool explicitInnerScope = (propertiesObject.TryGetPropertyByName("expressionEvaluationOptions", StringComparison.OrdinalIgnoreCase)?.Value as ObjectSyntax)
                    ?.TryGetPropertyByName("scope", StringComparison.OrdinalIgnoreCase)?.Value is SyntaxBase explicitScope &&
                    model.GetTypeInfo(explicitScope) is StringLiteralType folded &&
                    folded.RawStringValue.Equals("inner", StringComparison.OrdinalIgnoreCase);

                if (explicitInnerScope || innerScopedEvaluationIsDefault)
                {
                    foreach (var (symbolReferenced, references) in SymbolicReferenceCollector.CollectSymbolsReferenced(model.Binder, nestedTemplate))
                    {
                        if (!topLevelSymbols.Contains(symbolReferenced))
                        {
                            continue;
                        }

                        foreach (var reference in references)
                        {
                            yield return CreateDiagnosticForSpan(diagnosticLevel, reference.Span, symbolReferenced.Name);
                        }
                    }
                }
            }
        }
    }
}
