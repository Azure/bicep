using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public interface ITypeManager
    {
        public TypeSymbol GetTypeInfo(SyntaxBase syntax, TypeManagerContext context);

        public TypeSymbol? GetTypeByName(string? typeName);
    }
}
