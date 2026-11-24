// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Azure.Deployments.Core.Definitions;
using Bicep.Core.Configuration;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Azure;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using static Bicep.Local.Deploy.Extensibility.NestedDeploymentExtension;

namespace Bicep.Orchestration.Local.UnitTests;

[TestClass]
public class ProgressProviderTests
{
    private static readonly DeploymentLocator Locator = new("", null, Guid.Empty.ToString(), "test", "app");

    [TestMethod]
    [DataRow("Failed", "Skipped", "Failed")]
    [DataRow("Canceled", "Canceled", "Canceled")]
    public void Terminal_operations_do_not_leave_stacks_queued_or_running(string terminal, string queued, string running)
    {
        var operation = new OrchestrationService.Operation("operation", "plan", "JSON SIMULATION", "host", 0, []);
        operation.Add("waiting", "Queued");
        operation.Add("running", "Running");
        operation.Add("completed", "Succeeded");
        operation.Add("failed", "Failed");

        operation.Complete(terminal);

        var result = operation.View();
        result.Status.Should().Be(terminal);
        result.Events.Last(e => e.Stack == "waiting").Status.Should().Be(queued);
        result.Events.Last(e => e.Stack == "running").Status.Should().Be(running);
        result.Events.Last(e => e.Stack == "completed").Status.Should().Be("Succeeded");
        result.Events.Last(e => e.Stack == "failed").Status.Should().Be("Failed");
    }

    [TestMethod]
    public async Task Simulation_waits_five_to_ten_seconds_before_persisting_a_stack()
    {
        var configuration = Mock.Of<IBicepConfiguration>();
        var config = new StacksConfig(null, null, null);
        var inner = new Mock<IArmDeploymentProvider>(MockBehavior.Strict);
        var timer = new Stopwatch();
        TimeSpan? persistedAt = null;
        var expectedConfig = config with { Version = "2.0.0" };
        inner.Setup(provider => provider.StartDeploymentStack(configuration, Locator, "template", "parameters", expectedConfig, It.IsAny<CancellationToken>()))
            .Callback(() => persistedAt = timer.Elapsed)
            .Returns(Task.CompletedTask);
        var progress = new List<string>();
        var provider = new ProgressProvider(inner.Object, "2.0.0", (_, status) => { progress.Add(status); return Task.CompletedTask; });

        timer.Start();
        var deployment = provider.StartDeploymentStack(configuration, Locator, "template", "parameters", config, CancellationToken.None);
        progress.Should().Equal("Running");
        persistedAt.Should().BeNull();
        await deployment;

        persistedAt.Should().NotBeNull();
        persistedAt.GetValueOrDefault().Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(5000))
            .And.BeLessThan(TimeSpan.FromSeconds(11), "the maximum delay is 10 seconds, with scheduling tolerance");
        inner.VerifyAll();
    }

    [TestMethod]
    public async Task Canceling_the_delay_does_not_persist_a_stack()
    {
        var inner = new Mock<IArmDeploymentProvider>(MockBehavior.Strict);
        var progress = new List<string>();
        var provider = new ProgressProvider(inner.Object, "2.0.0", (_, status) => { progress.Add(status); return Task.CompletedTask; });
        using var cancellation = new CancellationTokenSource();

        await FluentActions.Awaiting(async () =>
        {
            var deployment = provider.StartDeploymentStack(Mock.Of<IBicepConfiguration>(), Locator, "", "", new(null, null, null), cancellation.Token);
            cancellation.Cancel();
            await deployment;
        }).Should().ThrowAsync<OperationCanceledException>();

        progress.Should().Equal("Running", "Canceled");
        inner.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task Polling_failure_is_reported_as_a_failed_stack()
    {
        var configuration = Mock.Of<IBicepConfiguration>();
        var inner = new Mock<IArmDeploymentProvider>(MockBehavior.Strict);
        inner.Setup(provider => provider.CheckDeploymentStack(configuration, Locator, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Provider unavailable."));
        var progress = new List<string>();
        var provider = new ProgressProvider(inner.Object, "2.0.0", (_, status) => { progress.Add(status); return Task.CompletedTask; });

        await FluentActions.Awaiting(() => provider.CheckDeploymentStack(configuration, Locator, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>();

        progress.Should().Equal("Failed");
    }

    [TestMethod]
    public void Next_version_uses_all_deployed_stacks_and_numeric_order()
    {
        var state = JArray.Parse("""
            [
              { "type": "Microsoft.Resources/deploymentStacks", "tags": { "version": "1.9.99" } },
              { "type": "microsoft.resources/deploymentstacks", "tags": { "version": "1.10.2" } },
              { "type": "Microsoft.Resources/deploymentStacks" },
              { "type": "other", "tags": { "version": "99.0.0" } }
            ]
            """);

        SimulationVersion.Next(state).Should().Be("1.10.3");
        SimulationVersion.Next(new JArray()).Should().Be("0.0.1");
    }

    [TestMethod]
    public void Next_version_rejects_non_numeric_deployed_versions()
    {
        var state = JArray.Parse("""
            [{ "type": "Microsoft.Resources/deploymentStacks", "tags": { "version": "preview" } }]
            """);

        FluentActions.Invoking(() => SimulationVersion.Next(state))
            .Should().Throw<OrchestrationRequestException>().WithMessage("*non-numeric version*");
    }
}
