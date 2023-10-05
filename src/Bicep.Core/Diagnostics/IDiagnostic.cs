// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    public interface IDiagnostic : IPositionable
    {
        string Code { get; }
        string Source { get; }
        DiagnosticStyling Styling { get; }
        DiagnosticLevel Level { get; }
        string Message { get; }
        Uri? Uri { get; }
    }
}
