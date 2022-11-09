// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics
{
    // SimpleDiagnosticWriter is a diagnostic writer that returns a boolean indicating whether or not diagnostics were written.
    // By using a boolean we can avoid storing the diagnostics in a list like ToListDiagnosticWriter.
    public class SimpleDiagnosticWriter : IDiagnosticWriter
    {
        private bool hasDiagnostics;

        public SimpleDiagnosticWriter()
        {
            this.hasDiagnostics = false;
        }

        public void Write(IDiagnostic diagnostic) => hasDiagnostics = true;

        public bool HasDiagnostics() => hasDiagnostics;
    }
}
