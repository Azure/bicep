using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeCache : ISemanticContext
    {
        public TypeCache()
        {
        }

        public TypeSymbol? GetTypeInfo(SyntaxBase? syntax)
        {
            // TODO: When we have expressions, this class will cache the result, so we don't have walk the three all the time.
            switch (syntax)
            {
                case null:
                    // a null node has no type
                    // which is different than "any" type
                    return null;

                case BooleanLiteralSyntax _:
                    return LanguageConstants.Bool;

                case NumericLiteralSyntax _:
                    return LanguageConstants.Int;

                case StringSyntax _:
                    return LanguageConstants.String;

                case ObjectSyntax _:
                    return LanguageConstants.Object;

                default:
                    // this expression has an unknown type
                    return new ErrorTypeSymbol(new Error("This value has an unexpected type", syntax.Span));
            }
        }

        public TypeSymbol? GetTypeByName(string typeName)
        {
            LanguageConstants.ParameterTypes.TryGetValue(typeName, out TypeSymbol parameterType);
            return parameterType;
        }
    }
}
