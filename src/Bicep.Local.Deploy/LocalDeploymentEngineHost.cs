// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Identifiers;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.EventSources;
using Azure.Deployments.Core.Exceptions;
using Azure.Deployments.Core.FeatureEnablement;
using Azure.Deployments.Core.Telemetry;
using Azure.Deployments.Engine.Host.Azure.Interfaces;
using Azure.Deployments.Engine.Host.Azure.Workers.Metadata;
using Azure.Deployments.Engine.Host.External;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.ResourceMetadata.Contracts;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Microsoft.WindowsAzure.ResourceStack.Common.Services.ADAuthentication;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy;

#nullable disable

public class LocalDeploymentEngineHost : DeploymentEngineHostBase
{
    private readonly LocalExtensibilityHostManager extensibilityHandler;

    public readonly record struct ExtensionInfo(string ExtensionName, string ExtensionVersion, string Method);

    public LocalDeploymentEngineHost(
        LocalExtensibilityHostManager extensibilityHandler,
        IDeploymentsRequestContext requestContext,
        IDeploymentEventSource deploymentEventSource,
        IKeyVaultDataProvider keyVaultDataProvider,
        IAzureDeploymentSettings settings,
        IDataProviderHolder dataProviderHolder,
        ITemplateExceptionHandler exceptionHandler,
        IEnablementConfigProvider enablementConfigProvider,
        IHttpContentHandler contentHandler,
        IDeploymentMetricsReporter metricsReporter)
        : base(settings, contentHandler, deploymentEventSource, metricsReporter, keyVaultDataProvider, requestContext, dataProviderHolder, exceptionHandler, enablementConfigProvider)
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
        string oboCorrelationId,
        string auxToken)
        => throw new NotImplementedException();

    public override Task<IReadOnlyList<ResourceId>> GetTrackedResourceIds(
        ResourceGroupInfo resourceGroup,
        Func<ResourceGroupLevelResourceId, bool> resourceIdFilterFunc,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId,
        string auxToken)
        => throw new NotImplementedException();

    public override Task<ResourceTypeRegistrationInfo[]> FindRegistrationsForSubscription(
        string subscriptionId,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId,
        string auxToken)
        => throw new NotImplementedException();

    public override Task<HttpResponseMessage> CallFrontdoorService(
        HttpMethod requestMethod,
        Uri requestUri,
        CancellationToken cancellationToken,
        string oboToken,
        string oboCorrelationId,
        string auxToken,
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


    public override async Task<HttpResponseMessage> CallExtensibilityHostV2(
        HttpMethod requestMethod,
        Uri requestUri,
        HttpContent content,
        AuthenticationToken extensibilityHostToken,
        CancellationToken cancellationToken)
    {
        var extensionName = requestUri.Segments[^4].TrimEnd('/');
        var extensionVersion = requestUri.Segments[^3].TrimEnd('/');
        var method = requestUri.Segments[^1].TrimEnd('/');

        var extensionInfo = new ExtensionInfo(extensionName, extensionVersion, method);

        return await extensibilityHandler.CallExtensibilityHost(extensionInfo, content, cancellationToken);
    }

    protected override Task<JToken> GetEnvironmentKey()
        => Task.FromResult<JToken>(new JObject());

    public override Task ValidateDeploymentLocationAcceptable(IDeploymentRequestContext deploymentContext, string deploymentLocation, string oboToken, string oboCorrelationId, string auxToken)
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

    public override void ValidateNetworkAddress(Uri uri)
    {
        return;
    }
}
