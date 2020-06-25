using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public class ErrorTypeSymbol : TypeSymbol
    {
        private readonly Error error;

        public ErrorTypeSymbol(Error error)
            : base("error")
        {
            this.error = error;
        }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitErrorTypeSymbol(this);
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            yield return this.error;
        }

        public override TypeKind TypeKind => TypeKind.Error;

        public override bool Equals(TypeSymbol other)
        {
            // error type is equal to no other type, including another error or even itself
            return false;
        }
    }
}
