using System;
using Bicep.Core.SemanticModel;

namespace Bicep.LanguageServer
{
    public interface ICompilationManager
    {
        Compilation? UpsertCompilation(Uri uri, long version, string text);

        void CloseCompilation(Uri uri);

        Compilation? GetCompilation(Uri uri);
    }
}
