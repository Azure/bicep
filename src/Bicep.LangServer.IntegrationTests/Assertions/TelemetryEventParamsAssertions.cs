// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core.UnitTests.Assertions;

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

        public AndConstraint<TelemetryEventParamsAssertions> HaveEventNameAndProperties(string eventName, JObject properties, string because = "", params object[] becauseArgs)
        {
            (Subject.ExtensionData["eventName"] as string)!.Should().Be(eventName, because, becauseArgs);
            (Subject.ExtensionData["properties"] as JToken)!.Should().DeepEqual(properties, because, becauseArgs);

            return new(this);
        }
    }
}
