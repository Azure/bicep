// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Represents the restore status of a module
    /// </summary>
    public enum ModuleRestoreStatus
    {
        /// <summary>
        /// We have not yet attempted to restore the module.
        /// </summary>
        Unknown,

        /// <summary>
        /// The module restore has failed.
        /// </summary>
        Failed,

        /// <summary>
        /// Module restore has succeeded.
        /// </summary>
        Succeeded,
    }
}
