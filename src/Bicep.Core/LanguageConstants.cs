using System;
using System.Collections.Immutable;
using Bicep.Core.Extensions;

namespace Bicep.Core
{
    class LanguageConstants
    {
        public const int MaxIdentifierLength = 255;

        public static readonly ImmutableSortedSet<string> PropertyTypes = new[]
        {
            "string",
            "object",
            "int",
            "bool",
            "array"
        }.ToImmutableSortedSet(StringComparer.Ordinal);

        public static readonly string PropertyTypesString = LanguageConstants.PropertyTypes.ConcatString(", ");
    }
}
