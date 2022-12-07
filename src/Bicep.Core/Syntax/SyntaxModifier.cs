// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Syntax;

public static class SyntaxModifier
{
    public static ObjectSyntax? TryUpdatePropertyValue(ObjectSyntax @object, string key, Func<SyntaxBase, SyntaxBase> updateFunc)
    {
        if (@object.TryGetPropertyByName(key) is not {} property)
        {
            return null;
        }

        var newProperty = new ObjectPropertySyntax(property.Key, property.Colon, updateFunc(property.Value));

        return new ObjectSyntax(
            @object.OpenBrace,
            @object.Children.Replace(property, newProperty),
            @object.CloseBrace);
    }

    public static ObjectSyntax AddProperty(ObjectSyntax @object, ObjectPropertySyntax newProperty, bool atStart)
    {
        return @object.AddPropertiesWithFormatting(new [] {
            newProperty,
        }, atStart);
    }
}
