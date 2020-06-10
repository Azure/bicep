using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    public class Scope
    {
        public IDictionary<string, SyntaxBase> Declarations { get; } = new Dictionary<string, SyntaxBase>();
    }
}