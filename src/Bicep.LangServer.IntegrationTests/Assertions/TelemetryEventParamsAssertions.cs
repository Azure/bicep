// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class TelemetryEventParamsExtensions
    {
        public static TelemetryEventParamsAssertions Should(this TelemetryEventParams instance)
        {
            return new TelemetryEventParamsAssertions(instance);
        }
    }

    public class TelemetryEventParamsAssertions : ReferenceTypeAssertions<TelemetryEventParams, TelemetryEventParamsAssertions>
    {
        public TelemetryEventParamsAssertions(TelemetryEventParams instance)
            : base(instance)
        {
        }

        protected override string Identifier => "telemetry event";
    }
}
