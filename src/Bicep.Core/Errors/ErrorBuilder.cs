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
                "BCP001",
                $"The following token is not recognized: '{token}'.");

            public Error UnterminatedMultilineComment() => new Error(
                TextSpan,
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public Error UnterminatedString() => new Error(
                TextSpan,
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public Error UnterminatedStringWithNewLine() => new Error(
                TextSpan,
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public Error UnterminatedStringEscapeSequenceAtEof() => new Error(
                TextSpan,
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public Error UnterminatedStringEscapeSequenceUnrecognized(object escapeChars) => new Error(
                TextSpan,
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: {escapeChars}.");

            public Error UnrecognizedDeclaration() => new Error(
                TextSpan,
                "BCP007",
                "This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.");

            public Error ExpectedParameterContinuation() => new Error(
                TextSpan,
                "BCP008",
                "Expected the default keyword, a parameter modifier, or a newline at this location.");

            public Error UnrecognizedExpression() => new Error(
                TextSpan,
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public Error InvalidInteger() => new Error(
                TextSpan,
                "BCP010",
                "Expected a valid 32-bit signed integer.");

            public Error InvalidType() => new Error(
                TextSpan,
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public Error ExpectedKeyword(object keyword) => new Error(
                TextSpan,
                "BCP012",
                $"Expected the '{keyword}' keyword at this location.");

            public Error ExpectedParameterIdentifier() => new Error(
                TextSpan,
                "BCP013",
                "Expected a parameter identifier at this location.");

            public Error ExpectedParameterType() => new Error(
                TextSpan,
                "BCP014",
                $"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Error ExpectedVariableIdentifier() => new Error(
                TextSpan,
                "BCP015",
                "Expected a variable identifier at this location.");

            public Error ExpectedOutputIdentifier() => new Error(
                TextSpan,
                "BCP016",
                "Expected an output identifier at this location.");

            public Error ExpectedResourceIdentifier() => new Error(
                TextSpan,
                "BCP017",
                "Expected a resource identifier at this location.");

            public Error ExpectedCharacter(object character) => new Error(
                TextSpan,
                "BCP018",
                $"Expected the '{character}' character at this location.");

            public Error ExpectedNewLine() => new Error(
                TextSpan,
                "BCP019",
                "Expected a new line character at this location.");

            public Error ExpectedPropertyIdentifier() => new Error(
                TextSpan,
                "BCP020",
                "Expected a property identifier at this location.");

            public Error ExpectedNumericLiteral() => new Error(
                TextSpan,
                "BCP021",
                "Expected a numeric literal at this location.");

            public Error ExpectedPropertyName() => new Error(
                TextSpan,
                "BCP022",
                "Expected a property name at this location.");

            public Error ExpectedFunctionName() => new Error(
                TextSpan,
                "BCP023",
                "Expected a function name at this location.");

            public Error IdentifierNameExceedsLimit() => new Error(
                TextSpan,
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public Error PropertyMultipleDeclarations(object property) => new Error(
                TextSpan,
                "BCP025",
                $"The property '{property}' is declared multiple times in this object. Remove or rename the duplicate properties.");

            public Error OutputTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                "BCP026",
                $"The output expects a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Error ParameterTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                "BCP027",
                $"The parameter expects a default value of type '{expectedType}' but provided value is of type '{actualType}'.");

            public Error IdentifierMultipleDeclarations(object identifier) => new Error(
                TextSpan,
                "BCP028",
                $"Identifier '{identifier}' is declared multiple times. Remove or rename the duplicates.");

            public Error InvalidResourceType() => new Error(
                TextSpan,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type.");

            public Error InvalidOutputType() => new Error(
                TextSpan,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Error InvalidParameterType() => new Error(
                TextSpan,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Error CompileTimeConstantRequired() => new Error(
                TextSpan,
                "BCP032",
                "The value must be a compile-time constant.");

            public Error ExpectdValueTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                "BCP033",
                $"Expected a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Error ArrayTypeMismatch(object expectedType, object actualType) => new Error(
                TextSpan,
                "BCP034",
                $"The enclosing array expected an item of type '{expectedType}', but the provided item was of type '{actualType}'.");

            public Error MissingRequiredProperties(object properties) => new Error(
                TextSpan,
                "BCP035",
                $"The specified object is missing the following required properties: {properties}.");

            public Error PropertyTypeMismatch(object property, object expectedType, object actualType) => new Error(
                TextSpan,
                "BCP036",
                $"The property '{property}' expected a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Error DisallowedProperty(object property, object type) => new Error(
                TextSpan,
                "BCP037",
                $"The property '{property}' is not allowed on objects of type '{type}'.");

            public Error NotImplementedFunctionArgs() => new Error(
                TextSpan,
                "BCP038",
                "Function arguments are not currently supported.");

            public Error NotImplementedFunctionCalls() => new Error(
                TextSpan,
                "BCP039",
                "Function calls are not currently supported.");

            public Error NotImplementedPropertyAccess() => new Error(
                TextSpan,
                "BCP040",
                "Property access is not currently supported.");

            public Error NotImplementedArrayAccess() => new Error(
                TextSpan,
                "BCP041",
                "Array access is not currently supported.");

            public Error NotImplementedVariableAccess() => new Error(
                TextSpan,
                "BCP042",
                "Variable access is not currently supported.");

            public Error InvalidExpression() => new Error(
                TextSpan,
                "BCP043",
                "This is not a valid expression.");

            public Error UnaryOperatorInvalidType(object operatorName, object type) => new Error(
                TextSpan,
                "BCP044",
                $"Cannot apply operator '{operatorName}' to operand of type '{type}'.");

            public Error BinaryOperatorInvalidType(object operatorName, object type1, object type2) => new Error(
                TextSpan,
                "BCP045",
                $"Cannot apply operator '{operatorName}' to operands of type '{type1}' and '{type2}'.");

            public Error ValueTypeMismatch(object type) => new Error(
                TextSpan,
                "BCP046",
                $"Expected a value of type '{type}'.");
        }

        public static ErrorBuilderInternal ForPosition(TextSpan span)
            => new ErrorBuilderInternal(span);

        public static ErrorBuilderInternal ForPosition(IPositionable positionable)
            => new ErrorBuilderInternal(positionable.Span);
    }
}