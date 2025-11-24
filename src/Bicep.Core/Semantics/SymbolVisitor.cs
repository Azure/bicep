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

        public virtual void VisitMetadataSymbol(MetadataSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitParamAssignmentSymbol(ParameterAssignmentSymbol symbol)
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

        public virtual void VisitDeclaredFunctionSymbol(DeclaredFunctionSymbol symbol)
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

        public virtual void VisitTestSymbol(TestSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitOutputSymbol(OutputSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitAssertSymbol(AssertSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitTypeSymbol(TypeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitTypeAliasSymbol(TypeAliasSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitAmbientTypeSymbol(AmbientTypeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitBuiltInNamespaceSymbol(BuiltInNamespaceSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitExtensionNamespaceSymbol(ExtensionNamespaceSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitExtensionConfigAssignmentSymbol(ExtensionConfigAssignmentSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitFunctionSymbol(FunctionSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitPropertySymbol(PropertySymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitLocalScope(LocalScope symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitLocalVariableSymbol(LocalVariableSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitLocalThisNamespaceSymbol(LocalThisNamespaceSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitErrorSymbol(ErrorSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitWildcardImportSymbol(WildcardImportSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitImportedTypeSymbol(ImportedTypeSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitImportedVariableSymbol(ImportedVariableSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitImportedFunctionSymbol(ImportedFunctionSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitErroredImportSymbol(ErroredImportSymbol symbol)
        {
            VisitDescendants(symbol);
        }

        public virtual void VisitWildcardImportInstanceFunctionSymbol(WildcardImportInstanceFunctionSymbol symbol)
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
