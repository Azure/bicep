// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using System;

namespace Bicep.Core.Diagnostics
{
    public interface IDiagnostic : IPositionable
    {
        string Code { get; }
        string Source { get; }
        DiagnosticLabel? Label { get; }
        DiagnosticLevel Level { get; }
        string Message { get; }
        Uri? Uri { get; }
    }
}
