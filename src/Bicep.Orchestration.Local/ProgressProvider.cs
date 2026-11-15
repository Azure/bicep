// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions;
using System.Security.Cryptography;
using Bicep.Core.Configuration;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Azure;
using static Bicep.Local.Deploy.Extensibility.NestedDeploymentExtension;

namespace Bicep.Orchestration.Local;

// Reports stack lifecycle only. Resource-level progress is intentionally not synthesized.
internal sealed class ProgressProvider(IArmDeploymentProvider inner, Func<DeploymentLocator, string, Task> progress) : IArmDeploymentProvider
{
    public bool SupportsExternalExtensions => inner.SupportsExternalExtensions;
    public Task CreateResourceGroup(IBicepConfiguration c, DeploymentLocator l, string region, CancellationToken ct) => inner.CreateResourceGroup(c, l, region, ct);
    public Task StartDeployment(IBicepConfiguration c, DeploymentLocator l, string t, string p, CancellationToken ct) => inner.StartDeployment(c, l, t, p, ct);
    public Task<LocalDeploymentResult> CheckDeployment(IBicepConfiguration c, DeploymentLocator l, CancellationToken ct) => inner.CheckDeployment(c, l, ct);
    public async Task StartDeploymentStack(IBicepConfiguration c, DeploymentLocator l, string t, string p, StacksConfig config, CancellationToken ct)
    {
        await progress(l, "Running");
        try
        {
            await Task.Delay(RandomNumberGenerator.GetInt32(5000, 10001), ct);
            await inner.StartDeploymentStack(c, l, t, p, config, ct);
        }
        catch (OperationCanceledException) { await progress(l, "Canceled"); throw; }
        catch { await progress(l, "Failed"); throw; }
    }
    public async Task<LocalDeploymentResult> CheckDeploymentStack(IBicepConfiguration c, DeploymentLocator l, CancellationToken ct)
    {
        try
        {
            var result = await inner.CheckDeploymentStack(c, l, ct);
            var status = result.Deployment.Properties.ProvisioningState?.ToString();
            if (status is "Succeeded" or "Failed" or "Canceled") { await progress(l, status); }
            return result;
        }
        catch (OperationCanceledException) { await progress(l, "Canceled"); throw; }
        catch { await progress(l, "Failed"); throw; }
    }
    public Task<IReadOnlyList<DeploymentWhatIfResourceChangeDefinition>> WhatIf(IBicepConfiguration c, DeploymentLocator l, string region, string t, string p, StacksConfig config, CancellationToken ct) => inner.WhatIf(c, l, region, t, p, config, ct);
    public Task TeardownStack(IBicepConfiguration c, DeploymentLocator l, StacksConfig config, CancellationToken ct) => inner.TeardownStack(c, l, config, ct);
}
