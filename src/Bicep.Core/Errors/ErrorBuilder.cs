using System;
using Bicep.Core.Parser;

namespace Bicep.Core.Errors
{
    public static class ErrorBuilder
    {
        public delegate Error BuildDelegate(ErrorBuilderInternal builder);

        public class ErrorBuilderInternal
        {
            public ErrorBuilderInternal(TextSpan textSpan)
            {
                TextSpan = textSpan;
            }

            public TextSpan TextSpan { get; }

            public Error UnrecognizedToken(object token) => new Error(
                TextSpan,
                ErrorCode.ErrUnrecognizedToken,
                "BCP001",
                $"The following token is not recognized: '{token}'.");

            public Error UnterminatedMultilineComment() => new Error(
                TextSpan,
                ErrorCode.ErrUnterminatedMultilineComment,
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public Error UnterminatedString() => new Error(
                TextSpan,
                ErrorCode.ErrUnterminatedString,
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public Error UnterminatedStringWithNewLine() => new Error(
                TextSpan,
                ErrorCode.ErrUnterminatedStringWithNewLine,
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public Error UnterminatedStringEscapeSequenceAtEof() => new Error(
                TextSpan,
                ErrorCode.ErrUnterminatedStringEscapeSequenceAtEof,
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public Error UnterminatedStringEscapeSequenceUnrecognized(object escapeChars) => new Error(
                TextSpan,
                ErrorCode.ErrUnterminatedStringEscapeSequenceUnrecognized,
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: {escapeChars}.");

            public Error UnrecognizedDeclaration() => new Error(
                TextSpan,
                ErrorCode.ErrUnrecognizedDeclaration,
                "BCP007",
                "This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.");

            public Error ExpectedParameterContinuation() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedParameterContinuation,
                "BCP008",
                "Expected the default keyword, a parameter modifier, or a newline at this location.");

            public Error UnrecognizedExpression() => new Error(
                TextSpan,
                ErrorCode.ErrUnrecognizedExpression,
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public Error InvalidInteger() => new Error(
                TextSpan,
                ErrorCode.ErrInvalidInteger,
                "BCP010",
                "Expected a valid 32-bit signed integer.");

            public Error InvalidType() => new Error(
                TextSpan,
                ErrorCode.ErrInvalidType,
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public Error ExpectedKeyword(object keyword) => new Error(
                TextSpan,
                ErrorCode.ErrExpectedKeyword,
                "BCP012",
                $"Expected the '{keyword}' keyword at this location.");

            public Error ExpectedParameterIdentifier() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedParameterIdentifier,
                "BCP013",
                "Expected a parameter identifier at this location.");

            public Error ExpectedParameterType() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedParameterType,
                "BCP014",
                $"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Error ExpectedVariableIdentifier() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedVariableIdentifier,
                "BCP015",
                "Expected a variable identifier at this location.");

            public Error ExpectedOutputIdentifier() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedOutputIdentifier,
                "BCP016",
                "Expected an output identifier at this location.");

            public Error ExpectedResourceIdentifier() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedResourceIdentifier,
                "BCP017",
                "Expected a resource identifier at this location.");

            public Error ExpectedCharacter(object character) => new Error(
                TextSpan,
                ErrorCode.ErrExpectedCharacter,
                "BCP018",
                $"Expected the '{character}' character at this location.");

            public Error ExpectedNewLine() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedNewLine,
                "BCP019",
                "Expected a new line character at this location.");

            public Error ExpectedPropertyIdentifier() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedPropertyIdentifier,
                "BCP020",
                "Expected a property identifier at this location.");

            public Error ExpectedNumericLiteral() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedNumericLiteral,
                "BCP021",
                "Expected a numeric literal at this location.");

            public Error ExpectedPropertyName() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedPropertyName,
                "BCP022",
                "Expected a property name at this location.");

            public Error ExpectedFunctionName() => new Error(
                TextSpan,
                ErrorCode.ErrExpectedFunctionName,
                "BCP023",
                "Expected a function name at this location.");

            public Error IdentifierNameExceedsLimit() => new Error(
                TextSpan,
                ErrorCode.ErrIdentifierNameExceedsLimit,
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public Error PropertyMultipleDeclarations(object property) => new Error(
                TextSpan,
                ErrorCode.ErrPropertyMultipleDeclarations,
                "BCP025",
                $"The property '{property}' is declared multiple times in this object. Remove or rename the duplicate properties.");

            public Error OutputTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                ErrorCode.ErrOutputTypeMismatch,
                "BCP026",
                $"The output expects a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Error ParameterTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                ErrorCode.ErrParameterTypeMismatch,
                "BCP027",
                $"The parameter expects a default value of type '{expectedType}' but provided value is of type '{actualType}'.");

            public Error IdentifierMultipleDeclarations(object identifier) => new Error(
                TextSpan,
                ErrorCode.ErrIdentifierMultipleDeclarations,
                "BCP028",
                $"Identifier '{identifier}' is declared multiple times. Remove or rename the duplicates.");

            public Error InvalidResourceType() => new Error(
                TextSpan,
                ErrorCode.ErrInvalidResourceType,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type.");

            public Error InvalidOutputType() => new Error(
                TextSpan,
                ErrorCode.ErrInvalidOutputType,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Error InvalidParameterType() => new Error(
                TextSpan,
                ErrorCode.ErrInvalidParameterType,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Error CompileTimeConstantRequired() => new Error(
                TextSpan,
                ErrorCode.ErrCompileTimeConstantRequired,
                "BCP032",
                "The value must be a compile-time constant.");

            public Error ExpectdValueTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                ErrorCode.ErrExpectdValueTypeMismatch,
                "BCP033",
                $"Expected a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Error ArrayTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                ErrorCode.ErrArrayTypeMismatch,
                "BCP034",
                $"The enclosing array expected an item of type '{expectedType}', but the provided item was of type '{actualType}'.");

            public Error MissingRequiredProperties(object properties) => new Error(
                TextSpan,
                ErrorCode.ErrMissingRequiredProperties,
                "BCP035",
                $"The specified object is missing the following required properties: {properties}.");

            public Error PropertyTypeMismatch(object property, object expectedType, object actualType) => new Error(
                TextSpan,
                ErrorCode.ErrPropertyTypeMismatch,
                "BCP036",
                $"The property '{property}' expected a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Error DisallowedProperty(object property, object type) => new Error(
                TextSpan,
                ErrorCode.ErrDisallowedProperty,
                "BCP037",
                $"The property '{property}' is not allowed on objects of type '{type}'.");

            public Error NotImplementedFunctionArgs() => new Error(
                TextSpan,
                ErrorCode.ErrNotImplementedFunctionArgs,
                "BCP038",
                "Function arguments are not currently supported.");

            public Error NotImplementedFunctionCalls() => new Error(
                TextSpan,
                ErrorCode.ErrNotImplementedFunctionCalls,
                "BCP039",
                "Function calls are not currently supported.");

            public Error NotImplementedPropertyAccess() => new Error(
                TextSpan,
                ErrorCode.ErrNotImplementedPropertyAccess,
                "BCP040",
                "Property access is not currently supported.");

            public Error NotImplementedArrayAccess() => new Error(
                TextSpan,
                ErrorCode.ErrNotImplementedArrayAccess,
                "BCP041",
                "Array access is not currently supported.");

            public Error NotImplementedVariableAccess() => new Error(
                TextSpan,
                ErrorCode.ErrNotImplementedVariableAccess,
                "BCP042",
                "Variable access is not currently supported.");

            public Error InvalidExpression() => new Error(
                TextSpan,
                ErrorCode.ErrInvalidExpression,
                "BCP043",
                "This is not a valid expression.");

            public Error UnaryOperatorInvalidType(object operatorName, object type) => new Error(
                TextSpan,
                ErrorCode.ErrUnaryOperatorInvalidType,
                "BCP044",
                $"Cannot apply operator '{operatorName}' to operand of type '{type}'.");

            public Error BinaryOperatorInvalidType(object operatorName, object type1, object type2) => new Error(
                TextSpan,
                ErrorCode.ErrBinaryOperatorInvalidType,
                "BCP045",
                $"Cannot apply operator '{operatorName}' to operands of type '{type1}' and '{type2}'.");

            public Error ValueTypeMismatch(object type) => new Error(
                TextSpan,
                ErrorCode.ErrValueTypeMismatch,
                "BCP046",
                $"Expected a value of type '{type}'.");
        }

        public static ErrorBuilderInternal ForPosition(TextSpan span)
            => new ErrorBuilderInternal(span);

        public static ErrorBuilderInternal ForPosition(IPositionable positionable)
            => new ErrorBuilderInternal(positionable.Span);
    }
}