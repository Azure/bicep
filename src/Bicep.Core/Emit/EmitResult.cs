// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Emit;

/// <param name="Status">The status of the emit operation.</param>
/// <param name="Diagnostics">The list of diagnostics collected during the emit operation.</param>
/// <param name="SourceMap">Source map created during the emit operation.</param>
public record EmitResult(
    EmitStatus Status,
    IEnumerable<IDiagnostic> Diagnostics,
    SourceMap? SourceMap = null);