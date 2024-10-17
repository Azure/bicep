// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Emit;

public static class EmitConstants
{
    public const string UserDefinedFunctionsNamespace = "__bicep";

    // IMPORTANT: Do not update this API version until the new one is confirmed to be deployed and available in ALL the clouds.
    public const string NestedDeploymentResourceApiVersion = "2022-09-01";
}
