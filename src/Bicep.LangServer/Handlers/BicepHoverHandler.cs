// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepHoverHandler : HoverHandlerBase
    {
        private readonly ISymbolResolver symbolResolver;

        private const int MaxHoverMarkdownCodeBlockLength = 90000;
        //actual limit for hover in VS code is 100,000 characters.

        public BicepHoverHandler(ISymbolResolver symbolResolver)
        {
            this.symbolResolver = symbolResolver;
        }

        public override Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult<Hover?>(null);
            }

            var markdown = GetMarkdown(request, result);
            if (markdown == null)
            {
                return Task.FromResult<Hover?>(null);
            }

            return Task.FromResult<Hover?>(new Hover
            {
                Contents = markdown,
                Range = PositionHelper.GetNameRange(result.Context.LineStarts, result.Origin)
            });
        }

        private static string? TryGetDescriptionMarkdown(SymbolResolutionResult result, DeclaredSymbol symbol)
        {
            if (symbol.DeclaringSyntax is StatementSyntax statementSyntax &&
                SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), statementSyntax) is { } description)
            {
                return description;
            }

            return null;
        }

        private static MarkedStringsOrMarkupContent? GetMarkdown(HoverParams request, SymbolResolutionResult result)
        {
            // all of the generated markdown includes the language id to avoid VS code rendering
            // with multiple borders
            switch (result.Symbol)
            {
                case ImportedNamespaceSymbol import:
                    return WithMarkdown(CodeBlockWithDescription(
                        $"import {import.Name}", TryGetDescriptionMarkdown(result, import)));
                
                case MetadataSymbol metadata:
                    return WithMarkdown(CodeBlockWithDescription(
                        $"metadata {metadata.Name}: {metadata.Type}", TryGetDescriptionMarkdown(result, metadata)));


                case ParameterSymbol parameter:
                    return WithMarkdown(CodeBlockWithDescription(
                        $"param {parameter.Name}: {parameter.Type}", TryGetDescriptionMarkdown(result, parameter)));

                case VariableSymbol variable:
                    return WithMarkdown(CodeBlockWithDescription($"var {variable.Name}: {variable.Type}", TryGetDescriptionMarkdown(result, variable)));

                case ResourceSymbol resource:
                    var docsSuffix = TryGetTypeDocumentationLink(resource) is { } typeDocsLink ? $"[View Type Documentation]({typeDocsLink})" : "";
                    var description = TryGetDescriptionMarkdown(result, resource);

                    return WithMarkdown(CodeBlockWithDescription(
                        $"resource {resource.Name} {(resource.Type is ResourceType ? $"'{resource.Type}'" : resource.Type)}",
                        description is { } ? $"{description}\n{docsSuffix}" : docsSuffix));

                case ModuleSymbol module:
                    var filePath = SyntaxHelper.TryGetModulePath(module.DeclaringModule, out _);
                    if (filePath != null)
                    {
                        return WithMarkdown(CodeBlockWithDescription($"module {module.Name} '{filePath}'", TryGetDescriptionMarkdown(result, module)));
                    }

                    return WithMarkdown(CodeBlockWithDescription($"module {module.Name}", TryGetDescriptionMarkdown(result, module)));

                case OutputSymbol output:
                    return WithMarkdown(CodeBlockWithDescription(
                        $"output {output.Name}: {output.Type}", TryGetDescriptionMarkdown(result, output)));

                case BuiltInNamespaceSymbol builtInNamespace:
                    return WithMarkdown(CodeBlock($"{builtInNamespace.Name} namespace"));

                case FunctionSymbol function when result.Origin is FunctionCallSyntaxBase functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return GetFunctionMarkdown(function, functionCall, result.Context.Compilation.GetEntrypointSemanticModel());

                case PropertySymbol property:
                    return WithMarkdown(CodeBlockWithDescription($"{property.Name}: {property.Type}", property.Description));

                case LocalVariableSymbol local:
                    return WithMarkdown(CodeBlock($"{local.Name}: {local.Type}"));

                default:
                    return null;
            }
        }


        //we need to check for overflow due to using code blocks.
        //if we reach limit in a code block vscode will truncate it automatically, the block will not be terminated so the hover will not be properly formatted
        //therefore we need to check for the limit ourselves and truncate text inside code block, making sure it's terminated properly.
        private static string CodeBlock(string content) =>
        $"```bicep\n{(content.Length > MaxHoverMarkdownCodeBlockLength ? content.Substring(0, MaxHoverMarkdownCodeBlockLength) : content)}\n```\n";

        // Markdown needs two leading whitespaces before newline to insert a line break
        private static string CodeBlockWithDescription(string content, string? description) => CodeBlock(content) + (description is not null ? $"{description.Replace("\n", "  \n")}\n" : string.Empty);

        private static MarkedStringsOrMarkupContent GetFunctionMarkdown(FunctionSymbol function, FunctionCallSyntaxBase functionCall, SemanticModel model)
        {
            if (model.TypeManager.GetMatchedFunctionOverload(functionCall) is { } matchedOverload)
            {
                return WithMarkdown(GetFunctionOverloadMarkdown(matchedOverload, function.Overloads.Length - 1));
            }

            var potentialMatches =
                function.Overloads
                .Select(overload => (overload, matchType:
                    overload.Match(functionCall.Arguments.Select(model.GetTypeInfo).ToList(), out _, out _)))
                .Where(t => t.matchType == FunctionMatchResult.Match || t.matchType == FunctionMatchResult.PotentialMatch)
                .Select(t => t.overload)
                .ToList();

            // If there are no potential matches, just show all overloads
            IEnumerable<FunctionOverload> toShow = potentialMatches.Count > 0 ? potentialMatches : function.Overloads;

            return WithMarkdown(toShow.Select(GetFunctionOverloadMarkdown));
        }

        private static string GetFunctionOverloadMarkdown(FunctionOverload overload, int functionOverloadCount)
            => CodeBlockWithDescription($"function {overload.Name}{overload.TypeSignature}", overload.Description);

        private static string? TryGetTypeDocumentationLink(ResourceSymbol resource)
        {
            if (resource.TryGetResourceType() is { } resourceType &&
                resourceType.DeclaringNamespace.ProviderNameEquals(AzNamespaceType.BuiltInName) &&
                resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
            {
                var provider = resourceType.TypeReference.TypeSegments.First().ToLowerInvariant();
                var typePath = resourceType.TypeReference.TypeSegments.Skip(1).Select(x => x.ToLowerInvariant());

                return $"https://docs.microsoft.com/azure/templates/{provider}/{string.Join('/', typePath)}?tabs=bicep";
            }

            return null;
        }

        private static MarkedStringsOrMarkupContent WithMarkdown(string markdown) => new MarkedStringsOrMarkupContent(new MarkupContent
        {
            Kind = MarkupKind.Markdown,
            Value = markdown,
        });

        private static MarkedStringsOrMarkupContent WithMarkdown(IEnumerable<string> markdown)
            => new MarkedStringsOrMarkupContent(markdown.Select(md => new MarkedString(md)));

        protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}
