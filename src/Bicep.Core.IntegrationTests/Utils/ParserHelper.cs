using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Tests.Utils
{
    public static class ParserHelper
    {
        public static SyntaxBase Parse(string contents)
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            var tokens = lexer.GetTokens();
            var parser = new Parser.Parser(tokens);

            var program = parser.Parse();

            var errors = new List<Error>();
            var collector = new ParseErrorCollector(errors);

            collector.Visit(program);

            return program;
        }
    }
}
