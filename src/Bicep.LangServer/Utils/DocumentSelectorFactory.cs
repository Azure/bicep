using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Utils
{
    public class DocumentSelectorFactory
    {
        public static DocumentSelector Create() => DocumentSelector.ForLanguage(LanguageServerConstants.LanguageId);
    }
}
