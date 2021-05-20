// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.Telemetry;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepTelemetryHandler : ExecuteCommandHandler
    {
        private readonly ITelemetryProvider TelemetryProvider;

        public BicepTelemetryHandler(ITelemetryProvider telemetryProvider)
            : base(GetExecuteCommandRegistrationOptions())
        {
            TelemetryProvider = telemetryProvider;
        }

        private static ExecuteCommandRegistrationOptions GetExecuteCommandRegistrationOptions()
            => new ExecuteCommandRegistrationOptions()
            {
                Commands = new Container<string>("bicep.telemetry")
            };

        public override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            JArray? arguments = request.Arguments;
            if (arguments is not null && arguments.Any() &&
                arguments[0] is JToken jToken &&
                jToken.ToObject<TelemetryEvent>() is TelemetryEvent telemetryEvent)
            {
                TelemetryProvider.PostEvent(telemetryEvent);
            }

            return Unit.Task;
        }
    }
}
