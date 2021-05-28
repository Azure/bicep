// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics
{
    public interface IDiagnosticWriter
    {
        void Write(IDiagnostic diagnostic);
    }
}
