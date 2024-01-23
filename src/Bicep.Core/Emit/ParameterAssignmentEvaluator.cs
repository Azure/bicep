// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Expressions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit.CompileTimeImports;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

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

    private readonly ConcurrentDictionary<ParameterAssignmentSymbol, Result> results = new();
    private readonly ConcurrentDictionary<VariableSymbol, Result> varResults = new();
    private readonly ConcurrentDictionary<ImportedVariableSymbol, Result> importResults = new();
    private readonly ConcurrentDictionary<WildcardImportPropertyReference, Result> wildcardImportVariableResults = new();
    private readonly ConcurrentDictionary<Expression, Result> synthesizedVariableResults = new();
    private readonly ConcurrentDictionary<SemanticModel, ResultWithDiagnostic<Template>> templateResults = new();
    private readonly ConcurrentDictionary<Template, TemplateVariablesEvaluator> armEvaluators = new();
    private readonly ImmutableDictionary<string, ParameterAssignmentSymbol> paramsByName;
    private readonly ImmutableDictionary<string, VariableSymbol> variablesByName;
    private readonly ImmutableDictionary<string, ImportedVariableSymbol> importsByName;
    private readonly ImmutableDictionary<string, WildcardImportPropertyReference> wildcardImportPropertiesByName;
    private readonly ImmutableDictionary<string, Expression> synthesizedVariableValuesByName;
    private readonly ExpressionConverter converter;

    public ParameterAssignmentEvaluator(SemanticModel model)
    {
        this.paramsByName = model.Root.ParameterAssignments
            .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        this.variablesByName = model.Root.VariableDeclarations
            .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);

        EmitterContext context = new(model);
        this.converter = new(context);
        this.importsByName = context.ImportClosureInfo.ImportedSymbolNames.Keys
            .OfType<ImportedVariableSymbol>()
            .Select(importedVariable => (context.ImportClosureInfo.ImportedSymbolNames[importedVariable], importedVariable))
            .GroupBy(x => x.Item1, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First().importedVariable, LanguageConstants.IdentifierComparer);
        this.wildcardImportPropertiesByName = context.ImportClosureInfo.WildcardImportPropertyNames
            .GroupBy(x => x.Value, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First().Key, LanguageConstants.IdentifierComparer);
        this.synthesizedVariableValuesByName = context.FunctionVariables.Values
            .GroupBy(result => result.Name)
            .ToImmutableDictionary(x => x.Key, x => x.First().Value);
    }

    public Result EvaluateParameter(ParameterAssignmentSymbol parameter)
        => results.GetOrAdd(
            parameter,
            parameter =>
            {
                var context = GetExpressionEvaluationContext();

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
        => varResults.GetOrAdd(
            variable,
            variable =>
            {
                try
                {
                    var context = GetExpressionEvaluationContext();
                    var intermediate = converter.ConvertToIntermediateExpression(variable.DeclaringVariable.Value);

                    return Result.For(converter.ConvertExpression(intermediate).EvaluateExpression(context));
                }
                catch (Exception ex)
                {
                    return Result.For(DiagnosticBuilder.ForPosition(variable.DeclaringVariable.Value)
                        .FailedToEvaluateVariable(variable.Name, ex.Message));
                }
            });

    private Result EvaluateSynthesizeVariableExpression(string name, Expression expression)
        => synthesizedVariableResults.GetOrAdd(
            expression,
            expression =>
            {
                try
                {
                    return Result.For(converter.ConvertExpression(expression).EvaluateExpression(GetExpressionEvaluationContext()));
                }
                catch (Exception e)
                {
                    return Result.For(DiagnosticBuilder.ForDocumentStart().FailedToEvaluateVariable(name, e.Message));
                }
            });

    private Result EvaluateImport(ImportedVariableSymbol import) => importResults.GetOrAdd(import, import =>
    {
        if (import.Kind == SymbolKind.Variable)
        {
            return import.SourceModel switch
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
        if (templateResults.GetOrAdd(importedFrom, GetTemplate).IsSuccess(out var template, out var diagnosticBuilder))
        {
            return EvaluateImportedVariable(template, symbol);
        }

        return Result.For(diagnosticBuilder(DiagnosticBuilder.ForPosition(symbol.DeclaringImportedSymbolsListItem.OriginalSymbolName)));
    }

    private Result EvaluateImportedVariable(Template importedFrom, ImportedSymbol symbol)
    {
        if (symbol.OriginalSymbolName is not string originalSymbolName)
        {
            return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringImportedSymbolsListItem.OriginalSymbolName).ExpectedVariableIdentifier());
        }

        try
        {
            return Result.For(armEvaluators.GetOrAdd(importedFrom, t => new(t)).GetEvaluatedVariableValue(originalSymbolName));
        }
        catch (Exception e)
        {
            return Result.For(DiagnosticBuilder.ForPosition(symbol.DeclaringImportedSymbolsListItem.OriginalSymbolName)
                .FailedToEvaluateVariable(originalSymbolName, e.Message));
        }
    }

    private Result EvaluateWildcardImportPropertyAsVariable(WildcardImportPropertyReference propertyReference) => wildcardImportVariableResults.GetOrAdd(propertyReference, r => r.WildcardImport.SourceModel switch
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
        if (templateResults.GetOrAdd(importedFrom, GetTemplate).IsSuccess(out var template, out var diagnosticBuilder))
        {
            return EvaluateWildcardImportPropertyAsVariable(importedFrom, template, propertyReference);
        }

        return Result.For(diagnosticBuilder(DiagnosticBuilder.ForPosition(propertyReference.WildcardImport.DeclaringSyntax)));
    }

    private Result EvaluateWildcardImportPropertyAsVariable(ISemanticModel model, Template importedFrom, WildcardImportPropertyReference propertyReference)
    {
        if (InvalidPropertyRefDiagnostic(model, propertyReference) is IDiagnostic diagnostic)
        {
            return Result.For(diagnostic);
        }

        var evaluator = armEvaluators.GetOrAdd(importedFrom, t => new(t));
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

            if (wildcardImportPropertiesByName.TryGetValue(name, out var wildcardImportProperty))
            {
                return EvaluateWildcardImportPropertyAsVariable(wildcardImportProperty).Value
                    ?? throw new InvalidOperationException($"Imported variable {wildcardImportProperty.WildcardImport.Name}.{wildcardImportProperty.PropertyName} has an invalid value");
            }

            if (synthesizedVariableValuesByName.TryGetValue(name, out var value))
            {
                return EvaluateSynthesizeVariableExpression(name, value).Value
                    ?? throw new InvalidOperationException($"Synthesized variable {name} has an invalid value");
            }

            throw new InvalidOperationException($"Variable {name} not found");
        };
        helper.OnGetParameter = (name, _) =>
        {
            return EvaluateParameter(paramsByName[name]).Value ?? throw new InvalidOperationException($"Parameter {name} has an invalid value");
        };

        return helper.EvaluationContext;
    }

    private static ResultWithDiagnostic<Template> GetTemplate(SemanticModel model)
    {
        if (model.HasErrors())
        {
            return new(x => x.ReferencedModuleHasErrors());
        }

        try
        {
            var textWriter = new StringWriter();
            using var writer = new SourceAwareJsonTextWriter(textWriter)
            {
                // don't close the textWriter when writer is disposed
                CloseOutput = false,
                Formatting = Formatting.Indented
            };
            var (template, _) = new TemplateWriter(model).GetTemplate(writer);

            TemplateEngine.ProcessTemplateLanguageExpressions(null, null, null, template, TemplateWriter.NestedDeploymentResourceApiVersion, null);

            return new(template);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Failed to generate template for {model.Root.FileUri}: {ex}");
            return new(x => x.ReferencedModuleHasErrors());
        }
    }
}
