// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentStartParams(string documentPath, string parameterFilePath, string id, string deploymentScope, string location, string template, string token, string expiresOnTimestamp, string deployId, string portalUrl, IEnumerable<BicepDeploymentMissingParam> missingParamsWithValues) : IRequest<string>;

    public record BicepDeploymentStartResponse(bool isSuccess, string outputMessage, string? viewDeploymentInPortalMessage);

    public record BicepDeploymentMissingParam(string name, string value);

    public class BicepDeploymentStartCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeploymentStartParams, BicepDeploymentStartResponse>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly IMissingParamsCache missingParamsCache;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepDeploymentStartCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider, IDeploymentOperationsCache deploymentOperationsCache, ISerializer serializer, IMissingParamsCache missingParamsCache, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployStartCommand, serializer)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.missingParamsCache = missingParamsCache;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<BicepDeploymentStartResponse> Handle(BicepDeploymentStartParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential);

            string deploymentName = "bicep_deployment_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var bicepDeploymentStartResponse = await DeploymentHelper.StartDeploymentAsync(
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
                deploymentOperationsCache,
                request.missingParamsWithValues);

            missingParamsCache.CacheBicepDeploymentMissingParams(DocumentUri.FromFileSystemPath(request.documentPath), request.missingParamsWithValues);

            PostDeployStartResultTelemetryEvent(request.deployId, bicepDeploymentStartResponse.isSuccess);

            return bicepDeploymentStartResponse;
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
