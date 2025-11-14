// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;
using System.Text;
using Azure.Deployments.Expression.Extensions;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Intermediate;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using static Bicep.Core.Semantics.FunctionOverloadBuilder;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class SystemNamespaceType
    {
        private record LoadTextContentResult(IOUri FileUri, string Content);
        private record struct LoadDirectoryFileInfoResult(string RelativePath, string BaseName, string Extension);

        public const string BuiltInName = "sys";
        public const long UniqueStringHashLength = 13;
        private const string ConcatDescription = "Combines multiple arrays and returns the concatenated array, or combines multiple string values and returns the concatenated string.";
        private const string TakeDescription = "Returns an array or string. An array has the specified number of elements from the start of the array. A string has the specified number of characters from the start of the string.";
        private const string SkipDescription = "Returns a string with all the characters after the specified number of characters, or an array with all the elements after the specified number of elements.";
        private const string ContainsDescription = "Checks whether an array contains a value, an object contains a key, or a string contains a substring. The string comparison is case-sensitive. However, when testing if an object contains a key, the comparison is case-insensitive.";
        private const string IntersectionDescription = "Returns a single array or object with the common elements from the parameters.";
        private const string UnionDescription = "Returns a single array or object with all elements from the parameters. Duplicate values or keys are only included once.";
        private const string FirstDescription = "Returns the first element of the array, or first character of the string.";
        private const string LastDescription = "Returns the last element of the array, or last character of the string.";
        private const string MinDescription = "Returns the minimum value from an array of integers or a comma-separated list of integers.";
        private const string MaxDescription = "Returns the maximum value from an array of integers or a comma-separated list of integers.";
        private const long GuidLength = 36;

        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepExtensionName: BuiltInName,
            ConfigurationType: null,
            TemplateExtensionName: "System",
            TemplateExtensionVersion: "1.0.0");

        private delegate bool VisibilityDelegate(IFeatureProvider featureProvider, BicepSourceFileKind sourceFileKind);

        private record NamespaceValue<T>(T Value, VisibilityDelegate IsVisible);

        private static readonly ImmutableArray<NamespaceValue<NamedTypeProperty>> AmbientSymbols = [.. GetSystemAmbientSymbols()];

        private static IEnumerable<NamespaceValue<FunctionOverload>> GetSystemOverloads(IFeatureProvider featureProvider)
        {
            static IEnumerable<FunctionOverload> GetAlwaysPermittedOverloads()
            {
                yield return new FunctionOverloadBuilder(LanguageConstants.AnyFunction)
                    .WithReturnType(LanguageConstants.Any)
                    .WithGenericDescription("Converts the specified value to the `any` type.")
                    .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to `any` type")
                    .WithEvaluator(expression => expression.Parameters[0])
                    .Build();

                yield return new FunctionOverloadBuilder("concat")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("concat", (_, _, _, argumentTypes) =>
                    {
                        if (argumentTypes.All(t => t is TupleType))
                        {
                            return new(new TupleType([.. argumentTypes.OfType<TupleType>().SelectMany(tt => tt.Items)], default));
                        }

                        BigInteger minLength = 0;
                        BigInteger? maxLength = null;
                        var itemTypes = new ITypeReference[argumentTypes.Length];

                        for (int i = 0; i < argumentTypes.Length; i++)
                        {
                            if (argumentTypes[i] is not ArrayType arr)
                            {
                                return new(LanguageConstants.Array);
                            }

                            itemTypes[i] = arr.Item;

                            minLength += arr.MinLength ?? 0;

                            if (i == 0)
                            {
                                maxLength = arr.MaxLength;
                            }
                            else if (maxLength.HasValue && arr.MaxLength.HasValue)
                            {
                                maxLength = maxLength.Value + arr.MaxLength.Value;
                            }
                            else
                            {
                                maxLength = null;
                            }
                        }

                        return new(TypeFactory.CreateArrayType(TypeHelper.CreateTypeUnion(itemTypes),
                            minLength switch
                            {
                                var zero when zero <= 0 => null,
                                _ => (long)BigInteger.Min(minLength, long.MaxValue),
                            },
                            maxLength switch
                            {
                                BigInteger bi => (long)BigInteger.Min(bi, long.MaxValue),
                                _ => null,
                            },
                            TypeSymbolValidationFlags.Default));
                    }),
                    LanguageConstants.Array)
                    .WithGenericDescription(ConcatDescription)
                    .WithDescription("Combines multiple arrays and returns the concatenated array.")
                    .WithVariableParameter("arg", LanguageConstants.Array, minimumCount: 1, "The array for concatenation")
                    .Build();

                yield return new FunctionOverloadBuilder("cidrSubnet")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Splits the specified IP address range in CIDR notation into subnets with a new CIDR value and returns the IP address range of the subnet with the specified index.")
                    .WithRequiredParameter("network", LanguageConstants.String, "String containing an IP address range to convert in CIDR notation.")
                    .WithRequiredParameter("cidr", LanguageConstants.Int, "An integer representing the CIDR to be used to subnet. This value should be equal or larger than the CIDR value in the network parameter.")
                    .WithRequiredParameter("subnetIndex", LanguageConstants.Int, "Index of the desired subnet IP address range to return.")
                    .Build();

                yield return new FunctionOverloadBuilder("cidrHost")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Calculates the usable IP address of the host with the specified index on the specified IP address range in CIDR notation.")
                    .WithRequiredParameter("network", LanguageConstants.String, "String containing an ip network to convert (must be correct networking format)")
                    .WithRequiredParameter("hostIndex", LanguageConstants.Int, "The index of the host IP address to return.")
                    .Build();

                yield return new FunctionOverloadBuilder("parseCidr")
                    .WithReturnType(GetParseCidrReturnType())
                    .WithGenericDescription("Parses an IP address range in CIDR notation to get various properties of the address range.")
                    .WithRequiredParameter("network", LanguageConstants.String, "String in CIDR notation containing an IP address range to be converted.")
                    .Build();

                yield return new FunctionOverloadBuilder("buildUri")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Constructs a URI from specified components and returns an object with these components.")
                    .WithRequiredParameter("components", GetParseOrBuildUriReturnType(), "An object containing URI components such as scheme, host, port, path, and query.")
                    .Build();

                yield return new FunctionOverloadBuilder("parseUri")
                    .WithReturnType(GetParseOrBuildUriReturnType())
                    .WithGenericDescription("Parses a URI string into its components (scheme, host, port, path, query).")
                    .WithRequiredParameter("baseUrl", LanguageConstants.String, "The complete URI to parse.")
                    .Build();

                yield return new FunctionOverloadBuilder("concat")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription(ConcatDescription)
                    .WithDescription("Combines multiple string, integer, or boolean values and returns them as a concatenated string.")
                    .WithVariableParameter("arg", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool), minimumCount: 1, "The string, int, or boolean value for concatenation")
                    .Build();

                yield return new FunctionOverloadBuilder("format")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("format", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Creates a formatted string from input values.")
                    .WithRequiredParameter("formatString", LanguageConstants.String, "The composite format string.")
                    .WithVariableParameter("arg", LanguageConstants.Any, minimumCount: 0, "The value to include in the formatted string.")
                    .Build();

                yield return new FunctionOverloadBuilder("base64")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("base64", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Returns the base64 representation of the input string.")
                    .WithRequiredParameter("inputString", LanguageConstants.String, "The value to return as a base64 representation.")
                    .Build();

                yield return new FunctionOverloadBuilder("padLeft")
                    .WithReturnResultBuilder(
                        TryDeriveLiteralReturnType("padLeft", (_, _, _, argumentTypes) =>
                        {
                            (long? minLength, long? maxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(argumentTypes[0]);

                            if (argumentTypes[1] is not IntegerLiteralType literalLength)
                            {
                                return new(TypeFactory.CreateStringType(minLength: minLength, validationFlags: argumentTypes[0].ValidationFlags));
                            }

                            return new(TypeFactory.CreateStringType(
                                minLength.HasValue ? Math.Max(minLength.Value, literalLength.Value) : null,
                                maxLength.HasValue ? Math.Max(maxLength.Value, literalLength.Value) : null,
                                validationFlags: argumentTypes[0].ValidationFlags));
                        }),
                        LanguageConstants.String)
                    .WithGenericDescription("Returns a right-aligned string by adding characters to the left until reaching the total specified length.")
                    .WithRequiredParameter("valueToPad", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), "The value to right-align.")
                    .WithRequiredParameter("totalLength", LanguageConstants.Int, "The total number of characters in the returned string.")
                    .WithOptionalParameter("paddingCharacter", LanguageConstants.String, "The character to use for left-padding until the total length is reached. The default value is a space.")
                    .Build();

                yield return new FunctionOverloadBuilder("replace")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("replace", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Returns a new string with all instances of one string replaced by another string.")
                    .WithRequiredParameter("originalString", LanguageConstants.String, "The original string.")
                    .WithRequiredParameter("oldString", LanguageConstants.String, "The string to be removed from the original string.")
                    .WithRequiredParameter("newString", LanguageConstants.String, "The string to add in place of the removed string.")
                    .Build();

                yield return new FunctionOverloadBuilder("toLower")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("toLower", (_, _, _, argumentTypes) => new(argumentTypes.FirstOrDefault() is StringType @string ? @string : LanguageConstants.String)), LanguageConstants.String)
                    .WithGenericDescription("Converts the specified string to lower case.")
                    .WithRequiredParameter("stringToChange", LanguageConstants.String, "The value to convert to lower case.")
                    .Build();

                yield return new FunctionOverloadBuilder("toUpper")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("toUpper", (_, _, _, argumentTypes) => new(argumentTypes.FirstOrDefault() is StringType @string ? @string : LanguageConstants.String)), LanguageConstants.String)
                    .WithGenericDescription("Converts the specified string to upper case.")
                    .WithRequiredParameter("stringToChange", LanguageConstants.String, "The value to convert to upper case.")
                    .Build();


                static int MinLength(ObjectType @object) =>
                    @object.Properties.Where(kvp => kvp.Value.Flags.HasFlag(TypePropertyFlags.Required) && TypeHelper.TryRemoveNullability(kvp.Value.TypeReference.Type) is null).Count();

                static int? MaxLength(ObjectType @object) => @object.AdditionalProperties is null ? @object.Properties.Count : null;

                yield return new FunctionOverloadBuilder("length")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("length", (_, _, _, argumentTypes) => new(argumentTypes.FirstOrDefault() switch
                    {
                        StringType @string => TypeFactory.CreateIntegerType(@string.MinLength ?? 0, @string.MaxLength, @string.ValidationFlags),
                        ObjectType @object => TypeFactory.CreateIntegerType(MinLength(@object), MaxLength(@object), @object.ValidationFlags),
                        DiscriminatedObjectType discriminatedObject => TypeFactory.CreateIntegerType(
                            minValue: discriminatedObject.UnionMembersByKey.Values.Min(MinLength),
                            maxValue: discriminatedObject.UnionMembersByKey.Values
                                .Aggregate((long?)0, (acc, memberObject) => acc.HasValue && MaxLength(memberObject) is int maxLength
                                    ? Math.Max(acc.Value, maxLength) : null)),
                        _ => LanguageConstants.Int,
                    })), LanguageConstants.Int)
                    .WithGenericDescription("Returns the number of characters in a string, elements in an array, or root-level properties in an object.")
                    .WithRequiredParameter("arg", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Object), "The string to use for getting the number of characters or the object to use for getting the number of root-level properties.")
                    .Build();

                yield return new FunctionOverloadBuilder("length")
                    .WithReturnResultBuilder(
                        (_, _, _, argumentTypes) => (argumentTypes.IsEmpty ? null : argumentTypes[0]) switch
                        {
                            TupleType tupleType => new(TypeFactory.CreateIntegerLiteralType(tupleType.Items.Length)),
                            ArrayType arrayType => new(TypeFactory.CreateIntegerType(arrayType.MinLength ?? 0, arrayType.MaxLength)),
                            _ => new(LanguageConstants.Int),
                        },
                        LanguageConstants.Int)
                    .WithGenericDescription("Returns the number of characters in a string, elements in an array, or root-level properties in an object.")
                    .WithRequiredParameter("arg", LanguageConstants.Array, "The array to use for getting the number of elements.")
                    .Build();

                yield return new FunctionOverloadBuilder("split")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("split", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default, minLength: 1)), new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default))
                    .WithGenericDescription("Returns an array of strings that contains the substrings of the input string that are delimited by the specified delimiters.")
                    .WithRequiredParameter("inputString", LanguageConstants.String, "The string to split.")
                    .WithRequiredParameter("delimiter", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Array), "The delimiter to use for splitting the string.")
                    .Build();

                yield return new FunctionOverloadBuilder("join")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("join", (_, _, _, argumentTypes) =>
                    {
                        (long delimiterMinLength, long? delimiterMaxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(argumentTypes[1]);

                        long minLength = 0;
                        long? maxLength = null;
                        switch (argumentTypes.FirstOrDefault())
                        {
                            case TupleType inputTuple:
                                maxLength = 0;
                                foreach (var item in inputTuple.Items)
                                {
                                    (long itemMinLength, long? itemMaxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(item.Type);

                                    minLength += itemMinLength;
                                    if (maxLength.HasValue)
                                    {
                                        if (itemMaxLength.HasValue)
                                        {
                                            maxLength += itemMaxLength.Value;
                                        }
                                        else
                                        {
                                            maxLength = null;
                                        }
                                    }
                                }

                                minLength += Math.Max(inputTuple.Items.Length - 1, 0) * delimiterMinLength;
                                maxLength = maxLength.HasValue && delimiterMaxLength.HasValue
                                    ? maxLength.Value + (Math.Max(inputTuple.Items.Length - 1, 0) * delimiterMaxLength.Value)
                                    : null;
                                break;

                            case ArrayType inputArray:
                                (long elementMinLength, long? elementMaxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(inputArray.Item.Type);
                                minLength = (inputArray.MinLength ?? 0) * elementMinLength;
                                minLength += Math.Max((inputArray.MinLength ?? 0) - 1, 0) * delimiterMinLength;

                                if (elementMaxLength.HasValue && delimiterMaxLength.HasValue && inputArray.MaxLength.HasValue)
                                {
                                    maxLength = elementMaxLength.Value * inputArray.MaxLength.Value;
                                    maxLength += Math.Max(inputArray.MaxLength.Value - 1, 0) * delimiterMaxLength.Value;
                                }
                                break;
                        }

                        return new(TypeFactory.CreateStringType(maxLength: maxLength, minLength: minLength switch
                        {
                            <= 0 => null,
                            _ => minLength,
                        }));
                    }), LanguageConstants.String)
                    .WithGenericDescription("Joins multiple strings into a single string, separated using a delimiter.")
                    .WithRequiredParameter("inputArray", new TypedArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool), TypeSymbolValidationFlags.Default), "An array of strings to join.")
                    .WithRequiredParameter("delimiter", LanguageConstants.String, "The delimiter to use to join the string.")
                    .Build();

                yield return new FunctionOverloadBuilder("string")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Converts the specified value to a string.")
                    .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to string. Any type of value can be converted, including objects and arrays.")
                    .Build();

                yield return new FunctionOverloadBuilder("int")
                    .WithReturnType(LanguageConstants.Int)
                    .WithGenericDescription("Converts the specified value to an integer.")
                    .WithRequiredParameter("valueToConvert", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), "The value to convert to an integer.")
                    .Build();

                yield return new FunctionOverloadBuilder("uniqueString")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("uniqueString", TypeFactory.CreateStringType(minLength: UniqueStringHashLength, maxLength: UniqueStringHashLength)),
                        TypeFactory.CreateStringType(minLength: UniqueStringHashLength, maxLength: UniqueStringHashLength))
                    .WithGenericDescription("Creates a deterministic hash string based on the values provided as parameters. The returned value is 13 characters long.")
                    .WithVariableParameter("arg", LanguageConstants.String, minimumCount: 1, "The value used in the hash function to create a unique string.")
                    .Build();

                yield return new FunctionOverloadBuilder("guid")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("guid", TypeFactory.CreateStringType(minLength: GuidLength, maxLength: GuidLength)),
                        TypeFactory.CreateStringType(minLength: GuidLength, maxLength: GuidLength))
                    .WithGenericDescription("Creates a value in the format of a globally unique identifier based on the values provided as parameters.")
                    .WithVariableParameter("arg", LanguageConstants.String, minimumCount: 1, "The value used in the hash function to create the GUID.")
                    .Build();

                yield return new FunctionOverloadBuilder("trim")
                    .WithReturnResultBuilder(
                        TryDeriveLiteralReturnType("trim",
                            (_, _, _, argumentTypes) => new(argumentTypes.FirstOrDefault() is StringType @string
                                ? TypeFactory.CreateStringType(minLength: null, @string.MaxLength, validationFlags: @string.ValidationFlags)
                                : LanguageConstants.String)),
                        LanguageConstants.String)
                    .WithGenericDescription("Removes all leading and trailing white-space characters from the specified string.")
                    .WithRequiredParameter("stringToTrim", LanguageConstants.String, "The value to trim.")
                    .Build();

                yield return new FunctionOverloadBuilder("uri")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("uri", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Creates an absolute URI by combining the baseUri and the relativeUri string.")
                    .WithRequiredParameter("baseUri", LanguageConstants.String, "The base uri string.")
                    .WithRequiredParameter("relativeUri", LanguageConstants.String, "The relative uri string to add to the base uri string.")
                    .Build();

                // TODO: Docs deviation
                yield return new FunctionOverloadBuilder("substring")
                    .WithReturnResultBuilder(
                        TryDeriveLiteralReturnType("substring", (_, _, _, argumentTypes) =>
                        {
                            switch (argumentTypes.Skip(2).FirstOrDefault())
                            {
                                case IntegerLiteralType intLiteral:
                                    return new(TypeFactory.CreateStringType(
                                        intLiteral.Value,
                                        intLiteral.Value,
                                        validationFlags: argumentTypes[0].ValidationFlags));
                                case IntegerType @int:
                                    return new(TypeFactory.CreateStringType(
                                        @int.MinValue,
                                        @int.MaxValue,
                                        validationFlags: argumentTypes[0].ValidationFlags));
                            }

                            (long? minLength, long? maxLength) = argumentTypes[0] switch
                            {
                                StringLiteralType literal => (literal.RawStringValue.Length, literal.RawStringValue.Length),
                                StringType @string => (@string.MinLength, @string.MaxLength),
                                _ => (null, null),
                            };

                            switch (argumentTypes[1])
                            {
                                case IntegerLiteralType intLiteral:
                                    minLength = minLength.HasValue && minLength.Value >= intLiteral.Value
                                        ? minLength.Value - intLiteral.Value
                                        : null;
                                    maxLength = maxLength.HasValue && maxLength.Value >= intLiteral.Value
                                        ? maxLength.Value - intLiteral.Value
                                        : maxLength;
                                    break;
                                case IntegerType @int:
                                    minLength = minLength.HasValue && @int.MaxValue.HasValue
                                        ? Math.Max(0, minLength.Value - @int.MaxValue.Value)
                                        : null;
                                    maxLength = maxLength.HasValue && @int.MinValue.HasValue
                                        ? Math.Max(0, maxLength.Value - @int.MinValue.Value)
                                        : maxLength;
                                    break;
                                default:
                                    minLength = null;
                                    break;
                            }

                            return new(TypeFactory.CreateStringType(minLength, maxLength, validationFlags: argumentTypes[0].ValidationFlags));
                        }),
                        LanguageConstants.String)
                    .WithGenericDescription("Returns a substring that starts at the specified character position and contains the specified number of characters.")
                    .WithRequiredParameter(
                        "stringToParse",
                        LanguageConstants.String,
                        "The original string from which the substring is extracted.")
                    .WithRequiredParameter(
                        "startIndex",
                        TypeFactory.CreateIntegerType(minValue: 0),
                        "The zero-based starting character position for the substring.",
                        (getArgumentType, _) => TypeFactory.CreateIntegerType(
                            minValue: 0,
                            maxValue: getArgumentType(0) switch
                            {
                                StringLiteralType stringLiteral => stringLiteral.RawStringValue.Length,
                                StringType @string => @string.MaxLength,
                                _ => null
                            }))
                    .WithOptionalParameter(
                        "length",
                        TypeFactory.CreateIntegerType(minValue: 0),
                        "The number of characters for the substring. Must refer to a location within the string. Must be zero or greater.",
                        (getArgumentType, _) =>
                        {
                            var maxInputLength = getArgumentType(0) switch
                            {
                                StringLiteralType stringLiteral => stringLiteral.RawStringValue.Length,
                                StringType @string => @string.MaxLength,
                                _ => null
                            };
                            return TypeFactory.CreateIntegerType(
                                minValue: 0,
                                maxValue: maxInputLength is long derivedMax
                                    ? getArgumentType(1) switch
                                    {
                                        IntegerLiteralType intLiteral when derivedMax >= intLiteral.Value
                                            => derivedMax - intLiteral.Value,
                                        IntegerType { MinValue: long minStart } when derivedMax >= minStart
                                            => derivedMax - minStart,
                                        _ => derivedMax,
                                    }
                                    : null);
                        })
                    .Build();

                yield return new FunctionOverloadBuilder("take")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("take", (_, _, functionCall, argumentTypes) =>
                    {
                        (long? originalMinLength, long? originalMaxLength) = argumentTypes[0] switch
                        {
                            ArrayType array => (array.MinLength, array.MaxLength),
                            _ => (null, null),
                        };
                        (long minToTake, long maxToTake) = argumentTypes[1] switch
                        {
                            IntegerLiteralType integerLiteral => (integerLiteral.Value, integerLiteral.Value),
                            IntegerType integer => (integer.MinValue ?? long.MinValue, integer.MaxValue ?? long.MaxValue),
                            _ => (long.MinValue, long.MaxValue),
                        };

                        return new(argumentTypes[0] switch
                        {
                            TupleType tupleType when minToTake == maxToTake && minToTake >= tupleType.Items.Length => tupleType,
                            TupleType tupleType when minToTake == maxToTake && minToTake <= 0 => new TupleType([], tupleType.ValidationFlags),
                            TupleType tupleType when minToTake == maxToTake && minToTake <= int.MaxValue => new TupleType([.. tupleType.Items.Take((int)minToTake)], tupleType.ValidationFlags),
                            ArrayType array => TypeFactory.CreateArrayType(array.Item,
                                !array.MinLength.HasValue ? null : minToTake switch
                                {
                                    <= 0 => null,
                                    _ => Math.Min(array.MinLength.Value, minToTake),
                                },
                                Math.Min(array.MaxLength ?? long.MaxValue, maxToTake) switch
                                {
                                    long.MaxValue => null,
                                    < 0 => 0,
                                    long otherwise => otherwise,
                                },
                                array.ValidationFlags),
                            _ => TypeFactory.CreateArrayType(null, maxToTake, argumentTypes[0].ValidationFlags),
                        });
                    }), LanguageConstants.Array)
                    .WithGenericDescription(TakeDescription)
                    .WithDescription("Returns an array with the specified number of elements from the start of the array.")
                    .WithRequiredParameter("originalValue", LanguageConstants.Array, "The array to take the elements from.")
                    .WithRequiredParameter("numberToTake", LanguageConstants.Int, "The number of elements to take. If this value is 0 or less, an empty array is returned. If it is larger than the length of the given array, all the elements in the array are returned.")
                    .Build();

                yield return new FunctionOverloadBuilder("take")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("take", (_, _, functionCall, argumentTypes) =>
                    {
                        (long? originalMinLength, long? originalMaxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(argumentTypes[0]);
                        (long minToTake, long maxToTake) = argumentTypes[1] switch
                        {
                            IntegerLiteralType integerLiteral => (integerLiteral.Value, integerLiteral.Value),
                            IntegerType integer => (integer.MinValue ?? long.MinValue, integer.MaxValue ?? long.MaxValue),
                            _ => (long.MinValue, long.MaxValue),
                        };

                        return new(TypeFactory.CreateStringType(
                            !originalMinLength.HasValue ? null : minToTake switch
                            {
                                <= 0 => null,
                                _ => Math.Min(originalMinLength.Value, minToTake),
                            },
                            Math.Min(originalMaxLength ?? long.MaxValue, maxToTake) switch
                            {
                                long.MaxValue => null,
                                < 0 => 0,
                                long otherwise => otherwise,
                            },
                            validationFlags: argumentTypes[0].ValidationFlags));
                    }), LanguageConstants.String)
                    .WithGenericDescription(TakeDescription)
                    .WithDescription("Returns a string with the specified number of characters from the start of the string.")
                    .WithRequiredParameter("originalValue", LanguageConstants.String, "The string to take the elements from.")
                    .WithRequiredParameter("numberToTake", LanguageConstants.Int, "The number of characters to take. If this value is 0 or less, an empty string is returned. If it is larger than the length of the given string, all the characters are returned.")
                    .Build();

                yield return new FunctionOverloadBuilder("skip")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("skip", (_, _, functionCall, argumentTypes) =>
                    {
                        (long minToSkip, long maxToSkip) = argumentTypes[1] switch
                        {
                            IntegerLiteralType integerLiteral => (integerLiteral.Value, integerLiteral.Value),
                            IntegerType integer => (integer.MinValue ?? long.MinValue, integer.MaxValue ?? long.MaxValue),
                            _ => (long.MinValue, long.MaxValue),
                        };

                        return new(argumentTypes[0] switch
                        {
                            TypeSymbol original when maxToSkip <= 0 => original,
                            TupleType tupleType when minToSkip == maxToSkip && minToSkip <= int.MaxValue => new TupleType([.. tupleType.Items.Skip((int)minToSkip)], tupleType.ValidationFlags),
                            ArrayType array => TypeFactory.CreateArrayType(array.Item,
                                ((array.MinLength ?? 0) - maxToSkip) switch
                                {
                                    <= 0 => null,
                                    var otherwise => otherwise,
                                },
                                !array.MaxLength.HasValue ? null : (array.MaxLength.Value - Math.Max(0, minToSkip)) switch
                                {
                                    < 0 => 0,
                                    long otherwise => otherwise,
                                },
                                array.ValidationFlags),
                            _ => TypeFactory.CreateArrayType(validationFlags: argumentTypes[0].ValidationFlags),
                        });
                    }), LanguageConstants.Array)
                    .WithGenericDescription(SkipDescription)
                    .WithDescription("Returns an array with all the elements after the specified number in the array.")
                    .WithRequiredParameter("originalValue", LanguageConstants.Array, "The array to use for skipping.")
                    .WithRequiredParameter("numberToSkip", LanguageConstants.Int, "The number of elements to skip. If this value is 0 or less, all the elements in the value are returned. If it is larger than the length of the array, an empty array is returned.")
                    .Build();

                yield return new FunctionOverloadBuilder("skip")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("skip", (_, _, functionCall, argumentTypes) =>
                    {
                        (long? originalMinLength, long? originalMaxLength) = TypeHelper.GetMinAndMaxLengthOfStringified(argumentTypes[0]);
                        (long minToSkip, long maxToSkip) = argumentTypes[1] switch
                        {
                            IntegerLiteralType integerLiteral => (integerLiteral.Value, integerLiteral.Value),
                            IntegerType integer => (integer.MinValue ?? long.MinValue, integer.MaxValue ?? long.MaxValue),
                            _ => (long.MinValue, long.MaxValue),
                        };

                        if (maxToSkip <= 0)
                        {
                            return new(argumentTypes[0]);
                        }

                        return new(TypeFactory.CreateStringType(
                            ((originalMinLength ?? 0) - maxToSkip) switch
                            {
                                <= 0 => null,
                                var otherwise => otherwise,
                            },
                            !originalMaxLength.HasValue ? null : (originalMaxLength.Value - Math.Max(0, minToSkip)) switch
                            {
                                < 0 => 0,
                                long otherwise => otherwise,
                            },
                            validationFlags: argumentTypes[0].ValidationFlags));
                    }), LanguageConstants.String)
                    .WithGenericDescription(SkipDescription)
                    .WithDescription("Returns a string with all the characters after the specified number in the string.")
                    .WithRequiredParameter("originalValue", LanguageConstants.String, "The string to use for skipping.")
                    .WithRequiredParameter("numberToSkip", LanguageConstants.Int, "The number of characters to skip. If this value is 0 or less, all the characters in the value are returned. If it is larger than the length of the string, an empty string is returned.")
                    .Build();

                yield return new FunctionOverloadBuilder("empty")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("empty", LanguageConstants.Bool), LanguageConstants.Bool)
                    .WithGenericDescription("Determines if an array, object, or string is empty.")
                    .WithRequiredParameter("itemToTest", TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String), "The value to check if it is empty.")
                    .Build();

                yield return new FunctionOverloadBuilder("contains")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("contains", LanguageConstants.Bool), LanguageConstants.Bool)
                    .WithGenericDescription(ContainsDescription)
                    .WithDescription("Checks whether an object contains a property. The property name comparison is case-insensitive.")
                    .WithRequiredParameter("object", LanguageConstants.Object, "The object")
                    .WithRequiredParameter("propertyName", LanguageConstants.String, "The property name.")
                    .Build();

                yield return new FunctionOverloadBuilder("contains")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("contains", LanguageConstants.Bool), LanguageConstants.Bool)
                    .WithGenericDescription(ContainsDescription)
                    .WithDescription("Checks whether an array contains a value. For arrays of simple values, exact match is done (case-sensitive for strings). For arrays of objects or arrays a deep comparison is done.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array")
                    .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("contains")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("contains", LanguageConstants.Bool), LanguageConstants.Bool)
                    .WithGenericDescription(ContainsDescription)
                    .WithDescription("Checks whether a string contains a substring. The string comparison is case-sensitive.")
                    .WithRequiredParameter("string", LanguageConstants.String, "The string.")
                    .WithRequiredParameter("itemToFind", LanguageConstants.String, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("intersection")
                    // TODO even with non-literal types, some type arithmetic could be performed
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("intersection", LanguageConstants.Object), LanguageConstants.Object)
                    .WithGenericDescription(IntersectionDescription)
                    .WithDescription("Returns a single object with the common elements from the parameters.")
                    .WithVariableParameter("object", LanguageConstants.Object, minimumCount: 2, "The object to use for finding common elements.")
                    .Build();

                yield return new FunctionOverloadBuilder("intersection")
                    // TODO even with non-literal types, some type arithmetic could be performed
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("intersection", LanguageConstants.Array), LanguageConstants.Array)
                    .WithGenericDescription(IntersectionDescription)
                    .WithDescription("Returns a single array with the common elements from the parameters.")
                    .WithVariableParameter("array", LanguageConstants.Array, minimumCount: 2, "The array to use for finding common elements.")
                    .Build();

                yield return new FunctionOverloadBuilder("union")
                    // TODO even with non-literal types, some type arithmetic could be performed
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("union", LanguageConstants.Object), LanguageConstants.Object)
                    .WithGenericDescription(UnionDescription)
                    .WithDescription("Returns a single object with all elements from the parameters. Duplicate keys are only included once.")
                    .WithVariableParameter("object", LanguageConstants.Object, minimumCount: 2, "The first object to use for joining elements.")
                    .Build();

                yield return new FunctionOverloadBuilder("union")
                    // TODO even with non-literal types, some type arithmetic could be performed
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("union", LanguageConstants.Array), LanguageConstants.Array)
                    .WithGenericDescription(UnionDescription)
                    .WithDescription("Returns a single array with all elements from the parameters. Duplicate values are only included once.")
                    .WithVariableParameter("object", LanguageConstants.Array, minimumCount: 2, "The first array to use for joining elements.")
                    .Build();

                yield return new FunctionOverloadBuilder("shallowMerge")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("shallowMerge", LanguageConstants.Object), LanguageConstants.Object)
                    .WithGenericDescription(UnionDescription)
                    .WithDescription("Returns a single object with all elements from the parameters. If there are duplicate keys, the last key wins.")
                    .WithRequiredParameter("entries", new TypedArrayType(LanguageConstants.Object, TypeSymbolValidationFlags.Default), "The array of objects to merge.")
                    .Build();

                yield return new FunctionOverloadBuilder("first")
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => new(argumentTypes[0] switch
                    {
                        TupleType tupleType => tupleType.Items.FirstOrDefault()?.Type ?? LanguageConstants.Null,
                        ArrayType arrayType when arrayType.MinLength.HasValue && arrayType.MinLength.Value > 0 => arrayType.Item.Type,
                        ArrayType arrayType => TypeHelper.CreateTypeUnion(LanguageConstants.Null, arrayType.Item.Type),
                        _ => LanguageConstants.Any
                    }), LanguageConstants.Any)
                    .WithGenericDescription(FirstDescription)
                    .WithDescription("Returns the first element of the array.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The value to retrieve the first element.")
                    .Build();

                yield return new FunctionOverloadBuilder("first")
                    .WithReturnResultBuilder(
                        TryDeriveLiteralReturnType("first",
                            (_, _, _, argumentTypes) => new(argumentTypes.FirstOrDefault() is StringType @string
                                ? TypeFactory.CreateStringType(
                                    @string.MinLength.HasValue ? Math.Min(@string.MinLength.Value, 1) : null,
                                    maxLength: 1,
                                    validationFlags: @string.ValidationFlags)
                                : TypeFactory.CreateStringType(
                                    minLength: null,
                                    maxLength: 1,
                                    validationFlags: argumentTypes[0].ValidationFlags))),
                        LanguageConstants.String)
                    .WithGenericDescription(FirstDescription)
                    .WithDescription("Returns the first character of the string.")
                    .WithRequiredParameter("string", LanguageConstants.String, "The value to retrieve the first character.")
                    .Build();

                yield return new FunctionOverloadBuilder("last")
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => new(argumentTypes[0] switch
                    {
                        TupleType tupleType => tupleType.Items.LastOrDefault()?.Type ?? LanguageConstants.Null,
                        ArrayType arrayType when arrayType.MinLength.HasValue && arrayType.MinLength.Value > 0 => arrayType.Item.Type,
                        ArrayType arrayType => TypeHelper.CreateTypeUnion(LanguageConstants.Null, arrayType.Item.Type),
                        _ => LanguageConstants.Any,
                    }), LanguageConstants.Any)
                    .WithGenericDescription(LastDescription)
                    .WithDescription("Returns the last element of the array.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The value to retrieve the last element.")
                    .Build();

                yield return new FunctionOverloadBuilder("last")
                    .WithReturnResultBuilder(
                        TryDeriveLiteralReturnType("last",
                            (_, _, _, argumentTypes) => new(argumentTypes.FirstOrDefault() is StringType @string
                                ? TypeFactory.CreateStringType(
                                    minLength: @string.MinLength.HasValue ? Math.Min(@string.MinLength.Value, 1) : null,
                                    maxLength: 1,
                                    validationFlags: @string.ValidationFlags)
                                : TypeFactory.CreateStringType(
                                    minLength: null,
                                    maxLength: 1,
                                    validationFlags: argumentTypes[0].ValidationFlags))),
                        LanguageConstants.String)
                    .WithGenericDescription(LastDescription)
                    .WithDescription("Returns the last character of the string.")
                    .WithRequiredParameter("string", LanguageConstants.String, "The value to retrieve the last character.")
                    .Build();

                yield return new FunctionOverloadBuilder("indexOf")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("indexOf", LanguageConstants.Int), LanguageConstants.Int)
                    .WithGenericDescription("Returns the first position of a value within a string. The comparison is case-insensitive.")
                    .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                    .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("indexOf")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("indexOf", LanguageConstants.Int), LanguageConstants.Int)
                    .WithGenericDescription("Returns the first position of a value within an array. For arrays of simple values, exact match is done (case-sensitive for strings). For arrays of objects or arrays a deep comparison is done.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array that contains the item to find.")
                    .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("lastIndexOf")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("lastIndexOf", LanguageConstants.Int), LanguageConstants.Int)
                    .WithGenericDescription("Returns the last position of a value within a string. The comparison is case-insensitive.")
                    .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                    .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("lastIndexOf")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("lastIndexOf", LanguageConstants.Int), LanguageConstants.Int)
                    .WithGenericDescription("Returns the last position of a value within an array. For arrays of simple values, exact match is done (case-sensitive for strings). For arrays of objects or arrays a deep comparison is done.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array that contains the item to find.")
                    .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("startsWith")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("startsWith", LanguageConstants.Bool), LanguageConstants.Bool)
                    .WithGenericDescription("Determines whether a string starts with a value. The comparison is case-insensitive.")
                    .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                    .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                    .Build();

                yield return new FunctionOverloadBuilder("endsWith")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("endsWith", LanguageConstants.Bool), LanguageConstants.Bool)
                    .WithGenericDescription("Determines whether a string ends with a value. The comparison is case-insensitive.")
                    .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                    .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                    .Build();

                static long? MinOf(TypeSymbol type) => type switch
                {
                    IntegerType @int => @int.MinValue,
                    IntegerLiteralType intLiteral => intLiteral.Value,
                    _ => null,
                };
                static long? MaxOf(TypeSymbol type) => type switch
                {
                    IntegerType @int => @int.MaxValue,
                    IntegerLiteralType intLiteral => intLiteral.Value,
                    _ => null,
                };

                static FunctionResult CalculateMin(
                    SemanticModel model,
                    IDiagnosticWriter diagnostics,
                    FunctionCallSyntaxBase syntax,
                    ImmutableArray<TypeSymbol> argumentTypes)
                {
                    long? min = MinOf(argumentTypes[0]);
                    long? max = MaxOf(argumentTypes[0]);

                    for (int i = 1; i < argumentTypes.Length; i++)
                    {
                        if (min.HasValue)
                        {
                            min = MinOf(argumentTypes[i]) is { } minAtIdx
                                ? Math.Min(min.Value, minAtIdx)
                                : null;
                        }

                        if (MaxOf(argumentTypes[i]) is { } maxAtIdx)
                        {
                            max = max.HasValue
                                ? Math.Min(max.Value, maxAtIdx)
                                : maxAtIdx;
                        }
                    }

                    return new(TypeFactory.CreateIntegerType(min, max));
                }

                static FunctionResult CalculateMax(
                    SemanticModel model,
                    IDiagnosticWriter diagnostics,
                    FunctionCallSyntaxBase syntax,
                    ImmutableArray<TypeSymbol> argumentTypes)
                {
                    long? min = MinOf(argumentTypes[0]);
                    long? max = MaxOf(argumentTypes[0]);

                    for (int i = 1; i < argumentTypes.Length; i++)
                    {
                        if (MinOf(argumentTypes[i]) is { } minAtIdx)
                        {
                            min = min.HasValue ? Math.Max(min.Value, minAtIdx) : minAtIdx;
                        }

                        if (max.HasValue)
                        {
                            max = MaxOf(argumentTypes[i]) is { } maxAtIdx
                                ? Math.Max(max.Value, maxAtIdx)
                                : null;
                        }
                    }

                    return new(TypeFactory.CreateIntegerType(min, max));
                }

                // TODO: Needs to support number type as well
                // TODO: Docs need updates
                yield return new FunctionOverloadBuilder("min")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("min", CalculateMin), LanguageConstants.Int)
                    .WithGenericDescription(MinDescription)
                    .WithDescription("Returns the minimum value from the specified integers.")
                    .WithVariableParameter("int", LanguageConstants.Int, minimumCount: 1, "One of the integers used to calculate the minimum value")
                    .Build();

                // TODO: Docs need updates
                yield return new FunctionOverloadBuilder("min")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("min", (model, diagnostics, syntax, argumentTypes) => argumentTypes[0] switch
                    {
                        TupleType tuple => CalculateMin(model, diagnostics, syntax, [.. tuple.Items.Select(t => t.Type)]),
                        TypedArrayType typedArray when typedArray.Item.Type is IntegerType or IntegerLiteralType
                            => new(typedArray.Item.Type),
                        _ => new(LanguageConstants.Int),
                    }), LanguageConstants.Int)
                    .WithGenericDescription(MinDescription)
                    .WithDescription("Returns the minimum value from an array of integers.")
                    .WithRequiredParameter("intArray", new TypedArrayType(LanguageConstants.Int, TypeSymbolValidationFlags.Default), "The array of integers.")
                    .Build();

                // TODO: Needs to support number type as well
                // TODO: Docs need updates
                yield return new FunctionOverloadBuilder("max")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("max", CalculateMax), LanguageConstants.Int)
                    .WithGenericDescription(MaxDescription)
                    .WithDescription("Returns the maximum value from the specified integers.")
                    .WithVariableParameter("int", LanguageConstants.Int, minimumCount: 1, "One of the integers used to calculate the maximum value")
                    .Build();

                // TODO: Docs need updates
                yield return new FunctionOverloadBuilder("max")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("max", (model, diagnostics, syntax, argumentTypes) => argumentTypes[0] switch
                    {
                        TupleType tuple => CalculateMax(model, diagnostics, syntax, [.. tuple.Items.Select(t => t.Type)]),
                        TypedArrayType typedArray when typedArray.Item.Type is IntegerType or IntegerLiteralType
                            => new(typedArray.Item.Type),
                        _ => new(LanguageConstants.Int),
                    }), LanguageConstants.Int)
                    .WithGenericDescription(MaxDescription)
                    .WithDescription("Returns the maximum value from an array of integers.")
                    .WithRequiredParameter("intArray", new TypedArrayType(LanguageConstants.Int, TypeSymbolValidationFlags.Default), "The array of integers.")
                    .Build();

                yield return new FunctionOverloadBuilder("range")
                    .WithReturnResultBuilder(
                        (_, _, _, argumentTypes) =>
                        {
                            static TypeSymbol GetRangeReturnElementType(TypeSymbol arg0Type, TypeSymbol arg1Type) => (arg0Type, arg1Type) switch
                            {
                                (UnionType start, _) => TypeHelper.CreateTypeUnion(start.Members.Select(m => GetRangeReturnElementType(m.Type, arg1Type))) switch
                                {
                                    TypeSymbol type when TypeCollapser.TryCollapse(type) is TypeSymbol collapsed => collapsed,
                                    TypeSymbol type => type,
                                },
                                (_, UnionType count) => TypeHelper.CreateTypeUnion(count.Members.Select(m => GetRangeReturnElementType(arg0Type, m.Type))) switch
                                {
                                    TypeSymbol type when TypeCollapser.TryCollapse(type) is TypeSymbol collapsed => collapsed,
                                    TypeSymbol type => type,
                                },

                                (IntegerLiteralType start, IntegerLiteralType count) => TypeFactory.CreateIntegerType(start.Value, start.Value + count.Value - 1),
                                (IntegerLiteralType start, IntegerType count) when count.MaxValue.HasValue => TypeFactory.CreateIntegerType(start.Value, start.Value + count.MaxValue.Value - 1),
                                (IntegerLiteralType start, _) => TypeFactory.CreateIntegerType(start.Value),

                                (IntegerType start, IntegerLiteralType count) when start.MaxValue.HasValue
                                    => TypeFactory.CreateIntegerType(start.MinValue, start.MaxValue.Value + count.Value - 1),
                                (IntegerType start, IntegerType count) when start.MaxValue.HasValue && count.MaxValue.HasValue
                                    => TypeFactory.CreateIntegerType(start.MinValue, start.MaxValue.Value + count.MaxValue.Value - 1),
                                (IntegerType start, _) => TypeFactory.CreateIntegerType(start.MinValue),

                                _ => LanguageConstants.Int,
                            };

                            static (long? minLength, long? maxLength) GetRangeLengthBounds(TypeSymbol countType, bool tryCollapse = true) => countType switch
                            {
                                IntegerLiteralType literal => (literal.Value, literal.Value),
                                IntegerType @int => (@int.MinValue, @int.MaxValue),
                                TypeSymbol otherwise when tryCollapse && TypeCollapser.TryCollapse(otherwise) is TypeSymbol collapsed => GetRangeLengthBounds(collapsed, tryCollapse: false),
                                _ => (null, null),
                            };

                            var elementType = GetRangeReturnElementType(argumentTypes[0], argumentTypes[1]);
                            var (minLength, maxLength) = GetRangeLengthBounds(argumentTypes[1]);
                            return new(TypeFactory.CreateArrayType(elementType, minLength, maxLength));
                        },
                        new TypedArrayType(LanguageConstants.Int, default))
                    .WithGenericDescription("Creates an array of integers from a starting integer and containing a number of items.")
                    .WithRequiredParameter("startIndex", LanguageConstants.Int, "The first integer in the array. The sum of startIndex and count must be no greater than 2147483647.")
                    .WithRequiredParameter("count", LanguageConstants.Int, "The number of integers in the array. Must be non-negative integer up to 10000.")
                    .Build();

                yield return new FunctionOverloadBuilder("base64ToString")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("base64ToString", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Converts a base64 representation to a string.")
                    .WithRequiredParameter("base64Value", LanguageConstants.String, "The base64 representation to convert to a string.")
                    .Build();

                yield return new FunctionOverloadBuilder("base64ToJson")
                    .WithReturnType(LanguageConstants.Any)
                    .WithGenericDescription("Converts a base64 representation to a JSON object.")
                    .WithRequiredParameter("base64Value", LanguageConstants.String, "The base64 representation to convert to a JSON object.")
                    .Build();

                yield return new FunctionOverloadBuilder("uriComponentToString")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("uriComponentToString", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Returns a string of a URI encoded value.")
                    .WithRequiredParameter("uriEncodedString", LanguageConstants.String, "The URI encoded value to convert to a string.")
                    .Build();

                yield return new FunctionOverloadBuilder("uriComponent")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("uriComponent", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Encodes a URI.")
                    .WithRequiredParameter("stringToEncode", LanguageConstants.String, "The value to encode.")
                    .Build();

                yield return new FunctionOverloadBuilder("dataUriToString")
                    .WithGenericDescription("Converts a data URI formatted value to a string.")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("dataUriToString", LanguageConstants.String), LanguageConstants.String)
                    .WithRequiredParameter("dataUriToConvert", LanguageConstants.String, "The data URI value to convert.")
                    .Build();

                // TODO: Docs have wrong param type and param name (any is actually supported)
                yield return new FunctionOverloadBuilder("dataUri")
                    .WithReturnResultBuilder(TryDeriveLiteralReturnType("dataUri", LanguageConstants.String), LanguageConstants.String)
                    .WithGenericDescription("Converts a value to a data URI.")
                    .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to a data URI.")
                    .Build();

                yield return new FunctionOverloadBuilder("array")
                    .WithGenericDescription("Converts the value to an array.")
                    .WithReturnType(LanguageConstants.Array)
                    .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to an array.")
                    .Build();

                yield return new FunctionOverloadBuilder("bool")
                    .WithReturnType(LanguageConstants.Bool)
                    .WithGenericDescription("Converts the parameter to a boolean.")
                    .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to a boolean.")
                    .Build();

                yield return new FunctionOverloadBuilder("json")
                    .WithGenericDescription("Converts a valid JSON string into a JSON data type.")
                    .WithRequiredParameter("json", LanguageConstants.String, "The value to convert to JSON. The string must be a properly formatted JSON string.")
                    .WithReturnResultBuilder(JsonResultBuilder, LanguageConstants.Any)
                    .Build();

                yield return new FunctionOverloadBuilder("dateTimeAdd")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Adds a time duration to a base value. ISO 8601 format is expected.")
                    .WithRequiredParameter("base", LanguageConstants.String, "The starting datetime value for the addition. [Use ISO 8601 timestamp format](https://en.wikipedia.org/wiki/ISO_8601).")
                    .WithRequiredParameter("duration", LanguageConstants.String, "The time value to add to the base. It can be a negative value. Use [ISO 8601 duration format](https://en.wikipedia.org/wiki/ISO_8601#Durations).")
                    .WithOptionalParameter("format", LanguageConstants.String, "The output format for the date time result. If not provided, the format of the base value is used. Use either [standard format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) or [custom format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).")
                    .Build();

                yield return new FunctionOverloadBuilder("dateTimeToEpoch")
                    .WithReturnType(LanguageConstants.Int)
                    .WithGenericDescription("Converts an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string to an epoch time integer value.")
                    .WithOptionalParameter("dateTime", LanguageConstants.String, "An [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) formatted dateTime string to be converted to epoch time.")
                    .Build();

                yield return new FunctionOverloadBuilder("dateTimeFromEpoch")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Converts an epoch time integer value to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string.")
                    .WithOptionalParameter("epochTime", LanguageConstants.Int, "An epoch time value that will be converted to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime formatted string.")
                    .Build();

                // newGuid and utcNow are only allowed in parameter default values
                yield return new FunctionOverloadBuilder("utcNow")
                    .WithReturnType(LanguageConstants.String)
                    .WithGenericDescription("Returns the current (UTC) datetime value in the specified format. If no format is provided, the ISO 8601 (yyyyMMddTHHmmssZ) format is used. **This function can only be used in the default value for a parameter**.")
                    .WithOptionalParameter("format", LanguageConstants.String, "The format. Use either [standard format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) or [custom format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).")
                    .WithFlags(FunctionFlags.ParamDefaultsOnly)
                    .Build();

                yield return new FunctionOverloadBuilder("newGuid")
                    .WithReturnType(TypeFactory.CreateStringType(minLength: GuidLength, maxLength: GuidLength))
                    .WithGenericDescription("Returns a value in the format of a globally unique identifier. **This function can only be used in the default value for a parameter**.")
                    .WithFlags(FunctionFlags.ParamDefaultsOnly)
                    .Build();

                yield return new FunctionOverloadBuilder("loadTextContent")
                    .WithGenericDescription($"Loads the content of the specified file into a string. Content loading occurs during compilation, not at runtime. The maximum allowed content size is {LanguageConstants.MaxLiteralCharacterLimit} characters (including line endings).")
                    .WithRequiredParameter("filePath", LanguageConstants.StringFilePath, "The path to the file that will be loaded.")
                    .WithOptionalParameter("encoding", LanguageConstants.LoadTextContentEncodings, "File encoding. If not provided, UTF-8 will be used.")
                    .WithReturnResultBuilder(LoadTextContentResultBuilder, LanguageConstants.String)
                    .WithFlags(FunctionFlags.GenerateIntermediateVariableOnIndirectAssignment)
                    .Build();

                yield return new FunctionOverloadBuilder("loadFileAsBase64")
                    .WithGenericDescription($"Loads the specified file as base64 string. File loading occurs during compilation, not at runtime. The maximum allowed size is {LanguageConstants.MaxLiteralCharacterLimit / 4 * 3 / 1024} Kb.")
                    .WithRequiredParameter("filePath", LanguageConstants.StringFilePath, "The path to the file that will be loaded.")
                    .WithReturnResultBuilder(LoadContentAsBase64ResultBuilder, LanguageConstants.String)
                    .WithFlags(FunctionFlags.GenerateIntermediateVariableOnIndirectAssignment)
                    .Build();

                yield return new FunctionOverloadBuilder("loadJsonContent")
                    .WithGenericDescription($"Loads the specified JSON file as bicep object. File loading occurs during compilation, not at runtime.")
                    .WithRequiredParameter("filePath", LanguageConstants.StringJsonFilePath, "The path to the file that will be loaded.")
                    .WithOptionalParameter("jsonPath", LanguageConstants.String, "JSONPath expression to narrow down the loaded file. If not provided, a root element indicator '$' is used")
                    .WithOptionalParameter("encoding", LanguageConstants.LoadTextContentEncodings, "File encoding. If not provided, UTF-8 will be used.")
                    .WithReturnResultBuilder(LoadJsonContentResultBuilder, LanguageConstants.Any)
                    .WithFlags(FunctionFlags.GenerateIntermediateVariableAlways)
                    .Build();

                yield return new FunctionOverloadBuilder("loadYamlContent")
                .WithGenericDescription($"Loads the specified YAML file as bicep object. File loading occurs during compilation, not at runtime.")
                .WithRequiredParameter("filePath", LanguageConstants.StringYamlFilePath, "The path to the file that will be loaded.")
                .WithOptionalParameter("pathFilter", LanguageConstants.String, "The path filter is a JSONPath expression to narrow down the loaded file. If not provided, a root element indicator '$' is used")
                .WithOptionalParameter("encoding", LanguageConstants.LoadTextContentEncodings, "File encoding. If not provided, UTF-8 will be used.")
                .WithReturnResultBuilder(LoadYamlContentResultBuilder, LanguageConstants.Any)
                .WithFlags(FunctionFlags.GenerateIntermediateVariableAlways)
                .Build();

                yield return new FunctionOverloadBuilder("items")
                    .WithGenericDescription("Returns an array of keys and values for an object. Elements are consistently ordered alphabetically by key.")
                    .WithRequiredParameter("object", LanguageConstants.Object, "The object to return keys and values for")
                    .WithReturnResultBuilder(ItemsResultBuilder, GetItemsReturnType(LanguageConstants.String, LanguageConstants.Any))
                    .Build();

                yield return new FunctionOverloadBuilder("objectKeys")
                    .WithGenericDescription("Returns an array of object keys. Elements are consistently ordered alphabetically.")
                    .WithRequiredParameter("object", LanguageConstants.Object, "The object to return keys for")
                    .WithReturnResultBuilder(ObjectKeysResultBuilder, new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default))
                    .Build();

                yield return new FunctionOverloadBuilder("flatten")
                    .WithGenericDescription("Takes an array of arrays, and returns an array of sub-array elements, in the original order. Sub-arrays are only flattened once, not recursively.")
                    .WithRequiredParameter("array", new TypedArrayType(LanguageConstants.Array, TypeSymbolValidationFlags.Default), "The array of sub-arrays to flatten.")
                    .WithReturnResultBuilder((_, _, functionCall, argTypes) => new(TypeHelper.FlattenType(argTypes[0], functionCall.Arguments[0])), LanguageConstants.Array)
                    .Build();

                yield return new FunctionOverloadBuilder("filter")
                    .WithGenericDescription("Filters an array with a custom filtering function.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array to filter.")
                    .WithRequiredParameter("predicate", TypeHelper.CreateLambdaType([LanguageConstants.Any], [LanguageConstants.Int], LanguageConstants.Bool), "The predicate applied to each input array element. If false, the item will be filtered out of the output array.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => TypeHelper.CreateLambdaType([t], [LanguageConstants.Int], LanguageConstants.Bool)))
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => new(argumentTypes[0] switch
                    {
                        // If a tuple is filtered, each member of the resulting array will be assignable to <input tuple>.Item, but information about specific indices and tuple length is no longer reliable.
                        // For example, given a symbol `a` of type `[0, 1, 2, 3, 4]`, the expression `filter(a, x => x % 2 == 0)` returns an array in which each member is assignable to `0 | 1 | 2 | 3 | 4`,
                        // but the returned array (which has a concrete value of `[0, 2, 4]`) will not be assignable to the input tuple type of `[0, 1, 2, 3, 4]`
                        TupleType tuple => tuple.ToTypedArray(minLength: null, maxLength: tuple.MaxLength),
                        ArrayType arrayType => TypeFactory.CreateArrayType(arrayType.Item, minLength: null, maxLength: arrayType.MaxLength, arrayType.ValidationFlags),
                        var otherwise => otherwise,
                    }), LanguageConstants.Array)
                    .Build();

                yield return new FunctionOverloadBuilder("map")
                    .WithGenericDescription("Applies a custom mapping function to each element of an array and returns the result array.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array to map.")
                    .WithRequiredParameter("predicate", TypeHelper.CreateLambdaType([LanguageConstants.Any], [LanguageConstants.Int], LanguageConstants.Any), "The predicate applied to each input array element, in order to generate the output array.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => TypeHelper.CreateLambdaType([t], [LanguageConstants.Int], LanguageConstants.Any)))
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => argumentTypes[1] switch
                    {
                        LambdaType lambdaType => new(new TypedArrayType(lambdaType.ReturnType.Type, TypeSymbolValidationFlags.Default)),
                        _ => new(LanguageConstants.Any),
                    }, LanguageConstants.Array)
                    .Build();

                yield return new FunctionOverloadBuilder("mapValues")
                    .WithGenericDescription("Applies a custom mapping function to the values of an object and returns the result object.")
                    .WithRequiredParameter("object", LanguageConstants.Object, "The object to map.")
                    .WithRequiredParameter("predicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.Any), "The predicate applied to each input object value, in order to generate the output object.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromObjectValues(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.Any)))
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => argumentTypes[1] switch
                    {
                        LambdaType lambdaType => new(TypeHelper.CreateDictionaryType("object", TypeSymbolValidationFlags.Default, lambdaType.ReturnType.Type)),
                        _ => new(LanguageConstants.Any),
                    }, LanguageConstants.Array)
                    .Build();

                yield return new FunctionOverloadBuilder("sort")
                    .WithGenericDescription("Sorts an array with a custom sort function.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array to sort.")
                    .WithRequiredParameter("predicate", TypeHelper.CreateLambdaType([LanguageConstants.Any, LanguageConstants.Any], [], LanguageConstants.Bool), "The predicate used to compare two array elements for ordering. If true, the second element will be ordered after the first in the output array.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => TypeHelper.CreateLambdaType([t, t], [], LanguageConstants.Bool)))
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => new(argumentTypes[0] switch
                    {
                        // When a tuple is sorted, the resultant array will be of the same length as the input tuple, but the information about which member resides at which index can no longer be relied upon.
                        TupleType tuple => tuple.ToTypedArray(),
                        var otherwise => otherwise,
                    }), LanguageConstants.Array)
                    .Build();

                yield return new FunctionOverloadBuilder("reduce")
                    .WithGenericDescription("Reduces an array with a custom reduce function.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array to reduce.")
                    .WithRequiredParameter("initialValue", LanguageConstants.Any, "The initial value.")
                    .WithRequiredParameter("predicate", TypeHelper.CreateLambdaType([LanguageConstants.Any, LanguageConstants.Any], [LanguageConstants.Int], LanguageConstants.Any), "The predicate used to aggregate the current value and the next value. ",
                        calculator: (getArgumentType, _) =>
                        {
                            var toReduceType = getArgumentType(0);

                            var elementType = toReduceType switch
                            {
                                ArrayType array => array.Item.Type,
                                AnyType => LanguageConstants.Any,
                                _ => null,
                            };

                            var seedType = getArgumentType(1);

                            if (elementType is null || elementType is ErrorType || seedType is ErrorType)
                            {
                                return null;
                            }

                            var accumulationType = seedType switch
                            {
                                ErrorType error => error,
                                _ when TypeHelper.TryGetArmPrimitiveType(seedType) is TypeSymbol armPrimitive => armPrimitive,
                                _ => LanguageConstants.Any,
                            };
                            var firstArgType = toReduceType is ArrayType { MaxLength: <= 1 } ? seedType : accumulationType;

                            return TypeHelper.CreateLambdaType(
                                argumentTypes: [firstArgType, elementType],
                                optionalArgumentTypes: [CalculateIndexTypeOfArray(toReduceType)],
                                returnType: accumulationType);
                        })
                    .WithReturnType(LanguageConstants.Any)
                    .WithReturnResultBuilder((_, _, _, argumentTypes) => argumentTypes[2] switch
                    {
                        LambdaType lambdaType => new(lambdaType.ReturnType.Type),
                        _ => new(LanguageConstants.Any),
                    }, LanguageConstants.Any)
                    .Build();

                yield return new FunctionOverloadBuilder("toObject")
                    .WithGenericDescription("Converts an array to an object with a custom key function and optional custom value function.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array to map to an object.")
                    .WithRequiredParameter("keyPredicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.String), "The predicate applied to each input array element to return the object key.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.String)))
                    .WithOptionalParameter("valuePredicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.Any), "The optional predicate applied to each input array element to return the object value.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.Any)))
                    .WithReturnType(LanguageConstants.Any)
                    .WithReturnResultBuilder((_, _, _, argumentTypes) =>
                    {
                        if (argumentTypes.Length == 2 && argumentTypes[0] is ArrayType arrayArgType)
                        {
                            return new(TypeHelper.CreateDictionaryType("object", TypeSymbolValidationFlags.Default, arrayArgType.Item));
                        }

                        if (argumentTypes.Length == 3 && argumentTypes[2] is LambdaType valueLambdaType)
                        {
                            return new(TypeHelper.CreateDictionaryType("object", TypeSymbolValidationFlags.Default, valueLambdaType.ReturnType));
                        }

                        return new(LanguageConstants.Object);
                    }, LanguageConstants.Object)
                    .Build();

                yield return new FunctionOverloadBuilder("groupBy")
                    .WithGenericDescription("Converts an array to an object containing a lookup from key to array values filtered by said key. Values can be optionally translated using a mapping function.")
                    .WithRequiredParameter("array", LanguageConstants.Array, "The array to map to an object.")
                    .WithRequiredParameter("keyPredicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.String), "The predicate applied to each input array element to return the object key.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.String)))
                    .WithOptionalParameter("valuePredicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.Any), "The optional predicate applied to each input array element to return the object value.",
                        calculator: (getArgumentType, _) => CalculateLambdaFromArrayParam(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.Any)))
                    .WithReturnType(LanguageConstants.Any)
                    .WithReturnResultBuilder((_, _, _, argumentTypes) =>
                    {
                        if (argumentTypes.Length == 2 && argumentTypes[0] is ArrayType arrayArgType)
                        {
                            var valueType = new TypedArrayType(arrayArgType.Item, arrayArgType.ValidationFlags);
                            return new(TypeHelper.CreateDictionaryType("object", TypeSymbolValidationFlags.Default, valueType));
                        }

                        if (argumentTypes.Length == 3 && argumentTypes[2] is LambdaType valueLambdaType)
                        {
                            var valueType = new TypedArrayType(valueLambdaType.ReturnType, valueLambdaType.ReturnType.Type.ValidationFlags);
                            return new(TypeHelper.CreateDictionaryType("object", TypeSymbolValidationFlags.Default, valueType));
                        }

                        return new(LanguageConstants.Object);
                    }, LanguageConstants.Object)
                    .Build();

                yield return new FunctionOverloadBuilder(LanguageConstants.NameofFunctionName)
                    .WithGenericDescription("Returns the name of a declared symbol or property. Evaluation occurs during compilation, not at runtime.")
                    .WithRequiredParameter("symbol", LanguageConstants.Any, "The declared symbol or property to get the name of.")
                    .WithReturnResultBuilder((model, diagnostics, call, argumentTypes) =>
                    {
                        var argument = call.Arguments[0].Expression;
                        if (GetNameOfReturnValue(argument) is not { } returnValue)
                        {
                            return new(ErrorType.Create(DiagnosticBuilder.ForPosition(call.Arguments[0]).NameofInvalidOnUnnamedExpression()));
                        }

                        return new(
                            new StringLiteralType(returnValue, TypeSymbolValidationFlags.Default),
                            new StringLiteralExpression(argument, returnValue));
                    }, LanguageConstants.String)
                    .WithFlags(FunctionFlags.IsArgumentValueIndependent)
                    .Build();

                yield return new FunctionOverloadBuilder("fail")
                    .WithGenericDescription("Raises a runtime error with the provided message. Will cause a deployment to fail when evaluated.")
                    .WithRequiredParameter("message", LanguageConstants.String, "The error message to use.")
                    .WithReturnType(LanguageConstants.Never)
                    .Build();

                yield return new FunctionOverloadBuilder("loadDirectoryFileInfo")
                    .WithGenericDescription($"Loads basic information about a directory's files as bicep object. File loading occurs during compilation, not at runtime.")
                    .WithRequiredParameter("directoryPath", LanguageConstants.StringDirectoryPath, "The path to the directory that will be loaded.")
                    .WithOptionalParameter("searchPattern", LanguageConstants.String, "The searchPattern is a glob pattern to narrow down the loaded files. If not provided, all files are loaded. Supports both any number of characters '*' and any single character '?' wildcards.")
                    .WithReturnResultBuilder(LoadDirectoryFileInfoResultBuilder, LanguageConstants.Any)
                    .WithFlags(FunctionFlags.GenerateIntermediateVariableAlways)
                    .Build();
            }

            static IEnumerable<FunctionOverload> GetParamsFilePermittedOverloads(IFeatureProvider featureProvider)
            {
                if (!featureProvider.DeployCommandsEnabled)
                {
                    yield return new FunctionOverloadBuilder("readEnvironmentVariable")
                        .WithGenericDescription($"Reads the specified Environment variable as bicep string. Variable loading occurs during compilation, not at runtime.")
                        .WithRequiredParameter("variableName", LanguageConstants.String, "Environment Variable Name.")
                        .WithReturnResultBuilder(ReadEnvironmentVariableResultBuilder, LanguageConstants.String)
                        .WithFlags(FunctionFlags.GenerateIntermediateVariableAlways)
                        .WithOptionalParameter("default", LanguageConstants.String, "Default value to return if environment variable is not found.")
                        .Build();
                }

                if (featureProvider.DeployCommandsEnabled)
                {
                    yield return new FunctionOverloadBuilder(LanguageConstants.ReadEnvVarBicepFunctionName)
                        .WithGenericDescription($"Reads the specified environment variable as bicep string.")
                        .WithRequiredParameter("variableName", LanguageConstants.String, "The name of the environment variable.")
                        .WithEvaluator(exp => new FunctionCallExpression(exp.SourceSyntax, LanguageConstants.ExternalInputsArmFunctionName, [new StringLiteralExpression(null, "sys.envVar"), .. exp.Parameters]))
                        .WithReturnType(LanguageConstants.String)
                        .Build();

                    yield return new FunctionOverloadBuilder(LanguageConstants.ReadCliArgBicepFunctionName)
                        .WithGenericDescription($"Reads the specified CLI argument as bicep string.")
                        .WithRequiredParameter("argumentName", LanguageConstants.String, "The name of the CLI argument.")
                        .WithEvaluator(exp => new FunctionCallExpression(exp.SourceSyntax, LanguageConstants.ExternalInputsArmFunctionName, [new StringLiteralExpression(null, "sys.cliArg"), .. exp.Parameters]))
                        .WithReturnType(LanguageConstants.String)
                        .Build();
                }

                yield return new FunctionOverloadBuilder(LanguageConstants.ExternalInputBicepFunctionName)
                    .WithGenericDescription("Resolves input from an external source. The input value is resolved during deployment, not at compile time.")
                    .WithRequiredParameter("name", LanguageConstants.String, "The name of the input provided by the external tool.")
                    .WithOptionalParameter("config", LanguageConstants.Any, "The configuration for the input. The configuration is specific to the external tool.")
                    .WithEvaluator(exp => new FunctionCallExpression(exp.SourceSyntax, LanguageConstants.ExternalInputsArmFunctionName, exp.Parameters))
                    .WithReturnResultBuilder((model, diagnostics, functionCall, argumentTypes) =>
                    {
                        var visitor = new CompileTimeConstantVisitor(diagnostics);
                        foreach (var arg in functionCall.Arguments)
                        {
                            arg.Accept(visitor);
                        }

                        return new(LanguageConstants.Any);
                    }, LanguageConstants.Any)
                    .Build();
            }

            foreach (var overload in GetAlwaysPermittedOverloads())
            {
                yield return new(overload, (_, _) => true);
            }

            foreach (var overload in GetParamsFilePermittedOverloads(featureProvider))
            {
                yield return new(overload, (_, sfk) => sfk == BicepSourceFileKind.ParamsFile);
            }
        }

        private static ObjectType GetParseCidrReturnType()
        {
            return new ObjectType("parseCidr", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("network", LanguageConstants.String),
                new NamedTypeProperty("netmask", LanguageConstants.String),
                new NamedTypeProperty("broadcast", LanguageConstants.String),
                new NamedTypeProperty("firstUsable", LanguageConstants.String),
                new NamedTypeProperty("lastUsable", LanguageConstants.String),
                new NamedTypeProperty("cidr", TypeFactory.CreateIntegerType(0, 255)),
            }, null);
        }

        private static ObjectType GetParseOrBuildUriReturnType()
        {
            return new ObjectType("parseUri", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("scheme", LanguageConstants.String, TypePropertyFlags.Required),
                new NamedTypeProperty("host", LanguageConstants.String, TypePropertyFlags.Required),
                new NamedTypeProperty("port", LanguageConstants.Int, TypePropertyFlags.None),
                new NamedTypeProperty("path", LanguageConstants.String, TypePropertyFlags.None),
                new NamedTypeProperty("query", LanguageConstants.String, TypePropertyFlags.None),
             }, null);
        }

        private static FunctionOverload.ResultBuilderDelegate TryDeriveLiteralReturnType(string armFunctionName, TypeSymbol nonLiteralReturnType) =>
            TryDeriveLiteralReturnType(armFunctionName, (_, _, _, _) => new(nonLiteralReturnType));

        private static FunctionOverload.ResultBuilderDelegate TryDeriveLiteralReturnType(string armFunctionName, FunctionOverload.ResultBuilderDelegate nonLiteralReturnResultBuilder) =>
            (model, diagnostics, functionCall, argumentTypes) =>
            {
                FunctionResult returnType = ArmFunctionReturnTypeEvaluator.TryEvaluate(armFunctionName, out var diagnosticBuilders, argumentTypes) is { } literalReturnType
                    ? new(literalReturnType)
                    : nonLiteralReturnResultBuilder.Invoke(model, diagnostics, functionCall, argumentTypes);

                var diagnosticTarget = functionCall.Arguments.Any()
                    ? TextSpan.Between(functionCall.Arguments.First(), functionCall.Arguments.Last())
                    : TextSpan.Between(functionCall.OpenParen, functionCall.CloseParen);
                diagnostics.WriteMultiple(diagnosticBuilders.Select(b => b(DiagnosticBuilder.ForPosition(diagnosticTarget))));

                return returnType;
            };

        private static TypeSymbol? CalculateLambdaFromArrayParam(GetFunctionArgumentType getArgumentType, int arrayIndex, Func<TypeSymbol, LambdaType> lambdaBuilder)
        {
            if (getArgumentType(arrayIndex) is not ArrayType arrayType)
            {
                return null;
            }

            var itemType = arrayType.Item;
            return lambdaBuilder(itemType.Type);
        }

        private static TypeSymbol CalculateIndexTypeOfArray(TypeSymbol arrayType) => TypeFactory.CreateIntegerType(
            minValue: 0,
            maxValue: (arrayType as ArrayType)?.MaxLength,
            validationFlags: arrayType.ValidationFlags);

        private static TypeSymbol? CalculateLambdaFromObjectValues(GetFunctionArgumentType getArgumentType, int arrayIndex, Func<TypeSymbol, LambdaType> lambdaBuilder)
        {
            if (getArgumentType(arrayIndex) is not ObjectType objectType)
            {
                return null;
            }

            var (_, valueTypes) = GetReadableObjectKeysAndValues(objectType);
            return lambdaBuilder(TypeHelper.CreateTypeUnion(valueTypes));
        }

        private static LambdaType OneParamLambda(TypeSymbol paramType, TypeSymbol returnType)
            => TypeHelper.CreateLambdaType([paramType], [], returnType);

        private static FunctionResult LoadTextContentResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            if (TryLoadTextContentFromFile(model, diagnostics, (arguments[0], argumentTypes[0]), arguments.Length > 1 ? (arguments[1], argumentTypes[1]) : null, LanguageConstants.MaxLiteralCharacterLimit)
                .IsSuccess(out var result, out var errorDiagnostic))
            {
                return new(TypeFactory.CreateStringLiteralType(result.Content), new StringLiteralExpression(functionCall, result.Content));
            }

            return new(ErrorType.Create(errorDiagnostic));
        }

        private static FunctionResult LoadJsonContentResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => LoadContentResultBuilder(new JsonObjectParser(), model, diagnostics, functionCall, argumentTypes);

        private static FunctionResult LoadYamlContentResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
            => LoadContentResultBuilder(new YamlObjectParser(), model, diagnostics, functionCall, argumentTypes);

        private static FunctionResult LoadContentResultBuilder(ObjectParser objectParser, SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            string? tokenSelectorPath = null;
            IPositionable[] positionables = arguments.Length > 1 ? [arguments[0], arguments[1]] : [arguments[0]];
            if (arguments.Length > 1)
            {
                if (argumentTypes[1] is not StringLiteralType tokenSelectorType)
                {
                    return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[1]).CompileTimeConstantRequired()));
                }
                tokenSelectorPath = tokenSelectorType.RawStringValue;
            }

            int? characterLimit = model.Features.LocalDeployEnabled ? null : LanguageConstants.MaxJsonFileCharacterLimit;

            if (TryLoadTextContentFromFile(model, diagnostics, (arguments[0], argumentTypes[0]), arguments.Length > 2 ? (arguments[2], argumentTypes[2]) : null, characterLimit)
                .IsSuccess(out var result, out var errorDiagnostic) &&
                objectParser.TryExtractFromObject(result.Content, tokenSelectorPath, positionables, out errorDiagnostic, out var token))
            {
                return new(ConvertJsonToBicepType(token), ConvertJsonToExpression(token));
            }

            return new(ErrorType.Create(errorDiagnostic));
        }

        private static FunctionResult ReadEnvironmentVariableResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();

            if (argumentTypes.Length < 1 || argumentTypes[0] is not StringLiteralType stringLiteral)
            {
                return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[0]).CompileTimeConstantRequired()));
            }
            var envVariableName = stringLiteral.RawStringValue;
            var envVariableValue = model.Environment.GetVariable(envVariableName);

            if (envVariableValue == null)
            {
                if (argumentTypes.Length == 2 && argumentTypes[1] is StringLiteralType stringLiteral2)
                {
                    return new(TypeFactory.CreateStringLiteralType(stringLiteral2.RawStringValue),
                new StringLiteralExpression(null, stringLiteral2.RawStringValue));
                }
                else
                {
                    var envVariableNames = model.Environment.GetVariableNames();
                    var suggestion = SpellChecker.GetSpellingSuggestion(envVariableName, envVariableNames);
                    if (suggestion != null)
                    {
                        suggestion = $" Did you mean \"{suggestion}\"?";
                    }
                    //log available environment variables if verbose logging is enabled
                    if (model.Configuration.Analyzers.GetValue(LinterAnalyzer.LinterEnabledSetting, false) && model.Configuration.Analyzers.GetValue(LinterAnalyzer.LinterVerboseSetting, false))
                    {
                        diagnostics.Write(
                            new Diagnostic(
                                arguments[0].Span,
                                DiagnosticLevel.Info,
                                DiagnosticSource.CoreLinter,
                                "Bicepparam ReadEnvironmentVariable function",
                                $"Available environment variables are: {string.Join(", ", envVariableNames)}")
                        );
                    }

                    //error to fail the build-param with clear message of the missing env var name
                    return new(
                        ErrorType.Create(
                            DiagnosticBuilder.ForPosition(arguments[0])
                                .EnvironmentVariableDoesNotExist(envVariableName, suggestion)));
                }
            }
            return new(TypeFactory.CreateStringLiteralType(envVariableValue),
                new StringLiteralExpression(null, envVariableValue));
        }

        private static ResultWithDiagnostic<LoadTextContentResult> TryLoadTextContentFromFile(SemanticModel model, IDiagnosticWriter diagnostics, (FunctionArgumentSyntax syntax, TypeSymbol typeSymbol) filePathArgument, (FunctionArgumentSyntax syntax, TypeSymbol typeSymbol)? encodingArgument, int? maxCharacters)
        {
            if (filePathArgument.typeSymbol is not StringLiteralType filePathType)
            {
                return new(DiagnosticBuilder.ForPosition(filePathArgument.syntax).CompileTimeConstantRequired());
            }


            var encoding = Encoding.UTF8;
            if (encodingArgument is not null)
            {
                if (encodingArgument.Value.typeSymbol is not StringLiteralType encodingType)
                {
                    return new(DiagnosticBuilder.ForPosition(encodingArgument.Value.syntax).CompileTimeConstantRequired());
                }

                encoding = LanguageConstants.SupportedEncodings[encodingType.RawStringValue];
            }

            var auxiliaryFileLoadResult = RelativePath.TryCreate(filePathType.RawStringValue).Transform(model.SourceFile.TryLoadAuxiliaryFile);

            if (!auxiliaryFileLoadResult.IsSuccess(out var auxiliaryFile, out var errorBuilder))
            {
                return new(errorBuilder(DiagnosticBuilder.ForPosition(filePathArgument.syntax)));
            }

            if (encodingArgument is not null && auxiliaryFile.TryDetectEncodingFromByteOrderMarks() is { } detectedEncoding && !Equals(encoding, detectedEncoding))
            {
                // FileEncodingMimatch has DiagnosticLevel.Info
                diagnostics.Write(DiagnosticBuilder.ForPosition(encodingArgument.Value.syntax).FileEncodingMismatch(detectedEncoding.WebName));
            }

            if (!auxiliaryFile.TryReadText(encoding, maxCharacters).IsSuccess(out var content, out errorBuilder))
            {
                return new(errorBuilder(DiagnosticBuilder.ForPosition(filePathArgument.syntax)));
            }

            return new(new LoadTextContentResult(auxiliaryFile.Uri, content));
        }

        private static ResultWithDiagnostic<LoadTextContentResult> TryLoadTextContentAsBase64(SemanticModel model, (FunctionArgumentSyntax syntax, TypeSymbol typeSymbol) filePathArgument)
        {
            if (filePathArgument.typeSymbol is not StringLiteralType filePathType)
            {
                return new(DiagnosticBuilder.ForPosition(filePathArgument.syntax).CompileTimeConstantRequired());
            }

            var auxiliaryFileLoadResult = RelativePath.TryCreate(filePathType.RawStringValue).Transform(model.SourceFile.TryLoadAuxiliaryFile);

            if (!auxiliaryFileLoadResult.IsSuccess(out var auxiliaryFile, out var readErrorBuilder))
            {
                return new(readErrorBuilder(DiagnosticBuilder.ForPosition(filePathArgument.syntax)));
            }

            var maxFileSizeInBytes = LanguageConstants.MaxLiteralCharacterLimit / 4 * 3; //each base64 character represents 6 bits

            if (!auxiliaryFile.TryReadBytes(maxFileSizeInBytes).IsSuccess(out var bytes, out var errorBuilder))
            {
                return new(errorBuilder(DiagnosticBuilder.ForPosition(filePathArgument.syntax)));
            }

            var content = Convert.ToBase64String(bytes.Span, Base64FormattingOptions.None);

            return new(new LoadTextContentResult(auxiliaryFile.Uri, content));
        }

        private static FunctionResult LoadContentAsBase64ResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            if (TryLoadTextContentAsBase64(model, (arguments[0], argumentTypes[0])).IsSuccess(out var result, out var errorDiagnostic))
            {
                return new(
                    new StringLiteralType(result.FileUri.GetPathRelativeTo(model.SourceFile.FileHandle.Uri), result.Content, TypeSymbolValidationFlags.Default),
                    new StringLiteralExpression(functionCall, result.Content));
            }

            return new(ErrorType.Create(errorDiagnostic));
        }

        private static readonly ImmutableHashSet<JTokenType> SupportedJsonTokenTypes = [JTokenType.Object, JTokenType.Array, JTokenType.String, JTokenType.Integer, JTokenType.Float, JTokenType.Boolean, JTokenType.Null];
        private static Expression ConvertJsonToExpression(JToken token)
            => token switch
            {
                JObject @object => new ObjectExpression(null, [.. @object.Properties()
                    .Where(x => SupportedJsonTokenTypes.Contains(x.Value.Type))
                    .Select(x => new ObjectPropertyExpression(null, new StringLiteralExpression(null, x.Name), ConvertJsonToExpression(x.Value)))]),
                JArray @array => new ArrayExpression(null, [.. @array
                    .Where(x => SupportedJsonTokenTypes.Contains(x.Type))
                    .Select(ConvertJsonToExpression)]),
                JValue value => value.Type switch
                {
                    JTokenType.String => new StringLiteralExpression(null, value.ToString(CultureInfo.InvariantCulture)),
                    JTokenType.Integer => new IntegerLiteralExpression(null, value.ToObject<long>()),
                    // Floats are currently not supported in Bicep, so fall back to the default behavior of "any"
                    JTokenType.Float => new FunctionCallExpression(null, "json", [new StringLiteralExpression(null, value.ToObject<double>().ToString(CultureInfo.InvariantCulture))]),
                    JTokenType.Boolean => new BooleanLiteralExpression(null, value.ToObject<bool>()),
                    JTokenType.Null => new NullLiteralExpression(null),
                    _ => throw new InvalidOperationException($"Cannot parse JSON object. Unsupported value token type: {value.Type}"),
                },
                _ => throw new InvalidOperationException($"Cannot parse JSON object. Unsupported token: {token.Type}")
            };

        private static TypeSymbol GetItemsReturnType(TypeSymbol keyType, TypeSymbol valueType)
            => new TypedArrayType(
                new ObjectType(
                    "object",
                    TypeSymbolValidationFlags.Default,
                    new[] {
                        new NamedTypeProperty("key", keyType, Description: "The key of the object property being iterated over."),
                        new NamedTypeProperty("value", valueType, Description: "The value of the object property being iterated over."),
                    },
                    null),
                TypeSymbolValidationFlags.Default);

        private static (IReadOnlyList<TypeSymbol> keyTypes, IReadOnlyList<TypeSymbol> valueTypes) GetReadableObjectKeysAndValues(ObjectType objectType)
        {
            var keyTypes = new List<TypeSymbol>();
            var valueTypes = new List<TypeSymbol>();
            foreach (var property in objectType.Properties.Values)
            {
                if (property.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                {
                    // we're reading the values - exclude write-only properties
                    continue;
                }

                keyTypes.Add(TypeFactory.CreateStringLiteralType(property.Name));
                valueTypes.Add(property.TypeReference.Type);
            }

            if (objectType.AdditionalProperties?.TypeReference.Type is { } additionalPropertiesType)
            {
                keyTypes.Add(LanguageConstants.String);
                valueTypes.Add(additionalPropertiesType);
            }

            return (keyTypes, valueTypes);
        }

        private static FunctionResult ItemsResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            if (argumentTypes[0] is not ObjectType objectType)
            {
                return new(GetItemsReturnType(LanguageConstants.String, LanguageConstants.Any));
            }

            var (keyTypes, valueTypes) = GetReadableObjectKeysAndValues(objectType);

            return new(GetItemsReturnType(
                keyType: TypeHelper.CreateTypeUnion(keyTypes),
                valueType: TypeHelper.TryCollapseTypes(valueTypes) ?? LanguageConstants.Any));
        }

        private static FunctionResult ObjectKeysResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            if (argumentTypes[0] is not ObjectType objectType)
            {
                return new(new TypedArrayType(LanguageConstants.String, argumentTypes[0].ValidationFlags));
            }

            var (keyTypes, _) = GetReadableObjectKeysAndValues(objectType);
            return new(new TypedArrayType(TypeHelper.CreateTypeUnion(keyTypes), objectType.ValidationFlags));
        }

        public static TypeSymbol ConvertJsonToBicepType(JToken token)
            => token switch
            {
                JObject @object => new ObjectType(
                    "object",
                    TypeSymbolValidationFlags.Default,
                    @object.Properties().Where(x => SupportedJsonTokenTypes.Contains(x.Value.Type)).Select(x => new NamedTypeProperty(x.Name, ConvertJsonToBicepType(x.Value), TypePropertyFlags.ReadOnly | TypePropertyFlags.ReadableAtDeployTime)),
                    null),
                JArray @array => new TypedArrayType(
                    TypeHelper.CreateTypeUnion(@array.Where(x => SupportedJsonTokenTypes.Contains(x.Type)).Select(ConvertJsonToBicepType)),
                    TypeSymbolValidationFlags.Default),
                JValue value => value.Type switch
                {
                    JTokenType.String => TypeFactory.CreateStringLiteralType(value.ToString(CultureInfo.InvariantCulture)),
                    JTokenType.Integer => TypeFactory.CreateIntegerLiteralType(value.ToLong()),
                    // Floats are currently not supported in Bicep, so fall back to the default behavior of "any"
                    JTokenType.Float => LanguageConstants.Any,
                    JTokenType.Boolean => TypeFactory.CreateBooleanLiteralType(value.ToObject<bool>()),
                    JTokenType.Null => LanguageConstants.Null,
                    _ => LanguageConstants.Any,
                },
                _ => LanguageConstants.Any,
            };

        private static FunctionResult JsonResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            if (argumentTypes.Length != 1 || argumentTypes[0] is not StringLiteralType stringLiteral)
            {
                return new(LanguageConstants.Any);
            }

            // Purposefully use the same method and parsing settings as the deployment engine,
            // to provide as much consistency as possible.
            if (stringLiteral.RawStringValue.TryFromJson<JToken>() is not { } token)
            {
                // Instead of catching and returning the JSON parse exception, we simply return a generic error.
                // This avoids having to deal with localization, and avoids possible confusion regarding line endings in the message.
                // If the in-line JSON is so complex that troubleshooting is difficult, then that's a sign that the user should
                // instead break it out into a separate file and use loadTextContent().
                var error = DiagnosticBuilder.ForPosition(arguments[0].Expression).UnparsableJsonType();

                return new(ErrorType.Create(error));
            }

            return new(ConvertJsonToBicepType(token));
        }

        // TODO: Add copyIndex here when we support loops.
        private static readonly ImmutableArray<BannedFunction> BannedFunctions =
        [
            /*
             * The true(), false(), and null() functions are not included in this list because
             * we parse true, false and null as keywords in the lexer, so they can't be used as functions anyway.
             */

            new BannedFunction("variables", builder => builder.VariablesFunctionNotSupported()),
            new BannedFunction("parameters", builder => builder.ParametersFunctionNotSupported()),
            new BannedFunction("if", builder => builder.IfFunctionNotSupported()),
            new BannedFunction("createArray", builder => builder.CreateArrayFunctionNotSupported()),
            new BannedFunction("createObject", builder => builder.CreateObjectFunctionNotSupported()),

            BannedFunction.CreateForOperator("add", "+"),
            BannedFunction.CreateForOperator("sub", "-"),
            BannedFunction.CreateForOperator("mul", "*"),
            BannedFunction.CreateForOperator("div", "/"),
            BannedFunction.CreateForOperator("mod", "%"),
            BannedFunction.CreateForOperator("less", "<"),
            BannedFunction.CreateForOperator("lessOrEquals", "<="),
            BannedFunction.CreateForOperator("greater", ">"),
            BannedFunction.CreateForOperator("greaterOrEquals", ">="),
            BannedFunction.CreateForOperator("equals", "=="),
            BannedFunction.CreateForOperator("not", "!"),
            BannedFunction.CreateForOperator("and", "&&"),
            BannedFunction.CreateForOperator("or", "||"),
            BannedFunction.CreateForOperator("coalesce", "??")
        ];

        private static IEnumerable<NamespaceValue<Decorator>> GetSystemDecorators(IFeatureProvider featureProvider)
        {
            static SyntaxBase SingleArgumentSelector(DecoratorSyntax decoratorSyntax) => decoratorSyntax.Arguments.Single().Expression;

            static long? TryGetIntegerLiteralValue(SyntaxBase syntax) => syntax switch
            {
                // if integerLiteralSyntax.Value is within the 64 bit integer range, negate it after casting to a long type
                // long.MaxValue + 1 (9,223,372,036,854,775,808) is the only invalid 64 bit integer value that may be passed. we avoid casting to a long because this causes overflow. we need to just return long.MinValue (-9,223,372,036,854,775,808)
                // if integerLiteralSyntax.Value is outside the range, return null. it should have already been caught by a different validation
                UnaryOperationSyntax { Operator: UnaryOperator.Minus } unaryOperatorSyntax
                    when unaryOperatorSyntax.Expression is IntegerLiteralSyntax integerLiteralSyntax => integerLiteralSyntax.Value switch
                    {
                        <= long.MaxValue => -(long)integerLiteralSyntax.Value,
                        (ulong)long.MaxValue + 1 => long.MinValue,
                        _ => null
                    },

                // this ternary check is to make sure that the integer value is within the range of a signed 64 bit integer before casting to a long type
                // if not, it would have been caught already by a different validation
                IntegerLiteralSyntax integerLiteralSyntax => integerLiteralSyntax.Value <= long.MaxValue ? (long)integerLiteralSyntax.Value : null,
                _ => null,
            };

            static SyntaxBase? UnwrapNullableSyntax(SyntaxBase? maybeNullable) => maybeNullable switch
            {
                NullableTypeSyntax nullable => nullable.Base,
                var otherwise => otherwise,
            };

            static void EmitDiagnosticIfTargetingAlias(string decoratorName, DecoratorSyntax decoratorSyntax, SyntaxBase? decoratorParentTypeSyntax, IBinder binder, IDiagnosticWriter diagnosticWriter)
            {
                if (RefersToTypeAlias(decoratorParentTypeSyntax, binder))
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DecoratorMayNotTargetTypeAlias(decoratorName));
                }
            }

            static bool RefersToTypeAlias(SyntaxBase? typeSyntax, IBinder binder) => UnwrapNullableSyntax(typeSyntax) switch
            {
                TypeVariableAccessSyntax variableAccess => binder.GetSymbolInfo(variableAccess) is TypeAliasSymbol or ImportedTypeSymbol or WildcardImportSymbol,
                TypePropertyAccessSyntax typePropertyAccess => RefersToTypeAlias(typePropertyAccess.BaseExpression, binder),
                TypeAdditionalPropertiesAccessSyntax typeAdditionalPropertiesAccess => RefersToTypeAlias(typeAdditionalPropertiesAccess.BaseExpression, binder),
                TypeArrayAccessSyntax typeArrayAccess => RefersToTypeAlias(typeArrayAccess.BaseExpression, binder) || RefersToTypeAlias(typeArrayAccess.IndexExpression, binder),
                TypeItemsAccessSyntax typeItemsAccess => RefersToTypeAlias(typeItemsAccess.BaseExpression, binder),
                _ => false,
            };

            static void EmitDiagnosticIfTargetingLiteral(string decoratorName, DecoratorSyntax decoratorSyntax, SyntaxBase? decoratorParentTypeSyntax, ITypeManager typeManager, IDiagnosticWriter diagnosticWriter)
            {
                if (IsLiteralSyntax(UnwrapNullableSyntax(decoratorParentTypeSyntax), typeManager))
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DecoratorNotPermittedOnLiteralType(decoratorName));
                }
            }

            static void ValidateNotTargetingAlias(string decoratorName, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IBinder binder, IDiagnosticLookup parsingErrorLookup, IDiagnosticWriter diagnosticWriter)
                => EmitDiagnosticIfTargetingAlias(decoratorName, decoratorSyntax, GetDeclaredTypeSyntaxOfParent(decoratorSyntax, binder), binder, diagnosticWriter);

            static void ValidateLength(string decoratorName, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IBinder binder, IDiagnosticLookup parsingErrorLookup, IDiagnosticWriter diagnosticWriter)
            {
                ValidateNotTargetingAlias(decoratorName, decoratorSyntax, targetType, typeManager, binder, parsingErrorLookup, diagnosticWriter);

                if (targetType is UnionType || TypeHelper.IsLiteralType(targetType))
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DecoratorNotPermittedOnLiteralType(decoratorName));
                }

                var lengthSyntax = SingleArgumentSelector(decoratorSyntax);

                if (TryGetIntegerLiteralValue(lengthSyntax) is not null and < 0)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(lengthSyntax).LengthMustNotBeNegative());
                }
            }

            static IEnumerable<Decorator> GetAlwaysPermittedDecorators()
            {
                yield return new DecoratorBuilder(LanguageConstants.MetadataDescriptionPropertyName)
                    .WithDescription("Describes the parameter.")
                    .WithParameter("text", LanguageConstants.String, "The description.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.AnyDecorator)
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is DescribableExpression describable &&
                            functionCall.Parameters.FirstOrDefault() is { } description)
                        {
                            if (description is StringLiteralExpression stringLiteral)
                            {
                                description = stringLiteral with { Value = StringUtils.NormalizeIndent(stringLiteral.Value) };
                            }

                            return describable with { Description = description };
                        }

                        return decorated;
                    })
                    .Build();
            }

            static IEnumerable<Decorator> GetBicepTemplateDecorators(IFeatureProvider featureProvider)
            {
                var secureAttachableType = TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Object);
                yield return new DecoratorBuilder(LanguageConstants.ParameterSecurePropertyName)
                    .WithDescription("Makes the parameter a secure parameter.")
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithAttachableType(secureAttachableType)
                    .WithValidator((decoratorName, decoratorSyntax, targetType, typeManager, binder, parsingErrorLookup, diagnosticWriter) =>
                    {
                        if (!TypeValidator.AreTypesAssignable(targetType, secureAttachableType))
                        {
                            // skip further validation if we're already reporting an error
                            return;
                        }

                        if (TypeHelper.TryGetArmPrimitiveType(targetType) is null)
                        {
                            // if we won't be emitting a "type" constraint in the compiled JSON, we can't make it a secure type
                            diagnosticWriter.Write(
                                DiagnosticBuilder.ForPosition(decoratorSyntax).SecureDecoratorTargetMustFitWithinStringOrObject());
                        }

                        ValidateNotTargetingAlias(decoratorName, decoratorSyntax, targetType, typeManager, binder, parsingErrorLookup, diagnosticWriter);
                    })
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression)
                        {
                            return typeDeclaringExpression with { Secure = functionCall };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ParameterAllowedPropertyName)
                    .WithDescription("Defines the allowed values of the parameter.")
                    .WithParameter("values", LanguageConstants.Array, "The allowed values.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterDecorator)
                    .WithValidator((decoratorName, decoratorSyntax, targetType, typeManager, binder, parsingErrorLookup, diagnosticWriter) =>
                    {
                        var parentTypeSyntax = GetDeclaredTypeSyntaxOfParent(decoratorSyntax, binder);

                        EmitDiagnosticIfTargetingAlias(decoratorName, decoratorSyntax, parentTypeSyntax, binder, diagnosticWriter);
                        EmitDiagnosticIfTargetingLiteral(decoratorName, decoratorSyntax, parentTypeSyntax, typeManager, diagnosticWriter);

                        if (TypeValidator.AreTypesAssignable(targetType, LanguageConstants.Array) &&
                            SingleArgumentSelector(decoratorSyntax) is ArraySyntax allowedValues &&
                            allowedValues.Items.All(item => item.Value is not ArraySyntax))
                        {
                            /*
                             * ARM handles array params with allowed values differently. If none of items of
                             * the allowed values is array, it will check if the parameter value is a subset
                             * of the allowed values.
                             */
                            return;
                        }

                        TypeValidator.NarrowTypeAndCollectDiagnostics(
                            typeManager,
                            binder,
                            parsingErrorLookup,
                            diagnosticWriter,
                            SingleArgumentSelector(decoratorSyntax),
                            new TypedArrayType(targetType, TypeSymbolValidationFlags.Default));
                    })
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is DeclaredParameterExpression declaredParameterExpression &&
                            functionCall.Parameters.FirstOrDefault() is { } allowedValues)
                        {
                            return declaredParameterExpression with { AllowedValues = allowedValues };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ParameterMinValuePropertyName)
                    .WithDescription("Defines the minimum value of the parameter.")
                    .WithParameter("value", LanguageConstants.Int, "The minimum value.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithAttachableType(LanguageConstants.Int)
                    .WithValidator(ValidateNotTargetingAlias)
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression &&
                            functionCall.Parameters.FirstOrDefault() is { } minValue)
                        {
                            return typeDeclaringExpression with { MinValue = minValue };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ParameterMaxValuePropertyName)
                    .WithDescription("Defines the maximum value of the parameter.")
                    .WithParameter("value", LanguageConstants.Int, "The maximum value.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithAttachableType(LanguageConstants.Int)
                    .WithValidator(ValidateNotTargetingAlias)
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression &&
                            functionCall.Parameters.FirstOrDefault() is { } maxValue)
                        {
                            return typeDeclaringExpression with { MaxValue = maxValue };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ParameterMinLengthPropertyName)
                    .WithDescription("Defines the minimum length of the parameter.")
                    .WithParameter("length", LanguageConstants.Int, "The minimum length.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithAttachableType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Array))
                    .WithValidator(ValidateLength)
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression &&
                            functionCall.Parameters.FirstOrDefault() is { } minLength)
                        {
                            return typeDeclaringExpression with { MinLength = minLength };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ParameterMaxLengthPropertyName)
                    .WithDescription("Defines the maximum length of the parameter.")
                    .WithParameter("length", LanguageConstants.Int, "The maximum length.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithAttachableType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Array))
                    .WithValidator(ValidateLength)
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression &&
                            functionCall.Parameters.FirstOrDefault() is { } maxLength)
                        {
                            return typeDeclaringExpression with { MaxLength = maxLength };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ParameterMetadataPropertyName)
                    .WithDescription("Defines metadata of the parameter.")
                    .WithParameter("object", LanguageConstants.Object, "The metadata object.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithValidator((_, decoratorSyntax, _, typeManager, binder, parsingErrorLookup, diagnosticWriter) =>
                        TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, parsingErrorLookup, diagnosticWriter, SingleArgumentSelector(decoratorSyntax), LanguageConstants.ParameterModifierMetadata))
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression &&
                            functionCall.Parameters.FirstOrDefault() is { } metadata)
                        {
                            return typeDeclaringExpression with { Metadata = metadata };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.BatchSizePropertyName)
                    .WithDescription("Causes the resource or module for-expression to be run in sequential batches of specified size instead of the default behavior where all the resources or modules are deployed in parallel.")
                    .WithParameter(LanguageConstants.BatchSizePropertyName, LanguageConstants.Int, "The size of the batch", FunctionParameterFlags.Required)
                    .WithFlags(FunctionFlags.ResourceOrModuleDecorator)
                    // the decorator is constrained to resources and modules already - checking for array alone is simple and should be sufficient
                    .WithValidator((decoratorName, decoratorSyntax, targetType, typeManager, binder, _, diagnosticWriter) =>
                    {
                        if (!TypeValidator.AreTypesAssignable(targetType, LanguageConstants.Array))
                        {
                            // the resource/module declaration is not a collection
                            // (the compile-time constant and resource/module placement is already enforced, so we don't need a deeper type check)
                            diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).BatchSizeNotAllowed(decoratorName));
                        }

                        const long minimumBatchSize = 1;
                        SyntaxBase batchSizeSyntax = SingleArgumentSelector(decoratorSyntax);
                        long? batchSize = TryGetIntegerLiteralValue(batchSizeSyntax);

                        if (batchSize is not null and < minimumBatchSize)
                        {
                            diagnosticWriter.Write(DiagnosticBuilder.ForPosition(batchSizeSyntax).BatchSizeTooSmall(batchSize.Value, minimumBatchSize));
                        }
                    })
                    .Build();

                if (featureProvider.WaitAndRetryEnabled)
                {
                    yield return new DecoratorBuilder(LanguageConstants.WaitUntilPropertyName)
                        .WithDescription("Causes the resource deployment to wait until the given condition is satisfied")
                        .WithParameter(
                            name: "predicate",
                            type: OneParamLambda(LanguageConstants.Object, LanguageConstants.Bool),
                            description: "The predicate applied to the resource.",
                            flags: FunctionParameterFlags.Required)
                        .WithParameter(
                            name: "maxWaitTime",
                            type: LanguageConstants.String,
                            description: "Maximum time used to wait until the predicate is true. Please be cautious as max wait time adds to total deployment time. It cannot be a negative value. Use [ISO 8601 duration format](https://en.wikipedia.org/wiki/ISO_8601#Durations).",
                            flags: FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                        .WithFlags(FunctionFlags.ResourceDecorator)// the decorator is constrained to resources
                        .WithEvaluator(AddDecoratorConfigToResource)
                        .Build();

                    yield return new DecoratorBuilder(LanguageConstants.RetryOnPropertyName)
                        .WithDescription("Causes the resource deployment to retry when deployment failed with one of the exceptions listed")
                        .WithParameter("exceptionCodes", LanguageConstants.StringArray, "List of exceptions.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                        .WithParameter("retryCount", TypeFactory.CreateIntegerType(minValue: 1), "Maximum number if retries on the exception.", FunctionParameterFlags.Constant)
                        .WithFlags(FunctionFlags.ResourceDecorator)// the decorator is constrained to resources
                        .WithEvaluator(AddDecoratorConfigToResource)
                        .Build();
                }


                yield return new DecoratorBuilder(LanguageConstants.OnlyIfNotExistsPropertyName)
                    .WithDescription("Causes the resource deployment to be skipped if the resource already exists")
                    .WithFlags(FunctionFlags.ResourceDecorator)// the decorator is constrained to resources
                    .WithEvaluator(AddDecoratorConfigToResource)
                    .Build();


                yield return new DecoratorBuilder(LanguageConstants.ParameterSealedPropertyName)
                    .WithDescription("Marks an object parameter as only permitting properties specifically included in the type definition")
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithAttachableType(LanguageConstants.Object)
                    .WithValidator((decoratorName, decoratorSyntax, targetType, typeManager, binder, parsingErrorLookup, diagnosticWriter) =>
                    {
                        switch (UnwrapNullableSyntax(GetDeclaredTypeSyntaxOfParent(decoratorSyntax, binder)))
                        {
                            case VariableAccessSyntax variableAccess when binder.GetSymbolInfo(variableAccess) is not AmbientTypeSymbol:
                                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DecoratorMayNotTargetTypeAlias(decoratorName));
                                break;
                            case AccessExpressionSyntax accessExpression when binder.GetSymbolInfo(accessExpression.BaseExpression) is not BuiltInNamespaceSymbol:
                                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DecoratorMayNotTargetTypeAlias(decoratorName));
                                break;
                            case ParameterizedTypeInstantiationSyntaxBase parameterized when LanguageConstants.ResourceDerivedTypeNames.Contains(parameterized.Name.IdentifierName):
                                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DecoratorMayNotTargetResourceDerivedType(decoratorName));
                                break;
                            case ObjectTypeSyntax @object when @object.AdditionalProperties is not null:
                                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).SealedIncompatibleWithAdditionalPropertiesDeclaration());
                                break;
                        }
                    })
                    .WithEvaluator((functionCall, decorated) =>
                    {
                        if (decorated is TypeDeclaringExpression typeDeclaringExpression)
                        {
                            return typeDeclaringExpression with { Sealed = functionCall };
                        }

                        return decorated;
                    })
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.TypeDiscriminatorDecoratorName)
                    .WithDescription("Defines the discriminator property to use for a tagged union that is shared between all union members")
                    .WithParameter("value", LanguageConstants.String, "The discriminator property name.", FunctionParameterFlags.Required | FunctionParameterFlags.Constant)
                    .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                    .WithValidator(ValidateTypeDiscriminator)
                    .WithAttachableType(LanguageConstants.Object)
                    .Build();

                yield return new DecoratorBuilder(LanguageConstants.ExportPropertyName)
                    .WithDescription("Allows a type, variable, or function to be imported into other Bicep files.")
                    .WithFlags(FunctionFlags.TypeVariableOrFunctionDecorator)
                    .WithEvaluator(static (functionCall, decorated) => decorated switch
                    {
                        DeclaredTypeExpression declaredType => declaredType with { Exported = functionCall },
                        DeclaredVariableExpression declaredVariable => declaredVariable with { Exported = functionCall },
                        DeclaredFunctionExpression declaredFunction => declaredFunction with { Exported = functionCall },
                        _ => decorated,
                    })
                    .WithValidator(static (decoratorName, decoratorSyntax, _, _, binder, _, diagnosticWriter) =>
                    {
                        var decoratorTarget = binder.GetParent(decoratorSyntax);

                        if (decoratorTarget is not ITopLevelNamedDeclarationSyntax)
                        {
                            diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).ExportDecoratorMustTargetStatement());
                        }

                        if (decoratorTarget is not null && binder.GetSymbolInfo(decoratorTarget) is DeclaredSymbol targetedDeclaration)
                        {
                            var nonExportableSymbolsInClosure = binder.GetReferencedSymbolClosureFor(targetedDeclaration)
                                .Where(s => s is not VariableSymbol and
                                    not TypeAliasSymbol and
                                    not DeclaredFunctionSymbol and
                                    not ImportedSymbol and
                                    not WildcardImportSymbol and
                                    not LocalVariableSymbol)
                                .Select(s => s.Name)
                                .Order()
                                .ToImmutableArray();

                            if (nonExportableSymbolsInClosure.Any())
                            {
                                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).ClosureContainsNonExportableSymbols(nonExportableSymbolsInClosure));
                            }
                        }
                    })
                    .Build();

                if (featureProvider.UserDefinedConstraintsEnabled)
                {
                    yield return new DecoratorBuilder("validate")
                        .WithDescription("Applies a custom validation lambda to a type")
                        .WithFlags(FunctionFlags.ParameterOutputOrTypeDecorator)
                        .WithParameter(
                            name: "predicate",
                            type: OneParamLambda(LanguageConstants.Any, LanguageConstants.Bool),
                            description: "Validation predicate. Return true for valid values.",
                            flags: FunctionParameterFlags.Required | FunctionParameterFlags.DeployTimeConstant,
                            calculator: static (_, getAttachedType) => OneParamLambda(getAttachedType(), LanguageConstants.Bool))
                        .WithParameter(
                            name: "errorMessage",
                            type: LanguageConstants.String,
                            description: "Error message to use when the value is not valid according to the predicate.",
                            flags: FunctionParameterFlags.Constant)
                        .WithEvaluator(static (functionCall, decorated) =>
                        {
                            if (decorated is TypeDeclaringExpression typeDeclaringExpression)
                            {
                                return typeDeclaringExpression with
                                {
                                    UserDefinedConstraint = new ArrayExpression(functionCall.SourceSyntax, functionCall.Parameters),
                                };
                            }

                            return decorated;
                        })
                        .Build();
                }
            }

            foreach (var decorator in GetAlwaysPermittedDecorators())
            {
                yield return new(decorator, (_, _) => true);
            }

            foreach (var decorator in GetBicepTemplateDecorators(featureProvider))
            {
                yield return new(decorator, (_, sfk) => sfk == BicepSourceFileKind.BicepFile);
            }
        }

        private static void ValidateTypeDiscriminator(string decoratorName, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IBinder binder, IDiagnosticLookup parsingErrorLookup, IDiagnosticWriter diagnosticWriter)
        {
            if (targetType is not DiscriminatedObjectType && targetType is not ErrorType)
            {
                diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DiscriminatorDecoratorOnlySupportedForObjectUnions());
            }

            if (targetType is DiscriminatedObjectType discriminatedObjectType)
            {
                var discriminatorPropertyName = (decoratorSyntax.Arguments.FirstOrDefault()?.Expression as StringSyntax)?.TryGetLiteralValue();

                if (discriminatorPropertyName != null &&
                    discriminatorPropertyName != discriminatedObjectType.DiscriminatorKey)
                {
                    // case when a decorator is applied to type that is already a valid discriminated union
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).DiscriminatorPropertyNameMustMatch(discriminatorPropertyName));
                }
            }
        }

        private static SyntaxBase? GetDeclaredTypeSyntaxOfParent(DecoratorSyntax syntax, IBinder binder) => binder.GetParent(syntax) switch
        {
            ParameterDeclarationSyntax parameterDeclaration => parameterDeclaration.Type,
            OutputDeclarationSyntax outputDeclaration => outputDeclaration.Type,
            VariableDeclarationSyntax variableDeclaration => variableDeclaration.Type,
            TypeDeclarationSyntax typeDeclaration => typeDeclaration.Value,
            ObjectTypePropertySyntax objectTypeProperty => objectTypeProperty.Value,
            ObjectTypeAdditionalPropertiesSyntax objectTypeAdditionalProperties => objectTypeAdditionalProperties.Value,
            TupleTypeItemSyntax tupleTypeItem => tupleTypeItem.Value,
            _ => null,
        };

        private static bool IsLiteralSyntax(SyntaxBase? syntax, ITypeManager typeManager) => syntax switch
        {
            IntegerTypeLiteralSyntax or
            BooleanTypeLiteralSyntax or
            UnaryTypeOperationSyntax or
            StringTypeLiteralSyntax or
            // union types may contain symbols, but the type manager will enforce that they must resolve to a flat union of literals
            UnionTypeSyntax => true,
            // certain aggregate types may contain symbols and still be literal types (iff the symbols themselves resolve to literal types)
            // unlike with union types, we get no guarantees from the type checker and need to inspect the declared type to verify that this is a literal
            ObjectTypeSyntax @object when TypeHelper.IsLiteralType(typeManager.GetDeclaredType(@object) ?? ErrorType.Empty()) => true,
            TupleTypeSyntax tuple when TypeHelper.IsLiteralType(typeManager.GetDeclaredType(tuple) ?? ErrorType.Empty()) => true,
            _ => false,
        };

        private static IEnumerable<NamespaceValue<NamedTypeProperty>> GetSystemAmbientSymbols()
        {
            static IEnumerable<NamedTypeProperty> GetArmPrimitiveTypes()
                => LanguageConstants.DeclarationTypes.Select(t => new NamedTypeProperty(t.Key, new TypeType(t.Value)));

            static IEnumerable<NamedTypeProperty> GetResourceDerivedTypesTypeProperties()
            {
                ImmutableArray<TypeParameter> resourceInputParameters = [new TypeParameter(
                    "ResourceTypeIdentifier",
                    "A string of the format `<type-name>@<api-version>` that identifies the kind of resource whose body type definition is to be used.",
                    LanguageConstants.StringResourceIdentifier)];

                static TypeTemplate.InstantiatorDelegate GetResourceDerivedTypeInstantiator(ResourceDerivedTypeVariant derivedTypeVariant)
                    => (binder, syntax, argumentTypes) =>
                    {
                        if (syntax.Arguments.FirstOrDefault()?.Expression is not StringTypeLiteralSyntax stringArg || stringArg.SegmentValues.Length > 1)
                        {
                            return new(DiagnosticBuilder.ForPosition(TextSpan.BetweenExclusive(syntax.OpenChevron, syntax.CloseChevron)).CompileTimeConstantRequired());
                        }

                        if (!TypeHelper.GetResourceTypeFromString(binder, stringArg.SegmentValues[0], ResourceTypeGenerationFlags.None, parentResourceType: null)
                            .IsSuccess(out var resourceType, out var errorBuilder))
                        {
                            return new(errorBuilder(DiagnosticBuilder.ForPosition(syntax.GetArgumentByPosition(0))));
                        }

                        return new(new ResourceDerivedTypeExpression(syntax, resourceType, derivedTypeVariant));
                    };

                var resourceDerivedTypeNotaBene = "NB: The type definition will be checked by Bicep when the template is compiled but will not be enforced by the ARM engine during a deployment.";

                yield return new(
                    LanguageConstants.TypeNameResourceInput,
                    new TypeTemplate(
                        LanguageConstants.TypeNameResourceInput,
                        resourceInputParameters,
                        GetResourceDerivedTypeInstantiator(ResourceDerivedTypeVariant.Input)),
                    Description: $"""
                        Use the type definition of the input for a specific resource rather than a user-defined type.

                        {resourceDerivedTypeNotaBene}
                        """);

                yield return new(
                    LanguageConstants.TypeNameResourceOutput,
                    new TypeTemplate(
                        LanguageConstants.TypeNameResourceOutput,
                        resourceInputParameters,
                        GetResourceDerivedTypeInstantiator(ResourceDerivedTypeVariant.Output)),
                    Description: $"""
                        Use the type definition of the return value of a specific resource rather than a user-defined type.

                        {resourceDerivedTypeNotaBene}
                        """);

            }

            foreach (var typeProp in GetArmPrimitiveTypes())
            {
                yield return new(typeProp, (features, sfk) => true);
            }

            foreach (var typeProp in GetResourceDerivedTypesTypeProperties())
            {
                yield return new(typeProp, (features, sfk) => sfk == BicepSourceFileKind.BicepFile);
            }
        }

        private static string? GetNameOfReturnValue(SyntaxBase syntax)
        {
            return syntax switch
            {
                VariableAccessSyntax variableAccess => variableAccess.Name.IdentifierName,
                PropertyAccessSyntax propertyAccess => propertyAccess.PropertyName.IdentifierName,
                ResourceAccessSyntax resourceAccess => resourceAccess.ResourceName.IdentifierName,
                ModuleDeclarationSyntax moduleDeclaration => moduleDeclaration.Name.IdentifierName,
                ArrayAccessSyntax { IndexExpression: StringSyntax indexExpression } when indexExpression.TryGetLiteralValue() is { } literalValue => literalValue,
                _ => null,
            };
        }

        public static NamespaceType Create(string aliasName, IFeatureProvider featureProvider, BicepSourceFileKind sourceFileKind)
        {
            return new NamespaceType(
                aliasName,
                Settings,
                AmbientSymbols.Where(x => x.IsVisible(featureProvider, sourceFileKind)).Select(x => x.Value),
                GetSystemOverloads(featureProvider).Where(x => x.IsVisible(featureProvider, sourceFileKind)).Select(x => x.Value),
                BannedFunctions,
                GetSystemDecorators(featureProvider).Where(x => x.IsVisible(featureProvider, sourceFileKind)).Select(x => x.Value),
                new EmptyResourceTypeProvider());
        }

        private static Expression AddDecoratorConfigToResource(FunctionCallExpression functionCall, Expression decorated)
        {
            if (decorated is DeclaredResourceExpression declaredResourceExpression)
            {
                return declaredResourceExpression with
                {
                    DecoratorConfig = declaredResourceExpression.DecoratorConfig.Add(functionCall.Name, new ArrayExpression(functionCall.SourceSyntax, functionCall.Parameters))
                };
            }
            return decorated;
        }

        private static FunctionResult LoadDirectoryFileInfoResultBuilder(SemanticModel model, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments;
            var pathSearchPattern = string.Empty;
            if (arguments.Length > 1)
            {
                if (argumentTypes[1] is not StringLiteralType tokenSelectorType)
                {
                    return new FunctionResult(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[1]).CompileTimeConstantRequired()));
                }
                pathSearchPattern = tokenSelectorType.RawStringValue;
            }

            if (TryLoadFilesFromDirectoryPath(model, (arguments[0], argumentTypes[0]), pathSearchPattern)
                    .IsSuccess(out var result, out var errorDiagnostic))
            {
                var token = result
                    // ensure determinism by ordering the results
                    .OrderByAscending(x => x.RelativePath, StringComparer.OrdinalIgnoreCase)
                    .ToJToken();

                return new FunctionResult(ConvertJsonToBicepType(token), ConvertJsonToExpression(token));
            }

            return new FunctionResult(ErrorType.Create(errorDiagnostic));
        }

        private static ResultWithDiagnostic<IEnumerable<LoadDirectoryFileInfoResult>> TryLoadFilesFromDirectoryPath(SemanticModel model, (FunctionArgumentSyntax syntax, TypeSymbol typeSymbol) directoryPathArgument, string pathSearchPattern)
        {
            if (directoryPathArgument.typeSymbol is not StringLiteralType directoryPathType)
            {
                return new(DiagnosticBuilder.ForPosition(directoryPathArgument.syntax).CompileTimeConstantRequired());
            }

            var directoryFileLoadResult = RelativePath.TryCreate(directoryPathType.RawStringValue).Transform(path => model.SourceFile.TryListFilesInDirectory(path, pathSearchPattern));

            if (!directoryFileLoadResult.IsSuccess(out var directoryFiles, out var errorBuilder))
            {
                return new(errorBuilder(DiagnosticBuilder.ForPosition(directoryPathArgument.syntax)));
            }

            var thisFileUri = model.SourceFile.FileHandle.Uri;
            return new(directoryFiles.Select(uri =>
            {
                var baseName = uri.GetFileName();
                var extension = uri.GetExtension().ToString();
                // avoid returning "" if the file is the same as the current file
                var relativePath = uri == thisFileUri ? baseName : uri.GetPathRelativeTo(thisFileUri);

                return new LoadDirectoryFileInfoResult(
                    RelativePath: relativePath,
                    BaseName: baseName,
                    Extension: extension);
            }));
        }
    }
}
