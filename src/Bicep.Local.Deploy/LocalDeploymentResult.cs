// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Local.Deploy;

public record LocalDeploymentResult(
    DeploymentContent Deployment,
    ImmutableArray<DeploymentOperationDefinition> Operations);
