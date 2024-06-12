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
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Identifiers;
using Azure.Deployments.Core.Definitions.Resources;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Core.EventSources;
using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Core.Extensions;
using Azure.Deployments.Core.FeatureEnablement;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Core.PerformanceCounters;
using Azure.Deployments.Core.Uri;
using Azure.Deployments.Engine.Extensions;
using Azure.Deployments.Engine.Helpers;
using Azure.Deployments.Engine.Host.Azure.Constants;
using Azure.Deployments.Engine.Host.Azure.Definitions;
using Azure.Deployments.Engine.Host.Azure.Exceptions;
using Azure.Deployments.Engine.Host.Azure.Interfaces;
using Azure.Deployments.Engine.Host.Azure.Workers.Metadata;
using Azure.Deployments.Engine.Host.External;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.Extensibility.Messages;
using Azure.Deployments.ResourceMetadata.Contracts;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Instrumentation;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Microsoft.WindowsAzure.ResourceStack.Common.Services.ADAuthentication;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy;

#nullable disable

public class LocalDeploymentEngineHost : DeploymentEngineHostBase
{
    private readonly LocalExtensibilityHandler extensibilityHandler;

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
    }

    public override Task<HttpResponseMessage> DownloadContent(Uri requestUri, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public override Task<ResourceGroupInfo> GetResourceGroup(
        string subscriptionId,
        string resourceGroupName,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId)
        => throw new NotImplementedException();

    public override Task<IReadOnlyList<ResourceId>> GetTrackedResourceIds(
        ResourceGroupInfo resourceGroup,
        Func<ResourceGroupLevelResourceId, bool> resourceIdFilterFunc,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId)
        => throw new NotImplementedException();

    public override Task<ResourceTypeRegistrationInfo[]> FindRegistrationsForSubscription(
        string subscriptionId,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId)
        => throw new NotImplementedException();

    public override Task<HttpResponseMessage> CallFrontdoorService(
        HttpMethod requestMethod,
        Uri requestUri,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId,
        HttpRequestMessage requestMessage = null,
        HttpContent content = null,
        Action<HttpRequestHeaders> addHeadersFunc = null,
        Action<IDictionary<string, object>> requestPropertiesEnricher = null,
        AuthenticationToken frontdoorAuthenticationToken = null)
        => throw new NotImplementedException();


    public override Task<HttpResponseMessage> CallFrontdoorServiceWithoutAuthentication(HttpMethod requestMethod, Uri requestUri, CancellationToken cancellationToken, HttpContent content = null)
        => throw new NotImplementedException();

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
    }

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

    protected override Task<JToken> GetEnvironmentKey()
        => Task.FromResult<JToken>(new JObject());

    public override Task ValidateDeploymentLocationAcceptable(IDeploymentRequestContext deploymentContext, string deploymentLocation, string oboToken, string oboCorrelationId)
        => Task.CompletedTask;

    public override void AddAsyncNotificationUri(HttpRequestHeaders httpHeaders, BackgroundJob backgroundJob, DeploymentResourceJobMetadata deploymentJobMetadata, JobLogger jobLogger)
        => throw new NotImplementedException();

    public override EnablementConfig GetEnablementConfig(PreviewDeploymentFunction feature)
        => new()
        {
            FeatureName = feature.ToString()
        };

    public override IResourceTypeMetadataProvider GetResourceTypeMetadataProvider()
        => throw new NotImplementedException();
}
