using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;

namespace Bicep.LanguageServer
{
    // TODO: This can go away once we remove lexer errors
    public class CompilationContext
    {
        public CompilationContext(Lexer lexer, Compilation compilation)
        {
            this.Lexer = lexer;
            this.Compilation = compilation;
        }

        public Lexer Lexer { get; }

        public Compilation Compilation { get; }
    }
}