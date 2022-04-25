// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public record BicepDeployWaitForCompletionParams(string deployId, string documentPath) : IRequest<string>;

    public class BicepDeployWaitForCompletionCommandHandler : ExecuteTypedResponseCommandHandlerBase<BicepDeployWaitForCompletionParams, string>
    {
        private readonly IDeploymentOperationsCache deploymentOperationsCache;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepDeployWaitForCompletionCommandHandler(IDeploymentOperationsCache deploymentOperationsCache, ISerializer serializer, ITelemetryProvider telemetryProvider)
            : base(LangServerConstants.DeployWaitForCompletionCommand, serializer)
        {
            this.deploymentOperationsCache = deploymentOperationsCache;
            this.telemetryProvider = telemetryProvider;
        }

        public override async Task<string> Handle(BicepDeployWaitForCompletionParams request, CancellationToken cancellationToken)
        {
            (bool isSuccess, string deploymentOutput) = await DeploymentHelper.WaitForDeploymentCompletionAsync(
                request.deployId,
                request.documentPath,
                deploymentOperationsCache);

            PostDeployResultTelemetryEvent(request.deployId, isSuccess);

            return deploymentOutput;
        }

        private void PostDeployResultTelemetryEvent(string deployId, bool isSuccess)
        {
            var result = isSuccess ? BicepTelemetryEvent.Result.Succeeded : BicepTelemetryEvent.Result.Failed;
            var telemetryEvent = BicepTelemetryEvent.CreateDeployResult(deployId, result);

            telemetryProvider.PostEvent(telemetryEvent);
        }
    }
}
