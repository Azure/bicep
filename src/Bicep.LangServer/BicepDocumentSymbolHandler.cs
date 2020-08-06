﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.SemanticModel;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;

namespace Bicep.LanguageServer
{
    public class BicepDocumentSymbolHandler: DocumentSymbolHandler
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;
        private readonly ICompilationManager compilationManager;

        public BicepDocumentSymbolHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager)
            : base(GetSymbolRegistrationOptions())
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

                return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(new SymbolInformationOrDocumentSymbol()));
            }

            return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(GetSymbols(context)));
        }

        private static DocumentSymbolRegistrationOptions GetSymbolRegistrationOptions()
        {
            return new DocumentSymbolRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage(LanguageServerConstants.LanguageId)
            };
        }

        private IEnumerable<SymbolInformationOrDocumentSymbol> GetSymbols(CompilationContext context)
        {
            return context.Compilation.GetSemanticModel().Root.AllDeclarations
                .OrderBy(symbol=>symbol.DeclaringSyntax.Span.Position)
                .Select(symbol => new SymbolInformationOrDocumentSymbol(CreateDocumentSymbol(symbol, context.LineStarts)));
        }

        private DocumentSymbol CreateDocumentSymbol(DeclaredSymbol symbol, ImmutableArray<int> lineStarts) =>
            new DocumentSymbol
            {
                Name = symbol.Name,
                Kind = SelectSymbolKind(symbol),
                Detail = FormatDetail(symbol),
                Range = symbol.DeclaringSyntax.ToRange(lineStarts),
                // use the name node span with fallback to entire declaration span
                SelectionRange = (symbol.NameSyntax ?? symbol.DeclaringSyntax).ToRange(lineStarts)
            };

        private SymbolKind SelectSymbolKind(DeclaredSymbol symbol)
        {
            switch (symbol)
            {
                case ParameterSymbol _:
                    return SymbolKind.Field;

                case VariableSymbol _:
                    return SymbolKind.Variable;

                case ResourceSymbol resource:
                    return SymbolKind.Object;

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

                case OutputSymbol output:
                    return output.Type.Name;

                default:
                    return string.Empty;
            }
        }
    }
}
