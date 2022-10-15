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
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context is null)
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


        private static SymbolKind SelectSymbolKind(DeclaredSymbol symbol) => symbol switch
        {
            ImportedNamespaceSymbol => SymbolKind.Namespace,
            ParameterSymbol => SymbolKind.Field,
            VariableSymbol => SymbolKind.Variable,
            ResourceSymbol => SymbolKind.Object,
            ModuleSymbol => SymbolKind.Module,
            OutputSymbol => SymbolKind.Interface,
            ParameterAssignmentSymbol => SymbolKind.Constant,
            _ => SymbolKind.Key,
        };

        private static string FormatDetail(DeclaredSymbol symbol) => symbol switch
        {
            ParameterSymbol parameter => parameter.Type.Name,
            VariableSymbol variable => variable.Type.Name,
            ResourceSymbol resource => resource.Type.Name,
            ModuleSymbol module => module.Type.Name,
            OutputSymbol output => output.Type.Name,
            ParameterAssignmentSymbol paramAssignment => paramAssignment.Type.Name,
            _ => string.Empty,
        };

        protected override DocumentSymbolRegistrationOptions CreateRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = DocumentSelectorFactory.CreateForBicepAndParams()
        };
    }
}

