// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using static Bicep.LanguageServer.Deploy.DeploymentsCache;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeployParams(
        string subscriptionId,
        string documentPath,
        string parameterFilePath,
        string id,
        string deploymentScope,
        string location,
        string template,
        string token,
        string expiresOnTimestamp,
        string deployId) : IRequest<string>;

    public class BicepDeployCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeployParams, string>
    {
        private readonly IDeploymentsCache deploymentsCache;
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepDeployCommandHandler(IDeploymentsCache deploymentsCache, IDeploymentCollectionProvider deploymentCollectionProvider, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployCommand, serializer)
        {
            this.deploymentsCache = deploymentsCache;
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<string> Handle(BicepDeployParams request, CancellationToken cancellationToken)
        {
            PostDeployStartTelemetryEvent(request.deployId);

            var credential = new CredentialFromTokenAndTimeStamp(request.token, request.expiresOnTimestamp);
            var armClient = new ArmClient(credential);

            (bool isSuccess, string deploymentOutput) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider,
                armClient,
                request.documentPath,
                request.template,
                request.parameterFilePath,
                request.id,
                request.deploymentScope,
                request.location);

            PostDeployResultTelemetryEvent(request.deployId, isSuccess);

            var lastUsedDefaults = new LastUsedDefaults(request.subscriptionId, request.id, request.parameterFilePath, request.location);
            deploymentsCache.UpdateDeploymentsCache(request.documentPath, lastUsedDefaults);

            return deploymentOutput;
        }

        private void PostDeployStartTelemetryEvent(string deployId)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStart(deployId);

            telemetryProvider.PostEvent(telemetryEvent);
        }

        private void PostDeployResultTelemetryEvent(string deployId, bool isSuccess)
        {
            var result = isSuccess ? BicepTelemetryEvent.Result.Succeeded : BicepTelemetryEvent.Result.Failed;
            var telemetryEvent = BicepTelemetryEvent.CreateDeployResult(deployId, result);

            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
