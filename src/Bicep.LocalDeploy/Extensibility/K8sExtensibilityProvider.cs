// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Core.Json;
using Azure.Deployments.Extensibility.Messages;
using Azure.Deployments.Extensibility.Providers.Kubernetes;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Azure.Bicep.LocalDeploy.Extensibility;

public partial class K8sExtensibilityProvider : IExtensibilityProvider
{
    public Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => ExecuteRequest(request, (coreRequest) => new KubernetesProvider().DeleteAsync(coreRequest, cancellationToken));

    public Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => ExecuteRequest(request, (coreRequest) => new KubernetesProvider().GetAsync(coreRequest, cancellationToken));

    public Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => ExecuteRequest(request, (coreRequest) => new KubernetesProvider().PreviewSaveAsync(coreRequest, cancellationToken));

    public Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
        => ExecuteRequest(request, (coreRequest) => new KubernetesProvider().SaveAsync(coreRequest, cancellationToken));

    private static async Task<ExtensibilityOperationResponse> ExecuteRequest(ExtensibilityOperationRequest request, Func<Deployments.Extensibility.Core.ExtensibilityOperationRequest, Task<Deployments.Extensibility.Core.ExtensibilityOperationResponse>> func)
    {
        var coreRequest = ToCoreRequest(request);
        var coreResponse = await func(coreRequest);

        return FromCoreResponse(coreResponse);
    }

    private static Deployments.Extensibility.Core.ExtensibilityOperationRequest ToCoreRequest(ExtensibilityOperationRequest request)
    {
        return ExtensibilityJsonSerializer.Default.Deserialize<Deployments.Extensibility.Core.ExtensibilityOperationRequest>(request.ToJson())
            ?? throw new InvalidOperationException($"Failed to deserialize {nameof(ExtensibilityOperationRequest)}");
    }

    private static ExtensibilityOperationResponse FromCoreResponse(Deployments.Extensibility.Core.ExtensibilityOperationResponse response)
    {
        switch (response)
        {
            case Deployments.Extensibility.Core.ExtensibilityOperationSuccessResponse success:
                return ExtensibilityJsonSerializer.Default.Serialize(success).FromJson<ExtensibilityOperationResponse>();
            case Deployments.Extensibility.Core.ExtensibilityOperationErrorResponse error:
                return ExtensibilityJsonSerializer.Default.Serialize(error).FromJson<ExtensibilityOperationResponse>();
            default:
                throw new NotImplementedException();
        }
    }
}