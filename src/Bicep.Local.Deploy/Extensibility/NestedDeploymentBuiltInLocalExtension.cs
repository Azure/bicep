// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Extensibility.Contract;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Extensibility.Data;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Json.More;
using Json.Pointer;
using Microsoft.WindowsAzure.ResourceStack.Common.Algorithms;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.Extensibility;

public class NestedDeploymentBuiltInLocalExtension : LocalExtensibilityHost
{
    private readonly LocalDeploymentEngine localDeploymentEngine;
    private readonly IConfigurationManager configurationManager;
    private readonly ITokenCredentialFactory credentialFactory;

    public NestedDeploymentBuiltInLocalExtension(LocalDeploymentEngine localDeploymentEngine, IConfigurationManager configurationManager, ITokenCredentialFactory credentialFactory)
    {
        this.localDeploymentEngine = localDeploymentEngine;
        this.configurationManager = configurationManager;
        this.credentialFactory = credentialFactory;
    }

    private record DeploymentIdentifiers(
        string? SubscriptionId,
        string? ResourceGroup,
        string Name,
        string? SourceUri);

    private static DeploymentIdentifiers GetDeploymentIdentifiers(JsonObject properties)
    {
        var name = properties["name"]?.GetValue<string>();
        if (name == null)
        {
            throw new InvalidOperationException("Deployment name must have been set.");
        }

        var subscriptionId = properties["subscriptionId"]?.GetValue<string>();
        var resourceGroupName = properties["resourceGroup"]?.GetValue<string>();

        var scope = properties["scope"]?.GetValue<string>();
        if (scope != null)
        {
            throw new InvalidOperationException("Above-subscription scope is not supported yet.");
        }

        var sourceUri = properties["sourceUri"]?.GetValue<string>();

        return new(subscriptionId, resourceGroupName, name, sourceUri);
    }

    private static JsonObject FromIdentifiers(DeploymentIdentifiers identifiers)
        => JsonObject.Parse(identifiers.ToJsonStream())?.AsObject() ?? throw new UnreachableException("Serialization is not expected to fail.");

    private static void EnsureDeploymentType(string type)
    {
        if (type != "Microsoft.Resources/deployments")
        {
            throw new InvalidOperationException($"Unsupported type: {type}");
        }
    }

    private static LocalExtensibilityOperationResponse GetResponse(string type, string? apiVersion, DeploymentIdentifiers identifiers, LocalDeploymentResult result)
    {
        if (result.Deployment.Properties.ProvisioningState == ProvisioningState.Failed)
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
                identifiers: FromIdentifiers(identifiers),
                type: type,
                apiVersion: apiVersion,
                status: result.Deployment.Properties.ProvisioningState.ToString(),
                properties: JsonObject.Parse(result.Deployment.Properties.ToJsonStream())?.AsObject() ?? throw new UnreachableException("Serialization is not expected to fail.")),
            ErrorData: null);
    }

    public override async Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, CancellationToken cancellationToken)
    {
        EnsureDeploymentType(request.Type);

        var template = request.Properties["template"]!.ToString();
        var parameters = new JsonObject
        {
            ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
            ["contentVersion"] = "1.0.0.0",
            ["parameters"] = request.Properties["parameters"]?.DeepClone(),
        };

        var identifiers = GetDeploymentIdentifiers(request.Properties);

        if (identifiers.SubscriptionId != null)
        {
            try
            {
                GuardHelper.ArgumentNotNull(identifiers.SourceUri);
                var configuration = configurationManager.GetConfiguration(new Uri(identifiers.SourceUri));
                DeploymentLocator locator = new("", null, identifiers.SubscriptionId, identifiers.ResourceGroup, identifiers.Name);

                await LocalAzureDeployment.StartDeployment(configuration, credentialFactory, locator, template, parameters.ToJsonString(), cancellationToken);
                var result = await LocalAzureDeployment.CheckDeployment(configuration, credentialFactory, locator, cancellationToken);

                return GetResponse(request.Type, request.ApiVersion, identifiers, result);
            }
            catch (Exception ex)
            {
                return new LocalExtensibilityOperationResponse(
                    Resource: null,
                    ErrorData: new(new("InvalidDeployment", $"Failed to deploy to Azure. {ex}")));
            }
        }
        else
        {
            await localDeploymentEngine.StartDeployment(identifiers.Name, template, parameters.ToJsonString(), cancellationToken);
            var result = await localDeploymentEngine.CheckDeployment(identifiers.Name);

            return GetResponse(request.Type, request.ApiVersion, identifiers, result);
        }
    }

    public override async Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, CancellationToken cancellationToken)
    {
        EnsureDeploymentType(request.Type);

        var identifiers = GetDeploymentIdentifiers(request.Identifiers);

        if (identifiers.SubscriptionId != null)
        {
            try
            {
                GuardHelper.ArgumentNotNull(identifiers.SourceUri);
                var configuration = configurationManager.GetConfiguration(new Uri(identifiers.SourceUri));
                DeploymentLocator locator = new("", null, identifiers.SubscriptionId, identifiers.ResourceGroup, identifiers.Name);

                var result = await LocalAzureDeployment.CheckDeployment(configuration, credentialFactory, locator, cancellationToken);

                return GetResponse(request.Type, request.ApiVersion, identifiers, result);
            }
            catch (Exception ex)
            {
                return new LocalExtensibilityOperationResponse(
                    Resource: null,
                    ErrorData: new(new("InvalidDeployment", $"Failed to deploy to Azure. {ex}")));
            }
        }
        else
        {
            var result = await localDeploymentEngine.CheckDeployment(identifiers.Name);

            return GetResponse(request.Type, request.ApiVersion, identifiers, result);
        }
    }

    public override async Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        EnsureDeploymentType(request.Type);

        var identifiers = GetDeploymentIdentifiers(request.Properties);

        return new LocalExtensibilityOperationResponse(
            Resource: new Resource(
                identifiers: FromIdentifiers(identifiers),
                type: request.Type,
                apiVersion: request.ApiVersion,
                status: ProvisioningState.Succeeded.ToString(),
                properties: request.Properties),
            ErrorData: null);
    }

    public override Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}
