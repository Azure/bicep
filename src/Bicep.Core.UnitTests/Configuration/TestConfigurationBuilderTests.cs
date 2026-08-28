// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Testing;
using Bicep.Testing.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class TestConfigurationBuilderTests
{
    [TestMethod]
    public void Build_applies_configuration_overrides()
    {
        var cloud = new CloudConfiguration(new(), new("https://management.example.com"), new("https://login.example.com"));
        var experimentalFeatures = ExperimentalFeaturesEnabled.AllDisabled with { Assertions = true };

        var configuration = TestConfigurationBuilder
            .Create()
            .WithAllAnalyzersDisabled()
            .WithAnalyzer("test-rule", DiagnosticLevel.Warning)
            .WithCloudConfiguration(cloud)
            .WithExtensions("""{"foo":"builtin:"}""")
            .WithImplicitExtensions("""["foo"]""")
            .WithExperimentalFeaturesEnabled(experimentalFeatures)
            .Build();

        configuration.Analyzers.GetValue("core.rules.test-rule.level", string.Empty).Should().Be("warning");
        configuration.Cloud.Should().BeSameAs(cloud);
        configuration.Extensions.TryGetExtensionSource("foo").IsSuccess(out var extension).Should().BeTrue();
        extension!.Value.Should().Be("builtin:");
        configuration.ImplicitExtensions.GetImplicitExtensionNames().Should().Equal("foo");
        configuration.ExperimentalFeaturesEnabled.Should().Be(experimentalFeatures);
    }

    [TestMethod]
    public void WithAllAnalyzers_enables_rules_that_are_off_by_default()
    {
        var configuration = TestConfigurationBuilder
            .Create()
            .WithAllAnalyzers()
            .WithAnalyzersDisabled(NoHardcodedOutputsRule.Code)
            .Build();

        configuration.Analyzers.GetValue($"core.rules.{NoHardcodedOutputsRule.Code}.level", string.Empty).Should().Be("off");
    }
}
