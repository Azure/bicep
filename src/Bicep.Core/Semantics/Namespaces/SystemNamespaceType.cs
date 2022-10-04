// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Bicep.Core.Semantics.FunctionOverloadBuilder;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class SystemNamespaceType
    {
        public const string BuiltInName = "sys";
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

        private static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: BuiltInName,
            ConfigurationType: null,
            ArmTemplateProviderName: "System",
            ArmTemplateProviderVersion: "1.0");

        private static IEnumerable<FunctionOverload> GetSystemOverloads(IFeatureProvider featureProvider)
        {
            yield return new FunctionOverloadBuilder(LanguageConstants.AnyFunction)
                .WithReturnType(LanguageConstants.Any)
                .WithGenericDescription("Converts the specified value to the `any` type.")
                .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to `any` type")
                .WithEvaluator((functionCall, _, _, _, _) => functionCall.Arguments.Single().Expression)
                .Build();

            yield return new FunctionOverloadBuilder("concat")
                .WithReturnType(LanguageConstants.Array)
                .WithGenericDescription(ConcatDescription)
                .WithDescription("Combines multiple arrays and returns the concatenated array.")
                .WithVariableParameter("arg", LanguageConstants.Array, minimumCount: 1, "The array for concatenation")
                .Build();

            yield return new FunctionOverloadBuilder("concat")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription(ConcatDescription)
                .WithDescription("Combines multiple string, integer, or boolean values and returns them as a concatenated string.")
                .WithVariableParameter("arg", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool), minimumCount: 1, "The string, int, or boolean value for concatenation")
                .Build();

            yield return new FunctionOverloadBuilder("format")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription("Creates a formatted string from input values.")
                .WithRequiredParameter("formatString", LanguageConstants.String, "The composite format string.")
                .WithVariableParameter("arg", LanguageConstants.Any, minimumCount: 0, "The value to include in the formatted string.")
                .Build();

            yield return new FunctionOverloadBuilder("base64")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("base64"), LanguageConstants.String)
                .WithGenericDescription("Returns the base64 representation of the input string.")
                .WithRequiredParameter("inputString", LanguageConstants.String, "The value to return as a base64 representation.")
                .Build();

            yield return new FunctionOverloadBuilder("padLeft")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription("Returns a right-aligned string by adding characters to the left until reaching the total specified length.")
                .WithRequiredParameter("valueToPad", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), "The value to right-align.")
                .WithRequiredParameter("totalLength", LanguageConstants.Int, "The total number of characters in the returned string.")
                .WithOptionalParameter("paddingCharacter", LanguageConstants.String, "The character to use for left-padding until the total length is reached. The default value is a space.")
                .Build();

            yield return new FunctionOverloadBuilder("replace")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("replace"), LanguageConstants.String)
                .WithGenericDescription("Returns a new string with all instances of one string replaced by another string.")
                .WithRequiredParameter("originalString", LanguageConstants.String, "The original string.")
                .WithRequiredParameter("oldString", LanguageConstants.String, "The string to be removed from the original string.")
                .WithRequiredParameter("newString", LanguageConstants.String, "The string to add in place of the removed string.")
                .Build();

            yield return new FunctionOverloadBuilder("toLower")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("toLower"), LanguageConstants.String)
                .WithGenericDescription("Converts the specified string to lower case.")
                .WithRequiredParameter("stringToChange", LanguageConstants.String, "The value to convert to lower case.")
                .Build();

            yield return new FunctionOverloadBuilder("toUpper")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("toUpper"), LanguageConstants.String)
                .WithGenericDescription("Converts the specified string to upper case.")
                .WithRequiredParameter("stringToChange", LanguageConstants.String, "The value to convert to upper case.")
                .Build();

            yield return new FunctionOverloadBuilder("length")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription("Returns the number of characters in a string, elements in an array, or root-level properties in an object.")
                .WithRequiredParameter("arg", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Object, LanguageConstants.Array), "The array to use for getting the number of elements, the string to use for getting the number of characters, or the object to use for getting the number of root-level properties.")
                .Build();

            yield return new FunctionOverloadBuilder("split")
                .WithReturnType(LanguageConstants.Array)
                .WithGenericDescription("Returns an array of strings that contains the substrings of the input string that are delimited by the specified delimiters.")
                .WithRequiredParameter("inputString", LanguageConstants.String, "The string to split.")
                .WithRequiredParameter("delimiter", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Array), "The delimiter to use for splitting the string.")
                .Build();

            yield return new FunctionOverloadBuilder("join")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription("Joins multiple strings into a single string, separated using a delimiter.")
                .WithRequiredParameter("inputArray", new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default), "An array of strings to join.")
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
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("uniqueString"), LanguageConstants.String)
                .WithGenericDescription("Creates a deterministic hash string based on the values provided as parameters. The returned value is 13 characters long.")
                .WithVariableParameter("arg", LanguageConstants.String, minimumCount: 1, "The value used in the hash function to create a unique string.")
                .Build();

            yield return new FunctionOverloadBuilder("guid")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("guid"), LanguageConstants.String)
                .WithGenericDescription("Creates a value in the format of a globally unique identifier based on the values provided as parameters.")
                .WithVariableParameter("arg", LanguageConstants.String, minimumCount: 1, "The value used in the hash function to create the GUID.")
                .Build();

            yield return new FunctionOverloadBuilder("trim")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("trim"), LanguageConstants.String)
                .WithGenericDescription("Removes all leading and trailing white-space characters from the specified string.")
                .WithRequiredParameter("stringToTrim", LanguageConstants.String, "The value to trim.")
                .Build();

            yield return new FunctionOverloadBuilder("uri")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription("Creates an absolute URI by combining the baseUri and the relativeUri string.")
                .WithRequiredParameter("baseUri", LanguageConstants.String, "The base uri string.")
                .WithRequiredParameter("relativeUri", LanguageConstants.String, "The relative uri string to add to the base uri string.")
                .Build();

            // TODO: Docs deviation
            yield return new FunctionOverloadBuilder("substring")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription("Returns a substring that starts at the specified character position and contains the specified number of characters.")
                .WithRequiredParameter("stringToParse", LanguageConstants.String, "The original string from which the substring is extracted.")
                .WithRequiredParameter("startIndex", LanguageConstants.Int, "The zero-based starting character position for the substring.")
                .WithOptionalParameter("length", LanguageConstants.Int, "The number of characters for the substring. Must refer to a location within the string. Must be zero or greater.")
                .Build();

            yield return new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.Array)
                .WithGenericDescription(TakeDescription)
                .WithDescription("Returns an array with the specified number of elements from the start of the array.")
                .WithRequiredParameter("originalValue", LanguageConstants.Array, "The array to take the elements from.")
                .WithRequiredParameter("numberToTake", LanguageConstants.Int, "The number of elements to take. If this value is 0 or less, an empty array is returned. If it is larger than the length of the given array, all the elements in the array are returned.")
                .Build();

            yield return new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription(TakeDescription)
                .WithDescription("Returns a string with the specified number of characters from the start of the string.")
                .WithRequiredParameter("originalValue", LanguageConstants.String, "The string to take the elements from.")
                .WithRequiredParameter("numberToTake", LanguageConstants.Int, "The number of characters to take. If this value is 0 or less, an empty string is returned. If it is larger than the length of the given string, all the characters are returned.")
                .Build();

            yield return new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.Array)
                .WithGenericDescription(SkipDescription)
                .WithDescription("Returns an array with all the elements after the specified number in the array.")
                .WithRequiredParameter("originalValue", LanguageConstants.Array, "The array to use for skipping.")
                .WithRequiredParameter("numberToSkip", LanguageConstants.Int, "The number of elements to skip. If this value is 0 or less, all the elements in the value are returned. If it is larger than the length of the array, an empty array is returned.")
                .Build();

            yield return new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription(SkipDescription)
                .WithDescription("Returns a string with all the characters after the specified number in the string.")
                .WithRequiredParameter("originalValue", LanguageConstants.String, "The string to use for skipping.")
                .WithRequiredParameter("numberToSkip", LanguageConstants.Int, "The number of characters to skip. If this value is 0 or less, all the characters in the value are returned. If it is larger than the length of the string, an empty string is returned.")
                .Build();

            yield return new FunctionOverloadBuilder("empty")
                .WithReturnType(LanguageConstants.Bool)
                .WithGenericDescription("Determines if an array, object, or string is empty.")
                .WithRequiredParameter("itemToTest", TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String), "The value to check if it is empty.")
                .Build();

            yield return new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithGenericDescription(ContainsDescription)
                .WithDescription("Checks whether an object contains a property. The property name comparison is case-insensitive.")
                .WithRequiredParameter("object", LanguageConstants.Object, "The object")
                .WithRequiredParameter("propertyName", LanguageConstants.String, "The property name.")
                .Build();

            yield return new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithGenericDescription(ContainsDescription)
                .WithDescription("Checks whether an array contains a value. For arrays of simple values, exact match is done (case-sensitive for strings). For arrays of objects or arrays a deep comparison is done.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array")
                .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithGenericDescription(ContainsDescription)
                .WithDescription("Checks whether a string contains a substring. The string comparison is case-sensitive.")
                .WithRequiredParameter("string", LanguageConstants.String, "The string.")
                .WithRequiredParameter("itemToFind", LanguageConstants.String, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("intersection")
                .WithReturnType(LanguageConstants.Object)
                .WithGenericDescription(IntersectionDescription)
                .WithDescription("Returns a single object with the common elements from the parameters.")
                .WithVariableParameter("object", LanguageConstants.Object, minimumCount: 2, "The object to use for finding common elements.")
                .Build();

            yield return new FunctionOverloadBuilder("intersection")
                .WithReturnType(LanguageConstants.Array)
                .WithGenericDescription(IntersectionDescription)
                .WithDescription("Returns a single array with the common elements from the parameters.")
                .WithVariableParameter("array", LanguageConstants.Array, minimumCount: 2, "The array to use for finding common elements.")
                .Build();

            yield return new FunctionOverloadBuilder("union")
                .WithReturnType(LanguageConstants.Object)
                .WithGenericDescription(UnionDescription)
                .WithDescription("Returns a single object with all elements from the parameters. Duplicate keys are only included once.")
                .WithVariableParameter("object", LanguageConstants.Object, minimumCount: 2, "The first object to use for joining elements.")
                .Build();

            yield return new FunctionOverloadBuilder("union")
                .WithReturnType(LanguageConstants.Array)
                .WithGenericDescription(UnionDescription)
                .WithDescription("Returns a single array with all elements from the parameters. Duplicate values are only included once.")
                .WithVariableParameter("object", LanguageConstants.Array, minimumCount: 2, "The first array to use for joining elements.")
                .Build();

            yield return new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.Any)
                .WithGenericDescription(FirstDescription)
                .WithDescription("Returns the first element of the array.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The value to retrieve the first element.")
                .Build();

            yield return new FunctionOverloadBuilder("first")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("first"), LanguageConstants.String)
                .WithGenericDescription(FirstDescription)
                .WithDescription("Returns the first character of the string.")
                .WithRequiredParameter("string", LanguageConstants.String, "The value to retrieve the first character.")
                .Build();

            yield return new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.Any)
                .WithGenericDescription(LastDescription)
                .WithDescription("Returns the last element of the array.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The value to retrieve the last element.")
                .Build();

            yield return new FunctionOverloadBuilder("last")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("last"), LanguageConstants.String)
                .WithGenericDescription(LastDescription)
                .WithDescription("Returns the last character of the string.")
                .WithRequiredParameter("string", LanguageConstants.String, "The value to retrieve the last character.")
                .Build();

            yield return new FunctionOverloadBuilder("indexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription("Returns the first position of a value within a string. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("indexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription("Returns the first position of a value within an array. For arrays of simple values, exact match is done (case-sensitive for strings). For arrays of objects or arrays a deep comparison is done.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array that contains the item to find.")
                .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("lastIndexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription("Returns the last position of a value within a string. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("lastIndexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription("Returns the last position of a value within an array. For arrays of simple values, exact match is done (case-sensitive for strings). For arrays of objects or arrays a deep comparison is done.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array that contains the item to find.")
                .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("startsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithGenericDescription("Determines whether a string starts with a value. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build();

            yield return new FunctionOverloadBuilder("endsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithGenericDescription("Determines whether a string ends with a value. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build();

            // TODO: Needs to support number type as well
            // TODO: Docs need updates
            yield return new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription(MinDescription)
                .WithDescription("Returns the minimum value from the specified integers.")
                .WithVariableParameter("int", LanguageConstants.Int, minimumCount: 1, "One of the integers used to calculate the minimum value")
                .Build();

            // TODO: Docs need updates
            yield return new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription(MinDescription)
                .WithDescription("Returns the minimum value from an array of integers.")
                .WithRequiredParameter("intArray", LanguageConstants.Array, "The array of integers.")
                .Build();

            // TODO: Needs to support number type as well
            // TODO: Docs need updates
            yield return new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription(MaxDescription)
                .WithDescription("Returns the maximum value from the specified integers.")
                .WithVariableParameter("int", LanguageConstants.Int, minimumCount: 1, "One of the integers used to calculate the maximum value")
                .Build();

            // TODO: Docs need updates
            yield return new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithGenericDescription(MaxDescription)
                .WithDescription("Returns the maximum value from an array of integers.")
                .WithRequiredParameter("intArray", LanguageConstants.Array, "The array of integers.")
                .Build();

            yield return new FunctionOverloadBuilder("range")
                .WithReturnType(new TypedArrayType(LanguageConstants.Int, TypeSymbolValidationFlags.Default))
                .WithGenericDescription("Creates an array of integers from a starting integer and containing a number of items.")
                .WithRequiredParameter("startIndex", LanguageConstants.Int, "The first integer in the array. The sum of startIndex and count must be no greater than 2147483647.")
                .WithRequiredParameter("count", LanguageConstants.Int, "The number of integers in the array. Must be non-negative integer up to 10000.")
                .Build();

            yield return new FunctionOverloadBuilder("base64ToString")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("base64ToString"), LanguageConstants.String)
                .WithGenericDescription("Converts a base64 representation to a string.")
                .WithRequiredParameter("base64Value", LanguageConstants.String, "The base64 representation to convert to a string.")
                .Build();

            yield return new FunctionOverloadBuilder("base64ToJson")
                .WithReturnType(LanguageConstants.Any)
                .WithGenericDescription("Converts a base64 representation to a JSON object.")
                .WithRequiredParameter("base64Value", LanguageConstants.String, "The base64 representation to convert to a JSON object.")
                .Build();

            yield return new FunctionOverloadBuilder("uriComponentToString")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("uriComponentToString"), LanguageConstants.String)
                .WithGenericDescription("Returns a string of a URI encoded value.")
                .WithRequiredParameter("uriEncodedString", LanguageConstants.String, "The URI encoded value to convert to a string.")
                .Build();

            yield return new FunctionOverloadBuilder("uriComponent")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("uriComponent"), LanguageConstants.String)
                .WithGenericDescription("Encodes a URI.")
                .WithRequiredParameter("stringToEncode", LanguageConstants.String, "The value to encode.")
                .Build();

            yield return new FunctionOverloadBuilder("dataUriToString")
                .WithGenericDescription("Converts a data URI formatted value to a string.")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("dataUriToString"), LanguageConstants.String)
                .WithRequiredParameter("dataUriToConvert", LanguageConstants.String, "The data URI value to convert.")
                .Build();

            // TODO: Docs have wrong param type and param name (any is actually supported)
            yield return new FunctionOverloadBuilder("dataUri")
                .WithReturnResultBuilder(PerformArmConversionOfStringLiterals("dataUri"), LanguageConstants.String)
                .WithGenericDescription("Converts a value to a data URI.")
                .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to a data URI.")
                .Build();

            yield return new FunctionOverloadBuilder("array")
                .WithGenericDescription("Converts the value to an array.")
                .WithReturnType(LanguageConstants.Array)
                .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to an array.")
                .Build();

            yield return new FunctionOverloadBuilder("coalesce")
                .WithReturnType(LanguageConstants.Any)
                .WithGenericDescription("Returns first non-null value from the parameters. Empty strings, empty arrays, and empty objects are not null.")
                .WithVariableParameter("arg", LanguageConstants.Any, minimumCount: 1, "The value to coalesce")
                .Build();

            // TODO: Requires number type
            //yield return new FunctionOverloadBuilder("float")
            //    .WithReturnType(LanguageConstants.Number)
            //    .WithDescription("Converts the value to a floating point number. You only use this function when passing custom parameters to an application, such as a Logic App.")
            //    .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to a floating point number.")
            //    .Build();

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
                .WithReturnType(LanguageConstants.String)
                .WithGenericDescription("Returns a value in the format of a globally unique identifier. **This function can only be used in the default value for a parameter**.")
                .WithFlags(FunctionFlags.ParamDefaultsOnly)
                .Build();

            yield return new FunctionOverloadBuilder("loadTextContent")
                .WithGenericDescription($"Loads the content of the specified file into a string. Content loading occurs during compilation, not at runtime. The maximum allowed content size is {LanguageConstants.MaxLiteralCharacterLimit} characters (including line endings).")
                .WithRequiredParameter("filePath", LanguageConstants.StringFilePath, "The path to the file that will be loaded.")
                .WithOptionalParameter("encoding", LanguageConstants.LoadTextContentEncodings, "File encoding. If not provided, UTF-8 will be used.")
                .WithReturnResultBuilder(LoadTextContentResultBuilder, LanguageConstants.String)
                .WithEvaluator(StringLiteralFunctionReturnTypeEvaluator)
                .WithVariableGenerator(StringLiteralFunctionVariableGenerator)
                .Build();

            yield return new FunctionOverloadBuilder("loadFileAsBase64")
                .WithGenericDescription($"Loads the specified file as base64 string. File loading occurs during compilation, not at runtime. The maximum allowed size is {LanguageConstants.MaxLiteralCharacterLimit / 4 * 3 / 1024} Kb.")
                .WithRequiredParameter("filePath", LanguageConstants.StringFilePath, "The path to the file that will be loaded.")
                .WithReturnResultBuilder(LoadContentAsBase64ResultBuilder, LanguageConstants.String)
                .WithEvaluator(StringLiteralFunctionReturnTypeEvaluator)
                .WithVariableGenerator(StringLiteralFunctionVariableGenerator)
                .Build();
            yield return new FunctionOverloadBuilder("loadJsonContent")
                .WithGenericDescription($"Loads the specified JSON file as bicep object. File loading occurs during compilation, not at runtime.")
                .WithRequiredParameter("filePath", LanguageConstants.StringJsonFilePath, "The path to the file that will be loaded.")
                .WithOptionalParameter("jsonPath", LanguageConstants.String, "JSONPath expression to narrow down the loaded file. If not provided, a root element indicator '$' is used")
                .WithOptionalParameter("encoding", LanguageConstants.LoadTextContentEncodings, "File encoding. If not provided, UTF-8 will be used.")
                .WithReturnResultBuilder(LoadJsonContentResultBuilder, LanguageConstants.Any)
                .WithEvaluator(JsonContentFunctionReturnTypeEvaluator)
                .WithVariableGenerator(JsonContentFunctionVariableGenerator)
                .Build();

            yield return new FunctionOverloadBuilder("items")
                .WithGenericDescription("Returns an array of keys and values for an object. Elements are consistently ordered alphabetically by key.")
                .WithRequiredParameter("object", LanguageConstants.Object, "The object to return keys and values for")
                .WithReturnResultBuilder(ItemsResultBuilder, GetItemsReturnType(LanguageConstants.String, LanguageConstants.Any))
                .Build();

            yield return new FunctionOverloadBuilder("flatten")
                .WithGenericDescription("Takes an array of arrays, and returns an array of sub-array elements, in the original order. Sub-arrays are only flattened once, not recursively.")
                .WithVariableParameter("array", new TypedArrayType(LanguageConstants.Array, TypeSymbolValidationFlags.Default), 0, "The array of sub-arrays to flatten.")
                .WithReturnType(LanguageConstants.Array)
                .Build();

            yield return new FunctionOverloadBuilder("filter")
                .WithGenericDescription("Filters an array with a custom filtering function.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array to filter.")
                .WithRequiredParameter("predicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.Bool), "The predicate applied to each input array element. If false, the item will be filtered out of the output array.",
                    calculator: getArgumentType => CalculateLambdaFromArrayParam(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.Bool)))
                .WithReturnResultBuilder((binder, fileResolver, diagnostics, arguments, argumentTypes) => {
                    return new(argumentTypes[0]);
                }, LanguageConstants.Array)
                .Build();

            yield return new FunctionOverloadBuilder("map")
                .WithGenericDescription("Applies a custom mapping function to each element of an array and returns the result array.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array to map.")
                .WithRequiredParameter("predicate", OneParamLambda(LanguageConstants.Any, LanguageConstants.Any), "The predicate applied to each input array element, in order to generate the output array.",
                    calculator: getArgumentType => CalculateLambdaFromArrayParam(getArgumentType, 0, t => OneParamLambda(t, LanguageConstants.Any)))
                .WithReturnResultBuilder((binder, fileResolver, diagnostics, arguments, argumentTypes) => argumentTypes[1] switch {
                    LambdaType lambdaType => new(new TypedArrayType(lambdaType.ReturnType.Type, TypeSymbolValidationFlags.Default)),
                    _ => new(LanguageConstants.Any),
                }, LanguageConstants.Array)
                .Build();

            yield return new FunctionOverloadBuilder("sort")
                .WithGenericDescription("Sorts an array with a custom sort function.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array to sort.")
                .WithRequiredParameter("predicate", TwoParamLambda(LanguageConstants.Any, LanguageConstants.Any, LanguageConstants.Bool), "The predicate used to compare two array elements for ordering. If true, the second element will be ordered after the first in the output array.",
                    calculator: getArgumentType => CalculateLambdaFromArrayParam(getArgumentType, 0, t => TwoParamLambda(t, t, LanguageConstants.Bool)))
                .WithReturnResultBuilder((binder, fileResolver, diagnostics, arguments, argumentTypes) => {
                    return new(argumentTypes[0]);
                }, LanguageConstants.Array)
                .Build();

            yield return new FunctionOverloadBuilder("reduce")
                .WithGenericDescription("Reduces an array with a custom reduce function.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array to reduce.")
                .WithRequiredParameter("initialValue", LanguageConstants.Any, "The initial value.")
                .WithRequiredParameter("predicate", TwoParamLambda(LanguageConstants.Any, LanguageConstants.Any, LanguageConstants.Any), "The predicate used to aggregate the current value and the next value. ",
                    calculator: getArgumentType => CalculateLambdaFromArrayParam(getArgumentType, 0, t => TwoParamLambda(t, t, LanguageConstants.Any)))
                .WithReturnType(LanguageConstants.Any)
                .WithReturnResultBuilder((binder, fileResolver, diagnostics, arguments, argumentTypes) => argumentTypes[2] switch {
                    LambdaType lambdaType => new(lambdaType.ReturnType.Type),
                    _ => new(LanguageConstants.Any),
                }, LanguageConstants.Array)
                .Build();
        }

        private static bool TryGetFileUriWithDiagnostics(IBinder binder, IFileResolver fileResolver, string filePath, SyntaxBase filePathArgument, [NotNullWhen(true)] out Uri? fileUri, [NotNullWhen(false)] out ErrorDiagnostic? error)
        {
            if (!LocalModuleReference.Validate(filePath, out var validateFilePathFailureBuilder))
            {
                fileUri = null;
                error = validateFilePathFailureBuilder.Invoke(DiagnosticBuilder.ForPosition(filePathArgument));
                return false;
            }
            fileUri = fileResolver.TryResolveFilePath(binder.FileSymbol.FileUri, filePath);
            if (fileUri is null)
            {
                error = DiagnosticBuilder.ForPosition(filePathArgument).FilePathCouldNotBeResolved(filePath, binder.FileSymbol.FileUri.LocalPath);
                return false;
            }

            if (!fileUri.IsFile)
            {
                error = DiagnosticBuilder.ForPosition(filePathArgument).UnableToLoadNonFileUri(fileUri);
                return false;
            }
            error = null;
            return true;
        }

        private static FunctionOverload.ResultBuilderDelegate PerformArmConversionOfStringLiterals(string armFunctionName) =>
            (_, _, diagnostics, functionCall, argumentTypes) =>
            {
                var arguments = functionCall.Arguments.ToImmutableArray();
                if (arguments.Length > 0 && argumentTypes.All(s => s is StringLiteralType))
                {
                    var parameters = argumentTypes.OfType<StringLiteralType>()
                        .Select(slt => new FunctionArgument(JValue.CreateString(slt.RawStringValue))).ToArray();
                    try
                    {
                        if (ExpressionBuiltInFunctions.Functions.EvaluateFunction(armFunctionName, parameters, new()) is JValue { Value: string stringValue })
                        {
                            return new(new StringLiteralType(stringValue));
                        }
                    }
                    catch (Exception e)
                    {
                        // The ARM function invoked will almost certainly fail at runtime, but there's a chance a fix has been
                        // deployed to ARM since this version of Bicep was released. Given that context, this failure will only
                        // be reported as a warning, and the fallback type will be used.
                        diagnostics.Write(
                            DiagnosticBuilder.ForPosition(TextSpan.Between(arguments.First().Span, arguments.Last().Span))
                                .ArmFunctionLiteralTypeConversionFailedWithMessage(
                                    string.Join(", ", parameters.Select(t => t.ToString())),
                                    armFunctionName,
                                    e.Message));
                    }
                }

                return new(LanguageConstants.String);
            };

        private static TypeSymbol? CalculateLambdaFromArrayParam(GetFunctionArgumentType getArgumentType, int arrayIndex, Func<TypeSymbol, LambdaType> lambdaBuilder)
        {
            if (getArgumentType(arrayIndex) is ArrayType arrayType)
            {
                var itemType = arrayType.Item;

                return lambdaBuilder(itemType.Type);
            }

            return null;
        }

        private static LambdaType OneParamLambda(TypeSymbol paramType, TypeSymbol returnType)
            => new LambdaType(ImmutableArray.Create<ITypeReference>(paramType), returnType);

        private static LambdaType TwoParamLambda(TypeSymbol param1Type, TypeSymbol param2Type, TypeSymbol returnType)
            => new LambdaType(ImmutableArray.Create<ITypeReference>(param1Type, param2Type), returnType);

        private static FunctionResult LoadTextContentResultBuilder(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            return TryLoadTextContentFromFile(binder, fileResolver, diagnostics,
                (arguments[0], argumentTypes[0]),
                arguments.Length > 1 ? (arguments[1], argumentTypes[1]) : null,
                out var fileContent,
                out var errorDiagnostic,
                LanguageConstants.MaxLiteralCharacterLimit)
                ? new(new StringLiteralType(fileContent))
                : new(ErrorType.Create(errorDiagnostic));
        }

        private static FunctionResult LoadJsonContentResultBuilder(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            string? tokenSelectorPath = null;
            if (arguments.Length > 1)
            {
                if (argumentTypes[1] is not StringLiteralType tokenSelectorType)
                {
                    return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[1]).CompileTimeConstantRequired()));
                }
                tokenSelectorPath = tokenSelectorType.RawStringValue;
            }
            if (!TryLoadTextContentFromFile(binder, fileResolver, diagnostics,
                    (arguments[0], argumentTypes[0]),
                    arguments.Length > 2 ? (arguments[2], argumentTypes[2]) : null,
                    out var fileContent,
                    out var errorDiagnostic,
                    LanguageConstants.MaxJsonFileCharacterLimit))
            {
                return new(ErrorType.Create(errorDiagnostic));
            }

            if (fileContent.TryFromJson<JToken>() is not { } token)
            {
                // Instead of catching and returning the JSON parse exception, we simply return a generic error.
                // This avoids having to deal with localization, and avoids possible confusion regarding line endings in the message.
                return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[0]).UnparseableJsonType()));
            }

            if (tokenSelectorPath is not null)
            {
                try
                {
                    var selectTokens = token.SelectTokens(tokenSelectorPath, false).ToList();
                    switch (selectTokens.Count)
                    {
                        case 0:
                            return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[1]).NoJsonTokenOnPathOrPathInvalid()));
                        case 1:
                            token = selectTokens.First();
                            break;
                        default:
                            token = new JArray();
                            foreach (var selectToken in selectTokens)
                            {
                                ((JArray)token).Add(selectToken);
                            }
                            break;
                    }
                }
                catch (JsonException)
                {
                    //path is invalid or user hasn't finished typing it yet
                    return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[1]).NoJsonTokenOnPathOrPathInvalid()));
                }
            }
            return new(ConvertJsonToBicepType(token), token);
        }

        private static bool TryLoadTextContentFromFile(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, (FunctionArgumentSyntax syntax, TypeSymbol typeSymbol) filePathArgument, (FunctionArgumentSyntax syntax, TypeSymbol typeSymbol)? encodingArgument, [NotNullWhen(true)] out string? fileContent, [NotNullWhen(false)] out ErrorDiagnostic? errorDiagnostic, int maxCharacters = -1)
        {
            fileContent = null;
            errorDiagnostic = null;

            if (filePathArgument.typeSymbol is not StringLiteralType filePathType)
            {
                errorDiagnostic = DiagnosticBuilder.ForPosition(filePathArgument.syntax).CompileTimeConstantRequired();
                return false;
            }
            var filePathValue = filePathType.RawStringValue;


            if (!TryGetFileUriWithDiagnostics(binder, fileResolver, filePathValue, filePathArgument.syntax, out var fileUri, out errorDiagnostic))
            {
                return false;
            }

            var fileEncoding = Encoding.UTF8;
            if (encodingArgument is not null)
            {
                if (encodingArgument.Value.typeSymbol is not StringLiteralType encodingType)
                {
                    errorDiagnostic = DiagnosticBuilder.ForPosition(encodingArgument.Value.syntax).CompileTimeConstantRequired();
                    return false;
                }
                fileEncoding = LanguageConstants.SupportedEncodings[encodingType.RawStringValue];
            }

            if (!fileResolver.TryRead(fileUri, out fileContent, out var fileReadFailureBuilder, fileEncoding, maxCharacters, out var detectedEncoding))
            {
                errorDiagnostic = fileReadFailureBuilder.Invoke(DiagnosticBuilder.ForPosition(filePathArgument.syntax));
                return false;
            }

            if (encodingArgument is not null && !Equals(fileEncoding, detectedEncoding))
            {
                diagnostics.Write(DiagnosticBuilder.ForPosition(encodingArgument.Value.syntax).FileEncodingMismatch(detectedEncoding.WebName));
            }

            return true;
        }

        private static FunctionResult LoadContentAsBase64ResultBuilder(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            var arguments = functionCall.Arguments.ToImmutableArray();
            if (argumentTypes[0] is not StringLiteralType filePathType)
            {
                diagnostics.Write(DiagnosticBuilder.ForPosition(arguments[0]).CompileTimeConstantRequired());
                return new(LanguageConstants.String);
            }
            var filePathValue = filePathType.RawStringValue;

            if (!TryGetFileUriWithDiagnostics(binder, fileResolver, filePathValue, arguments[0], out var fileUri, out var errorDiagnostic))
            {
                return new(ErrorType.Create(errorDiagnostic));
            }
            if (!fileResolver.TryReadAsBase64(fileUri, out var fileContent, out var fileReadFailureBuilder, LanguageConstants.MaxLiteralCharacterLimit))
            {
                return new(ErrorType.Create(fileReadFailureBuilder.Invoke(DiagnosticBuilder.ForPosition(arguments[0]))));
            }

            return new(new StringLiteralType(binder.FileSymbol.FileUri.MakeRelativeUri(fileUri).ToString(), fileContent));
        }

        private static SyntaxBase StringLiteralFunctionReturnTypeEvaluator(FunctionCallSyntaxBase functionCall, Symbol symbol, TypeSymbol typeSymbol, FunctionVariable? functionVariable, object? functionValue)
        {
            if (functionVariable is not null)
            {
                return SyntaxFactory.CreateExplicitVariableAccess(functionVariable.Name);
            }

            return CreateStringLiteral(typeSymbol);
        }

        private static SyntaxBase? StringLiteralFunctionVariableGenerator(FunctionCallSyntaxBase functionCall, Symbol symbol, TypeSymbol typeSymbol, bool directVariableAssignment, object? functionValue)
        {
            if (directVariableAssignment)
            {
                return null;
            }
            return CreateStringLiteral(typeSymbol);
        }

        private static SyntaxBase CreateStringLiteral(TypeSymbol typeSymbol)
        {
            if (typeSymbol is not StringLiteralType stringLiteral)
            {
                throw new InvalidOperationException($"Expecting function to return {nameof(StringLiteralType)}, but {typeSymbol.GetType().Name} received.");
            }

            return SyntaxFactory.CreateStringLiteral(stringLiteral.RawStringValue);
        }

        private static SyntaxBase JsonContentFunctionReturnTypeEvaluator(FunctionCallSyntaxBase functionCall, Symbol symbol, TypeSymbol typeSymbol, FunctionVariable? functionVariable, object? functionValue)
        {
            if (functionVariable is null)
            {
                // TemplateEmitter when emitting ARM-json code instead function will use createObject functions instead putting raw JSON.
                // This can be avoided using functionVariables where bicep syntax is processed by Emitter itself, not by ExpressionConverter.
                throw new InvalidOperationException($"Function Variable must be used");
            }

            return SyntaxFactory.CreateExplicitVariableAccess(functionVariable.Name);

        }

        private static SyntaxBase JsonContentFunctionVariableGenerator(FunctionCallSyntaxBase functionCall, Symbol symbol, TypeSymbol typeSymbol, bool directVariableAssignment, object? functionValue)
        {
            //converting JSON to bicep syntax and then back to ARM-JSON escapes ARM template expressions (`[variables('')]`, etc.) out of the box
            return ConvertJsonToBicepSyntax(functionValue as JToken ?? throw new InvalidOperationException($"Expecting function to return {nameof(JToken)}, but {functionValue?.GetType().ToString() ?? "null"} received."));
        }

        private static SyntaxBase ConvertJsonToBicepSyntax(JToken token) =>
        token switch
        {
            JObject @object => SyntaxFactory.CreateObject(@object.Properties().Select(x => SyntaxFactory.CreateObjectProperty(x.Name, ConvertJsonToBicepSyntax(x.Value)))),
            JArray @array => SyntaxFactory.CreateArray(@array.Select(ConvertJsonToBicepSyntax)),
            JValue value => value.Type switch
            {
                JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString(CultureInfo.InvariantCulture)),
                JTokenType.Integer => value.ToObject<long>() < 0 ? SyntaxFactory.CreateNegativeIntegerLiteral((ulong)(0 - value.ToObject<long>())) : SyntaxFactory.CreateIntegerLiteral(value.ToObject<ulong>()),
                // Floats are currently not supported in Bicep, so fall back to the default behavior of "any"
                JTokenType.Float => SyntaxFactory.CreateFunctionCall("json", SyntaxFactory.CreateStringLiteral(value.ToObject<double>().ToString(CultureInfo.InvariantCulture))),
                JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.ToObject<bool>()),
                JTokenType.Null => SyntaxFactory.CreateFunctionCall("null"),
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
                        new TypeProperty("key", keyType, description: "The key of the object property being iterated over."),
                        new TypeProperty("value", valueType, description: "The value of the object property being iterated over."),
                    },
                    null),
                TypeSymbolValidationFlags.Default);

        private static FunctionResult ItemsResultBuilder(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
        {
            if (argumentTypes[0] is not ObjectType objectType)
            {
                return new(GetItemsReturnType(LanguageConstants.String, LanguageConstants.Any));
            }

            var keyTypes = new List<TypeSymbol>();
            var valueTypes = new List<TypeSymbol>();
            foreach (var property in objectType.Properties.Values)
            {
                if (property.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                {
                    // we're reading the values - exclude write-only properties
                    continue;
                }

                keyTypes.Add(new StringLiteralType(property.Name));
                valueTypes.Add(property.TypeReference.Type);
            }

            if (objectType.AdditionalPropertiesType?.Type is { } additionalPropertiesType)
            {
                keyTypes.Add(LanguageConstants.String);
                valueTypes.Add(additionalPropertiesType);
            }

            return new(GetItemsReturnType(
                keyType: TypeHelper.CreateTypeUnion(keyTypes),
                valueType: TypeHelper.TryCollapseTypes(valueTypes) ?? LanguageConstants.Any));
        }

        private static TypeSymbol ConvertJsonToBicepType(JToken token)
            => token switch
            {
                JObject @object => new ObjectType(
                    "object",
                    TypeSymbolValidationFlags.Default,
                    @object.Properties().Select(x => new TypeProperty(x.Name, ConvertJsonToBicepType(x.Value), TypePropertyFlags.ReadOnly | TypePropertyFlags.ReadableAtDeployTime)),
                    null),
                JArray @array => new TypedArrayType(
                    TypeHelper.CreateTypeUnion(@array.Select(ConvertJsonToBicepType)),
                    TypeSymbolValidationFlags.Default),
                JValue value => value.Type switch
                {
                    JTokenType.String => new StringLiteralType(value.ToString(CultureInfo.InvariantCulture)),
                    JTokenType.Integer => LanguageConstants.Int,
                    // Floats are currently not supported in Bicep, so fall back to the default behavior of "any"
                    JTokenType.Float => LanguageConstants.Any,
                    JTokenType.Boolean => LanguageConstants.Bool,
                    JTokenType.Null => LanguageConstants.Null,
                    _ => LanguageConstants.Any,
                },
                _ => LanguageConstants.Any,
            };

        private static FunctionResult JsonResultBuilder(IBinder binder, IFileResolver fileResolver, IDiagnosticWriter diagnostics, FunctionCallSyntaxBase functionCall, ImmutableArray<TypeSymbol> argumentTypes)
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
                var error = DiagnosticBuilder.ForPosition(arguments[0].Expression).UnparseableJsonType();

                return new(ErrorType.Create(error));
            }

            return new(ConvertJsonToBicepType(token));
        }

        // TODO: Add copyIndex here when we support loops.
        private static readonly ImmutableArray<BannedFunction> BannedFunctions = new[]
        {
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
        }.ToImmutableArray();

        private static IEnumerable<Decorator> GetSystemDecorators()
        {
            static DecoratorEvaluator MergeToTargetObject(string propertyName, Func<DecoratorSyntax, SyntaxBase> propertyValueSelector) =>
                (decoratorSyntax, _, targetObject) =>
                    targetObject.MergeProperty(propertyName, propertyValueSelector(decoratorSyntax));

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

            static void ValidateLength(string decoratorName, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IBinder binder, IDiagnosticWriter diagnosticWriter)
            {
                var lengthSyntax = SingleArgumentSelector(decoratorSyntax);

                if (TryGetIntegerLiteralValue(lengthSyntax) is not null and < 0)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(lengthSyntax).LengthMustNotBeNegative());
                }
            }

            yield return new DecoratorBuilder(LanguageConstants.ParameterSecurePropertyName)
                .WithDescription("Makes the parameter a secure parameter.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Object))
                .WithEvaluator((_, targetType, targetObject) =>
                {
                    if (ReferenceEquals(targetType, LanguageConstants.String))
                    {
                        return targetObject.MergeProperty("type", "secureString");
                    }

                    if (ReferenceEquals(targetType, LanguageConstants.Object))
                    {
                        return targetObject.MergeProperty("type", "secureObject");
                    }

                    return targetObject;
                })
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.ParameterAllowedPropertyName)
                .WithDescription("Defines the allowed values of the parameter.")
                .WithRequiredParameter("values", LanguageConstants.Array, "The allowed values.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator((_, decoratorSyntax, targetType, typeManager, binder, diagnosticWriter) =>
                {
                    if (ReferenceEquals(targetType, LanguageConstants.Array) &&
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
                        diagnosticWriter,
                        SingleArgumentSelector(decoratorSyntax),
                        new TypedArrayType(targetType, TypeSymbolValidationFlags.Default));
                })
                .WithEvaluator(MergeToTargetObject("allowedValues", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.ParameterMinValuePropertyName)
                .WithDescription("Defines the minimum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The minimum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(LanguageConstants.Int)
                .WithEvaluator(MergeToTargetObject(LanguageConstants.ParameterMinValuePropertyName, SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.ParameterMaxValuePropertyName)
                .WithDescription("Defines the maximum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The maximum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(LanguageConstants.Int)
                .WithEvaluator(MergeToTargetObject(LanguageConstants.ParameterMaxValuePropertyName, SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.ParameterMinLengthPropertyName)
                .WithDescription("Defines the minimum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The minimum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Array))
                .WithValidator(ValidateLength)
                .WithEvaluator(MergeToTargetObject(LanguageConstants.ParameterMinLengthPropertyName, SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.ParameterMaxLengthPropertyName)
                .WithDescription("Defines the maximum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The maximum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Array))
                .WithValidator(ValidateLength)
                .WithEvaluator(MergeToTargetObject(LanguageConstants.ParameterMaxLengthPropertyName, SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.ParameterMetadataPropertyName)
                .WithDescription("Defines metadata of the parameter.")
                .WithRequiredParameter("object", LanguageConstants.Object, "The metadata object.")
                .WithFlags(FunctionFlags.ParamterOrOutputDecorator)
                .WithValidator((_, decoratorSyntax, _, typeManager, binder, diagnosticWriter) =>
                    TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, binder, diagnosticWriter, SingleArgumentSelector(decoratorSyntax), LanguageConstants.ParameterModifierMetadata))
                .WithEvaluator(MergeToTargetObject(LanguageConstants.ParameterMetadataPropertyName, SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.MetadataDescriptionPropertyName)
                .WithDescription("Describes the parameter.")
                .WithRequiredParameter("text", LanguageConstants.String, "The description.")
                .WithFlags(FunctionFlags.AnyDecorator)
                .WithEvaluator(MergeToTargetObject("metadata", decoratorSyntax => SyntaxFactory.CreateObject(
                    SyntaxFactory.CreateObjectProperty("description", SingleArgumentSelector(decoratorSyntax)).AsEnumerable())))
                .Build();

            yield return new DecoratorBuilder(LanguageConstants.BatchSizePropertyName)
                .WithDescription("Causes the resource or module for-expression to be run in sequential batches of specified size instead of the default behavior where all the resources or modules are deployed in parallel.")
                .WithRequiredParameter(LanguageConstants.BatchSizePropertyName, LanguageConstants.Int, "The size of the batch")
                .WithFlags(FunctionFlags.ResourceOrModuleDecorator)
                // the decorator is constrained to resources and modules already - checking for array alone is simple and should be sufficient
                .WithValidator((decoratorName, decoratorSyntax, targetType, typeManager, binder, diagnosticWriter) =>
                {
                    if (!TypeValidator.AreTypesAssignable(targetType, LanguageConstants.Array))
                    {
                        // the resource/module declaration is not a collection
                        // (the compile-time constnat and resource/module placement is already enforced, so we don't need a deeper type check)
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
                .WithEvaluator(MergeToTargetObject(LanguageConstants.BatchSizePropertyName, SingleArgumentSelector))
                .Build();
        }

        public static NamespaceType Create(string aliasName, IFeatureProvider featureProvider)
        {
            return new NamespaceType(
                aliasName,
                Settings,
                ImmutableArray<TypeProperty>.Empty,
                GetSystemOverloads(featureProvider),
                BannedFunctions,
                GetSystemDecorators(),
                new EmptyResourceTypeProvider());
        }
    }
}
