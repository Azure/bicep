// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Flags that may be placed on functions to indicate where they can be placed in semantic tree
    /// </summary>
    [Flags]
    public enum FunctionPlacementFlags
    {
        /// <summary>
        /// The default, no restrictions
        /// </summary>
        Default = 0,

        /// <summary>
        /// The function can only be used in assigning to a module param with secure decorator.
        /// </summary>
        ModuleSecureParameterOnly = 1 << 0,
    }
}
