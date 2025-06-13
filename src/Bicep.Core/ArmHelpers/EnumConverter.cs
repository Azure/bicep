// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Core.Configuration;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.ArmHelpers;

public static class EnumConverter
{
    public static TemplateDeploymentScope? ToTemplateDeploymentScope(ResourceScope targetScope) => targetScope switch
    {
        ResourceScope.Tenant => TemplateDeploymentScope.Tenant,
        ResourceScope.ManagementGroup => TemplateDeploymentScope.ManagementGroup,
        ResourceScope.Subscription => TemplateDeploymentScope.Subscription,
        ResourceScope.ResourceGroup => TemplateDeploymentScope.ResourceGroup,
        _ => null,
    };
}
