// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    public class BicepTelemetryHandler(ITelemetryProvider telemetryProvider, ISerializer serializer) : ExecuteTypedCommandHandlerBase<BicepTelemetryEvent>(TelemetryConstants.CommandName, serializer)
    {
        private readonly ITelemetryProvider TelemetryProvider = telemetryProvider;

        public override Task<Unit> Handle(BicepTelemetryEvent bicepTelemetryEvent, CancellationToken cancellationToken)
        {
            TelemetryProvider.PostEvent(bicepTelemetryEvent);
            return Unit.Task;
        }
    }
}
