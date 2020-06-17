using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SemanticModel
    {
        private readonly TypeCache typeCache = new TypeCache();

        public SemanticModel(FileSymbol root)
        {
            this.Root = root;
        }

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }

        public TypeSymbol? GetTypeInfo(SyntaxBase? syntax) => this.typeCache.GetTypeInfo(syntax);

        public TypeSymbol? GetTypeByName(string typeName) => this.typeCache.GetTypeByName(typeName);
    }
}