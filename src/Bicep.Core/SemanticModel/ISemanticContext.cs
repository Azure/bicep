using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public interface ISemanticContext
    {
        public TypeSymbol GetTypeInfo(SyntaxBase syntax);

        public TypeSymbol? GetTypeByName(string? typeName);
    }
}
