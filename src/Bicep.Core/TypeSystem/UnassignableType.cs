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
        private readonly TypeKind kind;

        public UnassignableTypeSymbol(string name, TypeKind kind)
            : base(name)
        {
            this.kind = kind;

            // if errors are not initialized, initialize it as an empty array
            if (this.errors.IsDefault)
            {
                this.errors = ImmutableArray<ErrorDiagnostic>.Empty;
            }
        }

        public UnassignableTypeSymbol(ErrorDiagnostic error)
            : this(ErrorTypeName, TypeKind.Error)
        {
            this.errors = ImmutableArray.Create<ErrorDiagnostic>(error);
        }

        public UnassignableTypeSymbol(IEnumerable<ErrorDiagnostic> errors)
            : this(ErrorTypeName, TypeKind.Error)
        {
            this.errors = errors.ToImmutableArray();
        }

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            return this.errors;
        }

        public override TypeKind TypeKind => kind;
    }
}

