// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.UnitTests.MiddleLayerProviders
{
    [TestClass]
    public class TelemetryCustomMessageTargetTests
    {
        [DataRow("{}")]
        [DataRow(@"{ ""a"": ""b""}")]
        [DataRow(@"{ ""a"": 1 }")]
        [DataRow(@"{ ""eventName"": """" }")]
        [DataRow(@"{ ""eventName"": ""   "" }")]
        [DataTestMethod]
        public void GetTelemetryEvent_WithInvalidInput_ShouldReturnNull(string input)
        {
            var jObject = JObject.Parse(input);
            var telemetryCustomMessageTarget = new TelemetryCustomMessageTarget(TelemetryService.DefaultSession);
            var result = telemetryCustomMessageTarget.GetTelemetryEvent(jObject);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetTelemetryEvent_WithEventNameAndProperties_ShouldReturnTelemetryEvent()
        {
            var input = JObject.Parse(@"{
  ""eventName"": ""diagnostics/editlinterrule"",
  ""properties"": {
    ""code"": ""adminusername-should-not-be-literal"",
    ""newConfigFile"": ""false"",
    ""newRuleAdded"": ""false"",
    ""error"": """",
    ""result"": ""succeeded""
  }
}");
            var telemetryCustomMessageTarget = new TelemetryCustomMessageTarget(TelemetryService.DefaultSession);
            var result = telemetryCustomMessageTarget.GetTelemetryEvent(input);

            result.Should().NotBeNull();
            result!.Name.Should().Be("vs/bicep/diagnostics/editlinterrule");
            result.Properties.Count.Should().Be(5);
            result.Properties["code"].Should().Be("adminusername-should-not-be-literal");
            result.Properties["newConfigFile"].Should().Be("false");
            result.Properties["newRuleAdded"].Should().Be("false");
            result.Properties["error"].Should().Be(string.Empty);
            result.Properties["result"].Should().Be("succeeded");
        }

        [TestMethod]
        public void GetTelemetryEvent_WithoutProperties_ShouldReturnTelemetryEvent()
        {
            var input = JObject.Parse(@"{
  ""eventName"": ""diagnostics/editlinterrule""
}");
            var telemetryCustomMessageTarget = new TelemetryCustomMessageTarget(TelemetryService.DefaultSession);
            var result = telemetryCustomMessageTarget.GetTelemetryEvent(input);

            result.Should().NotBeNull();
            result!.Name.Should().Be("vs/bicep/diagnostics/editlinterrule");
            result.Properties.Should().BeEmpty();
        }
    }
}
