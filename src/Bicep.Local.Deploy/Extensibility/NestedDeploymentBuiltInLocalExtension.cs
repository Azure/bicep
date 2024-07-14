// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Extensibility.Data;
using Azure.Deployments.Extensibility.Messages;
using Json.More;
using Json.Pointer;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.Extensibility;

public class NestedDeploymentBuiltInLocalExtension : LocalExtensibilityHost
{
    private readonly LocalExtensibilityHostManager extensibilityHostManager;

    public NestedDeploymentBuiltInLocalExtension(LocalExtensibilityHostManager extensibilityHandler)
    {
        this.extensibilityHostManager = extensibilityHandler;
    }

    private record DeploymentIdentifiers(string DeploymentName);

    private JsonObject CreateDeploymentIdentifiers(DeploymentContent deployment)
        => JsonObject.Parse(new DeploymentIdentifiers(deployment.Name).ToJsonStream())?.AsObject() ?? throw new UnreachableException("Serialization is not expected to fail.");

    public override async Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken)
    {
        switch (request.Type)
        {
            case "Microsoft.Resources/deployments":
                {
                    var template = request.Properties["template"]!.ToString();
                    var parameters = new JsonObject
                    {
                        ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                        ["contentVersion"] = "1.0.0.0",
                        ["parameters"] = request.Properties["parameters"]?.DeepClone(),
                    };

                    var result = await LocalDeployment.Deploy(extensibilityHostManager, template, parameters.ToJsonString(), cancellationToken);

                    if (result.Deployment.Properties.ProvisioningState != ProvisioningState.Succeeded)
                    {
                        return new LocalExtensibilityOperationResponse(
                            Resource: null,
                            ErrorData: new ErrorData(
                             error: new Error(
                                result.Deployment.Properties.Error.Code,
                                result.Deployment.Properties.Error.Message,
                                JsonPointer.Empty,
                                result.Deployment.Properties.Error.Details.SelectArray(x => new ErrorDetail(x.Code, x.Message, JsonPointer.Empty)))));
                    }

                    return new LocalExtensibilityOperationResponse(
                        Resource: new Resource(
                            identifiers: CreateDeploymentIdentifiers(result.Deployment),
                            type: request.Type,
                            apiVersion: request.ApiVersion,
                            status: result.Deployment.Properties.ProvisioningState.ToString(),
                            properties: JsonObject.Parse(result.Deployment.Properties.ToJsonStream())?.AsObject() ?? throw new UnreachableException("Serialization is not expected to fail.")),
                        ErrorData: null);
                }
        }

        throw new NotImplementedException();
    }

    public override Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
