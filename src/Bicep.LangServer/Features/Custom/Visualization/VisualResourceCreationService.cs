// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.CodeAction;
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
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Thrown by <see cref="IVisualResourceCreationService"/> when a <c>textDocument/prepareVisualResource</c>
    /// request cannot be satisfied. Handlers are expected to translate this into an RPC error response.
    /// </summary>
    public sealed class VisualResourceCreationException : Exception
    {
        public VisualResourceCreationException(string message) : base(message)
        {
        }
    }

    public interface IVisualResourceCreationService
    {
        /// <summary>
        /// Builds a paged, filtered catalog of the resource types available for the Az namespace in the
        /// active document's compilation.
        /// </summary>
        VisualResourceTypesResult GetResourceTypes(
            SemanticModel model,
            string? query,
            bool includePreview,
            int pageSize,
            string? continuationToken);

        /// <summary>
        /// Generates a top-level resource declaration for the requested resource type and returns a
        /// versioned <see cref="WorkspaceEdit"/> that inserts it into the active document.
        /// </summary>
        PrepareVisualResourceResult PrepareResource(
            BicepCompiler compiler,
            CompilationContext context,
            PrepareVisualResourceParams request);
    }

    public class VisualResourceCreationService : IVisualResourceCreationService
    {
        // Client-supplied pageSize values are clamped server-side rather than echoed verbatim.
        public const int DefaultPageSize = 50;
        public const int MaxPageSize = 200;

        private const string FallbackSymbolicName = "resource";

        public VisualResourceTypesResult GetResourceTypes(
            SemanticModel model,
            string? query,
            bool includePreview,
            int pageSize,
            string? continuationToken)
        {
            var nsResolver = model.Binder.NamespaceResolver;

            var catalog = BuildCatalog(nsResolver.GetAvailableAzureResourceTypes())
                .Where(entry => includePreview || !entry.IsPreview)
                .Where(entry => string.IsNullOrEmpty(query) ||
                    entry.FullyQualifiedType.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(entry => entry.FullyQualifiedType, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(entry => entry.ApiVersion, StringComparer.OrdinalIgnoreCase)
                .ToImmutableArray();

            var effectivePageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
            var offset = ParseContinuationToken(continuationToken);

            var page = catalog.Skip(offset).Take(effectivePageSize).ToImmutableArray();
            var nextOffset = offset + page.Length;
            var nextContinuationToken = nextOffset < catalog.Length
                ? nextOffset.ToString(CultureInfo.InvariantCulture)
                : null;

            return new VisualResourceTypesResult(page, nextContinuationToken);
        }

        private static IEnumerable<VisualResourceTypeCatalogEntry> BuildCatalog(
            IEnumerable<ResourceTypeReference> resourceTypes)
        {
            foreach (var resourceType in resourceTypes)
            {
                if (resourceType.ApiVersion is not { } apiVersion)
                {
                    continue;
                }

                yield return new VisualResourceTypeCatalogEntry(
                    resourceType.FormatType(),
                    apiVersion,
                    IsPreviewApiVersion(apiVersion));
            }
        }

        public PrepareVisualResourceResult PrepareResource(
            BicepCompiler compiler,
            CompilationContext context,
            PrepareVisualResourceParams request)
        {
            if (context.SourceFileKind != BicepSourceFileKind.BicepFile)
            {
                throw new VisualResourceCreationException("Visual resource creation is only supported for Bicep files.");
            }

            var model = context.Compilation.GetEntrypointSemanticModel();
            var nsResolver = model.Binder.NamespaceResolver;

            var requestedTypeReference = new ResourceTypeReference(
                request.ResourceType.FullyQualifiedType,
                request.ResourceType.ApiVersion);

            if (!nsResolver.GetAvailableAzureResourceTypes().Contains(requestedTypeReference))
            {
                throw new VisualResourceCreationException(
                    $"Resource type \"{requestedTypeReference.FormatName()}\" was not found.");
            }

            var resourceType = nsResolver.GetMatchingResourceTypes(requestedTypeReference, ResourceTypeGenerationFlags.None).FirstOrDefault();
            if (resourceType is null)
            {
                throw new VisualResourceCreationException(
                    $"Unable to resolve a type definition for resource type \"{requestedTypeReference.FormatName()}\".");
            }

            var symbolicName = GenerateSymbolicName(requestedTypeReference, model);
            var (bodyProperties, unresolvedRequiredProperties) = GenerateBody(resourceType);

            var resourceDeclaration = CreateResourceSyntax(symbolicName, requestedTypeReference, bodyProperties);
            var insertContext = GetInsertContext(context);
            var replacement = GenerateCodeReplacement(compiler, model.Configuration, resourceDeclaration, insertContext);

            var textEdit = new TextEdit
            {
                Range = replacement.ToRange(context.LineStarts),
                NewText = replacement.Text,
            };

            var textDocumentEdit = new TextDocumentEdit
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = request.TextDocument.Uri,
                    Version = request.TextDocument.Version,
                },
                Edits = new TextEditContainer(textEdit),
            };

            var edit = new WorkspaceEdit
            {
                DocumentChanges = new Container<WorkspaceEditDocumentChange>(textDocumentEdit),
            };

            return new PrepareVisualResourceResult(
                request.OperationId,
                symbolicName,
                symbolicName,
                unresolvedRequiredProperties,
                edit);
        }

        private static int ParseContinuationToken(string? continuationToken)
        {
            if (continuationToken is not null &&
                int.TryParse(continuationToken, NumberStyles.None, CultureInfo.InvariantCulture, out var offset) &&
                offset >= 0)
            {
                return offset;
            }

            return 0;
        }

        private static bool IsPreviewApiVersion(string apiVersion) =>
            AzureResourceApiVersion.TryParse(apiVersion, out var parsed) && parsed.IsPreview;

        /// <summary>
        /// Derives a deterministic, valid, case-insensitively-unique symbolic name for a new resource of the
        /// given type. Collisions with existing top-level declarations are resolved by appending the smallest
        /// positive integer suffix that produces a unique name (case-insensitive).
        /// </summary>
        internal static string GenerateSymbolicName(ResourceTypeReference typeReference, SemanticModel model)
        {
            var baseName = DeriveBaseSymbolicName(typeReference);

            // Bicep's own declaration lookup is case-sensitive (ordinal), but symbolic name generation must
            // avoid case-insensitive collisions, so a dedicated comparer is used here.
            var existingNames = new HashSet<string>(
                model.Root.Declarations.Select(declaration => declaration.Name),
                StringComparer.OrdinalIgnoreCase);

            if (existingNames.Add(baseName))
            {
                return baseName;
            }

            var suffix = 1;
            string candidate;
            do
            {
                candidate = $"{baseName}{suffix}";
                suffix++;
            } while (!existingNames.Add(candidate));

            return candidate;
        }

        internal static string DeriveBaseSymbolicName(ResourceTypeReference typeReference)
        {
            var lastSegment = typeReference.TypeSegments.LastOrDefault() ?? string.Empty;
            var candidate = Sanitize(ToCamelCase(Singularize(lastSegment)));

            if (candidate.Length == 0 || !Lexer.IsValidIdentifier(candidate))
            {
                return FallbackSymbolicName;
            }

            return candidate;
        }

        private static string Singularize(string value)
        {
            if (value.EndsWith("ies", StringComparison.Ordinal) && value.Length > 3)
            {
                return string.Concat(value.AsSpan(0, value.Length - 3), "y");
            }

            if (value.Length > 1 &&
                value.EndsWith('s') &&
                !value.EndsWith("ss", StringComparison.Ordinal))
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

        // Mirrors Lexer's private identifier start/continuation checks - kept in sync deliberately rather
        // than reusing Lexer.IsValidIdentifier() mid-derivation (only used as the final validity gate).
        private static bool IsIdentifierStartChar(char c) =>
            (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private static bool IsIdentifierContinuationChar(char c) =>
            IsIdentifierStartChar(c) || (c >= '0' && c <= '9');

        /// <summary>
        /// Computes the literal-valued top-level properties to include in the generated resource body, along
        /// with the names of required properties that could not be resolved to a deterministic literal value.
        /// No arbitrary/sample/placeholder values are ever generated.
        /// </summary>
        internal static (ImmutableArray<ObjectPropertySyntax> Properties, ImmutableArray<string> UnresolvedRequiredProperties) GenerateBody(ResourceType resourceType)
        {
            var properties = ImmutableArray.CreateBuilder<ObjectPropertySyntax>();
            var unresolvedRequiredProperties = ImmutableArray.CreateBuilder<string>();

            switch (resourceType.Body.Type)
            {
                case DiscriminatedObjectType discriminatedObjectType:
                    // The discriminator selects which of several possible bodies applies; without it, no
                    // literal properties can be generated, so it is reported as unresolved.
                    unresolvedRequiredProperties.Add(discriminatedObjectType.DiscriminatorKey);
                    break;

                case ObjectType objectType:
                    foreach (var property in objectType.Properties.Values.Where(TypeHelper.IsRequired))
                    {
                        if (property.TypeReference.Type is StringLiteralType stringLiteralType)
                        {
                            properties.Add(SyntaxFactory.CreateObjectProperty(
                                property.Name,
                                SyntaxFactory.CreateStringLiteral(stringLiteralType.RawStringValue)));
                        }
                        else
                        {
                            unresolvedRequiredProperties.Add(property.Name);
                        }
                    }
                    break;
            }

            return (properties.ToImmutable(), unresolvedRequiredProperties.ToImmutable());
        }

        private static ResourceDeclarationSyntax CreateResourceSyntax(
            string symbolicName,
            ResourceTypeReference typeReference,
            ImmutableArray<ObjectPropertySyntax> bodyProperties) =>
            new(
                [],
                SyntaxFactory.ResourceKeywordToken,
                SyntaxFactory.CreateIdentifierWithTrailingSpace(symbolicName),
                SyntaxFactory.CreateStringLiteral(typeReference.FormatName()),
                null,
                SyntaxFactory.CreateToken(TokenType.Assignment),
                [],
                SyntaxFactory.CreateObject(bodyProperties));

        private record InsertContext(
            int LeadingNewlineCount,
            int TrailingNewlineCount,
            int InsertOffset);

        private static InsertContext GetInsertContext(CompilationContext context)
        {
            var declarations = context.ProgramSyntax.Declarations.ToArray();
            var lastResource = declarations.OfType<ResourceDeclarationSyntax>().LastOrDefault();

            if (lastResource is not null)
            {
                return new(LeadingNewlineCount: 2, TrailingNewlineCount: 0, InsertOffset: lastResource.GetEndPosition());
            }

            // Keep resources after file-level declarations, parameters, and variables, but before modules and
            // outputs. The whitespace and comments following the anchor remain attached to the next declaration.
            var lastPreambleDeclaration = declarations
                .TakeWhile(declaration => declaration is not ModuleDeclarationSyntax and not OutputDeclarationSyntax)
                .LastOrDefault();

            if (lastPreambleDeclaration is not null)
            {
                return new(LeadingNewlineCount: 2, TrailingNewlineCount: 0, InsertOffset: lastPreambleDeclaration.GetEndPosition());
            }

            return declarations.Length > 0
                ? new(LeadingNewlineCount: 0, TrailingNewlineCount: 2, InsertOffset: 0)
                : new(LeadingNewlineCount: 0, TrailingNewlineCount: 0, InsertOffset: 0);
        }

        private static CodeReplacement GenerateCodeReplacement(
            BicepCompiler compiler,
            Bicep.Core.Configuration.RootConfiguration configuration,
            ResourceDeclarationSyntax resourceDeclaration,
            InsertContext insertContext)
        {
            // Build a throwaway document containing only the new resource so that syntax rewriters and the
            // formatter can be applied before generating the code replacement, matching InsertResourceHandler.
            var program = new ProgramSyntax(
                [resourceDeclaration],
                SyntaxFactory.EndOfFileToken);

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

            // Self-validation: the generated replacement must itself be syntactically valid Bicep before it is
            // ever returned to the client. Semantic completeness is not required here - resource types with
            // unresolved required properties (reported via unresolvedRequiredProperties) are expected to
            // surface ordinary compiler diagnostics once inserted; those remain the editor's responsibility.
            if (bicepFile.LexingErrorLookup.Any() || bicepFile.ParsingErrorLookup.Any())
            {
                throw new VisualResourceCreationException("Generated resource declaration failed self-validation.");
            }

            var printerOptions = configuration.Formatting.Data;
            var printed = PrettyPrinterV2.PrintValid(bicepFile.ProgramSyntax, printerOptions);

            var newline = printerOptions.NewlineKind.ToEscapeSequence();
            var newlineCharacters = newline.ToCharArray();
            printed = printed.Trim(newlineCharacters);
            printed = $"{string.Concat(Enumerable.Repeat(newline, insertContext.LeadingNewlineCount))}{printed}" +
                string.Concat(Enumerable.Repeat(newline, insertContext.TrailingNewlineCount));

            return new CodeReplacement(new TextSpan(insertContext.InsertOffset, 0), printed);
        }
    }
}
