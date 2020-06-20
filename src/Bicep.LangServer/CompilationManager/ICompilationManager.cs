using System;

namespace Bicep.LanguageServer.CompilationManager
{
    public interface ICompilationManager
    {
        CompilationContext? UpsertCompilation(Uri uri, long version, string text);

        void CloseCompilation(Uri uri);

        CompilationContext? GetCompilation(Uri uri);
    }
}
