using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Utils
{
    public static class SyntaxFactory
    {
        public static ProgramSyntax CreateFromText(string text)
        {
            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            var parser = new Parser.Parser(lexer.GetTokens());

            return parser.Parse();
        }
    }
}
