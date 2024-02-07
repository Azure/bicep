// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics
{
    public class ToListDiagnosticWriter(List<IDiagnostic> diagnostics) : IDiagnosticWriter
    {
        private readonly List<IDiagnostic> diagnostics = diagnostics;

        public static ToListDiagnosticWriter Create()
            => new(new List<IDiagnostic>());

        public void Write(IDiagnostic diagnostic)
            => diagnostics.Add(diagnostic);

        public IReadOnlyList<IDiagnostic> GetDiagnostics() => diagnostics;
    }
}
