// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Emit
{
    public enum EmitStatus
    {
        /// <summary>
        /// Emitting the template succeeded. There were no errors. Warnings may have been suppressed. Informational diagnostics may have been emitted. 
        /// </summary>
        Succeeded,

        /// <summary>
        /// Emitting the template succeeded with warnings. There were no errors. Informational diagnostics may have been emitted.
        /// </summary>
        SucceededWithWarnings,

        /// <summary>
        /// Emitting the template failed due to errors. Warnings and informational diagnostics may have been emitted also.
        /// </summary>
        Failed
    }
}
