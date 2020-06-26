using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Text;
using Bicep.LanguageServer.CompilationManager;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// Creates compilation contexts.
    /// </summary>
    /// <remarks>This class exists only so we can mock fatal exceptions in tests.</remarks>
    public class BicepCompilationProvider: ICompilationProvider
    {
        public CompilationContext Create(string text)
        {
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);

            var parser = new Parser(text);
            var program = parser.Parse();

            var compilation = new Compilation(program);

            return new CompilationContext(compilation, lineStarts);
        }
    }
}
