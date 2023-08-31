// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeploymentWaitForCompletionParams(string deployId, string documentPath) : IRequest<bool>;

    public record BicepDeploymentWaitForCompletionResponse(bool isSuccess, string outputMessage);

    public class BicepDeploymentWaitForCompletionCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeploymentWaitForCompletionParams, bool>
    {
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly ILanguageServerFacade server;
        private readonly ITelemetryProvider telemetryProvider;

        /// <summary>
        /// Handles "deploy/waitForCompletion" LSP request.
        /// This handler waits for the deployment to complete and sends a "deploymentComplete" notification to the client.
        /// This notification can be used on the client side to write success/failure messsage to the output channel without
        /// blocking other commands.
        /// Note: Base handler (ExecuteTypedResponseCommandHandlerBase) is serial. This blocks other commands on the client side.
        /// To avoid the above issue, we changed the RequestProcessType to parallel in <see cref="Server"/>
        /// We need to make sure changes to this handler are thread safe.
        /// </summary>
        public BicepDeploymentWaitForCompletionCommandHandler(IDeploymentOperationsCache deploymentOperationsCache, ILanguageServerFacade server, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployWaitForCompletionCommand, serializer)
        {
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.server = server;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<bool> Handle(BicepDeploymentWaitForCompletionParams request, CancellationToken cancellationToken)
        {
            // Wait for the deployment to complete
            var bicepDeployWaitForCompletionResponse = await DeploymentHelper.WaitForDeploymentCompletionAsync(
                request.deployId,
                request.documentPath,
                deploymentOperationsCache);

            // Send notification to client informing deployment completion, with message to be written to output channel
            server.SendNotification(LangServerConstants.DeployCompleteMethod, bicepDeployWaitForCompletionResponse.outputMessage);

            PostDeployResultTelemetryEvent(request.deployId, bicepDeployWaitForCompletionResponse.isSuccess);

            return true;
        }

        private void PostDeployResultTelemetryEvent(string deployId, bool isSuccess)
        {
            var telemetryEvent = BicepTelemetryEvent.CreateDeployStartOrWaitForCompletionResult(
                TelemetryConstants.EventNames.DeployResult,
                deployId,
                isSuccess);

            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
