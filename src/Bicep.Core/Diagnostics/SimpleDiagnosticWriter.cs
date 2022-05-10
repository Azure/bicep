// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics
{
    public class SimpleDiagnosticWriter : IDiagnosticWriter
    {
        private bool hasDiagnostics;

        public SimpleDiagnosticWriter()
        {
            this.hasDiagnostics = false;
        }

        public static SimpleDiagnosticWriter Create()
            => new SimpleDiagnosticWriter();

        public void Write(IDiagnostic diagnostic)
            => hasDiagnostics = true;

        public bool HasDiagnostics() => hasDiagnostics;
    }
}
