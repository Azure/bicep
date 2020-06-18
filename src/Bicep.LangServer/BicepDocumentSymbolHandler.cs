using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer
{
    public class BicepDocumentSymbolHandler: DocumentSymbolHandler
    {
        public BicepDocumentSymbolHandler(ProgressManager progressManager)
            : base(GetSymbolRegistrationOptions(), progressManager)
        {
        }

        public override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(new SymbolInformationOrDocumentSymbol()));
        }

        private static DocumentSymbolRegistrationOptions GetSymbolRegistrationOptions()
        {
            return new DocumentSymbolRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage("bicep")
            };
        }
    }
}
