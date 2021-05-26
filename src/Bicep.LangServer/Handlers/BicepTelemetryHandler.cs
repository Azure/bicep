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
    public class BicepTelemetryHandler : ExecuteCommandHandlerBase<BicepTelemetryEvent>
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
