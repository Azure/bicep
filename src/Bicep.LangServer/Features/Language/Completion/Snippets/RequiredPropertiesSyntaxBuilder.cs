// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.LanguageServer.Features.Language.Completion.Snippets;

internal static class RequiredPropertiesSyntaxBuilder
{
    private static readonly ImmutableArray<string> PropertiesSortPreferenceList = ["scope", "parent", "name", "location", "zones", "sku", "kind", "scale", "plan", "identity", "tags", "properties", "dependsOn"];

    public static ObjectSyntax Build(
        ObjectType objectType,
        Func<NamedTypeProperty, bool, SyntaxBase> createUnresolvedValue,
        string? discriminatedObjectKey = null)
        => Build(objectType, createUnresolvedValue, discriminatedObjectKey, isTopLevel: true);

    private static ObjectSyntax Build(
        ObjectType objectType,
        Func<NamedTypeProperty, bool, SyntaxBase> createUnresolvedValue,
        string? discriminatedObjectKey,
        bool isTopLevel)
    {
        var typeProperties = objectType.Properties.Values.OrderBy(x =>
            PropertiesSortPreferenceList.IndexOf(x.Name) switch
            {
                -1 => int.MaxValue,
                int index => index,
            })
            .Where(TypeHelper.IsRequired);

        var objectProperties = new List<ObjectPropertySyntax>();
        foreach (var typeProperty in typeProperties)
        {
            // DFS keeps unresolved values in the same order as their properties.
            objectProperties.Add(BuildProperty(typeProperty, createUnresolvedValue, discriminatedObjectKey, isTopLevel));
        }

        return SyntaxFactory.CreateObject(objectProperties);
    }

    public static ObjectSyntax Build(
        DiscriminatedObjectType discriminatedObjectType,
        Func<SyntaxBase> createUnresolvedValue) =>
        SyntaxFactory.CreateObject(
        [
            SyntaxFactory.CreateObjectProperty(
                discriminatedObjectType.DiscriminatorKey,
                createUnresolvedValue()),
        ]);

    private static ObjectPropertySyntax BuildProperty(
        NamedTypeProperty typeProperty,
        Func<NamedTypeProperty, bool, SyntaxBase> createUnresolvedValue,
        string? discriminatedObjectKey,
        bool isTopLevel)
    {
        var valueType = typeProperty.TypeReference.Type;
        if (valueType is ObjectType objectType)
        {
            return SyntaxFactory.CreateObjectProperty(
                typeProperty.Name,
                Build(objectType, createUnresolvedValue, discriminatedObjectKey: null, isTopLevel: false));
        }

        if (discriminatedObjectKey is { } &&
            valueType is StringLiteralType stringLiteralType &&
            stringLiteralType.Name == discriminatedObjectKey)
        {
            return SyntaxFactory.CreateObjectProperty(
                typeProperty.Name,
                SyntaxFactory.CreateStringLiteral(stringLiteralType.RawStringValue));
        }

        return SyntaxFactory.CreateObjectProperty(
            typeProperty.Name,
            createUnresolvedValue(typeProperty, isTopLevel));
    }
}
