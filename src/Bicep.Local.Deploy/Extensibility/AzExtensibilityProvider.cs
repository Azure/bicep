// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Engine.Host.Azure.ExtensibilityV2.Contract.Models;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Data;
using Azure.Deployments.Extensibility.Messages;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.Extensibility;

public class AzExtensibilityProvider : LocalExtensibilityProviderV2
{
    private readonly LocalExtensibilityHandler extensibilityHandler;

    public AzExtensibilityProvider(LocalExtensibilityHandler extensibilityHandler)
    {
        this.extensibilityHandler = extensibilityHandler;
    }

    public override async Task<ResourceResponseBody> CreateOrUpdateResourceAsync(ResourceRequestBody request, CancellationToken cancellationToken)
    {
        switch (request.Type)
        {
            case "Microsoft.Resources/deployments":
                {
                    var template = request.Properties["template"]!.ToString();
                    var parameters = new JObject
                    {
                        ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                        ["contentVersion"] = "1.0.0.0",
                        ["parameters"] = request.Properties["parameters"],
                    };

                    var result = await LocalDeployment.Deploy(extensibilityHandler, template, parameters.ToJson(), cancellationToken);

                    if (result.Deployment.Properties.ProvisioningState != ProvisioningState.Succeeded)
                    {
                        return new ResourceResponseBody(
                            error: new ErrorPayload(
                                result.Deployment.Properties.Error.Code,
                                result.Deployment.Properties.Error.Target,
                                result.Deployment.Properties.Error.Message,
                                result.Deployment.Properties.Error.Details.SelectArray(x => new ErrorDetail(x.Code, x.Target, x.Message))),
                            identifiers: null,
                            type: null,
                            status: null,
                            properties: null);
                    }

                    return new ResourceResponseBody(
                        error: null,
                        identifiers: result.Deployment.ToJToken(),
                        type: request.Type,
                        status: result.Deployment.Properties.ProvisioningState.ToString(),
                        properties: result.Deployment.Properties.ToJToken());
                }
        }

        throw new NotImplementedException();
    }

    public override Task<ResourceResponseBody> DeleteResourceAsync(ResourceReferenceRequestBody request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<ResourceResponseBody> GetResourceAsync(ResourceReferenceRequestBody request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<ResourceResponseBody> PreviewResourceCreateOrUpdateAsync(ResourceRequestBody request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
