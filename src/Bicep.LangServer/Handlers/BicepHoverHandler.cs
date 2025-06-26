// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepHoverHandler : HoverHandlerBase
    {
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IArtifactRegistryProvider moduleRegistryProvider;
        private readonly ISymbolResolver symbolResolver;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepHoverHandler(
            IModuleDispatcher moduleDispatcher,
            IArtifactRegistryProvider moduleRegistryProvider,
            ISymbolResolver symbolResolver,
            DocumentSelectorFactory documentSelectorFactory)
        {
            this.moduleDispatcher = moduleDispatcher;
            this.moduleRegistryProvider = moduleRegistryProvider;
            this.symbolResolver = symbolResolver;
            this.documentSelectorFactory = documentSelectorFactory;
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

        private static string? TryGetDescription(SymbolResolutionResult result, DeclaredSymbol symbol)
        {
            if (symbol.DeclaringSyntax is DecorableSyntax decorableSyntax)
            {
                return DescriptionHelper.TryGetFromDecorator(result.Context.Compilation.GetEntrypointSemanticModel(), decorableSyntax);
            }

            return null;
        }

        private static string? TryGetDescription(SymbolResolutionResult result, WildcardImportSymbol symbol)
            => DescriptionHelper.TryGetFromDecorator(result.Context.Compilation.GetEntrypointSemanticModel(), symbol.EnclosingDeclaration);

        private static async Task<MarkedStringsOrMarkupContent?> GetMarkdown(
            HoverParams request,
            SymbolResolutionResult result,
            IModuleDispatcher moduleDispatcher,
            IArtifactRegistryProvider moduleRegistryProvider)
        {
            // all of the generated markdown includes the language id to avoid VS code rendering
            // with multiple borders
            switch (result.Symbol)
            {
                case ExtensionNamespaceSymbol extension:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        $"{LanguageConstants.ExtensionKeyword} {extension.Name}", TryGetDescription(result, extension)));

                case MetadataSymbol metadata:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        $"metadata {metadata.Name}: {metadata.Type}", TryGetDescription(result, metadata)));

                case ParameterSymbol parameter:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        WithTypeModifiers($"param {parameter.Name}: {parameter.Type}", parameter.Type), TryGetDescription(result, parameter)));

                case TypeAliasSymbol declaredType:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        WithTypeModifiers($"type {declaredType.Name}: {declaredType.Type}", declaredType.Type), TryGetDescription(result, declaredType)));

                case ImportedTypeSymbol importedType:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        WithTypeModifiers($"type {importedType.Name}: {importedType.Type}", importedType.Type),
                        importedType.Description));

                case ImportedVariableSymbol importedVariable:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription($"var {importedVariable.Name}: {importedVariable.Type}", importedVariable.Description));

                case AmbientTypeSymbol ambientType:
                    return AsMarkdown(MarkdownHelper.CodeBlock(WithTypeModifiers($"type {ambientType.Name}: {ambientType.Type}", ambientType.Type)));

                case VariableSymbol variable:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription($"var {variable.Name}: {variable.Type}", TryGetDescription(result, variable)));

                case ResourceSymbol resource:
                    var docsSuffix = TryGetTypeDocumentationLink(resource) is { } typeDocsLink ? MarkdownHelper.GetDocumentationLink(typeDocsLink) : "";
                    var description = TryGetDescription(result, resource);

                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        $"resource {resource.Name} {(resource.Type is ResourceType ? $"'{resource.Type}'" : resource.Type)}",
                        $"{MarkdownHelper.AppendNewline(description)}{docsSuffix}"));

                case ModuleSymbol module:
                    return await GetModuleMarkdown(request, result, moduleDispatcher, moduleRegistryProvider, module);

                case TestSymbol test:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription($"test {test.Name}", TryGetDescription(result, test)));
                case OutputSymbol output:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        WithTypeModifiers($"output {output.Name}: {output.Type}", output.Type), TryGetDescription(result, output)));

                case BuiltInNamespaceSymbol builtInNamespace:
                    return AsMarkdown(MarkdownHelper.CodeBlock($"{builtInNamespace.Name} namespace"));

                case WildcardImportSymbol wildcardImport:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription($"{wildcardImport.Name} namespace", TryGetDescription(result, wildcardImport)));

                case IFunctionSymbol function when result.Origin is FunctionCallSyntaxBase functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return GetFunctionMarkdown(function, functionCall, result.Context.Compilation.GetEntrypointSemanticModel());

                case DeclaredFunctionSymbol function:
                    return AsMarkdown(GetFunctionOverloadMarkdown(function.Overload));

                case ImportedFunctionSymbol importedFunction:
                    return AsMarkdown(GetFunctionOverloadMarkdown(importedFunction.Overload));

                case PropertySymbol property:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(WithTypeModifiers($"{property.Name}: {property.Type}", property.Type), property.Description));

                case LocalVariableSymbol local:
                    return AsMarkdown(MarkdownHelper.CodeBlock($"{local.Name}: {local.Type}"));

                case ParameterAssignmentSymbol parameterAssignment:
                    if (GetBicepFileSemanticModelOrDefault(parameterAssignment)?.Parameters.TryGetValue(parameterAssignment.Name, out var declaredParamMetadata) is not true)
                    {
                        return null;
                    }

                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription(
                        WithTypeModifiers($"param {parameterAssignment.Name}: {declaredParamMetadata.TypeReference.Type}", declaredParamMetadata.TypeReference.Type), declaredParamMetadata.Description));

                case ExtensionConfigAssignmentSymbol extensionConfigAssignment:
                    if (GetBicepFileSemanticModelOrDefault(extensionConfigAssignment)?.Extensions.TryGetValue(extensionConfigAssignment.Name, out var declaredExtMetadata) is not true || declaredExtMetadata.ConfigType is null)
                    {
                        return null;
                    }

                    return AsMarkdown(MarkdownHelper.CodeBlock(WithTypeModifiers($"{LanguageConstants.ExtensionConfigKeyword} {declaredExtMetadata.Alias}: {declaredExtMetadata.ConfigType}", declaredExtMetadata.ConfigType)));
                case AssertSymbol assert:
                    return AsMarkdown(MarkdownHelper.CodeBlockWithDescription($"assert {assert.Name}: {assert.Type}", TryGetDescription(result, assert)));

                default:
                    return null;
            }
        }

        private static async Task<MarkedStringsOrMarkupContent> GetModuleMarkdown(
            HoverParams request,
            SymbolResolutionResult result,
            IModuleDispatcher moduleDispatcher,
            IArtifactRegistryProvider moduleRegistryProvider,
            ModuleSymbol module)
        {
            var filePath = module.DeclaringModule.TryGetPath()?.TryGetLiteralValue() is string modulePath ? modulePath : string.Empty;
            var descriptionLines = new List<string?>
            {
                TryGetDescription(result, module)
            };

            if (moduleDispatcher.TryGetArtifactReference(module.Context.SourceFile, module.DeclaringModule).IsSuccess(out var moduleReference) &&
                moduleReference is not null)
            {
                var registry = moduleRegistryProvider.TryGetRegistry(moduleReference.Scheme);
                if (registry is not null)
                {
                    try
                    {
                        descriptionLines.Add(await registry.TryGetModuleDescription(module, moduleReference));
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

            var descriptions = MarkdownHelper.JoinWithNewlines(descriptionLines.WhereNotNull());
            return AsMarkdown(MarkdownHelper.CodeBlockWithDescription($"module {module.Name} '{filePath}'", descriptions));
        }

        private static ISemanticModel? GetBicepFileSemanticModelOrDefault(DeclaredSymbol symbol)
            => symbol.Context.Binder.FileSymbol.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var bicepFileModel) ? bicepFileModel : null;

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
            => MarkdownHelper.CodeBlockWithDescription($"function {overload.Name}{overload.TypeSignature}", overload.Description);

        private static string? TryGetTypeDocumentationLink(ResourceSymbol resource)
        {
            if (resource.TryGetResourceType() is { } resourceType &&
                resourceType.DeclaringNamespace.ExtensionNameEquals(AzNamespaceType.BuiltInName) &&
                resourceType.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(resourceType.TypeReference))
            {
                var provider = resourceType.TypeReference.TypeSegments.First().ToLowerInvariant();
                var typePath = resourceType.TypeReference.TypeSegments.Skip(1).Select(x => x.ToLowerInvariant());

                return $"https://learn.microsoft.com/azure/templates/{provider}/{string.Join('/', typePath)}?pivots=deployment-language-bicep";
            }

            return null;
        }

        private static MarkedStringsOrMarkupContent AsMarkdown(string markdown) => new(new MarkupContent
        {
            Kind = MarkupKind.Markdown,
            Value = markdown,
        });

        private static MarkedStringsOrMarkupContent AsMarkdown(IEnumerable<string> markdown)
            => new(markdown.Select(md => new MarkedString(md)));

        protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams()
        };
    }
}
