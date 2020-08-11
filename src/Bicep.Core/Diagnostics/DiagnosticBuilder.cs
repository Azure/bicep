using System;
using Bicep.Core.Parser;

namespace Bicep.Core.Diagnostics
{
    public static class DiagnosticBuilder
    {
        public delegate Diagnostic BuildDelegate(DiagnosticBuilderInternal builder);

        public class DiagnosticBuilderInternal
        {
            public DiagnosticBuilderInternal(TextSpan textSpan)
            {
                TextSpan = textSpan;
            }

            public TextSpan TextSpan { get; }

            public Diagnostic UnrecognizedToken(object token) => new Diagnostic(
                TextSpan,
                "BCP001",
                $"The following token is not recognized: '{token}'.");

            public Diagnostic UnterminatedMultilineComment() => new Diagnostic(
                TextSpan,
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public Diagnostic UnterminatedString() => new Diagnostic(
                TextSpan,
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public Diagnostic UnterminatedStringWithNewLine() => new Diagnostic(
                TextSpan,
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public Diagnostic UnterminatedStringEscapeSequenceAtEof() => new Diagnostic(
                TextSpan,
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public Diagnostic UnterminatedStringEscapeSequenceUnrecognized(object escapeChars) => new Diagnostic(
                TextSpan,
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: {escapeChars}.");

            public Diagnostic UnrecognizedDeclaration() => new Diagnostic(
                TextSpan,
                "BCP007",
                "This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.");

            public Diagnostic ExpectedParameterContinuation() => new Diagnostic(
                TextSpan,
                "BCP008",
                "Expected the '=' token, a parameter modifier, or a newline at this location.");

            public Diagnostic UnrecognizedExpression() => new Diagnostic(
                TextSpan,
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public Diagnostic InvalidInteger() => new Diagnostic(
                TextSpan,
                "BCP010",
                "Expected a valid 32-bit signed integer.");

            public Diagnostic InvalidType() => new Diagnostic(
                TextSpan,
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public Diagnostic ExpectedKeyword(object keyword) => new Diagnostic(
                TextSpan,
                "BCP012",
                $"Expected the '{keyword}' keyword at this location.");

            public Diagnostic ExpectedParameterIdentifier() => new Diagnostic(
                TextSpan,
                "BCP013",
                "Expected a parameter identifier at this location.");

            public Diagnostic ExpectedParameterType() => new Diagnostic(
                TextSpan,
                "BCP014",
                $"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Diagnostic ExpectedVariableIdentifier() => new Diagnostic(
                TextSpan,
                "BCP015",
                "Expected a variable identifier at this location.");

            public Diagnostic ExpectedOutputIdentifier() => new Diagnostic(
                TextSpan,
                "BCP016",
                "Expected an output identifier at this location.");

            public Diagnostic ExpectedResourceIdentifier() => new Diagnostic(
                TextSpan,
                "BCP017",
                "Expected a resource identifier at this location.");

            public Diagnostic ExpectedCharacter(object character) => new Diagnostic(
                TextSpan,
                "BCP018",
                $"Expected the '{character}' character at this location.");

            public Diagnostic ExpectedNewLine() => new Diagnostic(
                TextSpan,
                "BCP019",
                "Expected a new line character at this location.");

            public Diagnostic ExpectedPropertyIdentifier() => new Diagnostic(
                TextSpan,
                "BCP020",
                "Expected a property identifier at this location.");

            public Diagnostic ExpectedNumericLiteral() => new Diagnostic(
                TextSpan,
                "BCP021",
                "Expected a numeric literal at this location.");

            public Diagnostic ExpectedPropertyName() => new Diagnostic(
                TextSpan,
                "BCP022",
                "Expected a property name at this location.");

            public Diagnostic ExpectedFunctionName() => new Diagnostic(
                TextSpan,
                "BCP023",
                "Expected a function name at this location.");

            public Diagnostic IdentifierNameExceedsLimit() => new Diagnostic(
                TextSpan,
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public Diagnostic PropertyMultipleDeclarations(object property) => new Diagnostic(
                TextSpan,
                "BCP025",
                $"The property '{property}' is declared multiple times in this object. Remove or rename the duplicate properties.");

            public Diagnostic OutputTypeMismatch(object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                "BCP026",
                $"The output expects a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Diagnostic ParameterTypeMismatch(object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                "BCP027",
                $"The parameter expects a default value of type '{expectedType}' but provided value is of type '{actualType}'.");

            public Diagnostic IdentifierMultipleDeclarations(object identifier) => new Diagnostic(
                TextSpan,
                "BCP028",
                $"Identifier '{identifier}' is declared multiple times. Remove or rename the duplicates.");

            public Diagnostic InvalidResourceType() => new Diagnostic(
                TextSpan,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type.");

            public Diagnostic InvalidOutputType() => new Diagnostic(
                TextSpan,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Diagnostic InvalidParameterType() => new Diagnostic(
                TextSpan,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.PrimitiveTypesString}.");

            public Diagnostic CompileTimeConstantRequired() => new Diagnostic(
                TextSpan,
                "BCP032",
                "The value must be a compile-time constant.");

            public Diagnostic ExpectdValueTypeMismatch(object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                "BCP033",
                $"Expected a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Diagnostic ArrayTypeMismatch(object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                "BCP034",
                $"The enclosing array expected an item of type '{expectedType}', but the provided item was of type '{actualType}'.");

            public Diagnostic MissingRequiredProperties(object properties) => new Diagnostic(
                TextSpan,
                "BCP035",
                $"The specified object is missing the following required properties: {properties}.");

            public Diagnostic PropertyTypeMismatch(object property, object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                "BCP036",
                $"The property '{property}' expected a value of type '{expectedType}' but the provided value is of type '{actualType}'.");

            public Diagnostic DisallowedProperty(object property, object type) => new Diagnostic(
                TextSpan,
                "BCP037",
                $"The property '{property}' is not allowed on objects of type '{type}'.");

            public Diagnostic NotImplementedFunctionArgs() => new Diagnostic(
                TextSpan,
                "BCP038",
                "Function arguments are not currently supported.");

            public Diagnostic NotImplementedFunctionCalls() => new Diagnostic(
                TextSpan,
                "BCP039",
                "Function calls are not currently supported.");

            public Diagnostic NotImplementedPropertyAccess() => new Diagnostic(
                TextSpan,
                "BCP040",
                "Property access is not currently supported.");

            public Diagnostic NotImplementedArrayAccess() => new Diagnostic(
                TextSpan,
                "BCP041",
                "Array access is not currently supported.");

            public Diagnostic NotImplementedVariableAccess() => new Diagnostic(
                TextSpan,
                "BCP042",
                "Variable access is not currently supported.");

            public Diagnostic InvalidExpression() => new Diagnostic(
                TextSpan,
                "BCP043",
                "This is not a valid expression.");

            public Diagnostic UnaryOperatorInvalidType(object operatorName, object type) => new Diagnostic(
                TextSpan,
                "BCP044",
                $"Cannot apply operator '{operatorName}' to operand of type '{type}'.");

            public Diagnostic BinaryOperatorInvalidType(object operatorName, object type1, object type2) => new Diagnostic(
                TextSpan,
                "BCP045",
                $"Cannot apply operator '{operatorName}' to operands of type '{type1}' and '{type2}'.");

            public Diagnostic ValueTypeMismatch(object type) => new Diagnostic(
                TextSpan,
                "BCP046",
                $"Expected a value of type '{type}'.");
        }

        public static DiagnosticBuilderInternal ForPosition(TextSpan span)
            => new DiagnosticBuilderInternal(span);

        public static DiagnosticBuilderInternal ForPosition(IPositionable positionable)
            => new DiagnosticBuilderInternal(positionable.Span);
    }
}