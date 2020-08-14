using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class TypeManagerContext
    {
        private readonly HashSet<SyntaxBase> visited = new HashSet<SyntaxBase>();

        public bool TryMarkVisited(SyntaxBase syntax) => this.visited.Add(syntax);

        public IEnumerable<SyntaxBase> GetVisitedNodes() => this.visited;
    }
}
