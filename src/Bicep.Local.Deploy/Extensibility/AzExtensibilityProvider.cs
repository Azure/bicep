// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Data;
using Azure.Deployments.Extensibility.Messages;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.Extensibility;

public class AzExtensibilityProvider : LocalExtensibilityProvider
{
    private readonly LocalExtensibilityHandler extensibilityHandler;

    public AzExtensibilityProvider(LocalExtensibilityHandler extensibilityHandler)
    {
        this.extensibilityHandler = extensibilityHandler;
    }

    public override Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override async Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, CancellationToken cancellationToken)
    {
        switch (request.Resource.Type)
        {
            case "Microsoft.Resources/deployments":
                {
                    var template = request.Resource.Properties["template"]!.ToString();
                    var parameters = new JObject
                    {
                        ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                        ["contentVersion"] = "1.0.0.0",
                        ["parameters"] = request.Resource.Properties["parameters"],
                    };

                    var result = await LocalDeployment.Deploy(extensibilityHandler, template, parameters.ToJson(), cancellationToken);

                    if (result.Deployment.Properties.ProvisioningState != ProvisioningState.Succeeded)
                    {
                        return new(
                            null,
                            null,
                            result.Deployment.Properties.Error.Details.SelectArray(x => new ExtensibilityError(x.Code, x.Message, x.Target)));
                    }

                    return new(
                        new ExtensibleResourceData(request.Resource.Type, new JObject
                        {
                            ["outputs"] = result.Deployment.Properties.Outputs?.ToJToken(),
                        }),
                        null,
                        null);
                }
        }

        throw new NotImplementedException();
    }
}
