using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer
{
    public class BicepDocumentSymbolHandler: DocumentSymbolHandler
    {
        private readonly ICompilationManager compilationManager;

        public BicepDocumentSymbolHandler(ICompilationManager compilationManager, ProgressManager progressManager)
            : base(GetSymbolRegistrationOptions(), progressManager)
        {
            this.compilationManager = compilationManager;
        }

        public override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(new SymbolInformationOrDocumentSymbol()));
        }

        private static DocumentSymbolRegistrationOptions GetSymbolRegistrationOptions()
        {
            return new DocumentSymbolRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage(LanguageServerConstants.LanguageId)
            };
        }
    }
}
