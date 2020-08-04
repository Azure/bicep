using System.Collections.Immutable;

namespace Bicep.Core.Syntax
{
    public static class ObjectSyntaxExtensions
    {
        /// <summary>
        /// Converts a valid object syntax node to a property dictionary. May throw if you provide a node with parse errors.
        /// </summary>
        /// <param name="syntax">The object syntax node</param>
        public static ImmutableDictionary<string, SyntaxBase> ToPropertyDictionary(this ObjectSyntax syntax) =>
            syntax.Properties
                .OfType<ObjectPropertySyntax>()
                .ToImmutableDictionary(p => p.Identifier.IdentifierName, p => p.Value);
    }
}
