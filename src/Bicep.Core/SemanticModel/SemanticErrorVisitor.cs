using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SemanticErrorVisitor : SymbolVisitor
    {
        private readonly IList<ErrorDiagnostic> diagnostics;

        public SemanticErrorVisitor(IList<ErrorDiagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
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

        public override void VisitOutputSymbol(OutputSymbol symbol)
        {
            base.VisitOutputSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        protected void CollectDiagnostics(Symbol symbol)
        {
            foreach (ErrorDiagnostic diagnostic in symbol.GetDiagnostics())
            {
                this.diagnostics.Add(diagnostic);
            }
        }
    }
}
