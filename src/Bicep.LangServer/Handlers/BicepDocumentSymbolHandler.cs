// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentSymbolHandler : DocumentSymbolHandlerBase
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;
        private readonly ICompilationManager compilationManager;

        public BicepDocumentSymbolHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            CompilationContext? context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("Document symbol request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer());
            }

            return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(GetSymbols(context)));
        }

        private IEnumerable<SymbolInformationOrDocumentSymbol> GetSymbols(CompilationContext context)
        {
            var model = context.Compilation.GetEntrypointSemanticModel();
            return model.Root.Declarations
                .OrderBy(symbol => symbol.DeclaringSyntax.Span.Position)
                .Select(symbol => new SymbolInformationOrDocumentSymbol(CreateDocumentSymbol(model, symbol, context.LineStarts)));
        }

        private DocumentSymbol CreateDocumentSymbol(SemanticModel model, DeclaredSymbol symbol, ImmutableArray<int> lineStarts)
        {
            var children = Enumerable.Empty<DocumentSymbol>();
            if (symbol is ResourceSymbol resourceSymbol &&
                resourceSymbol.DeclaringResource.TryGetBody() is ObjectSyntax body)
            {
                children = body.Resources
                    .Select(r => model.GetSymbolInfo(r) as ResourceSymbol)
                    .Where(s => s is ResourceSymbol)
                    .Select(s => CreateDocumentSymbol(model, s!, lineStarts));
            }

            return new DocumentSymbol
            {
                Name = symbol.Name,
                Kind = SelectSymbolKind(symbol),
                Detail = FormatDetail(symbol),
                Children = new Container<DocumentSymbol>(children),
                Range = symbol.DeclaringSyntax.ToRange(lineStarts),
                // use the name node span with fallback to entire declaration span
                SelectionRange = symbol.NameSyntax.ToRange(lineStarts)
            };
        }


        private SymbolKind SelectSymbolKind(DeclaredSymbol symbol)
        {
            switch (symbol)
            {
                case ImportedNamespaceSymbol _:
                    return SymbolKind.Namespace;

                case ParameterSymbol _:
                    return SymbolKind.Field;

                case VariableSymbol _:
                    return SymbolKind.Variable;

                case ResourceSymbol resource:
                    return SymbolKind.Object;

                case ModuleSymbol module:
                    return SymbolKind.Module;

                case OutputSymbol output:
                    return SymbolKind.Interface;

                default:
                    return SymbolKind.Key;
            }
        }

        private string FormatDetail(DeclaredSymbol symbol)
        {
            switch (symbol)
            {
                case ParameterSymbol parameter:
                    return parameter.Type.Name;

                case VariableSymbol variable:
                    return variable.Type.Name;

                case ResourceSymbol resource:
                    return resource.Type.Name;

                case ModuleSymbol module:
                    return module.Type.Name;

                case OutputSymbol output:
                    return output.Type.Name;

                default:
                    return string.Empty;
            }
        }

        protected override DocumentSymbolRegistrationOptions CreateRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.Create()
        };
    }
}

