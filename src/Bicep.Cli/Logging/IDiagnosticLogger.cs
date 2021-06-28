// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Cli.Logging
{
    public interface IDiagnosticLogger
    {
        void LogDiagnostic(Uri fileUri, IDiagnostic diagnostic, ImmutableArray<int> lineStarts);
        void LogSummary();
        int ErrorCount { get; set; }
    }
}

