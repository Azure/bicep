using System;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public static class LanguageConstants
    {
        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;

        public const string ListSeparator = ", ";

        public static readonly TypeSymbol String = new PrimitiveTypeSymbol("string");
        public static readonly TypeSymbol Object = new PrimitiveTypeSymbol("object");
        public static readonly TypeSymbol Int = new PrimitiveTypeSymbol("int");
        public static readonly TypeSymbol Bool = new PrimitiveTypeSymbol("bool");
        public static readonly TypeSymbol Array = new PrimitiveTypeSymbol("array");

        public static readonly ImmutableSortedDictionary<string, TypeSymbol> ParameterTypes = new[] {String, Object, Int, Bool, Array}.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly string PropertyTypesString = LanguageConstants.ParameterTypes.Keys.ConcatString(ListSeparator);
    }
}
