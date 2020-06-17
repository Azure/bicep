using System.Collections.Generic;
using Bicep.Core.Parser;

namespace Bicep.Core.SemanticModel
{
    public abstract class Symbol
    {
        protected Symbol(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the symbol. Returns an empty string if the symbol is not named.
        /// </summary>
        public string Name { get; }

        

        public abstract SymbolKind Kind { get; }

        public virtual IEnumerable<Error> GetErrors()
        {
            yield break;
        }

        protected Error CreateError(string message, IPositionable positionable) => new Error(message, positionable.Span);
    }
}