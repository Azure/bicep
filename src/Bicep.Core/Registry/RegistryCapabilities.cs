// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Registry
{
    [Flags]
    public enum RegistryCapabilities
    {
        /// <summary>
        /// Modules can be restored from the registry. All registries must support this capability.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Modules can be published to the registry.
        /// </summary>
        Publish = 1 << 0
    }
}
