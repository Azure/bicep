// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Diagnostics
{
    public static class DiagnosticBuilder
    {
        public const string UseStringInterpolationInsteadClause = "Use string interpolation instead.";

        public delegate Diagnostic DiagnosticBuilderDelegate(DiagnosticBuilderInternal builder);

        public class DiagnosticBuilderInternal
        {

            private const string TypeInaccuracyClause = " If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues.";

            public DiagnosticBuilderInternal(TextSpan textSpan)
            {
                TextSpan = textSpan;
            }

            public TextSpan TextSpan { get; }

            private Diagnostic CoreDiagnostic(DiagnosticLevel level, string code, string message) => new(
                TextSpan,
                level,
                DiagnosticSource.Compiler,
                code,
                message)
            { Uri = new($"https://aka.ms/bicep/core-diagnostics#{code}") };

            private Diagnostic CoreError(string code, string message) => CoreDiagnostic(
                DiagnosticLevel.Error,
                code,
                message);

            private Diagnostic CoreWarning(string code, string message) => CoreDiagnostic(
                DiagnosticLevel.Warning,
                code,
                message);

            private static string ToQuotedString(IEnumerable<string> elements)
                => elements.Any() ? $"\"{elements.ConcatString("\", \"")}\"" : "";

            private static string ToQuotedStringWithCaseInsensitiveOrdering(IEnumerable<string> elements)
                => ToQuotedString(elements.OrderBy(s => s, StringComparer.OrdinalIgnoreCase));

            private static string BuildVariableDependencyChainClause(IEnumerable<string>? variableDependencyChain) => variableDependencyChain is not null
                ? $" You are referencing a variable which cannot be calculated at the start (\"{string.Join("\" -> \"", variableDependencyChain)}\")."
                : string.Empty;

            private static string BuildNonDeployTimeConstantPropertyClause(string? accessedSymbolName, string? propertyName) =>
                accessedSymbolName is not null && propertyName is not null
                    ? $" The property \"{propertyName}\" of {accessedSymbolName} cannot be calculated at the start."
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

            private static string BuildBicepConfigurationClause(IOUri? configFileUri) => configFileUri is not null
                ? $"Bicep configuration \"{configFileUri}\""
                : $"built-in Bicep configuration";

            public Diagnostic UnrecognizedToken(string token) => CoreError(
                "BCP001",
                $"The following token is not recognized: \"{token}\".");

            public Diagnostic UnterminatedMultilineComment() => CoreError(
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public Diagnostic UnterminatedString() => CoreError(
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public Diagnostic UnterminatedStringWithNewLine() => CoreError(
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public Diagnostic UnterminatedStringEscapeSequenceAtEof() => CoreError(
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public Diagnostic UnterminatedStringEscapeSequenceUnrecognized(IEnumerable<string> escapeSequences) => CoreError(
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following escape sequences are allowed: {ToQuotedString(escapeSequences)}.");

            public Diagnostic UnrecognizedDeclaration() => CoreError(
                "BCP007",
                "This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration.");

            public Diagnostic ExpectedParameterContinuation() => CoreError(
                "BCP008",
                "Expected the \"=\" token, or a newline at this location.");

            public Diagnostic UnrecognizedExpression() => CoreError(
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public Diagnostic InvalidInteger() => CoreError(
                "BCP010",
                "Expected a valid 64-bit signed integer.");

            public Diagnostic InvalidType() => CoreError(
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public Diagnostic ExpectedKeyword(string keyword) => CoreError(
                "BCP012",
                $"Expected the \"{keyword}\" keyword at this location.");

            public Diagnostic ExpectedParameterIdentifier() => CoreError(
                "BCP013",
                "Expected a parameter identifier at this location.");

            public Diagnostic ExpectedVariableIdentifier() => CoreError(
                "BCP015",
                "Expected a variable identifier at this location.");

            public Diagnostic ExpectedOutputIdentifier() => CoreError(
                "BCP016",
                "Expected an output identifier at this location.");

            public Diagnostic ExpectedResourceIdentifier() => CoreError(
                "BCP017",
                "Expected a resource identifier at this location.");

            public Diagnostic ExpectedCharacter(string character) => CoreError(
                "BCP018",
                $"Expected the \"{character}\" character at this location.");

            public Diagnostic ExpectedNewLine() => CoreError(
                "BCP019",
                "Expected a new line character at this location.");

            public Diagnostic ExpectedFunctionOrPropertyName() => CoreError(
                "BCP020",
                "Expected a function or property name at this location.");

            public Diagnostic ExpectedNumericLiteral() => CoreError(
                "BCP021",
                "Expected a numeric literal at this location.");

            public Diagnostic ExpectedPropertyName() => CoreError(
                "BCP022",
                "Expected a property name at this location.");

            public Diagnostic ExpectedVariableOrFunctionName() => CoreError(
                "BCP023",
                "Expected a variable or function name at this location.");

            public Diagnostic IdentifierNameExceedsLimit() => CoreError(
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public Diagnostic PropertyMultipleDeclarations(string property) => CoreError(
                "BCP025",
                $"The property \"{property}\" is declared multiple times in this object. Remove or rename the duplicate properties.");

            public Diagnostic OutputTypeMismatch(TypeSymbol expectedType, TypeSymbol actualType) => CoreError(
                "BCP026",
                $"The output expects a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic IdentifierMultipleDeclarations(string identifier) => CoreError(
                "BCP028",
                $"Identifier \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public Diagnostic InvalidResourceType() => CoreError(
                "BCP029",
                "The resource type is not valid. Specify a valid resource type of format \"<type-name>@<apiVersion>\".");

            public Diagnostic InvalidOutputType(IEnumerable<string> validTypes) => CoreError(
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic InvalidParameterType(IEnumerable<string> validTypes) => CoreError(
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic CompileTimeConstantRequired() => CoreError(
                "BCP032",
                "The value must be a compile-time constant.");

            public Diagnostic ExpectedValueTypeMismatch(bool warnInsteadOfError, TypeSymbol expectedType, TypeSymbol actualType) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP033",
                $"Expected a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic ArrayTypeMismatch(bool warnInsteadOfError, TypeSymbol expectedType, TypeSymbol actualType) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP034",
                $"The enclosing array expected an item of type \"{expectedType}\", but the provided item was of type \"{actualType}\".");

            public Diagnostic MissingRequiredProperties(bool warnInsteadOfError, Symbol? sourceDeclaration, ObjectSyntax? objectSyntax, ICollection<string> properties, string blockName, bool showTypeInaccuracy, IDiagnosticLookup parsingErrorLookup)
            {
                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" from source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                if (objectSyntax is null ||
                    SyntaxModifier.TryAddProperties(
                        objectSyntax,
                        properties.Select(p => SyntaxFactory.CreateObjectProperty(p, SyntaxFactory.EmptySkippedTrivia)),
                        parsingErrorLookup) is not { } newSyntax)
                {
                    // We're unable to come up with an automatic code fix - most likely because there are unhandled parse errors
                    return CoreDiagnostic(
                        warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                        "BCP035",
                        $"The specified \"{blockName}\" declaration is missing the following required properties{sourceDeclarationClause}: {ToQuotedString(properties)}.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}");
                }

                var codeFix = new CodeFix("Add required properties", true, CodeFixKind.QuickFix, new CodeReplacement(objectSyntax.Span, newSyntax.ToString()));

                return CoreDiagnostic(
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP035",
                    $"The specified \"{blockName}\" declaration is missing the following required properties{sourceDeclarationClause}: {ToQuotedString(properties)}.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}")
                    with
                { Fixes = [codeFix] };
            }

            public Diagnostic PropertyTypeMismatch(bool warnInsteadOfError, Symbol? sourceDeclaration, string property, TypeSymbol expectedType, TypeSymbol actualType, bool showTypeInaccuracy = false)
            {
                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" in source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return CoreDiagnostic(
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP036",
                    $"The property \"{property}\" expected a value of type \"{expectedType}\" but the provided value{sourceDeclarationClause} is of type \"{actualType}\".{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}");
            }

            public Diagnostic DisallowedProperty(bool warnInsteadOfError, Symbol? sourceDeclaration, string property, TypeSymbol type, ICollection<string> validUnspecifiedProperties, bool showTypeInaccuracy)
            {
                var permissiblePropertiesClause = validUnspecifiedProperties.Any()
                    ? $" Permissible properties include {ToQuotedString(validUnspecifiedProperties)}."
                    : $" No other properties are allowed.";

                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" from source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return CoreDiagnostic(
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP037",
                    $"The property \"{property}\"{sourceDeclarationClause} is not allowed on objects of type \"{type}\".{permissiblePropertiesClause}{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}");
            }

            public Diagnostic DisallowedInterpolatedKeyProperty(bool warnInsteadOfError, Symbol? sourceDeclaration, TypeSymbol type, ICollection<string> validUnspecifiedProperties)
            {
                var permissiblePropertiesClause = validUnspecifiedProperties.Any()
                    ? $" Permissible properties include {ToQuotedString(validUnspecifiedProperties)}."
                    : $" No other properties are allowed.";

                var sourceDeclarationClause = sourceDeclaration is not null
                    ? $" in source declaration \"{sourceDeclaration.Name}\""
                    : string.Empty;

                return CoreDiagnostic(
                    warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP040",
                    $"String interpolation is not supported for keys on objects of type \"{type}\"{sourceDeclarationClause}.{permissiblePropertiesClause}");
            }

            public Diagnostic VariableTypeAssignmentDisallowed(TypeSymbol valueType) => CoreError(
                "BCP041",
                $"Values of type \"{valueType}\" cannot be assigned to a variable.");

            public Diagnostic InvalidExpression() => CoreError(
                "BCP043",
                "This is not a valid expression.");

            public Diagnostic UnaryOperatorInvalidType(string operatorName, TypeSymbol type) => CoreError(
                "BCP044",
                $"Cannot apply operator \"{operatorName}\" to operand of type \"{type}\".");

            public Diagnostic BinaryOperatorInvalidType(string operatorName, TypeSymbol type1, TypeSymbol type2, string? additionalInfo) => CoreError(
                "BCP045",
                $"Cannot apply operator \"{operatorName}\" to operands of type \"{type1}\" and \"{type2}\".{(additionalInfo is null ? string.Empty : " " + additionalInfo)}");

            public Diagnostic ValueTypeMismatch(TypeSymbol type) => CoreError(
                "BCP046",
                $"Expected a value of type \"{type}\".");

            public Diagnostic ResourceTypeInterpolationUnsupported() => CoreError(
                "BCP047",
                "String interpolation is unsupported for specifying the resource type.");

            public Diagnostic CannotResolveFunctionOverload(IList<string> overloadSignatures, TypeSymbol argumentType, IList<TypeSymbol> parameterTypes)
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

                return CoreError(
                    "BCP048",
                    message);
            }

            public Diagnostic StringOrIntegerIndexerRequired(TypeSymbol wrongType) => CoreError(
                "BCP049",
                $"The array index must be of type \"{LanguageConstants.String}\" or \"{LanguageConstants.Int}\" but the provided index was of type \"{wrongType}\".");

            public Diagnostic FilePathIsEmpty() => CoreError(
                "BCP050",
                "The specified path is empty.");

            public Diagnostic FilePathIsAbsolute() => CoreError(
                "BCP051",
                "The specified path seems to reference an absolute path. Files must be referenced using relative paths.");

            public Diagnostic UnknownProperty(bool warnInsteadOfError, TypeSymbol type, string badProperty) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP052",
                $"The type \"{type}\" does not contain property \"{badProperty}\".");

            public Diagnostic UnknownPropertyWithAvailableProperties(bool warnInsteadOfError, TypeSymbol type, string badProperty, IEnumerable<string> availableProperties) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP053",
                $"The type \"{type}\" does not contain property \"{badProperty}\". Available properties include {ToQuotedString(availableProperties)}.");

            public Diagnostic NoPropertiesAllowed(TypeSymbol type) => CoreError(
                "BCP054",
                $"The type \"{type}\" does not contain any properties.");

            public Diagnostic ObjectRequiredForPropertyAccess(TypeSymbol wrongType) => CoreError(
                "BCP055",
                $"Cannot access properties of type \"{wrongType}\". An \"{LanguageConstants.Object}\" type is required.");

            public Diagnostic AmbiguousSymbolReference(string name, IEnumerable<string> namespaces) => CoreError(
                "BCP056",
                $"The reference to name \"{name}\" is ambiguous because it exists in namespaces {ToQuotedString(namespaces)}. The reference must be fully-qualified.");

            public Diagnostic SymbolicNameDoesNotExist(string name) => CoreError(
                "BCP057",
                $"The name \"{name}\" does not exist in the current context.");

            public Diagnostic SymbolicNameIsNotAFunction(string name) => CoreError(
                "BCP059",
                $"The name \"{name}\" is not a function.");

            public Diagnostic VariablesFunctionNotSupported() => CoreError(
                "BCP060",
                $"The \"variables\" function is not supported. Directly reference variables by their symbolic names.");

            public Diagnostic ParametersFunctionNotSupported() => CoreError(
                "BCP061",
                $"The \"parameters\" function is not supported. Directly reference parameters by their symbolic names.");

            public Diagnostic ReferencedSymbolHasErrors(string name) => CoreError(
                "BCP062",
                $"The referenced declaration with name \"{name}\" is not valid.");

            public Diagnostic SymbolicNameIsNotAVariableOrParameter(string name) => CoreError(
                "BCP063",
                $"The name \"{name}\" is not a parameter, variable, resource or module.");

            public Diagnostic UnexpectedTokensInInterpolation() => CoreError(
                "BCP064",
                "Found unexpected tokens in interpolated expression.");

            public Diagnostic FunctionOnlyValidInParameterDefaults(string functionName) => CoreError(
                "BCP065",
                $"Function \"{functionName}\" is not valid at this location. It can only be used as a parameter default value.");

            public Diagnostic FunctionOnlyValidInResourceBody(string functionName) => CoreError(
                "BCP066",
                $"Function \"{functionName}\" is not valid at this location. It can only be used in resource declarations.");

            public Diagnostic ObjectRequiredForMethodAccess(TypeSymbol wrongType) => CoreError(
                "BCP067",
                $"Cannot call functions on type \"{wrongType}\". An \"{LanguageConstants.Object}\" type is required.");

            public Diagnostic ExpectedResourceTypeString() => CoreError(
                "BCP068",
                "Expected a resource type string. Specify a valid resource type of format \"<type-name>@<apiVersion>\".");

            public Diagnostic FunctionNotSupportedOperatorAvailable(string function, string @operator) => CoreError(
                "BCP069",
                $"The function \"{function}\" is not supported. Use the \"{@operator}\" operator instead.");

            public Diagnostic ArgumentTypeMismatch(TypeSymbol argumentType, TypeSymbol parameterType) => CoreError(
                "BCP070",
                $"Argument of type \"{argumentType}\" is not assignable to parameter of type \"{parameterType}\".");

            public Diagnostic ArgumentCountMismatch(int argumentCount, int minimumArgumentCount, int? maximumArgumentCount)
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

                return CoreError(
                    "BCP071",
                    $"Expected {expected}, but got {argumentCount}.");
            }

            public Diagnostic CannotReferenceSymbolInParamDefaultValue() => CoreError(
                "BCP072",
                "This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values.");

            public Diagnostic CannotAssignToReadOnlyProperty(bool warnInsteadOfError, string property, bool showTypeInaccuracy) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP073",
                $"The property \"{property}\" is read-only. Expressions cannot be assigned to read-only properties.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}");

            public Diagnostic ArraysRequireIntegerIndex(TypeSymbol wrongType) => CoreError(
                "BCP074",
                $"Indexing over arrays requires an index of type \"{LanguageConstants.Int}\" but the provided index was of type \"{wrongType}\".");

            public Diagnostic ObjectsRequireStringIndex(TypeSymbol wrongType) => CoreError(
                "BCP075",
                $"Indexing over objects requires an index of type \"{LanguageConstants.String}\" but the provided index was of type \"{wrongType}\".");

            public Diagnostic IndexerRequiresObjectOrArray(TypeSymbol wrongType) => CoreError(
                "BCP076",
                $"Cannot index over expression of type \"{wrongType}\". Arrays or objects are required.");

            public Diagnostic WriteOnlyProperty(bool warnInsteadOfError, TypeSymbol type, string badProperty) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP077",
                $"The property \"{badProperty}\" on type \"{type}\" is write-only. Write-only properties cannot be accessed.");

            public Diagnostic MissingRequiredProperty(bool warnInsteadOfError, string propertyName, TypeSymbol expectedType) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP078",
                $"The property \"{propertyName}\" requires a value of type \"{expectedType}\", but none was supplied.");

            public Diagnostic CyclicExpressionSelfReference() => CoreError(
                "BCP079",
                "This expression is referencing its own declaration, which is not allowed.");

            public Diagnostic CyclicExpression(IEnumerable<string> cycle) => CoreError(
                "BCP080",
                $"The expression is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ResourceTypesUnavailable(ResourceTypeReference resourceTypeReference) => CoreWarning(
                "BCP081",
                $"Resource type \"{resourceTypeReference.FormatName()}\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.");

            public Diagnostic SymbolicNameDoesNotExistWithSuggestion(string name, string suggestedName) => CoreError(
                "BCP082",
                $"The name \"{name}\" does not exist in the current context. Did you mean \"{suggestedName}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName))
                ]
            };

            public Diagnostic UnknownPropertyWithSuggestion(bool warnInsteadOfError, TypeSymbol type, string badProperty, string suggestedProperty) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP083",
                $"The type \"{type}\" does not contain property \"{badProperty}\". Did you mean \"{suggestedProperty}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{badProperty}\" to \"{suggestedProperty}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedProperty))
                ]
            };

            public Diagnostic SymbolicNameCannotUseReservedNamespaceName(string name, IEnumerable<string> namespaces) => CoreError(
                "BCP084",
                $"The symbolic name \"{name}\" is reserved. Please use a different symbolic name. Reserved namespaces are {ToQuotedString(namespaces.OrderBy(ns => ns))}.");

            public Diagnostic FilePathContainsForbiddenCharacters(IEnumerable<char> forbiddenChars) => CoreError(
                "BCP085",
                $"The specified file path contains one ore more invalid path characters. The following are not permitted: {ToQuotedString(forbiddenChars.OrderBy(x => x).Select(x => x.ToString()))}.");

            public Diagnostic FilePathHasForbiddenTerminator(IEnumerable<char> forbiddenPathTerminatorChars) => CoreError(
                "BCP086",
                $"The specified file path ends with an invalid character. The following are not permitted: {ToQuotedString(forbiddenPathTerminatorChars.OrderBy(x => x).Select(x => x.ToString()))}.");

            public Diagnostic ComplexLiteralsNotAllowed() => CoreError(
                "BCP087",
                "Array and object literals are not allowed here.");

            public Diagnostic PropertyStringLiteralMismatchWithSuggestion(bool warnInsteadOfError, string property, TypeSymbol expectedType, string actualStringLiteral, string suggestedStringLiteral) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP088",
                $"The property \"{property}\" expected a value of type \"{expectedType}\" but the provided value is of type \"{actualStringLiteral}\". Did you mean \"{suggestedStringLiteral}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{actualStringLiteral}\" to \"{suggestedStringLiteral}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedStringLiteral))
                ]
            };

            public Diagnostic DisallowedPropertyWithSuggestion(bool warnInsteadOfError, string property, TypeSymbol type, string suggestedProperty) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP089",
                $"The property \"{property}\" is not allowed on objects of type \"{type}\". Did you mean \"{suggestedProperty}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{property}\" to \"{suggestedProperty}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedProperty))
                ]
            };

            public Diagnostic ModulePathHasNotBeenSpecified() => CoreError(
                "BCP090",
                "This module declaration is missing a file path reference.");

            public Diagnostic ErrorOccurredReadingFile(string failureMessage) => CoreError(
                "BCP091",
                $"An error occurred reading file. {failureMessage}");

            public Diagnostic FilePathInterpolationUnsupported() => CoreError(
                "BCP092",
                "String interpolation is not supported in file paths.");

            public Diagnostic FilePathCouldNotBeResolved(string filePath, string baseUri) => CoreError(
                "BCP093",
                $"File path \"{filePath}\" could not be resolved relative to \"{baseUri}\".");

            public Diagnostic CyclicModuleSelfReference() => CoreError(
                "BCP094",
                "This module references itself, which is not allowed.");

            public Diagnostic CyclicFile(IEnumerable<string> cycle) => CoreError(
                "BCP095",
                $"The file is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ExpectedModuleIdentifier() => CoreError(
                "BCP096",
                "Expected a module identifier at this location.");

            public Diagnostic ExpectedModulePathString() => CoreError(
                "BCP097",
                "Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep'");

            public Diagnostic FilePathContainsBackSlash() => CoreError(
                "BCP098",
                "The specified file path contains a \"\\\" character. Use \"/\" instead as the directory separator character.");

            public Diagnostic AllowedMustContainItems() => CoreError(
                "BCP099",
                $"The \"{LanguageConstants.ParameterAllowedPropertyName}\" array must contain one or more items.");

            public Diagnostic IfFunctionNotSupported() => CoreError(
                "BCP100",
                "The function \"if\" is not supported. Use the \"?:\" (ternary conditional) operator instead, e.g. condition ? ValueIfTrue : ValueIfFalse");

            public Diagnostic CreateArrayFunctionNotSupported() => CoreError(
                "BCP101",
                "The \"createArray\" function is not supported. Construct an array literal using [].");

            public Diagnostic CreateObjectFunctionNotSupported() => CoreError(
                "BCP102",
                "The \"createObject\" function is not supported. Construct an object literal using {}.");

            public Diagnostic DoubleQuoteToken(string token) => CoreError(
                "BCP103",
                $"The following token is not recognized: \"{token}\". Strings are defined using single quotes in bicep.");

            public Diagnostic ReferencedModuleHasErrors() => CoreError(
                "BCP104",
                $"The referenced module has errors.");

            public Diagnostic UnableToLoadNonFileUri(Uri fileUri) => CoreError(
                "BCP105",
                $"Unable to load file from URI \"{fileUri}\".");

            public Diagnostic UnexpectedCommaSeparator() => CoreError(
                "BCP106",
                "Expected a new line character at this location. Commas are not used as separator delimiters.");

            public Diagnostic FunctionDoesNotExistInNamespace(Symbol namespaceType, string name) => CoreError(
                "BCP107",
                $"The function \"{name}\" does not exist in namespace \"{namespaceType.Name}\".");

            public Diagnostic FunctionDoesNotExistInNamespaceWithSuggestion(Symbol namespaceType, string name, string suggestedName) => CoreError(
                "BCP108",
                $"The function \"{name}\" does not exist in namespace \"{namespaceType.Name}\". Did you mean \"{suggestedName}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName))
                ]
            };

            public Diagnostic FunctionDoesNotExistOnObject(TypeSymbol type, string name) => CoreError(
                "BCP109",
                $"The type \"{type}\" does not contain function \"{name}\".");

            public Diagnostic FunctionDoesNotExistOnObjectWithSuggestion(TypeSymbol type, string name, string suggestedName) => CoreError(
                "BCP110",
                $"The type \"{type}\" does not contain function \"{name}\". Did you mean \"{suggestedName}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName))
                ]
            };

            public Diagnostic FilePathContainsControlChars() => CoreError(
                "BCP111",
                $"The specified file path contains invalid control code characters.");

            public Diagnostic TargetScopeMultipleDeclarations() => CoreError(
                "BCP112",
                $"The \"{LanguageConstants.TargetScopeKeyword}\" cannot be declared multiple times in one file.");

            public Diagnostic InvalidModuleScopeForTenantScope() => CoreError(
                "BCP113",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeTenant}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include tenant: tenant(), named management group: managementGroup(<name>), named subscription: subscription(<subId>), or named resource group in a named subscription: resourceGroup(<subId>, <name>).");

            public Diagnostic InvalidModuleScopeForManagementScope() => CoreError(
                "BCP114",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeManagementGroup}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include current management group: managementGroup(), named management group: managementGroup(<name>), named subscription: subscription(<subId>), tenant: tenant(), or named resource group in a named subscription: resourceGroup(<subId>, <name>).");

            public Diagnostic InvalidModuleScopeForSubscriptionScope() => CoreError(
                "BCP115",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeSubscription}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include current subscription: subscription(), named subscription: subscription(<subId>), named resource group in same subscription: resourceGroup(<name>), named resource group in different subscription: resourceGroup(<subId>, <name>), or tenant: tenant().");

            public Diagnostic InvalidModuleScopeForResourceGroup() => CoreError(
                "BCP116",
                $"Unsupported scope for module deployment in a \"{LanguageConstants.TargetScopeTypeResourceGroup}\" target scope. Omit this property to inherit the current scope, or specify a valid scope. " +
                $"Permissible scopes include current resource group: resourceGroup(), named resource group in same subscription: resourceGroup(<name>), named resource group in a different subscription: resourceGroup(<subId>, <name>), current subscription: subscription(), named subscription: subscription(<subId>) or tenant: tenant().");

            public Diagnostic EmptyIndexerNotAllowed() => CoreError(
                "BCP117",
                "An empty indexer is not allowed. Specify a valid expression."
            );

            public Diagnostic ExpectBodyStartOrIfOrLoopStart() => CoreError(
                "BCP118",
                "Expected the \"{\" character, the \"[\" character, or the \"if\" keyword at this location.");

            public Diagnostic InvalidExtensionResourceScope() => CoreError(
                "BCP119",
                $"Unsupported scope for extension resource deployment. Expected a resource reference.");

            public Diagnostic RuntimeValueNotAllowedInProperty(string propertyName, string? objectTypeName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP120",
                    $"This expression is being used in an assignment to the \"{propertyName}\" property of the \"{objectTypeName}\" type, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic ResourceMultipleDeclarations(IEnumerable<string> resourceNames) => CoreError(
                "BCP121",
                $"Resources: {ToQuotedString(resourceNames)} are defined with this same name in a file. Rename them or split into different modules.");

            public Diagnostic ModuleMultipleDeclarations(IEnumerable<string> moduleNames) => CoreError(
                "BCP122",
                $"Modules: {ToQuotedString(moduleNames)} are defined with this same name and this same scope in a file. Rename them or split into different modules.");

            public Diagnostic ExpectedNamespaceOrDecoratorName() => CoreError(
                "BCP123",
                "Expected a namespace or decorator name at this location.");

            public Diagnostic CannotAttachDecoratorToTarget(string decoratorName, TypeSymbol attachableType, TypeSymbol targetType) => CoreError(
                "BCP124",
                $"The decorator \"{decoratorName}\" can only be attached to targets of type \"{attachableType}\", but the target has type \"{targetType}\".");

            public Diagnostic CannotUseFunctionAsParameterDecorator(string functionName) => CoreError(
                "BCP125",
                $"Function \"{functionName}\" cannot be used as a parameter decorator.");

            public Diagnostic CannotUseFunctionAsVariableDecorator(string functionName) => CoreError(
                "BCP126",
                $"Function \"{functionName}\" cannot be used as a variable decorator.");

            public Diagnostic CannotUseFunctionAsResourceDecorator(string functionName) => CoreError(
                "BCP127",
                $"Function \"{functionName}\" cannot be used as a resource decorator.");

            public Diagnostic CannotUseFunctionAsModuleDecorator(string functionName) => CoreError(
                "BCP128",
                $"Function \"{functionName}\" cannot be used as a module decorator.");

            public Diagnostic CannotUseFunctionAsOutputDecorator(string functionName) => CoreError(
                "BCP129",
                $"Function \"{functionName}\" cannot be used as an output decorator.");

            public Diagnostic DecoratorsNotAllowed() => CoreError(
                "BCP130",
                "Decorators are not allowed here.");

            public Diagnostic ExpectedDeclarationAfterDecorator() => CoreError(
                "BCP132",
                "Expected a declaration after the decorator.");

            public Diagnostic InvalidUnicodeEscape() => CoreError(
                "BCP133",
                "The unicode escape sequence is not valid. Valid unicode escape sequences range from \\u{0} to \\u{10FFFF}.");

            public Diagnostic UnsupportedModuleScope(ResourceScope suppliedScope, ResourceScope supportedScopes) => CoreError(
                "BCP134",
                $"Scope {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(suppliedScope))} is not valid for this module. Permitted scopes: {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(supportedScopes))}.");

            public Diagnostic UnsupportedResourceScope(ResourceScope suppliedScope, ResourceScope supportedScopes) => CoreError(
                "BCP135",
                $"Scope {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(suppliedScope))} is not valid for this resource type. Permitted scopes: {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(supportedScopes))}.");

            public Diagnostic ExpectedLoopVariableIdentifier() => CoreError(
                "BCP136",
                "Expected a loop item variable identifier at this location.");

            public Diagnostic LoopArrayExpressionTypeMismatch(TypeSymbol actualType) => CoreError(
                "BCP137",
                $"Loop expected an expression of type \"{LanguageConstants.Array}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic ForExpressionsNotSupportedHere() => CoreError(
                "BCP138",
                "For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties.");

            public Diagnostic InvalidCrossResourceScope() => CoreError(
                "BCP139",
                $"A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope.");

            public Diagnostic UnterminatedMultilineString() => CoreError(
                "BCP140",
                $"The multi-line string at this location is not terminated. Terminate it with \"'''\".");

            public Diagnostic ExpressionNotCallable() => CoreError(
                "BCP141",
                "The expression cannot be used as a decorator as it is not callable.");

            public Diagnostic TooManyPropertyForExpressions() => CoreError(
                "BCP142",
                "Property value for-expressions cannot be nested.");

            public Diagnostic ExpressionedPropertiesNotAllowedWithLoops() => CoreError(
                "BCP143",
                "For-expressions cannot be used with properties whose names are also expressions.");

            public Diagnostic DirectAccessToCollectionNotSupported(IEnumerable<string>? accessChain = null)
            {
                var accessChainClause = accessChain?.Any() ?? false
                    ? $"The collection was accessed by the chain of \"{string.Join("\" -> \"", accessChain)}\". "
                    : "";

                return CoreError(
                    "BCP144",
                    $"Directly referencing a resource or module collection is not currently supported here. {accessChainClause}Apply an array indexer to the expression.");
            }

            public Diagnostic OutputMultipleDeclarations(string identifier) => CoreError(
                "BCP145",
                $"Output \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public Diagnostic ExpectedParameterDeclarationAfterDecorator() => CoreError(
                "BCP147",
                "Expected a parameter declaration after the decorator.");

            public Diagnostic ExpectedVariableDeclarationAfterDecorator() => CoreError(
                "BCP148",
                "Expected a variable declaration after the decorator.");

            public Diagnostic ExpectedResourceDeclarationAfterDecorator() => CoreError(
                "BCP149",
                "Expected a resource declaration after the decorator.");

            public Diagnostic ExpectedModuleDeclarationAfterDecorator() => CoreError(
                "BCP150",
                "Expected a module declaration after the decorator.");

            public Diagnostic ExpectedOutputDeclarationAfterDecorator() => CoreError(
                "BCP151",
                "Expected an output declaration after the decorator.");

            public Diagnostic CannotUseFunctionAsDecorator(string functionName) => CoreError(
                "BCP152",
                $"Function \"{functionName}\" cannot be used as a decorator.");

            public Diagnostic ExpectedResourceOrModuleDeclarationAfterDecorator() => CoreError(
                "BCP153",
                "Expected a resource or module declaration after the decorator.");

            public Diagnostic BatchSizeTooSmall(long value, long limit) => CoreError(
                "BCP154",
                $"Expected a batch size of at least {limit} but the specified value was \"{value}\".");

            public Diagnostic BatchSizeNotAllowed(string decoratorName) => CoreError(
                "BCP155",
                $"The decorator \"{decoratorName}\" can only be attached to resource or module collections.");

            public Diagnostic InvalidResourceTypeSegment(string typeSegment) => CoreError(
                "BCP156",
                $"The resource type segment \"{typeSegment}\" is invalid. Nested resources must specify a single type segment, and optionally can specify an api version using the format \"<type>@<apiVersion>\".");

            public Diagnostic InvalidAncestorResourceType() => CoreError(
                "BCP157",
                $"The resource type cannot be determined due to an error in the containing resource.");

            public Diagnostic ResourceRequiredForResourceAccess(string wrongType) => CoreError(
                "BCP158",
                $"Cannot access nested resources of type \"{wrongType}\". A resource type is required.");

            public Diagnostic NestedResourceNotFound(string resourceName, string identifierName, IEnumerable<string> nestedResourceNames)
            {
                var nestedResourceNamesClause = nestedResourceNames.Any()
                    ? $" Known nested resources are: {ToQuotedString(nestedResourceNames)}."
                    : string.Empty;

                return CoreError(
                    "BCP159",
                    $"""The resource "{resourceName}" does not contain a nested resource named "{identifierName}".{nestedResourceNamesClause}""");
            }

            public Diagnostic NestedResourceNotAllowedInLoop() => CoreError(
                "BCP160",
                $"A nested resource cannot appear inside of a resource with a for-expression.");

            public Diagnostic ExpectedLoopItemIdentifierOrVariableBlockStart() => CoreError(
                "BCP162",
                "Expected a loop item variable identifier or \"(\" at this location.");

            public Diagnostic ScopeUnsupportedOnChildResource() => CoreError(
                "BCP164",
                $"A child resource's scope is computed based on the scope of its ancestor resource. This means that using the \"{LanguageConstants.ResourceScopePropertyName}\" property on a child resource is unsupported.");

            public Diagnostic ScopeDisallowedForAncestorResource(string ancestorIdentifier) => CoreError(
                "BCP165",
                $"A resource's computed scope must match that of the Bicep file for it to be deployable. This resource's scope is computed from the \"{LanguageConstants.ResourceScopePropertyName}\" property value assigned to ancestor resource \"{ancestorIdentifier}\". You must use modules to deploy resources to a different scope.");

            public Diagnostic DuplicateDecorator(string decoratorName) => CoreError(
                "BCP166",
                $"Duplicate \"{decoratorName}\" decorator.");

            public Diagnostic ExpectBodyStartOrIf() => CoreError(
                "BCP167",
                "Expected the \"{\" character or the \"if\" keyword at this location.");

            public Diagnostic LengthMustNotBeNegative() => CoreError(
                "BCP168",
                $"Length must not be a negative value.");

            public Diagnostic TopLevelChildResourceNameIncorrectQualifierCount(int expectedSlashCount) => CoreError(
                "BCP169",
                $"Expected resource name to contain {expectedSlashCount} \"/\" character(s). The number of name segments must match the number of segments in the resource type.");

            public Diagnostic ChildResourceNameContainsQualifiers() => CoreError(
                "BCP170",
                $"Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name.");

            public Diagnostic ResourceTypeIsNotValidParent(string resourceType, string parentResourceType) => CoreError(
                "BCP171",
                $"Resource type \"{resourceType}\" is not a valid child resource of parent \"{parentResourceType}\".");

            public Diagnostic ParentResourceTypeHasErrors(string resourceName) => CoreError(
                "BCP172",
                $"The resource type cannot be validated due to an error in parent resource \"{resourceName}\".");

            public Diagnostic CannotUsePropertyInExistingResource(string property) => CoreError(
                "BCP173",
                $"The property \"{property}\" cannot be used in an existing resource declaration.");

            public Diagnostic ResourceTypeContainsProvidersSegment() => CoreWarning(
                "BCP174",
                $"Type validation is not available for resource types declared containing a \"/providers/\" segment. Please instead use the \"scope\" property.");

            public Diagnostic AnyTypeIsNotAllowed() => CoreError(
                "BCP176",
                $"Values of the \"any\" type are not allowed here.");

            public Diagnostic RuntimeValueNotAllowedInIfConditionExpression(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP177",
                    $"This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic RuntimeValueNotAllowedInForExpression(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP178",
                    $"This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic ForExpressionContainsLoopInvariants(string itemVariableName, string? indexVariableName, IEnumerable<string> expectedVariantProperties) => CoreWarning(
                "BCP179",
                indexVariableName is null
                    ? $"Unique resource or deployment name is required when looping. The loop item variable \"{itemVariableName}\" must be referenced in at least one of the value expressions of the following properties: {ToQuotedString(expectedVariantProperties)}"
                    : $"Unique resource or deployment name is required when looping. The loop item variable \"{itemVariableName}\" or the index variable \"{indexVariableName}\" must be referenced in at least one of the value expressions of the following properties in the loop body: {ToQuotedString(expectedVariantProperties)}");

            public Diagnostic FunctionOnlyValidInModuleSecureParameterAndExtensionConfigAssignment(string functionName, bool moduleExtensionConfigsEnabled) => CoreError(
                "BCP180",
                moduleExtensionConfigsEnabled
                    ? $"Function \"{functionName}\" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator or a secure extension configuration property."
                    : $"Function \"{functionName}\" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator.");

            public Diagnostic RuntimeValueNotAllowedInRunTimeFunctionArguments(string functionName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP181",
                    $"This expression is being used in an argument of the function \"{functionName}\", which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic RuntimeValueNotAllowedInVariableForBody(string variableName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain, string? violatingPropertyName)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var violatingPropertyNameClause = BuildNonDeployTimeConstantPropertyClause(accessedSymbolName, violatingPropertyName);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP182",
                    $"This expression is being used in the for-body of the variable \"{variableName}\", which requires values that can be calculated at the start of the deployment.{variableDependencyChainClause}{violatingPropertyNameClause}{accessiblePropertiesClause}");
            }

            public Diagnostic PropertyRequiresObjectLiteral(string propertyName) => CoreError(
                "BCP183",
                $"The value of the module \"{propertyName}\" property must be an object literal.");

            public Diagnostic FileExceedsMaximumSize(string filePath, long maxSize, string unit) => CoreError(
                "BCP184",
                $"File '{filePath}' exceeded maximum size of {maxSize} {unit}.");

            public Diagnostic FileEncodingMismatch(string detectedEncoding) => CoreDiagnostic(
                DiagnosticLevel.Info,
                "BCP185",
                $"Encoding mismatch. File was loaded with '{detectedEncoding}' encoding.");

            public Diagnostic UnparsableJsonType() => CoreError(
                "BCP186",
                $"Unable to parse literal JSON value. Please ensure that it is well-formed.");

            public Diagnostic FallbackPropertyUsed(bool shouldDowngrade, string property) => CoreDiagnostic(
                shouldDowngrade ? DiagnosticLevel.Info : DiagnosticLevel.Warning,
                "BCP187",
                $"The property \"{property}\" does not exist in the resource or type definition, although it might still be valid.{TypeInaccuracyClause}");

            public Diagnostic ReferencedArmTemplateHasErrors() => CoreError(
                "BCP188",
                $"The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template.");

            public Diagnostic UnknownModuleReferenceScheme(string badScheme, ImmutableArray<string> allowedSchemes)
            {
                string FormatSchemes() => ToQuotedString(allowedSchemes.Where(scheme => !string.Equals(scheme, ArtifactReferenceSchemes.Local)));

                return CoreError(
                    "BCP189",
                    (allowedSchemes.Contains(ArtifactReferenceSchemes.Local, StringComparer.Ordinal), allowedSchemes.Any(scheme => !string.Equals(scheme, ArtifactReferenceSchemes.Local, StringComparison.Ordinal))) switch
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
            public Diagnostic ArtifactRequiresRestore(string artifactRef) => CoreError(
                "BCP190",
                $"The artifact with reference \"{artifactRef}\" has not been restored.");

            public Diagnostic ArtifactRestoreFailed(string artifactRef) => CoreError(
                "BCP191",
                $"Unable to restore the artifact with reference \"{artifactRef}\".");

            public Diagnostic ArtifactRestoreFailedWithMessage(string artifactRef, string message) => CoreError(
                "BCP192",
                $"Unable to restore the artifact with reference \"{artifactRef}\": {message}");

            public Diagnostic InvalidOciArtifactReference(string? aliasName, string badRef) => CoreError(
                "BCP193",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} Specify a reference in the format of \"{ArtifactReferenceSchemes.Oci}:<artifact-uri>:<tag>\", or \"{ArtifactReferenceSchemes.Oci}/<module-alias>:<module-name-or-path>:<tag>\".");

            public Diagnostic InvalidTemplateSpecReference(string? aliasName, string badRef) => CoreError(
                "BCP194",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, badRef)} Specify a reference in the format of \"{ArtifactReferenceSchemes.TemplateSpecs}:<subscription-ID>/<resource-group-name>/<template-spec-name>:<version>\", or \"{ArtifactReferenceSchemes.TemplateSpecs}/<module-alias>:<template-spec-name>:<version>\".");

            public Diagnostic InvalidOciArtifactReferenceInvalidPathSegment(string? aliasName, string badRef, string badSegment) => CoreError(
                "BCP195",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The artifact path segment \"{badSegment}\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".");

            public Diagnostic InvalidOciArtifactReferenceMissingTagOrDigest(string? aliasName, string badRef) => CoreError(
                "BCP196",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The module tag or digest is missing.");

            public Diagnostic InvalidOciArtifactReferenceTagTooLong(string? aliasName, string badRef, string badTag, int maxLength) => CoreError(
                "BCP197",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The tag \"{badTag}\" exceeds the maximum length of {maxLength} characters.");

            public Diagnostic InvalidOciArtifactReferenceInvalidTag(string? aliasName, string badRef, string badTag) => CoreError(
                "BCP198",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The tag \"{badTag}\" is not valid. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".");

            public Diagnostic InvalidOciArtifactReferenceRepositoryTooLong(string? aliasName, string badRef, string badRepository, int maxLength) => CoreError(
                "BCP199",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} Module path \"{badRepository}\" exceeds the maximum length of {maxLength} characters.");

            public Diagnostic InvalidOciArtifactReferenceRegistryTooLong(string? aliasName, string badRef, string badRegistry, int maxLength) => CoreError(
                "BCP200",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The registry \"{badRegistry}\" exceeds the maximum length of {maxLength} characters.");

            public Diagnostic ExpectedExtensionSpecification() => CoreError(
                "BCP201",
                "Expected an extension specification string. This should either be a relative path, or a valid OCI artifact specification.");

            public Diagnostic ExpectedExtensionAliasName() => CoreError(
                "BCP202",
                "Expected an extension alias name at this location.");

            public Diagnostic UnrecognizedExtension(string identifier) => CoreError(
                "BCP204",
                $"Extension \"{identifier}\" is not recognized.");

            public Diagnostic ExtensionDoesNotSupportConfiguration(string identifier) => CoreError(
                "BCP205",
                $"Extension \"{identifier}\" does not support configuration.");

            public Diagnostic ExtensionRequiresConfiguration(string identifier) => CoreError(
                "BCP206",
                $"Extension \"{identifier}\" requires configuration, but none was provided.");

            public Diagnostic NamespaceMultipleDeclarations(string identifier) => CoreError(
                "BCP207",
                $"Namespace \"{identifier}\" is declared multiple times. Remove the duplicates.");

            public Diagnostic UnknownResourceReferenceScheme(string badNamespace, IEnumerable<string> allowedNamespaces) => CoreError(
                "BCP208",
                $"The specified namespace \"{badNamespace}\" is not recognized. Specify a resource reference using one of the following namespaces: {ToQuotedString(allowedNamespaces)}.");

            public Diagnostic FailedToFindResourceTypeInNamespace(string @namespace, string resourceType) => CoreError(
                "BCP209",
                $"Failed to find resource type \"{resourceType}\" in namespace \"{@namespace}\".");

            public Diagnostic ParentResourceInDifferentNamespace(string childNamespace, string parentNamespace) => CoreError(
                "BCP210",
                $"Resource type belonging to namespace \"{childNamespace}\" cannot have a parent resource type belonging to different namespace \"{parentNamespace}\".");

            public Diagnostic InvalidModuleAliasName(string aliasName) => CoreError(
                "BCP211",
                $"The module alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");

            public Diagnostic TemplateSpecModuleAliasNameDoesNotExistInConfiguration(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP212",
                $"The Template Spec module alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configFileUri)}.");

            public Diagnostic OciArtifactModuleAliasNameDoesNotExistInConfiguration(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP213",
                $"The OCI artifact module alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configFileUri)}.");

            public Diagnostic InvalidTemplateSpecAliasSubscriptionNullOrUndefined(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP214",
                $"The Template Spec module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is in valid. The \"subscription\" property cannot be null or undefined.");

            public Diagnostic InvalidTemplateSpecAliasResourceGroupNullOrUndefined(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP215",
                $"The Template Spec module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is in valid. The \"resourceGroup\" property cannot be null or undefined.");

            public Diagnostic InvalidOciArtifactModuleAliasRegistryNullOrUndefined(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP216",
                $"The OCI artifact module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is invalid. The \"registry\" property cannot be null or undefined.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidSubscriptionId(string? aliasName, string subscriptionId, string referenceValue) => CoreError(
                "BCP217",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The subscription ID \"{subscriptionId}\" in is not a GUID.");

            public Diagnostic InvalidTemplateSpecReferenceResourceGroupNameTooLong(string? aliasName, string resourceGroupName, string referenceValue, int maximumLength) => CoreError(
                "BCP218",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The resource group name \"{resourceGroupName}\" exceeds the maximum length of {maximumLength} characters.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidResourceGroupName(string? aliasName, string resourceGroupName, string referenceValue) => CoreError(
                "BCP219",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The resource group name \"{resourceGroupName}\" is invalid. Valid characters are alphanumeric, unicode characters, \".\", \"_\", \"-\", \"(\", or \")\", but the resource group name cannot end with \".\".");

            public Diagnostic InvalidTemplateSpecReferenceTemplateSpecNameTooLong(string? aliasName, string templateSpecName, string referenceValue, int maximumLength) => CoreError(
                "BCP220",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec name \"{templateSpecName}\" exceeds the maximum length of {maximumLength} characters.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidTemplateSpecName(string? aliasName, string templateSpecName, string referenceValue) => CoreError(
                "BCP221",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec name \"{templateSpecName}\" is invalid. Valid characters are alphanumeric, \".\", \"_\", \"-\", \"(\", or \")\", but the Template Spec name cannot end with \".\".");

            public Diagnostic InvalidTemplateSpecReferenceTemplateSpecVersionTooLong(string? aliasName, string templateSpecVersion, string referenceValue, int maximumLength) => CoreError(
                "BCP222",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec version \"{templateSpecVersion}\" exceeds the maximum length of {maximumLength} characters.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidTemplateSpecVersion(string? aliasName, string templateSpecVersion, string referenceValue) => CoreError(
                "BCP223",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec version \"{templateSpecVersion}\" is invalid. Valid characters are alphanumeric, \".\", \"_\", \"-\", \"(\", or \")\", but the Template Spec name cannot end with \".\".");

            public Diagnostic InvalidOciArtifactReferenceInvalidDigest(string? aliasName, string badRef, string badDigest) => CoreError(
                "BCP224",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The digest \"{badDigest}\" is not valid. The valid format is a string \"sha256:\" followed by exactly 64 lowercase hexadecimal digits.");

            public Diagnostic AmbiguousDiscriminatorPropertyValue(string propertyName) => CoreWarning(
                "BCP225",
                $"The discriminator property \"{propertyName}\" value cannot be determined at compilation time. Type checking for this object is disabled.");

            public Diagnostic MissingDiagnosticCodes() => CoreError(
                "BCP226",
                "Expected at least one diagnostic code at this location. Valid format is \"#disable-next-line diagnosticCode1 diagnosticCode2 ...\""
            );

            public Diagnostic UnsupportedResourceTypeParameterOrOutputType(string resourceType) => CoreError(
                "BCP227",
                $"The type \"{resourceType}\" cannot be used as a parameter or output type. Resource types from extensions are currently not supported as parameters or outputs.");

            public Diagnostic InvalidResourceScopeCannotBeResourceTypeParameter(string parameterName) => CoreError(
                "BCP229",
                $"The parameter \"{parameterName}\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource.");

            public Diagnostic ModuleParamOrOutputResourceTypeUnavailable(ResourceTypeReference resourceTypeReference) => CoreWarning(
                "BCP230",
                $"The referenced module uses resource type \"{resourceTypeReference.FormatName()}\" which does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.");

            public Diagnostic ParamOrOutputResourceTypeUnsupported() => CoreError(
                "BCP231",
                $@"Using resource-typed parameters and outputs requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs)}"".");

            public Diagnostic ArtifactDeleteFailed(string moduleRef) => CoreError(
                "BCP232",
                $"Unable to delete the module with reference \"{moduleRef}\" from cache.");

            public Diagnostic ArtifactDeleteFailedWithMessage(string moduleRef, string message) => CoreError(
                "BCP233",
                $"Unable to delete the module with reference \"{moduleRef}\" from cache: {message}");

            public Diagnostic ArmFunctionLiteralTypeConversionFailedWithMessage(string literalValue, string armFunctionName, string message) => CoreWarning(
                "BCP234",
                $"The ARM function \"{armFunctionName}\" failed when invoked on the value [{literalValue}]: {message}");

            public Diagnostic NoJsonTokenOnPathOrPathInvalid() => CoreError(
                "BCP235",
                $"Specified JSONPath does not exist in the given file or is invalid.");

            public Diagnostic ExpectedNewLineOrCommaSeparator() => CoreError(
                "BCP236",
                "Expected a new line or comma character at this location.");

            public Diagnostic ExpectedCommaSeparator() => CoreError(
                "BCP237",
                "Expected a comma character at this location.");

            public Diagnostic UnexpectedNewLineAfterCommaSeparator() => CoreError(
                "BCP238",
                "Unexpected new line character after a comma.");

            public Diagnostic ReservedIdentifier(string name) => CoreError(
                "BCP239",
                $"Identifier \"{name}\" is a reserved Bicep symbol name and cannot be used in this context.");

            public Diagnostic InvalidValueForParentProperty() => CoreError(
                "BCP240",
                "The \"parent\" property only permits direct references to resources. Expressions are not supported.");

            public Diagnostic DeprecatedProvidersFunction(string functionName) => CoreWarning(
                "BCP241",
                $"The \"{functionName}\" function is deprecated and will be removed in a future release of Bicep. Please add a comment to https://github.com/Azure/bicep/issues/2017 if you believe this will impact your workflow.")
                with
            { Styling = DiagnosticStyling.ShowCodeDeprecated };

            public Diagnostic LambdaFunctionsOnlyValidInFunctionArguments() => CoreError(
                "BCP242",
                $"Lambda functions may only be specified directly as function arguments.");

            public Diagnostic ParenthesesMustHaveExactlyOneItem() => CoreError(
                "BCP243",
                "Parentheses must contain exactly one expression.");

            public Diagnostic LambdaExpectedArgCountMismatch(TypeSymbol lambdaType, int minArgCount, int maxArgCount, int actualArgCount) => CoreError(
                "BCP244",
                minArgCount == maxArgCount ?
                    $"Expected lambda expression of type \"{lambdaType}\" with {minArgCount} arguments but received {actualArgCount} arguments." :
                    $"Expected lambda expression of type \"{lambdaType}\" with between {minArgCount} and {maxArgCount} arguments but received {actualArgCount} arguments.");

            public Diagnostic ResourceTypeIsReadonly(ResourceTypeReference resourceTypeReference) => CoreWarning(
                "BCP245",
                $"Resource type \"{resourceTypeReference.FormatName()}\" can only be used with the 'existing' keyword.");

            public Diagnostic ResourceTypeIsReadonlyAtScope(ResourceTypeReference resourceTypeReference, ResourceScope writableScopes) => CoreWarning(
                "BCP246",
                $"Resource type \"{resourceTypeReference.FormatName()}\" can only be used with the 'existing' keyword at the requested scope."
                    + $" Permitted scopes for deployment: {ToQuotedString(LanguageConstants.GetResourceScopeDescriptions(writableScopes))}.");

            public Diagnostic LambdaVariablesInResourceOrModuleArrayAccessUnsupported(IEnumerable<string> variableNames) => CoreError(
                "BCP247",
                $"Using lambda variables inside resource or module array access is not currently supported."
                    + $" Found the following lambda variable(s) being accessed: {ToQuotedString(variableNames)}.");

            public Diagnostic LambdaVariablesInInlineFunctionUnsupported(string functionName, IEnumerable<string> variableNames) => CoreError(
                "BCP248",
                $"Using lambda variables inside the \"{functionName}\" function is not currently supported."
                    + $" Found the following lambda variable(s) being accessed: {ToQuotedString(variableNames)}.");

            public Diagnostic ExpectedLoopVariableBlockWith2Elements(int actualCount) => CoreError(
                "BCP249",
                $"Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found {actualCount}.");

            public Diagnostic ParameterMultipleAssignments(string identifier) => CoreError(
                "BCP250",
                $"Parameter \"{identifier}\" is assigned multiple times. Remove or rename the duplicates.");

            public Diagnostic UsingPathHasNotBeenSpecified() => CoreError(
                "BCP256",
                "The using declaration is missing a bicep template file path reference.");

            public Diagnostic ExpectedFilePathString() => CoreError(
                "BCP257",
                "Expected a Bicep file path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep'");

            public IDiagnostic MissingParameterAssignment(IEnumerable<string> identifiers, CodeFix insertMissingCodefix) => CoreError(
                "BCP258",
                $"The following parameters are declared in the Bicep file but are missing an assignment in the params file: {ToQuotedString(identifiers)}.")
                with
            { Fixes = [insertMissingCodefix] };

            public Diagnostic MissingParameterDeclaration(string? identifier) => CoreError(
                "BCP259",
                $"The parameter \"{identifier}\" is assigned in the params file without being declared in the Bicep file.");

            public Diagnostic ParameterTypeMismatch(string? identifier, TypeSymbol expectedType, TypeSymbol actualType) => CoreError(
                "BCP260",
                $"The parameter \"{identifier}\" expects a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic UsingDeclarationNotSpecified() => CoreError(
                "BCP261",
                "A using declaration must be present in this parameters file.");

            public Diagnostic MoreThanOneUsingDeclarationSpecified() => CoreError(
                "BCP262",
                "More than one using declaration are present");

            public Diagnostic UsingDeclarationReferencesInvalidFile() => CoreError(
                "BCP263",
                "The file specified in the using declaration path does not exist");

            public Diagnostic AmbiguousResourceTypeBetweenImports(string resourceTypeName, IEnumerable<string> namespaces) => CoreError(
                "BCP264",
                $"Resource type \"{resourceTypeName}\" is declared in multiple imported namespaces ({ToQuotedStringWithCaseInsensitiveOrdering(namespaces)}), and must be fully-qualified.");

            public Diagnostic SymbolicNameShadowsAKnownFunction(string name, string knownFunctionNamespace, string knownFunctionName) => CoreError(
                "BCP265",
                $"The name \"{name}\" is not a function. Did you mean \"{knownFunctionNamespace}.{knownFunctionName}\"?")
                with
            {
                Fixes = [
                    new CodeFix($"Change \"{name}\" to \"{knownFunctionNamespace}.{knownFunctionName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, $"{knownFunctionNamespace}.{knownFunctionName}"))
                ]
            };

            public Diagnostic ExpectedMetadataIdentifier() => CoreError(
                "BCP266",
                "Expected a metadata identifier at this location.");

            public Diagnostic ExpectedMetadataDeclarationAfterDecorator() => CoreError(
                "BCP267",
                "Expected an metadata declaration after the decorator.");

            public Diagnostic ReservedMetadataIdentifier(string name) => CoreError(
                "BCP268",
                $"Invalid identifier: \"{name}\". Metadata identifiers starting with '_' are reserved. Please use a different identifier.");

            public Diagnostic CannotUseFunctionAsMetadataDecorator(string functionName) => CoreError(
                "BCP269",
                $"Function \"{functionName}\" cannot be used as a metadata decorator.");

            public Diagnostic UnparsableBicepConfigFile(IOUri configFileUri, string parsingErrorMessage) => CoreError(
                "BCP271",
                $"Failed to parse the contents of the Bicep configuration file \"{configFileUri}\" as valid JSON: {parsingErrorMessage.TrimEnd('.')}.");

            public Diagnostic UnloadableBicepConfigFile(IOUri configFileUri, string loadErrorMessage) => CoreError(
                "BCP272",
                $"Could not load the Bicep configuration file \"{configFileUri}\": {loadErrorMessage.TrimEnd('.')}.");

            public Diagnostic InvalidBicepConfigFile(IOUri configFileUri, string parsingErrorMessage) => CoreError(
                "BCP273",
                $"Failed to parse the contents of the Bicep configuration file \"{configFileUri}\": {parsingErrorMessage.TrimEnd('.')}.");

            public Diagnostic PotentialConfigDirectoryCouldNotBeScanned(IOUri? directoryIdentifier, string scanErrorMessage) => CoreDiagnostic(
                DiagnosticLevel.Info, // should this be a warning instead?
                "BCP274",
                $"Error scanning \"{directoryIdentifier}\" for bicep configuration: {scanErrorMessage.TrimEnd('.')}.");

            public Diagnostic FoundDirectoryInsteadOfFile(string directoryPath) => CoreError(
                "BCP275",
                $"Unable to open file at path \"{directoryPath}\". Found a directory instead.");

            public Diagnostic UsingDeclarationMustReferenceBicepFile() => CoreError(
                "BCP276",
                "A using declaration can only reference a Bicep file.");

            public Diagnostic ModuleDeclarationMustReferenceBicepModule() => CoreError(
                "BCP277",
                "A module declaration can only reference a Bicep File, an ARM template, a registry reference or a template spec reference.");

            public Diagnostic CyclicParametersSelfReference() => CoreError(
                "BCP278",
                "This parameters file references itself, which is not allowed.");

            public Diagnostic UnrecognizedTypeExpression() => CoreError(
                "BCP279",
                $"Expected a type at this location. Please specify a valid type expression or one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public Diagnostic TypeExpressionLiteralConversionFailed() => CoreError(
                "BCP285",
                "The type expression could not be reduced to a literal value.");

            public Diagnostic InvalidUnionTypeMember(string keystoneType) => CoreError(
                "BCP286",
                $"This union member is invalid because it cannot be assigned to the '{keystoneType}' type.");

            public Diagnostic ValueSymbolUsedAsType(string symbolName) => CoreError(
                "BCP287",
                // TODO: Add "Did you mean 'typeof({symbolName})'?" When support for typeof has been added.
                $"'{symbolName}' refers to a value but is being used as a type here.");

            public Diagnostic TypeSymbolUsedAsValue(string symbolName) => CoreError(
                "BCP288",
                $"'{symbolName}' refers to a type but is being used as a value here.");

            public Diagnostic InvalidTypeDefinition() => CoreError(
                "BCP289",
                $"The type definition is not valid.");

            public Diagnostic ExpectedParameterOrTypeDeclarationAfterDecorator() => CoreError(
                "BCP290",
                "Expected a parameter or type declaration after the decorator.");

            public Diagnostic ExpectedParameterOrOutputDeclarationAfterDecorator() => CoreError(
                "BCP291",
                "Expected a parameter or output declaration after the decorator.");

            public Diagnostic ExpectedParameterOutputOrTypeDeclarationAfterDecorator() => CoreError(
                "BCP292",
                "Expected a parameter, output, or type declaration after the decorator.");

            public Diagnostic NonLiteralUnionMember() => CoreError(
                "BCP293",
                "All members of a union type declaration must be literal values.");

            public Diagnostic InvalidTypeUnion() => CoreError(
                "BCP294",
                "Type unions must be reducible to a single ARM type (such as 'string', 'int', or 'bool').");

            public Diagnostic DecoratorNotPermittedOnLiteralType(string decoratorName) => CoreError(
                "BCP295",
                $"The '{decoratorName}' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically.");

            public Diagnostic NonConstantTypeProperty() => CoreError(
                "BCP296",
                "Property names on types must be compile-time constant values.");

            public Diagnostic CannotUseFunctionAsTypeDecorator(string functionName) => CoreError(
                "BCP297",
                $"Function \"{functionName}\" cannot be used as a type decorator.");

            public Diagnostic CyclicTypeSelfReference() => CoreError(
                "BCP298",
                "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled.");

            public Diagnostic CyclicType(IEnumerable<string> cycle) => CoreError(
                "BCP299",
                $"This type definition includes itself as a required component via a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ExpectedTypeLiteral() => CoreError(
                "BCP300",
                $"Expected a type literal at this location. Please specify a concrete value or a reference to a literal type.");

            public Diagnostic ReservedTypeName(string reservedName) => CoreError(
                "BCP301",
                $@"The type name ""{reservedName}"" is reserved and may not be attached to a user-defined type.");

            public Diagnostic SymbolicNameIsNotAType(string name, IEnumerable<string> validTypes) => CoreError(
                "BCP302",
                $@"The name ""{name}"" is not a valid type. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic ExtensionSpecificationInterpolationUnsupported() => CoreError(
                "BCP303",
                "String interpolation is unsupported for specifying the extension.");

            public Diagnostic ExpectedWithOrAsKeywordOrNewLine() => CoreError(
                "BCP305",
                $"Expected the \"with\" keyword, \"as\" keyword, or a new line character at this location.");

            public Diagnostic NamespaceSymbolUsedAsType(string name) => CoreError(
                "BCP306",
                $@"The name ""{name}"" refers to a namespace, not to a type.");

            public Diagnostic NestedRuntimePropertyAccessNotSupported(string? resourceSymbol, IEnumerable<string> runtimePropertyNames, IEnumerable<string> accessiblePropertyNames, IEnumerable<string> accessibleFunctionNames)
            {
                var accessiblePropertyNamesClause = accessiblePropertyNames.Any() ? @$" the accessible properties of ""{resourceSymbol}"" include {ToQuotedString(accessiblePropertyNames.OrderBy(x => x))}." : "";
                var accessibleFunctionNamesClause = accessibleFunctionNames.Any() ? @$" The accessible functions of ""{resourceSymbol}"" include {ToQuotedString(accessibleFunctionNames.OrderBy(x => x))}." : "";

                return CoreError(
                    "BCP307",
                    $"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including {ToQuotedString(runtimePropertyNames.OrderBy(x => x))} cannot be calculated at the start of the deployment. In this situation,{accessiblePropertyNamesClause}{accessibleFunctionNamesClause}");
            }

            public Diagnostic DecoratorMayNotTargetTypeAlias(string decoratorName) => CoreError(
                "BCP308",
                $@"The decorator ""{decoratorName}"" may not be used on statements whose declared type is a reference to a user-defined type.");

            public Diagnostic ValueCannotBeFlattened(TypeSymbol flattenInputType, TypeSymbol incompatibleType) => CoreError(
                "BCP309",
                $@"Values of type ""{flattenInputType.Name}"" cannot be flattened because ""{incompatibleType.Name}"" is not an array type.");

            public Diagnostic IndexOutOfBounds(string typeName, long tupleLength, long indexSought)
            {
                var message = new StringBuilder("The provided index value of \"").Append(indexSought).Append("\" is not valid for type \"").Append(typeName).Append("\".");
                if (tupleLength > 0)
                {
                    message.Append(" Indexes for this type must be between 0 and ").Append(tupleLength - 1).Append('.');
                }

                return CoreError(
                    "BCP311",
                    message.ToString());
            }

            public Diagnostic MultipleAdditionalPropertiesDeclarations() => CoreError(
                "BCP315",
                "An object type may have at most one additional properties declaration.");

            public Diagnostic SealedIncompatibleWithAdditionalPropertiesDeclaration() => CoreError(
                "BCP316",
                $@"The ""{LanguageConstants.ParameterSealedPropertyName}"" decorator may not be used on object types with an explicit additional properties type declaration.");

            public Diagnostic ExpectedPropertyNameOrMatcher() => CoreError(
                "BCP317",
                "Expected an identifier, a string, or an asterisk at this location.");

            public Diagnostic DereferenceOfPossiblyNullReference(string possiblyNullType, AccessExpressionSyntax accessExpression) => CoreWarning(
                "BCP318",
                $@"The value of type ""{possiblyNullType}"" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail.")
                with
            {
                Fixes = [
                    new(
                        "If you do not know whether the value will be null and the template would handle a null value for the overall expression, use a `.?` (safe dereference) operator to short-circuit the access expression if the base expression's value is null",
                        true,
                        CodeFixKind.QuickFix,
                        new(accessExpression.Span, accessExpression.AsSafeAccess().ToString())),
                    AsNonNullable(accessExpression.BaseExpression),
                ]
            };

            private static CodeFix AsNonNullable(SyntaxBase expression) => new(
                "If you know the value will not be null, use a non-null assertion operator to inform the compiler that the value will not be null",
                false,
                CodeFixKind.QuickFix,
                new(expression.Span, SyntaxFactory.AsNonNullable(expression).ToString()));

            public Diagnostic UnresolvableArmJsonType(string errorSource, string message) => CoreError(
                "BCP319",
                $@"The type at ""{errorSource}"" could not be resolved by the ARM JSON template engine. Original error message: ""{message}""");

            public Diagnostic ModuleOutputResourcePropertyAccessDetected() => CoreError(
                "BCP320",
                "The properties of module output resources cannot be accessed directly. To use the properties of this resource, pass it as a resource-typed parameter to another module and access the parameter's properties therein.");

            public Diagnostic PossibleNullReferenceAssignment(TypeSymbol expectedType, TypeSymbol actualType, SyntaxBase expression) => CoreWarning(
                "BCP321",
                $"Expected a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".")
                with
            { Fixes = [AsNonNullable(expression)] };

            public Diagnostic SafeDereferenceNotPermittedOnInstanceFunctions() => CoreError(
                "BCP322",
                "The `.?` (safe dereference) operator may not be used on instance function invocations.");

            public Diagnostic SafeDereferenceNotPermittedOnResourceCollections() => CoreError(
                "BCP323",
                "The `[?]` (safe dereference) operator may not be used on resource or module collections.");

            public Diagnostic ExpectedTypeIdentifier() => CoreError(
                "BCP325",
                "Expected a type identifier at this location.");

            public Diagnostic NullableTypedParamsMayNotHaveDefaultValues() => CoreError(
                "BCP326",
                "Nullable-typed parameters may not be assigned default values. They have an implicit default of 'null' that cannot be overridden.");

            public Diagnostic SourceIntDomainDisjointFromTargetIntDomain_SourceHigh(bool warnInsteadOfError, long sourceMin, long targetMax) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP327",
                $"The provided value (which will always be greater than or equal to {sourceMin}) is too large to assign to a target for which the maximum allowable value is {targetMax}.");

            public Diagnostic SourceIntDomainDisjointFromTargetIntDomain_SourceLow(bool warnInsteadOfError, long sourceMax, long targetMin) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP328",
                $"The provided value (which will always be less than or equal to {sourceMax}) is too small to assign to a target for which the minimum allowable value is {targetMin}.");

            public Diagnostic SourceIntDomainExtendsBelowTargetIntDomain(long sourceMin, long targetMin) => CoreWarning(
                "BCP329",
                $"The provided value can be as small as {sourceMin} and may be too small to assign to a target with a configured minimum of {targetMin}.");

            public Diagnostic SourceIntDomainExtendsAboveTargetIntDomain(long sourceMax, long targetMax) => CoreWarning(
                "BCP330",
                $"The provided value can be as large as {sourceMax} and may be too large to assign to a target with a configured maximum of {targetMax}.");

            public Diagnostic MinMayNotExceedMax(string minDecoratorName, long minValue, string maxDecoratorName, long maxValue) => CoreError(
                "BCP331",
                $@"A type's ""{minDecoratorName}"" must be less than or equal to its ""{maxDecoratorName}"", but a minimum of {minValue} and a maximum of {maxValue} were specified.");

            public Diagnostic SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(bool warnInsteadOfError, long sourceMinLength, long targetMaxLength) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP332",
                $"The provided value (whose length will always be greater than or equal to {sourceMinLength}) is too long to assign to a target for which the maximum allowable length is {targetMaxLength}.");

            public Diagnostic SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(bool warnInsteadOfError, long sourceMaxLength, long targetMinLength) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP333",
                $"The provided value (whose length will always be less than or equal to {sourceMaxLength}) is too short to assign to a target for which the minimum allowable length is {targetMinLength}.");

            public Diagnostic SourceValueLengthDomainExtendsBelowTargetValueLengthDomain(long sourceMinLength, long targetMinLength) => CoreWarning(
                "BCP334",
                $"The provided value can have a length as small as {sourceMinLength} and may be too short to assign to a target with a configured minimum length of {targetMinLength}.");

            public Diagnostic SourceValueLengthDomainExtendsAboveTargetValueLengthDomain(long sourceMaxLength, long targetMaxLength) => CoreWarning(
                "BCP335",
                $"The provided value can have a length as large as {sourceMaxLength} and may be too long to assign to a target with a configured maximum length of {targetMaxLength}.");

            public Diagnostic UnrecognizedParamsFileDeclaration(bool moduleExtensionConfigsEnabled)
            {
                List<string> supportedDeclarations = [
                    LanguageConstants.UsingKeyword,
                    LanguageConstants.ExtendsKeyword,
                    LanguageConstants.ParameterKeyword,
                    LanguageConstants.VariableKeyword,
                    LanguageConstants.TypeKeyword,
                ];

                if (moduleExtensionConfigsEnabled)
                {
                    supportedDeclarations.Add(LanguageConstants.ExtensionConfigKeyword);
                }

                return CoreError(
                    "BCP337",
                    $@"This declaration type is not valid for a Bicep Parameters file. Supported declarations: {ToQuotedString(supportedDeclarations)}.");
            }

            public Diagnostic FailedToEvaluateSubject(string subjectType, string subjectName, string message) => CoreError(
                "BCP338",
                $"Failed to evaluate {subjectType} \"{subjectName}\": {message}");

            public Diagnostic ArrayIndexOutOfBounds(long indexSought) => CoreError(
                "BCP339",
                $"""The provided array index value of "{indexSought}" is not valid. Array index should be greater than or equal to 0.""");

            public Diagnostic UnparsableYamlType() => CoreError(
                "BCP340",
                $"Unable to parse literal YAML value. Please ensure that it is well-formed.");

            public Diagnostic RuntimeValueNotAllowedInFunctionDeclaration(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP341",
                    $"This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic UserDefinedTypesNotAllowedInFunctionDeclaration() => CoreError(
                "BCP342",
                $"""User-defined types are not supported in user-defined function parameters or outputs.""");

            public Diagnostic ExpectedAssertIdentifier() => CoreError(
                "BCP344",
                "Expected an assert identifier at this location.");

            public Diagnostic TestDeclarationMustReferenceBicepTest() => CoreError(
                "BCP345",
                "A test declaration can only reference a Bicep File");

            public Diagnostic ExpectedTestIdentifier() => CoreError(
                "BCP346",
                "Expected a test identifier at this location.");

            public Diagnostic ExpectedTestPathString() => CoreError(
                "BCP347",
                "Expected a test path string at this location.");
            public Diagnostic TestDeclarationStatementsUnsupported() => CoreError(
                "BCP348",
                $@"Using a test declaration statement requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.TestFramework)}"".");

            public Diagnostic AssertsUnsupported() => CoreError(
                "BCP349",
                $@"Using an assert declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.Assertions)}"".");

            public Diagnostic InvalidAssertAssignment(TypeSymbol valueType) => CoreError(
                "BCP350",
                $"Value of type \"{valueType}\" cannot be assigned to an assert. Asserts can take values of type 'bool' only.");

            public Diagnostic FunctionOnlyValidWithDirectAssignment(string functionName) => CoreError(
                "BCP351",
                $"Function \"{functionName}\" is not valid at this location. It can only be used when directly assigning to a parameter.");

            public Diagnostic FailedToEvaluateVariable(string name, string message) => CoreError(
                "BCP352",
                $"Failed to evaluate variable \"{name}\": {message}");

            public Diagnostic ItemsMustBeCaseInsensitivelyUnique(string itemTypePluralName, IEnumerable<string> itemNames) => CoreError(
                "BCP353",
                $"The {itemTypePluralName} {ToQuotedString(itemNames)} differ only in casing. The ARM deployments engine is not case sensitive and will not be able to distinguish between them.");

            public Diagnostic ExpectedSymbolListOrWildcard() => CoreError(
                "BCP354",
                "Expected left brace ('{') or asterisk ('*') character at this location.");

            public Diagnostic ExpectedExportedSymbolName() => CoreError(
                "BCP355",
                "Expected the name of an exported symbol at this location.");

            public Diagnostic ExpectedNamespaceIdentifier() => CoreError(
                "BCP356",
                "Expected a valid namespace identifier at this location.");

            public Diagnostic PathHasNotBeenSpecified() => CoreError(
                "BCP358",
                "This declaration is missing a template file path reference.");

            public Diagnostic ImportedSymbolNotFound(string symbolName) => CoreError(
                "BCP360",
                $"The '{symbolName}' symbol was not found in (or was not exported by) the imported template.");

            public Diagnostic ExportDecoratorMustTargetStatement() => CoreError(
                "BCP361",
                @"The ""@export()"" decorator must target a top-level statement.");

            public Diagnostic SymbolImportedMultipleTimes(params string[] importedAs) => CoreError(
                "BCP362",
                $"This symbol is imported multiple times under the names {string.Join(", ", importedAs.Select(identifier => $"'{identifier}'"))}.");

            public Diagnostic DiscriminatorDecoratorOnlySupportedForObjectUnions() => CoreError(
                "BCP363",
                $"The \"{LanguageConstants.TypeDiscriminatorDecoratorName}\" decorator can only be applied to object-only union types with unique member types.");

            public Diagnostic DiscriminatorPropertyMustBeRequiredStringLiteral(string discriminatorPropertyName) => CoreError(
                "BCP364",
                $"The property \"{discriminatorPropertyName}\" must be a required string literal on all union member types.");

            public Diagnostic DiscriminatorPropertyMemberDuplicatedValue(string discriminatorPropertyName, string discriminatorPropertyValue) => CoreError(
                "BCP365",
                $"The value \"{discriminatorPropertyValue}\" for discriminator property \"{discriminatorPropertyName}\" is duplicated across multiple union member types. The value must be unique across all union member types.");

            public Diagnostic DiscriminatorPropertyNameMustMatch(string acceptablePropertyName) => CoreError(
                "BCP366",
                $"The discriminator property name must be \"{acceptablePropertyName}\" on all union member types.");

            public Diagnostic FeatureIsTemporarilyDisabled(string featureName) => CoreError(
                "BCP367",
                $"The \"{featureName}\" feature is temporarily disabled.");

            public Diagnostic ParameterReferencesKeyVaultSuppliedParameter(string targetName) => CoreError(
                "BCP368",
                $"The value of the \"{targetName}\" parameter cannot be known until the template deployment has started because it uses a reference to a secret value in Azure Key Vault. Expressions that refer to the \"{targetName}\" parameter may be used in {LanguageConstants.LanguageFileExtension} files but not in {LanguageConstants.ParamsFileExtension} files.");

            public Diagnostic ParameterReferencesDefaultedParameter(string targetName) => CoreError(
                "BCP369",
                $"The value of the \"{targetName}\" parameter cannot be known until the template deployment has started because it uses the default value defined in the template. Expressions that refer to the \"{targetName}\" parameter may be used in {LanguageConstants.LanguageFileExtension} files but not in {LanguageConstants.ParamsFileExtension} files.");

            public Diagnostic ClosureContainsNonExportableSymbols(IEnumerable<string> nonExportableSymbols) => CoreError(
                "BCP372",
                @$"The ""@export()"" decorator may not be applied to variables that refer to parameters, modules, or resource, either directly or indirectly. The target of this decorator contains direct or transitive references to the following unexportable symbols: {ToQuotedString(nonExportableSymbols)}.");

            public Diagnostic ImportedSymbolHasErrors(string name, string message) => CoreError(
                "BCP373",
                $"Unable to import the symbol named \"{name}\": {message}");

            public Diagnostic ImportedModelContainsAmbiguousExports(IEnumerable<string> ambiguousExportNames) => CoreError(
                "BCP374",
                $"The imported model cannot be loaded with a wildcard because it contains the following duplicated exports: {ToQuotedString(ambiguousExportNames)}.");

            public Diagnostic ImportListItemDoesNotIncludeDeclaredSymbolName() => CoreError(
                "BCP375",
                "An import list item that identifies its target with a quoted string must include an 'as <alias>' clause.");

            public Diagnostic ImportedSymbolKindNotSupportedInSourceFileKind(string name, ExportMetadataKind exportMetadataKind, BicepSourceFileKind sourceFileKind) => CoreError(
                "BCP376",
                $"The \"{name}\" symbol cannot be imported because imports of kind {exportMetadataKind} are not supported in files of kind {sourceFileKind}.");

            public Diagnostic InvalidExtensionAliasName(string aliasName) => CoreError(
                "BCP377",
                $"The extension alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");

            public Diagnostic InvalidOciArtifactExtensionAliasRegistryNullOrUndefined(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP378",
                $"The OCI artifact extension alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is invalid. The \"registry\" property cannot be null or undefined.");

            public Diagnostic OciArtifactExtensionAliasNameDoesNotExistInConfiguration(string aliasName, IOUri? configFileUri) => CoreError(
                "BCP379",
                $"The OCI artifact extension alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configFileUri)}.");

            public Diagnostic UnsupportedArtifactType(ArtifactType artifactType) => CoreError(
                "BCP380",
                $"Artifacts of type: \"{artifactType}\" are not supported."
            );

            public Diagnostic TypeIsNotParameterizable(string typeName) => CoreError(
                "BCP383",
                $"The \"{typeName}\" type is not parameterizable.");

            public Diagnostic TypeRequiresParameterization(string typeName, int requiredArgumentCount) => CoreError(
                "BCP384",
                $"The \"{typeName}\" type requires {requiredArgumentCount} argument(s).");

            public Diagnostic DecoratorMayNotTargetResourceDerivedType(string decoratorName) => CoreError(
                "BCP386",
                $@"The decorator ""{decoratorName}"" may not be used on statements whose declared type is a reference to a resource-derived type.");

            public Diagnostic NegatedTypeIndexSought() => CoreError(
                "BCP387",
                "Indexing into a type requires an integer greater than or equal to 0.");

            public Diagnostic TupleRequiredForIndexAccess(TypeSymbol wrongType) => CoreError(
                "BCP388",
                $"Cannot access elements of type \"{wrongType}\" by index. An tuple type is required.");

            public Diagnostic ExplicitAdditionalPropertiesTypeRequiredForAccessThereto(TypeSymbol wrongType) => CoreError(
                "BCP389",
                $"The type \"{wrongType}\" does not declare an additional properties type.");

            public Diagnostic ExplicitItemsTypeRequiredForAccessThereto() => CoreError(
                "BCP390",
                $"The array item type access operator ('[*]') can only be used with typed arrays.");

            public Diagnostic AccessExpressionForbiddenBase() => CoreError(
                "BCP391",
                "Type member access is only supported on a reference to a named type.");

            public Diagnostic InvalidResourceTypeIdentifier(string resourceTypeIdentifier) => CoreWarning(
                "BCP392",
                $"""The supplied resource type identifier "{resourceTypeIdentifier}" was not recognized as a valid resource type name.""");

            public Diagnostic UnrecognizedResourceDerivedTypePointerSegment(string unrecognizedSegment) => CoreWarning(
                "BCP393",
                $"""The type pointer segment "{unrecognizedSegment}" was not recognized. Supported pointer segments are: "properties", "items", "prefixItems", and "additionalProperties".""");

            public Diagnostic CannotUseEntireResourceBodyAsType() => CoreError(
                "BCP394",
                "Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted.");

            public Diagnostic InvalidTypesTgzPackage_DeserializationFailed() => CoreError(
                "BCP396",
                "The referenced extension types artifact has been published with malformed content.");

            public Diagnostic InvalidExtension_ImplicitExtensionMissingConfig(IOUri? configFileUri, string name) => CoreError(
                "BCP397",
                $"""Extension {name} is incorrectly configured in the {BuildBicepConfigurationClause(configFileUri)}. It is referenced in the "{RootConfiguration.ImplicitExtensionsKey}" section, but is missing corresponding configuration in the "{RootConfiguration.ExtensionsKey}" section.""");

            public Diagnostic InvalidExtension_NotABuiltInExtension(IOUri? configFileUri, string name) => CoreError(
                "BCP398",
                $"""Extension {name} is incorrectly configured in the {BuildBicepConfigurationClause(configFileUri)}. It is configured as built-in in the "{RootConfiguration.ExtensionsKey}" section, but no built-in extension exists.""");

            public Diagnostic SpreadOperatorUnsupportedInLocation(SpreadExpressionSyntax spread) => CoreError(
                "BCP401",
                $"The spread operator \"{spread.Ellipsis.Text}\" is not permitted in this location.");

            public Diagnostic SpreadOperatorRequiresAssignableValue(SpreadExpressionSyntax spread, TypeSymbol requiredType) => CoreError(
                "BCP402",
                $"The spread operator \"{spread.Ellipsis.Text}\" can only be used in this context for an expression assignable to type \"{requiredType}\".");

            public Diagnostic ArrayTypeMismatchSpread(bool warnInsteadOfError, TypeSymbol expectedType, TypeSymbol actualType) => CoreDiagnostic(
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP403",
                $"The enclosing array expects elements of type \"{expectedType}\", but the array being spread contains elements of incompatible type \"{actualType}\".");

            public Diagnostic ExtendsPathHasNotBeenSpecified() => CoreError(
                "BCP404",
                $"The \"{LanguageConstants.ExtendsKeyword}\" declaration is missing a bicepparam file path reference");

            public Diagnostic MoreThanOneExtendsDeclarationSpecified() => CoreError(
                "BCP405",
                $"More than one \"{LanguageConstants.ExtendsKeyword}\" declaration are present");

            public Diagnostic ExtendsNotSupported() => CoreError(
                "BCP406",
                $"Using \"{LanguageConstants.ExtendsKeyword}\" keyword requires enabling EXPERIMENTAL feature \"{nameof(ExperimentalFeaturesEnabled.ExtendableParamFiles)}\".");

            public Diagnostic MicrosoftGraphBuiltinRetired(ExtensionDeclarationSyntax? syntax)
            {
                var msGraphRegistryPath = "br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.9-preview";
                var codeFix = new CodeFix(
                    $"Replace built-in extension \'microsoftGraph\' with dynamic types registry path",
                    true,
                    CodeFixKind.QuickFix,
                    new CodeReplacement(syntax?.SpecificationString.Span ?? TextSpan, $"\'{msGraphRegistryPath}\'"));

                return CoreError(
                "BCP407",
                $"Built-in extension \"microsoftGraph\" is retired. Use dynamic types instead. See https://aka.ms/graphbicep/dynamictypes")
                with
                {
                    Fixes = [codeFix]
                };
            }

            public Diagnostic NameofInvalidOnUnnamedExpression() => CoreError(
                "BCP408",
                $"The \"{LanguageConstants.NameofFunctionName}\" function can only be used with an expression which has a name.");

            public Diagnostic ResourceParameterizedTypeIsDeprecated(ParameterizedTypeInstantiationSyntaxBase syntax)
            {
                var fixToResourceInput = new CodeFix(
                    $"Replace the 'resource<>' parameterized type with the 'resourceInput<>' parameterized type (for values that will be used in the right-hand side of a `resource` statement)",
                    true,
                    CodeFixKind.QuickFix,
                    new CodeReplacement(syntax.Name.Span, LanguageConstants.TypeNameResourceInput));

                var fixToResourceOutput = new CodeFix(
                    $"Replace the 'resource<>' parameterized type with the 'resourceOutput<>' parameterized type (for values that should match the value of a `resource` symbol after it has been declared)",
                    // we've encouraged users to adopt resource-derived types for when values will be passed to resource statements. Few if any existing usages should align with `resourceOutput<>`
                    isPreferred: false,
                    CodeFixKind.QuickFix,
                    new CodeReplacement(syntax.Name.Span, LanguageConstants.TypeNameResourceOutput));

                return CoreWarning(
                    "BCP409",
                    "The 'resource<>' parameterized type has been deprecated. Please specify whether you want this type to correspond to the resource input or the resource output.")
                    with
                { Fixes = [fixToResourceInput, fixToResourceOutput] };
            }

            public Diagnostic AttemptToDivideByZero() => CoreError("BCP410", "Division by zero is not supported.");

            public Diagnostic TypeExpressionResolvesToUnassignableType(TypeSymbol type) => CoreError(
                "BCP411",
                $"The type \"{type}\" cannot be used in a type assignment because it does not fit within one of ARM's primitive type categories (string, int, bool, array, object).{TypeInaccuracyClause}");

            public Diagnostic InvalidVariableType(IEnumerable<string> validTypes) => CoreError(
                "BCP412",
                $"The variable type is not valid. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic FromEndArrayAccessNotSupportedOnBaseType(TypeSymbol baseType) => CoreError(
                "BCP414",
                $"The \"^\" indexing operator cannot be used on base expressions of type \"{baseType}\".");

            public Diagnostic FromEndArrayAccessNotSupportedWithIndexType(TypeSymbol indexType) => CoreError(
                "BCP415",
                $"The \"^\" indexing operator cannot be used with index expressions of type \"{indexType}\".");

            public Diagnostic SuppliedStringDoesNotMatchExpectedPattern(bool shouldWarn, string expectedPattern)
                => CoreDiagnostic(
                    shouldWarn ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                    "BCP416",
                    $"The supplied string does not match the expected pattern of /${expectedPattern}/.");

            public Diagnostic SpreadOperatorCannotBeUsedWithForLoop(SpreadExpressionSyntax spread) => CoreError(
                "BCP417",
                $"The spread operator \"{spread.Ellipsis.Text}\" cannot be used inside objects with property for-expressions.");

            public Diagnostic ExtensionCannotBeReferenced() => CoreError(
                "BCP418",
                "Extensions cannot be referenced here. Extensions can only be referenced by module extension configurations.");

            public Diagnostic InvalidReservedImplicitExtensionNamespace(string name) => CoreError(
                "BCP419",
                $"Namespace name \"{name}\", and cannot be used an extension name.");


            public Diagnostic ScopeKindUnresolvableAtCompileTime() => CoreError(
                "BCP420",
                "The scope could not be resolved at compile time because the supplied expression is ambiguous or too complex. Scoping expressions must be reducible to a specific kind of scope without knowledge of parameter values.");

            public Diagnostic SecureOutputsNotSupportedWithLocalDeploy(string moduleName) => CoreError(
                "BCP421",
                $"""Module "{moduleName}" contains one or more secure outputs, which are not supported with "{LanguageConstants.TargetScopeKeyword}" set to "{LanguageConstants.TargetScopeTypeLocal}".""");

            public Diagnostic InstanceFunctionCallOnPossiblyNullBase(TypeSymbol baseType, SyntaxBase expression) => CoreWarning(
                "BCP422",
                $"A resource of type \"{baseType}\" may or may not exist when this function is called, which could cause the deployment to fail.")
                with
            { Fixes = [AsNonNullable(expression)] };

            public Diagnostic ExtensionAliasMustBeDefinedForInlinedRegistryExtensionDeclaration() => CoreError(
                "BCP423",
                "An extension alias must be defined for an extension declaration with an inlined registry reference.");

            public Diagnostic MissingExtensionConfigAssignments(IEnumerable<string> identifiers) => CoreError(
                "BCP424",
                $"The following extensions are declared in the Bicep file but are missing a configuration assignment in the params files: {ToQuotedString(identifiers)}.");

            public Diagnostic ExtensionConfigAssignmentDoesNotMatchToExtension(string identifier) => CoreError(
                "BCP425",
                $"The extension configuration assignment for \"{identifier}\" does not match an extension in the Bicep file.");

            public Diagnostic SecureOutputsOnlyAllowedOnDirectModuleReference() => CoreError(
                "BCP426",
                "Secure outputs may only be accessed via a direct module reference. Only non-sensitive outputs are supported when dereferencing a module indirectly via a variable or lambda.");

            public Diagnostic EnvironmentVariableDoesNotExist(string name, string? suggestion) => CoreError(
                "BCP427",
                $"Environment variable \"{name}\" does not exist and there's no default value set.{suggestion}");

            public Diagnostic DirectoryDoesNotExist(string relativePath) => CoreError(
                "BCP428",
                $"Directory \"{relativePath}\" does not exist or additional permissions are necessary to access it.");

            public Diagnostic ErrorOccuredBrowsingDirectory(string exceptionMessage) => CoreError(
                "BCP429",
                $"An error occured browsing directory. {exceptionMessage}");

            public Diagnostic FoundFileInsteadOfDirectory(string filePath) => CoreError(
                "BCP430",
                $"Unable to open directory at path \"{filePath}\". Found a file instead.");

            public Diagnostic InvalidModuleExtensionConfigAssignmentExpression(string propertyName) => CoreError(
                "BCP431",
                $"The value of the \"{propertyName}\" property must be an object literal or a valid extension config inheritance expression.");

            public Diagnostic RuntimeValueNotAllowedInFunctionArgument(string? functionName, string? parameterName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP432",
                    $"This expression is being used in parameter \"{parameterName ?? "unknown"}\" of the function \"{functionName ?? "unknown"}\", which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic InlinedResourcesCannotHaveExplicitDependencies(string symbolicName, IEnumerable<string> runtimePropertyNames) => CoreError(
                "BCP433",
                $"The resource \"{symbolicName}\" cannot declare explicit dependencies because its identifier properties including {ToQuotedString(runtimePropertyNames.OrderBy(x => x))} cannot be calculated at the start of the deployment.");

            public Diagnostic CannotExplicitlyDependOnInlinedResource(string dependentName, string dependencyName, IEnumerable<string> runtimePropertyNames) => CoreError(
                "BCP434",
                $"The resource \"{dependentName}\" cannot declare an explicit dependency on \"{dependencyName}\" because the identifier properties of the latter including {ToQuotedString(runtimePropertyNames.OrderBy(x => x))} cannot be calculated at the start of the deployment.");

            public Diagnostic UsingWithClauseRequiresExperimentalFeature() => CoreError(
                "BCP435",
                $"Using the \"{LanguageConstants.WithKeyword}\" keyword with a \"{LanguageConstants.UsingKeyword}\" statement requires enabling EXPERIMENTAL feature \"{nameof(ExperimentalFeaturesEnabled.DeployCommands)}\".");

            public Diagnostic ExpectedWithKeywordOrNewLine() => CoreError(
                "BCP436",
                $"Expected the \"with\" keyword or a new line character at this location.");

            public Diagnostic BaseIdentifierNotAvailableWithoutExtends() => CoreError(
                "BCP437",
                $"The identifier '{LanguageConstants.BaseIdentifier}' is only available in parameter files that declare an '{LanguageConstants.ExtendsKeyword}' clause.");

            public Diagnostic BaseIdentifierRedeclared() => CoreError(
                "BCP438",
                $"The identifier '{LanguageConstants.BaseIdentifier}' is reserved and cannot be declared.");

            public Diagnostic SecureDecoratorOnlyAllowedOnStringsAndObjects() => CoreError(
                "BCP439",
                "The @secure() decorator can only be used on statements whose type clause is \"string,\", \"object\", or a literal type.");

            public Diagnostic SecureDecoratorTargetMustFitWithinStringOrObject() => CoreError(
                "BCP440",
                "The @secure() decorator can only be used on statements whose type is a subtype of \"string\" or \"object\".");

            public Diagnostic CannotUseExistingWithWriteOnlyResource(ResourceTypeReference resourceTypeReference) => CoreError(
                "BCP441",
                $"Resource type \"{resourceTypeReference.FormatName()}\" cannot be used with the 'existing' keyword.");

            public Diagnostic MultilineStringRequiresExperimentalFeature() => CoreError(
                "BCP442",
                $"Using multiline string interpolation requires enabling EXPERIMENTAL feature \"{nameof(ExperimentalFeaturesEnabled.MultilineStringInterpolation)}\".");

            public Diagnostic UsingWithClauseRequiredIfExperimentalFeatureEnabled() => CoreError(
                "BCP443",
                $"""The "{LanguageConstants.UsingKeyword}" statement requires a "{LanguageConstants.WithKeyword}" clause if the EXPERIMENTAL feature "{nameof(ExperimentalFeaturesEnabled.DeployCommands)}" is enabled.""");

            public Diagnostic RuntimeValueNotAllowedInExtensionDeclarationWithClause(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return CoreError(
                    "BCP444",
                    $"This expression is being used as a default value for an extension configuration property, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }
        }

        public static DiagnosticBuilderInternal ForPosition(TextSpan span)
            => new(span);

        public static DiagnosticBuilderInternal ForPosition(IPositionable positionable)
            => new(positionable.Span);

        public static DiagnosticBuilderInternal ForDocumentStart()
            => new(TextSpan.TextDocumentStart);
    }
}
