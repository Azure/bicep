// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.ResourceManager.Resources.Models;

namespace Bicep.Core.Models
{
    public record ArmDeploymentDefinition(string? ManagementGroupId, string? SubscriptionId, string? ResourceGroupName, string Name, ArmDeploymentProperties Properties)
    {
        public static string Type => "Microsoft.Resources/deployments";

        public ResourceIdentifier Id => (ManagementGroupId, SubscriptionId, ResourceGroupName) switch
        {
            (not null, null, null) => new($"/{ManagementGroupId}/providers/{Type}/{Name}"),
            (null, not null, null) => new($"/subscriptions/{SubscriptionId}/providers/{Type}/{Name}"),
            (null, not null, not null) => new($"/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroupName}/providers/{Type}/{Name}"),
            (null, null, null) => new($"/providers/{Type}/{Name}"),
            _ => throw new InvalidOperationException("Invalid deployment definition scope."),
        };

        public void Write(Utf8JsonWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
