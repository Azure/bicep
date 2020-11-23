// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Parsing
{
    /// <summary>
    /// Flags that control the parser recovery behavior.
    /// </summary>
    [Flags]
    public enum RecoveryFlags
    {
        /// <summary>
        /// Default behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// The terminator token will be consumed by the recovery logic.
        /// </summary>
        ConsumeTerminator = 1 << 0,

        /// <summary>
        /// The diagnostics captured as part of the recovery will be suppressed.
        /// </summary>
        SuppressDiagnostics = 1 << 1
    }
}
