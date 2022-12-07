// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Bicep.Core.Parsing;

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

    public static ObjectSyntax RemoveProperty(ObjectSyntax @object, ObjectPropertySyntax property)
    {
        var endIndex = @object.Children.IndexOf(property);
        var prevChild = @object.Children
            .Where((x, i) => i < endIndex && x is not Token { Type: TokenType.NewLine} && x is not Token { Type: TokenType.Comma })
            .LastOrDefault();
        var startIndex = prevChild is null ? 0 : @object.Children.IndexOf(prevChild);

        var newChildren = @object.Children.Where((x, i) => i <= startIndex || i > endIndex);

        return new ObjectSyntax(@object.OpenBrace, newChildren, @object.CloseBrace);
    }

    public static ArraySyntax RemoveItem(ArraySyntax array, ArrayItemSyntax item)
    {
        var endIndex = array.Children.IndexOf(item);
        var prevChild = array.Children
            .Where((x, i) => i < endIndex && x is not Token { Type: TokenType.NewLine} && x is not Token { Type: TokenType.Comma })
            .LastOrDefault();
        var startIndex = prevChild is null ? 0 : array.Children.IndexOf(prevChild);

        var newChildren = array.Children.Where((x, i) => i <= startIndex || i > endIndex);

        return new ArraySyntax(array.OpenBracket, newChildren, array.CloseBracket);
    }
}
