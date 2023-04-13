// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics;

// ErrorTrackingDiagnosticWriterDecorator wraps another diagnostic writer and will track whether any error-level diagnostics have been written.
public class ErrorTrackingDiagnosticWriterDecorator : IDiagnosticWriter
{
    private readonly IDiagnosticWriter decorated;
    private bool hasErrors = false;

    public ErrorTrackingDiagnosticWriterDecorator(IDiagnosticWriter decorated)
    {
        this.decorated = decorated;
    }

    public void Write(IDiagnostic diagnostic)
    {
        if (diagnostic.Level == DiagnosticLevel.Error)
        {
            hasErrors = true;
        }

        decorated.Write(diagnostic);
    }

    public bool HasErrors => hasErrors;
}
