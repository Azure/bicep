// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions.Collections;
using System.Collections.Generic;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class TelemetryEventParamsExtensions
    {
        public static TelemetryEventParamsAssertions Should(this TelemetryEventParams instance)
        {
            return new TelemetryEventParamsAssertions(instance);
        }

        public static TelemetryEventParamsCollectionAssertions Should(this IEnumerable<TelemetryEventParams> instance)
        {
            return new TelemetryEventParamsCollectionAssertions(instance);
        }
    }

    public class TelemetryEventParamsAssertions : ReferenceTypeAssertions<TelemetryEventParams, TelemetryEventParamsAssertions>
    {
        public TelemetryEventParamsAssertions(TelemetryEventParams instance)
            : base(instance)
        {
        }

        protected override string Identifier => "telemetry event";

        public AndConstraint<TelemetryEventParamsAssertions> HaveProperties(JObject properties, string because = "", params object[] becauseArgs)
        {
            (Subject.ExtensionData["properties"] as JToken)!.Should().DeepEqual(properties, because, becauseArgs);

            return new(this);
        }
    }

    public class TelemetryEventParamsCollectionAssertions : GenericCollectionAssertions<IEnumerable<TelemetryEventParams>, TelemetryEventParams>
    {
        public TelemetryEventParamsCollectionAssertions(IEnumerable<TelemetryEventParams> instance)
            : base(instance)
        {
        }

        protected override string Identifier => "telemetry";

        public AndConstraint<TelemetryEventParamsCollectionAssertions> ContainEvent(string eventName, JObject properties, string because = "", params object[] becauseArgs)
        {
            Subject.Should().ContainSingle(x => x.ExtensionData["eventName"] as string == eventName)
                .Which.Should().HaveProperties(properties);

            return new(this);
        }
    }
}
