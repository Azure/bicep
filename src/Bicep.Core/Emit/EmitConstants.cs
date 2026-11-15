// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Core.Features;

namespace Bicep.Core.Emit;

public static class EmitConstants
{
    public const string UserDefinedFunctionsNamespace = "__bicep";

    public const string NestedDeploymentResourceApiVersion = "2025-04-01";

    public const string OrchestrationGroupLabelsProperty = "__bicepGroupLabels";

    public const string OrchestrationStackLabelsProperty = "__bicepStackLabels";

    public static readonly FrozenSet<string> ResourceInfoProperties
        = new[] { "id", "name", "type", "apiVersion" }.ToFrozenSet();
}
