using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    class Scope
    {
        public IDictionary<string, SyntaxBase> Declarations { get; } = new Dictionary<string, SyntaxBase>();
    }
}