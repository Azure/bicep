// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class SemanticDiagnosticVisitor : SymbolVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;

        public SemanticDiagnosticVisitor(IDiagnosticWriter diagnosticWriter)
        {
            this.diagnosticWriter = diagnosticWriter;
        }

        public override void VisitTypeSymbol(TypeSymbol symbol)
        {
            base.VisitTypeSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitParameterSymbol(ParameterSymbol symbol)
        {
            base.VisitParameterSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitFileSymbol(FileSymbol symbol)
        {
            base.VisitFileSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitVariableSymbol(VariableSymbol symbol)
        {
            base.VisitVariableSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitResourceSymbol(ResourceSymbol symbol)
        {
            base.VisitResourceSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitModuleSymbol(ModuleSymbol symbol)
        {
            base.VisitModuleSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitOutputSymbol(OutputSymbol symbol)
        {
            base.VisitOutputSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitErrorSymbol(ErrorSymbol symbol)
        {
            base.VisitErrorSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitBuiltInNamespaceSymbol(BuiltInNamespaceSymbol symbol)
        {
            base.VisitBuiltInNamespaceSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitImportedNamespaceSymbol(ImportedNamespaceSymbol symbol)
        {
            base.VisitImportedNamespaceSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitFunctionSymbol(FunctionSymbol symbol)
        {
            base.VisitFunctionSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitLocalScope(LocalScope symbol)
        {
            base.VisitLocalScope(symbol);
            this.CollectDiagnostics(symbol);
        }

        protected void CollectDiagnostics(Symbol symbol)
        {
            diagnosticWriter.WriteMultiple(symbol.GetDiagnostics());
        }
    }
}

