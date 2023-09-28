// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    public class ParameterAssignmentEvaluator
    {
        public class Result
        {
            private Result(JToken? value, ParameterKeyVaultReferenceExpression? keyVaultReference, IDiagnostic? diagnostic)
            {
                Value = value;
                KeyVaultReference = keyVaultReference;
                Diagnostic = diagnostic;
            }

            public JToken? Value { get; }
            public ParameterKeyVaultReferenceExpression? KeyVaultReference { get; }
            public IDiagnostic? Diagnostic { get; }

            public static Result For(JToken value) => new(value, null, null);

            public static Result For(ParameterKeyVaultReferenceExpression expression) => new(null, expression, null);

            public static Result For(IDiagnostic diagnostic) => new(null, null, diagnostic);
        }

        private readonly ConcurrentDictionary<ParameterAssignmentSymbol, Result> Results = new();
        private readonly ConcurrentDictionary<VariableSymbol, Result> VarResults = new();
        private readonly ConcurrentDictionary<ImportedSymbol, Result> ImportResults = new();
        private readonly ConcurrentDictionary<WildcardImportSymbol, Result> WildcardImportVariablesResults = new();
        private readonly ConcurrentDictionary<SemanticModel, ParameterAssignmentEvaluator> BicepEvaluators = new();
        private readonly ConcurrentDictionary<Template, TemplateVariablesEvaluator> ArmEvaluators = new();
        private readonly SemanticModel model;
        private readonly ImmutableDictionary<string, ParameterAssignmentSymbol> paramsByName;
        private readonly ImmutableDictionary<string, VariableSymbol> variablesByName;
        private readonly ImmutableDictionary<string, ImportedSymbol> importsByName;
        private readonly ImmutableDictionary<string, WildcardImportSymbol> wildcardImportsByName;

        public ParameterAssignmentEvaluator(SemanticModel model)
        {
            this.model = model;
            this.paramsByName = model.Root.ParameterAssignments
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
            this.variablesByName = model.Root.VariableDeclarations
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
            this.importsByName = model.Root.ImportedSymbols
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
            this.wildcardImportsByName = model.Root.WildcardImports
                .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        public Result EvaluateParameter(ParameterAssignmentSymbol parameter)
            => Results.GetOrAdd(
                parameter,
                parameter => {
                    var context = GetExpressionEvaluationContext();
                    var converter = new ExpressionConverter(new(model));

                    var intermediate = converter.ConvertToIntermediateExpression(parameter.DeclaringParameterAssignment.Value);

                    if (intermediate is ParameterKeyVaultReferenceExpression keyVaultReferenceExpression)
                    {
                        return Result.For(keyVaultReferenceExpression);
                    }

                    try
                    {
                        return Result.For(converter.ConvertExpression(intermediate).EvaluateExpression(context));
                    }
                    catch (Exception ex)
                    {
                        return Result.For(DiagnosticBuilder.ForPosition(parameter.DeclaringParameterAssignment.Value)
                            .FailedToEvaluateParameter(parameter.Name, ex.Message));
                    }
                });

        private Result EvaluateVariable(VariableSymbol variable)
            => VarResults.GetOrAdd(
                variable,
                variable => {
                    try
                    {
                        var context = GetExpressionEvaluationContext();
                        var converter = new ExpressionConverter(new(model));
                        var expression = converter.ConvertExpression(variable.DeclaringVariable.Value);

                        return Result.For(expression.EvaluateExpression(context));
                    }
                    catch (Exception ex)
                    {
                        return Result.For(DiagnosticBuilder.ForPosition(variable.DeclaringVariable.Value)
                            .FailedToEvaluateVariable(variable.Name, ex.Message));
                    }
                });

        private Result EvaluateImport(ImportedSymbol import) => ImportResults.GetOrAdd(import, import =>
        {
            if (import.Kind == SymbolKind.Variable)
            {
                return import.TryGetSemanticModel() switch
                {
                    SemanticModel bicepModel
                        => EvaluateImportedVariable(bicepModel, import),
                    ArmTemplateSemanticModel armModel when armModel.SourceFile.Template is Template template
                        => EvaluateImportedVariable(template, import),
                    TemplateSpecSemanticModel tsModel when tsModel.SourceFile.MainTemplateFile.Template is Template template
                        => EvaluateImportedVariable(template, import),
                    ArmTemplateSemanticModel or TemplateSpecSemanticModel
                        => Result.For(DiagnosticBuilder.ForPosition(import.EnclosingDeclaration.FromClause).ReferencedArmTemplateHasErrors()),
                    _ => throw new UnreachableException(),
                };
            }

            return Result.For(DiagnosticBuilder.ForPosition(import.DeclaringImportedSymbolsListItem.OriginalSymbolName).TypeSymbolUsedAsValue(import.Name));
        });

        private Result EvaluateImportedVariable(SemanticModel importedFrom, ImportedSymbol symbol)
        {
            if (importedFrom.Root.VariableDeclarations
                .Where(v => importedFrom.Exports.ContainsKey(v.Name))
                .Where(v => LanguageConstants.IdentifierComparer.Equals(v.Name, symbol.OriginalSymbolName))
                .FirstOrDefault() is VariableSymbol exportedVariable)
            {
                return BicepEvaluators.GetOrAdd(importedFrom, model => new(model)).EvaluateVariable(exportedVariable);
            }

            return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringImportedSymbolsListItem.OriginalSymbolName).ImportedSymbolNotFound(symbol.OriginalSymbolName ?? LanguageConstants.MissingName));
        }

        private Result EvaluateImportedVariable(Template importedFrom, ImportedSymbol symbol)
        {
            if (symbol.OriginalSymbolName is not string originalSymbolName)
            {
                return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringImportedSymbolsListItem.OriginalSymbolName).ExpectedVariableIdentifier());
            }

            try
            {
                return Result.For(ArmEvaluators.GetOrAdd(importedFrom, t => new(t)).GetEvaluatedVariableValue(originalSymbolName));
            }
            catch (Exception e)
            {
                return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringImportedSymbolsListItem.OriginalSymbolName)
                    .FailedToEvaluateVariable(originalSymbolName, e.Message));
            }
        }

        private Result EvaluateWildcardImportAsVariable(WildcardImportSymbol symbol) => WildcardImportVariablesResults.GetOrAdd(symbol, s => s.TryGetSemanticModel() switch
        {
            SemanticModel bicepModel
                => EvaluateWildcardImportAsVariable(bicepModel, s),
            ArmTemplateSemanticModel armModel when armModel.SourceFile.Template is Template template
                => EvaluateWildcardImportAsVariable(armModel, template, s),
            TemplateSpecSemanticModel tsModel when tsModel.SourceFile.MainTemplateFile.Template is Template template
                => EvaluateWildcardImportAsVariable(tsModel, template, s),
            ArmTemplateSemanticModel or TemplateSpecSemanticModel
                => Result.For(DiagnosticBuilder.ForPosition(s.EnclosingDeclaration.FromClause).ReferencedArmTemplateHasErrors()),
            _ => throw new UnreachableException(),
        });

        private Result EvaluateWildcardImportAsVariable(SemanticModel importedFrom, WildcardImportSymbol symbol)
        {
            var evaluator = BicepEvaluators.GetOrAdd(importedFrom, m => new(m));
            JObject target = new();

            foreach (var variableDeclaration in importedFrom.Root.VariableDeclarations.Where(v => importedFrom.Exports.ContainsKey(v.Name)))
            {
                var evaluated = evaluator.EvaluateVariable(variableDeclaration);
                if (evaluated.Value is not null)
                {
                    target[variableDeclaration.Name] = evaluated.Value;
                }
                else
                {
                    return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringSyntax).FailedToEvaluateVariable(variableDeclaration.Name, evaluated.Diagnostic!.Message));
                }
            }

            return Result.For(target);
        }

        private Result EvaluateWildcardImportAsVariable(ISemanticModel model, Template importedFrom, WildcardImportSymbol symbol)
        {
            var evaluator = ArmEvaluators.GetOrAdd(importedFrom, t => new(t));
            JObject target = new();

            foreach (var variableMetadata in model.Exports.Values.OfType<ExportedVariableMetadata>())
            {
                try
                {
                    target[variableMetadata.Name] = evaluator.GetEvaluatedVariableValue(variableMetadata.Name);
                }
                catch (Exception e)
                {
                    return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringSyntax).FailedToEvaluateVariable(variableMetadata.Name, e.Message));
                }
            }

            return Result.For(target);
        }

        private ExpressionEvaluationContext GetExpressionEvaluationContext()
        {
            var helper = new TemplateExpressionEvaluationHelper();
            helper.OnGetVariable = (name, _) => {
                if (variablesByName.TryGetValue(name, out var variable))
                {
                    return EvaluateVariable(variable).Value ?? throw new InvalidOperationException($"Variable {name} has an invalid value");
                }

                if (importsByName.TryGetValue(name, out var imported))
                {
                    return EvaluateImport(imported).Value ?? throw new InvalidOperationException($"Imported variable {name} has an invalid value");
                }

                if (wildcardImportsByName.TryGetValue(name, out var wildcardImport))
                {
                    return EvaluateWildcardImportAsVariable(wildcardImport).Value ?? throw new InvalidOperationException($"Imported namespace {name} is not valid");
                }

                throw new InvalidOperationException($"Variable {name} not found");
            };
            helper.OnGetParameter = (name, _) => {
                return EvaluateParameter(paramsByName[name]).Value ?? throw new InvalidOperationException($"Parameter {name} has an invalid value");
            };

            return helper.EvaluationContext;
        }
    }
}
