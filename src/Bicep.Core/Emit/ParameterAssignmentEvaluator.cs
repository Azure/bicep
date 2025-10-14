// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Expressions;
using Azure.Deployments.Templates.Extensions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit.CompileTimeImports;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public class ParameterAssignmentEvaluator
{
    private class ParameterAssignmentEvaluationContext : IEvaluationContext
    {
        private readonly TemplateExpressionEvaluationHelper evaluationHelper;
        private readonly ParameterAssignmentEvaluator evaluator;

        public ParameterAssignmentEvaluationContext(TemplateExpressionEvaluationHelper evaluationHelper, ParameterAssignmentEvaluator evaluator)
            : this(evaluationHelper, evaluator, evaluationHelper.EvaluationContext.Scope)
        {
        }

        private ParameterAssignmentEvaluationContext(TemplateExpressionEvaluationHelper evaluationHelper, ParameterAssignmentEvaluator evaluator, ExpressionScope scope)
        {
            this.evaluationHelper = evaluationHelper;
            this.evaluator = evaluator;
            this.Scope = scope;
        }

        public bool IsShortCircuitAllowed => evaluationHelper.EvaluationContext.IsShortCircuitAllowed;

        public ExpressionScope Scope { get; }

        public bool AllowInvalidProperty(Exception exception, FunctionExpression functionExpression, FunctionArgument[] functionParametersValues, JToken[] selectedProperties) =>
            evaluationHelper.EvaluationContext.AllowInvalidProperty(exception, functionExpression, functionParametersValues, selectedProperties);

        public JToken EvaluateFunction(FunctionExpression functionExpression, FunctionArgument[] parameters, IEvaluationContext context, TemplateErrorAdditionalInfo? additionalnfo)
        {
            // IsTemplateFunction checks if the function is a user defined function.
            if (TemplateFunction.IsTemplateFunction(functionExpression.Function))
            {
                return this.EvaluateUserDefinedFunction(functionExpression, parameters, additionalnfo);
            }

            return evaluationHelper.EvaluationContext.EvaluateFunction(functionExpression, parameters, this, additionalnfo);
        }

        public bool ShouldIgnoreExceptionDuringEvaluation(Exception exception) =>
            this.evaluationHelper.EvaluationContext.ShouldIgnoreExceptionDuringEvaluation(exception);

        public IEvaluationContext WithNewScope(ExpressionScope scope) => new ParameterAssignmentEvaluationContext(this.evaluationHelper, this.evaluator, scope);

        private JToken EvaluateUserDefinedFunction(FunctionExpression expression, FunctionArgument[] parameters, TemplateErrorAdditionalInfo? additionalProperties)
        {
            JToken evaluateFunction(Template template, string originalFunctionName)
            {
                var functionsLookup = template.GetFunctionDefinitions().ToOrdinalInsensitiveDictionary(x => x.Key, x => x.Function);
                TemplateVariablesEvaluator variablesEvaluator = new(template);

                var rewrittenExpression = new FunctionExpression(
                    $"{EmitConstants.UserDefinedFunctionsNamespace}.{originalFunctionName}",
                    expression.Parameters,
                    expression.Properties);

                // we must explicitly ensure the evaluation takes place in the context of the referenced template,
                // so that accessing scoped functions works as expected
                var helper = new TemplateExpressionEvaluationHelper
                {
                    OnGetFunction = (name, _) => functionsLookup[name],
                    OnGetVariable = (name, _) => variablesEvaluator.GetEvaluatedVariableValue(name),
                    ValidationContext = SchemaValidationContext.ForTemplate(template),
                };

                return helper.EvaluationContext.EvaluateFunction(rewrittenExpression, parameters, this, additionalProperties);
            }

            if (expression.Function.StartsWith($"{EmitConstants.UserDefinedFunctionsNamespace}.") &&
                expression.Function[$"{EmitConstants.UserDefinedFunctionsNamespace}.".Length..] is { } functionName)
            {
                if (this.evaluator.importsByName.TryGetValue(functionName, out var imported) &&
                    imported.OriginalSymbolName is { } originalSymbolName)
                {
                    var template = this.evaluator.GetTemplateWithCaching(imported.SourceModel).Unwrap();
                    var evaluator = this.evaluator.armEvaluators.GetOrAdd(template, importedFrom => new(importedFrom));

                    return evaluateFunction(template, originalSymbolName);
                }

                if (this.evaluator.functionsByName.TryGetValue(functionName, out var functionSymbol))
                {
                    var template = this.evaluator.GetTemplateWithCaching(functionSymbol.GetSemanticModel()).Unwrap();

                    return evaluateFunction(template, functionName);
                }
            }

            if (this.evaluator.wildcardImportPropertiesByName.TryGetValue(expression.Function, out var wildcardImportProperty))
            {
                var template = this.evaluator.GetTemplateWithCaching(wildcardImportProperty.WildcardImport.SourceModel).Unwrap();

                return evaluateFunction(template, wildcardImportProperty.PropertyName);
            }

            throw new InvalidOperationException($"Function {expression.Function} not found");
        }
    }

    public class Result
    {
        private Result(JToken? value, Expression? expression, ParameterKeyVaultReferenceExpression? keyVaultReference, IDiagnostic? diagnostic)
        {
            Value = value;
            KeyVaultReference = keyVaultReference;
            Expression = expression;
            Diagnostic = diagnostic;
        }

        public JToken? Value { get; }
        public Expression? Expression { get; }
        public ParameterKeyVaultReferenceExpression? KeyVaultReference { get; }
        public IDiagnostic? Diagnostic { get; }

        public static Result For(JToken value) => new(value, null, null, null);
        public static Result For(Expression expression) => new(null, expression, null, null);
        public static Result For(ParameterKeyVaultReferenceExpression expression) => new(null, null, expression, null);

        public static Result For(IDiagnostic diagnostic) => new(null, null, null, diagnostic);
    }

    private readonly ConcurrentDictionary<ParameterAssignmentSymbol, Result> results = new();
    private readonly ConcurrentDictionary<ExtensionConfigAssignmentSymbol, ImmutableDictionary<string, Result>> extensionConfigAssignmentResults = new();
    private readonly ConcurrentDictionary<VariableSymbol, Result> varResults = new();
    private readonly ConcurrentDictionary<ImportedVariableSymbol, Result> importResults = new();
    private readonly ConcurrentDictionary<WildcardImportPropertyReference, Result> wildcardImportVariableResults = new();
    private readonly ConcurrentDictionary<Expression, Result> synthesizedVariableResults = new();
    private readonly ConcurrentDictionary<SemanticModel, ResultWithDiagnosticBuilder<Template>> templateResults = new();
    private readonly ConcurrentDictionary<Template, TemplateVariablesEvaluator> armEvaluators = new();
    private readonly ImmutableDictionary<string, ParameterAssignmentSymbol> paramsByName;
    private readonly ImmutableDictionary<string, VariableSymbol> variablesByName;
    private readonly ImmutableDictionary<string, DeclaredFunctionSymbol> functionsByName;
    private readonly ImmutableDictionary<string, ImportedSymbol> importsByName;
    private readonly ImmutableDictionary<string, WildcardImportPropertyReference> wildcardImportPropertiesByName;
    private readonly ImmutableDictionary<string, Expression> synthesizedVariableValuesByName;
    private readonly ExternalInputReferences externalInputReferences;
    private readonly ExpressionConverter converter;

    public ParameterAssignmentEvaluator(SemanticModel model)
    {
        this.paramsByName = model.Root.ParameterAssignments
            .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        this.variablesByName = model.Root.VariableDeclarations
            .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        this.functionsByName = model.Root.FunctionDeclarations
            .GroupBy(x => x.Name, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);

        EmitterContext context = new(model);
        this.converter = new(context);
        this.importsByName = context.SemanticModel.ImportClosureInfo.ImportedSymbolNames.Keys
            .Select(importedVariable => (context.SemanticModel.ImportClosureInfo.ImportedSymbolNames[importedVariable], importedVariable))
            .GroupBy(x => x.Item1, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First().importedVariable, LanguageConstants.IdentifierComparer);
        this.wildcardImportPropertiesByName = context.SemanticModel.ImportClosureInfo.WildcardImportPropertyNames
            .GroupBy(x => x.Value, LanguageConstants.IdentifierComparer)
            .ToImmutableDictionary(x => x.Key, x => x.First().Key, LanguageConstants.IdentifierComparer);
        this.synthesizedVariableValuesByName = context.FunctionVariables.Values
            .GroupBy(result => result.Name)
            .ToImmutableDictionary(x => x.Key, x => x.First().Value);

        this.externalInputReferences = context.ExternalInputReferences;
    }

    public Result EvaluateParameter(ParameterAssignmentSymbol parameter)
        => results.GetOrAdd(
            parameter,
            parameter =>
            {
                var context = GetExpressionEvaluationContext();

                var declaringParam = parameter.DeclaringParameterAssignment;

                var intermediate = converter.ConvertToIntermediateExpression(declaringParam.Value);

                if (this.externalInputReferences.ParametersReferences.Contains(parameter))
                {
                    var rewrittenExpression = ExternalInputExpressionRewriter
                        .Rewrite(intermediate, this.externalInputReferences);

                    return Result.For(rewrittenExpression);
                }

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
                    return Result.For(DiagnosticBuilder.ForPosition(declaringParam.Value)
                        .FailedToEvaluateSubject("parameter", parameter.Name, ex.Message));
                }
            });

    public Result? EvaluateUsingConfig(FileSymbol file)
    {
        if (file.UsingDeclarationSyntax?.Config is not { } config)
        {
            return null;
        }

        var intermediate = converter.ConvertToIntermediateExpression(config);

        var rewrittenExpression = ExternalInputExpressionRewriter
            .Rewrite(intermediate, this.externalInputReferences);

        return Result.For(rewrittenExpression);
    }

    public ImmutableDictionary<string, Result> EvaluateExtensionConfigAssignment(ExtensionConfigAssignmentSymbol inputExtConfigAssignment)
        => extensionConfigAssignmentResults.GetOrAdd(
            inputExtConfigAssignment,
            extConfigAssignment =>
            {
                if (extConfigAssignment.DeclaringExtensionConfigAssignment.Config is null)
                {
                    return ImmutableDictionary<string, Result>.Empty;
                }

                var context = GetExpressionEvaluationContext();
                var configAssignmentProperties = extConfigAssignment.DeclaringExtensionConfigAssignment.Config.Properties;

                var resultBuilder = ImmutableDictionary.CreateBuilder<string, Result>();

                foreach (var property in configAssignmentProperties)
                {
                    var propertyName = property.TryGetKeyText();

                    if (propertyName is null)
                    {
                        continue;
                    }

                    var intermediate = converter.ConvertToIntermediateExpression(property.Value);

                    Result propertyResult;

                    if (intermediate is ParameterKeyVaultReferenceExpression keyVaultReferenceExpr)
                    {
                        propertyResult = Result.For(keyVaultReferenceExpr);
                    }
                    else
                    {
                        try
                        {
                            propertyResult = Result.For(converter.ConvertExpression(intermediate).EvaluateExpression(context));
                        }
                        catch (Exception ex)
                        {
                            propertyResult = Result.For(
                                DiagnosticBuilder.ForPosition(property.Value)
                                    .FailedToEvaluateSubject("extension config", extConfigAssignment.Name, ex.Message));
                        }
                    }

                    resultBuilder.Add(propertyName, propertyResult);
                }

                return resultBuilder.ToImmutableDictionary();
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

    private ResultWithDiagnosticBuilder<Template> GetTemplateWithCaching(ISemanticModel model)
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

    private ParameterAssignmentEvaluationContext GetExpressionEvaluationContext()
    {
        var helper = new TemplateExpressionEvaluationHelper
        {
            OnGetVariable = (name, _) =>
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
            },
            OnGetParameter = (name, _) =>
            {
                return EvaluateParameter(paramsByName[name]).Value ?? throw new InvalidOperationException($"Parameter {name} has an invalid value");
            },
        };

        return new(helper, this);
    }

    private static ResultWithDiagnosticBuilder<Template> GetTemplate(SemanticModel model)
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
            Trace.WriteLine($"Failed to generate template for {model.SourceFile.FileHandle.Uri}: {ex}");
            return new(x => x.ReferencedModuleHasErrors());
        }
    }

    public ExpressionEvaluationResult EvaluateExpression(SyntaxBase expressionSyntax)
    {
        try
        {
            var context = GetExpressionEvaluationContext();
            var intermediate = converter.ConvertToIntermediateExpression(expressionSyntax);
            var result = converter.ConvertExpression(intermediate).EvaluateExpression(context);
            return ExpressionEvaluationResult.For(result);
        }
        catch (Exception ex)
        {
            return ExpressionEvaluationResult.For([DiagnosticBuilder.ForPosition(expressionSyntax)
                .FailedToEvaluateSubject("expression", expressionSyntax.ToString(), ex.Message)]);
        }
    }

    /// <summary>
    /// Rewrites the external input function calls to use the externalInputs function with the index of the external input.
    /// e.g. externalInput('sys.cli', 'foo') becomes externalInputs('0')
    /// </summary>
    private class ExternalInputExpressionRewriter : ExpressionRewriteVisitor
    {
        private readonly ExternalInputReferences externalInputReferences;

        private ExternalInputExpressionRewriter(
            ExternalInputReferences externalInputReferences)
        {
            this.externalInputReferences = externalInputReferences;
        }

        public static Expression Rewrite(
            Expression expression,
            ExternalInputReferences externalInputReferences)
        {
            var visitor = new ExternalInputExpressionRewriter(externalInputReferences);
            var rewritten = visitor.Replace(expression);
            return rewritten;
        }

        public override Expression ReplaceFunctionCallExpression(FunctionCallExpression expression)
        {
            if (expression.SourceSyntax is FunctionCallSyntaxBase functionCallSyntax &&
                externalInputReferences.ExternalInputIndexMap.TryGetValue(functionCallSyntax, out var definitionKey))
            {
                return new FunctionCallExpression(
                    functionCallSyntax,
                    LanguageConstants.ExternalInputsArmFunctionName,
                    [ExpressionFactory.CreateStringLiteral(definitionKey)]
                );
            }

            return base.ReplaceFunctionCallExpression(expression);
        }
    }
}