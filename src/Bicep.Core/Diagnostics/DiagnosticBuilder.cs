// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.TypeSystem;
using Bicep.Core.SemanticModel;
using Bicep.Core.Resources;

namespace Bicep.Core.Diagnostics
{
    public static class DiagnosticBuilder
    {
        public delegate ErrorDiagnostic ErrorBuilderDelegate(DiagnosticBuilderInternal builder);

        public class DiagnosticBuilderInternal
        {
            public DiagnosticBuilderInternal(TextSpan textSpan)
            {
                TextSpan = textSpan;
            }

            public TextSpan TextSpan { get; }

            public ErrorDiagnostic UnrecognizedToken(object token) => new ErrorDiagnostic(
                TextSpan,
                "BCP001",
                $"The following token is not recognized: '{token}'.");

            public ErrorDiagnostic UnterminatedMultilineComment() => new ErrorDiagnostic(
                TextSpan,
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public ErrorDiagnostic UnterminatedString() => new ErrorDiagnostic(
                TextSpan,
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public ErrorDiagnostic UnterminatedStringWithNewLine() => new ErrorDiagnostic(
                TextSpan,
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public ErrorDiagnostic UnterminatedStringEscapeSequenceAtEof() => new ErrorDiagnostic(
                TextSpan,
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public ErrorDiagnostic UnterminatedStringEscapeSequenceUnrecognized(object escapeChars) => new ErrorDiagnostic(
                TextSpan,
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: {escapeChars}.");

            public ErrorDiagnostic UnrecognizedDeclaration() => new ErrorDiagnostic(
                TextSpan,
                "BCP007",
                "This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.");

            public ErrorDiagnostic ExpectedParameterContinuation() => new ErrorDiagnostic(
                TextSpan,
                "BCP008",
                "Expected the '=' token, a parameter modifier, or a newline at this location.");

            public ErrorDiagnostic UnrecognizedExpression() => new ErrorDiagnostic(
                TextSpan,
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public ErrorDiagnostic InvalidInteger() => new ErrorDiagnostic(
                TextSpan,
                "BCP010",
                "Expected a valid 32-bit signed integer.");

            public ErrorDiagnostic InvalidType() => new ErrorDiagnostic(
                TextSpan,
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public ErrorDiagnostic ExpectedKeyword(object keyword) => new ErrorDiagnostic(
                TextSpan,
                "BCP012",
                $"Expected the '{keyword}' keyword at this location.");

            public ErrorDiagnostic ExpectedParameterIdentifier() => new ErrorDiagnostic(
                TextSpan,
                "BCP013",
                "Expected a parameter identifier at this location.");

            public ErrorDiagnostic ExpectedParameterType() => new ErrorDiagnostic(
                TextSpan,
                "BCP014",
                $"Expected a parameter type at this location. Please specify one of the following types: {LanguageConstants.DeclarationTypesString}.");

            public ErrorDiagnostic ExpectedVariableIdentifier() => new ErrorDiagnostic(
                TextSpan,
                "BCP015",
                "Expected a variable identifier at this location.");

            public ErrorDiagnostic ExpectedOutputIdentifier() => new ErrorDiagnostic(
                TextSpan,
                "BCP016",
                "Expected an output identifier at this location.");

            public ErrorDiagnostic ExpectedResourceIdentifier() => new ErrorDiagnostic(
                TextSpan,
                "BCP017",
                "Expected a resource identifier at this location.");

            public ErrorDiagnostic ExpectedCharacter(object character) => new ErrorDiagnostic(
                TextSpan,
                "BCP018",
                $"Expected the '{character}' character at this location.");

            public ErrorDiagnostic ExpectedNewLine() => new ErrorDiagnostic(
                TextSpan,
                "BCP019",
                "Expected a new line character at this location.");

            public ErrorDiagnostic ExpectedFunctionOrPropertyName() => new ErrorDiagnostic(
                TextSpan,
                "BCP020",
                "Expected a function or property name at this location.");

            public ErrorDiagnostic ExpectedNumericLiteral() => new ErrorDiagnostic(
                TextSpan,
                "BCP021",
                "Expected a numeric literal at this location.");

            public ErrorDiagnostic ExpectedPropertyName() => new ErrorDiagnostic(
                TextSpan,
                "BCP022",
                "Expected a property name at this location.");

            public ErrorDiagnostic ExpectedVariableOrFunctionName() => new ErrorDiagnostic(
                TextSpan,
                "BCP023",
                "Expected a variable or function name at this location.");

            public ErrorDiagnostic IdentifierNameExceedsLimit() => new ErrorDiagnostic(
                TextSpan,
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public ErrorDiagnostic PropertyMultipleDeclarations(object property) => new ErrorDiagnostic(
                TextSpan,
                "BCP025",
                $"The property '{property}' is declared multiple times in this object. Remove or rename the duplicate properties.");

            public ErrorDiagnostic OutputTypeMismatch(object expectedType, object actualType) => new ErrorDiagnostic(
                TextSpan,
                "BCP026",
                $"The output expects a value of type {expectedType} but the provided value is of type {actualType}.");

            public ErrorDiagnostic ParameterTypeMismatch(object expectedType, object actualType) => new ErrorDiagnostic(
                TextSpan,
                "BCP027",
                $"The parameter expects a default value of type {expectedType} but provided value is of type {actualType}.");

            public ErrorDiagnostic IdentifierMultipleDeclarations(object identifier) => new ErrorDiagnostic(
                TextSpan,
                "BCP028",
                $"Identifier '{identifier}' is declared multiple times. Remove or rename the duplicates.");

            public ErrorDiagnostic InvalidResourceType() => new ErrorDiagnostic(
                TextSpan,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type of format '<provider>/<types>@<apiVersion>'.");

            public ErrorDiagnostic InvalidOutputType() => new ErrorDiagnostic(
                TextSpan,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {LanguageConstants.DeclarationTypesString}.");

            public ErrorDiagnostic InvalidParameterType() => new ErrorDiagnostic(
                TextSpan,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {LanguageConstants.DeclarationTypesString}.");

            public ErrorDiagnostic CompileTimeConstantRequired() => new ErrorDiagnostic(
                TextSpan,
                "BCP032",
                "The value must be a compile-time constant.");

            public Diagnostic ExpectedValueTypeMismatch(bool warnInsteadOfError, object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP033",
                $"Expected a value of type {expectedType} but the provided value is of type {actualType}.");

            public Diagnostic ArrayTypeMismatch(bool warnInsteadOfError, object expectedType, object actualType) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP034",
                $"The enclosing array expected an item of type {expectedType}, but the provided item was of type {actualType}.");

            public Diagnostic MissingRequiredProperties(bool warnInsteadOfError, object properties) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP035",
                $"The specified object is missing the following required properties: {properties}.");

            public Diagnostic PropertyTypeMismatch(bool warnInsteadOfError, string property, TypeSymbol expectedType, TypeSymbol actualType) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP036",
                $"The property '{property}' expected a value of type {expectedType} but the provided value is of type {actualType}.");

            public Diagnostic DisallowedProperty(bool warnInsteadOfError, object property, object type) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP037",
                $"The property '{property}' is not allowed on objects of type {type}.");

            public Diagnostic DisallowedPropertyWithPermissibleProperties(bool warnInsteadOfError, object property, object type, IEnumerable<string> validUnspecifiedProperties) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP038",
                $"The property '{property}' is not allowed on objects of type {type}. Permissible properties include '{validUnspecifiedProperties.ConcatString("', '")}'.");

            public Diagnostic DisallowedInterpolatedKeyProperty(bool warnInsteadOfError, object type) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP039",
                $"String interpolation is not supported for keys on objects of type {type}.");

            public Diagnostic DisallowedInterpolatedKeyPropertyWithPermissibleProperties(bool warnInsteadOfError, object type, IEnumerable<string> validUnspecifiedProperties) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP040",
                $"String interpolation is not supported for keys on objects of type {type}. Permissible properties include '{validUnspecifiedProperties.ConcatString("', '")}'.");

            public ErrorDiagnostic InvalidExpression() => new ErrorDiagnostic(
                TextSpan,
                "BCP043",
                "This is not a valid expression.");

            public ErrorDiagnostic UnaryOperatorInvalidType(object operatorName, object type) => new ErrorDiagnostic(
                TextSpan,
                "BCP044",
                $"Cannot apply operator '{operatorName}' to operand of type {type}.");

            public ErrorDiagnostic BinaryOperatorInvalidType(object operatorName, object type1, object type2) => new ErrorDiagnostic(
                TextSpan,
                "BCP045",
                $"Cannot apply operator '{operatorName}' to operands of type {type1} and {type2}.");

            public ErrorDiagnostic ValueTypeMismatch(object type) => new ErrorDiagnostic(
                TextSpan,
                "BCP046",
                $"Expected a value of type {type}.");

            public ErrorDiagnostic ResourceTypeInterpolationUnsupported() => new ErrorDiagnostic(
                TextSpan,
                "BCP047",
                "String interpolation is unsupported for specifying the resource type.");

            public ErrorDiagnostic CannotResolveFunctionOverload(IList<string> overloadSignatures, TypeSymbol argumentType, IList<TypeSymbol> parameterTypes)
            {
                var messageBuilder = new StringBuilder();
                var overloadCount = overloadSignatures.Count;

                messageBuilder.Append("Cannot resolve function overload.");

                for (int i = 0; i < overloadCount; i++)
                {
                    messageBuilder
                        .Append("\n")
                        .Append($"  Overload {i + 1} of {overloadCount}, '{overloadSignatures[i]}', gave the following error:\n")
                        .Append($"    Argument of type {argumentType} is not assignable to parameter of type {parameterTypes[i]}.");
                }

                var message = messageBuilder.ToString();

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP048",
                    message);
            }

            public ErrorDiagnostic StringOrIntegerIndexerRequired(TypeSymbol wrongType) => new ErrorDiagnostic(
                TextSpan,
                "BCP049",
                $"The array index must be of type {LanguageConstants.String} or {LanguageConstants.Int} but the provided index was of type {wrongType}.");

            public ErrorDiagnostic UnknownProperty(TypeSymbol type, string badProperty) => new ErrorDiagnostic(
                TextSpan,
                "BCP052",
                $"The type {type} does not contain property '{badProperty}'.");

            public ErrorDiagnostic UnknownPropertyWithAvailableProperties(TypeSymbol type, string badProperty, IEnumerable<string> availableProperties) => new ErrorDiagnostic(
                TextSpan,
                "BCP053",
                $"The type {type} does not contain property '{badProperty}'. Available properties include '{availableProperties.ConcatString("', '")}'.");

            public ErrorDiagnostic NoPropertiesAllowed(TypeSymbol type) => new ErrorDiagnostic(
                TextSpan,
                "BCP054",
                $"The type {type} does not contain any properties.");

            public ErrorDiagnostic ObjectRequiredForPropertyAccess(TypeSymbol wrongType) => new ErrorDiagnostic(
                TextSpan,
                "BCP055",
                $"Cannot access properties of type {wrongType}. An {LanguageConstants.Object} type is required.");

            public ErrorDiagnostic AmbiguousSymbolReference(string name, IEnumerable<string> namespaces) => new ErrorDiagnostic(
                TextSpan,
                "BCP056",
                $"The reference to name '{name}' is ambiguous because it exists in namespaces '{namespaces.ConcatString(", ")}'. The reference must be fully-qualified.");

            public ErrorDiagnostic SymbolicNameDoesNotExist(string name) => new ErrorDiagnostic(
                TextSpan,
                "BCP057",
                $"The name '{name}' does not exist in the current context.");

            public ErrorDiagnostic OutputReferenceNotSupported(string name) => new ErrorDiagnostic(
                TextSpan,
                "BCP058",
                $"The name '{name}' is an output. Outputs cannot be referenced in expressions.");

            public ErrorDiagnostic SymbolicNameIsNotAFunction(string name) => new ErrorDiagnostic(
                TextSpan,
                "BCP059",
                $"The name '{name}' is not a function.");

            public ErrorDiagnostic CyclicExpression(IEnumerable<string> cycle) => new ErrorDiagnostic(
                TextSpan,
                "BCP080",
                $"The expression is involved in a cycle ({string.Join(" -> ", cycle)}).");

            public ErrorDiagnostic ReferencedSymbolHasErrors(string name) => new ErrorDiagnostic(
                TextSpan,
                "BCP062",
                $"The referenced declaration with name '{name}' is not valid.");

            public ErrorDiagnostic SymbolicNameIsNotAVariableOrParameter(string name) => new ErrorDiagnostic(
                TextSpan,
                "BCP063",
                $"The name '{name}' is not a parameter or variable.");

            public ErrorDiagnostic UnexpectedTokensInInterpolation() => new ErrorDiagnostic(
                TextSpan,
                "BCP064",
                "Found unexpected tokens in interpolated expression.");

            public ErrorDiagnostic FunctionOnlyValidInParameterDefaults(string functionName) => new ErrorDiagnostic(
                TextSpan,
                "BCP065",
                $"Function '{functionName}' is not valid at this location. It can only be used in parameter default declarations.");

            public ErrorDiagnostic FunctionOnlyValidInResourceBody(string functionName) => new ErrorDiagnostic(
                TextSpan,
                "BCP066",
                $"Function '{functionName}' is not valid at this location. It can only be used in resource declarations.");

            public ErrorDiagnostic ExpectedResourceTypeString() => new ErrorDiagnostic(
                TextSpan,
                "BCP068",
                "Expected a resource type string. Specify a valid resource type of format '<provider>/<types>@<apiVersion>'.");

            public ErrorDiagnostic ExpressionContainsObjectLiteralContainingOtherExpressions() => new ErrorDiagnostic(
                TextSpan,
                "BCP069",
                "The expression is inside an object literal that is itself part of another expression. This is not currently supported.");

            public ErrorDiagnostic ArgumentTypeMismatch(TypeSymbol argumentType, TypeSymbol parameterType) => new ErrorDiagnostic(
                TextSpan,
                "BCP070",
                $"Argument of type {argumentType} is not assignable to parameter of type {parameterType}.");

            public ErrorDiagnostic ArgumentCountMismatch(int argumentCount, int mininumArgumentCount, int? maximumArgumentCount)
            {
                string expected;

                if (!maximumArgumentCount.HasValue)
                {
                    expected = $"as least {mininumArgumentCount} {(mininumArgumentCount == 1 ? "argument" : "arguments")}";
                }
                else if (mininumArgumentCount == maximumArgumentCount.Value)
                {
                    expected = $"{mininumArgumentCount} {(mininumArgumentCount == 1 ? "argument" : "arguments")}";
                }
                else
                {
                    expected = $"{mininumArgumentCount} to {maximumArgumentCount} arguments";
                }

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP071",
                    $"Expected {expected}, but got {argumentCount}.");
            }

            public ErrorDiagnostic CannotReferenceSymbolInParamDefaultValue() => new ErrorDiagnostic(
                TextSpan,
                "BCP072",
                "This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values.");

            public Diagnostic CannotAssignToReadOnlyProperty(bool warnInsteadOfError, string property) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP073",
                $"The property '{property}' is read-only. Expressions cannot be assigned to read-only properties.");

            public ErrorDiagnostic ArraysRequireIntegerIndex(TypeSymbol wrongType) => new ErrorDiagnostic(
                TextSpan,
                "BCP074",
                $"Indexing over arrays requires an index of type {LanguageConstants.Int} but the provided index was of type {wrongType}.");

            public ErrorDiagnostic ObjectsRequireStringIndex(TypeSymbol wrongType) => new ErrorDiagnostic(
                TextSpan,
                "BCP075",
                $"Indexing over objects requires an index of type {LanguageConstants.String} but the provided index was of type {wrongType}.");

            public ErrorDiagnostic IndexerRequiresObjectOrArray(TypeSymbol wrongType) => new ErrorDiagnostic(
                TextSpan,
                "BCP076",
                $"Cannot index over expression of type {wrongType}. Arrays or objects are required.");

            public ErrorDiagnostic WriteOnlyProperty(TypeSymbol type, string badProperty) => new ErrorDiagnostic(
                TextSpan,
                "BCP077",
                $"The property '{badProperty}' on type '{type}' is write-only. Write-only properties cannot be accessed.");

            public Diagnostic MissingRequiredProperty(bool warnInsteadOfError, string propertyName, TypeSymbol expectedType) => new Diagnostic(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP078",
                $"The property '{propertyName}' requires a value of type {expectedType}, but none was supplied.");

            public ErrorDiagnostic CyclicSelfReference() => new ErrorDiagnostic(
                TextSpan,
                "BCP079",
                "This expression is referencing its own declaration, which is not allowed.");
        }

        public static DiagnosticBuilderInternal ForPosition(TextSpan span)
            => new DiagnosticBuilderInternal(span);

        public static DiagnosticBuilderInternal ForPosition(IPositionable positionable)
            => new DiagnosticBuilderInternal(positionable.Span);
    }
}
