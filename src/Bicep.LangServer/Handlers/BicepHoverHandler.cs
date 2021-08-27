// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
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
        private readonly ICompilationManager compilationManager;

        private const int MaxHoverMarkdownCodeBlockLength = 90000;
        //actual limit for hover in VS code is 100,000 characters.

        public BicepHoverHandler(ISymbolResolver symbolResolver, ICompilationManager compilationManager)
        {
            this.symbolResolver = symbolResolver;
            this.compilationManager = compilationManager;
        }

        public override Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
        {
            var result = this.symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);
            if (result == null)
            {
                return Task.FromResult<Hover?>(null);
            }
            
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context == null)
            {
                return Task.FromResult<Hover?>(null);
            }
            var markdown = GetMarkdown(result, context);
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

        private static string? GetMarkdown(SymbolResolutionResult result, CompilationContext context)
        {
            // all of the generated markdown includes the language id to avoid VS code rendering 
            // with multiple borders
            switch (result.Symbol)
            {
                case ParameterSymbol parameter:
                    return CodeBlock(
                        $"param {parameter.Name}: {parameter.Type}", GetDescription(parameter.DeclaringParameter, context));

                case VariableSymbol variable:
                    return CodeBlock($"var {variable.Name}: {variable.Type}", GetDescription(variable.DeclaringVariable, context));

                case ResourceSymbol resource:
                    return CodeBlock(
                        $"resource {resource.Name}\n{resource.Type}", GetDescription(resource.DeclaringResource, context));

                case ModuleSymbol module:
                    var filePath = SyntaxHelper.TryGetModulePath(module.DeclaringModule, out _);
                    if (filePath != null)
                    {
                        return CodeBlock($"module {module.Name}\n'{filePath}'", GetDescription(module.DeclaringModule, context));
                    }

                    return CodeBlock($"module {module.Name}", GetDescription(module.DeclaringModule, context));

                case OutputSymbol output:
                    return CodeBlock(
                        $"output {output.Name}: {output.Type}", GetDescription(output.DeclaringOutput, context));

                case NamespaceSymbol namespaceSymbol:
                    return CodeBlock($"{namespaceSymbol.Name} namespace", null);

                case FunctionSymbol function when result.Origin is FunctionCallSyntax functionCall:
                    // it's not possible for a non-function call syntax to resolve to a function symbol
                    // but this simplifies the checks
                    return CodeBlock(GetFunctionMarkdown(function, functionCall.Arguments, result.Origin, result.Context.Compilation.GetEntrypointSemanticModel()), null);

                case PropertySymbol property:
                    return $"{CodeBlock($"{property.Name}: {property.Type}", property.Description)}";

                case FunctionSymbol function when result.Origin is InstanceFunctionCallSyntax functionCall:
                    return CodeBlock(
                        GetFunctionMarkdown(function, functionCall.Arguments, result.Origin, result.Context.Compilation.GetEntrypointSemanticModel()), null);

                case LocalVariableSymbol local:
                    return CodeBlock($"{local.Name}: {local.Type}", null);

                default:
                    return null;
            }
        }

        private static string CodeBlock(string content, string? description)
        {
            //we need to check for overflow due to using code blocks.
            //if we reach limit in a code block vscode will truncate it automatically, the block will not be terminated so the hover will not be properly formatted
            //therefore we need to check for the limit ourselves and truncate text inside code block, making sure it's terminated properly.
            var markdown = $"```bicep\n{(content.Length > MaxHoverMarkdownCodeBlockLength ? content.Substring(0, MaxHoverMarkdownCodeBlockLength) : content)}\n```";
            if (description is not null)
            {
                markdown += $"\n{description}\n";
            }
            return markdown;
        }

        private static string? GetDescription(StatementSyntax statement, CompilationContext context)
        {
            var semanticModel = context.Compilation.GetEntrypointSemanticModel();
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);
            if (statement.Decorators
                .Select(decoratorSyntax => decoratorSyntax.Expression)
                .OfType<FunctionCallSyntax>()
                .Where(function => string.Equals(function.Name.IdentifierName, "description", System.StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault() is FunctionCallSyntax descriptionSyntax
                && descriptionSyntax.Arguments.FirstOrDefault()?.Expression is StringSyntax stringSyntax
                && stringSyntax.TryGetLiteralValue() is string description)
            {
                // markdown requires two spaces before newline to add a linebreak.
                return description.Replace("\n", "  \n") + "\n";
            }
            return null;
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

