// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Emit
{
    public class EmitResult
    {
        public EmitResult(EmitStatus status, IEnumerable<IDiagnostic> diagnostics)
        {
            this.Status = status;
            this.Diagnostics = diagnostics.ToImmutableArray();
        }

        /// <summary>
        /// Gets the status of the emit operation.
        /// </summary>
        public EmitStatus Status { get; }

        /// <summary>
        /// Gets a list of diagnostics collected during the emit operation.
        /// </summary>
        public ImmutableArray<IDiagnostic> Diagnostics { get; }
    }
}
