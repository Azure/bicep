// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
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

            var markdown = GetMarkdown(result);
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

        private static string? GetMarkdown(SymbolResolutionResult result)
        {
            //we need to check for overflow due to using code blocks.
            //if we reach limit in a code block vscode will truncate it automatically, the block will not be terminated so the hover will not be properly formatted
            //therefore we need to check for the limit ourselves and truncate text inside code block, making sure it's terminated properly.
            static string CodeBlock(string content) => $"```bicep\n{(content.Length > MaxHoverMarkdownCodeBlockLength ? content.Substring(0, MaxHoverMarkdownCodeBlockLength) : content)}\n```";

            // all of the generated markdown includes the language id to avoid VS code rendering 
            // with multiple borders
            switch (result.Symbol)
            {
                case ParameterSymbol parameter:
                    return CodeBlock($"param {parameter.Name}: {parameter.Type}");

                case VariableSymbol variable:
                    return CodeBlock($"var {variable.Name}: {variable.Type}");

                case ResourceSymbol resource:
                    return CodeBlock($"resource {resource.Name}\n{resource.Type}");

                case ModuleSymbol module:
                    var filePath = SyntaxHelper.TryGetModulePath(module.DeclaringModule, out _);
                    if (filePath != null)
                    {
                        return CodeBlock($"module {module.Name}\n{filePath}");
                    }

                    return CodeBlock($"module {module.Name}");

                case OutputSymbol output:
                    return CodeBlock($"output {output.Name}: {output.Type}");

                case NamespaceSymbol namespaceSymbol:
                    return CodeBlock($"{namespaceSymbol.Name} namespace");

                case FunctionSymbol function when result.Origin is FunctionCallSyntax functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return CodeBlock(GetFunctionMarkdown(function, functionCall.Arguments, result.Origin, result.Context.Compilation.GetEntrypointSemanticModel()));

                case PropertySymbol property:
                    var markdown = $"{CodeBlock($"{property.Name}: {property.Type}")}\n";
                    if (property.Description is not null)
                    {
                        markdown += $"{property.Description}\n";
                    }

                    return markdown;

                case FunctionSymbol function when result.Origin is InstanceFunctionCallSyntax functionCall:
                    return CodeBlock(GetFunctionMarkdown(function, functionCall.Arguments, result.Origin, result.Context.Compilation.GetEntrypointSemanticModel()));

                case LocalVariableSymbol local:
                    return CodeBlock($"{local.Name}: {local.Type}");

                default:
                    return null;
            }
        }

        private static string GetFunctionMarkdown(FunctionSymbol function, ImmutableArray<FunctionArgumentSyntax> arguments, SyntaxBase functionCall, SemanticModel model)
        {
            var buffer = new StringBuilder();
            buffer.Append($"function ");
            buffer.Append(function.Name);
            buffer.Append('(');

            const string argumentSeparator = ", ";
            foreach (FunctionArgumentSyntax argumentSyntax in arguments)
            {
                var argumentType = model.GetTypeInfo(argumentSyntax);
                buffer.Append(argumentType);

                buffer.Append(argumentSeparator);
            }

            // remove trailing argument separator (if any)
            if (arguments.Length > 0)
            {
                buffer.Remove(buffer.Length - argumentSeparator.Length, argumentSeparator.Length);
            }

            buffer.Append("): ");
            buffer.Append(model.GetTypeInfo(functionCall));

            return buffer.ToString();
        }

        protected override HoverRegistrationOptions CreateRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}

