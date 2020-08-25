// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem
{
    public class ErrorTypeSymbol : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly ImmutableArray<ErrorDiagnostic> errors;

        public ErrorTypeSymbol(ErrorDiagnostic error)
            : base(ErrorTypeName)
        {
            this.errors = ImmutableArray.Create<ErrorDiagnostic>(error);
        }

        public ErrorTypeSymbol(IEnumerable<ErrorDiagnostic> errors)
            : base(ErrorTypeName)
        {
            this.errors = errors.ToImmutableArray();
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            return this.errors;
        }

        public override TypeKind TypeKind => TypeKind.Error;
    }
}

