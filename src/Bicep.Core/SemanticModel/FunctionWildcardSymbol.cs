using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Bicep.Core.SemanticModel
{
    public class FunctionWildcardSymbol : FunctionSymbol
    {
        public FunctionWildcardSymbol(string name, Regex regexName, IEnumerable<FunctionOverload> overloads)
            : base(name, overloads)
        {
            RegexName = regexName;
        }

        public Regex RegexName { get; }
    }
}