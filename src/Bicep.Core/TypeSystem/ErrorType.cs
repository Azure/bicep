using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Errors;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;

namespace Bicep.Core.TypeSystem
{
    public class ErrorTypeSymbol : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly Error? error;
        private readonly ImmutableArray<Error>? errors;

        public ErrorTypeSymbol(Error error)
            : base(ErrorTypeName)
        {
            this.error = error;
            this.errors = null;
        }

        public ErrorTypeSymbol(IEnumerable<Error> errors)
            : base(ErrorTypeName)
        {
            this.error = null;
            this.errors = errors.ToImmutableArray();
        }

        public override IEnumerable<Error> GetDiagnostics()
        {
            if (this.error != null)
            {
                return this.error.AsEnumerable();
            }

            return this.errors!;
        }

        public override TypeKind TypeKind => TypeKind.Error;
    }
}
