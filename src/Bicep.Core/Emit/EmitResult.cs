// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.SourceMapping;

namespace Bicep.Core.Emit
{
    public class EmitResult
    {
        public EmitResult(EmitStatus status, IEnumerable<IDiagnostic> diagnostics, SourceMap? sourceMap = null)
        {
            this.Status = status;
            this.Diagnostics = diagnostics.ToImmutableArray();
            this.sourceMap = sourceMap;
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
        public SourceMap? sourceMap { get; }
        
    }
}
