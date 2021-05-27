// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to send a request from the client to the server(E.g. command sent as part
    // of completion item of top level declaration snippet) which in turn triggers a request from server to client,
    // asking the client to log a telemetry event
    // Flow of events:
    // 1. workspace/executeCommand request is sent from the client to the server
    // 2. The above triggers telemetry/event from server to client
    public class BicepTelemetryHandler : ExecuteTypedCommandHandlerBase<BicepTelemetryEvent>
    {
        private readonly ITelemetryProvider TelemetryProvider;

        public BicepTelemetryHandler(ITelemetryProvider telemetryProvider, ISerializer serializer)
           : base(TelemetryConstants.CommandName, serializer)
        {
            TelemetryProvider = telemetryProvider;
        }

        public override Task<Unit> Handle(BicepTelemetryEvent bicepTelemetryEvent, CancellationToken cancellationToken)
        {
            TelemetryProvider.PostEvent(bicepTelemetryEvent);
            return Unit.Task;
        }
    }
}
