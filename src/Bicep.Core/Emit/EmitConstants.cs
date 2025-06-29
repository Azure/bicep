// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Core.Features;

namespace Bicep.Core.Emit;

public static class EmitConstants
{
    public const string UserDefinedFunctionsNamespace = "__bicep";

    // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
    public const string NestedDeploymentResourceApiVersion = "2022-09-01";

    public const string NestedDeploymentResourceApiVersionWithModuleConfigsEnabled = "2025-03-01";

    public const string NestedDeploymentResourceApiVersionWithIdentityInModulesSupport = "2025-04-01";

    public static readonly FrozenSet<string> ResourceInfoProperties
        = new[] { "id", "name", "type", "apiVersion" }.ToFrozenSet();

    public static string GetNestedDeploymentResourceApiVersion(IFeatureProvider features)
    {
        // TODO: Remove this function and set NestedDeploymentResourceApiVersion to 2025-03-01 once the API version rolls out everywhere.
        return features switch
        {
            { ModuleIdentityEnabled: true } => NestedDeploymentResourceApiVersionWithIdentityInModulesSupport,
            { ModuleExtensionConfigsEnabled: true } => NestedDeploymentResourceApiVersionWithModuleConfigsEnabled,
            _ => NestedDeploymentResourceApiVersion,
        };
    }
}
