using System.Collections.Generic;

namespace Bicep.Core.SemanticModel
{
    public class FileSymbol : Symbol
    {
        public FileSymbol(string name) : base(name)
        {
        }

        public IEnumerable<ParameterSymbol> GetParameterMembers() => new ParameterSymbol[0];


        public override SymbolKind Kind => SymbolKind.File;
    }
}