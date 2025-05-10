// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;

namespace Bicep.Core.TypeSystem.Types
{
    public class ErrorType : TypeSymbol
    {
        private const string ErrorTypeName = "error";
        private readonly ImmutableArray<IDiagnostic> diagnostics;

        private ErrorType(ImmutableArray<IDiagnostic> diagnostics)
            : base(ErrorTypeName)
        {
            this.diagnostics = diagnostics;
        }

        public static ErrorType Create(IDiagnostic diagnostic)
            => Create(diagnostic.AsEnumerable());

        public static ErrorType Create(IEnumerable<IDiagnostic> diagnostics)
            => new([.. diagnostics]);

        public static ErrorType Empty()
            => new([]);

        public override IEnumerable<IDiagnostic> GetDiagnostics() => diagnostics;

        public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

        public override TypeKind TypeKind => TypeKind.Error;
    }
}
