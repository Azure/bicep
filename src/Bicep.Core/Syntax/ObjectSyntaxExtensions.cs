using System;
using System.Collections.Immutable;

namespace Bicep.Core.Syntax
{
    public static class ObjectSyntaxExtensions
    {
        /// <summary>
        /// Converts a valid object syntax node to a property dictionary. May throw if you provide a node with duplicate properties.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, SyntaxBase> ToPropertyValueDictionary(this ObjectSyntax syntax) =>
            syntax.Properties.ToImmutableDictionary(p => p.GetKeyText(), p => p.Value, LanguageConstants.IdentifierComparer);

        public static ImmutableDictionary<string, ObjectPropertySyntax> ToPropertyDictionary(this ObjectSyntax syntax) =>
            syntax.Properties.ToImmutableDictionary(p => p.GetKeyText(), LanguageConstants.IdentifierComparer);
    }
}
