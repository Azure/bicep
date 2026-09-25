// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Resources;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Language.Completion.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// A new top-level resource declaration with required properties. Values needing user input remain empty
    /// in the formatted insertion and are reported as unresolved.
    /// </summary>
    internal sealed record GeneratedResourceDeclaration(
        string SymbolicName,
        ResourceDeclarationSyntax Declaration,
        ImmutableArray<string> UnresolvedRequiredProperties)
    {
        private const string FallbackSymbolicName = "resource";
        private const string RequiredPropertyPlaceholder = "__bicep_visual_resource_creation_required_property__";

        /// <param name="existingNames">Names already declared in the document, which the new symbolic name must avoid.</param>
        public static GeneratedResourceDeclaration Create(
            ResourceTypeReference typeReference,
            ResourceType resourceType,
            IEnumerable<string> existingNames,
            SemanticModel model)
        {
            var symbolicName = GenerateSymbolicName(typeReference, existingNames);
            var (body, unresolvedRequiredProperties) = GenerateBody(resourceType, model, symbolicName);
            var declaration = new ResourceDeclarationSyntax(
                [],
                SyntaxFactory.ResourceKeywordToken,
                SyntaxFactory.CreateIdentifierWithTrailingSpace(symbolicName),
                SyntaxFactory.CreateStringLiteral(typeReference.FormatName()),
                null,
                SyntaxFactory.CreateToken(TokenType.Assignment),
                [],
                body);

            return new(symbolicName, declaration, unresolvedRequiredProperties);
        }

        public TextEdit CreateInsertionEdit(BicepCompiler compiler, CompilationContext context)
        {
            var (offset, leadingNewlines, trailingNewlines) = GetInsertionPoint(context.ProgramSyntax);
            var formattingOptions = context.Compilation.GetEntrypointSemanticModel().Configuration.Formatting.Data;
            var newline = formattingOptions.NewlineKind.ToEscapeSequence();
            var text = Format(compiler, this.Declaration, formattingOptions).Trim(newline.ToCharArray());

            return new TextEdit
            {
                Range = new TextSpan(offset, 0).ToRange(context.LineStarts),
                NewText = string.Concat(Enumerable.Repeat(newline, leadingNewlines)) + text + string.Concat(Enumerable.Repeat(newline, trailingNewlines)),
            };
        }

        /// <summary>
        /// Resources go after the last resource. Without one, they go after file-level declarations, parameters, and
        /// variables, but before modules and outputs; the whitespace and comments following the anchor stay attached to
        /// the next declaration.
        /// </summary>
        private static (int Offset, int LeadingNewlines, int TrailingNewlines) GetInsertionPoint(ProgramSyntax program)
        {
            var declarations = program.Declarations.ToArray();
            SyntaxBase? anchor = declarations.OfType<ResourceDeclarationSyntax>().LastOrDefault() ??
                declarations.TakeWhile(declaration => declaration is not ModuleDeclarationSyntax and not OutputDeclarationSyntax).LastOrDefault();

            return anchor is not null
                ? (anchor.GetEndPosition(), 2, 0)
                : (0, 0, declarations.Length > 0 ? 2 : 0);
        }

        private static string Format(BicepCompiler compiler, ResourceDeclarationSyntax declaration, PrettyPrinterV2Options options)
        {
            // Rewrite and format a throwaway document holding only the new resource, matching InsertResourceHandler.
            var program = new ProgramSyntax([declaration], SyntaxFactory.EndOfFileToken);
            BicepSourceFile bicepFile = compiler.SourceFileFactory.CreateBicepFile(DummyFileHandle.Default, program.ToString());

            var workspace = new ActiveSourceFileSet();
            workspace.UpsertSourceFile(bicepFile);
            var compilation = compiler.CreateCompilationWithoutRestore(bicepFile.FileHandle.Uri, workspace);

            bicepFile = RewriterHelper.RewriteMultiple(
                compiler,
                compilation,
                bicepFile,
                rewritePasses: 5,
                model => new TypeCasingFixerRewriter(model),
                model => new ReadOnlyPropertyRemovalRewriter(model));

            // Temporary values keep the declaration valid while rewriters and formatting run. The final edit leaves
            // unresolved values empty so the editor can surface their diagnostics after insertion.
            if (bicepFile.LexingErrorLookup.Any() || bicepFile.ParsingErrorLookup.Any())
            {
                throw new VisualResourceCreationException("Generated resource declaration failed self-validation.");
            }

            var printed = PrettyPrinterV2.PrintValid(bicepFile.ProgramSyntax, options);
            printed = printed.Replace($" {RequiredPropertyPlaceholder}", string.Empty, StringComparison.Ordinal);
            if (printed.Contains(RequiredPropertyPlaceholder, StringComparison.Ordinal))
            {
                throw new VisualResourceCreationException("Generated resource declaration contains an unresolved formatting placeholder.");
            }

            return printed;
        }

        /// <summary>
        /// Derives the name from the type's last segment (for example <c>storageAccount</c>). A name that collides with an
        /// existing one, case-insensitively, gets the smallest positive integer suffix that makes it unique.
        /// </summary>
        private static string GenerateSymbolicName(ResourceTypeReference typeReference, IEnumerable<string> existingNames)
        {
            var candidate = Sanitize(ToCamelCase(Singularize(typeReference.TypeSegments.LastOrDefault() ?? string.Empty)));
            var baseName = candidate.Length > 0 && Lexer.IsValidIdentifier(candidate) ? candidate : FallbackSymbolicName;

            // Bicep's own declaration lookup is case-sensitive, but names differing only by case would be confusing.
            var takenNames = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
            if (!takenNames.Contains(baseName))
            {
                return baseName;
            }

            var suffix = 1;
            while (takenNames.Contains($"{baseName}{suffix}"))
            {
                suffix++;
            }

            return $"{baseName}{suffix}";
        }

        private static (ObjectSyntax Body, ImmutableArray<string> UnresolvedRequiredProperties) GenerateBody(
            ResourceType resourceType,
            SemanticModel model,
            string symbolicName)
        {
            var locationParameters = model.Root.ParameterDeclarations
                .Where(parameter => LanguageConstants.IdentifierComparer.Equals(parameter.Name, LanguageConstants.ResourceLocationPropertyName))
                .Take(2)
                .ToArray();

            SyntaxBase CreatePlaceholder() => SyntaxFactory.CreateVariableAccess(RequiredPropertyPlaceholder);

            SyntaxBase CreateValue(NamedTypeProperty property, bool isTopLevel) =>
                TryGeneratePropertyValue(property, isTopLevel, symbolicName, locationParameters, model.TargetScope) ??
                CreatePlaceholder();

            return resourceType.Body.Type switch
            {
                DiscriminatedObjectType discriminatedObjectType => (
                    RequiredPropertiesSyntaxBuilder.Build(discriminatedObjectType, CreatePlaceholder),
                    [discriminatedObjectType.DiscriminatorKey]),
                ObjectType objectType => (
                    RequiredPropertiesSyntaxBuilder.Build(objectType, CreateValue),
                    objectType.Properties.Values
                        .Where(TypeHelper.IsRequired)
                        .Where(property => RequiresUserInput(property, isTopLevel: true, symbolicName, locationParameters, model.TargetScope))
                        .Select(property => property.Name)
                        .ToImmutableArray()),
                _ => (SyntaxFactory.CreateObject([]), []),
            };
        }

        private static bool RequiresUserInput(
            NamedTypeProperty property,
            bool isTopLevel,
            string symbolicName,
            IReadOnlyList<ParameterSymbol> locationParameters,
            ResourceScope targetScope) =>
            property.TypeReference.Type switch
            {
                ObjectType objectType => objectType.Properties.Values
                    .Where(TypeHelper.IsRequired)
                    .Any(nestedProperty => RequiresUserInput(nestedProperty, isTopLevel: false, symbolicName, locationParameters, targetScope)),
                _ => TryGeneratePropertyValue(property, isTopLevel, symbolicName, locationParameters, targetScope) is null,
            };

        private static SyntaxBase? TryGeneratePropertyValue(
            NamedTypeProperty property,
            bool isTopLevel,
            string symbolicName,
            IReadOnlyList<ParameterSymbol> locationParameters,
            ResourceScope targetScope)
        {
            var valueType = property.TypeReference.Type;
            if (TryCreateLiteral(valueType) is { } literal)
            {
                return literal;
            }

            if (isTopLevel &&
                LanguageConstants.IdentifierComparer.Equals(property.Name, AzResourceTypeProvider.ResourceNamePropertyName) &&
                TypeValidator.AreTypesAssignable(TypeFactory.CreateStringLiteralType(symbolicName), valueType))
            {
                return SyntaxFactory.CreateStringLiteral(symbolicName);
            }

            if (!isTopLevel ||
                !LanguageConstants.IdentifierComparer.Equals(property.Name, LanguageConstants.ResourceLocationPropertyName))
            {
                return null;
            }

            if (locationParameters.Count == 1)
            {
                var locationParameter = locationParameters[0];
                return TypeValidator.AreTypesAssignable(locationParameter.Type, valueType)
                    ? SyntaxFactory.CreateVariableAccess(locationParameter.Name)
                    : null;
            }

            if (locationParameters.Count == 0 &&
                targetScope == ResourceScope.ResourceGroup &&
                TypeValidator.AreTypesAssignable(LanguageConstants.String, valueType))
            {
                return SyntaxFactory.CreateAccessSyntax(
                    SyntaxFactory.CreateFunctionCall("resourceGroup"),
                    LanguageConstants.ResourceLocationPropertyName);
            }

            return null;
        }

        private static SyntaxBase? TryCreateLiteral(TypeSymbol type) => type switch
        {
            StringLiteralType stringLiteral => SyntaxFactory.CreateStringLiteral(stringLiteral.RawStringValue),
            IntegerLiteralType integerLiteral => SyntaxFactory.CreatePositiveOrNegativeInteger(integerLiteral.Value),
            BooleanLiteralType booleanLiteral => SyntaxFactory.CreateBooleanLiteral(booleanLiteral.Value),
            _ => null,
        };

        private static string Singularize(string value)
        {
            if (value.Length > 3 && value.EndsWith("ies", StringComparison.Ordinal))
            {
                return string.Concat(value.AsSpan(0, value.Length - 3), "y");
            }

            if (value.Length > 1 && value.EndsWith('s') && !value.EndsWith("ss", StringComparison.Ordinal))
            {
                return value[..^1];
            }

            return value;
        }

        private static string ToCamelCase(string value) =>
            value.Length == 0 ? value : string.Concat(char.ToLowerInvariant(value[0]).ToString(), value.AsSpan(1));

        private static string Sanitize(string value)
        {
            var builder = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (builder.Length == 0 ? IsIdentifierStartChar(c) : IsIdentifierContinuationChar(c))
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        // Mirrors Lexer's private identifier character checks. Lexer.IsValidIdentifier() only validates a whole name, so
        // it serves as the final gate above.
        private static bool IsIdentifierStartChar(char c) =>
            (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private static bool IsIdentifierContinuationChar(char c) =>
            IsIdentifierStartChar(c) || (c >= '0' && c <= '9');
    }
}
