// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class SymbolVisitor
    {
        public void Visit(Symbol node)
        {
            VisitInternal(node);
        }

        protected virtual void VisitInternal(Symbol node)
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

        public virtual void VisitVariableSymbol(VariableSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitResourceSymbol(ResourceSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitModuleSymbol(ModuleSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitOutputSymbol(OutputSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitTypeSymbol(TypeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitNamespaceSymbol(NamespaceSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitFunctionSymbol(FunctionSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitLocalScopeSymbol(LocalScopeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitLocalSymbol(LocalSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitErrorSymbol(ErrorSymbol symbol)
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

