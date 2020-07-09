using Bicep.Core.Parser;
using Bicep.Core.Resources;
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

                case ArraySyntax _:
                    return LanguageConstants.Array;

                default:
                    // this expression has an unknown type
                    return new ErrorTypeSymbol(new Error("This value has an unexpected type", syntax.Span));
            }
        }

        // TODO: This does not recognize non-resource named objects yet
        public TypeSymbol? GetTypeByName(string? typeName)
        {
            if (typeName == null)
            {
                return null;
            }

            if (LanguageConstants.PrimitiveTypes.TryGetValue(typeName, out TypeSymbol primitiveType))
            {
                return primitiveType;
            }

            // TODO: This needs proper namespace, type, and version resolution logic in the future
            ResourceTypeReference? typeReference = ResourceTypeParser.TryParse(typeName);
            if (typeReference == null)
            {
                return null;
            }

            // TODO: Construct/lookup type information based on JSON schema or swagger
            // for now assuming very basic resource schema
            return new ResourceType(typeName, LanguageConstants.TopLevelResourceProperties, additionalPropertiesType: null);
        }
    }
}
