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
using Bicep.Core.Emit.CompileTimeImports;
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
        private readonly ConcurrentDictionary<WildcardImportPropertyReference, Result> WildcardImportVariablesResults = new();
        private readonly ConcurrentDictionary<SemanticModel, ParameterAssignmentEvaluator> BicepEvaluators = new();
        private readonly ConcurrentDictionary<Template, TemplateVariablesEvaluator> ArmEvaluators = new();
        private readonly SemanticModel model;
        private readonly ImmutableDictionary<string, ParameterAssignmentSymbol> paramsByName;
        private readonly ImmutableDictionary<string, VariableSymbol> variablesByName;
        private readonly ImmutableDictionary<string, ImportedSymbol> importsByName;
        private readonly ImmutableDictionary<string, WildcardImportSymbol> wildcardImportsByName;
        private readonly ImportReferenceExpressionRewriter importReferenceExpressionRewriter;

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
            this.importReferenceExpressionRewriter = new(model.Root.ImportedSymbols.ToImmutableDictionary(s => s, s => s.Name),
                model.Root.WildcardImports
                    .SelectMany(w => w.TryGetSemanticModel() is ISemanticModel sourceModel
                        ? sourceModel.Exports.Keys.Select(name => new WildcardImportPropertyReference(w, name))
                        : Enumerable.Empty<WildcardImportPropertyReference>())
                    .ToImmutableDictionary(w => w, w => $"{w.WildcardImport.Name}.{w.PropertyName}"),
                sourceSyntax: null);
        }

        public Result EvaluateParameter(ParameterAssignmentSymbol parameter)
            => Results.GetOrAdd(
                parameter,
                parameter =>
                {
                    var context = GetExpressionEvaluationContext();
                    var converter = new ExpressionConverter(new(model));

                    var intermediate = converter.ConvertToIntermediateExpression(parameter.DeclaringParameterAssignment.Value);

                    if (intermediate is ParameterKeyVaultReferenceExpression keyVaultReferenceExpression)
                    {
                        return Result.For(keyVaultReferenceExpression);
                    }

                    intermediate = importReferenceExpressionRewriter.ReplaceImportReferences(intermediate);

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
                variable =>
                {
                    try
                    {
                        var context = GetExpressionEvaluationContext();
                        var converter = new ExpressionConverter(new(model));
                        var intermediate = converter.ConvertToIntermediateExpression(variable.DeclaringVariable.Value);
                        intermediate = importReferenceExpressionRewriter.ReplaceImportReferences(intermediate);

                        return Result.For(converter.ConvertExpression(intermediate).EvaluateExpression(context));
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

        private Result EvaluateWildcardImportPropertyAsVariable(WildcardImportPropertyReference propertyReference) => WildcardImportVariablesResults.GetOrAdd(propertyReference, r => r.WildcardImport.TryGetSemanticModel() switch
        {
            SemanticModel bicepModel
                => EvaluateWildcardImportPropertyAsVariable(bicepModel, r),
            ArmTemplateSemanticModel armModel when armModel.SourceFile.Template is Template template
                => EvaluateWildcardImportPropertyAsVariable(armModel, template, r),
            TemplateSpecSemanticModel tsModel when tsModel.SourceFile.MainTemplateFile.Template is Template template
                => EvaluateWildcardImportPropertyAsVariable(tsModel, template, r),
            ArmTemplateSemanticModel or TemplateSpecSemanticModel
                => Result.For(DiagnosticBuilder.ForPosition(r.WildcardImport.EnclosingDeclaration.FromClause).ReferencedArmTemplateHasErrors()),
            _ => throw new UnreachableException(),
        });

        private Result EvaluateWildcardImportPropertyAsVariable(SemanticModel importedFrom, WildcardImportPropertyReference propertyReference)
        {
            if (InvalidPropertyRefDiagnostic(importedFrom, propertyReference) is IDiagnostic diagnostic)
            {
                return Result.For(diagnostic);
            }

            var variableDeclaration = importedFrom.Root.VariableDeclarations.Where(v => v.Name.Equals(propertyReference.PropertyName, StringComparison.OrdinalIgnoreCase)).Single();
            var evaluated = BicepEvaluators.GetOrAdd(importedFrom, m => new(m)).EvaluateVariable(variableDeclaration);
            if (evaluated.Value is not null)
            {
                return Result.For(evaluated.Value);
            }

            return Result.For(DiagnosticBuilder.ForPosition(propertyReference.WildcardImport.DeclaringSyntax).FailedToEvaluateVariable(variableDeclaration.Name, evaluated.Diagnostic!.Message));
        }

        private Result EvaluateWildcardImportPropertyAsVariable(ISemanticModel model, Template importedFrom, WildcardImportPropertyReference propertyReference)
        {
            if (InvalidPropertyRefDiagnostic(model, propertyReference) is IDiagnostic diagnostic)
            {
                return Result.For(diagnostic);
            }

            var evaluator = ArmEvaluators.GetOrAdd(importedFrom, t => new(t));
            try
            {
                return Result.For(evaluator.GetEvaluatedVariableValue(propertyReference.PropertyName));
            }
            catch (Exception e)
            {
                return Result.For(DiagnosticBuilder.ForPosition(propertyReference.WildcardImport.DeclaringSyntax).FailedToEvaluateVariable(propertyReference.PropertyName, e.Message));
            }
        }

        private static IDiagnostic? InvalidPropertyRefDiagnostic(ISemanticModel importedFrom, WildcardImportPropertyReference propertyReference)
        {
            if (!importedFrom.Exports.TryGetValue(propertyReference.PropertyName, out var exportMetadata))
            {
                return DiagnosticBuilder.ForPosition(propertyReference.WildcardImport.DeclaringSyntax).ImportedSymbolNotFound(propertyReference.PropertyName);
            }

            if (exportMetadata.Kind != ExportMetadataKind.Variable)
            {
                return DiagnosticBuilder.ForPosition(propertyReference.WildcardImport.DeclaringSyntax).TypeSymbolUsedAsValue($"{propertyReference.WildcardImport.Name}.{propertyReference.PropertyName}");
            }

            return null;
        }

        private ExpressionEvaluationContext GetExpressionEvaluationContext()
        {
            var helper = new TemplateExpressionEvaluationHelper();
            helper.OnGetVariable = (name, _) =>
            {
                if (variablesByName.TryGetValue(name, out var variable))
                {
                    return EvaluateVariable(variable).Value ?? throw new InvalidOperationException($"Variable {name} has an invalid value");
                }

                if (importsByName.TryGetValue(name, out var imported))
                {
                    return EvaluateImport(imported).Value ?? throw new InvalidOperationException($"Imported variable {name} has an invalid value");
                }

                if (name.IndexOf(".") is int separatorIndex && separatorIndex > -1 && wildcardImportsByName.TryGetValue(name[..separatorIndex], out var wildcardImport))
                {
                    return EvaluateWildcardImportPropertyAsVariable(new(wildcardImport, name[(separatorIndex + 1)..])).Value ?? throw new InvalidOperationException($"Imported variable {name} has an invalid value");
                }

                throw new InvalidOperationException($"Variable {name} not found");
            };
            helper.OnGetParameter = (name, _) =>
            {
                return EvaluateParameter(paramsByName[name]).Value ?? throw new InvalidOperationException($"Parameter {name} has an invalid value");
            };

            return helper.EvaluationContext;
        }
    }
}
