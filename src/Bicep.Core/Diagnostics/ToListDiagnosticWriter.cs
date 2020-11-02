// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.Core.Diagnostics
{
    public class ToListDiagnosticWriter : IDiagnosticWriter
    {
        private readonly List<Diagnostic> diagnostics;

        public ToListDiagnosticWriter(List<Diagnostic> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public static ToListDiagnosticWriter Create()
            => new ToListDiagnosticWriter(new List<Diagnostic>());

        public void Write(Diagnostic diagnostic)
            => diagnostics.Add(diagnostic);

        public IReadOnlyList<Diagnostic> GetDiagnostics() => diagnostics;
    }
}