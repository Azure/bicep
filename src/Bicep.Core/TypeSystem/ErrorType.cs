using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;

namespace Bicep.Core.TypeSystem
{
    public class ErrorTypeSymbol : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly ErrorDiagnostic? error;
        private readonly ImmutableArray<ErrorDiagnostic>? errors;

        public ErrorTypeSymbol(ErrorDiagnostic error)
            : base(ErrorTypeName)
        {
            this.error = error;
            this.errors = null;
        }

        public ErrorTypeSymbol(IEnumerable<ErrorDiagnostic> errors)
            : base(ErrorTypeName)
        {
            this.error = null;
            this.errors = errors.ToImmutableArray();
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
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
