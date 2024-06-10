// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.Engine.Host.Azure;
using Azure.Deployments.Engine.Host.Azure.Constants;
using Azure.Deployments.Engine.Host.Azure.Definitions;
using Azure.Deployments.Engine.Host.Azure.Exceptions;
using Azure.Deployments.Engine.Host.Azure.Helpers;
using Azure.Deployments.Engine.Host.Azure.Interfaces;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.Templates.Engines;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Azure.Deployments.Shared.Host;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs.Exceptions;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Instrumentation;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Local.Deploy;

internal class LocalDeploymentEngine
{

    public LocalDeploymentEngine(
        LocalRequestContext requestContext,
        IAzureDeploymentSettings settings,
        AzureDeploymentEngine deploymentEngine,
        IDataProviderHolder dataProviderHolder,
        IDeploymentJobsDataProvider jobProvider,
        IAzureDeploymentEngineHost host,
        IDeploymentEntityFactory deploymentEntityFactory)
    {
        this.requestContext = requestContext;
        this.settings = settings;
        this.deploymentEngine = deploymentEngine;
        this.dataProviderHolder = dataProviderHolder;
        this.jobProvider = jobProvider;
        this.host = host;
        this.deploymentEntityFactory = deploymentEntityFactory;
    }

    private readonly LocalRequestContext requestContext;
    private readonly IAzureDeploymentSettings settings;
    private readonly AzureDeploymentEngine deploymentEngine;
    private readonly IDataProviderHolder dataProviderHolder;
    private readonly IDeploymentJobsDataProvider jobProvider;
    private readonly IAzureDeploymentEngineHost host;
    private readonly IDeploymentEntityFactory deploymentEntityFactory;

    private static (Template template, Dictionary<string, DeploymentParameterDefinition> parameters) ParseTemplateAndParameters(string templateString, string parametersString)
    {
        var template = TemplateParsingEngine.ParseTemplate(templateString);

        var parameters = parametersString.FromJson<DeploymentParametersDefinition>().Parameters;

        return (template, parameters);
    }

    public async Task<LocalDeployment.Result> Deploy(string templateString, string parametersString, CancellationToken cancellationToken)
    {
        using (RequestCorrelationContext.NewCorrelationContextScope())
        {
            RequestCorrelationContext.Current.Initialize(apiVersion: requestContext.ApiVersion);

            var (template, parameters) = ParseTemplateAndParameters(templateString, parametersString);

            if (template.Resources.Any(x => x.Import is null))
            {
                throw new NotImplementedException("Only resources with imports are supported");
            }

            var context = DeploymentContextWithScopeDefinition.CreateAtResourceGroup(
                tenantId: Guid.Empty.ToString(),
                subscriptionId: Guid.Empty.ToString(),
                resourceGroupName: "mockRg",
                resourceGroupLocation: requestContext.Location,
                deploymentName: "main",
                subscriptionDefinition: null,
                resourceGroupDefinition: null,
                tenantDefinition: null);

            var definition = new DeploymentContent
            {
                Properties = new DeploymentPropertiesContent
                {
                    Template = template,
                    Parameters = parameters,
                    Mode = DeploymentMode.Incremental,
                },
            };

            var oboToken = "dummyToken";
            var oboCorrelationId = RequestCorrelationContext.Current.CorrelationId;

            var deploymentPlan = await this.deploymentEngine.ProcessDeployment(
                preflightSettings: new(settings, syncMode: true),
                deploymentContext: context,
                definition: definition,
                originalDeploymentName: context.DeploymentName,
                isNestedDeployment: false,
                validateOnly: false,
                request: null,
                frontdoorEndpointUri: new Uri(requestContext.FrontdoorEndpoint),
                cancellationToken: cancellationToken,
                oboToken: oboToken,
                oboCorrelationId: oboCorrelationId);

            await StartDeployment(deploymentPlan);

            var dataProvider = dataProviderHolder.GetDeploymentDataProvider(deploymentPlan.DeploymentLocation);
            return await WaitForDeploymentToComplete(dataProvider, context, cancellationToken);
        }
    }

    private async Task<LocalDeployment.Result> WaitForDeploymentToComplete(IDeploymentDataProvider dataProvider, DeploymentContext context, CancellationToken cancellationToken)
    {
        var entity = await dataProvider.FindDeployment(context.SubscriptionId, context.ResourceGroupName, context.DeploymentName);
        while (!entity.ProvisioningState.IsTerminal())
        {
            await Task.Delay(20, cancellationToken);

            entity = await dataProvider.FindDeployment(context.SubscriptionId, context.ResourceGroupName, context.DeploymentName);
        }

        var operationEntities = await dataProvider.FindDeploymentOperations(context.SubscriptionId, context.ResourceGroupName, context.DeploymentName, entity.SequenceId, -1);

        return new(
            deploymentEngine.CreateDeploymentDefinition(entity, requestContext.ApiVersion),
            operationEntities
                .Where(operation => operation.TargetResource is not null)
                .Select(operation => deploymentEngine.CreateDeploymentOperationDefinition(entity, operation)).ToImmutableArray());
    }

    private async Task StartDeployment(DeploymentPlan deploymentPlan)
    {
        try
        {
            await jobProvider.CreateDeploymentJob(
                deploymentJob: deploymentPlan.DeploymentJob,
                onCommitJobDefinitionDelegate: () => SaveDeployment(deploymentPlan)).ConfigureAwait(false);
        }
        catch (JobSizeExceededException exception)
        {
            var message = ErrorResponseMessages.DeploymentJobSizeExceeded.ToLocalizedMessage();

            throw new ErrorResponseMessageException(
                httpStatus: HttpStatusCode.BadRequest,
                errorCode: DeploymentsErrorResponseCode.DeploymentSizeExceeded,
                errorMessage: message,
                innerException: exception);
        }
    }

    private async Task SaveDeployment(DeploymentPlan deploymentPlan)
    {
        var deploymentDataProvider = dataProviderHolder.GetDeploymentDataProvider(deploymentPlan.DeploymentLocation);
        var deploymentStatusDataProvider = dataProviderHolder.GetDeploymentStatusDataProvider(deploymentPlan.DeploymentLocation);

        if (deploymentPlan.OldDeployment != null)
        {
            await deploymentDataProvider.DeleteDeployment(
                deployment: deploymentPlan.OldDeployment).ConfigureAwait(false);
        }

        await deploymentDataProvider.SaveDeployment(
            deployment: deploymentPlan.NewDeployment).ConfigureAwait(false);

        var statusEntity = deploymentEntityFactory.CreateDeploymentStatusEntity(
            partitionKey: deploymentPlan.NewDeployment.PartitionKey,
            entityId: deploymentPlan.NewDeployment.StatusEntityId,
            provisioningState: deploymentPlan.NewDeployment.ProvisioningState);

        await deploymentStatusDataProvider.SaveDeploymentStatus(
            entity: statusEntity).ConfigureAwait(false);
    }
}
