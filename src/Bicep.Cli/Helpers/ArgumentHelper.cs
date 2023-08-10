// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Core.FileSystem;
using System;
using System.IO;

namespace Bicep.Cli.Helpers;

public class ArgumentHelper
{
    public static DiagnosticsFormat ToDiagnosticsFormat(string? format)
    {
        if(format is null || (format is not null && format.Equals("default", StringComparison.OrdinalIgnoreCase)))
        {
            return Arguments.DiagnosticsFormat.Default;
        }
        else if(format is not null && format.Equals("sarif", StringComparison.OrdinalIgnoreCase))
        {
            return Arguments.DiagnosticsFormat.Sarif;
        }

        throw new ArgumentException($"Unrecognized diagnostics format {format}");
    }
}