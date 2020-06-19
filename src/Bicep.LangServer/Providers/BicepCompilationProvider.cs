using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;

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
            var lineStarts = PositionHelper.GetLineStarts(text);

            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            var parser = new Parser(lexer.GetTokens());
            var program = parser.Parse();

            var compilation = new Compilation(program);

            return new CompilationContext(lexer, compilation, lineStarts);
        }
    }
}
