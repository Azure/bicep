// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    [Flags]
    public enum ResourceFlags
    {
        /// <summary>
        /// No flags specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The property is read-only.
        /// </summary>
        ReadOnly = 1 << 0,
    }
}
