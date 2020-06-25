using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.SemanticModel
{
    public class SemanticErrorVisitor : SymbolVisitor
    {
        private readonly IList<Error> diagnostics;

        public SemanticErrorVisitor(IList<Error> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public override void VisitErrorTypeSymbol(ErrorTypeSymbol symbol)
        {
            // TODO: Need a better pattern for collecting diagnostics from all the symbols - otherwise, we'll forget and miss something
            base.VisitErrorTypeSymbol(symbol);
            this.CollectDiagnostics(symbol);
        }

        public override void VisitPrimitiveTypeSymbol(PrimitiveTypeSymbol symbol)
        {
            base.VisitPrimitiveTypeSymbol(symbol);
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

        protected void CollectDiagnostics(Symbol symbol)
        {
            foreach (Error diagnostic in symbol.GetDiagnostics())
            {
                this.diagnostics.Add(diagnostic);
            }
        }
    }
}
