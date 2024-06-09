// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Azure.Deployments.Service.Shared.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Local.Deploy;

public static class LocalDeployment
{
    public record Result(
        DeploymentContent Deployment,
        ImmutableArray<DeploymentOperationDefinition> Operations);

    public static async Task<Result> Deploy(LocalExtensibilityHandler extensibilityHandler, string templateString, string parametersString, CancellationToken cancellationToken)
    {
        var services = new ServiceCollection()
            .RegisterLocalDeployServices(extensibilityHandler)
            .BuildServiceProvider();

        var engine = services.GetRequiredService<LocalDeploymentEngine>();
        var dispatcher = services.GetRequiredService<WorkerJobDispatcherClient>();

        try
        {
            return await engine.Deploy(templateString, parametersString, cancellationToken);
        }
        finally
        {
            await dispatcher.StopAsync();
        }
    }
}
