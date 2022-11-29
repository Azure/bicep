// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
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
        public const string UseStringInterpolationInsteadClause = "Use string interpolation instead.";

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

            private static string ToQuotedStringWithCaseInsensitiveOrdering(IEnumerable<string> elements)
                => ToQuotedString(elements.OrderBy(s => s, StringComparer.OrdinalIgnoreCase));

            private static string BuildVariableDependencyChainClause(IEnumerable<string>? variableDependencyChain) => variableDependencyChain is not null
                ? $" You are referencing a variable which cannot be calculated at the start (\"{string.Join("\" -> \"", variableDependencyChain)}\")."
                : string.Empty;

            private static string BuildAccessiblePropertiesClause(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames) => accessedSymbolName is not null && accessiblePropertyNames is not null
                ? $" Properties of {accessedSymbolName} which can be calculated at the start include {ToQuotedString(accessiblePropertyNames.OrderBy(s => s))}."
                : string.Empty;

            private static string BuildInvalidOciArtifactReferenceClause(string? aliasName, string referenceValue) => aliasName is not null
                ? $"The OCI artifact reference \"{referenceValue}\" after resolving alias \"{aliasName}\" is not valid."
                : $"The specified OCI artifact reference \"{referenceValue}\" is not valid.";

            private static string BuildInvalidTemplateSpecReferenceClause(string? aliasName, string referenceValue) => aliasName is not null
                ? $"The Template Spec reference \"{referenceValue}\" after resolving alias \"{aliasName}\" is not valid."
                : $"The specified Template Spec reference \"{referenceValue}\" is not valid.";

            private static string BuildBicepConfigurationClause(string? configurationPath) => configurationPath is not null
                ? $"Bicep configuration \"{configurationPath}\""
                : $"built-in Bicep configuration";

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
                "This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration.");

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
                "Expected a valid 64-bit signed integer.");

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

            public ErrorDiagnostic IdentifierMultipleDeclarations(string identifier) => new(
                TextSpan,
                "BCP028",
                $"Identifier \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public ErrorDiagnostic InvalidResourceType() => new(
                TextSpan,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\".");

            public ErrorDiagnostic InvalidOutputType() => new(
                TextSpan,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public ErrorDiagnostic InvalidParameterType(IEnumerable<string> validTypes) => new(
                TextSpan,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {ToQuotedString(validTypes)}.");

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

            public Diagnostic MissingRequiredProperties(bool warnInsteadOfError, Symbol? sourceDeclaration, ObjectSyntax objectSyntax, ICollection<string> properties, string blockName, bool showTypeInaccuracy)
            {
                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" from source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                var newSyntax = objectSyntax.AddChildrenWithFormatting(
                    properties.Select(p => SyntaxFactory.CreateObjectProperty(p, SyntaxFactory.EmptySkippedTrivia))
                );

                var codeFix = new CodeFix("Add required properties", true, CodeFixKind.QuickFix, new CodeReplacement(objectSyntax.Span, newSyntax.ToTextPreserveFormatting()));

                return new FixableDiagnostic(
                    TextSpan,
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP035",
                    $"The specified \"{blockName}\" declaration is missing the following required properties{sourceDeclarationClause}: {ToQuotedString(properties)}.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}",
                    showTypeInaccuracy ? TypeInaccuracyLink : null,
                    DiagnosticStyling.Default,
                    codeFix);
            }

            public Diagnostic PropertyTypeMismatch(bool warnInsteadOfError, Symbol? sourceDeclaration, string property, TypeSymbol expectedType, TypeSymbol actualType, bool showTypeInaccuracy = false)
            {
                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" in source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return new(
                    TextSpan,
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP036",
                    $"The property \"{property}\" expected a value of type \"{expectedType}\" but the provided value{sourceDeclarationClause} is of type \"{actualType}\".{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}",
                    showTypeInaccuracy ? TypeInaccuracyLink : null);
            }

            public Diagnostic DisallowedProperty(bool warnInsteadOfError, Symbol? sourceDeclaration, string property, TypeSymbol type, ICollection<string> validUnspecifiedProperties, bool showTypeInaccuracy)
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
                    $"The property \"{property}\"{sourceDeclarationClause} is not allowed on objects of type \"{type}\".{permissiblePropertiesClause}{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}", showTypeInaccuracy ? TypeInaccuracyLink : null);
            }

            public Diagnostic DisallowedInterpolatedKeyProperty(bool warnInsteadOfError, Symbol? sourceDeclaration, TypeSymbol type, ICollection<string> validUnspecifiedProperties)
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

            public ErrorDiagnostic BinaryOperatorInvalidType(string operatorName, TypeSymbol type1, TypeSymbol type2, string? additionalInfo) => new(
                TextSpan,
                "BCP045",
                $"Cannot apply operator \"{operatorName}\" to operands of type \"{type1}\" and \"{type2}\".{(additionalInfo is null ? string.Empty : " " + additionalInfo)}");

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
                "Expected a resource type string. Specify a valid resource type of format \"<types>@<apiVersion>\".");

            public ErrorDiagnostic FunctionNotSupportedOperatorAvailable(string function, string @operator) => new(
                TextSpan,
                "BCP069",
                $"The function \"{function}\" is not supported. Use the \"{@operator}\" operator instead.");

            public ErrorDiagnostic ArgumentTypeMismatch(TypeSymbol argumentType, TypeSymbol parameterType) => new(
                TextSpan,
                "BCP070",
                $"Argument of type \"{argumentType}\" is not assignable to parameter of type \"{parameterType}\".");

            public ErrorDiagnostic ArgumentCountMismatch(int argumentCount, int minimumArgumentCount, int? maximumArgumentCount)
            {
                string expected;

                if (!maximumArgumentCount.HasValue)
                {
                    expected = $"at least {minimumArgumentCount} {(minimumArgumentCount == 1 ? "argument" : "arguments")}";
                }
                else if (minimumArgumentCount == maximumArgumentCount.Value)
                {
                    expected = $"{minimumArgumentCount} {(minimumArgumentCount == 1 ? "argument" : "arguments")}";
                }
                else
                {
                    expected = $"{minimumArgumentCount} to {maximumArgumentCount} arguments";
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

            public Diagnostic CannotAssignToReadOnlyProperty(bool warnInsteadOfError, string property, bool showTypeInaccuracy) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP073",
                $"The property \"{property}\" is read-only. Expressions cannot be assigned to read-only properties.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}", showTypeInaccuracy ? TypeInaccuracyLink : null);

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
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName)));

            public FixableDiagnostic UnknownPropertyWithSuggestion(bool warnInsteadOfError, TypeSymbol type, string badProperty, string suggestedProperty) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP083",
                $"The type \"{type}\" does not contain property \"{badProperty}\". Did you mean \"{suggestedProperty}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{badProperty}\" to \"{suggestedProperty}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedProperty)));

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
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{actualStringLiteral}\" to \"{suggestedStringLiteral}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedStringLiteral)));

            public FixableDiagnostic DisallowedPropertyWithSuggestion(bool warnInsteadOfError, string property, TypeSymbol type, string suggestedProperty) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP089",
                $"The property \"{property}\" is not allowed on objects of type \"{type}\". Did you mean \"{suggestedProperty}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{property}\" to \"{suggestedProperty}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedProperty)));

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

            public ErrorDiagnostic CyclicFile(IEnumerable<string> cycle) => new(
                TextSpan,
                "BCP095",
                $"The file is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

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
                "The function \"if\" is not supported. Use the \"?:\" (ternary conditional) operator instead, e.g. condition ? ValueIfTrue : ValueIfFalse");

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
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName)));

            public ErrorDiagnostic FunctionDoesNotExistOnObject(TypeSymbol type, string name) => new(
                TextSpan,
                "BCP109",
                $"The type \"{type}\" does not contain function \"{name}\".");

            public FixableErrorDiagnostic FunctionDoesNotExistOnObjectWithSuggestion(TypeSymbol type, string name, string suggestedName) => new(
                TextSpan,
                "BCP110",
                $"The type \"{type}\" does not contain function \"{name}\". Did you mean \"{suggestedName}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName)));

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
                $"A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope.");

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

            public ErrorDiagnostic InvalidAncestorResourceType() => new(
                TextSpan,
                "BCP157",
                $"The resource type cannot be determined due to an error in the containing resource.");

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

            public ErrorDiagnostic ScopeUnsupportedOnChildResource(string parentIdentifier) => new(
                TextSpan,
                "BCP164",
                $"A child resource's scope is computed based on the scope of its ancestor resource. This means that using the \"{LanguageConstants.ResourceScopePropertyName}\" property on a child resource is unsupported.");

            public ErrorDiagnostic ScopeDisallowedForAncestorResource(string ancestorIdentifier) => new(
                TextSpan,
                "BCP165",
                $"A resource's computed scope must match that of the Bicep file for it to be deployable. This resource's scope is computed from the \"{LanguageConstants.ResourceScopePropertyName}\" property value assigned to ancestor resource \"{ancestorIdentifier}\". You must use modules to deploy resources to a different scope.");

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
                    ? $"Unique resource or deployment name is required when looping. The loop item variable \"{itemVariableName}\" must be referenced in at least one of the value expressions of the following properties: {ToQuotedString(expectedVariantProperties)}"
                    : $"Unique resource or deployment name is required when looping. The loop item variable \"{itemVariableName}\" or the index variable \"{indexVariableName}\" must be referenced in at least one of the value expressions of the following properties in the loop body: {ToQuotedString(expectedVariantProperties)}");

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

            public ErrorDiagnostic InvalidOciArtifactReference(string? aliasName, string badRef) => new(
                TextSpan,
                "BCP193",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} Specify a reference in the format of \"{ModuleReferenceSchemes.Oci}:<artifact-uri>:<tag>\", or \"{ModuleReferenceSchemes.Oci}/<module-alias>:<module-name-or-path>:<tag>\".");

            public ErrorDiagnostic InvalidTemplateSpecReference(string? aliasName, string badRef) => new(
                TextSpan,
                "BCP194",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, badRef)} Specify a reference in the format of \"{ModuleReferenceSchemes.TemplateSpecs}:<subscription-ID>/<resource-group-name>/<template-spec-name>:<version>\", or \"{ModuleReferenceSchemes.TemplateSpecs}/<module-alias>:<template-spec-name>:<version>\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceInvalidPathSegment(string? aliasName, string badRef, string badSegment) => new(
                TextSpan,
                "BCP195",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The module path segment \"{badSegment}\" is not valid. Each module name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceMissingTagOrDigest(string? aliasName, string badRef) => new(
                TextSpan,
                "BCP196",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The module tag or digest is missing.");

            public ErrorDiagnostic InvalidOciArtifactReferenceTagTooLong(string? aliasName, string badRef, string badTag, int maxLength) => new(
                TextSpan,
                "BCP197",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The tag \"{badTag}\" exceeds the maximum length of {maxLength} characters.");

            public ErrorDiagnostic InvalidOciArtifactReferenceInvalidTag(string? aliasName, string badRef, string badTag) => new(
                TextSpan,
                "BCP198",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The tag \"{badTag}\" is not valid. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceRepositoryTooLong(string? aliasName, string badRef, string badRepository, int maxLength) => new(
                TextSpan,
                "BCP199",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} Module path \"{badRepository}\" exceeds the maximum length of {maxLength} characters.");

            public ErrorDiagnostic InvalidOciArtifactReferenceRegistryTooLong(string? aliasName, string badRef, string badRegistry, int maxLength) => new(
                TextSpan,
                "BCP200",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The registry \"{badRegistry}\" exceeds the maximum length of {maxLength} characters.");

            public ErrorDiagnostic ExpectedProviderSpecification() => new(
                TextSpan,
                "BCP201",
                "Expected a provider specification string. Specify a valid provider of format \"<providerName>@<providerVersion>\".");

            public ErrorDiagnostic ExpectedImportAliasName() => new(
                TextSpan,
                "BCP202",
                "Expected an import alias name at this location.");

            public ErrorDiagnostic ImportsAreDisabled() => new(
                TextSpan,
                "BCP203",
                $@"Using import statements requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.Extensibility)}"".");

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

            public ErrorDiagnostic UnknownResourceReferenceScheme(string badNamespace, IEnumerable<string> allowedNamespaces) => new(
                TextSpan,
                "BCP208",
                $"The specified namespace \"{badNamespace}\" is not recognized. Specify a resource reference using one of the following namespaces: {ToQuotedString(allowedNamespaces)}.");

            public ErrorDiagnostic FailedToFindResourceTypeInNamespace(string @namespace, string resourceType) => new(
                TextSpan,
                "BCP209",
                $"Failed to find resource type \"{resourceType}\" in namespace \"{@namespace}\".");

            public ErrorDiagnostic ParentResourceInDifferentNamespace(string childNamespace, string parentNamespace) => new(
                TextSpan,
                "BCP210",
                $"Resource type belonging to namespace \"{childNamespace}\" cannot have a parent resource type belonging to different namespace \"{parentNamespace}\".");

            public ErrorDiagnostic InvalidModuleAliasName(string aliasName) => new(
                TextSpan,
                "BCP211",
                $"The module alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");

            public ErrorDiagnostic TemplateSpecModuleAliasNameDoesNotExistInConfiguration(string aliasName, string? configurationPath) => new(
                TextSpan,
                "BCP212",
                $"The Template Spec module alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configurationPath)}.");

            public ErrorDiagnostic OciArtifactModuleAliasNameDoesNotExistInConfiguration(string aliasName, string? configurationPath) => new(
                TextSpan,
                "BCP213",
                $"The OCI artifact module alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configurationPath)}.");

            public ErrorDiagnostic InvalidTemplateSpecAliasSubscriptionNullOrUndefined(string aliasName, string? configurationPath) => new(
                TextSpan,
                "BCP214",
                $"The Template Spec module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configurationPath)} is in valid. The \"subscription\" property cannot be null or undefined.");

            public ErrorDiagnostic InvalidTemplateSpecAliasResourceGroupNullOrUndefined(string aliasName, string? configurationPath) => new(
                TextSpan,
                "BCP215",
                $"The Template Spec module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configurationPath)} is in valid. The \"resourceGroup\" property cannot be null or undefined.");

            public ErrorDiagnostic InvalidOciArtifactModuleAliasRegistryNullOrUndefined(string aliasName, string? configurationPath) => new(
                TextSpan,
                "BCP216",
                $"The OCI artifact module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configurationPath)} is invalid. The \"registry\" property cannot be null or undefined.");

            public ErrorDiagnostic InvalidTemplateSpecReferenceInvalidSubscirptionId(string? aliasName, string subscriptionId, string referenceValue) => new(
                TextSpan,
                "BCP217",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The subscription ID \"{subscriptionId}\" in is not a GUID.");

            public ErrorDiagnostic InvalidTemplateSpecReferenceResourceGroupNameTooLong(string? aliasName, string resourceGroupName, string referenceValue, int maximumLength) => new(
                TextSpan,
                "BCP218",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The resource group name \"{resourceGroupName}\" exceeds the maximum length of {maximumLength} characters.");

            public ErrorDiagnostic InvalidTemplateSpecReferenceInvalidResourceGroupName(string? aliasName, string resourceGroupName, string referenceValue) => new(
                TextSpan,
                "BCP219",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The resource group name \"{resourceGroupName}\" is invalid. Valid characters are alphanumeric, unicode charaters, \".\", \"_\", \"-\", \"(\", or \")\", but the resource group name cannot end with \".\".");

            public ErrorDiagnostic InvalidTemplateSpecReferenceTemplateSpecNameTooLong(string? aliasName, string templateSpecName, string referenceValue, int maximumLength) => new(
                TextSpan,
                "BCP220",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec name \"{templateSpecName}\" exceeds the maximum length of {maximumLength} characters.");

            public ErrorDiagnostic InvalidTemplateSpecReferenceInvalidTemplateSpecName(string? aliasName, string templateSpecName, string referenceValue) => new(
                TextSpan,
                "BCP221",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec name \"{templateSpecName}\" is invalid. Valid characters are alphanumeric, \".\", \"_\", \"-\", \"(\", or \")\", but the Template Spec name cannot end with \".\".");

            public ErrorDiagnostic InvalidTemplateSpecReferenceTemplateSpecVersionTooLong(string? aliasName, string templateSpecVersion, string referenceValue, int maximumLength) => new(
                TextSpan,
                "BCP222",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec version \"{templateSpecVersion}\" exceeds the maximum length of {maximumLength} characters.");

            public ErrorDiagnostic InvalidTemplateSpecReferenceInvalidTemplateSpecVersion(string? aliasName, string templateSpecVersion, string referenceValue) => new(
                TextSpan,
                "BCP223",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec version \"{templateSpecVersion}\" is invalid. Valid characters are alphanumeric, \".\", \"_\", \"-\", \"(\", or \")\", but the Template Spec name cannot end with \".\".");

            public ErrorDiagnostic InvalidOciArtifactReferenceInvalidDigest(string? aliasName, string badRef, string badDigest) => new(
                TextSpan,
                "BCP224",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The digest \"{badDigest}\" is not valid. The valid format is a string \"sha256:\" followed by exactly 64 lowercase hexadecimal digits.");

            public Diagnostic AmbiguousDiscriminatorPropertyValue(string propertyName) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP225",
                $"The discriminator property \"{propertyName}\" value cannot be determined at compilation time. Type checking for this object is disabled.");

            public ErrorDiagnostic MissingDiagnosticCodes() => new(
                TextSpan,
                "BCP226",
                "Expected at least one diagnostic code at this location. Valid format is \"#disable-next-line diagnosticCode1 diagnosticCode2 ...\""
            );

            public ErrorDiagnostic UnsupportedResourceTypeParameterOrOutputType(string resourceType) => new(
                TextSpan,
                "BCP227",
                $"The type \"{resourceType}\" cannot be used as a parameter or output type. Extensibility types are currently not supported as parameters or outputs.");

            public ErrorDiagnostic InvalidResourceScopeCannotBeResourceTypeParameter(string parameterName) => new(
                TextSpan,
                "BCP229",
                $"The parameter \"{parameterName}\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource.");

            public Diagnostic ModuleParamOrOutputResourceTypeUnavailable(ResourceTypeReference resourceTypeReference) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP230",
                $"The referenced module uses resource type \"{resourceTypeReference.FormatName()}\" which does not have types available.");

            public ErrorDiagnostic ParamOrOutputResourceTypeUnsupported() => new(
                TextSpan,
                "BCP231",
                $@"Using resource-typed parameters and outputs requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs)}"".");

            public ErrorDiagnostic ModuleDeleteFailed(string moduleRef) => new(
                TextSpan,
                "BCP232",
                $"Unable to delete the module with reference \"{moduleRef}\" from cache.");

            public ErrorDiagnostic ModuleDeleteFailedWithMessage(string moduleRef, string message) => new(
                TextSpan,
                "BCP233",
                $"Unable to delete the module with reference \"{moduleRef}\" from cache: {message}");

            public Diagnostic ArmFunctionLiteralTypeConversionFailedWithMessage(string literalValue, string armFunctionName, string message) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP234",
                $"The ARM function \"{armFunctionName}\" failed when invoked on the value [{literalValue}]: {message}");

            public ErrorDiagnostic NoJsonTokenOnPathOrPathInvalid() => new(
                TextSpan,
                "BCP235",
                $"Specified JSONPath does not exist in the given file or is invalid.");

            public ErrorDiagnostic ExpectedNewLineOrCommaSeparator() => new(
                TextSpan,
                "BCP236",
                "Expected a new line or comma character at this location.");

            public ErrorDiagnostic ExpectedCommaSeparator() => new(
                TextSpan,
                "BCP237",
                "Expected a comma character at this location.");

            public ErrorDiagnostic UnexpectedNewLineAfterCommaSeparator() => new(
                TextSpan,
                "BCP238",
                "Unexpected new line character after a comma.");

            public ErrorDiagnostic ReservedIdentifier(string name) => new(
                TextSpan,
                "BCP239",
                $"Identifier \"{name}\" is a reserved Bicep symbol name and cannot be used in this context.");

            public ErrorDiagnostic InvalidValueForParentProperty() => new(
                TextSpan,
                "BCP240",
                "The \"parent\" property only permits direct references to resources. Expressions are not supported.");

            public Diagnostic DeprecatedProvidersFunction(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP241",
                $"The \"{functionName}\" function is deprecated and will be removed in a future release of Bicep. Please add a comment to https://github.com/Azure/bicep/issues/2017 if you believe this will impact your workflow.",
                styling: DiagnosticStyling.ShowCodeDeprecated);

            public ErrorDiagnostic LambdaFunctionsOnlyValidInFunctionArguments() => new(
                TextSpan,
                "BCP242",
                $"Lambda functions may only be specified directly as function arguments.");

            public ErrorDiagnostic ParenthesesMustHaveExactlyOneItem() => new(
                TextSpan,
                "BCP243",
                "Parentheses must contain exactly one expression.");

            public ErrorDiagnostic LambdaExpectedArgCountMismatch(TypeSymbol lambdaType, int expectedArgCount, int actualArgCount) => new (
                TextSpan,
                "BCP244",
                $"Expected lambda expression of type \"{lambdaType}\" with {expectedArgCount} arguments but received {actualArgCount} arguments.");

            public Diagnostic ResourceTypeIsReadonly(ResourceTypeReference resourceTypeReference) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP245",
                $"Resource type \"{resourceTypeReference.FormatName()}\" can only be used with the 'existing' keyword.");

            public Diagnostic ResourceTypeIsReadonlyAtScope(ResourceTypeReference resourceTypeReference, ResourceScope writableScopes) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP246",
                $"Resource type \"{resourceTypeReference.FormatName()}\" can only be used with the 'existing' keyword at the requested scope."
                    + $" Permitted scopes for deployment: {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(writableScopes))}.");

            public ErrorDiagnostic LambdaVariablesInResourceOrModuleArrayAccessUnsupported(IEnumerable<string> variableNames) => new(
                TextSpan,
                "BCP247",
                $"Using lambda variables inside resource or module array access is not currently supported."
                    + $" Found the following lambda variable(s) being accessed: {ToQuotedString(variableNames)}.");

            public ErrorDiagnostic LambdaVariablesInInlineFunctionUnsupported(string functionName, IEnumerable<string> variableNames) => new(
                TextSpan,
                "BCP248",
                $"Using lambda variables inside the \"{functionName}\" function is not currently supported."
                    + $" Found the following lambda variable(s) being accessed: {ToQuotedString(variableNames)}.");

            public ErrorDiagnostic ExpectedLoopVariableBlockWith2Elements(int actualCount) => new(
                TextSpan,
                "BCP249",
                $"Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found {actualCount}.");

            public ErrorDiagnostic ParameterMultipleAssignments(string identifier) => new(
                TextSpan,
                "BCP250",
                $"Parameter \"{identifier}\" is assigned multiple times. Remove or rename the duplicates.");

            public ErrorDiagnostic ParameterTernaryOperationNotSupported() => new(
                TextSpan,
                "BCP251",
                $"Ternary operator is not allowed in Bicep parameter file.");

            public ErrorDiagnostic ParameterBinaryOperationNotSupported() => new(
                TextSpan,
                "BCP252",
                $"Binary operator is not allowed in Bicep parameter file.");

            public ErrorDiagnostic ParameterUnaryOperationNotSupported() => new(
                TextSpan,
                "BCP253",
                $"Unary operator is not allowed in Bicep parameter file.");

            public ErrorDiagnostic ParameterLambdaFunctionNotSupported() => new(
                TextSpan,
                "BCP254",
                $"Lambda function is not allowed in Bicep parameter file.");

            public ErrorDiagnostic ParameterFunctionCallNotSupported() => new(
                TextSpan,
                "BCP255",
                $"Function call is not allowed in Bicep parameter file.");

            public ErrorDiagnostic TemplatePathHasNotBeenSpecified() => new(
                TextSpan,
                "BCP256",
                "The using declaration is missing a bicep template file path reference.");

            public ErrorDiagnostic ExpectedFilePathString() => new(
                TextSpan,
                "BCP257",
                "Expected a Bicep file path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep'");

            public ErrorDiagnostic MissingParameterAssignment(IEnumerable<string> identifiers) => new(
                TextSpan,
                "BCP258",
                $"The following parameters are declared in the Bicep file but are missing an assignment in the params file: {ToQuotedString(identifiers)}.");

            public ErrorDiagnostic MissingParameterDeclaration(string? identifier) => new(
                TextSpan,
                "BCP259",
                $"The parameter \"{identifier}\" is assigned in the params file without being declared in the Bicep file.");

            public ErrorDiagnostic ParameterTypeMismatch(string? identifier, TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                "BCP260",
                $"The parameter \"{identifier}\" expects a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic UsingDeclarationNotSpecified() => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP261",
                "No using declaration is present in this parameters file. Parameter validation/completions will not be available");

            public ErrorDiagnostic MoreThanOneUsingDeclarationSpecified() => new(
                TextSpan,
                "BCP262",
                "More than one using declaration are present");

            public ErrorDiagnostic UsingDeclarationReferencesInvalidFile() => new(
                TextSpan,
                "BCP263",
                "The file specified in the using declaration path does not exist");

            public ErrorDiagnostic AmbiguousResourceTypeBetweenImports(string resourceTypeName, IEnumerable<string> namespaces) => new(
                TextSpan,
                "BCP264",
                $"Resource type \"{resourceTypeName}\" is declared in multiple imported namespaces ({ToQuotedStringWithCaseInsensitiveOrdering(namespaces)}), and must be fully-qualified.");

            public FixableErrorDiagnostic SymbolicNameShadowsAKnownFunction(string name, string knownFunctionNamespace, string knownFunctionName) => new(
                TextSpan,
                "BCP265",
                $"The name \"{name}\" is not a function. Did you mean \"{knownFunctionNamespace}.{knownFunctionName}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{knownFunctionNamespace}.{knownFunctionName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, $"{knownFunctionNamespace}.{knownFunctionName}")));

            public ErrorDiagnostic ExpectedMetadataIdentifier() => new(
                TextSpan,
                "BCP266",
                "Expected a metadata identifier at this location.");

            public ErrorDiagnostic ExpectedMetadataDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP267",
                "Expected an metadata declaration after the decorator.");

            public ErrorDiagnostic ReservedMetadataIdentifier(string name) => new(
                TextSpan,
                "BCP268",
                $"Invalid identifier: \"{name}\". Metadata identifiers starting with '_' are reserved. Please use a different identifier.");

            public ErrorDiagnostic CannotUseFunctionAsMetadataDecorator(string functionName) => new(
                TextSpan,
                "BCP269",
                $"Function \"{functionName}\" cannot be used as a metadata decorator.");

            public ErrorDiagnostic UnparsableBicepConfigFile(string configurationPath, string parsingErrorMessage) => new(
                TextSpan,
                "BCP271",
                $"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: \"{parsingErrorMessage}\".");

            public ErrorDiagnostic UnloadableBicepConfigFile(string configurationPath, string loadErrorMessage) => new(
                TextSpan,
                "BCP272",
                $"Could not load the Bicep configuration file \"{configurationPath}\": \"{loadErrorMessage}\".");

            public ErrorDiagnostic InvalidBicepConfigFile(string configurationPath, string parsingErrorMessage) => new(
                TextSpan,
                "BCP273",
                $"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\": \"{parsingErrorMessage}\".");

            public Diagnostic PotentialConfigDirectoryCouldNotBeScanned(string? directoryPath, string scanErrorMessage) => new(
                TextSpan,
                DiagnosticLevel.Info, // should this be a warning instead?
                "BCP274",
                $"Error scanning \"{directoryPath}\" for bicep configuration: \"{scanErrorMessage}\".");

            public ErrorDiagnostic FoundDirectoryInsteadOfFile(string directoryPath) => new(
                TextSpan,
                "BCP275",
                $"Unable to open file at path \"{directoryPath}\". Found a directory instead.");

            public ErrorDiagnostic UsingDeclarationMustReferenceBicepFile() => new(
                TextSpan,
                "BCP276",
                "A using declaration can only reference a Bicep file.");

            public ErrorDiagnostic ModuleDeclarationMustReferenceBicepModule() => new(
                TextSpan,
                "BCP277",
                "A module declaration can only reference a Bicep File, an ARM template, a registry reference or a template spec reference.");

            public ErrorDiagnostic CyclicParametersSelfReference() => new(
                TextSpan,
                "BCP278",
                "This parameters file references itself, which is not allowed.");

            public ErrorDiagnostic UnrecognizedTypeExpression() => new(
                TextSpan,
                "BCP279",
                $"Expected a type at this location. Please specify a valid type expression or one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public ErrorDiagnostic TypeDeclarationStatementsUnsupported() => new(
                TextSpan,
                "BCP280",
                $@"Using a type declaration statement requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedTypes)}"".");

            public ErrorDiagnostic TypedArrayDeclarationsUnsupported() => new(
                TextSpan,
                "BCP281",
                $@"Using a typed array type declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedTypes)}"".");

            public ErrorDiagnostic TypedObjectDeclarationsUnsupported() => new(
                TextSpan,
                "BCP282",
                $@"Using a strongly-typed object type declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedTypes)}"".");

            public ErrorDiagnostic TypeLiteralDeclarationsUnsupported() => new(
                TextSpan,
                "BCP283",
                $@"Using a literal value as a type requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedTypes)}"".");

            public ErrorDiagnostic TypeUnionDeclarationsUnsupported() => new(
                TextSpan,
                "BCP284",
                $@"Using a type union declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedTypes)}"".");

            public ErrorDiagnostic TypeExpressionLiteralConversionFailed() => new(
                TextSpan,
                "BCP285",
                "The type expression could not be reduced to a literal value.");

            public ErrorDiagnostic InvalidUnionTypeMember(string keystoneType) => new(
                TextSpan,
                "BCP286",
                $"This union member is invalid because it cannot be assigned to the '{keystoneType}' type.");

            public ErrorDiagnostic ValueSymbolUsedAsType(string symbolName) => new(
                TextSpan,
                "BCP287",
                // TODO: Add "Did you mean 'typeof({symbolName})'?" When support for typeof has been added.
                $"'{symbolName}' refers to a value but is being used as a type here.");

            public ErrorDiagnostic TypeSymbolUsedAsValue(string symbolName) => new(
                TextSpan,
                "BCP288",
                $"'{symbolName}' refers to a type but is being used as a value here.");

            public ErrorDiagnostic InvalidTypeDefinition() => new(
                TextSpan,
                "BCP289",
                $"The type definition is not valid.");

            public ErrorDiagnostic ExpectedParameterOrTypeDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP290",
                "Expected a parameter or type declaration after the decorator.");

            public ErrorDiagnostic ExpectedParameterOrOutputDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP291",
                "Expected a parameter or output declaration after the decorator.");

            public ErrorDiagnostic ExpectedParameterOutputOrTypeDeclarationAfterDecorator() => new(
                TextSpan,
                "BCP292",
                "Expected a parameter, output, or type declaration after the decorator.");

            public ErrorDiagnostic NonLiteralUnionMember() => new(
                TextSpan,
                "BCP293",
                "All members of a union type declaration must be literal values.");

            public ErrorDiagnostic InvalidTypeUnion() => new(
                TextSpan,
                "BCP294",
                "Type unions must be reduceable to a single ARM type (such as 'string', 'int', or 'bool').");

            public ErrorDiagnostic DecoratorNotPermittedOnLiteralType(string decoratorName) => new(
                TextSpan,
                "BCP295",
                $"The '{decoratorName}' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically.");

            public ErrorDiagnostic NonConstantTypeProperty() => new(
                TextSpan,
                "BCP296",
                "Property names on types must be compile-time constant values.");

            public ErrorDiagnostic CannotUseFunctionAsTypeDecorator(string functionName) => new(
                TextSpan,
                "BCP297",
                $"Function \"{functionName}\" cannot be used as a type decorator.");

            public ErrorDiagnostic CyclicTypeSelfReference() => new(
                TextSpan,
                "BCP298",
                "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled.");

            public ErrorDiagnostic CyclicType(IEnumerable<string> cycle) => new(
                TextSpan,
                "BCP299",
                $"This type definition includes itself as a required component via a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public ErrorDiagnostic ExpectedTypeLiteral() => new(
                TextSpan,
                "BCP300",
                $"Expected a type literal at this location. Please specify a concrete value or a reference to a literal type.");

            public ErrorDiagnostic ReservedTypeName(string reservedName) => new(
                TextSpan,
                "BCP301",
                $@"The type name ""{reservedName}"" is reserved and may not be attached to a user-defined type.");

            public ErrorDiagnostic SymbolicNameIsNotAType(string name, IEnumerable<string> validTypes) => new(
                TextSpan,
                "BCP302",
                $@"The name ""{name}"" is not a valid type. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public ErrorDiagnostic ProviderSpecificationInterpolationUnsupported() => new(
                TextSpan,
                "BCP303",
                "String interpolation is unsupported for specifying the provider.");

            public ErrorDiagnostic InvalidProviderSpecification() => new(
                TextSpan,
                "BCP304",
                "Invalid provider specifier string. Specify a valid provider of format \"<providerName>@<providerVersion>\".");

            public ErrorDiagnostic ExpectedWithOrAsKeywordOrNewLine() => new(
                TextSpan,
                "BCP305",
                $"Expected the \"with\" keyword, \"as\" keyword, or a new line character at this location.");

            public ErrorDiagnostic NamespaceSymbolUsedAsType(string name) => new(
                TextSpan,
                "BCP306",
                $@"The name ""{name}"" refers to a namespace, not to a type.");

            public ErrorDiagnostic NestedRuntimePropertyAccessNotSupported(string? resourceSymbol, IEnumerable<string> accessiblePropertyNames, IEnumerable<string> accessibleFunctionNames)
            {
                var accessiblePropertyNamesClause = accessiblePropertyNames.Any() ? @$" the accessible properties of ""{resourceSymbol}"" include {ToQuotedString(accessiblePropertyNames.OrderBy(x => x))}." : "";
                var accessibleFunctionNamesClause = accessibleFunctionNames.Any() ? @$" The accessible functions of ""{resourceSymbol}"" include {ToQuotedString(accessibleFunctionNames.OrderBy(x => x))}." : "";

                return new(
                    TextSpan,
                    "BCP307",
                    $"The expression cannot be evaluated, because the \"name\" property of the referenced existing resource contains a value that cannot be calculated at the start of the deployment. In this situation,{accessiblePropertyNamesClause}{accessibleFunctionNamesClause}");
            }

            public ErrorDiagnostic DecoratorMayNotTargetTypeAlias(string decoratorName) => new(
                TextSpan,
                "BCP308",
                $@"The decorator ""{decoratorName}"" may not be used on statements whose declared type is a reference to a user-defined type.");

            public ErrorDiagnostic ValueCannotBeFlattened(TypeSymbol flattenInputType, TypeSymbol incompatibleType) => new(
                TextSpan,
                "BCP309",
                $@"Values of type ""{flattenInputType.Name}"" cannot be flattened because ""{incompatibleType.Name}"" is not an array type.");

            public ErrorDiagnostic TypedTupleDeclarationsUnsupported() => new(
                TextSpan,
                "BCP310",
                $@"Using a strongly-typed tuple type declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedTypes)}"".");
        }

        public static DiagnosticBuilderInternal ForPosition(TextSpan span)
            => new(span);

        public static DiagnosticBuilderInternal ForPosition(IPositionable positionable)
            => new(positionable.Span);

        public static DiagnosticBuilderInternal ForDocumentStart()
            => new(TextSpan.TextDocumentStart);
    }
}
