using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core;

namespace Bicep.Core.Errors
{
    public static class ErrorFormatter
    {
        private static ImmutableDictionary<ErrorCode, string> FormatStringLookup { get; } = new Dictionary<ErrorCode, string>
        {
            [ErrorCode.ErrUnrecognizedToken] = "The following token is not recognized: {0}",
            [ErrorCode.ErrUnterminatedMultilineComment] = "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.",
            [ErrorCode.ErrUnterminatedString] = "The string at this location is not terminated. Terminate the string with a single quote character.",
            [ErrorCode.ErrUnterminatedStringWithNewLine] = "The string at this location is not terminated due to an unexpected new line character.",
            [ErrorCode.ErrUnterminatedStringEscapeSequenceAtEof] = "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.",
            [ErrorCode.ErrUnterminatedStringEscapeSequenceUnrecognized] = "The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: {0}",
            [ErrorCode.ErrUnrecognizedDeclaration] = "This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.",
            [ErrorCode.ErrExpectedParameterContinuation] = "Expected the default keyword, a parameter modifier, or a newline at this location.",
            [ErrorCode.ErrUnrecognizedExpression] = "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.",
            [ErrorCode.ErrInvalidInteger] = "Expected a valid 32-bit signed integer.",
            [ErrorCode.ErrInvalidType] = "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.",
            [ErrorCode.ErrExpectedKeyword] = "Expected the '{0}' keyword at this location.",
            [ErrorCode.ErrExpectedParameterIdentifier] = "Expected a parameter identifier at this location.",
            [ErrorCode.ErrExpectedParameterType] = $"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}",
            [ErrorCode.ErrExpectedVariableIdentifier] = "Expected a variable identifier at this location.",
            [ErrorCode.ErrExpectedOutputIdentifier] = "Expected an output identifier at this location.",
            [ErrorCode.ErrExpectedResourceIdentifier] = "Expected a resource identifier at this location.",
            [ErrorCode.ErrExpectedCharacter] = "Expected the '{0}' character at this location.",
            [ErrorCode.ErrExpectedNewLine] = "Expected a new line character at this location.",
            [ErrorCode.ErrExpectedPropertyIdentifier] = "Expected a property identifier at this location.",
            [ErrorCode.ErrExpectedNumericLiteral] = "Expected a numeric literal at this location.",
            [ErrorCode.ErrExpectedPropertyName] = "Expected a property name at this location.",
            [ErrorCode.ErrExpectedFunctionName] = "Expected a function name at this location.",
            [ErrorCode.ErrIdentifierNameExceedsLimit] = $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.",
            [ErrorCode.ErrPropertyMultipleDeclarations] = "The property '{0}' is declared multiple times in this object. Remove or rename the duplicate properties.",
            [ErrorCode.ErrOutputTypeMismatch] = "The output expects a value of type '{0} but the provided value is of type '{1}'.",
            [ErrorCode.ErrParameterTypeMismatch] = "The parameter expects a default value of type '{0}' but provided value is of type '{1}'.",
            [ErrorCode.ErrIdentifierMultipleDeclarations] = "Identifier '{0}' is declared multiple times. Remove or rename the duplicates.",
            [ErrorCode.ErrInvalidResourceType] = "The resource type is not valid. Specify a valid resource type.",
            [ErrorCode.ErrInvalidOutputType] = $"The output type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}",
            [ErrorCode.ErrInvalidParameterType] = $"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}",
            [ErrorCode.ErrCompileTimeConstantRequired] = "The value must be a compile-time constant.",
            [ErrorCode.ErrExpectdValueTypeMismatch] = "Expected a value of type '{0}' but the provided value is of type '{1}'.",
            [ErrorCode.ErrArrayTypeMismatch] = "The enclosing array expected an item of type '{0}', but the provided item was of type '{1}'.",
            [ErrorCode.ErrMissingRequiredProperties] = "The specified object is missing the following required properties: {0}.",
            [ErrorCode.ErrPropertyTypeMismatch] = "The property '{0}' expected a value of type '{1}' but the provided value is of type '{2}'.",
            [ErrorCode.ErrDisallowedProperty] = "The property '{0}' is not allowed on objects of type '{1}'.",
            [ErrorCode.ErrNotImplementedFunctionArgs] = "Function arguments are not currently supported.",
            [ErrorCode.ErrNotImplementedFunctionCalls] = "Function calls are not currently supported.",
            [ErrorCode.ErrNotImplementedPropertyAccess] = "Property access is not currently supported.",
            [ErrorCode.ErrNotImplementedArrayAccess] = "Array access is not currently supported.",
            [ErrorCode.ErrNotImplementedVariableAccess] = "Variable access is not currently supported.",
            [ErrorCode.ErrInvalidExpression] = "This is not a valid expression.",
            [ErrorCode.ErrUnaryOperatorInvalidType] = "Cannot apply operator '{0}' to operand of type '{1}'.",
            [ErrorCode.ErrBinaryOperatorInvalidType] = "Cannot apply operator '{0}' to operands of type '{1}' and '{2}'.",
            [ErrorCode.ErrValueTypeMismatch] = "Expected a value of type '{0}'.",
        }.ToImmutableDictionary();

        public static string Format(ErrorCode code, object[] arguments)
            => string.Format(FormatStringLookup[code], arguments);
    }
}