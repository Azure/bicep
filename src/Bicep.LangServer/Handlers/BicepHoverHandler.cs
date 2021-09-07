// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
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
            // all of the generated markdown includes the language id to avoid VS code rendering 
            // with multiple borders
            var compilation = result.Context.Compilation;
            switch (result.Symbol)
            {
                case ParameterSymbol parameter:
                    return CodeBlockWithDescriptionDecorator(
                        $"param {parameter.Name}: {parameter.Type}", parameter.DeclaringParameter.TryGetDecoratorSyntax("description"));

                case VariableSymbol variable:
                    return CodeBlockWithDescriptionDecorator($"var {variable.Name}: {variable.Type}", variable.DeclaringVariable.TryGetDecoratorSyntax("description"));

                case ResourceSymbol resource:
                    return CodeBlockWithDescriptionDecorator(
                        $"resource {resource.Name}\n{resource.Type}", resource.DeclaringResource.TryGetDecoratorSyntax("description"));

                case ModuleSymbol module:
                    var filePath = SyntaxHelper.TryGetModulePath(module.DeclaringModule, out _);
                    if (filePath != null)
                    {
                        return CodeBlockWithDescriptionDecorator($"module {module.Name}\n'{filePath}'", module.DeclaringModule.TryGetDecoratorSyntax("description"));
                    }

                    return CodeBlockWithDescriptionDecorator($"module {module.Name}", module.DeclaringModule.TryGetDecoratorSyntax("description"));

                case OutputSymbol output:
                    return CodeBlockWithDescriptionDecorator(
                        $"output {output.Name}: {output.Type}", output.DeclaringOutput.TryGetDecoratorSyntax("description"));

                case NamespaceSymbol namespaceSymbol:
                    return CodeBlock($"{namespaceSymbol.Name} namespace");

                case FunctionSymbol function when result.Origin is FunctionCallSyntax functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return CodeBlock(GetFunctionMarkdown(function, functionCall.Arguments, result.Origin, result.Context.Compilation.GetEntrypointSemanticModel()));

                case PropertySymbol property:
                    return $"{CodeBlockWithDescription($"{property.Name}: {property.Type}", property.Description)}";

                case FunctionSymbol function when result.Origin is InstanceFunctionCallSyntax functionCall:
                    return CodeBlock(
                        GetFunctionMarkdown(function, functionCall.Arguments, result.Origin, result.Context.Compilation.GetEntrypointSemanticModel()));

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
        $"```bicep\n{(content.Length > MaxHoverMarkdownCodeBlockLength ? content.Substring(0, MaxHoverMarkdownCodeBlockLength) : content)}\n```";
        
        // Markdown needs two leading whitespaces before newline to insert a line break
        private static string CodeBlockWithDescription(string markdown, string? description) =>  description is not null ? markdown + $"\n{description.Replace("\n", "  \n")}\n" : markdown;

        private static string CodeBlockWithDescriptionDecorator(string content, DecoratorSyntax? descriptionDecorator)
        {
            var markdown = CodeBlock(content);
            if (descriptionDecorator is not null &&
                descriptionDecorator.Arguments.FirstOrDefault()?.Expression is StringSyntax stringSyntax
                && stringSyntax.TryGetLiteralValue() is string description)
            {
                return CodeBlockWithDescription(markdown, description);
            }
            return markdown;
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

