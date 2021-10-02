// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public static class ObjectSyntaxExtensions
    {
        private const string DefaultIndent = "  ";

        /// <summary>
        /// Converts a syntactically valid object syntax node to a dictionary mapping property name strings to property syntax nodes. Returns the first property in the case of duplicate names.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, ObjectPropertySyntax> ToNamedPropertyDictionary(this ObjectSyntax syntax)
        {
            var dictionary = new Dictionary<string, ObjectPropertySyntax>(LanguageConstants.IdentifierComparer);
            foreach (var property in syntax.Properties)
            {
                if (property.TryGetKeyText() is { } key && !dictionary.ContainsKey(key))
                {
                    dictionary[key] = property;
                }
            }

            return dictionary.ToImmutableDictionary(LanguageConstants.IdentifierComparer);
        }

        /// <summary>
        /// Converts a syntactically valid object syntax node to a dictionary mapping property name strings to property syntax node values. Returns the first property value in the case of duplicate names.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, SyntaxBase> ToNamedPropertyValueDictionary(this ObjectSyntax syntax)
            => ToNamedPropertyDictionary(syntax).ToImmutableDictionary(x => x.Key, x => x.Value.Value, LanguageConstants.IdentifierComparer);

        /// <summary>
        /// Returns the specified property by name on any valid or invalid object syntax node if there is exactly one property by that name.
        /// Returns null if the property does not exist or if multiple properties by that name exist. This method is intended for a single
        /// one-off property lookup and avoids allocation of a dictionary. If you need to make multiple look ups, use another extension in this class.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        /// <param name="propertyName">The property name</param>
        public static ObjectPropertySyntax? SafeGetPropertyByName(this ObjectSyntax syntax, string propertyName)
        {
            ObjectPropertySyntax? result = null;

            var matchingValidProperties = syntax.Properties
                .Where(p => p.TryGetKeyText() is { } validName && string.Equals(validName, propertyName, LanguageConstants.IdentifierComparison));

            foreach (var property in matchingValidProperties)
            {
                if (result == null)
                {
                    // we have not yet seen a name match
                    // store it
                    result = property;
                }
                else
                {
                    // we have already seen a name match, which means we have a duplicate property
                    // no point proceeding any further
                    return null;
                }
            }

            return result;
        }

        public static ObjectPropertySyntax? SafeGetPropertyByNameRecursive(this ObjectSyntax syntax, IList<string> propertyAccesses)
        {
            var currentSyntax = syntax;
            for (int i = 0; i < propertyAccesses.Count; i++)
            {
                if (currentSyntax.SafeGetPropertyByName(propertyAccesses[i]) is ObjectPropertySyntax propertySyntax)
                {
                    // we have found our last property access
                    if (i == propertyAccesses.Count-1)
                    {
                        return propertySyntax;
                    }
                    // we have successfully gone one level deeper into the object
                    else if (propertySyntax.Value is ObjectSyntax propertyObjectSyntax)
                    {
                        currentSyntax = propertyObjectSyntax;
                    }
                    // our path isn't fully traversed yet and we hit a terminal value (not an object)
                    else
                    {
                        break;
                    }
                }
                // we couldn't even find this property on the object
                else
                {
                   break;
                }
            }
            return null;
        }

        public static ObjectSyntax MergeProperty(this ObjectSyntax? syntax, string propertyName, string propertyValue) =>
            syntax.MergeProperty(propertyName, SyntaxFactory.CreateStringLiteral(propertyValue));

        public static ObjectSyntax MergeProperty(this ObjectSyntax? syntax, string propertyName, SyntaxBase propertyValue)
        {
            if (syntax == null)
            {
                return SyntaxFactory.CreateObject(SyntaxFactory.CreateObjectProperty(propertyName, propertyValue).AsEnumerable());
            }

            var properties = syntax.Properties.ToList();
            int matchingIndex = 0;

            while (matchingIndex < properties.Count)
            {
                if (string.Equals(properties[matchingIndex].TryGetKeyText(), propertyName, LanguageConstants.IdentifierComparison))
                {
                    break;
                }

                matchingIndex++;
            }

            if (matchingIndex < properties.Count)
            {
                // If both property values are objects, merge them. Otherwise, replace the matching property value.
                SyntaxBase mergedValue = properties[matchingIndex].Value is ObjectSyntax sourceObject && propertyValue is ObjectSyntax targetObject
                    ? sourceObject.DeepMerge(targetObject)
                    : propertyValue;

                properties[matchingIndex] = SyntaxFactory.CreateObjectProperty(propertyName, mergedValue);
            }
            else
            {
                properties.Add(SyntaxFactory.CreateObjectProperty(propertyName, propertyValue));
            }

            return SyntaxFactory.CreateObject(properties);
        }

        public static ObjectSyntax DeepMerge(this ObjectSyntax? sourceObject, ObjectSyntax targetObject)
        {
            if (sourceObject == null)
            {
                return targetObject;
            }

            return targetObject.Properties.Aggregate(sourceObject, (mergedObject, property) =>
                property.TryGetKeyText() is string propertyName
                    ? mergedObject.MergeProperty(propertyName, property.Value)
                    : mergedObject);
        }

        public static ObjectSyntax AddChildrenWithFormatting(this ObjectSyntax objectSyntax, IEnumerable<SyntaxBase> newChildren)
        {
            bool IsEmptyLine(Token token)
            {
                if (token.Type != TokenType.NewLine)
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

            var children = new List<SyntaxBase>(objectSyntax.Children);

            // Remove trailing empty lines
            Token? lastNode = null;
            while (children.Count > 0 && children[^1] is Token token && IsEmptyLine(token))
            {
                lastNode ??= token;
                children.Remove(token);
            }

            var indent = objectSyntax.GetBodyIndentation();

            foreach (var newChild in newChildren)
            {
                children.Add(SyntaxFactory.CreateNewLineWithIndent(indent));
                children.Add(newChild);
            }

            children.Add(new Token(
                TokenType.NewLine,
                SyntaxFactory.EmptySpan,
                Environment.NewLine,
                lastNode?.LeadingTrivia ?? ImmutableArray<SyntaxTrivia>.Empty,
                lastNode?.TrailingTrivia ?? ImmutableArray<SyntaxTrivia>.Empty));

            return new ObjectSyntax(objectSyntax.OpenBrace, children, objectSyntax.CloseBrace);
        }

        public static string GetBodyIndentation(this ObjectSyntax sourceObject)
        {
            string? GetIndent(SyntaxBase syntax)
            {
                if (syntax is Token { Type: TokenType.NewLine, TrailingTrivia: { Length: 1 } leadingTrivia } &&
                    leadingTrivia[0].Type == SyntaxTriviaType.Whitespace)
                {
                    return leadingTrivia[0].Text;
                }

                return null;
            }

            var children = sourceObject.Children;

            // Try to find an existing indented child
            if (children.Length > 0)
            {
                for (var index = 0; index < children.Length - 1; index++)
                {
                    var child = children[index];
                    if (GetIndent(child) is string indent)
                    {
                        return indent;
                    }
                }
            }

            // Try to guess from the last newline
            if (children.Length > 0 &&
                GetIndent(children[^1]) is string lastIndent)
            {
                return lastIndent + DefaultIndent;
            }

            return DefaultIndent;
        }
    }
}
