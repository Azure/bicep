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

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeployParams(string documentPath, string parameterFilePath, string id, string deploymentScope, string location, string template, string token, string expiresOnTimestamp, string deployId) : IRequest<string>;

    public class BicepDeployCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeployParams, string>
    {
        private readonly IDeploymentCollectionProvider deploymentCollectionProvider;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepDeployCommandHandler(IDeploymentCollectionProvider deploymentCollectionProvider, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployCommand, serializer)
        {
            this.deploymentCollectionProvider = deploymentCollectionProvider;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<string> Handle(BicepDeployParams request, CancellationToken cancellationToken)
        {
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

            PostTelemetryEvent(request.deployId, isSuccess);
            return deploymentOutput;
        }

        private void PostTelemetryEvent(string deployId, bool isSuccess)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployResult(deployId, isSuccess ? "Succeeded" : "Failed");
            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
