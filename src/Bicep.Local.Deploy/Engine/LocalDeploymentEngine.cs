// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Net;
using Azure.Deployments.Core.Constants;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Definitions.Extensibility;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.ErrorResponses;
using Azure.Deployments.Engine;
using Azure.Deployments.Engine.Exceptions;
using Azure.Deployments.Engine.Interfaces;
using Azure.Deployments.Templates.Engines;
using Microsoft.WindowsAzure.ResourceStack.Common.BackgroundJobs.Exceptions;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Instrumentation;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Local.Deploy.Engine;

public class LocalDeploymentEngine(
    LocalRequestContext requestContext,
    IAzureDeploymentSettings settings,
    AzureDeploymentEngine deploymentEngine,
    IDataProviderHolder dataProviderHolder,
    IDeploymentJobsDataProvider jobProvider,
    IDeploymentEntityFactory deploymentEntityFactory)
{
    static LocalDeploymentEngine()
    {
        CoreConstants.ResourcesLimitOverrideForLocalDeploy = int.MaxValue;
    }

    private static (Template template, Dictionary<string, DeploymentParameterDefinition> parameters, OrdinalDictionary<OrdinalDictionary<DeploymentExtensionConfigItem>>? extensionConfigs) ParseTemplateAndParameters(string templateString, string parametersString)
    {
        var template = TemplateParsingEngine.ParseTemplate(templateString);

        var paramsFileContent = parametersString.FromJson<DeploymentParametersDefinition>();

        return (template, paramsFileContent.Parameters, paramsFileContent.ExtensionConfigs);
    }

    public async Task StartDeployment(string name, string templateString, string parametersString, CancellationToken cancellationToken)
    {
        using (RequestCorrelationContext.NewCorrelationContextScope())
        {
            RequestCorrelationContext.Current.Initialize(apiVersion: requestContext.ApiVersion);

            var (template, parameters, extensionConfigs) = ParseTemplateAndParameters(templateString, parametersString);

            if (template.Resources.Any(x => x.Extension is null && x.Import is null))
            {
                throw new NotImplementedException("Only resources with extensions are supported");
            }

            var context = DeploymentRequestContextWithScopeDefinition.CreateAtResourceGroup(
                tenantId: Guid.Empty.ToString(),
                subscriptionId: Guid.Empty.ToString(),
                resourceGroupName: "local",
                resourceGroupLocation: requestContext.Location,
                deploymentName: name,
                subscriptionDefinition: null,
                resourceGroupDefinition: null,
                tenantDefinition: null);

            var definition = new DeploymentContent
            {
                Properties = new DeploymentPropertiesContent
                {
                    Template = template,
                    Parameters = parameters,
                    ExtensionConfigs = extensionConfigs,
                    Mode = DeploymentMode.Incremental,
                },
            };

            var oboToken = "dummyToken";
            var oboCorrelationId = RequestCorrelationContext.Current.CorrelationId;

            var deploymentPlan = await deploymentEngine.ProcessDeployment(
                preflightSettings: new(settings, syncMode: true),
                deploymentContext: context,
                definition: definition,
                originalDeploymentName: context.DeploymentName,
                isNestedDeployment: false,
                validateOnly: false,
                requestContext: null,
                frontdoorEndpointUri: new Uri(requestContext.FrontdoorEndpoint),
                cancellationToken: cancellationToken,
                oboToken: oboToken,
                oboCorrelationId: oboCorrelationId);

            await StartDeployment(deploymentPlan);
        }
    }

    public async Task<LocalDeploymentResult> CheckDeployment(string name)
    {
        using (RequestCorrelationContext.NewCorrelationContextScope())
        {
            RequestCorrelationContext.Current.Initialize(apiVersion: requestContext.ApiVersion);

            var context = DeploymentRequestContextWithScopeDefinition.CreateAtResourceGroup(
                tenantId: Guid.Empty.ToString(),
                subscriptionId: Guid.Empty.ToString(),
                resourceGroupName: "local",
                resourceGroupLocation: requestContext.Location,
                deploymentName: name,
                subscriptionDefinition: null,
                resourceGroupDefinition: null,
                tenantDefinition: null);

            var deploymentDataProvider = await dataProviderHolder.GetDeploymentDataProviderAsync(requestContext.Location);
            var entity = await deploymentDataProvider.FindDeployment(context.SubscriptionId, context.ResourceGroupName, context.DeploymentName);
            var operationEntities = await deploymentDataProvider.FindDeploymentOperations(
                context.SubscriptionId,
                context.ResourceGroupName,
                context.DeploymentName,
                entity.SequenceId,
                int.MaxValue,
                includeOrphanSequences: true);

            var deploymentEntityBySequence = new Dictionary<string, IDeploymentEntity>(StringComparer.OrdinalIgnoreCase)
            {
                [entity.SequenceId] = entity,
            };

            async Task<IDeploymentEntity> GetDeploymentEntityForSequence(string deploymentSequenceId)
            {
                if (deploymentEntityBySequence.TryGetValue(deploymentSequenceId, out var cachedEntity))
                {
                    return cachedEntity;
                }

                var deploymentEntity = await deploymentDataProvider.FindDeployment(
                    context.SubscriptionId,
                    context.ResourceGroupName,
                    context.DeploymentName,
                    deploymentSequenceId);

                deploymentEntityBySequence[deploymentSequenceId] = deploymentEntity;

                return deploymentEntity;
            }

            List<DeploymentOperationDefinition> operationDefinitions = [];
            foreach (var operation in operationEntities.Where(operation => operation.TargetResource is not null))
            {
                var deploymentEntity = await GetDeploymentEntityForSequence(operation.DeploymentSequenceId);
                operationDefinitions.Add(deploymentEngine.CreateDeploymentOperationDefinition(deploymentEntity, operation, requestContext.Location));
            }

            ImmutableArray<DeploymentOperationDefinition> operations = [.. operationDefinitions];

            var childDeployments = ImmutableDictionary.CreateBuilder<string, LocalDeploymentResult>(StringComparer.OrdinalIgnoreCase);
            foreach (var operation in operations)
            {
                if (operation.Properties.TargetResource is not { } targetResource ||
                    targetResource.Extension?.Alias is not "az0synthesized" ||
                    targetResource.Identifiers?["name"]?.ToString() is not { } childDeploymentName)
                {
                    continue;
                }

                // Only recurse into deployments that exist locally. Modules deployed to other scopes
                // (e.g. Azure) are not tracked here, so their wrapper operation is kept as a single row.
                if (await deploymentDataProvider.FindDeployment(context.SubscriptionId, context.ResourceGroupName, childDeploymentName) is null)
                {
                    continue;
                }

                childDeployments[targetResource.SymbolicName] = await CheckDeployment(childDeploymentName);
            }

            return new(
                deploymentEngine.CreateDeploymentDefinition(entity, requestContext.ApiVersion),
                operations,
                childDeployments.ToImmutable());
        }
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
        var deploymentDataProvider = await dataProviderHolder.GetDeploymentDataProviderAsync(requestContext.Location);
        var deploymentStatusDataProvider = await dataProviderHolder.GetDeploymentStatusDataProviderAsync(requestContext.Location);

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
