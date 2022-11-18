// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core.Configuration;
using Bicep.Core.Tracing;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepUpdatedDeploymentParameter(string name, string value, bool isSecure, ParameterType? parameterType);

    public record BicepDeploymentStartParams(
        string documentPath,
        string parametersFilePath,
        string id,
        string deploymentScope,
        string location,
        string template,
        string token,
        string expiresOnTimestamp,
        string deployId,
        string deploymentName,
        string portalUrl,
        bool parametersFileExists,
        string parametersFileName,
        ParametersFileUpdateOption parametersFileUpdateOption,
        List<BicepUpdatedDeploymentParameter> updatedDeploymentParameters) : IRequest<string>;

    public record BicepDeploymentStartResponse(bool isSuccess, string outputMessage, string? viewDeploymentInPortalMessage);

    public class BicepDeploymentStartCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeploymentStartParams, BicepDeploymentStartResponse>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly IConfigurationManager configurationManager;

        public BicepDeploymentStartCommandHandler(IConfigurationManager configurationManager, IDeploymentCollectionProvider deploymentCollectionProvider, IDeploymentOperationsCache deploymentOperationsCache, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployStartCommand, serializer)
        {
            this.configurationManager = configurationManager;
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<BicepDeploymentStartResponse> Handle(BicepDeploymentStartParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var options = GetArmClientOptions(request.documentPath);
            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential, default, options);

            var bicepDeploymentStartResponse = await DeploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider,
                armClient,
                request.documentPath,
                request.template,
                request.parametersFilePath,
                request.id,
                request.deploymentScope,
                request.location,
                request.deployId,
                request.parametersFileName,
                request.parametersFileUpdateOption,
                request.updatedDeploymentParameters,
                request.portalUrl,
                request.deploymentName,
                deploymentOperationsCache);

            PostDeployStartResultTelemetryEvent(request.deployId, bicepDeploymentStartResponse.isSuccess);

            return bicepDeploymentStartResponse;
        }

        private ArmClientOptions GetArmClientOptions(string documentPath)
        {
            var documentUri = DocumentUri.FromFileSystemPath(documentPath);
            var rootConfiguration = configurationManager.GetConfiguration(documentUri.ToUri());
            var armEnvironment = new ArmEnvironment(rootConfiguration.Cloud.ResourceManagerEndpointUri, rootConfiguration.Cloud.ResourceManagerAudience);

            var options = new ArmClientOptions();
            options.Environment = armEnvironment;
            options.Diagnostics.ApplySharedResourceManagerSettings();

            return options;
        }

        private void PostDeployStartTelemetryEvent(string deployId)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStart(deployId);

            telemetryProvider.PostEvent(telemetryEvent);
        }

        private void PostDeployStartResultTelemetryEvent(string deployId, bool isSuccess)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStartOrWaitForCompletionResult(
                TelemetryConstants.EventNames.DeployStartResult,
                deployId,
                isSuccess);

            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
