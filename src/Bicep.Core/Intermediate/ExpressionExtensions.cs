// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Semantics;

namespace Bicep.Core.Intermediate;

public static class ExpressionExtensions
{
    public static string? TryGetKeyText(this ObjectPropertyExpression property)
        => property.Key switch
        {
            StringLiteralExpression @string => @string.Value,
            _ => null,
        };

    public static bool HasKeyText(this ObjectPropertyExpression property, string key)
        => string.Equals(TryGetKeyText(property), key, LanguageConstants.IdentifierComparison);

    public static ObjectExpression MergeProperty(this ObjectExpression? expression, string propertyName, Expression propertyValue)
    {
        expression ??= new ObjectExpression(null, []);

        var properties = expression.Properties.ToList();
        int matchingIndex = 0;

        while (matchingIndex < properties.Count)
        {
            if (properties[matchingIndex].HasKeyText(propertyName))
            {
                break;
            }

            matchingIndex++;
        }

        if (matchingIndex < properties.Count)
        {
            // If both property values are objects, merge them. Otherwise, replace the matching property value.
            var mergedValue = properties[matchingIndex].Value is ObjectExpression sourceObject && propertyValue is ObjectExpression targetObject
                ? sourceObject.DeepMerge(targetObject)
                : propertyValue;

            properties[matchingIndex] = new ObjectPropertyExpression(
                properties[matchingIndex].SourceSyntax,
                properties[matchingIndex].Key,
                mergedValue);
        }
        else
        {
            properties.Add(new ObjectPropertyExpression(
                null,
                new StringLiteralExpression(null, propertyName),
                propertyValue));
        }

        return new ObjectExpression(expression.SourceSyntax, [.. properties]);
    }

    public static ObjectExpression DeepMerge(this ObjectExpression sourceObject, ObjectExpression targetObject)
    {
        return targetObject.Properties.Aggregate(sourceObject, (mergedObject, property) =>
            property.TryGetKeyText() is string propertyName
                ? mergedObject.MergeProperty(propertyName, property.Value)
                : mergedObject);
    }

    public static bool IsReferencingSecureOutputs(this PropertyAccessExpression expression, SemanticModel model) => (expression.SourceSyntax is null ||
                        (FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(model, expression.SourceSyntax).Any()));
}
