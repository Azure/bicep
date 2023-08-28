// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Workspaces;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal static class ArmTemplateHelpers
{
    private const string ArmTypeRefPrefix = "#/definitions/";
    private const string VariablesFunctionName = "variables";

    internal static Template TemplateFor(ArmTemplateFile templateFile)
    {
        if (templateFile.Template is not {} template)
        {
            throw new InvalidOperationException($"Source template of {templateFile.FileUri} is not valid");
        }

        return template;
    }

    internal static SchemaValidationContext ContextFor(ArmTemplateFile templateFile)
        => SchemaValidationContext.ForTemplate(TemplateFor(templateFile));

    internal static TemplateVariablesEvaluator VariablesEvaluatorFor(ArmTemplateFile templateFile)
        => new(TemplateFor(templateFile));

    internal static ITemplateSchemaNode DereferenceArmType(SchemaValidationContext context, string typePointer)
    {
        // TODO make LocalSchemaRefResolver in Azure.Deployments.Templates public
        if (!typePointer.StartsWith(ArmTypeRefPrefix) ||
            typePointer[ArmTypeRefPrefix.Length..].Contains('/') ||
            !context.Definitions.TryGetValue(typePointer[ArmTypeRefPrefix.Length..], out var typeDefinition))
        {
            throw new InvalidOperationException($"Invalid ARM template type reference ({typePointer}) encountered");
        }

        return typeDefinition;
    }

    internal static JToken DereferenceArmVariable(Template template, string name)
    {
        if (template.Variables?.TryGetValue(name, out var value) is true)
        {
            return value.Value;
        }

        throw new InvalidOperationException($"No variable named '{name}' was found in the template.");
    }

    internal static IEnumerable<string> EnumerateTypeReferencesUsedIn(SchemaValidationContext context, string typePointer)
        => EnumerateTypeReferencesUsedIn(DereferenceArmType(context, typePointer));

    private static IEnumerable<string> EnumerateTypeReferencesUsedIn(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.Ref?.Value is string @ref)
        {
            yield return @ref;
        }

        if (schemaNode.AdditionalProperties?.SchemaNode is {} addlPropertiesType)
        {
            foreach (var nested in EnumerateTypeReferencesUsedIn(addlPropertiesType))
            {
                yield return nested;
            }
        }

        if (schemaNode.Properties is {} properties)
        {
            foreach (var nested in properties.Values.SelectMany(EnumerateTypeReferencesUsedIn))
            {
                yield return nested;
            }
        }

        if (schemaNode.Items?.SchemaNode is {} itemsType)
        {
            foreach (var nested in EnumerateTypeReferencesUsedIn(itemsType))
            {
                yield return nested;
            }
        }

        if (schemaNode.PrefixItems is {} prefixItemTypes)
        {
            foreach (var nested in prefixItemTypes.SelectMany(EnumerateTypeReferencesUsedIn))
            {
                yield return nested;
            }
        }
    }

    internal static IEnumerable<string> EnumerateVariableReferencesUsedIn(TemplateVariablesEvaluator variablesEvaluator, string variableName)
    {
        if (variablesEvaluator.TryGetUnevaluatedDeclaringToken(variableName) is JToken declaration)
        {
            return EnumerateVariableReferencesUsedIn(variablesEvaluator, declaration);
        }

        if (variablesEvaluator.TryGetUnevaluatedCopyDeclaration(variableName) is not {} copyDeclaration)
        {
            throw new InvalidOperationException($"Unable to locate declaration of '{variableName}' variable.");
        }

        return EnumerateVariableReferencesUsedIn(variablesEvaluator, copyDeclaration.CountToken)
            .Concat(EnumerateVariableReferencesUsedIn(variablesEvaluator, copyDeclaration.ValueItemToken));
    }

    private static IEnumerable<string> EnumerateVariableReferencesUsedIn(TemplateVariablesEvaluator variablesEvaluator, JToken token)
        => ExpressionsEngine.ParseLanguageExpressionsRecursive(token).Values
            .OfType<FunctionExpression>()
            .SelectMany(func => EnumerateVariableReferencesUsedIn(variablesEvaluator, func));

    private static IEnumerable<string> EnumerateVariableReferencesUsedIn(TemplateVariablesEvaluator variablesEvaluator, FunctionExpression func)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(func.Function, VariablesFunctionName))
        {
            return new[]
            {
                variablesEvaluator.Evaluate(func.Parameters.Single()) switch
                {
                    TemplateVariablesEvaluator.EvaluatedValue value => value.Value switch
                    {
                        JValue { Value: string variableName } => variableName,
                        _ => throw new InvalidOperationException("Invalid 'variables' function expression encountered. The argument could not be statically evaluated to a string"),
                    },
                    TemplateVariablesEvaluator.EvaluationException exception => throw exception.Exception,
                    _ => throw new InvalidOperationException("This switch was already exhaustive"),
                },
            };
        }

        return func.Parameters.OfType<FunctionExpression>().SelectMany(param => EnumerateVariableReferencesUsedIn(variablesEvaluator, param))
            .Concat(func.Properties.OfType<FunctionExpression>().SelectMany(prop => EnumerateVariableReferencesUsedIn(variablesEvaluator, prop)));
    }
}
