using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeCache
    {
        public TypeCache()
        {
        }

        public TypeSymbol? GetTypeInfo(SyntaxBase? syntax)
        {
            // TODO: When we have expressions, this class will cache the result, so we don't have walk the three all the time.
            switch (syntax)
            {
                case BooleanLiteralSyntax _:
                    return LanguageConstants.Bool;

                case NumericLiteralSyntax _:
                    return LanguageConstants.Int;

                case StringSyntax _:
                    return LanguageConstants.String;

                default:
                    // the expression has no type
                    // this is different than an expression having "any" type
                    return null;
            }
        }
    }
}
