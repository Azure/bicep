// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem
{
    [Flags]
    public enum ResourceScope
    {
        None = 0,

        Resource = 1 << 0,

        Module = 1 << 1,

        Tenant = 1 << 2,

        ManagementGroup = 1 << 3,

        Subscription = 1 << 4,

        ResourceGroup = 1 << 5,
    }
}
