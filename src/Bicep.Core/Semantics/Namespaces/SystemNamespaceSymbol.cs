// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics.Namespaces
{
    public class SystemNamespaceSymbol : NamespaceSymbol
    {
        private static readonly ImmutableArray<FunctionOverload> SystemOverloads = new[]
        {
            new FunctionOverloadBuilder("any")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Converts the specified value to the `any` type.")
                .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to `any` type")
                .Build(),

            new FunctionOverloadBuilder("concat")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Combines multiple arrays and returns the concatenated array.")
                .WithVariableParameter("arg", LanguageConstants.Array, minimumCount: 1, "The array for concatenation")
                .Build(),

            new FunctionOverloadBuilder("concat")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Combines multiple string, integer, or boolean values and returns them as a concatenated string.")
                .WithVariableParameter("arg", UnionType.Create(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.Bool), minimumCount: 1, "The string, int, or boolean value for concatenation")
                .Build(),

            new FunctionOverloadBuilder("format")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Creates a formatted string from input values.")
                .WithRequiredParameter("formatString", LanguageConstants.String, "The composite format string.")
                .WithVariableParameter("arg", LanguageConstants.Any, minimumCount: 0, "The value to include in the formatted string.")
                .Build(),

            new FunctionOverloadBuilder("base64")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns the base64 representation of the input string.")
                .WithRequiredParameter("inputString", LanguageConstants.String, "The value to return as a base64 representation.")
                .Build(),

            new FunctionOverloadBuilder("padLeft")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a right-aligned string by adding characters to the left until reaching the total specified length.")
                .WithRequiredParameter("valueToPad", UnionType.Create(LanguageConstants.String, LanguageConstants.Int), "The value to right-align.")
                .WithRequiredParameter("totalLength", LanguageConstants.Int, "The total number of characters in the returned string.")
                .WithOptionalParameter("paddingCharacter", LanguageConstants.String, "The character to use for left-padding until the total length is reached. The default value is a space.")
                .Build(),

            new FunctionOverloadBuilder("replace")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a new string with all instances of one string replaced by another string.")
                .WithRequiredParameter("originalString", LanguageConstants.String, "The original string.")
                .WithRequiredParameter("oldString", LanguageConstants.String, "The string to be removed from the original string.")
                .WithRequiredParameter("newString", LanguageConstants.String, "The string to add in place of the removed string.")
                .Build(),

            new FunctionOverloadBuilder("toLower")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Converts the specified string to lower case.")
                .WithRequiredParameter("stringToChange", LanguageConstants.String, "The value to convert to lower case.")
                .Build(),

            new FunctionOverloadBuilder("toUpper")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Converts the specified string to upper case.")
                .WithRequiredParameter("stringToChange", LanguageConstants.String, "The value to convert to upper case.")
                .Build(),

            new FunctionOverloadBuilder("length")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the number of characters in a string, elements in an array, or root-level properties in an object.")
                .WithRequiredParameter("arg", UnionType.Create(LanguageConstants.String, LanguageConstants.Object, LanguageConstants.Array), "The array to use for getting the number of elements, the string to use for getting the number of characters, or the object to use for getting the number of root-level properties.")
                .Build(),

            new FunctionOverloadBuilder("split")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Returns an array of strings that contains the substrings of the input string that are delimited by the specified delimiters.")
                .WithRequiredParameter("inputString", LanguageConstants.String, "The string to split.")
                .WithRequiredParameter("delimiter", UnionType.Create(LanguageConstants.String, LanguageConstants.Array), "The delimiter to use for splitting the string.")
                .Build(),

            new FunctionOverloadBuilder("string")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Converts the specified value to a string.")
                .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to string. Any type of value can be converted, including objects and arrays.")
                .Build(),

            new FunctionOverloadBuilder("int")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Converts the specified value to an integer.")
                .WithRequiredParameter("valueToConvert", UnionType.Create(LanguageConstants.String, LanguageConstants.Int), "The value to convert to an integer.")
                .Build(),

            new FunctionOverloadBuilder("uniqueString")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Creates a deterministic hash string based on the values provided as parameters.")
                .WithVariableParameter("arg", LanguageConstants.String, minimumCount: 1, "The value used in the hash function to create a unique string.")
                .Build(),

            new FunctionOverloadBuilder("guid")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Creates a value in the format of a globally unique identifier based on the values provided as parameters.")
                .WithVariableParameter("arg", LanguageConstants.String, minimumCount: 1, "The value used in the hash function to create the GUID.")
                .Build(),

            new FunctionOverloadBuilder("trim")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Removes all leading and trailing white-space characters from the specified string.")
                .WithRequiredParameter("stringToTrim", LanguageConstants.String, "The value to trim.")
                .Build(),

            new FunctionOverloadBuilder("uri")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Creates an absolute URI by combining the baseUri and the relativeUri string.")
                .WithRequiredParameter("baseUri", LanguageConstants.String, "The base uri string.")
                .WithRequiredParameter("relativeUri", LanguageConstants.String, "The relative uri string to add to the base uri string.")
                .Build(),

            // TODO: Docs deviation
            new FunctionOverloadBuilder("substring")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a substring that starts at the specified character position and contains the specified number of characters.")
                .WithRequiredParameter("stringToParse", LanguageConstants.String, "The original string from which the substring is extracted.")
                .WithRequiredParameter("startIndex", LanguageConstants.Int, "The zero-based starting character position for the substring.")
                .WithOptionalParameter("length", LanguageConstants.Int, "The number of characters for the substring. Must refer to a location within the string. Must be zero or greater.")
                .Build(),

            new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Returns an array with the specified number of elements from the start of the array.")
                .WithRequiredParameter("originalValue", LanguageConstants.Array, "The array to take the elements from.")
                .WithRequiredParameter("numberToTake", LanguageConstants.Int, "The number of elements to take. If this value is 0 or less, an empty array is returned. If it is larger than the length of the given array, all the elements in the array are returned.")
                .Build(),

            new FunctionOverloadBuilder("take")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a string with the specified number of characters from the start of the string.")
                .WithRequiredParameter("originalValue", LanguageConstants.String, "The string to take the elements from.")
                .WithRequiredParameter("numberToTake", LanguageConstants.Int, "The number of characters to take. If this value is 0 or less, an empty string is returned. If it is larger than the length of the given string, all the characters are returned.")
                .Build(),

            new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Returns an array with all the elements after the specified number in the array.")
                .WithRequiredParameter("originalValue", LanguageConstants.Array, "The array to use for skipping.")
                .WithRequiredParameter("numberToSkip", LanguageConstants.Int, "The number of elements to skip. If this value is 0 or less, all the elements in the value are returned. If it is larger than the length of the array, an empty array is returned.")
                .Build(),

            new FunctionOverloadBuilder("skip")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a string with all the characters after the specified number in the string.")
                .WithRequiredParameter("originalValue", LanguageConstants.String, "The string to use for skipping.")
                .WithRequiredParameter("numberToSkip", LanguageConstants.Int, "The number of characters to skip. If this value is 0 or less, all the characters in the value are returned. If it is larger than the length of the string, an empty string is returned.")
                .Build(),

            new FunctionOverloadBuilder("empty")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Determines if an array, object, or string is empty.")
                .WithRequiredParameter("itemToTest", UnionType.Create(LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.Array, LanguageConstants.String), "The value to check if it is empty.")
                .Build(),

            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Checks whether an object contains a property. The property name comparison is case-insensitive.")
                .WithRequiredParameter("object", LanguageConstants.Object, "The object")
                .WithRequiredParameter("propertyName", LanguageConstants.String, "The property name.")
                .Build(),

            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Checks whether an array contains a value.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The array")
                .WithRequiredParameter("itemToFind", LanguageConstants.Any, "The value to find.")
                .Build(),

            new FunctionOverloadBuilder("contains")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Checks whether a string contains a substring. The string comparison is case-sensitive.")
                .WithRequiredParameter("string", LanguageConstants.String, "The string.")
                .WithRequiredParameter("itemToFind", LanguageConstants.String, "The value to find.")
                .Build(),

            new FunctionOverloadBuilder("intersection")
                .WithReturnType(LanguageConstants.Object)
                .WithDescription("Returns a single object with the common elements from the parameters.")
                .WithVariableParameter("object", LanguageConstants.Object, minimumCount: 2, "The object to use for finding common elements.")
                .Build(),

            new FunctionOverloadBuilder("intersection")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Returns a single array with the common elements from the parameters.")
                .WithVariableParameter("array", LanguageConstants.Array, minimumCount: 2, "The array to use for finding common elements.")
                .Build(),

            new FunctionOverloadBuilder("union")
                .WithReturnType(LanguageConstants.Object)
                .WithDescription("Returns a single object with all elements from the parameters. Duplicate keys are only included once.")
                .WithVariableParameter("object", LanguageConstants.Object, minimumCount: 2, "The first object to use for joining elements.")
                .Build(),

            new FunctionOverloadBuilder("union")
                .WithReturnType(LanguageConstants.Array)
                .WithDescription("Returns a single array with all elements from the parameters. Duplicate values are only included once.")
                .WithVariableParameter("object", LanguageConstants.Array, minimumCount: 2, "The first array to use for joining elements.")
                .Build(),

            new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Returns the first element of the array.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The value to retrieve the first element.")
                .Build(),

            new FunctionOverloadBuilder("first")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns the first character of the string.")
                .WithRequiredParameter("string", LanguageConstants.String, "The value to retrieve the first character.")
                .Build(),

            new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Returns the last element of the array.")
                .WithRequiredParameter("array", LanguageConstants.Array, "The value to retrieve the last element.")
                .Build(),

            new FunctionOverloadBuilder("last")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns the last character of the string.")
                .WithRequiredParameter("string", LanguageConstants.String, "The value to retrieve the last character.")
                .Build(),

            new FunctionOverloadBuilder("indexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the first position of a value within a string. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build(),

            new FunctionOverloadBuilder("lastIndexOf")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the last position of a value within a string. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build(),

            new FunctionOverloadBuilder("startsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Determines whether a string starts with a value. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build(),

            new FunctionOverloadBuilder("endsWith")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Determines whether a string ends with a value. The comparison is case-insensitive.")
                .WithRequiredParameter("stringToSearch", LanguageConstants.String, "The value that contains the item to find.")
                .WithRequiredParameter("stringToFind", LanguageConstants.String, "The value to find.")
                .Build(),

            // TODO: Needs to support number type as well
            // TODO: Docs need updates
            new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the minimum value from the specified integers.")
                .WithVariableParameter("int", LanguageConstants.Int, minimumCount: 1, "One of the integers used to calculate the minimum value")
                .Build(),

            // TODO: Docs need updates
            new FunctionOverloadBuilder("min")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the minimum value from an array of integers.")
                .WithRequiredParameter("intArray", LanguageConstants.Array, "The array of integers.")
                .Build(),

            // TODO: Needs to support number type as well
            // TODO: Docs need updates
            new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the maximum value from the specified integers.")
                .WithVariableParameter("int", LanguageConstants.Int, minimumCount: 1, "One of the integers used to calculate the maximum value")
                .Build(),

            // TODO: Docs need updates
            new FunctionOverloadBuilder("max")
                .WithReturnType(LanguageConstants.Int)
                .WithDescription("Returns the maximum value from an array of integers.")
                .WithRequiredParameter("intArray", LanguageConstants.Array, "The array of integers.")
                .Build(),

            new FunctionOverloadBuilder("range")
                .WithReturnType(new TypedArrayType(LanguageConstants.Int, TypeSymbolValidationFlags.Default))
                .WithDescription("Creates an array of integers from a starting integer and containing a number of items.")
                .WithRequiredParameter("startIndex", LanguageConstants.Int, "The first integer in the array. The sum of startIndex and count must be no greater than 2147483647.")
                .WithRequiredParameter("count", LanguageConstants.Int, "The number of integers in the array. Must be non-negative integer up to 10000.")
                .Build(),

            new FunctionOverloadBuilder("base64ToString")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Converts a base64 representation to a string.")
                .WithRequiredParameter("base64Value", LanguageConstants.String, "The base64 representation to convert to a string.")
                .Build(),

            new FunctionOverloadBuilder("base64ToJson")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Converts a base64 representation to a JSON object.")
                .WithRequiredParameter("base64Value", LanguageConstants.String, "The base64 representation to convert to a JSON object.")
                .Build(),

            new FunctionOverloadBuilder("uriComponentToString")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a string of a URI encoded value.")
                .WithRequiredParameter("uriEncodedString", LanguageConstants.String, "The URI encoded value to convert to a string.")
                .Build(),

            new FunctionOverloadBuilder("uriComponent")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Encodes a URI.")
                .WithRequiredParameter("stringToEncode", LanguageConstants.String, "The value to encode.")
                .Build(),

            new FunctionOverloadBuilder("dataUriToString")
                .WithDescription("Converts a data URI formatted value to a string.")
                .WithReturnType(LanguageConstants.String)
                .WithRequiredParameter("dataUriToConvert", LanguageConstants.String, "The data URI value to convert.")
                .Build(),

            // TODO: Docs have wrong param type and param name (any is actually supported)
            new FunctionOverloadBuilder("dataUri")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Converts a value to a data URI.")
                .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to a data URI.")
                .Build(),

            new FunctionOverloadBuilder("array")
                .WithDescription("Converts the value to an array.")
                .WithReturnType(LanguageConstants.Array)
                .WithRequiredParameter("valueToConvert", LanguageConstants.Any, "The value to convert to an array.")
                .Build(),

            new FunctionOverloadBuilder("coalesce")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Returns first non-null value from the parameters. Empty strings, empty arrays, and empty objects are not null.")
                .WithVariableParameter("arg", LanguageConstants.Any, minimumCount: 1, "The value to coalesce")
                .Build(),

            // TODO: Requires number type
            //new FunctionOverloadBuilder("float")
            //    .WithReturnType(LanguageConstants.Number)
            //    .WithDescription("Converts the value to a floating point number. You only use this function when passing custom parameters to an application, such as a Logic App.")
            //    .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to a floating point number.")
            //    .Build(),

            new FunctionOverloadBuilder("bool")
                .WithReturnType(LanguageConstants.Bool)
                .WithDescription("Converts the parameter to a boolean.")
                .WithRequiredParameter("value", LanguageConstants.Any, "The value to convert to a boolean.")
                .Build(),

            new FunctionOverloadBuilder("json")
                .WithReturnType(LanguageConstants.Any)
                .WithDescription("Converts a valid JSON string into a JSON data type.")
                .WithRequiredParameter("json", LanguageConstants.String, "The value to convert to JSON. The string must be a properly formatted JSON string.")
                .Build(),

            new FunctionOverloadBuilder("dateTimeAdd")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Adds a time duration to a base value. ISO 8601 format is expected.")
                .WithRequiredParameter("base", LanguageConstants.String, "The starting datetime value for the addition. [Use ISO 8601 timestamp format](https://en.wikipedia.org/wiki/ISO_8601).")
                .WithRequiredParameter("duration", LanguageConstants.String, "The time value to add to the base. It can be a negative value. Use [ISO 8601 duration format](https://en.wikipedia.org/wiki/ISO_8601#Durations).")
                .WithOptionalParameter("format", LanguageConstants.String, "The output format for the date time result. If not provided, the format of the base value is used. Use either [standard format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) or [custom format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).")
                .Build(),

            // newGuid and utcNow are only allowed in parameter default values
            new FunctionOverloadBuilder("utcNow")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns the current (UTC) datetime value in the specified format. If no format is provided, the ISO 8601 (yyyyMMddTHHmmssZ) format is used. **This function can only be used in the default value for a parameter**.")
                .WithOptionalParameter("format", LanguageConstants.String, "The format. Use either [standard format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) or [custom format strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).")
                .WithFlags(FunctionFlags.ParamDefaultsOnly)
                .Build(),

            new FunctionOverloadBuilder("newGuid")
                .WithReturnType(LanguageConstants.String)
                .WithDescription("Returns a value in the format of a globally unique identifier. **This function can only be used in the default value for a parameter**.")
                .WithFlags(FunctionFlags.ParamDefaultsOnly)
                .Build()
        }.ToImmutableArray();

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
                UnaryOperationSyntax { Operator: UnaryOperator.Minus } unaryOperatorSyntax
                    when unaryOperatorSyntax.Expression is IntegerLiteralSyntax integerLiteralSyntax => -1 * integerLiteralSyntax.Value,
                IntegerLiteralSyntax integerLiteralSyntax => integerLiteralSyntax.Value,
                _ => null,
            };

            static void ValidateLength(string decoratorName, DecoratorSyntax decoratorSyntax, TypeSymbol targetType, ITypeManager typeManager, IDiagnosticWriter diagnosticWriter)
            {
                var lengthSyntax = SingleArgumentSelector(decoratorSyntax);

                if (TryGetIntegerLiteralValue(lengthSyntax) is not null and < 0)
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(lengthSyntax).LengthMustNotBeNegative());
                }
            }

            yield return new DecoratorBuilder("secure")
                .WithDescription("Makes the parameter a secure parameter.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(UnionType.Create(LanguageConstants.String, LanguageConstants.Object))
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

            yield return new DecoratorBuilder("allowed")
                .WithDescription("Defines the allowed values of the parameter.")
                .WithRequiredParameter("values", LanguageConstants.Array, "The allowed values.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator((_, decoratorSyntax, targetType, typeManager, diagnosticWriter) =>
                     TypeValidator.NarrowTypeAndCollectDiagnostics(
                            typeManager,
                        SingleArgumentSelector(decoratorSyntax),
                        new TypedArrayType(targetType, TypeSymbolValidationFlags.Default),
                        diagnosticWriter))
                .WithEvaluator(MergeToTargetObject("allowedValues", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("minValue")
                .WithDescription("Defines the minimum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The minimum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(LanguageConstants.Int)
                .WithEvaluator(MergeToTargetObject("minValue", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("maxValue")
                .WithDescription("Defines the maximum value of the parameter.")
                .WithRequiredParameter("value", LanguageConstants.Int, "The maximum value.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(LanguageConstants.Int)
                .WithEvaluator(MergeToTargetObject("maxValue", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("minLength")
                .WithDescription("Defines the minimum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The minimum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(UnionType.Create(LanguageConstants.String, LanguageConstants.Array))
                .WithValidator(ValidateLength)
                .WithEvaluator(MergeToTargetObject("minLength", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("maxLength")
                .WithDescription("Defines the maximum length of the parameter.")
                .WithRequiredParameter("length", LanguageConstants.Int, "The maximum length.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithAttachableType(UnionType.Create(LanguageConstants.String, LanguageConstants.Array))
                .WithValidator(ValidateLength)
                .WithEvaluator(MergeToTargetObject("maxLength", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("metadata")
                .WithDescription("Defines metadata of the parameter.")
                .WithRequiredParameter("object", LanguageConstants.Object, "The metadata object.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithValidator((_, decoratorSyntax, _, typeManager, diagnosticWriter) => 
                    TypeValidator.NarrowTypeAndCollectDiagnostics(typeManager, SingleArgumentSelector(decoratorSyntax), LanguageConstants.ParameterModifierMetadata, diagnosticWriter))
                .WithEvaluator(MergeToTargetObject("metadata", SingleArgumentSelector))
                .Build();

            yield return new DecoratorBuilder("description")
                .WithDescription("Describes the parameter.")
                .WithRequiredParameter("text", LanguageConstants.String, "The description.")
                .WithFlags(FunctionFlags.ParameterDecorator)
                .WithEvaluator(MergeToTargetObject("metadata", decoratorSyntax => SyntaxFactory.CreateObject(
                    SyntaxFactory.CreateObjectProperty("description", SingleArgumentSelector(decoratorSyntax)).AsEnumerable())))
                .Build();

            yield return new DecoratorBuilder("batchSize")
                .WithDescription("Causes the resource or module for-expression to be run in sequential batches of specified size instead of the default behavior where all the resources or modules are deployed in parallel.")
                .WithRequiredParameter("batchSize", LanguageConstants.Int, "The size of the batch")
                .WithFlags(FunctionFlags.ResourceOrModuleDecorator)
                // the decorator is constrained to resources and modules already - checking for array alone is simple and should be sufficient
                .WithValidator((decoratorName, decoratorSyntax, targetType, typeManager, diagnosticWriter) =>
                {
                    if (!TypeValidator.AreTypesAssignable(targetType, LanguageConstants.Array))
                    {
                        // the resource/module declaration is not a collection
                        // (the compile-time constnat and resource/module placement is already enforced, so we don't need a deeper type check)
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(decoratorSyntax).BatchSizeNotAllowed(decoratorName));
                    }

                    const long MinimumBatchSize = 1;
                    SyntaxBase batchSizeSyntax = SingleArgumentSelector(decoratorSyntax);
                    long? batchSize = TryGetIntegerLiteralValue(batchSizeSyntax);

                    if (batchSize is not null and < MinimumBatchSize)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(batchSizeSyntax).BatchSizeTooSmall(batchSize.Value, MinimumBatchSize));
                    }
                })
                .WithEvaluator(MergeToTargetObject("batchSize", SingleArgumentSelector))
                .Build();
        }

        public SystemNamespaceSymbol() : base("sys", SystemOverloads, BannedFunctions, GetSystemDecorators())
        {
        }
    }
}
