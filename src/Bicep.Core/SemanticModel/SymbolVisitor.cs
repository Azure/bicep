using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SymbolVisitor
    {
        public void Visit(Symbol node)
        {
            node.Accept(this);
        }

        public virtual void VisitFileSymbol(FileSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitParameterSymbol(ParameterSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitErrorTypeSymbol(ErrorTypeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitPrimitiveTypeSymbol(PrimitiveTypeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        protected void VisitDescendants(Symbol symbol)
        {
            foreach (Symbol descendant in symbol.Descendants)
            {
                this.Visit(descendant);
            }
        }
    }
}
