using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

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

        public override IEnumerable<Error> GetDiagnostics()
        {
            yield return this.error;
        }

        public override TypeKind TypeKind => TypeKind.Error;
    }
}
