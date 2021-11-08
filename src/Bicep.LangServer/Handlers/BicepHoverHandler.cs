// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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
                Contents = new MarkedStringsOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = markdown
                }),
                Range = PositionHelper.GetNameRange(result.Context.LineStarts, result.Origin)
            });
        }

        private static string? GetMarkdown(HoverParams request, SymbolResolutionResult result)
        {
            // all of the generated markdown includes the language id to avoid VS code rendering 
            // with multiple borders
            switch (result.Symbol)
            {
                case ImportedNamespaceSymbol import:
                    return CodeBlockWithDescription(
                        $"import {import.Name}", SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), import.DeclaringImport));

                case ParameterSymbol parameter:
                    return CodeBlockWithDescription(
                        $"param {parameter.Name}: {parameter.Type}",  SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), parameter.DeclaringParameter));

                case VariableSymbol variable:
                    return CodeBlockWithDescription($"var {variable.Name}: {variable.Type}", SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), variable.DeclaringVariable));

                case ResourceSymbol resource:
                    return CodeBlockWithDescription(
                        $"resource {resource.Name}\n{resource.Type}", SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), resource.DeclaringResource));

                case ModuleSymbol module:
                    var filePath = SyntaxHelper.TryGetModulePath(module.DeclaringModule, out _);
                    if (filePath != null)
                    {
                        return CodeBlockWithDescription($"module {module.Name}\n'{filePath}'", SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), module.DeclaringModule));
                    }

                    return CodeBlockWithDescription($"module {module.Name}", SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), module.DeclaringModule));

                case OutputSymbol output:
                    return CodeBlockWithDescription(
                        $"output {output.Name}: {output.Type}",  SemanticModelHelper.TryGetDescription(result.Context.Compilation.GetEntrypointSemanticModel(), output.DeclaringOutput));

                case BuiltInNamespaceSymbol builtInNamespace:
                    return CodeBlock($"{builtInNamespace.Name} namespace");

                case FunctionSymbol function when result.Origin is FunctionCallSyntaxBase functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return GetFunctionMarkdown(function, functionCall, result.Context.Compilation.GetEntrypointSemanticModel());

                case PropertySymbol property:
                    return CodeBlockWithDescription($"{property.Name}: {property.Type}", property.Description);

                case LocalVariableSymbol local:
                    return CodeBlock($"{local.Name}: {local.Type}");

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

        private static string GetFunctionMarkdown(FunctionSymbol function, FunctionCallSyntaxBase functionCall, SemanticModel model)
        {
            var buffer = new StringBuilder();
            buffer.Append($"function ");
            buffer.Append(function.Name);
            buffer.Append('(');

            const string argumentSeparator = ", ";
            foreach (FunctionArgumentSyntax argumentSyntax in functionCall.Arguments)
            {
                var argumentType = model.GetTypeInfo(argumentSyntax);
                buffer.Append(argumentType);

                buffer.Append(argumentSeparator);
            }

            // remove trailing argument separator (if any)
            if (functionCall.Arguments.Length > 0)
            {
                buffer.Remove(buffer.Length - argumentSeparator.Length, argumentSeparator.Length);
            }

            buffer.Append("): ");
            buffer.Append(model.GetTypeInfo(functionCall));

            if (model.TypeManager.GetMatchedFunctionOverload(functionCall) is {} matchedOverload)
            {
                return CodeBlockWithDescription(buffer.ToString(), matchedOverload.Description);
            }

            // TODO fall back to displaying a more generic description if unable to resolve a particular overload, once https://github.com/Azure/bicep/issues/4588 has been implemented.
            return CodeBlock(buffer.ToString());
        }

        protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}

