using System.Collections.Immutable;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;

namespace Bicep.LanguageServer
{
    public class CompilationContext
    {
        public CompilationContext(Lexer lexer, Compilation compilation, ImmutableArray<int> lineStarts)
        {
            this.Lexer = lexer;
            this.Compilation = compilation;
            this.LineStarts = lineStarts;
        }

        public Lexer Lexer { get; }

        public Compilation Compilation { get; }

        public ImmutableArray<int> LineStarts { get; }
    }
}