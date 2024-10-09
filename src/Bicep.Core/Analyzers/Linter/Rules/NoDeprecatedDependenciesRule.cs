// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Cryptography.X509Certificates;
using Azure.Deployments.Templates.Export;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class NoDeprecatedDependenciesRule : LinterRuleBase
{
    public new const string Code = "no-deprecated-dependencies";

    public NoDeprecatedDependenciesRule() : base(
        code: Code,
        description: CoreResources.NoDeprecatedDependenciesRuleDescription,
        LinterRuleCategory.BestPractice,
        docUri: new Uri($"https://aka.ms/bicep/linter/{Code}"),
        diagnosticStyling: DiagnosticStyling.ShowCodeDeprecated)
    { }

    public override string FormatMessage(params object[] values)
        => values.Length == 1 ?
            string.Format(CoreResources.NoDeprecatedDependenciesRuleMessageFormat, values) :
            string.Format(CoreResources.NoDeprecatedDependenciesRuleMessageFormatWithDescription, values);

    private IDiagnostic CreateDeprecationDiagnostic(TextSpan span, string type, DeprecationMetadata deprecation)
        => deprecation.Description is null ? 
            this.CreateDiagnosticForSpan(DefaultDiagnosticLevel, span, type) :
            this.CreateDiagnosticForSpan(DefaultDiagnosticLevel, span, type, deprecation.Description);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        foreach (var module in model.Root.ModuleDeclarations)
        {
            if (module.TryGetBodyPropertyValue(LanguageConstants.ModuleParamsPropertyName) is ObjectSyntax paramsObj &&
                module.TryGetSemanticModel().TryUnwrap() is {} moduleModel)
            {
                foreach (var property in paramsObj.Properties)
                {
                    if (property.TryGetKeyText() is {} key &&
                        moduleModel.Parameters.TryGetValue(key, out var parameter) &&
                        parameter.DeprecationMetadata is {} deprecation)
                    {
                        yield return CreateDeprecationDiagnostic(property.Key.Span, parameter.Name, deprecation);
                    }
                }
            }
        }

        foreach (var outputAccess in SyntaxAggregator.AggregateByType<PropertyAccessSyntax>(model.SourceFile.ProgramSyntax))
        {
            if (outputAccess.BaseExpression is PropertyAccessSyntax propertyAccess &&
                propertyAccess.PropertyName.NameEquals(LanguageConstants.ModuleOutputsPropertyName) &&
                model.GetSymbolInfo(propertyAccess.BaseExpression) is ModuleSymbol module &&
                module.TryGetSemanticModel().TryUnwrap() is {} moduleModel &&
                moduleModel.Outputs.FirstOrDefault(x => outputAccess.PropertyName.NameEquals(x.Name)) is {} output &&
                output.DeprecationMetadata is {} deprecation)
            {
                yield return CreateDeprecationDiagnostic(outputAccess.Span, output.Name, deprecation);
            }
        }

        foreach (var import in model.Root.ImportedSymbols)
        {
            var sourceModel = import.SourceModel;

            if (import.OriginalSymbolName is {} originalName && 
                sourceModel.Exports.TryGetValue(originalName, out var export)&&
                export.DeprecationMetadata is {} deprecation)
            {
                yield return CreateDeprecationDiagnostic(import.DeclaringImportedSymbolsListItem.Span, export.Name, deprecation);
            }
        }
    }
}
