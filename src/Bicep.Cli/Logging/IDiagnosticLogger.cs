// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Core.Diagnostics;
using System;
using System.Collections.Immutable;

namespace Bicep.Cli.Logging
{
    public interface IDiagnosticLogger
    {
        void LogDiagnostic(Uri fileUri, IDiagnostic diagnostic, ImmutableArray<int> lineStarts);

        void SetupFormat (DiagnosticsFormat? format);

        void FlushLog();

        int ErrorCount { get; }
    }
}

