// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.Telemetry;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    // This handler is used to send a request from the client to the server(E.g. command sent as part
    // of completion item of top level declaration snippet) which in turn triggers a request from server to client,
    // asking the client to log a telemetry event
    // Flow of events:
    // 1. workspace/executeCommand request is sent from the client to the server
    // 2. The above triggers telemetry/event from server to client
    public class BicepTelemetryHandler : ExecuteCommandHandlerBase<BicepTelemetryEvent>
    {
        private readonly ITelemetryProvider TelemetryProvider;

        public BicepTelemetryHandler(ITelemetryProvider telemetryProvider)
        {
            TelemetryProvider = telemetryProvider;
        }

        public override Task<BicepTelemetryEvent> Handle(ExecuteCommandParams<BicepTelemetryEvent> request, CancellationToken cancellationToken)
        {
            JArray? arguments = request.Arguments;
            if (arguments is not null && arguments.Any() &&
                arguments[0] is JToken jToken &&
                jToken.ToObject<BicepTelemetryEvent>() is BicepTelemetryEvent telemetryEvent)
            {
                TelemetryProvider.PostEvent(telemetryEvent);
                return Task.FromResult(telemetryEvent);
            }

            return Task.FromResult(BicepTelemetryEvent.Create(string.Empty));
        }

        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            Commands = new Container<string>(TelemetryConstants.CommandName)
        };
    }
}
