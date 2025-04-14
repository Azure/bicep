// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        DesiredStateConfiguration = 1 << 6,

        Local = 1 << 7,
    }
}
