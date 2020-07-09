using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public static class StringSyntaxExtensions
    {
        public static string? TryGetValue(this StringSyntax syntax)
        {
            return Lexer.TryGetStringValue(syntax.StringToken);
        }

        public static string GetValue(this StringSyntax syntax)
        {
            return Lexer.GetStringValue(syntax.StringToken);
        }
    }
}
