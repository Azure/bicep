using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ParserHelper
    {
        public static SyntaxBase Parse(string text) => new Core.Parser.Parser(text).Program();

        public static SyntaxBase ParseExpression(string text) => new Core.Parser.Parser(text).Expression();
    }
}
