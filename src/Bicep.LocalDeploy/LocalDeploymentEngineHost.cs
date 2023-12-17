// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Azure.Bicep.LocalDeploy.Extensibility;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Identifiers;
using Azure.Deployments.Core.Definitions.Resources;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Core.EventSources;
using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Core.PerformanceCounters;
using Azure.Deployments.Core.Uri;
using Azure.Deployments.Engine.Extensions;
using Azure.Deployments.Engine.Helpers;
using Azure.Deployments.Engine.Host.Azure.Constants;
using Azure.Deployments.Engine.Host.Azure.Definitions;
using Azure.Deployments.Engine.Host.Azure.Exceptions;
using Azure.Deployments.Engine.Host.Azure.Interfaces;
using Azure.Deployments.Engine.Host.External;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.Extensibility.Messages;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Instrumentation;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Microsoft.WindowsAzure.ResourceStack.Common.Services.ADAuthentication;
using Newtonsoft.Json.Linq;

namespace Azure.Bicep.LocalDeploy;

#nullable disable

public class LocalDeploymentEngineHost : DeploymentEngineHostBase
{
    private readonly LocalExtensibilityHandler extensibilityHandler;

    private readonly IDeploymentsRequestContext requestContext;

    private const int maxPagingRequests = 100;

    public LocalDeploymentEngineHost(
        LocalExtensibilityHandler extensibilityHandler,
        IDeploymentsRequestContext requestContext,
        IDeploymentEventSource deploymentEventSource,
        IKeyVaultDataProvider keyVaultDataProvider,
        IAzureDeploymentSettings settings,
        IDataProviderHolder dataProviderHolder,
        ITemplateExceptionHandler exceptionHandler)
        : base(settings, deploymentEventSource, keyVaultDataProvider, requestContext, dataProviderHolder, exceptionHandler)
    {
        this.extensibilityHandler = extensibilityHandler;
        this.requestContext = requestContext;
    }

    public override async Task<HttpResponseMessage> DownloadContent(Uri requestUri, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        return await httpClient.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get resource group information from subscription Id and resource group name.
    /// </summary>
    /// <param name="subscriptionId">The subscription Id.</param>
    /// <param name="resourceGroupName">The resource group name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="oboToken">Deployments micro-service should set OBO token when calling back to frontdoor.</param>
    /// <param name="oboCorrelationId">Deployments micro-service should set the correlationId used for OBO token when calling back to frontdoor.</param>
    public override async Task<ResourceGroupInfo> GetResourceGroup(
        string subscriptionId,
        string resourceGroupName,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId)
    {
        using var timer = new ExecutionTimer($"{nameof(LocalDeploymentEngineHost)}.{nameof(GetResourceGroup)}", this.eventSource);
        var requestUri = $"{this.requestContext.FrontdoorEndpoint}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}?api-version={ServiceConstants.DefaultFrontdoorApiVersion}";

        using var response = await CallFrontdoorService(
                requestMethod: HttpMethod.Get,
                requestUri: new Uri(requestUri),
                cancellationToken: cancellationToken,
                oboToken: oboToken,
                oboCorrelationId: oboCorrelationId).ConfigureAwait(false);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            this.eventSource.Error(
                operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(GetResourceGroup)}",
                message: $"Frontdoor returned unexpected status code: {response.StatusCode} with content: {response.Content?.ReadAsStringAsync()}.");

            return null;
        }

        var resourceGroupResponse = await response.ToObjectFromJsonStream<ResourceGroupDefinitionInfo>().ConfigureAwait(false);
        var resourceGroup = resourceGroupResponse.ToResourceGroupInfo();
        if (string.IsNullOrEmpty(resourceGroup.SubscriptionId))
        {
            resourceGroup.SubscriptionId = subscriptionId; // Use the passed in subscriptionId in case we couldn't set it from the resource group response.
        }

        return resourceGroup;
    }

    /// <summary>
    /// Get a collection of ResourceId from a resource group.
    /// </summary>
    /// <param name="resourceGroup">The resource group info.</param>
    /// <param name="resourceIdFilterFunc">Function that filters resource Id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="oboToken">Deployments micro-service should set OBO token when calling back to frontdoor.</param>
    /// <param name="oboCorrelationId">Deployments micro-service should set the correlationId used for OBO token when calling back to frontdoor.</param>
    public override async Task<IReadOnlyList<ResourceId>> GetTrackedResourceIds(
        ResourceGroupInfo resourceGroup,
        Func<ResourceGroupLevelResourceId, bool> resourceIdFilterFunc,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId)
    {
        using var timer = new ExecutionTimer($"{nameof(LocalDeploymentEngineHost)}.{nameof(GetTrackedResourceIds)}", this.eventSource);
        var result = new List<ResourceId>();
        var nextLink = $"{this.requestContext.FrontdoorEndpoint}/subscriptions/{resourceGroup.SubscriptionId}/resourcegroups/{resourceGroup.ResourceGroupName}/resources?api-version={ServiceConstants.DefaultFrontdoorApiVersion}";
        var maxPagesRemaining = maxPagingRequests;

        while (maxPagesRemaining > 0 && nextLink != null)
        {
            using var response = await this.CallFrontdoorService(
                    requestMethod: HttpMethod.Get,
                    requestUri: new Uri(nextLink),
                    cancellationToken: cancellationToken,
                    oboToken: oboToken,
                    oboCorrelationId: oboCorrelationId).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                this.eventSource.Error(
                    operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(GetTrackedResourceIds)}",
                    message: $"Frontdoor returned unexpected status code: {response.StatusCode} with content: {response.Content?.ReadAsStringAsync()}.");

                throw new ErrorResponseMessageException(
                    httpStatus: HttpStatusCode.InternalServerError,
                    errorCode: DeploymentsErrorResponseCode.InternalServerError,
                    errorMessage: ErrorResponseMessages.UnexpectedResourcesAPICallStatusCode.ToLocalizedMessage(response.StatusCode));
            }

            var resourcesWithContinuation = await
                this.DeserializeAsResponseWithContinuation<ResourceProxyDefinition>(response.Content).ConfigureAwait(false);

            if (resourcesWithContinuation == null)
            {
                this.eventSource.Error(
                    operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(GetTrackedResourceIds)}",
                    message: "Frontdoor did not return valid content for collection request.");

                throw new ErrorResponseMessageException(
                    httpStatus: HttpStatusCode.InternalServerError,
                    errorCode: DeploymentsErrorResponseCode.InternalServerError,
                    errorMessage: ErrorResponseMessages.UnknownErrorOccurred.ToLocalizedMessage());
            }

            result.AddRange(FilterResources(resourceGroup.SubscriptionId, resourceGroup.ResourceGroupName, resourcesWithContinuation.Value, resourceIdFilterFunc).ToList());

            nextLink = string.IsNullOrEmpty(resourcesWithContinuation.NextLink)
                ? null
                : resourcesWithContinuation.NextLink;

            maxPagesRemaining--;
        }

        // Provider continued to indicate new page after the limit was reached
        if (maxPagesRemaining < 1 && nextLink != null)
        {
            this.eventSource.Error(
                operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(GetTrackedResourceIds)}",
                message: "Frontdoor should not provide next link after the request limit is reached.");
        }

        return result.AsReadOnly();
    }

    /// <summary>
    /// Fetches the available resource type registrations.
    /// </summary>
    /// <param name="subscriptionId">The subscription Id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="oboToken">Deployments micro-service should set OBO token when calling back to frontdoor.</param>
    /// <param name="oboCorrelationId">Deployments micro-service should set the correlationId used for OBO token when calling back to frontdoor.</param>
    public override async Task<ResourceTypeRegistrationInfo[]> FindRegistrationsForSubscription(
        string subscriptionId,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId)
    {
        using var timer = new ExecutionTimer($"{nameof(LocalDeploymentEngineHost)}.{nameof(FindRegistrationsForSubscription)}", this.eventSource);
        var nextLink = $"{this.requestContext.FrontdoorEndpoint}/subscriptions/{subscriptionId}/providers?api-version={ServiceConstants.ResourceProvidersDefaultApiVersion}&$expand=resourceTypes/deploymentsMetadata";
        var maxPagesRemaining = maxPagingRequests;
        var result = Enumerable.Empty<ResourceTypeRegistrationInfo>();

        while (maxPagesRemaining > 0 && nextLink != null)
        {
            using var response = await CallFrontdoorService(
                    requestMethod: HttpMethod.Get,
                    requestUri: new Uri(nextLink),
                    cancellationToken: cancellationToken,
                    oboToken: oboToken,
                    oboCorrelationId: oboCorrelationId).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                this.eventSource.Error(
                    operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(FindRegistrationsForSubscription)}",
                    message: $"Frontdoor returned unexpected status code: {response.StatusCode} with content: {response.Content?.ReadAsStringAsync()}.");

                throw new ErrorResponseMessageException(
                    httpStatus: HttpStatusCode.InternalServerError,
                    errorCode: DeploymentsErrorResponseCode.InternalServerError,
                    errorMessage: ErrorResponseMessages.UnexpectedProvidersAPICallStatusCode.ToLocalizedMessage(response.StatusCode));
            }

            var rpDefinitions = await
                this.DeserializeAsResponseWithContinuation<ResourceProviderInfo>(response.Content).ConfigureAwait(false);

            if (rpDefinitions == null)
            {
                this.eventSource.Error(
                    operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(FindRegistrationsForSubscription)}",
                    message: "Frontdoor did not return valid content for collection request.");

                throw new ErrorResponseMessageException(
                    httpStatus: HttpStatusCode.InternalServerError,
                    errorCode: DeploymentsErrorResponseCode.InternalServerError,
                    errorMessage: ErrorResponseMessages.UnknownErrorOccurred.ToLocalizedMessage());
            }

            result = result.CoalesceConcat(rpDefinitions.Value
                .SelectMany(
                    definition => definition.ResourceTypes,
                    (definition, resourceType) => new { Namespace = definition.Namespace, Type = resourceType })
                .SelectMany(
                    namespaceAndType => namespaceAndType.Type.ApiVersions,
                    (namespaceAndType, version) => new ResourceTypeRegistrationInfo
                    {
                        AsyncTimeoutRules = namespaceAndType.Type.DeploymentsMetadata?.AsyncTimeoutRules,
                        RoutingType = namespaceAndType.Type.DeploymentsMetadata?.RoutingType ?? RoutingTypeInfo.Default,
                        ResourceProviderNamespace = namespaceAndType.Type.DeploymentsMetadata?.ResourceProviderNamespace ?? namespaceAndType.Namespace,
                        ResourceType = namespaceAndType.Type.DeploymentsMetadata?.ResourceType ?? namespaceAndType.Type.ResourceType,
                        IsEnabled = namespaceAndType.Type.DeploymentsMetadata?.IsEnabled ?? true,
                        AllowedResourceNames = namespaceAndType.Type.DeploymentsMetadata?.AllowedResourceNames,
                        DisallowedActionVerbs = namespaceAndType.Type.DeploymentsMetadata?.DisallowedActionVerbs,
                        ApiVersion = version
                    }));

            nextLink = string.IsNullOrEmpty(rpDefinitions.NextLink) ? null : rpDefinitions.NextLink;
            maxPagesRemaining--;
        }

        // Provider continued to indicate new page after the limit was reached
        if (maxPagesRemaining < 1 && nextLink != null)
        {
            this.eventSource.Error(
                operationName: $"{nameof(LocalDeploymentEngineHost)}.{nameof(FindRegistrationsForSubscription)}",
                message: "Frontdoor should not provide next link after the request limit is reached.");
        }

        return result.ToArray();
    }

    /// <summary>
    /// Calls the front door service.
    /// </summary>
    /// <param name="requestMessage">The request message of the original HTTP request. It's not used in current implementation.</param>
    /// <param name="requestMethod">The request method.</param>
    /// <param name="requestUri">The request Uri.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="oboToken">Deployments micro-service should set OBO token when calling back to frontdoor.</param>
    /// <param name="oboCorrelationId">Deployments micro-service should set the correlationId used for OBO token when calling back to frontdoor.</param>
    /// <param name="content">The HTTP message content.</param>
    /// <param name="addHeadersFunc">Callback to add headers.</param>
    public override Task<HttpResponseMessage> CallFrontdoorService(
        HttpMethod requestMethod,
        Uri requestUri,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId,
        HttpRequestMessage requestMessage = null,
        HttpContent content = null,
        Action<HttpRequestHeaders> addHeadersFunc = null,
        Action<IDictionary<string, object>> requestPropertiesEnricher = null)
    {
        throw new NotImplementedException();
    }


    public override Task<HttpResponseMessage> CallFrontdoorServiceWithoutAuthentication(HttpMethod requestMethod, Uri requestUri, CancellationToken cancellationToken, HttpContent content = null)
    {
        throw new NotImplementedException();
    }

    public override void LogEventServiceEntry(Uri resourceUri, HttpStatusCode? statusCode, JToken statusMessage, int channels, string managementGroupHierarchy, string subscriptionId, string resourceProviderName, string jobId, string subStatus = null)
    {
    }

    public override string SanitizeString(string stringToBeSanitized)
    {
        return stringToBeSanitized;
    }

    protected override async Task<T> ReadAsJson<T>(HttpContent content, bool rewindContentStream = false)
    {
        using var contentStream = await content.ReadAsStreamAsync();

        return contentStream.FromJsonStream<T>();
    }

    protected override async Task<T> TryReadAsJson<T>(HttpContent content, bool rewindContentStream = false)
    {
        try
        {
            return await ReadAsJson<T>(content, rewindContentStream);
        }
        catch
        {
            return default;
        }
    }

    protected override async Task<string> TryReadAsString(HttpContent content, bool rewindContentStream = false)
    {
        try
        {
            return await content.ReadAsStringAsync();
        }
        catch
        {
            return default;
        }
    }    public override bool IsEnabledForSubscription(PreviewDeploymentFunction previewFunction, string subscriptionId)
        => true;

    public override bool IsEnabledForTenant(PreviewDeploymentFunction previewFunction, string tenantId)
        => true;

    public override async Task<HttpResponseMessage> CallExtensibilityHost(
        HttpMethod requestMethod,
        Uri requestUri,
        ExtensibilityOperationRequest request,
        AuthenticationToken extensibilityHostToken,
        CancellationToken cancellationToken)
    {
        var response = await extensibilityHandler.CallExtensibilityHost(requestUri.Segments[^1], request, cancellationToken);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(response.ToJson()),
        };
    }

    protected override async Task<JToken> GetEnvironmentKey()
    {
        // TODO detect whether we're running in 'local mode'
        await Task.Yield();

        return new JObject();
    }

    public override async Task ValidateDeploymentLocationAcceptable(IDeploymentRequestContext deploymentContext, string deploymentLocation, string oboToken, string oboCorrelationId)
    {
        // TODO detect whether we're running in 'local mode'
        await Task.Yield();
    }

    /// <summary>
    /// Filters resources by a given resource filter function
    /// </summary>
    /// <param name="resources">The resources</param>
    /// <param name="resourceIdFilterFunc">The resource filter function</param>
    private static IEnumerable<ResourceId> FilterResources(string subscriptionId, string resourceGroupName, IEnumerable<ResourceProxyDefinition> resources, Func<ResourceGroupLevelResourceId, bool> resourceIdFilterFunc)
    {
        foreach (var resource in resources)
        {
            // Need to explicitly set here since ResourceProxyDefinition from API response does not have subscritionId or resourceGroup.
            resource.SubscriptionId = subscriptionId;
            resource.ResourceGroup = resourceGroupName;

            if (ResourceGroupLevelResourceId.TryParse(resource.Id, out var resourceId)
                && resourceIdFilterFunc(resourceId))
            {
                yield return resourceId;
            }
        }
    }
}
