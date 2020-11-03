// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.TypeSystem
{
    [Flags]
    public enum ResourceScopeType
    {
        None = 0,

        ResourceScope = 1 << 0,

        ModuleScope = 1 << 1,

        TenantScope = 1 << 2,

        ManagementGroupScope = 1 << 3,

        SubscriptionScope = 1 << 4,

        ResourceGroupScope = 1 << 5,
    }
}
