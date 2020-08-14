using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public abstract class DeclaredSymbol : Symbol
    {
        protected DeclaredSymbol(ITypeManager typeManager, string name, SyntaxBase declaringSyntax)
            : base(name)
        {
            this.TypeManager = typeManager;
            this.DeclaringSyntax = declaringSyntax;
        }

        public ITypeManager TypeManager { get; }

        /// <summary>
        /// Gets the syntax node that declared this symbol.
        /// </summary>
        public SyntaxBase DeclaringSyntax { get; }

        /// <summary>
        /// Gets the syntax node of the identifier. May be null if the symbol is in an invalid state.
        /// </summary>
        public abstract SyntaxBase? NameSyntax { get; }

        protected TypeSymbol? GetPrimitiveTypeByName(string typeName)
        {
            var type = this.TypeManager.GetTypeByName(typeName);
            if (type?.TypeKind == TypeKind.Primitive)
            {
                return type;
            }

            return null;
        }
    }
}