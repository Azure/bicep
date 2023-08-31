// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Syntax;

public class SyntaxModifier
{
    public static ObjectSyntax? TryUpdatePropertyValue(ObjectSyntax @object, string key, Func<SyntaxBase, SyntaxBase> updateFunc)
    {
        if (@object.TryGetPropertyByName(key) is not { } property)
        {
            return null;
        }

        var newProperty = new ObjectPropertySyntax(property.Key, property.Colon, updateFunc(property.Value));

        return new ObjectSyntax(
            @object.OpenBrace,
            @object.Children.Replace(property, newProperty),
            @object.CloseBrace);
    }

    public static ObjectSyntax? TryAddProperty(ObjectSyntax @object, ObjectPropertySyntax newProperty, IDiagnosticLookup parsingErrorLookup, int? atIndex = null)
        => TryAddProperties(@object, newProperty.AsEnumerable(), parsingErrorLookup, atIndex);

    public static ObjectSyntax? TryAddProperties(ObjectSyntax @object, IEnumerable<ObjectPropertySyntax> newProperties, IDiagnosticLookup parsingErrorLookup, int? atIndex = null)
    {
        if (parsingErrorLookup.Contains(@object))
        {
            return null;
        }

        var indent = @object.GetBodyIndentation();
        SyntaxBase GetSeparator(Token? separatorToCopy) => separatorToCopy switch
        {
            Token { Type: TokenType.NewLine } => SyntaxFactory.CreateNewLineWithIndent(indent),
            Token { Type: TokenType.Comma } => SyntaxFactory.GetCommaToken(trailingTrivia: SyntaxFactory.SingleSpaceTrivia),
            // if we don't have a child then we're either at the start/end of a single-line object - default to comma
            _ => SyntaxFactory.GetCommaToken(trailingTrivia: SyntaxFactory.SingleSpaceTrivia),
        };

        // default to the end of the object
        var newPropertyIndex = atIndex ?? @object.Properties.Count();

        int spliceIndex;
        IEnumerable<SyntaxBase> childrenToAdd;

        if (@object.Properties.Skip(newPropertyIndex).FirstOrDefault() is { } propertyToAddBefore)
        {
            // we're inserting before an existing propert
            spliceIndex = @object.Children.IndexOf(propertyToAddBefore);
            var prevSeparator = spliceIndex > 0 ? @object.Children[spliceIndex - 1] as Token : null;

            childrenToAdd = newProperties.SelectMany(x => new[] { x, GetSeparator(prevSeparator) }).ToArray();
        }
        else
        {
            // we're at the end (also at the beginning if there are 0 existing properties)
            var lastSeparator = @object.Children.LastOrDefault() as Token;
            spliceIndex = lastSeparator is not null ? @object.Children.IndexOf(lastSeparator) : @object.Children.Length;

            childrenToAdd = newProperties.SelectMany(x => new[] { GetSeparator(lastSeparator), x }).ToArray();
        }

        var newChildren = @object.Children.Take(spliceIndex)
            .Concat(childrenToAdd)
            .Concat(@object.Children.Skip(spliceIndex));

        return new ObjectSyntax(@object.OpenBrace, CollapseLeadingAndTrailingNewlines(newChildren), @object.CloseBrace);
    }

    public static ObjectSyntax? TryRemoveProperty(ObjectSyntax @object, ObjectPropertySyntax property, IDiagnosticLookup parsingErrorLookup)
    {
        if (parsingErrorLookup.Contains(@object))
        {
            return null;
        }

        int startIndex;
        int endIndex;
        if (property == @object.Properties.First())
        {
            // we're dealing with the first item
            startIndex = @object.Children.IndexOf(property);
            // remove item + next separator
            endIndex = startIndex + 2;
        }
        else
        {
            startIndex = @object.Children.IndexOf(property) - 1;
            // remove prev separator + item
            endIndex = startIndex + 2;
        }

        var newChildren = @object.Children.Take(startIndex)
            .Concat(@object.Children.Skip(endIndex));

        return new(@object.OpenBrace, CollapseLeadingAndTrailingNewlines(newChildren), @object.CloseBrace);
    }

    public static ArraySyntax? TryRemoveItem(ArraySyntax array, ArrayItemSyntax item, IDiagnosticLookup parsingErrorLookup)
    {
        if (parsingErrorLookup.Contains(array))
        {
            return null;
        }

        int startIndex;
        int endIndex;
        if (item == array.Items.First())
        {
            // we're dealing with the first item
            startIndex = array.Children.IndexOf(item);
            // remove item + next separator
            endIndex = startIndex + 2;
        }
        else
        {
            startIndex = array.Children.IndexOf(item) - 1;
            // remove prev separator + item
            endIndex = startIndex + 2;
        }

        var newChildren = array.Children.Take(startIndex)
            .Concat(array.Children.Skip(endIndex));

        return new(array.OpenBracket, CollapseLeadingAndTrailingNewlines(newChildren), array.CloseBracket);
    }

    private static IEnumerable<SyntaxBase> CollapseLeadingAndTrailingNewlines(IEnumerable<SyntaxBase> syntaxEnumerable)
    {
        var syntaxList = syntaxEnumerable.ToArray();
        var emptyLeadingTokens = syntaxList.TakeWhile(IsEmptyLine).ToArray();
        var emptyTrailingTokens = syntaxList.Reverse().TakeWhile(IsEmptyLine).ToArray();

        if (emptyLeadingTokens.FirstOrDefault() is Token firstToken)
        {
            yield return SyntaxFactory.GetNewlineToken(firstToken.LeadingTrivia, firstToken.TrailingTrivia);
        }

        for (var i = emptyLeadingTokens.Length; i < syntaxList.Length - emptyTrailingTokens.Length; i++)
        {
            yield return syntaxList[i];
        }

        if (emptyTrailingTokens.FirstOrDefault() is Token lastToken)
        {
            yield return SyntaxFactory.GetNewlineToken(lastToken.LeadingTrivia, lastToken.TrailingTrivia);
        }
    }

    private static bool IsEmptyLine(SyntaxBase syntax)
    {
        if (syntax is not Token { Type: TokenType.NewLine } token)
        {
            return false;
        }

        foreach (var trivia in token.LeadingTrivia)
        {
            if (trivia.Type != SyntaxTriviaType.Whitespace)
            {
                return false;
            }
        }

        foreach (var trivia in token.TrailingTrivia)
        {
            if (trivia.Type != SyntaxTriviaType.Whitespace)
            {
                return false;
            }
        }

        return true;
    }
}
