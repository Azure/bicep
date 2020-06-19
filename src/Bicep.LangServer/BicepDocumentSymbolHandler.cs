using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.Extensions;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using SymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;

namespace Bicep.LanguageServer
{
    public class BicepDocumentSymbolHandler: DocumentSymbolHandler
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;
        private readonly ICompilationManager compilationManager;

        public BicepDocumentSymbolHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager, ProgressManager progressManager)
            : base(GetSymbolRegistrationOptions(), progressManager)
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
            // TODO: Add descendants so this is automatic
            return context.Compilation.GetSemanticModel().Root.ParameterDeclarations
                .Select(symbol => new SymbolInformationOrDocumentSymbol(new DocumentSymbol
                {
                    Name = symbol.Name,
                    Detail = $": {symbol.Type.Name}",
                    Kind = SymbolKind.Field,
                    Range = symbol.DeclaringSyntax.ToRange(context.LineStarts),
                    // TODO: Use name syntax node here
                    SelectionRange = symbol.DeclaringSyntax.ToRange(context.LineStarts)
                }));
        }
    }
}
