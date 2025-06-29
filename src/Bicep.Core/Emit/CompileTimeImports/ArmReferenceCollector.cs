// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Azure.Deployments.Templates.Extensions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.SourceGraph;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal partial class ArmReferenceCollector
{
    private const string VariablesFunctionName = "variables";
    private readonly SchemaValidationContext schemaContext;
    private readonly TemplateVariablesEvaluator variablesEvaluator;
    private readonly ImmutableDictionary<string, TemplateFunction> functionsByFullyQualifiedName;

    internal ArmReferenceCollector(ArmTemplateFile templateFile)
    {
        if (templateFile.Template is not { } template)
        {
            throw new InvalidOperationException($"Source template of {templateFile.FileHandle.Uri} is not valid");
        }

        schemaContext = SchemaValidationContext.ForTemplate(template);
        variablesEvaluator = new(template);
        functionsByFullyQualifiedName = template.GetFunctionDefinitions()
            .ToImmutableDictionary(fd => fd.Key, fd => fd.Function, StringComparer.OrdinalIgnoreCase);
    }

    internal IEnumerable<ArmIdentifier> EnumerateReferencesUsedInDefinitionOf(ArmIdentifier armReference) => armReference.SymbolType switch
    {
        ArmSymbolType.Type => EnumerateReferencesUsedIn(ArmTemplateHelpers.DereferenceArmType(schemaContext, armReference.Identifier)),
        ArmSymbolType.Variable => EnumerateReferencesUsedInVariable(armReference.Identifier),
        ArmSymbolType.Function => EnumerateReferencesUsedInFunction(functionsByFullyQualifiedName[armReference.Identifier]),
        _ => throw new UnreachableException(),
    };

    private IEnumerable<ArmIdentifier> EnumerateReferencesUsedInVariable(string variableName)
    {
        if (variablesEvaluator.TryGetUnevaluatedDeclaringToken(variableName) is JToken declaration)
        {
            return EnumerateReferencesUsedIn(declaration);
        }

        if (variablesEvaluator.TryGetUnevaluatedCopyDeclaration(variableName) is not { } copyDeclaration)
        {
            throw new InvalidOperationException($"Unable to locate declaration of '{variableName}' variable.");
        }

        return EnumerateReferencesUsedIn(copyDeclaration.CountToken)
            .Concat(EnumerateReferencesUsedIn(copyDeclaration.ValueItemToken));
    }

    private IEnumerable<ArmIdentifier> EnumerateReferencesUsedIn(JToken token)
        => ExpressionsEngine.ParseLanguageExpressionsRecursive(token).Values
            .OfType<FunctionExpression>()
            .SelectMany(EnumerateReferencesUsedIn);

    private IEnumerable<ArmIdentifier> EnumerateReferencesUsedIn(FunctionExpression func)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(func.Function, VariablesFunctionName))
        {
            yield return variablesEvaluator.Evaluate(func.Parameters.Single()) switch
            {
                JValue { Value: string variableName } => new(ArmSymbolType.Variable, variableName),
                _ => throw new InvalidOperationException("Invalid 'variables' function expression encountered. The argument could not be statically evaluated to a string"),
            };
        }

        // Any function name with a '.' in it is a user-defined function invocation
        if (func.Function.IndexOf('.') > -1)
        {
            yield return new(ArmSymbolType.Function, func.Function);
        }

        foreach (var referenceUsedInParameter in func.Parameters.OfType<FunctionExpression>().SelectMany(EnumerateReferencesUsedIn))
        {
            yield return referenceUsedInParameter;
        }

        foreach (var referenceUsedInProperty in func.Properties.OfType<FunctionExpression>().SelectMany(EnumerateReferencesUsedIn))
        {
            yield return referenceUsedInProperty;
        }
    }

    private IEnumerable<ArmIdentifier> EnumerateReferencesUsedInFunction(TemplateFunction function)
        => function.Parameters.SelectMany(EnumerateReferencesUsedIn)
            .Concat(EnumerateReferencesUsedIn(function.Output.Value.Value))
            .Concat(EnumerateReferencesUsedIn(function.Output));

    private static IEnumerable<ArmIdentifier> EnumerateReferencesUsedIn(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.Ref?.Value is string @ref)
        {
            yield return new(ArmSymbolType.Type, @ref);
        }

        if (schemaNode.AdditionalProperties?.SchemaNode is { } addlPropertiesType)
        {
            foreach (var nested in EnumerateReferencesUsedIn(addlPropertiesType))
            {
                yield return nested;
            }
        }

        if (schemaNode.Properties is { } properties)
        {
            foreach (var nested in properties.Values.SelectMany(EnumerateReferencesUsedIn))
            {
                yield return nested;
            }
        }

        if (schemaNode.Items?.SchemaNode is { } itemsType)
        {
            foreach (var nested in EnumerateReferencesUsedIn(itemsType))
            {
                yield return nested;
            }
        }

        if (schemaNode.PrefixItems is { } prefixItemTypes)
        {
            foreach (var nested in prefixItemTypes.SelectMany(EnumerateReferencesUsedIn))
            {
                yield return nested;
            }
        }

        if (schemaNode.Discriminator is { } discriminatorConstraint)
        {
            foreach (var nested in discriminatorConstraint.Mapping.Values.SelectMany(EnumerateReferencesUsedIn))
            {
                yield return nested;
            }
        }
    }
}
