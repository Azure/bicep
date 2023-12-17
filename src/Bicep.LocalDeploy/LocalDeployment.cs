// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace Azure.Bicep.LocalDeploy;

public static class LocalDeployment
{
    public record Result(
        DeploymentContent Deployment,
        ImmutableArray<DeploymentOperationDefinition> Operations);

    public static Task<Result> Deploy(string templateString, string parametersString, CancellationToken cancellationToken)
    {
        var services = new ServiceCollection()
            .RegisterLocalDeployServices()
            .BuildServiceProvider();
        
        var engine = services.GetRequiredService<LocalDeploymentEngine>();

        return engine.Deploy(templateString, parametersString, cancellationToken);
    }
}