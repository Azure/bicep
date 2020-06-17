using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SemanticModel
    {
        public SemanticModel(FileSymbol root)
        {
            this.Root = root;
        }

        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }

        
    }
}