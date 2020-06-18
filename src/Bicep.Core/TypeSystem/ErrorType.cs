using System.Collections.Generic;
using Bicep.Core.Parser;

namespace Bicep.Core.TypeSystem
{
    public class ErrorTypeSymbol : TypeSymbol
    {
        public ErrorTypeSymbol(Error error)
            : base("error")
        {
            this.Error = error;
        }

        public override IEnumerable<Error> GetErrors()
        {
            yield return this.Error;
        }

        public Error Error { get; }

        public override TypeKind TypeKind => TypeKind.Error;

        public override bool Equals(TypeSymbol other)
        {
            // error type is equal to no other type, including another error or even itself
            return false;
        }
    }
}
