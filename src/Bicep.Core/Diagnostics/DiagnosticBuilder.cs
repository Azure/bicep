// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Diagnostics
{
    public static class DiagnosticBuilder
    {
        public delegate ErrorDiagnostic ErrorBuilderDelegate(DiagnosticBuilderInternal builder);

        public delegate Diagnostic DiagnosticBuilderDelegate(DiagnosticBuilderInternal builder);

        public class DiagnosticBuilderInternal
        {
            private const string TypeInaccuracyClause = " If this is an inaccuracy in the documentation, please report it to the Bicep Team.";
            private static readonly Uri TypeInaccuracyLink = new("https://aka.ms/bicep-type-issues");

            public DiagnosticBuilderInternal(TextSpan textSpan)
            {
                TextSpan = textSpan;
            }

            public TextSpan TextSpan { get; }

            private static string ToQuotedString(IEnumerable<string> elements)
                => elements.Any() ? $"\"{elements.ConcatString("\", \"")}\"" : "";

            private static string BuildVariableDependencyChainClause(IEnumerable<string>? variableDependencyChain) => variableDependencyChain is not null
                ? $" You are referencing a variable which cannot be calculated at the start (\"{string.Join("\" -> \"", variableDependencyChain)}\")."
                : string.Empty;

            private static string BuildAccessiblePropertiesClause(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames) => accessedSymbolName is not null && accessiblePropertyNames is not null
                ? $" Properties of {accessedSymbolName} which can be calculated at the start include {ToQuotedString(accessiblePropertyNames.OrderBy(s => s))}."
                : string.Empty;

            public ErrorDiagnostic UnrecognizedToken(string token) => new(
                TextSpan,
                "BCP001",
                $"The following token is not recognized: \"{token}\".");

            public ErrorDiagnostic UnterminatedMultilineComment() => new(
                TextSpan,
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public ErrorDiagnostic UnterminatedString() => new(
                TextSpan,
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public ErrorDiagnostic UnterminatedStringWithNewLine() => new(
                TextSpan,
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public ErrorDiagnostic UnterminatedStringEscapeSequenceAtEof() => new(
                TextSpan,
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public ErrorDiagnostic UnterminatedStringEscapeSequenceUnrecognized(IEnumerable<string> escapeSequences) => new(
                TextSpan,
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following escape sequences are allowed: {ToQuotedString(escapeSequences)}.");

            public ErrorDiagnostic UnrecognizedDeclaration() => new(
                TextSpan,
                "BCP007",
                "This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.");

            public ErrorDiagnostic ExpectedParameterContinuation() => new(
                TextSpan,
                "BCP008",
                "Expected the \"=\" token, or a newline at this location.");

            public ErrorDiagnostic UnrecognizedExpression() => new(
                TextSpan,
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public ErrorDiagnostic InvalidInteger() => new(
                TextSpan,
                "BCP010",
                "Expected a valid 32-bit signed integer.");

            public ErrorDiagnostic InvalidType() => new(
                TextSpan,
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public ErrorDiagnostic ExpectedKeyword(string keyword) => new(
                TextSpan,
                "BCP012",
                $"Expected the \"{keyword}\" keyword at this location.");

            public ErrorDiagnostic ExpectedParameterIdentifier() => new(
                TextSpan,
                "BCP013",
                "Expected a parameter identifier at this location.");

            public ErrorDiagnostic ExpectedParameterType() => new(
                TextSpan,
                "BCP014",
                $"Expected a parameter type at this location. Please specify one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public ErrorDiagnostic ExpectedVariableIdentifier() => new(
                TextSpan,
                "BCP015",
                "Expected a variable identifier at this location.");

            public ErrorDiagnostic ExpectedOutputIdentifier() => new(
                TextSpan,
                "BCP016",
                "Expected an output identifier at this location.");

            public ErrorDiagnostic ExpectedResourceIdentifier() => new(
                TextSpan,
                "BCP017",
                "Expected a resource identifier at this location.");

            public ErrorDiagnostic ExpectedCharacter(string character) => new(
                TextSpan,
                "BCP018",
                $"Expected the \"{character}\" character at this location.");

            public ErrorDiagnostic ExpectedNewLine() => new(
                TextSpan,
                "BCP019",
                "Expected a new line character at this location.");

            public ErrorDiagnostic ExpectedFunctionOrPropertyName() => new(
                TextSpan,
                "BCP020",
                "Expected a function or property name at this location.");

            public ErrorDiagnostic ExpectedNumericLiteral() => new(
                TextSpan,
                "BCP021",
                "Expected a numeric literal at this location.");

            public ErrorDiagnostic ExpectedPropertyName() => new(
                TextSpan,
                "BCP022",
                "Expected a property name at this location.");

            public ErrorDiagnostic ExpectedVariableOrFunctionName() => new(
                TextSpan,
                "BCP023",
                "Expected a variable or function name at this location.");

            public ErrorDiagnostic IdentifierNameExceedsLimit() => new(
                TextSpan,
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public ErrorDiagnostic PropertyMultipleDeclarations(string property) => new(
                TextSpan,
                "BCP025",
                $"The property \"{property}\" is declared multiple times in this object. Remove or rename the duplicate properties.");

            public ErrorDiagnostic OutputTypeMismatch(TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                "BCP026",
                $"The output expects a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public ErrorDiagnostic ParameterTypeMismatch(TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                "BCP027",
                $"The parameter expects a default value of type \"{expectedType}\" but provided value is of type \"{actualType}\".");

            public ErrorDiagnostic IdentifierMultipleDeclarations(string identifier) => new(
                TextSpan,
                "BCP028",
                $"Identifier \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public ErrorDiagnostic InvalidResourceType() => new(
                TextSpan,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type of format \"<provider>/<types>@<apiVersion>\".");

            public ErrorDiagnostic InvalidOutputType() => new(
                TextSpan,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public ErrorDiagnostic InvalidParameterType() => new(
                TextSpan,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public ErrorDiagnostic CompileTimeConstantRequired() => new(
                TextSpan,
                "BCP032",
                "The value must be a compile-time constant.");

            public Diagnostic ExpectedValueTypeMismatch(bool warnInsteadOfError, TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP033",
                $"Expected a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic ArrayTypeMismatch(bool warnInsteadOfError, TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP034",
                $"The enclosing array expected an item of type \"{expectedType}\", but the provided item was of type \"{actualType}\".");

            public Diagnostic MissingRequiredProperties(bool warnInsteadOfError, Symbol? sourceDeclaration, ObjectSyntax objectSyntax, IEnumerable<string> properties, string blockName)
            {
                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" from source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                var newSyntax = objectSyntax.AddChildrenWithFormatting(
                    properties.Select(p => SyntaxFactory.CreateObjectProperty(p, SyntaxFactory.EmptySkippedTrivia))
                );

                var codeFix = new CodeFix("Add required properties", true, new CodeReplacement(objectSyntax.Span, newSyntax.ToTextPreserveFormatting()));

                return new FixableDiagnostic(
                    TextSpan,
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP035",
                    $"The specified \"{blockName}\" declaration is missing the following required properties{sourceDeclarationClause}: {ToQuotedString(properties)}.",
                    null,
                    null,
                    codeFix);
            }

            public Diagnostic PropertyTypeMismatch(bool warnInsteadOfError, Symbol? sourceDeclaration, string property, TypeSymbol expectedType, TypeSymbol actualType)
            {
                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" in source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return new(
                    TextSpan,
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP036",
                    $"The property \"{property}\" expected a value of type \"{expectedType}\" but the provided value{sourceDeclarationClause} is of type \"{actualType}\".");
            }

            public Diagnostic DisallowedProperty(bool warnInsteadOfError, Symbol? sourceDeclaration, string property, TypeSymbol type, IEnumerable<string> validUnspecifiedProperties, bool isResourceSyntax)
            {
                var permissiblePropertiesClause = validUnspecifiedProperties.Any()
                    ? $" Permissible properties include {ToQuotedString(validUnspecifiedProperties)}."
                    : $" No other properties are allowed.";

                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" from source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return new(
                    TextSpan,
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP037",
                    $"The property \"{property}\"{sourceDeclarationClause} is not allowed on objects of type \"{type}\".{permissiblePropertiesClause}{(isResourceSyntax ? TypeInaccuracyClause : string.Empty)}", isResourceSyntax ? TypeInaccuracyLink : null);
            }

            public Diagnostic DisallowedInterpolatedKeyProperty(bool warnInsteadOfError, Symbol? sourceDeclaration, TypeSymbol type, IEnumerable<string> validUnspecifiedProperties)
            {
                var permissiblePropertiesClause = validUnspecifiedProperties.Any()
                    ? $" Permissible properties include {ToQuotedString(validUnspecifiedProperties)}."
                    : $" No other properties are allowed.";

                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" in source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return new(
                    TextSpan,
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP040",
                    $"String interpolation is not supported for keys on objects of type \"{type}\"{sourceDeclarationClause}.{permissiblePropertiesClause}");
            }

            public ErrorDiagnostic VariableTypeAssignmentDisallowed(TypeSymbol valueType) => new(
                TextSpan,
                "BCP041",
                $"Values of type \"{valueType}\" cannot be assigned to a variable.");

            public ErrorDiagnostic InvalidExpression() => new(
                TextSpan,
                "BCP043",
                "This is not a valid expression.");

            public ErrorDiagnostic UnaryOperatorInvalidType(string operatorName, TypeSymbol type) => new(
                TextSpan,
                "BCP044",
                $"Cannot apply operator \"{operatorName}\" to operand of type \"{type}\".");

            public ErrorDiagnostic BinaryOperatorInvalidType(string operatorName, TypeSymbol type1, TypeSymbol type2) => new(
                TextSpan,
                "BCP045",
                $"Cannot apply operator \"{operatorName}\" to operands of type \"{type1}\" and \"{type2}\".");

            public ErrorDiagnostic ValueTypeMismatch(TypeSymbol type) => new(
                TextSpan,
                "BCP046",
                $"Expected a value of type \"{type}\".");

            public ErrorDiagnostic ResourceTypeInterpolationUnsupported() => new(
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
                        .Append('\n')
                        .Append($"  Overload {i + 1} of {overloadCount}, \"{overloadSignatures[i]}\", gave the following error:\n")
                        .Append($"    Argument of type \"{argumentType}\" is not assignable to parameter of type \"{parameterTypes[i]}\".");
                }

                var message = messageBuilder.ToString();

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP048",
                    message);
            }

            public ErrorDiagnostic StringOrIntegerIndexerRequired(TypeSymbol wrongType) => new(
                TextSpan,
                "BCP049",
                $"The array index must be of type \"{LanguageConstants.String}\" or \"{LanguageConstants.Int}\" but the provided index was of type \"{wrongType}\".");

            public ErrorDiagnostic FilePathIsEmpty() => new(
                TextSpan,
                "BCP050",
                "The specified path is empty.");

            public ErrorDiagnostic FilePathBeginsWithForwardSlash() => new(
                TextSpan,
                "BCP051",
                "The specified path begins with \"/\". Files must be referenced using relative paths.");

            public Diagnostic UnknownProperty(bool warnInsteadOfError, TypeSymbol type, string badProperty) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP052",
                $"The type \"{type}\" does not contain property \"{badProperty}\".");

            public Diagnostic UnknownPropertyWithAvailableProperties(bool warnInsteadOfError, TypeSymbol type, string badProperty, IEnumerable<string> availableProperties) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP053",
                $"The type \"{type}\" does not contain property \"{badProperty}\". Available properties include {ToQuotedString(availableProperties)}.");

            public ErrorDiagnostic NoPropertiesAllowed(TypeSymbol type) => new(
                TextSpan,
                "BCP054",
                $"The type \"{type}\" does not contain any properties.");

            public ErrorDiagnostic ObjectRequiredForPropertyAccess(TypeSymbol wrongType) => new(
                TextSpan,
                "BCP055",
                $"Cannot access properties of type \"{wrongType}\". An \"{LanguageConstants.Object}\" type is required.");

            public ErrorDiagnostic AmbiguousSymbolReference(string name, IEnumerable<string> namespaces) => new(
                TextSpan,
                "BCP056",
                $"The reference to name \"{name}\" is ambiguous because it exists in namespaces {ToQuotedString(namespaces)}. The reference must be fully-qualified.");

            public ErrorDiagnostic SymbolicNameDoesNotExist(string name) => new(
                TextSpan,
                "BCP057",
                $"The name \"{name}\" does not exist in the current context.");

            public ErrorDiagnostic OutputReferenceNotSupported(string name) => new(
                TextSpan,
                "BCP058",
                $"The name \"{name}\" is an output. Outputs cannot be referenced in expressions.");

            public ErrorDiagnostic SymbolicNameIsNotAFunction(string name) => new(
                TextSpan,
                "BCP059",
                $"The name \"{name}\" is not a function.");

            public ErrorDiagnostic VariablesFunctionNotSupported() => new(
                TextSpan,
                "BCP060",
                $"The \"variables\" function is not supported. Directly reference variables by their symbolic names.");

            public ErrorDiagnostic ParametersFunctionNotSupported() => new(
                TextSpan,
                "BCP061",
                $"The \"parameters\" function is not supported. Directly reference parameters by their symbolic names.");

            public ErrorDiagnostic ReferencedSymbolHasErrors(string name) => new(
                TextSpan,
                "BCP062",
                $"The referenced declaration with name \"{name}\" is not valid.");

            public ErrorDiagnostic SymbolicNameIsNotAVariableOrParameter(string name) => new(
                TextSpan,
                "BCP063",
                $"The name \"{name}\" is not a parameter, variable, resource or module.");

            public ErrorDiagnostic UnexpectedTokensInInterpolation() => new(
                TextSpan,
                "BCP064",
                "Found unexpected tokens in interpolated expression.");

            public ErrorDiagnostic FunctionOnlyValidInParameterDefaults(string functionName) => new(
                TextSpan,
                "BCP065",
                $"Function \"{functionName}\" is not valid at this location. It can only be used as a parameter default value.");

            public ErrorDiagnostic FunctionOnlyValidInResourceBody(string functionName) => new(
                TextSpan,
                "BCP066",
                $"Function \"{functionName}\" is not valid at this location. It can only be used in resource declarations.");

            public ErrorDiagnostic ObjectRequiredForMethodAccess(TypeSymbol wrongType) => new(
                TextSpan,
                "BCP067",
                $"Cannot call functions on type \"{wrongType}\". An \"{LanguageConstants.Object}\" type is required.");

            public ErrorDiagnostic ExpectedResourceTypeString() => new(
                TextSpan,
                "BCP068",
                "Expected a resource type string. Specify a valid resource type of format \"<provider>/<types>@<apiVersion>\".");

            public ErrorDiagnostic FunctionNotSupportedOperatorAvailable(string function, string @operator) => new(
                TextSpan,
                "BCP069",
                $"The function \"{function}\" is not supported. Use the \"{@operator}\" operator instead.");

            public ErrorDiagnostic ArgumentTypeMismatch(TypeSymbol argumentType, TypeSymbol parameterType) => new(
                TextSpan,
                "BCP070",
                $"Argument of type \"{argumentType}\" is not assignable to parameter of type \"{parameterType}\".");

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

            public ErrorDiagnostic CannotReferenceSymbolInParamDefaultValue() => new(
                TextSpan,
                "BCP072",
                "This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values.");

            public Diagnostic CannotAssignToReadOnlyProperty(bool warnInsteadOfError, string property) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP073",
                $"The property \"{property}\" is read-only. Expressions cannot be assigned to read-only properties.");

            public ErrorDiagnostic ArraysRequireIntegerIndex(TypeSymbol wrongType) => new(
                TextSpan,
                "BCP074",
                $"Indexing over arrays requires an index of type \"{LanguageConstants.Int}\" but the provided index was of type \"{wrongType}\".");

            public ErrorDiagnostic ObjectsRequireStringIndex(TypeSymbol wrongType) => new(
                TextSpan,
                "BCP075",
                $"Indexing over objects requires an index of type \"{LanguageConstants.String}\" but the provided index was of type \"{wrongType}\".");

            public ErrorDiagnostic IndexerRequiresObjectOrArray(TypeSymbol wrongType) => new(
                TextSpan,
                "BCP076",
                $"Cannot index over expression of type \"{wrongType}\". Arrays or objects are required.");

            public Diagnostic WriteOnlyProperty(bool warnInsteadOfError, TypeSymbol type, string badProperty) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP077",
                $"The property \"{badProperty}\" on type \"{type}\" is write-only. Write-only properties cannot be accessed.");

            public Diagnostic MissingRequiredProperty(bool warnInsteadOfError, string propertyName, TypeSymbol expectedType) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP078",
                $"The property \"{propertyName}\" requires a value of type \"{expectedType}\", but none was supplied.");

            public ErrorDiagnostic CyclicExpressionSelfReference() => new(
                TextSpan,
                "BCP079",
                "This expression is referencing its own declaration, which is not allowed.");

            public ErrorDiagnostic CyclicExpression(IEnumerable<string> cycle) => new(
                TextSpan,
                "BCP080",
                $"The expression is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ResourceTypesUnavailable(ResourceTypeReference resourceTypeReference) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP081",
                $"Resource type \"{resourceTypeReference.FormatName()}\" does not have types available.");

            public FixableErrorDiagnostic SymbolicNameDoesNotExistWithSuggestion(string name, string suggestedName) => new(
                TextSpan,
                "BCP082",
                $"The name \"{name}\" does not exist in the current context. Did you mean \"{suggestedName}\"?",
                null,
                null,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeManipulator.Replace(TextSpan, suggestedName)));

            public FixableDiagnostic UnknownPropertyWithSuggestion(bool warnInsteadOfError, TypeSymbol type, string badProperty, string suggestedProperty) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP083",
                $"The type \"{type}\" does not contain property \"{badProperty}\". Did you mean \"{suggestedProperty}\"?",
                null,
                null,
                new CodeFix($"Change \"{badProperty}\" to \"{suggestedProperty}\"", true, CodeManipulator.Replace(TextSpan, suggestedProperty)));

            public ErrorDiagnostic SymbolicNameCannotUseReservedNamespaceName(string name, IEnumerable<string> namespaces) => new(
                TextSpan,
                "BCP084",
                $"The symbolic name \"{name}\" is reserved. Please use a different symbolic name. Reserved namespaces are {ToQuotedString(namespaces.OrderBy(ns => ns))}.");

            public ErrorDiagnostic FilePathContainsForbiddenCharacters(IEnumerable<char> forbiddenChars) => new(
                TextSpan,
                "BCP085",
                $"The specified file path contains one ore more invalid path characters. The following are not permitted: {ToQuotedString(forbiddenChars.OrderBy(x => x).Select(x => x.ToString()))}.");

            public ErrorDiagnostic FilePathHasForbiddenTerminator(IEnumerable<char> forbiddenPathTerminatorChars) => new(
                TextSpan,
                "BCP086",
                $"The specified file path ends with an invalid character. The following are not permitted: {ToQuotedString(forbiddenPathTerminatorChars.OrderBy(x => x).Select(x => x.ToString()))}.");

            public ErrorDiagnostic ComplexLiteralsNotAllowed() => new(
                TextSpan,
                "BCP087",
                "Array and object literals are not allowed here.");

            public FixableDiagnostic PropertyStringLiteralMismatchWithSuggestion(bool warnInsteadOfError, string property, TypeSymbol expectedType, string actualStringLiteral, string suggestedStringLiteral) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP088",
                $"The property \"{property}\" expected a value of type \"{expectedType}\" but the provided value is of type \"{actualStringLiteral}\". Did you mean \"{suggestedStringLiteral}\"?",
                null,
                null,
                new CodeFix($"Change \"{actualStringLiteral}\" to \"{suggestedStringLiteral}\"", true, CodeManipulator.Replace(TextSpan, suggestedStringLiteral)));

            public FixableDiagnostic DisallowedPropertyWithSuggestion(bool warnInsteadOfError, string property, TypeSymbol type, string suggestedProperty) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP089",
                $"The property \"{property}\" is not allowed on objects of type \"{type}\". Did you mean \"{suggestedProperty}\"?",
                null,
                null,
                new CodeFix($"Change \"{property}\" to \"{suggestedProperty}\"", true, CodeManipulator.Replace(TextSpan, suggestedProperty)));

            public ErrorDiagnostic ModulePathHasNotBeenSpecified() => new(
                TextSpan,
                "BCP090",
                "This module declaration is missing a file path reference.");

            public ErrorDiagnostic ErrorOccurredReadingFile(string failureMessage) => new(
                TextSpan,
                "BCP091",
                $"An error occurred reading file. {failureMessage}");

            public ErrorDiagnostic FilePathInterpolationUnsupported() => new(
                TextSpan,
                "BCP092",
                "String interpolation is not supported in file paths.");

            public ErrorDiagnostic FilePathCouldNotBeResolved(string filePath, string parentPath) => new(
                TextSpan,
                "BCP093",
                $"File path \"{filePath}\" could not be resolved relative to \"{parentPath}\".");

            public ErrorDiagnostic CyclicModuleSelfReference() => new(
                TextSpan,
                "BCP094",
                "This module references itself, which is not allowed.");

            public ErrorDiagnostic CyclicModule(IEnumerable<string> cycle) => new(
                TextSpan,
                "BCP095",
                $"The module is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public ErrorDiagnostic ExpectedModuleIdentifier() => new(
                TextSpan,
                "BCP096",
                "Expected a module identifier at this location.");

            public ErrorDiagnostic ExpectedModulePathString() => new(
                TextSpan,
                "BCP097",
                "Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep'");

            public ErrorDiagnostic FilePathContainsBackSlash() => new(
                TextSpan,
                "BCP098",
                "The specified file path contains a \"\\\" character. Use \"/\" instead as the directory separator character.");

            public ErrorDiagnostic AllowedMustContainItems() => new(
                TextSpan,
                "BCP099",
                $"The \"{LanguageConstants.ParameterAllowedPropertyName}\" array must contain one or more items.");

            public ErrorDiagnostic IfFunctionNotSupported() => new(
                TextSpan,
                "BCP100",
                "The \"if\" function is not supported. Use the ternary conditional operator instead.");

            public ErrorDiagnostic CreateArrayFunctionNotSupported() => new(
                TextSpan,
                "BCP101",
                "The \"createArray\" function is not supported. Construct an array literal using [].");

            public ErrorDiagnostic CreateObjectFunctionNotSupported() => new(
                TextSpan,
                "BCP102",
                "The \"createObject\" function is not supported. Construct an object literal using {}.");

            public ErrorDiagnostic DoubleQuoteToken(string token) => new(
                TextSpan,
                "BCP103",
                $"The following token is not recognized: \"{token}\". Strings are defined using single quotes in bicep.");

            public ErrorDiagnostic ReferencedModuleHasErrors() => new(
                TextSpan,
                "BCP104",
                $"The referenced module has errors.");

            public ErrorDiagnostic UnableToLoadNonFileUri(Uri fileUri) => new(
                TextSpan,
                "BCP105",
                $"Unable to load file from URI \"{fileUri}\".");

            public ErrorDiagnostic UnexpectedCommaSeparator() => new(
                TextSpan,
                "BCP106",
                "Expected a new line character at this location. Commas are not used as separator delimiters.");

            public ErrorDiagnostic FunctionDoesNotExistInNamespace(Symbol namespaceType, string name) => new(
                TextSpan,
                "BCP107",
                $"The function \"{name}\" does not exist in namespace \"{namespaceType.Name}\".");

            public FixableErrorDiagnostic FunctionDoesNotExistInNamespaceWithSuggestion(Symbol namespaceType, string name, string suggestedName) => new(
                TextSpan,
                "BCP108",
                $"The function \"{name}\" does not exist in namespace \"{namespaceType.Name}\". Did you mean \"{suggestedName}\"?",
                null,
                null,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeManipulator.Replace(TextSpan, suggestedName)));

            public ErrorDiagnostic FunctionDoesNotExistOnObject(TypeSymbol type, string name) => new(
                TextSpan,
                "BCP109",
                $"The type \"{type}\" does not contain function \"{name}\".");

            public FixableErrorDiagnostic FunctionDoesNotExistOnObjectWithSuggestion(TypeSymbol type, string name, string suggestedName) => new(
                TextSpan,
                "BCP110",
                $"The type \"{type}\" does not contain function \"{name}\". Did you mean \"{suggestedName}\"?",
                null,
                null,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeManipulator.Replace(TextSpan, suggestedName)));

            public ErrorDiagnostic FilePathContainsControlChars() => new(
                TextSpan,
                "BCP111",
                $"The specified file path contains invalid control code characters.");

            public ErrorDiagnostic TargetScopeMultipleDeclarations() => new(
                TextSpan,
                "BCP112",
                $"The \"{LanguageConstants.TargetScopeKeyword}\" cannot be declared multiple times in one file.");

            public Diagnostic InvalidModuleScopeForTenantScope() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP113",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeTenant}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include tenant: tenant(), named management group: managementGroup(<name>), named subscription: subscription(<subId>), or named resource group in a named subscription: resourceGroup(<subId>, <name>).");

            public Diagnostic InvalidModuleScopeForManagementScope() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP114",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeManagementGroup}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include current management group: managementGroup(), named management group: managementGroup(<name>), named subscription: subscription(<subId>), tenant: tenant(), or named resource group in a named subscription: resourceGroup(<subId>, <name>).");

            public Diagnostic InvalidModuleScopeForSubscriptionScope() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP115",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeSubscription}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include current subscription: subscription(), named subscription: subscription(<subId>), named resource group in same subscription: resourceGroup(<name>), named resource group in different subscription: resourceGroup(<subId>, <name>), or tenant: tenant().");

            public Diagnostic InvalidModuleScopeForResourceGroup() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP116",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeResourceGroup}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include current resource group: resourceGroup(), named resource group in same subscription: resourceGroup(<name>), named resource group in a different subscription: resourceGroup(<subId>, <name>), current subscription: subscription(), named subscription: subscription(<subId>) or tenant: tenant().");

            public ErrorDiagnostic EmptyIndexerNotAllowed() => new(
                TextSpan,
                "BCP117",
                "An empty indexer is not allowed. Specify a valid expression."
            );

            public ErrorDiagnostic ExpectBodyStartOrIfOrLoopStart() => new(
                TextSpan,
                "BCP118",
                "Expected the \"{\" character, the \"[\" character, or the \"if\" keyword at this location.");

            public Diagnostic InvalidExtensionResourceScope() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP119",
                $"Unsupported scope for extension resource deployment. Expected a resource reference.");

            public Diagnostic RuntimeValueNotAllowedInProperty(string propertyName, string? objectTypeName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP120",
                    $"This expression is being used in an assignment to the \"{propertyName}\" property of the \"{objectTypeName}\" type, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public ErrorDiagnostic ResourceMultipleDeclarations(IEnumerable<string> resourceNames) => new(
                TextSpan,
                "BCP121",
                $"Resources: {ToQuotedString(resourceNames)} are defined with this same name in a file. Rename them or split into different modules.");

            public ErrorDiagnostic ModuleMultipleDeclarations(IEnumerable<string> moduleNames) => new(
                TextSpan,
                "BCP122",
                $"Modules: {ToQuotedString(moduleNames)} are defined with this same name and this same scope in a file. Rename them or split into different modules.");

            public ErrorDiagnostic ExpectedNamespaceOrDecoratorName() => new(
                TextSpan,
                "BCP123",
                "Expected a namespace or decorator name at this location.");

            public ErrorDiagnostic CannotAttachDecoratorToTarget(string decoratorName, TypeSymbol attachableType, TypeSymbol targetType) => new(
                TextSpan,
                "BCP124",
                $"The decorator \"{decoratorName}\" can only be attached to targets of type \"{attachableType}\", but the target has type \"{targetType}\".");

            public ErrorDiagnostic CannotUseFunctionAsParameterDecorator(string functionName) => new(
                TextSpan,
                "BCP125",
                $"Function \"{functionName}\" cannot be used as a parameter decorator.");

            public ErrorDiagnostic CannotUseFunctionAsVariableDecorator(string functionName) => new(
                TextSpan,
                "BCP126",
                $"Function \"{functionName}\" cannot be used as a variable decorator.");

            public ErrorDiagnostic CannotUseFunctionAsResourceDecorator(string functionName) => new(
                TextSpan,
                "BCP127",
                $"Function \"{functionName}\" cannot be used as a resource decorator.");

            public ErrorDiagnostic CannotUseFunctionAsModuleDecorator(string functionName) => new(
                TextSpan,
                "BCP128",
                $"Function \"{functionName}\" cannot be used as a module decorator.");

            public ErrorDiagnostic CannotUseFunctionAsOutputDecorator(string functionName) => new(
                TextSpan,
                "BCP129",
                $"Function \"{functionName}\" cannot be used as an output decorator.");

            public ErrorDiagnostic DecoratorsNotAllowed() => new(
                TextSpan,
                "BCP130",
                "Decorators are not allowed here.");

            public ErrorDiagnostic ExpectedDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP132",
                "Expected a declaration after the decorator.");

            public ErrorDiagnostic InvalidUnicodeEscape() => new(
                TextSpan,
                "BCP133",
                "The unicode escape sequence is not valid. Valid unicode escape sequences range from \\u{0} to \\u{10FFFF}.");

            public Diagnostic UnsupportedModuleScope(ResourceScope suppliedScope, ResourceScope supportedScopes) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP134",
                $"Scope {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(suppliedScope))} is not valid for this module. Permitted scopes: {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(supportedScopes))}.");

            public Diagnostic UnsupportedResourceScope(ResourceScope suppliedScope, ResourceScope supportedScopes) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP135",
                $"Scope {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(suppliedScope))} is not valid for this resource type. Permitted scopes: {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(supportedScopes))}.");

            public ErrorDiagnostic ExpectedLoopVariableIdentifier() => new(
                TextSpan,
                "BCP136",
                "Expected a loop item variable identifier at this location.");

            public ErrorDiagnostic LoopArrayExpressionTypeMismatch(TypeSymbol actualType) => new(
                TextSpan,
                "BCP137",
                $"Loop expected an expression of type \"{LanguageConstants.Array}\" but the provided value is of type \"{actualType}\".");

            public ErrorDiagnostic ForExpressionsNotSupportedHere() => new(
                TextSpan,
                "BCP138",
                "For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties.");

            public Diagnostic InvalidCrossResourceScope() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP139",
                $"The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module.");

            public ErrorDiagnostic UnterminatedMultilineString() => new(
                TextSpan,
                "BCP140",
                $"The multi-line string at this location is not terminated. Terminate it with \"'''\".");

            public ErrorDiagnostic ExpressionNotCallable() => new(
                TextSpan,
                "BCP141",
                "The expression cannot be used as a decorator as it is not callable.");

            public ErrorDiagnostic TooManyPropertyForExpressions() => new(
                TextSpan,
                "BCP142",
                "Property value for-expressions cannot be nested.");

            public ErrorDiagnostic ExpressionedPropertiesNotAllowedWithLoops() => new(
                TextSpan,
                "BCP143",
                "For-expressions cannot be used with properties whose names are also expressions.");

            public ErrorDiagnostic DirectAccessToCollectionNotSupported() => new(
                TextSpan,
                "BCP144",
                "Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression.");

            public ErrorDiagnostic OutputMultipleDeclarations(string identifier) => new(
                TextSpan,
                "BCP145",
                $"Output \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public ErrorDiagnostic ExpectedOutputType() => new(
                TextSpan,
                "BCP146",
                $"Expected an output type at this location. Please specify one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public ErrorDiagnostic ExpectedParameterDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP147",
                "Expected a parameter declaration after the decorator.");

            public ErrorDiagnostic ExpectedVariableDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP148",
                "Expected a variable declaration after the decorator.");

            public ErrorDiagnostic ExpectedResourceDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP149",
                "Expected a resource declaration after the decorator.");

            public ErrorDiagnostic ExpectedModuleDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP150",
                "Expected a module declaration after the decorator.");

            public ErrorDiagnostic ExpectedOutputDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP151",
                "Expected an output declaration after the decorator.");

            public ErrorDiagnostic CannotUseFunctionAsDecorator(string functionName) => new(
                TextSpan,
                "BCP152",
                $"Function \"{functionName}\" cannot be used as a decorator.");

            public ErrorDiagnostic ExpectedResourceOrModuleDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP153",
                "Expected a resource or module declaration after the decorator.");

            public ErrorDiagnostic BatchSizeTooSmall(long value, long limit) => new(
                TextSpan,
                "BCP154",
                $"Expected a batch size of at least {limit} but the specified value was \"{value}\".");

            public ErrorDiagnostic BatchSizeNotAllowed(string decoratorName) => new(
                TextSpan,
                "BCP155",
                $"The decorator \"{decoratorName}\" can only be attached to resource or module collections.");

            public ErrorDiagnostic InvalidResourceTypeSegment(string typeSegment) => new(
                TextSpan,
                "BCP156",
                $"The resource type segment \"{typeSegment}\" is invalid. Nested resources must specify a single type segment, and optionally can specify an api version using the format \"<type>@<apiVersion>\".");

            public ErrorDiagnostic InvalidAncestorResourceType(string resourceName) => new(
                TextSpan,
                "BCP157",
                $"The resource type cannot be determined due to an error in containing resource \"{resourceName}\".");

            public ErrorDiagnostic ResourceRequiredForResourceAccess(string wrongType) => new(
                TextSpan,
                "BCP158",
                $"Cannot access nested resources of type \"{wrongType}\". A resource type is required.");

            public ErrorDiagnostic NestedResourceNotFound(string resourceName, string identifierName, IEnumerable<string> nestedResourceNames) => new(
                TextSpan,
                "BCP159",
                $"The resource \"{resourceName}\" does not contain a nested resource named \"{identifierName}\". Known nested resources are: {ToQuotedString(nestedResourceNames)}.");

            public ErrorDiagnostic NestedResourceNotAllowedInLoop() => new(
                TextSpan,
                "BCP160",
                $"A nested resource cannot appear inside of a resource with a for-expression.");

            public ErrorDiagnostic ExpectedLoopItemIdentifierOrVariableBlockStart() => new(
                TextSpan,
                "BCP162",
                "Expected a loop item variable identifier or \"(\" at this location.");

            public ErrorDiagnostic ExpectedLoopIndexIdentifier() => new(
                TextSpan,
                "BCP163",
                "Expected a loop index variable identifier at this location.");

            public ErrorDiagnostic ScopeUnsupportedOnChildResource(string parentIdentifier) => new(
                TextSpan,
                "BCP164",
                $"The \"{LanguageConstants.ResourceScopePropertyName}\" property is unsupported for a resource with a parent resource. This resource has \"{parentIdentifier}\" declared as its parent.");

            public ErrorDiagnostic ScopeDisallowedForAncestorResource(string ancestorIdentifier) => new(
                TextSpan,
                "BCP165",
                $"Cannot deploy a resource with ancestor under a different scope. Resource \"{ancestorIdentifier}\" has the \"{LanguageConstants.ResourceScopePropertyName}\" property set.");

            public ErrorDiagnostic DuplicateDecorator(string decoratorName) => new(
                TextSpan,
                "BCP166",
                $"Duplicate \"{decoratorName}\" decorator.");

            public ErrorDiagnostic ExpectBodyStartOrIf() => new(
                TextSpan,
                "BCP167",
                "Expected the \"{\" character or the \"if\" keyword at this location.");

            public ErrorDiagnostic LengthMustNotBeNegative() => new(
                TextSpan,
                "BCP168",
                $"Length must not be a negative value.");

            public ErrorDiagnostic TopLevelChildResourceNameIncorrectQualifierCount(int expectedSlashCount) => new(
                TextSpan,
                "BCP169",
                $"Expected resource name to contain {expectedSlashCount} \"/\" character(s). The number of name segments must match the number of segments in the resource type.");

            public ErrorDiagnostic ChildResourceNameContainsQualifiers() => new(
                TextSpan,
                "BCP170",
                $"Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name.");

            public ErrorDiagnostic ResourceTypeIsNotValidParent(string resourceType, string parentResourceType) => new(
                TextSpan,
                "BCP171",
                $"Resource type \"{resourceType}\" is not a valid child resource of parent \"{parentResourceType}\".");

            public ErrorDiagnostic ParentResourceTypeHasErrors(string resourceName) => new(
                TextSpan,
                "BCP172",
                $"The resource type cannot be validated due to an error in parent resource \"{resourceName}\".");

            public ErrorDiagnostic CannotUsePropertyInExistingResource(string property) => new(
                TextSpan,
                "BCP173",
                $"The property \"{property}\" cannot be used in an existing resource declaration.");

            public Diagnostic ResourceTypeContainsProvidersSegment() => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP174",
                $"Type validation is not available for resource types declared containing a \"/providers/\" segment. Please instead use the \"scope\" property.",
                new Uri("https://aka.ms/BicepScopes"));

            public ErrorDiagnostic AnyTypeIsNotAllowed() => new(
                TextSpan,
                "BCP176",
                $"Values of the \"any\" type are not allowed here.");

            public ErrorDiagnostic RuntimeValueNotAllowedInIfConditionExpression(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP177",
                    $"This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public ErrorDiagnostic RuntimeValueNotAllowedInForExpression(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP178",
                    $"This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic ForExpressionContainsLoopInvariants(string itemVariableName, string? indexVariableName, IEnumerable<string> expectedVariantProperties) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP179",
                indexVariableName is null
                    ? $"The loop item variable \"{itemVariableName}\" must be referenced in at least one of the value expressions of the following properties: {ToQuotedString(expectedVariantProperties)}"
                    : $"The loop item variable \"{itemVariableName}\" or the index variable \"{indexVariableName}\" must be referenced in at least one of the value expressions of the following properties in the loop body: {ToQuotedString(expectedVariantProperties)}");

            public ErrorDiagnostic FunctionOnlyValidInModuleSecureParameterAssignment(string functionName) => new(
                TextSpan,
                "BCP180",
                $"Function \"{functionName}\" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator.");

            public ErrorDiagnostic RuntimeValueNotAllowedInRunTimeFunctionArguments(string functionName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP181",
                    $"This expression is being used in an argument of the function \"{functionName}\", which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public ErrorDiagnostic RuntimeValueNotAllowedInVariableForBody(string variableName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new ErrorDiagnostic(
                    TextSpan,
                    "BCP182",
                    $"This expression is being used in the for-body of the variable \"{variableName}\", which requires values that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public ErrorDiagnostic ModuleParametersPropertyRequiresObjectLiteral() => new(
                TextSpan,
                "BCP183",
                $"The value of the module \"{LanguageConstants.ModuleParamsPropertyName}\" property must be an object literal.");

            public ErrorDiagnostic FileExceedsMaximumSize(string filePath, long maxSize, string unit) => new(
               TextSpan,
               "BCP184",
               $"File '{filePath}' exceeded maximum size of {maxSize} {unit}.");

            public Diagnostic FileEncodingMismatch(string detectedEncoding) => new(
               TextSpan,
               DiagnosticLevel.Info,
               "BCP185",
               $"Encoding mismatch. File was loaded with '{detectedEncoding}' encoding.");

            public ErrorDiagnostic UnparseableJsonType() => new(
               TextSpan,
               "BCP186",
               $"Unable to parse literal JSON value. Please ensure that it is well-formed.");

            public Diagnostic FallbackPropertyUsed(string property) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP187",
                $"The property \"{property}\" does not exist in the resource definition, although it might still be valid.{TypeInaccuracyClause}", TypeInaccuracyLink);

            public ErrorDiagnostic ReferencedArmTemplateHasErrors() => new(
                TextSpan,
                "BCP188",
                $"The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template.");

            public ErrorDiagnostic UnknownModuleReferenceScheme(string badScheme, ImmutableArray<string> allowedSchemes)
            {
                string FormatSchemes() => ToQuotedString(allowedSchemes.Where(scheme => !string.Equals(scheme, ModuleReferenceSchemes.Local)));

                return new(
                    TextSpan,
                    "BCP189",
                    (allowedSchemes.Contains(ModuleReferenceSchemes.Local, StringComparer.Ordinal), allowedSchemes.Any(scheme => !string.Equals(scheme, ModuleReferenceSchemes.Local, StringComparison.Ordinal))) switch
                    {
                        (false, false) => "Module references are not supported in this context.",
                        (false, true) => $"The specified module reference scheme \"{badScheme}\" is not recognized. Specify a module reference using one of the following schemes: {FormatSchemes()}",
                        (true, false) => $"The specified module reference scheme \"{badScheme}\" is not recognized. Specify a path to a local module file.",
                        (true, true) => $"The specified module reference scheme \"{badScheme}\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: {FormatSchemes()}",
                    });
            }

            // TODO: This error is context sensitive:
            // - In CLI, it's permanent and only likely to occur with bicep build --no-restore.
            // - In VS code, it's transient until the background restore finishes.
            //
            // Should it be split into two separate errors instead?
            public ErrorDiagnostic ModuleRequiresRestore(string moduleRef) => new(
                TextSpan,
                "BCP190",
                $"The module with reference \"{moduleRef}\" has not been restored.");

            public ErrorDiagnostic ModuleRestoreFailed(string moduleRef) => new(
                TextSpan,
                "BCP191",
                $"Unable to restore the module with reference \"{moduleRef}\".");

            public ErrorDiagnostic ModuleRestoreFailedWithMessage(string moduleRef, string message) => new(
                TextSpan,
                "BCP192",
                $"Unable to restore the module with reference \"{moduleRef}\": {message}");

            public ErrorDiagnostic InvalidOciArtifactReference(string badRef) => new(
                TextSpan,
                "BCP193",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. Specify a reference in the format of \"{ModuleReferenceSchemes.Oci}:<artifact uri>:<tag>\".");

            public ErrorDiagnostic InvalidTemplateSpecReference(string invalidReference) => new(
                TextSpan,
                "BCP194",
                $"The specified template spec reference \"{invalidReference}\" is not valid. Specify a reference in the format of \"{ModuleReferenceSchemes.TemplateSpecs}:<resourceGroupName>/<templateSpecName>:<tag>\" or \"{ModuleReferenceSchemes.TemplateSpecs}:<subscriptionId>/<resourceGroupName>/<templateSpecName>:<tag>\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceInvalidPathSegment(string badRef, string badSegment) => new(
                TextSpan,
                "BCP195",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. The module path segment \"{badSegment}\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceMissingTag(string badRef) => new(
                TextSpan,
                "BCP196",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. The module tag is missing.");

            public ErrorDiagnostic InvalidOciArtifactReferenceTagTooLong(string badRef, string badTag, int maxLength) => new(
                TextSpan,
                "BCP197",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. The tag \"{badTag}\" exceeds the maximum length of {maxLength} characters.");

            public ErrorDiagnostic InvalidOciArtifactReferenceInvalidTag(string badRef, string badTag) => new(
                TextSpan,
                "BCP198",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. The tag \"{badTag}\" is not valid. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceRepositoryTooLong(string badRef, string badRepository, int maxLength) => new(
                TextSpan,
                "BCP199",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. Module path \"{badRepository}\" exceeds the maximum length of {maxLength} characters.");

            public ErrorDiagnostic InvalidOciArtifactReferenceRegistryTooLong(string badRef, string badRegistry, int maxLength) => new(
                TextSpan,
                "BCP200",
                $"The specified OCI artifact reference \"{badRef}\" is not valid. The registry \"{badRegistry}\" exceeds the maximum length of {maxLength} characters.");

            public ErrorDiagnostic ExpectedImportProviderName() => new(
                TextSpan,
                "BCP201",
                "Expected an import provider name at this location.");

            public ErrorDiagnostic ExpectedImportAliasName() => new(
                TextSpan,
                "BCP202",
                "Expected an import alias name at this location.");

            public ErrorDiagnostic ImportsAreDisabled() => new(
                TextSpan,
                "BCP203",
                "Import statements are currently not supported.");

            public ErrorDiagnostic UnrecognizedImportProvider(string identifier) => new(
                TextSpan,
                "BCP204",
                $"Imported namespace \"{identifier}\" is not recognized.");

            public ErrorDiagnostic ImportProviderDoesNotSupportConfiguration(string identifier) => new(
                TextSpan,
                "BCP205",
                $"Imported namespace \"{identifier}\" does not support configuration.");

            public ErrorDiagnostic ImportProviderRequiresConfiguration(string identifier) => new(
                TextSpan,
                "BCP206",
                $"Imported namespace \"{identifier}\" requires configuration, but none was provided.");

            public ErrorDiagnostic NamespaceMultipleDeclarations(string identifier) => new(
                TextSpan,
                "BCP207",
                $"Namespace \"{identifier}\" is imported multiple times. Remove the duplicates.");
        }

        public static DiagnosticBuilderInternal ForPosition(TextSpan span)
            => new(span);

        public static DiagnosticBuilderInternal ForPosition(IPositionable positionable)
            => new(positionable.Span);
    }
}
