using System;
using Bicep.Core.SemanticModel;

namespace Bicep.LanguageServer
{
    public interface ICompilationManager
    {
        CompilationContext? UpsertCompilation(Uri uri, long version, string text);

        void CloseCompilation(Uri uri);

        CompilationContext? GetCompilation(Uri uri);
    }
}
