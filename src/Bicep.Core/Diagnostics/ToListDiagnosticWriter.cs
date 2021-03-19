// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.Diagnostics
{
    public class ToListDiagnosticWriter : IDiagnosticWriter
    {
        private readonly List<IDiagnostic> diagnostics;

        public ToListDiagnosticWriter(List<IDiagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public static ToListDiagnosticWriter Create()
            => new ToListDiagnosticWriter(new List<IDiagnostic>());

        public void Write(IDiagnostic diagnostic)
            => diagnostics.Add(diagnostic);

        public IReadOnlyList<IDiagnostic> GetDiagnostics() => diagnostics;
    }
}
