using System;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.SemanticModel;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public class LanguageConstants
    {
        public const int MaxIdentifierLength = 255;

        public static readonly TypeSymbol String = new PrimitiveType("string");
        public static readonly TypeSymbol Object = new PrimitiveType("object");
        public static readonly TypeSymbol Int = new PrimitiveType("int");
        public static readonly TypeSymbol Bool = new PrimitiveType("bool");
        public static readonly TypeSymbol Array = new PrimitiveType("array");

        public static readonly ImmutableSortedDictionary<string, TypeSymbol> ParameterTypes = new[] {String, Object, Int, Bool, Array}.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly string PropertyTypesString = LanguageConstants.ParameterTypes.Keys.ConcatString(", ");
    }
}
