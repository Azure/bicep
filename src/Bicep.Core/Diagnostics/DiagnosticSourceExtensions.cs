// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics;

public static class DiagnosticSourceExtensions
{
    public static string ToSourceString(this DiagnosticSource source)
        => source switch
        {
            DiagnosticSource.Compiler => "bicep",
            DiagnosticSource.CoreLinter => "bicep core linter",
            _ => throw new ArgumentException($"Unrecognized source {source}"),
        };
}
