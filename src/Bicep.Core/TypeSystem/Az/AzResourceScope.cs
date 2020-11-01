// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem.Az
{
    [Flags]
    public enum AzResourceScope
    {
        None = 0,

        Tenant = 1 << 0,

        ManagementGroup = 1 << 1,

        Subscription = 1 << 2,

        ResourceGroup = 1 << 3,
    }
}