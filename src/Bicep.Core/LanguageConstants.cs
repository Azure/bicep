using System;
using System.Collections.Immutable;
using Bicep.Core.Extensions;

namespace Bicep.Core
{
    class LanguageConstants
    {
        public const int MaxIdentifierLength = 255;

       public const string StringType = "string";
       public const string ObjectType = "object";
       public const string IntegerType = "int";
       public const string BooleanType = "bool";
       public const string ArrayType = "array";

        public static readonly ImmutableSortedSet<string> PropertyTypes = new[]
        {
            StringType,
            ObjectType,
            IntegerType,
            BooleanType,
            ArrayType
        }.ToImmutableSortedSet(StringComparer.Ordinal);

        public static readonly string PropertyTypesString = LanguageConstants.PropertyTypes.ConcatString(", ");
    }
}
