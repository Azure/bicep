using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public static class SyntaxFactory
    {
        public static ProgramSyntax CreateFromText(string text)
        {
            var parser = new Parser.Parser(text);
            return parser.Program();
        }
    }
}
