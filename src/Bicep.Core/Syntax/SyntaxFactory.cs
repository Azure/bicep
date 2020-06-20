using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
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
