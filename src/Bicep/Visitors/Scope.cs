using System.Collections.Generic;
using Bicep.Syntax;

namespace Bicep.Visitors
{
    class Scope
    {
        public IDictionary<string, SyntaxBase> Declarations { get; } = new Dictionary<string, SyntaxBase>();
    }
}