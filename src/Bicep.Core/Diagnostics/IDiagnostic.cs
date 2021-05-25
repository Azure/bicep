// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Diagnostics
{
    public interface IDiagnostic: IPositionable
    {
        string Code { get; }
        DiagnosticLabel? Label { get; }
        DiagnosticLevel Level { get; }
        string Message { get; }
    }
}
