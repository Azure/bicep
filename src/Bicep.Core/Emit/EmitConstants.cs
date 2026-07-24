// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Core.Features;

namespace Bicep.Core.Emit;

public static class EmitConstants
{
    public const string UserDefinedFunctionsNamespace = "__bicep";

    public const string NestedDeploymentResourceApiVersion = "2025-04-01";

    /// <summary>
    /// The API version under which the <c>Microsoft.Resources/deployments/modules</c> child resource type is exposed.
    /// Used when the <c>moduleDeployments</c> experimental feature is enabled.
    /// </summary>
    public const string ModuleDeploymentResourceApiVersion = "2025-04-01";

    public static readonly FrozenSet<string> ResourceInfoProperties
        = new[] { "id", "name", "type", "apiVersion" }.ToFrozenSet();
}
