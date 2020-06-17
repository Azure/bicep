using System;
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public class SymbolCache
    {
        private readonly Dictionary<SyntaxBase, Symbol> symbols = new Dictionary<SyntaxBase, Symbol>();

        private readonly Dictionary<string, List<SyntaxBase>> identifiers = new Dictionary<string, List<SyntaxBase>>(StringComparer.Ordinal);

        public bool ContainsIdentifier(string identifierName) => identifiers.ContainsKey(identifierName);
    }
}