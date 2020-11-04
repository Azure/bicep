// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.TypeSystem
{
    public class ErrorType : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly ImmutableArray<ErrorDiagnostic> errors;

        private ErrorType(ImmutableArray<ErrorDiagnostic> errors)
            : base(ErrorTypeName)
        {
            this.errors = errors;
        }

        public static ErrorType Create(ErrorDiagnostic error)
            => Create(error.AsEnumerable());

        public static ErrorType Create(IEnumerable<ErrorDiagnostic> errors)
            => new ErrorType(errors.ToImmutableArray());

        public static ErrorType Empty()
            => new ErrorType(ImmutableArray<ErrorDiagnostic>.Empty);

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => this.errors;

        public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

        public override TypeKind TypeKind => TypeKind.Error;
    }
}

