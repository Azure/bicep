// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Emit
{
    public class EmitResult
    {
        public EmitResult(EmitStatus status, IEnumerable<IDiagnostic> diagnostics, (string, int)[]? sourceMap = null)
        {
            this.Status = status;
            this.Diagnostics = diagnostics.ToImmutableArray();
            this.SourceMap = sourceMap;
        }

        /// <summary>
        /// Gets the status of the emit operation.
        /// </summary>
        public EmitStatus Status { get; }

        /// <summary>
        /// Gets a list of diagnostics collected during the emit operation.
        /// </summary>
        public ImmutableArray<IDiagnostic> Diagnostics { get; }

        /// <summary>
        /// Source map created during the emit operation.
        /// </summary>
        public (string, int)[]? SourceMap { get; }
        
    }
}
