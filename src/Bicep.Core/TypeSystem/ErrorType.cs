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
        private readonly Diagnostic? error;
        private readonly ImmutableArray<Diagnostic>? errors;

        public ErrorTypeSymbol(Diagnostic error)
            : base(ErrorTypeName)
        {
            this.error = error;
            this.errors = null;
        }

        public ErrorTypeSymbol(IEnumerable<Diagnostic> errors)
            : base(ErrorTypeName)
        {
            this.error = null;
            this.errors = errors.ToImmutableArray();
        }

        public override IEnumerable<Diagnostic> GetDiagnostics()
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
