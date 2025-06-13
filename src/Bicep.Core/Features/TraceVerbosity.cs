// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Features
{
    /// <remarks>
    /// The enum values must be in order of increasing verbosity, as they're used to determine the level of detail in tracing output.
    /// See <see cref="FeatureProvider.HasTracingVerbosity"/> for more details on how this is used.
    /// </remarks>
    public enum TraceVerbosity
    {
        Basic = 0,
        Full = 1,
    }
}
