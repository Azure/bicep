// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.TypeSystem
{
    public class UnassignableTypeSymbol : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly ImmutableArray<ErrorDiagnostic> errors;

        public UnassignableTypeSymbol(string name)
            : base(name)
        {
        }

        public UnassignableTypeSymbol(ErrorDiagnostic error)
            : base(ErrorTypeName)
        {
            this.errors = ImmutableArray.Create<ErrorDiagnostic>(error);
        }

        public UnassignableTypeSymbol(IEnumerable<ErrorDiagnostic> errors)
            : base(ErrorTypeName)
        {
            this.errors = errors.ToImmutableArray();
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            return this.errors.IsDefaultOrEmpty ? ImmutableArray<ErrorDiagnostic>.Empty : this.errors;
        }

        public override TypeKind TypeKind => this.errors.IsDefaultOrEmpty ? TypeKind.Unassignable : TypeKind.Error;
    }
}

