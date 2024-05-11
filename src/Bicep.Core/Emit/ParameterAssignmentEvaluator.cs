// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Diagnostics;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Engines;
using Azure.Deployments.Templates.Expressions;
using Azure.Deployments.Templates.Extensions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit.CompileTimeImports;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
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
    private readonly ImmutableDictionary<string, ImportedSymbol> importsByName;
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

    private ResultWithDiagnostic<Template> GetTemplateWithCaching(ISemanticModel model)
        => model switch
        {
            SemanticModel bicepModel => templateResults.GetOrAdd(bicepModel, GetTemplate),
            ArmTemplateSemanticModel armModel when armModel.SourceFile.Template is Template template => new(template),
            TemplateSpecSemanticModel tsModel when tsModel.SourceFile.MainTemplateFile.Template is Template template => new(template),
            ArmTemplateSemanticModel or TemplateSpecSemanticModel => new(x => x.ReferencedArmTemplateHasErrors()),
            _ => throw new UnreachableException(),
        };

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

        try
        {
            return Result.For(armEvaluators.GetOrAdd(importedFrom, t => new(t)).GetEvaluatedVariableValue(propertyReference.PropertyName));
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

            if (importsByName.TryGetValue(name, out var imported) && imported is ImportedVariableSymbol importedVariable)
            {
                return EvaluateImport(importedVariable).Value ?? throw new InvalidOperationException($"Imported variable {name} has an invalid value");
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

        var defaultEvaluateFunction = helper.EvaluationContext.EvaluateFunction;
        helper.EvaluationContext.EvaluateFunction = (expression, parameters, additionalProperties) =>
        {
            if (TemplateFunction.IsTemplateFunction(expression.Function))
            {
                return EvaluateTemplateFunction(expression, parameters, additionalProperties);
            }

            return defaultEvaluateFunction(expression, parameters, additionalProperties);
        };

        return helper.EvaluationContext;
    }

    private JToken EvaluateTemplateFunction(FunctionExpression expression, FunctionArgument[] parameters, TemplateErrorAdditionalInfo additionalProperties)
    {
        JToken evaluateFunction(Template template, string originalFunctionName)
        {
            var functionsLookup = template.GetFunctionDefinitions().ToOrdinalInsensitiveDictionary(x => x.Key, x => x.Function);

            var rewrittenExpression = new FunctionExpression(
                $"{EmitConstants.UserDefinedFunctionsNamespace}.{originalFunctionName}",
                expression.Parameters,
                expression.Properties);

            // we must explicitly ensure the evaluation takes place in the context of the referenced template,
            // so that accessing scoped functions works as expected
            var helper = new TemplateExpressionEvaluationHelper
            {
                OnGetFunction = (name, _) => functionsLookup[name],
                ValidationContext = SchemaValidationContext.ForTemplate(template),
            };

            return helper.EvaluationContext.EvaluateFunction(rewrittenExpression, parameters, additionalProperties);
        }

        if (expression.Function.StartsWith($"{EmitConstants.UserDefinedFunctionsNamespace}.") &&
            expression.Function.Substring($"{EmitConstants.UserDefinedFunctionsNamespace}.".Length) is { } functionName &&
            importsByName.TryGetValue(functionName, out var imported) &&
            imported.OriginalSymbolName is { } originalSymbolName)
        {
            var template = GetTemplateWithCaching(imported.SourceModel).Unwrap();
            var evaluator = armEvaluators.GetOrAdd(template, importedFrom => new(importedFrom));


            return evaluateFunction(template, originalSymbolName);
        }

        if (wildcardImportPropertiesByName.TryGetValue(expression.Function, out var wildcardImportProperty))
        {
            var template = GetTemplateWithCaching(wildcardImportProperty.WildcardImport.SourceModel).Unwrap();

            return evaluateFunction(template, wildcardImportProperty.PropertyName);
        }

        throw new InvalidOperationException($"Function {expression.Function} not found");
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

            return new(template);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Failed to generate template for {model.Root.FileUri}: {ex}");
            return new(x => x.ReferencedModuleHasErrors());
        }
    }
}
