// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;

namespace Bicep.Core.Syntax
{
    public static class ObjectSyntaxExtensions
    {
        /// <summary>
        /// Converts a syntactically valid object syntax node to a hashset of property name strings. Will throw if you provide a node with duplicate properties.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableHashSet<string> ToKnownPropertyNames(this ObjectSyntax syntax) => 
            syntax.Properties.Select(p => p.TryGetKeyText()).ToImmutableHashSetExcludingNull(LanguageConstants.IdentifierComparer);

        /// <summary>
        /// Converts a syntactically valid object syntax node to a dictionary mapping property name strings to property value expressions. Will throw if you provide a node with duplicate properties.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, SyntaxBase> ToKnownPropertyValueDictionary(this ObjectSyntax syntax) =>
            syntax.Properties.ToImmutableDictionaryExcludingNull(p => p.TryGetKeyText(), p => p.Value, LanguageConstants.IdentifierComparer);

        /// <summary>
        /// Converts a syntactically valid object syntax node to a dictionary mapping property name strings to property syntax nodes. Will throw if you provide a node with duplicate properties.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, ObjectPropertySyntax> ToNamedPropertyDictionary(this ObjectSyntax syntax) =>	
            syntax.Properties.ToImmutableDictionaryExcludingNull(p => p.TryGetKeyText(), LanguageConstants.IdentifierComparer);

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
    }
}