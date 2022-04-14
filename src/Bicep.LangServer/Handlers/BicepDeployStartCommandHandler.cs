// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeployParams(string documentPath, string parameterFilePath, string id, string deploymentScope, string location, string template, string token, string expiresOnTimestamp, string deployId, string portalUrl) : IRequest<string>;

    public record BicepDeployStartResponse(bool isSuccess, string outputMessage);

    public class BicepDeployStartCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeployParams, BicepDeployStartResponse>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepDeployStartCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider, IDeploymentOperationsCache deploymentOperationsCache, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployStartCommand, serializer)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<BicepDeployStartResponse> Handle(BicepDeployParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential);

            string deploymentName = "bicep_deployment_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            (bool isSuccess, string deploymentOutput) = await DeploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider,
                armClient,
                request.documentPath,
                request.template,
                request.parameterFilePath,
                request.id,
                request.deploymentScope,
                request.location,
                request.deployId,
                request.portalUrl,
                deploymentName,
                deploymentOperationsCache);

            return new BicepDeployStartResponse(isSuccess, deploymentOutput);
        }

        private void PostDeployStartTelemetryEvent(string deployId)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStart(deployId);

            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
