// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Deployments.Core.Definitions;

namespace Bicep.Local.Deploy;

public record LocalDeploymentResult(
    DeploymentContent Deployment,
    ImmutableArray<DeploymentOperationDefinition> Operations,
    ImmutableDictionary<string, LocalDeploymentResult> ChildDeployments);
