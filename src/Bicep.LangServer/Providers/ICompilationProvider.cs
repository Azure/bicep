using Bicep.LanguageServer.CompilationManager;

namespace Bicep.LanguageServer.Providers
{
    public interface ICompilationProvider
    {
        CompilationContext Create(string text);
    }
}