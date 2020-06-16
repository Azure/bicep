using System.Collections.Generic;

namespace Bicep.Core.SemanticModel
{
    public class Compilation
    {
        /// <summary>
        /// Gets the file that was compiled.
        /// </summary>
        public FileSymbol Root { get; }
    }

    public abstract class Symbol
    {
        /// <summary>
        /// Gets the name of the symbol. Returns an empty string if the symbol is not named.
        /// </summary>
        public string Name { get; }
    }

    public class TypeSymbol : Symbol
    {

    }

    public class FileSymbol : Symbol
    {
        public IEnumerable<ParameterSymbol> GetParameterMembers()
        {

        }
    }

    public class ParameterSymbol : Symbol
    {

    }
}