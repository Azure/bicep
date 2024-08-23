// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Diagnostics
{
    public static class DiagnosticBuilder
    {
        public const string UseStringInterpolationInsteadClause = "Use string interpolation instead.";

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

            private static string BuildBicepConfigurationClause(Uri? configFileUri) => configFileUri is not null
                ? $"Bicep configuration \"{configFileUri.LocalPath}\""
                : $"built-in Bicep configuration";

            public Diagnostic UnrecognizedToken(string token) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP001",
                $"The following token is not recognized: \"{token}\".");

            public Diagnostic UnterminatedMultilineComment() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP002",
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.");

            public Diagnostic UnterminatedString() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP003",
                "The string at this location is not terminated. Terminate the string with a single quote character.");

            public Diagnostic UnterminatedStringWithNewLine() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP004",
                "The string at this location is not terminated due to an unexpected new line character.");

            public Diagnostic UnterminatedStringEscapeSequenceAtEof() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP005",
                "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.");

            public Diagnostic UnterminatedStringEscapeSequenceUnrecognized(IEnumerable<string> escapeSequences) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP006",
                $"The specified escape sequence is not recognized. Only the following escape sequences are allowed: {ToQuotedString(escapeSequences)}.");

            public Diagnostic UnrecognizedDeclaration() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP007",
                "This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration.");

            public Diagnostic ExpectedParameterContinuation() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP008",
                "Expected the \"=\" token, or a newline at this location.");

            public Diagnostic UnrecognizedExpression() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP009",
                "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.");

            public Diagnostic InvalidInteger() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP010",
                "Expected a valid 64-bit signed integer.");

            public Diagnostic InvalidType() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP011",
                "The type of the specified value is incorrect. Specify a string, boolean, or integer literal.");

            public Diagnostic ExpectedKeyword(string keyword) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP012",
                $"Expected the \"{keyword}\" keyword at this location.");

            public Diagnostic ExpectedParameterIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP013",
                "Expected a parameter identifier at this location.");

            public Diagnostic ExpectedVariableIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP015",
                "Expected a variable identifier at this location.");

            public Diagnostic ExpectedOutputIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP016",
                "Expected an output identifier at this location.");

            public Diagnostic ExpectedResourceIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP017",
                "Expected a resource identifier at this location.");

            public Diagnostic ExpectedCharacter(string character) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP018",
                $"Expected the \"{character}\" character at this location.");

            public Diagnostic ExpectedNewLine() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP019",
                "Expected a new line character at this location.");

            public Diagnostic ExpectedFunctionOrPropertyName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP020",
                "Expected a function or property name at this location.");

            public Diagnostic ExpectedNumericLiteral() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP021",
                "Expected a numeric literal at this location.");

            public Diagnostic ExpectedPropertyName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP022",
                "Expected a property name at this location.");

            public Diagnostic ExpectedVariableOrFunctionName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP023",
                "Expected a variable or function name at this location.");

            public Diagnostic IdentifierNameExceedsLimit() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP024",
                $"The identifier exceeds the limit of {LanguageConstants.MaxIdentifierLength}. Reduce the length of the identifier.");

            public Diagnostic PropertyMultipleDeclarations(string property) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP025",
                $"The property \"{property}\" is declared multiple times in this object. Remove or rename the duplicate properties.");

            public Diagnostic OutputTypeMismatch(TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP026",
                $"The output expects a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic IdentifierMultipleDeclarations(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP028",
                $"Identifier \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public Diagnostic InvalidResourceType() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP029",
                "The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\".");

            public Diagnostic InvalidOutputType(IEnumerable<string> validTypes) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP030",
                $"The output type is not valid. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic InvalidParameterType(IEnumerable<string> validTypes) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP031",
                $"The parameter type is not valid. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic CompileTimeConstantRequired() => new(
                TextSpan,
                DiagnosticLevel.Error,
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
                    return new Diagnostic(
                        TextSpan,
                        warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                        "BCP035",
                        $"The specified \"{blockName}\" declaration is missing the following required properties{sourceDeclarationClause}: {ToQuotedString(properties)}.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}",
                        showTypeInaccuracy ? TypeInaccuracyLink : null);
                }

                var codeFix = new CodeFix("Add required properties", true, CodeFixKind.QuickFix, new CodeReplacement(objectSyntax.Span, newSyntax.ToString()));

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

            public Diagnostic VariableTypeAssignmentDisallowed(TypeSymbol valueType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP041",
                $"Values of type \"{valueType}\" cannot be assigned to a variable.");

            public Diagnostic InvalidExpression() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP043",
                "This is not a valid expression.");

            public Diagnostic UnaryOperatorInvalidType(string operatorName, TypeSymbol type) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP044",
                $"Cannot apply operator \"{operatorName}\" to operand of type \"{type}\".");

            public Diagnostic BinaryOperatorInvalidType(string operatorName, TypeSymbol type1, TypeSymbol type2, string? additionalInfo) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP045",
                $"Cannot apply operator \"{operatorName}\" to operands of type \"{type1}\" and \"{type2}\".{(additionalInfo is null ? string.Empty : " " + additionalInfo)}");

            public Diagnostic ValueTypeMismatch(TypeSymbol type) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP046",
                $"Expected a value of type \"{type}\".");

            public Diagnostic ResourceTypeInterpolationUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
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

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP048",
                    message);
            }

            public Diagnostic StringOrIntegerIndexerRequired(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP049",
                $"The array index must be of type \"{LanguageConstants.String}\" or \"{LanguageConstants.Int}\" but the provided index was of type \"{wrongType}\".");

            public Diagnostic FilePathIsEmpty() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP050",
                "The specified path is empty.");

            public Diagnostic FilePathBeginsWithForwardSlash() => new(
                TextSpan,
                DiagnosticLevel.Error,
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

            public Diagnostic NoPropertiesAllowed(TypeSymbol type) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP054",
                $"The type \"{type}\" does not contain any properties.");

            public Diagnostic ObjectRequiredForPropertyAccess(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP055",
                $"Cannot access properties of type \"{wrongType}\". An \"{LanguageConstants.Object}\" type is required.");

            public Diagnostic AmbiguousSymbolReference(string name, IEnumerable<string> namespaces) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP056",
                $"The reference to name \"{name}\" is ambiguous because it exists in namespaces {ToQuotedString(namespaces)}. The reference must be fully-qualified.");

            public Diagnostic SymbolicNameDoesNotExist(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP057",
                $"The name \"{name}\" does not exist in the current context.");

            public Diagnostic SymbolicNameIsNotAFunction(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP059",
                $"The name \"{name}\" is not a function.");

            public Diagnostic VariablesFunctionNotSupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP060",
                $"The \"variables\" function is not supported. Directly reference variables by their symbolic names.");

            public Diagnostic ParametersFunctionNotSupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP061",
                $"The \"parameters\" function is not supported. Directly reference parameters by their symbolic names.");

            public Diagnostic ReferencedSymbolHasErrors(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP062",
                $"The referenced declaration with name \"{name}\" is not valid.");

            public Diagnostic SymbolicNameIsNotAVariableOrParameter(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP063",
                $"The name \"{name}\" is not a parameter, variable, resource or module.");

            public Diagnostic UnexpectedTokensInInterpolation() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP064",
                "Found unexpected tokens in interpolated expression.");

            public Diagnostic FunctionOnlyValidInParameterDefaults(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP065",
                $"Function \"{functionName}\" is not valid at this location. It can only be used as a parameter default value.");

            public Diagnostic FunctionOnlyValidInResourceBody(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP066",
                $"Function \"{functionName}\" is not valid at this location. It can only be used in resource declarations.");

            public Diagnostic ObjectRequiredForMethodAccess(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP067",
                $"Cannot call functions on type \"{wrongType}\". An \"{LanguageConstants.Object}\" type is required.");

            public Diagnostic ExpectedResourceTypeString() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP068",
                "Expected a resource type string. Specify a valid resource type of format \"<types>@<apiVersion>\".");

            public Diagnostic FunctionNotSupportedOperatorAvailable(string function, string @operator) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP069",
                $"The function \"{function}\" is not supported. Use the \"{@operator}\" operator instead.");

            public Diagnostic ArgumentTypeMismatch(TypeSymbol argumentType, TypeSymbol parameterType) => new(
                TextSpan,
                DiagnosticLevel.Error,
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

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP071",
                    $"Expected {expected}, but got {argumentCount}.");
            }

            public Diagnostic CannotReferenceSymbolInParamDefaultValue() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP072",
                "This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values.");

            public Diagnostic CannotAssignToReadOnlyProperty(bool warnInsteadOfError, string property, bool showTypeInaccuracy) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP073",
                $"The property \"{property}\" is read-only. Expressions cannot be assigned to read-only properties.{(showTypeInaccuracy ? TypeInaccuracyClause : string.Empty)}", showTypeInaccuracy ? TypeInaccuracyLink : null);

            public Diagnostic ArraysRequireIntegerIndex(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP074",
                $"Indexing over arrays requires an index of type \"{LanguageConstants.Int}\" but the provided index was of type \"{wrongType}\".");

            public Diagnostic ObjectsRequireStringIndex(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP075",
                $"Indexing over objects requires an index of type \"{LanguageConstants.String}\" but the provided index was of type \"{wrongType}\".");

            public Diagnostic IndexerRequiresObjectOrArray(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
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

            public Diagnostic CyclicExpressionSelfReference() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP079",
                "This expression is referencing its own declaration, which is not allowed.");

            public Diagnostic CyclicExpression(IEnumerable<string> cycle) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP080",
                $"The expression is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ResourceTypesUnavailable(ResourceTypeReference resourceTypeReference) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP081",
                $"Resource type \"{resourceTypeReference.FormatName()}\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.");

            public FixableDiagnostic SymbolicNameDoesNotExistWithSuggestion(string name, string suggestedName) => new(
                TextSpan,
                DiagnosticLevel.Error,
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

            public Diagnostic SymbolicNameCannotUseReservedNamespaceName(string name, IEnumerable<string> namespaces) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP084",
                $"The symbolic name \"{name}\" is reserved. Please use a different symbolic name. Reserved namespaces are {ToQuotedString(namespaces.OrderBy(ns => ns))}.");

            public Diagnostic FilePathContainsForbiddenCharacters(IEnumerable<char> forbiddenChars) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP085",
                $"The specified file path contains one ore more invalid path characters. The following are not permitted: {ToQuotedString(forbiddenChars.OrderBy(x => x).Select(x => x.ToString()))}.");

            public Diagnostic FilePathHasForbiddenTerminator(IEnumerable<char> forbiddenPathTerminatorChars) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP086",
                $"The specified file path ends with an invalid character. The following are not permitted: {ToQuotedString(forbiddenPathTerminatorChars.OrderBy(x => x).Select(x => x.ToString()))}.");

            public Diagnostic ComplexLiteralsNotAllowed() => new(
                TextSpan,
                DiagnosticLevel.Error,
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

            public Diagnostic ModulePathHasNotBeenSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP090",
                "This module declaration is missing a file path reference.");

            public Diagnostic ErrorOccurredReadingFile(string failureMessage) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP091",
                $"An error occurred reading file. {failureMessage}");

            public Diagnostic FilePathInterpolationUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP092",
                "String interpolation is not supported in file paths.");

            public Diagnostic FilePathCouldNotBeResolved(string filePath, string parentPath) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP093",
                $"File path \"{filePath}\" could not be resolved relative to \"{parentPath}\".");

            public Diagnostic CyclicModuleSelfReference() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP094",
                "This module references itself, which is not allowed.");

            public Diagnostic CyclicFile(IEnumerable<string> cycle) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP095",
                $"The file is involved in a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ExpectedModuleIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP096",
                "Expected a module identifier at this location.");

            public Diagnostic ExpectedModulePathString() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP097",
                "Expected a module path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep'");

            public Diagnostic FilePathContainsBackSlash() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP098",
                "The specified file path contains a \"\\\" character. Use \"/\" instead as the directory separator character.");

            public Diagnostic AllowedMustContainItems() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP099",
                $"The \"{LanguageConstants.ParameterAllowedPropertyName}\" array must contain one or more items.");

            public Diagnostic IfFunctionNotSupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP100",
                "The function \"if\" is not supported. Use the \"?:\" (ternary conditional) operator instead, e.g. condition ? ValueIfTrue : ValueIfFalse");

            public Diagnostic CreateArrayFunctionNotSupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP101",
                "The \"createArray\" function is not supported. Construct an array literal using [].");

            public Diagnostic CreateObjectFunctionNotSupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP102",
                "The \"createObject\" function is not supported. Construct an object literal using {}.");

            public Diagnostic DoubleQuoteToken(string token) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP103",
                $"The following token is not recognized: \"{token}\". Strings are defined using single quotes in bicep.");

            public Diagnostic ReferencedModuleHasErrors() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP104",
                $"The referenced module has errors.");

            public Diagnostic UnableToLoadNonFileUri(Uri fileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP105",
                $"Unable to load file from URI \"{fileUri}\".");

            public Diagnostic UnexpectedCommaSeparator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP106",
                "Expected a new line character at this location. Commas are not used as separator delimiters.");

            public Diagnostic FunctionDoesNotExistInNamespace(Symbol namespaceType, string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP107",
                $"The function \"{name}\" does not exist in namespace \"{namespaceType.Name}\".");

            public FixableDiagnostic FunctionDoesNotExistInNamespaceWithSuggestion(Symbol namespaceType, string name, string suggestedName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP108",
                $"The function \"{name}\" does not exist in namespace \"{namespaceType.Name}\". Did you mean \"{suggestedName}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName)));

            public Diagnostic FunctionDoesNotExistOnObject(TypeSymbol type, string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP109",
                $"The type \"{type}\" does not contain function \"{name}\".");

            public FixableDiagnostic FunctionDoesNotExistOnObjectWithSuggestion(TypeSymbol type, string name, string suggestedName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP110",
                $"The type \"{type}\" does not contain function \"{name}\". Did you mean \"{suggestedName}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{suggestedName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, suggestedName)));

            public Diagnostic FilePathContainsControlChars() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP111",
                $"The specified file path contains invalid control code characters.");

            public Diagnostic TargetScopeMultipleDeclarations() => new(
                TextSpan,
                DiagnosticLevel.Error,
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

            public Diagnostic EmptyIndexerNotAllowed() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP117",
                "An empty indexer is not allowed. Specify a valid expression."
            );

            public Diagnostic ExpectBodyStartOrIfOrLoopStart() => new(
                TextSpan,
                DiagnosticLevel.Error,
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

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP120",
                    $"This expression is being used in an assignment to the \"{propertyName}\" property of the \"{objectTypeName}\" type, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic ResourceMultipleDeclarations(IEnumerable<string> resourceNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP121",
                $"Resources: {ToQuotedString(resourceNames)} are defined with this same name in a file. Rename them or split into different modules.");

            public Diagnostic ModuleMultipleDeclarations(IEnumerable<string> moduleNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP122",
                $"Modules: {ToQuotedString(moduleNames)} are defined with this same name and this same scope in a file. Rename them or split into different modules.");

            public Diagnostic ExpectedNamespaceOrDecoratorName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP123",
                "Expected a namespace or decorator name at this location.");

            public Diagnostic CannotAttachDecoratorToTarget(string decoratorName, TypeSymbol attachableType, TypeSymbol targetType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP124",
                $"The decorator \"{decoratorName}\" can only be attached to targets of type \"{attachableType}\", but the target has type \"{targetType}\".");

            public Diagnostic CannotUseFunctionAsParameterDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP125",
                $"Function \"{functionName}\" cannot be used as a parameter decorator.");

            public Diagnostic CannotUseFunctionAsVariableDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP126",
                $"Function \"{functionName}\" cannot be used as a variable decorator.");

            public Diagnostic CannotUseFunctionAsResourceDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP127",
                $"Function \"{functionName}\" cannot be used as a resource decorator.");

            public Diagnostic CannotUseFunctionAsModuleDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP128",
                $"Function \"{functionName}\" cannot be used as a module decorator.");

            public Diagnostic CannotUseFunctionAsOutputDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP129",
                $"Function \"{functionName}\" cannot be used as an output decorator.");

            public Diagnostic DecoratorsNotAllowed() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP130",
                "Decorators are not allowed here.");

            public Diagnostic ExpectedDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP132",
                "Expected a declaration after the decorator.");

            public Diagnostic InvalidUnicodeEscape() => new(
                TextSpan,
                DiagnosticLevel.Error,
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

            public Diagnostic ExpectedLoopVariableIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP136",
                "Expected a loop item variable identifier at this location.");

            public Diagnostic LoopArrayExpressionTypeMismatch(TypeSymbol actualType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP137",
                $"Loop expected an expression of type \"{LanguageConstants.Array}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic ForExpressionsNotSupportedHere() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP138",
                "For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties.");

            public Diagnostic InvalidCrossResourceScope() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP139",
                $"A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope.");

            public Diagnostic UnterminatedMultilineString() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP140",
                $"The multi-line string at this location is not terminated. Terminate it with \"'''\".");

            public Diagnostic ExpressionNotCallable() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP141",
                "The expression cannot be used as a decorator as it is not callable.");

            public Diagnostic TooManyPropertyForExpressions() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP142",
                "Property value for-expressions cannot be nested.");

            public Diagnostic ExpressionedPropertiesNotAllowedWithLoops() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP143",
                "For-expressions cannot be used with properties whose names are also expressions.");

            public Diagnostic DirectAccessToCollectionNotSupported(IEnumerable<string>? accessChain = null)
            {
                var accessChainClause = accessChain?.Any() ?? false
                    ? $"The collection was accessed by the chain of \"{string.Join("\" -> \"", accessChain)}\". "
                    : "";

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP144",
                    $"Directly referencing a resource or module collection is not currently supported here. {accessChainClause}Apply an array indexer to the expression.");
            }

            public Diagnostic OutputMultipleDeclarations(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP145",
                $"Output \"{identifier}\" is declared multiple times. Remove or rename the duplicates.");

            public Diagnostic ExpectedParameterDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP147",
                "Expected a parameter declaration after the decorator.");

            public Diagnostic ExpectedVariableDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP148",
                "Expected a variable declaration after the decorator.");

            public Diagnostic ExpectedResourceDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP149",
                "Expected a resource declaration after the decorator.");

            public Diagnostic ExpectedModuleDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP150",
                "Expected a module declaration after the decorator.");

            public Diagnostic ExpectedOutputDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP151",
                "Expected an output declaration after the decorator.");

            public Diagnostic CannotUseFunctionAsDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP152",
                $"Function \"{functionName}\" cannot be used as a decorator.");

            public Diagnostic ExpectedResourceOrModuleDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP153",
                "Expected a resource or module declaration after the decorator.");

            public Diagnostic BatchSizeTooSmall(long value, long limit) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP154",
                $"Expected a batch size of at least {limit} but the specified value was \"{value}\".");

            public Diagnostic BatchSizeNotAllowed(string decoratorName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP155",
                $"The decorator \"{decoratorName}\" can only be attached to resource or module collections.");

            public Diagnostic InvalidResourceTypeSegment(string typeSegment) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP156",
                $"The resource type segment \"{typeSegment}\" is invalid. Nested resources must specify a single type segment, and optionally can specify an api version using the format \"<type>@<apiVersion>\".");

            public Diagnostic InvalidAncestorResourceType() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP157",
                $"The resource type cannot be determined due to an error in the containing resource.");

            public Diagnostic ResourceRequiredForResourceAccess(string wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP158",
                $"Cannot access nested resources of type \"{wrongType}\". A resource type is required.");

            public Diagnostic NestedResourceNotFound(string resourceName, string identifierName, IEnumerable<string> nestedResourceNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP159",
                $"The resource \"{resourceName}\" does not contain a nested resource named \"{identifierName}\". Known nested resources are: {ToQuotedString(nestedResourceNames)}.");

            public Diagnostic NestedResourceNotAllowedInLoop() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP160",
                $"A nested resource cannot appear inside of a resource with a for-expression.");

            public Diagnostic ExpectedLoopItemIdentifierOrVariableBlockStart() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP162",
                "Expected a loop item variable identifier or \"(\" at this location.");

            public Diagnostic ScopeUnsupportedOnChildResource() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP164",
                $"A child resource's scope is computed based on the scope of its ancestor resource. This means that using the \"{LanguageConstants.ResourceScopePropertyName}\" property on a child resource is unsupported.");

            public Diagnostic ScopeDisallowedForAncestorResource(string ancestorIdentifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP165",
                $"A resource's computed scope must match that of the Bicep file for it to be deployable. This resource's scope is computed from the \"{LanguageConstants.ResourceScopePropertyName}\" property value assigned to ancestor resource \"{ancestorIdentifier}\". You must use modules to deploy resources to a different scope.");

            public Diagnostic DuplicateDecorator(string decoratorName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP166",
                $"Duplicate \"{decoratorName}\" decorator.");

            public Diagnostic ExpectBodyStartOrIf() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP167",
                "Expected the \"{\" character or the \"if\" keyword at this location.");

            public Diagnostic LengthMustNotBeNegative() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP168",
                $"Length must not be a negative value.");

            public Diagnostic TopLevelChildResourceNameIncorrectQualifierCount(int expectedSlashCount) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP169",
                $"Expected resource name to contain {expectedSlashCount} \"/\" character(s). The number of name segments must match the number of segments in the resource type.");

            public Diagnostic ChildResourceNameContainsQualifiers() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP170",
                $"Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name.");

            public Diagnostic ResourceTypeIsNotValidParent(string resourceType, string parentResourceType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP171",
                $"Resource type \"{resourceType}\" is not a valid child resource of parent \"{parentResourceType}\".");

            public Diagnostic ParentResourceTypeHasErrors(string resourceName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP172",
                $"The resource type cannot be validated due to an error in parent resource \"{resourceName}\".");

            public Diagnostic CannotUsePropertyInExistingResource(string property) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP173",
                $"The property \"{property}\" cannot be used in an existing resource declaration.");

            public Diagnostic ResourceTypeContainsProvidersSegment() => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP174",
                $"Type validation is not available for resource types declared containing a \"/providers/\" segment. Please instead use the \"scope\" property.",
                new Uri("https://aka.ms/BicepScopes"));

            public Diagnostic AnyTypeIsNotAllowed() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP176",
                $"Values of the \"any\" type are not allowed here.");

            public Diagnostic RuntimeValueNotAllowedInIfConditionExpression(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP177",
                    $"This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic RuntimeValueNotAllowedInForExpression(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
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

            public Diagnostic FunctionOnlyValidInModuleSecureParameterAssignment(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP180",
                $"Function \"{functionName}\" is not valid at this location. It can only be used when directly assigning to a module parameter with a secure decorator.");

            public Diagnostic RuntimeValueNotAllowedInRunTimeFunctionArguments(string functionName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP181",
                    $"This expression is being used in an argument of the function \"{functionName}\", which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic RuntimeValueNotAllowedInVariableForBody(string variableName, string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain, string? violatingPropertyName)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var violatingPropertyNameClause = BuildNonDeployTimeConstantPropertyClause(accessedSymbolName, violatingPropertyName);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP182",
                    $"This expression is being used in the for-body of the variable \"{variableName}\", which requires values that can be calculated at the start of the deployment.{variableDependencyChainClause}{violatingPropertyNameClause}{accessiblePropertiesClause}");
            }

            public Diagnostic ModuleParametersPropertyRequiresObjectLiteral() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP183",
                $"The value of the module \"{LanguageConstants.ModuleParamsPropertyName}\" property must be an object literal.");

            public Diagnostic FileExceedsMaximumSize(string filePath, long maxSize, string unit) => new(
               TextSpan,
               DiagnosticLevel.Error,
               "BCP184",
               $"File '{filePath}' exceeded maximum size of {maxSize} {unit}.");

            public Diagnostic FileEncodingMismatch(string detectedEncoding) => new(
               TextSpan,
               DiagnosticLevel.Info,
               "BCP185",
               $"Encoding mismatch. File was loaded with '{detectedEncoding}' encoding.");

            public Diagnostic UnparsableJsonType() => new(
               TextSpan,
               DiagnosticLevel.Error,
               "BCP186",
               $"Unable to parse literal JSON value. Please ensure that it is well-formed.");

            public Diagnostic FallbackPropertyUsed(string property) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP187",
                $"The property \"{property}\" does not exist in the resource or type definition, although it might still be valid.{TypeInaccuracyClause}", TypeInaccuracyLink);

            public Diagnostic ReferencedArmTemplateHasErrors() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP188",
                $"The referenced ARM template has errors. Please see https://aka.ms/arm-template for information on how to diagnose and fix the template.");

            public Diagnostic UnknownModuleReferenceScheme(string badScheme, ImmutableArray<string> allowedSchemes)
            {
                string FormatSchemes() => ToQuotedString(allowedSchemes.Where(scheme => !string.Equals(scheme, ArtifactReferenceSchemes.Local)));

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
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
            public Diagnostic ArtifactRequiresRestore(string artifactRef) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP190",
                $"The artifact with reference \"{artifactRef}\" has not been restored.");

            public Diagnostic ArtifactRestoreFailed(string artifactRef) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP191",
                $"Unable to restore the artifact with reference \"{artifactRef}\".");

            public Diagnostic ArtifactRestoreFailedWithMessage(string artifactRef, string message) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP192",
                $"Unable to restore the artifact with reference \"{artifactRef}\": {message}");

            public Diagnostic InvalidOciArtifactReference(string? aliasName, string badRef) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP193",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} Specify a reference in the format of \"{ArtifactReferenceSchemes.Oci}:<artifact-uri>:<tag>\", or \"{ArtifactReferenceSchemes.Oci}/<module-alias>:<module-name-or-path>:<tag>\".");

            public Diagnostic InvalidTemplateSpecReference(string? aliasName, string badRef) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP194",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, badRef)} Specify a reference in the format of \"{ArtifactReferenceSchemes.TemplateSpecs}:<subscription-ID>/<resource-group-name>/<template-spec-name>:<version>\", or \"{ArtifactReferenceSchemes.TemplateSpecs}/<module-alias>:<template-spec-name>:<version>\".");

            public Diagnostic InvalidOciArtifactReferenceInvalidPathSegment(string? aliasName, string badRef, string badSegment) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP195",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The artifact path segment \"{badSegment}\" is not valid. Each artifact name path segment must be a lowercase alphanumeric string optionally separated by a \".\", \"_\" , or \"-\".");

            public Diagnostic InvalidOciArtifactReferenceMissingTagOrDigest(string? aliasName, string badRef) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP196",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The module tag or digest is missing.");

            public Diagnostic InvalidOciArtifactReferenceTagTooLong(string? aliasName, string badRef, string badTag, int maxLength) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP197",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The tag \"{badTag}\" exceeds the maximum length of {maxLength} characters.");

            public Diagnostic InvalidOciArtifactReferenceInvalidTag(string? aliasName, string badRef, string badTag) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP198",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The tag \"{badTag}\" is not valid. Valid characters are alphanumeric, \".\", \"_\", or \"-\" but the tag cannot begin with \".\", \"_\", or \"-\".");

            public Diagnostic InvalidOciArtifactReferenceRepositoryTooLong(string? aliasName, string badRef, string badRepository, int maxLength) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP199",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} Module path \"{badRepository}\" exceeds the maximum length of {maxLength} characters.");

            public Diagnostic InvalidOciArtifactReferenceRegistryTooLong(string? aliasName, string badRef, string badRegistry, int maxLength) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP200",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The registry \"{badRegistry}\" exceeds the maximum length of {maxLength} characters.");

            public Diagnostic ExpectedExtensionSpecification()
            {
                var message = """
                Expected an extension specification string with a valid format at this location. Valid formats:
                * "br:<extensionRegistryHost>/<extensionRepositoryPath>:<extensionVersion>"
                * "br/<extensionAlias>:<extensionName>:<extensionVersion>"
                """;
                return new(TextSpan, DiagnosticLevel.Error, "BCP201", message);
            }

            public Diagnostic ExpectedExtensionAliasName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP202",
                "Expected an extension alias name at this location.");

            public Diagnostic ExtensionsAreDisabled() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP203",
                $@"Using extension declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.Extensibility)}"".");

            public Diagnostic UnrecognizedExtension(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP204",
                $"Extension \"{identifier}\" is not recognized.");

            public Diagnostic ExtensionDoesNotSupportConfiguration(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP205",
                $"Extension \"{identifier}\" does not support configuration.");

            public Diagnostic ExtensionRequiresConfiguration(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP206",
                $"Extension \"{identifier}\" requires configuration, but none was provided.");

            public Diagnostic NamespaceMultipleDeclarations(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP207",
                $"Namespace \"{identifier}\" is declared multiple times. Remove the duplicates.");

            public Diagnostic UnknownResourceReferenceScheme(string badNamespace, IEnumerable<string> allowedNamespaces) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP208",
                $"The specified namespace \"{badNamespace}\" is not recognized. Specify a resource reference using one of the following namespaces: {ToQuotedString(allowedNamespaces)}.");

            public Diagnostic FailedToFindResourceTypeInNamespace(string @namespace, string resourceType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP209",
                $"Failed to find resource type \"{resourceType}\" in namespace \"{@namespace}\".");

            public Diagnostic ParentResourceInDifferentNamespace(string childNamespace, string parentNamespace) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP210",
                $"Resource type belonging to namespace \"{childNamespace}\" cannot have a parent resource type belonging to different namespace \"{parentNamespace}\".");

            public Diagnostic InvalidModuleAliasName(string aliasName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP211",
                $"The module alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");

            public Diagnostic TemplateSpecModuleAliasNameDoesNotExistInConfiguration(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP212",
                $"The Template Spec module alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configFileUri)}.");

            public Diagnostic OciArtifactModuleAliasNameDoesNotExistInConfiguration(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP213",
                $"The OCI artifact module alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configFileUri)}.");

            public Diagnostic InvalidTemplateSpecAliasSubscriptionNullOrUndefined(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP214",
                $"The Template Spec module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is in valid. The \"subscription\" property cannot be null or undefined.");

            public Diagnostic InvalidTemplateSpecAliasResourceGroupNullOrUndefined(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP215",
                $"The Template Spec module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is in valid. The \"resourceGroup\" property cannot be null or undefined.");

            public Diagnostic InvalidOciArtifactModuleAliasRegistryNullOrUndefined(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP216",
                $"The OCI artifact module alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is invalid. The \"registry\" property cannot be null or undefined.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidSubscriptionId(string? aliasName, string subscriptionId, string referenceValue) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP217",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The subscription ID \"{subscriptionId}\" in is not a GUID.");

            public Diagnostic InvalidTemplateSpecReferenceResourceGroupNameTooLong(string? aliasName, string resourceGroupName, string referenceValue, int maximumLength) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP218",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The resource group name \"{resourceGroupName}\" exceeds the maximum length of {maximumLength} characters.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidResourceGroupName(string? aliasName, string resourceGroupName, string referenceValue) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP219",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The resource group name \"{resourceGroupName}\" is invalid. Valid characters are alphanumeric, unicode characters, \".\", \"_\", \"-\", \"(\", or \")\", but the resource group name cannot end with \".\".");

            public Diagnostic InvalidTemplateSpecReferenceTemplateSpecNameTooLong(string? aliasName, string templateSpecName, string referenceValue, int maximumLength) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP220",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec name \"{templateSpecName}\" exceeds the maximum length of {maximumLength} characters.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidTemplateSpecName(string? aliasName, string templateSpecName, string referenceValue) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP221",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec name \"{templateSpecName}\" is invalid. Valid characters are alphanumeric, \".\", \"_\", \"-\", \"(\", or \")\", but the Template Spec name cannot end with \".\".");

            public Diagnostic InvalidTemplateSpecReferenceTemplateSpecVersionTooLong(string? aliasName, string templateSpecVersion, string referenceValue, int maximumLength) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP222",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec version \"{templateSpecVersion}\" exceeds the maximum length of {maximumLength} characters.");

            public Diagnostic InvalidTemplateSpecReferenceInvalidTemplateSpecVersion(string? aliasName, string templateSpecVersion, string referenceValue) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP223",
                $"{BuildInvalidTemplateSpecReferenceClause(aliasName, referenceValue)} The Template Spec version \"{templateSpecVersion}\" is invalid. Valid characters are alphanumeric, \".\", \"_\", \"-\", \"(\", or \")\", but the Template Spec name cannot end with \".\".");

            public Diagnostic InvalidOciArtifactReferenceInvalidDigest(string? aliasName, string badRef, string badDigest) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP224",
                $"{BuildInvalidOciArtifactReferenceClause(aliasName, badRef)} The digest \"{badDigest}\" is not valid. The valid format is a string \"sha256:\" followed by exactly 64 lowercase hexadecimal digits.");

            public Diagnostic AmbiguousDiscriminatorPropertyValue(string propertyName) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP225",
                $"The discriminator property \"{propertyName}\" value cannot be determined at compilation time. Type checking for this object is disabled.");

            public Diagnostic MissingDiagnosticCodes() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP226",
                "Expected at least one diagnostic code at this location. Valid format is \"#disable-next-line diagnosticCode1 diagnosticCode2 ...\""
            );

            public Diagnostic UnsupportedResourceTypeParameterOrOutputType(string resourceType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP227",
                $"The type \"{resourceType}\" cannot be used as a parameter or output type. Extensibility types are currently not supported as parameters or outputs.");

            public Diagnostic InvalidResourceScopeCannotBeResourceTypeParameter(string parameterName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP229",
                $"The parameter \"{parameterName}\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource.");

            public Diagnostic ModuleParamOrOutputResourceTypeUnavailable(ResourceTypeReference resourceTypeReference) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP230",
                $"The referenced module uses resource type \"{resourceTypeReference.FormatName()}\" which does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.");

            public Diagnostic ParamOrOutputResourceTypeUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP231",
                $@"Using resource-typed parameters and outputs requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.ResourceTypedParamsAndOutputs)}"".");

            public Diagnostic ArtifactDeleteFailed(string moduleRef) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP232",
                $"Unable to delete the module with reference \"{moduleRef}\" from cache.");

            public Diagnostic ArtifactDeleteFailedWithMessage(string moduleRef, string message) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP233",
                $"Unable to delete the module with reference \"{moduleRef}\" from cache: {message}");

            public Diagnostic ArmFunctionLiteralTypeConversionFailedWithMessage(string literalValue, string armFunctionName, string message) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP234",
                $"The ARM function \"{armFunctionName}\" failed when invoked on the value [{literalValue}]: {message}");

            public Diagnostic NoJsonTokenOnPathOrPathInvalid() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP235",
                $"Specified JSONPath does not exist in the given file or is invalid.");

            public Diagnostic ExpectedNewLineOrCommaSeparator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP236",
                "Expected a new line or comma character at this location.");

            public Diagnostic ExpectedCommaSeparator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP237",
                "Expected a comma character at this location.");

            public Diagnostic UnexpectedNewLineAfterCommaSeparator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP238",
                "Unexpected new line character after a comma.");

            public Diagnostic ReservedIdentifier(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP239",
                $"Identifier \"{name}\" is a reserved Bicep symbol name and cannot be used in this context.");

            public Diagnostic InvalidValueForParentProperty() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP240",
                "The \"parent\" property only permits direct references to resources. Expressions are not supported.");

            public Diagnostic DeprecatedProvidersFunction(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP241",
                $"The \"{functionName}\" function is deprecated and will be removed in a future release of Bicep. Please add a comment to https://github.com/Azure/bicep/issues/2017 if you believe this will impact your workflow.",
                styling: DiagnosticStyling.ShowCodeDeprecated);

            public Diagnostic LambdaFunctionsOnlyValidInFunctionArguments() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP242",
                $"Lambda functions may only be specified directly as function arguments.");

            public Diagnostic ParenthesesMustHaveExactlyOneItem() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP243",
                "Parentheses must contain exactly one expression.");

            public Diagnostic LambdaExpectedArgCountMismatch(TypeSymbol lambdaType, int minArgCount, int maxArgCount, int actualArgCount) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP244",
                minArgCount == maxArgCount ?
                    $"Expected lambda expression of type \"{lambdaType}\" with {minArgCount} arguments but received {actualArgCount} arguments." :
                    $"Expected lambda expression of type \"{lambdaType}\" with between {minArgCount} and {maxArgCount} arguments but received {actualArgCount} arguments.");

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

            public Diagnostic LambdaVariablesInResourceOrModuleArrayAccessUnsupported(IEnumerable<string> variableNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP247",
                $"Using lambda variables inside resource or module array access is not currently supported."
                    + $" Found the following lambda variable(s) being accessed: {ToQuotedString(variableNames)}.");

            public Diagnostic LambdaVariablesInInlineFunctionUnsupported(string functionName, IEnumerable<string> variableNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP248",
                $"Using lambda variables inside the \"{functionName}\" function is not currently supported."
                    + $" Found the following lambda variable(s) being accessed: {ToQuotedString(variableNames)}.");

            public Diagnostic ExpectedLoopVariableBlockWith2Elements(int actualCount) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP249",
                $"Expected loop variable block to consist of exactly 2 elements (item variable and index variable), but found {actualCount}.");

            public Diagnostic ParameterMultipleAssignments(string identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP250",
                $"Parameter \"{identifier}\" is assigned multiple times. Remove or rename the duplicates.");

            public Diagnostic UsingPathHasNotBeenSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP256",
                "The using declaration is missing a bicep template file path reference.");

            public Diagnostic ExpectedFilePathString() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP257",
                "Expected a Bicep file path string. This should be a relative path to another bicep file, e.g. 'myModule.bicep' or '../parent/myModule.bicep'");

            public IDiagnostic MissingParameterAssignment(IEnumerable<string> identifiers, CodeFix insertMissingCodefix)
            {
                return new FixableDiagnostic(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP258",
                    $"The following parameters are declared in the Bicep file but are missing an assignment in the params file: {ToQuotedString(identifiers)}.",
                    documentationUri: null,
                    DiagnosticStyling.Default,
                    insertMissingCodefix);
            }

            public Diagnostic MissingParameterDeclaration(string? identifier) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP259",
                $"The parameter \"{identifier}\" is assigned in the params file without being declared in the Bicep file.");

            public Diagnostic ParameterTypeMismatch(string? identifier, TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP260",
                $"The parameter \"{identifier}\" expects a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".");

            public Diagnostic UsingDeclarationNotSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP261",
                "A using declaration must be present in this parameters file.");

            public Diagnostic MoreThanOneUsingDeclarationSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP262",
                "More than one using declaration are present");

            public Diagnostic UsingDeclarationReferencesInvalidFile() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP263",
                "The file specified in the using declaration path does not exist");

            public Diagnostic AmbiguousResourceTypeBetweenImports(string resourceTypeName, IEnumerable<string> namespaces) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP264",
                $"Resource type \"{resourceTypeName}\" is declared in multiple imported namespaces ({ToQuotedStringWithCaseInsensitiveOrdering(namespaces)}), and must be fully-qualified.");

            public FixableDiagnostic SymbolicNameShadowsAKnownFunction(string name, string knownFunctionNamespace, string knownFunctionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP265",
                $"The name \"{name}\" is not a function. Did you mean \"{knownFunctionNamespace}.{knownFunctionName}\"?",
                null,
                DiagnosticStyling.Default,
                new CodeFix($"Change \"{name}\" to \"{knownFunctionNamespace}.{knownFunctionName}\"", true, CodeFixKind.QuickFix, CodeManipulator.Replace(TextSpan, $"{knownFunctionNamespace}.{knownFunctionName}")));

            public Diagnostic ExpectedMetadataIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP266",
                "Expected a metadata identifier at this location.");

            public Diagnostic ExpectedMetadataDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP267",
                "Expected an metadata declaration after the decorator.");

            public Diagnostic ReservedMetadataIdentifier(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP268",
                $"Invalid identifier: \"{name}\". Metadata identifiers starting with '_' are reserved. Please use a different identifier.");

            public Diagnostic CannotUseFunctionAsMetadataDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP269",
                $"Function \"{functionName}\" cannot be used as a metadata decorator.");

            public Diagnostic UnparsableBicepConfigFile(string configurationPath, string parsingErrorMessage) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP271",
                $"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: {parsingErrorMessage.TrimEnd('.')}.");

            public Diagnostic UnloadableBicepConfigFile(string configurationPath, string loadErrorMessage) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP272",
                $"Could not load the Bicep configuration file \"{configurationPath}\": {loadErrorMessage.TrimEnd('.')}.");

            public Diagnostic InvalidBicepConfigFile(string configurationPath, string parsingErrorMessage) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP273",
                $"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\": {parsingErrorMessage.TrimEnd('.')}.");

            public Diagnostic PotentialConfigDirectoryCouldNotBeScanned(string? directoryPath, string scanErrorMessage) => new(
                TextSpan,
                DiagnosticLevel.Info, // should this be a warning instead?
                "BCP274",
                $"Error scanning \"{directoryPath}\" for bicep configuration: {scanErrorMessage.TrimEnd('.')}.");

            public Diagnostic FoundDirectoryInsteadOfFile(string directoryPath) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP275",
                $"Unable to open file at path \"{directoryPath}\". Found a directory instead.");

            public Diagnostic UsingDeclarationMustReferenceBicepFile() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP276",
                "A using declaration can only reference a Bicep file.");

            public Diagnostic ModuleDeclarationMustReferenceBicepModule() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP277",
                "A module declaration can only reference a Bicep File, an ARM template, a registry reference or a template spec reference.");

            public Diagnostic CyclicParametersSelfReference() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP278",
                "This parameters file references itself, which is not allowed.");

            public Diagnostic UnrecognizedTypeExpression() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP279",
                $"Expected a type at this location. Please specify a valid type expression or one of the following types: {ToQuotedString(LanguageConstants.DeclarationTypes.Keys)}.");

            public Diagnostic TypeExpressionLiteralConversionFailed() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP285",
                "The type expression could not be reduced to a literal value.");

            public Diagnostic InvalidUnionTypeMember(string keystoneType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP286",
                $"This union member is invalid because it cannot be assigned to the '{keystoneType}' type.");

            public Diagnostic ValueSymbolUsedAsType(string symbolName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP287",
                // TODO: Add "Did you mean 'typeof({symbolName})'?" When support for typeof has been added.
                $"'{symbolName}' refers to a value but is being used as a type here.");

            public Diagnostic TypeSymbolUsedAsValue(string symbolName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP288",
                $"'{symbolName}' refers to a type but is being used as a value here.");

            public Diagnostic InvalidTypeDefinition() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP289",
                $"The type definition is not valid.");

            public Diagnostic ExpectedParameterOrTypeDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP290",
                "Expected a parameter or type declaration after the decorator.");

            public Diagnostic ExpectedParameterOrOutputDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP291",
                "Expected a parameter or output declaration after the decorator.");

            public Diagnostic ExpectedParameterOutputOrTypeDeclarationAfterDecorator() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP292",
                "Expected a parameter, output, or type declaration after the decorator.");

            public Diagnostic NonLiteralUnionMember() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP293",
                "All members of a union type declaration must be literal values.");

            public Diagnostic InvalidTypeUnion() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP294",
                "Type unions must be reducible to a single ARM type (such as 'string', 'int', or 'bool').");

            public Diagnostic DecoratorNotPermittedOnLiteralType(string decoratorName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP295",
                $"The '{decoratorName}' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically.");

            public Diagnostic NonConstantTypeProperty() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP296",
                "Property names on types must be compile-time constant values.");

            public Diagnostic CannotUseFunctionAsTypeDecorator(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP297",
                $"Function \"{functionName}\" cannot be used as a type decorator.");

            public Diagnostic CyclicTypeSelfReference() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP298",
                "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled.");

            public Diagnostic CyclicType(IEnumerable<string> cycle) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP299",
                $"This type definition includes itself as a required component via a cycle (\"{string.Join("\" -> \"", cycle)}\").");

            public Diagnostic ExpectedTypeLiteral() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP300",
                $"Expected a type literal at this location. Please specify a concrete value or a reference to a literal type.");

            public Diagnostic ReservedTypeName(string reservedName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP301",
                $@"The type name ""{reservedName}"" is reserved and may not be attached to a user-defined type.");

            public Diagnostic SymbolicNameIsNotAType(string name, IEnumerable<string> validTypes) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP302",
                $@"The name ""{name}"" is not a valid type. Please specify one of the following types: {ToQuotedString(validTypes)}.");

            public Diagnostic ExtensionSpecificationInterpolationUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP303",
                "String interpolation is unsupported for specifying the extension.");

            public Diagnostic ExpectedWithOrAsKeywordOrNewLine() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP305",
                $"Expected the \"with\" keyword, \"as\" keyword, or a new line character at this location.");

            public Diagnostic NamespaceSymbolUsedAsType(string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP306",
                $@"The name ""{name}"" refers to a namespace, not to a type.");

            public Diagnostic NestedRuntimePropertyAccessNotSupported(string? resourceSymbol, IEnumerable<string> runtimePropertyNames, IEnumerable<string> accessiblePropertyNames, IEnumerable<string> accessibleFunctionNames)
            {
                var accessiblePropertyNamesClause = accessiblePropertyNames.Any() ? @$" the accessible properties of ""{resourceSymbol}"" include {ToQuotedString(accessiblePropertyNames.OrderBy(x => x))}." : "";
                var accessibleFunctionNamesClause = accessibleFunctionNames.Any() ? @$" The accessible functions of ""{resourceSymbol}"" include {ToQuotedString(accessibleFunctionNames.OrderBy(x => x))}." : "";

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP307",
                    $"The expression cannot be evaluated, because the identifier properties of the referenced existing resource including {ToQuotedString(runtimePropertyNames.OrderBy(x => x))} cannot be calculated at the start of the deployment. In this situation,{accessiblePropertyNamesClause}{accessibleFunctionNamesClause}");
            }

            public Diagnostic DecoratorMayNotTargetTypeAlias(string decoratorName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP308",
                $@"The decorator ""{decoratorName}"" may not be used on statements whose declared type is a reference to a user-defined type.");

            public Diagnostic ValueCannotBeFlattened(TypeSymbol flattenInputType, TypeSymbol incompatibleType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP309",
                $@"Values of type ""{flattenInputType.Name}"" cannot be flattened because ""{incompatibleType.Name}"" is not an array type.");

            public Diagnostic IndexOutOfBounds(string typeName, long tupleLength, long indexSought)
            {
                var message = new StringBuilder("The provided index value of \"").Append(indexSought).Append("\" is not valid for type \"").Append(typeName).Append("\".");
                if (tupleLength > 0)
                {
                    message.Append(" Indexes for this type must be between 0 and ").Append(tupleLength - 1).Append('.');
                }

                return new(TextSpan, DiagnosticLevel.Error, "BCP311", message.ToString());
            }

            public Diagnostic MultipleAdditionalPropertiesDeclarations() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP315",
                "An object type may have at most one additional properties declaration.");

            public Diagnostic SealedIncompatibleWithAdditionalPropertiesDeclaration() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP316",
                $@"The ""{LanguageConstants.ParameterSealedPropertyName}"" decorator may not be used on object types with an explicit additional properties type declaration.");

            public Diagnostic ExpectedPropertyNameOrMatcher() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP317",
                "Expected an identifier, a string, or an asterisk at this location.");

            public FixableDiagnostic DereferenceOfPossiblyNullReference(string possiblyNullType, AccessExpressionSyntax accessExpression) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP318",
                $@"The value of type ""{possiblyNullType}"" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail.",
                null,
                DiagnosticStyling.Default,
                new(
                    "If you do not know whether the value will be null and the template would handle a null value for the overall expression, use a `.?` (safe dereference) operator to short-circuit the access expression if the base expression's value is null",
                    true,
                    CodeFixKind.QuickFix,
                    new(accessExpression.Span, accessExpression.AsSafeAccess().ToString())),
                AsNonNullable(accessExpression.BaseExpression));

            private static CodeFix AsNonNullable(SyntaxBase expression) => new(
                "If you know the value will not be null, use a non-null assertion operator to inform the compiler that the value will not be null",
                false,
                CodeFixKind.QuickFix,
                new(expression.Span, SyntaxFactory.AsNonNullable(expression).ToString()));

            public Diagnostic UnresolvableArmJsonType(string errorSource, string message) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP319",
                $@"The type at ""{errorSource}"" could not be resolved by the ARM JSON template engine. Original error message: ""{message}""");

            public Diagnostic ModuleOutputResourcePropertyAccessDetected() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP320",
                "The properties of module output resources cannot be accessed directly. To use the properties of this resource, pass it as a resource-typed parameter to another module and access the parameter's properties therein.");

            public FixableDiagnostic PossibleNullReferenceAssignment(TypeSymbol expectedType, TypeSymbol actualType, SyntaxBase expression) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP321",
                $"Expected a value of type \"{expectedType}\" but the provided value is of type \"{actualType}\".",
                documentationUri: null,
                styling: DiagnosticStyling.Default,
                fix: AsNonNullable(expression));

            public Diagnostic SafeDereferenceNotPermittedOnInstanceFunctions() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP322",
                "The `.?` (safe dereference) operator may not be used on instance function invocations.");

            public Diagnostic SafeDereferenceNotPermittedOnResourceCollections() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP323",
                "The `[?]` (safe dereference) operator may not be used on resource or module collections.");

            public Diagnostic ExpectedTypeIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP325",
                "Expected a type identifier at this location.");

            public Diagnostic NullableTypedParamsMayNotHaveDefaultValues() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP326",
                "Nullable-typed parameters may not be assigned default values. They have an implicit default of 'null' that cannot be overridden.");

            public Diagnostic SourceIntDomainDisjointFromTargetIntDomain_SourceHigh(bool warnInsteadOfError, long sourceMin, long targetMax) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP327",
                $"The provided value (which will always be greater than or equal to {sourceMin}) is too large to assign to a target for which the maximum allowable value is {targetMax}.");

            public Diagnostic SourceIntDomainDisjointFromTargetIntDomain_SourceLow(bool warnInsteadOfError, long sourceMax, long targetMin) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP328",
                $"The provided value (which will always be less than or equal to {sourceMax}) is too small to assign to a target for which the minimum allowable value is {targetMin}.");

            public Diagnostic SourceIntDomainExtendsBelowTargetIntDomain(long sourceMin, long targetMin) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP329",
                $"The provided value can be as small as {sourceMin} and may be too small to assign to a target with a configured minimum of {targetMin}.");

            public Diagnostic SourceIntDomainExtendsAboveTargetIntDomain(long sourceMax, long targetMax) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP330",
                $"The provided value can be as large as {sourceMax} and may be too large to assign to a target with a configured maximum of {targetMax}.");

            public Diagnostic MinMayNotExceedMax(string minDecoratorName, long minValue, string maxDecoratorName, long maxValue) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP331",
                $@"A type's ""{minDecoratorName}"" must be less than or equal to its ""{maxDecoratorName}"", but a minimum of {minValue} and a maximum of {maxValue} were specified.");

            public Diagnostic SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceHigh(bool warnInsteadOfError, long sourceMinLength, long targetMaxLength) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP332",
                $"The provided value (whose length will always be greater than or equal to {sourceMinLength}) is too long to assign to a target for which the maximum allowable length is {targetMaxLength}.");

            public Diagnostic SourceValueLengthDomainDisjointFromTargetValueLengthDomain_SourceLow(bool warnInsteadOfError, long sourceMaxLength, long targetMinLength) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP333",
                $"The provided value (whose length will always be less than or equal to {sourceMaxLength}) is too short to assign to a target for which the minimum allowable length is {targetMinLength}.");

            public Diagnostic SourceValueLengthDomainExtendsBelowTargetValueLengthDomain(long sourceMinLength, long targetMinLength) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP334",
                $"The provided value can have a length as small as {sourceMinLength} and may be too short to assign to a target with a configured minimum length of {targetMinLength}.");

            public Diagnostic SourceValueLengthDomainExtendsAboveTargetValueLengthDomain(long sourceMaxLength, long targetMaxLength) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP335",
                $"The provided value can have a length as large as {sourceMaxLength} and may be too long to assign to a target with a configured maximum length of {targetMaxLength}.");

            public Diagnostic UnrecognizedParamsFileDeclaration() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP337",
                $@"This declaration type is not valid for a Bicep Parameters file. Specify a ""{LanguageConstants.UsingKeyword}"", ""{LanguageConstants.ExtendsKeyword}"", ""{LanguageConstants.ParameterKeyword}"" or ""{LanguageConstants.VariableKeyword}"" declaration.");

            public Diagnostic FailedToEvaluateParameter(string parameterName, string message) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP338",
                $"Failed to evaluate parameter \"{parameterName}\": {message}");

            public Diagnostic ArrayIndexOutOfBounds(long indexSought) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP339",
                $"""The provided array index value of "{indexSought}" is not valid. Array index should be greater than or equal to 0.""");

            public Diagnostic UnparsableYamlType() => new(
               TextSpan,
               DiagnosticLevel.Error,
               "BCP340",
               $"Unable to parse literal YAML value. Please ensure that it is well-formed.");

            public Diagnostic RuntimeValueNotAllowedInFunctionDeclaration(string? accessedSymbolName, IEnumerable<string>? accessiblePropertyNames, IEnumerable<string>? variableDependencyChain)
            {
                var variableDependencyChainClause = BuildVariableDependencyChainClause(variableDependencyChain);
                var accessiblePropertiesClause = BuildAccessiblePropertiesClause(accessedSymbolName, accessiblePropertyNames);

                return new(
                    TextSpan,
                    DiagnosticLevel.Error,
                    "BCP341",
                    $"This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment.{variableDependencyChainClause}{accessiblePropertiesClause}");
            }

            public Diagnostic UserDefinedTypesNotAllowedInFunctionDeclaration() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP342",
                $"""User-defined types are not supported in user-defined function parameters or outputs.""");

            public Diagnostic ExpectedAssertIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP344",
                "Expected an assert identifier at this location.");

            public Diagnostic TestDeclarationMustReferenceBicepTest() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP345",
                "A test declaration can only reference a Bicep File");

            public Diagnostic ExpectedTestIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP346",
                "Expected a test identifier at this location.");

            public Diagnostic ExpectedTestPathString() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP347",
                "Expected a test path string at this location.");
            public Diagnostic TestDeclarationStatementsUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP348",
                $@"Using a test declaration statement requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.TestFramework)}"".");

            public Diagnostic AssertsUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP349",
                $@"Using an assert declaration requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.Assertions)}"".");

            public Diagnostic InvalidAssertAssignment(TypeSymbol valueType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP350",
                $"Value of type \"{valueType}\" cannot be assigned to an assert. Asserts can take values of type 'bool' only.");

            public Diagnostic FunctionOnlyValidWithDirectAssignment(string functionName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP351",
                $"Function \"{functionName}\" is not valid at this location. It can only be used when directly assigning to a parameter.");

            public Diagnostic FailedToEvaluateVariable(string name, string message) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP352",
                $"Failed to evaluate variable \"{name}\": {message}");

            public Diagnostic ItemsMustBeCaseInsensitivelyUnique(string itemTypePluralName, IEnumerable<string> itemNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP353",
                $"The {itemTypePluralName} {ToQuotedString(itemNames)} differ only in casing. The ARM deployments engine is not case sensitive and will not be able to distinguish between them.");

            public Diagnostic ExpectedSymbolListOrWildcard() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP354",
                "Expected left brace ('{') or asterisk ('*') character at this location.");

            public Diagnostic ExpectedExportedSymbolName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP355",
                "Expected the name of an exported symbol at this location.");

            public Diagnostic ExpectedNamespaceIdentifier() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP356",
                "Expected a valid namespace identifier at this location.");

            public Diagnostic PathHasNotBeenSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP358",
                "This declaration is missing a template file path reference.");

            public Diagnostic ImportedSymbolNotFound(string symbolName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP360",
                $"The '{symbolName}' symbol was not found in (or was not exported by) the imported template.");

            public Diagnostic ExportDecoratorMustTargetStatement() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP361",
                @"The ""@export()"" decorator must target a top-level statement.");

            public Diagnostic SymbolImportedMultipleTimes(params string[] importedAs) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP362",
                $"This symbol is imported multiple times under the names {string.Join(", ", importedAs.Select(identifier => $"'{identifier}'"))}.");

            public Diagnostic DiscriminatorDecoratorOnlySupportedForObjectUnions() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP363",
                $"The \"{LanguageConstants.TypeDiscriminatorDecoratorName}\" decorator can only be applied to object-only union types with unique member types.");

            public Diagnostic DiscriminatorPropertyMustBeRequiredStringLiteral(string discriminatorPropertyName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP364",
                $"The property \"{discriminatorPropertyName}\" must be a required string literal on all union member types.");

            public Diagnostic DiscriminatorPropertyMemberDuplicatedValue(string discriminatorPropertyName, string discriminatorPropertyValue) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP365",
                $"The value \"{discriminatorPropertyValue}\" for discriminator property \"{discriminatorPropertyName}\" is duplicated across multiple union member types. The value must be unique across all union member types.");

            public Diagnostic DiscriminatorPropertyNameMustMatch(string acceptablePropertyName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP366",
                $"The discriminator property name must be \"{acceptablePropertyName}\" on all union member types.");

            public Diagnostic FeatureIsTemporarilyDisabled(string featureName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP367",
                $"The \"{featureName}\" feature is temporarily disabled.");

            public Diagnostic ParameterReferencesKeyVaultSuppliedParameter(string targetName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP368",
                $"The value of the \"{targetName}\" parameter cannot be known until the template deployment has started because it uses a reference to a secret value in Azure Key Vault. Expressions that refer to the \"{targetName}\" parameter may be used in {LanguageConstants.LanguageFileExtension} files but not in {LanguageConstants.ParamsFileExtension} files.");

            public Diagnostic ParameterReferencesDefaultedParameter(string targetName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP369",
                $"The value of the \"{targetName}\" parameter cannot be known until the template deployment has started because it uses the default value defined in the template. Expressions that refer to the \"{targetName}\" parameter may be used in {LanguageConstants.LanguageFileExtension} files but not in {LanguageConstants.ParamsFileExtension} files.");

            public Diagnostic ClosureContainsNonExportableSymbols(IEnumerable<string> nonExportableSymbols) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP372",
                @$"The ""@export()"" decorator may not be applied to variables that refer to parameters, modules, or resource, either directly or indirectly. The target of this decorator contains direct or transitive references to the following unexportable symbols: {ToQuotedString(nonExportableSymbols)}.");

            public Diagnostic ImportedSymbolHasErrors(string name, string message) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP373",
                $"Unable to import the symbol named \"{name}\": {message}");

            public Diagnostic ImportedModelContainsAmbiguousExports(IEnumerable<string> ambiguousExportNames) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP374",
                $"The imported model cannot be loaded with a wildcard because it contains the following duplicated exports: {ToQuotedString(ambiguousExportNames)}.");

            public Diagnostic ImportListItemDoesNotIncludeDeclaredSymbolName() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP375",
                "An import list item that identifies its target with a quoted string must include an 'as <alias>' clause.");

            public Diagnostic ImportedSymbolKindNotSupportedInSourceFileKind(string name, ExportMetadataKind exportMetadataKind, BicepSourceFileKind sourceFileKind) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP376",
                $"The \"{name}\" symbol cannot be imported because imports of kind {exportMetadataKind} are not supported in files of kind {sourceFileKind}.");

            public Diagnostic InvalidExtensionAliasName(string aliasName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP377",
                $"The extension alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");

            public Diagnostic InvalidOciArtifactExtensionAliasRegistryNullOrUndefined(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP378",
                $"The OCI artifact extension alias \"{aliasName}\" in the {BuildBicepConfigurationClause(configFileUri)} is invalid. The \"registry\" property cannot be null or undefined.");

            public Diagnostic OciArtifactExtensionAliasNameDoesNotExistInConfiguration(string aliasName, Uri? configFileUri) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP379",
                $"The OCI artifact extension alias name \"{aliasName}\" does not exist in the {BuildBicepConfigurationClause(configFileUri)}.");

            public Diagnostic UnsupportedArtifactType(ArtifactType artifactType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP380",
                $"Artifacts of type: \"{artifactType}\" are not supported."
            );

            public FixableDiagnostic ExtensionDeclarationKeywordIsDeprecated(ExtensionDeclarationSyntax syntax)
            {
                var codeFix = new CodeFix(
                    $"Replace the {syntax.Keyword.Text} keyword with the extension keyword",
                    true,
                    CodeFixKind.QuickFix,
                    new CodeReplacement(syntax.Keyword.Span, LanguageConstants.ExtensionKeyword));

                return new FixableDiagnostic(
                    TextSpan,
                    DiagnosticLevel.Warning,
                    "BCP381",
                    @$"Declaring extension with the ""{syntax.Keyword.Text}"" keyword has been deprecated. Please use the ""extension"" keyword instead. Please see https://github.com/Azure/bicep/issues/14374 for more information.",
                    documentationUri: null,
                    DiagnosticStyling.Default,
                    codeFix);
            }

            public Diagnostic TypeIsNotParameterizable(string typeName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP383",
                $"The \"{typeName}\" type is not parameterizable.");

            public Diagnostic TypeRequiresParameterization(string typeName, int requiredArgumentCount) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP384",
                $"The \"{typeName}\" type requires {requiredArgumentCount} argument(s).");

            public Diagnostic ResourceDerivedTypesUnsupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP385",
                $@"Using resource-derived types requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.ResourceDerivedTypes)}"".");

            public Diagnostic DecoratorMayNotTargetResourceDerivedType(string decoratorName) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP386",
                $@"The decorator ""{decoratorName}"" may not be used on statements whose declared type is a reference to a resource-derived type.");

            public Diagnostic NegatedTypeIndexSought() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP387",
                "Indexing into a type requires an integer greater than or equal to 0.");

            public Diagnostic TupleRequiredForIndexAccess(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP388",
                $"Cannot access elements of type \"{wrongType}\" by index. An tuple type is required.");

            public Diagnostic ExplicitAdditionalPropertiesTypeRequiredForAccessThereto(TypeSymbol wrongType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP389",
                $"The type \"{wrongType}\" does not declare an additional properties type.");

            public Diagnostic ExplicitItemsTypeRequiredForAccessThereto() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP390",
                $"The array item type access operator ('[*]') can only be used with typed arrays.");

            public Diagnostic AccessExpressionForbiddenBase() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP391",
                "Type member access is only supported on a reference to a named type.");

            public Diagnostic InvalidResourceTypeIdentifier(string resourceTypeIdentifier) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP392",
                $"""The supplied resource type identifier "{resourceTypeIdentifier}" was not recognized as a valid resource type name.""");

            public Diagnostic UnrecognizedResourceDerivedTypePointerSegment(string unrecognizedSegment) => new(
                TextSpan,
                DiagnosticLevel.Warning,
                "BCP393",
                $"""The type pointer segment "{unrecognizedSegment}" was not recognized. Supported pointer segments are: "properties", "items", "prefixItems", and "additionalProperties".""");

            public Diagnostic CannotUseEntireResourceBodyAsType() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP394",
                "Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted.");

            public Diagnostic InvalidTypesTgzPackage_DeserializationFailed() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP396",
                "The referenced extension types artifact has been published with malformed content.");

            public Diagnostic InvalidExtension_ImplicitExtensionMissingConfig(Uri? configFileUri, string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP397",
                $"""Extension {name} is incorrectly configured in the {BuildBicepConfigurationClause(configFileUri)}. It is referenced in the "{RootConfiguration.ImplicitExtensionsKey}" section, but is missing corresponding configuration in the "{RootConfiguration.ExtensionsKey}" section.""");

            public Diagnostic InvalidExtension_NotABuiltInExtension(Uri? configFileUri, string name) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP398",
                $"""Extension {name} is incorrectly configured in the {BuildBicepConfigurationClause(configFileUri)}. It is configured as built-in in the "{RootConfiguration.ExtensionsKey}" section, but no built-in extension exists.""");

            public Diagnostic FetchingAzTypesRequiresExperimentalFeature() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP399",
                $"Fetching az types from the registry requires enabling EXPERIMENTAL feature \"{nameof(ExperimentalFeaturesEnabled.DynamicTypeLoading)}\".");

            public Diagnostic FetchingTypesRequiresExperimentalFeature() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP400",
                $"Fetching types from the registry requires enabling EXPERIMENTAL feature \"{nameof(ExperimentalFeaturesEnabled.ExtensionRegistry)}\".");

            public Diagnostic SpreadOperatorUnsupportedInLocation(SpreadExpressionSyntax spread) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP401",
                $"The spread operator \"{spread.Ellipsis.Text}\" is not permitted in this location.");

            public Diagnostic SpreadOperatorRequiresAssignableValue(SpreadExpressionSyntax spread, TypeSymbol requiredType) => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP402",
                $"The spread operator \"{spread.Ellipsis.Text}\" can only be used in this context for an expression assignable to type \"{requiredType}\".");

            public Diagnostic ArrayTypeMismatchSpread(bool warnInsteadOfError, TypeSymbol expectedType, TypeSymbol actualType) => new(
                TextSpan,
                warnInsteadOfError ? DiagnosticLevel.Warning : DiagnosticLevel.Error,
                "BCP403",
                $"The enclosing array expects elements of type \"{expectedType}\", but the array being spread contains elements of incompatible type \"{actualType}\".");

            public Diagnostic ExtendsPathHasNotBeenSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP404",
                $"The \"{LanguageConstants.ExtendsKeyword}\" declaration is missing a bicepparam file path reference");

            public Diagnostic MoreThanOneExtendsDeclarationSpecified() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP405",
                $"More than one \"{LanguageConstants.ExtendsKeyword}\" declaration are present");

            public Diagnostic ExtendsNotSupported() => new(
                TextSpan,
                DiagnosticLevel.Error,
                "BCP406",
                $"The \"{LanguageConstants.ExtendsKeyword}\" keyword is not supported");
        }

        public static DiagnosticBuilderInternal ForPosition(TextSpan span)
            => new(span);

        public static DiagnosticBuilderInternal ForPosition(IPositionable positionable)
            => new(positionable.Span);

        public static DiagnosticBuilderInternal ForDocumentStart()
            => new(TextSpan.TextDocumentStart);
    }
}
