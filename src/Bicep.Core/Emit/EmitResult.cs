// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;

namespace Bicep.Core.Emit;

/// <param name="Status">The status of the emit operation.</param>
/// <param name="Diagnostics">The list of diagnostics collected during the emit operation.</param>
/// <param name="Features">The features applied during the emit operation.</param>
/// <param name="SourceMap">Source map created during the emit operation.</param>
public record EmitResult(
    EmitStatus Status,
    ImmutableArray<IDiagnostic> Diagnostics,
    IFeatureProvider Features,
    SourceMap? SourceMap = null);
