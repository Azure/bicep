// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
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
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepDocumentSymbolHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager, DocumentSelectorFactory documentSelectorFactory)
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override async Task<SymbolInformationOrDocumentSymbolContainer?> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context is null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("Document symbol request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return null;
            }

            return new(GetSymbols(context));
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
                SelectionRange = symbol.NameSource.ToRange(lineStarts)
            };
        }


        private static SymbolKind SelectSymbolKind(DeclaredSymbol symbol) => symbol switch
        {
            ExtensionNamespaceSymbol => SymbolKind.Namespace,
            ParameterSymbol => SymbolKind.Field,
            TypeAliasSymbol => SymbolKind.Field,
            VariableSymbol => SymbolKind.Variable,
            DeclaredFunctionSymbol => SymbolKind.Function,
            ResourceSymbol => SymbolKind.Object,
            ModuleSymbol => SymbolKind.Module,
            OutputSymbol => SymbolKind.Interface,
            ParameterAssignmentSymbol => SymbolKind.Constant,
            ExtensionConfigAssignmentSymbol => SymbolKind.Constant,
            AssertSymbol => SymbolKind.Boolean,
            _ => SymbolKind.Key,
        };

        private static string FormatDetail(DeclaredSymbol symbol) => symbol switch
        {
            ParameterSymbol parameter => parameter.Type.Name,
            TypeAliasSymbol declaredType => declaredType.Type.Name,
            VariableSymbol variable => variable.Type.Name,
            DeclaredFunctionSymbol func => func.Type.Name,
            ResourceSymbol resource => resource.Type.Name,
            ModuleSymbol module => module.Type.Name,
            OutputSymbol output => output.Type.Name,
            ParameterAssignmentSymbol paramAssignment => paramAssignment.Type.Name,
            ExtensionConfigAssignmentSymbol extConfigAssignment => extConfigAssignment.Type.Name,
            AssertSymbol assert => assert.Type.Name,
            _ => string.Empty,
        };

        protected override DocumentSymbolRegistrationOptions CreateRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams()
        };
    }
}
