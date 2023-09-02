// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepHoverHandler : HoverHandlerBase
    {
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IModuleRegistryProvider moduleRegistryProvider;
        private readonly ISymbolResolver symbolResolver;

        private const int MaxHoverMarkdownCodeBlockLength = 90000;
        //actual limit for hover in VS code is 100,000 characters.

        public BicepHoverHandler(
            IModuleDispatcher moduleDispatcher,
            IModuleRegistryProvider moduleRegistryProvider,
            ISymbolResolver symbolResolver)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.moduleRegistryProvider = moduleRegistryProvider;
            this.symbolResolver = symbolResolver;
        }

        public override async Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return null;
            }

            var markdown = await GetMarkdown(request, result, this.moduleDispatcher, this.moduleRegistryProvider);
            if (markdown == null)
            {
                return null;
            }

            return new Hover
            {
                Contents = markdown,
                Range = PositionHelper.GetNameRange(result.Context.LineStarts, result.Origin)
            };
        }

        private static string? TryGetDescriptionMarkdown(SymbolResolutionResult result, DeclaredSymbol symbol)
        {
            if (symbol.DeclaringSyntax is DecorableSyntax decorableSyntax &&
                DescriptionHelper.TryGetFromDecorator(
                    result.Context.Compilation.GetEntrypointSemanticModel(),
                    decorableSyntax) is { } description)
            {
                return description;
            }

            return null;
        }

        private static string? TryGetDescriptionMarkdown(SymbolResolutionResult result, WildcardImportSymbol symbol)
            => DescriptionHelper.TryGetFromDecorator(result.Context.Compilation.GetEntrypointSemanticModel(), symbol.EnclosingDeclaration);

        private static async Task<MarkedStringsOrMarkupContent?> GetMarkdown(
            HoverParams request,
            SymbolResolutionResult result,
            IModuleDispatcher moduleDispatcher,
            IModuleRegistryProvider moduleRegistryProvider)
        {
            // all of the generated markdown includes the language id to avoid VS code rendering
            // with multiple borders
            switch (result.Symbol)
            {
                case ProviderNamespaceSymbol provider:
                    return AsMarkdown(CodeBlockWithDescription(
                        $"import {provider.Name}", TryGetDescriptionMarkdown(result, provider)));

                case MetadataSymbol metadata:
                    return AsMarkdown(CodeBlockWithDescription(
                        $"metadata {metadata.Name}: {metadata.Type}", TryGetDescriptionMarkdown(result, metadata)));

                case ParameterSymbol parameter:
                    return AsMarkdown(CodeBlockWithDescription(
                        WithTypeModifiers($"param {parameter.Name}: {parameter.Type}", parameter.Type), TryGetDescriptionMarkdown(result, parameter)));

                case TypeAliasSymbol declaredType:
                    return AsMarkdown(CodeBlockWithDescription(
                        WithTypeModifiers($"type {declaredType.Name}: {declaredType.Type}", declaredType.Type), TryGetDescriptionMarkdown(result, declaredType)));

                case ImportedTypeSymbol importedType:
                    return AsMarkdown(CodeBlockWithDescription(
                        WithTypeModifiers($"type {importedType.Name}: {importedType.Type}", importedType.Type),
                        importedType.TryGetSemanticModel(out var model, out _) && model.ExportedTypes.TryGetValue(importedType.OriginalSymbolName, out var exportedTypeMetadata)
                            ? exportedTypeMetadata.Description
                            : null));

                case AmbientTypeSymbol ambientType:
                    return AsMarkdown(CodeBlock(WithTypeModifiers($"type {ambientType.Name}: {ambientType.Type}", ambientType.Type)));

                case VariableSymbol variable:
                    return AsMarkdown(CodeBlockWithDescription($"var {variable.Name}: {variable.Type}", TryGetDescriptionMarkdown(result, variable)));

                case ResourceSymbol resource:
                    var docsSuffix = TryGetTypeDocumentationLink(resource) is { } typeDocsLink ? MarkdownHelper.GetDocumentationLink(typeDocsLink) : "";
                    var description = TryGetDescriptionMarkdown(result, resource);

                    return AsMarkdown(CodeBlockWithDescription(
                        $"resource {resource.Name} {(resource.Type is ResourceType ? $"'{resource.Type}'" : resource.Type)}",
                        description is { } ? $"{description}\n{docsSuffix}" : docsSuffix));

                case ModuleSymbol module:
                    return await GetModuleMarkdown(request, result, moduleDispatcher, moduleRegistryProvider, module);

                case TestSymbol test:
                    return AsMarkdown(CodeBlockWithDescription($"test {test.Name}", TryGetDescriptionMarkdown(result, test)));
                case OutputSymbol output:
                    return AsMarkdown(CodeBlockWithDescription(
                        WithTypeModifiers($"output {output.Name}: {output.Type}", output.Type), TryGetDescriptionMarkdown(result, output)));

                case BuiltInNamespaceSymbol builtInNamespace:
                    return AsMarkdown(CodeBlock($"{builtInNamespace.Name} namespace"));

                case WildcardImportSymbol wildcardImport:
                    return AsMarkdown(CodeBlockWithDescription($"{wildcardImport.Name} namespace", TryGetDescriptionMarkdown(result, wildcardImport)));

                case IFunctionSymbol function when result.Origin is FunctionCallSyntaxBase functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return GetFunctionMarkdown(function, functionCall, result.Context.Compilation.GetEntrypointSemanticModel());

                case DeclaredFunctionSymbol function:
                    // A declared function can only have a single overload!
                    return AsMarkdown(GetFunctionOverloadMarkdown(function.Overloads.Single()));

                case PropertySymbol property:
                    return AsMarkdown(CodeBlockWithDescription(WithTypeModifiers($"{property.Name}: {property.Type}", property.Type), property.Description));

                case LocalVariableSymbol local:
                    return AsMarkdown(CodeBlock($"{local.Name}: {local.Type}"));

                case ParameterAssignmentSymbol parameterAssignment:
                    if (GetDeclaredParameterMetadata(parameterAssignment) is not ParameterMetadata declaredParamMetadata)
                    {
                        return null;
                    }

                    return AsMarkdown(CodeBlockWithDescription(
                        WithTypeModifiers($"param {parameterAssignment.Name}: {declaredParamMetadata.TypeReference.Type}", declaredParamMetadata.TypeReference.Type), declaredParamMetadata.Description));

                case AssertSymbol assert:
                    return AsMarkdown(CodeBlockWithDescription($"assert {assert.Name}: {assert.Type}", TryGetDescriptionMarkdown(result, assert)));

                default:
                    return null;
            }
        }

        private static async Task<MarkedStringsOrMarkupContent> GetModuleMarkdown(
            HoverParams request,
            SymbolResolutionResult result,
            IModuleDispatcher moduleDispatcher,
            IModuleRegistryProvider moduleRegistryProvider,
            ModuleSymbol module)
        {
            if (!SyntaxHelper.TryGetForeignTemplatePath(module.DeclaringModule, out var filePath, out _))
            {
                filePath = string.Empty;
            }
            var descriptionLines = new List<string?>();
            descriptionLines.Add(TryGetDescriptionMarkdown(result, module));

            var uri = request.TextDocument.Uri.ToUriEncoded();
            var registries = moduleRegistryProvider.Registries(uri);

            if (registries != null &&
                registries.Any() &&
                moduleDispatcher.TryGetModuleReference(module.DeclaringModule, uri, out var moduleReference, out _) &&
                moduleReference is not null)
            {
                var registry = registries.FirstOrDefault(r => r.Scheme == moduleReference.Scheme);
                if (registry is not null)
                {
                    try
                    {
                        descriptionLines.Add(await registry.TryGetDescription(moduleReference));
                    }
                    catch
                    {
                        // ignore
                    }

                    try
                    {
                        if (registry.GetDocumentationUri(moduleReference) is string documentationUri && !string.IsNullOrEmpty(documentationUri))
                        {
                            descriptionLines.Add(MarkdownHelper.GetDocumentationLink(documentationUri));
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }

            var descriptions = string.Join("\n\n", descriptionLines.WhereNotNull());
            return AsMarkdown(CodeBlockWithDescription($"module {module.Name} '{filePath}'", descriptions));
        }

        private static ParameterMetadata? GetDeclaredParameterMetadata(ParameterAssignmentSymbol symbol)
        {
            if (!symbol.Context.Compilation.GetEntrypointSemanticModel().Root.TryGetBicepFileSemanticModelViaUsing(out var semanticModel, out _))
            {
                // failed to resolve using
                return null;
            }

            if (semanticModel.Parameters.TryGetValue(symbol.Name, out var parameterMetadata))
            {
                return parameterMetadata;
            }

            return null;
        }


        private static string WithTypeModifiers(string coreContent, TypeSymbol type)
        {
            type = UnwrapType(type);

            StringBuilder contentBuilder = new();
            switch (type)
            {
                case IntegerType integer:
                    if (integer.MinValue.HasValue)
                    {
                        contentBuilder.Append("@minValue(").Append(integer.MinValue.Value).Append(")\n");
                    }
                    if (integer.MaxValue.HasValue)
                    {
                        contentBuilder.Append("@maxValue(").Append(integer.MaxValue.Value).Append(")\n");
                    }
                    break;
                case StringType @string:
                    if (@string.MinLength.HasValue)
                    {
                        contentBuilder.Append("@minLength(").Append(@string.MinLength.Value).Append(")\n");
                    }
                    if (@string.MaxLength.HasValue)
                    {
                        contentBuilder.Append("@maxLength(").Append(@string.MaxLength.Value).Append(")\n");
                    }
                    break;
                case ArrayType array when array is not TupleType:
                    if (array.MinLength.HasValue)
                    {
                        contentBuilder.Append("@minLength(").Append(array.MinLength.Value).Append(")\n");
                    }
                    if (array.MaxLength.HasValue)
                    {
                        contentBuilder.Append("@maxLength(").Append(array.MaxLength.Value).Append(")\n");
                    }
                    break;
            }

            if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                contentBuilder.Append("@secure()\n");
            }

            contentBuilder.Append(coreContent);

            return contentBuilder.ToString();
        }

        private static TypeSymbol UnwrapType(TypeSymbol type) => type switch
        {
            TypeType tt => UnwrapType(tt.Unwrapped),
            _ when TypeHelper.TryRemoveNullability(type) is { } nonNullable => UnwrapType(nonNullable),
            _ => type,
        };

        //we need to check for overflow due to using code blocks.
        //if we reach limit in a code block vscode will truncate it automatically, the block will not be terminated so the hover will not be properly formatted
        //therefore we need to check for the limit ourselves and truncate text inside code block, making sure it's terminated properly.
        private static string CodeBlock(string content) =>
        $"```bicep\n{(content.Length > MaxHoverMarkdownCodeBlockLength ? content.Substring(0, MaxHoverMarkdownCodeBlockLength) : content)}\n```\n";

        // Markdown needs two leading whitespaces before newline to insert a line break
        private static string CodeBlockWithDescription(string content, string? description) =>
            CodeBlock(content) + (!string.IsNullOrEmpty(description) ? $"{description.Replace("\n", "  \n")}\n" : string.Empty);

        private static MarkedStringsOrMarkupContent GetFunctionMarkdown(IFunctionSymbol function, FunctionCallSyntaxBase functionCall, SemanticModel model)
        {
            if (model.TypeManager.GetMatchedFunctionOverload(functionCall) is { } matchedOverload)
            {
                return AsMarkdown(GetFunctionOverloadMarkdown(matchedOverload));
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

            return AsMarkdown(toShow.Select(GetFunctionOverloadMarkdown));
        }

        private static string GetFunctionOverloadMarkdown(FunctionOverload overload)
            => CodeBlockWithDescription($"function {overload.Name}{overload.TypeSignature}", overload.Description);

        private static string? TryGetTypeDocumentationLink(ResourceSymbol resource)
        {
            if (resource.TryGetResourceType() is { } resourceType &&
                resourceType.DeclaringNamespace.ProviderNameEquals(AzNamespaceType.BuiltInName) &&
                resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
            {
                var provider = resourceType.TypeReference.TypeSegments.First().ToLowerInvariant();
                var typePath = resourceType.TypeReference.TypeSegments.Skip(1).Select(x => x.ToLowerInvariant());

                return $"https://learn.microsoft.com/azure/templates/{provider}/{string.Join('/', typePath)}?pivots=deployment-language-bicep";
            }

            return null;
        }

        private static MarkedStringsOrMarkupContent AsMarkdown(string markdown) => new MarkedStringsOrMarkupContent(new MarkupContent
        {
            Kind = MarkupKind.Markdown,
            Value = markdown,
        });

        private static MarkedStringsOrMarkupContent AsMarkdown(IEnumerable<string> markdown)
            => new MarkedStringsOrMarkupContent(markdown.Select(md => new MarkedString(md)));

        protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.CreateForBicepAndParams()
        };
    }
}
