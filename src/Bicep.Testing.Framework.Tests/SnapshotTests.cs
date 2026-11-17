// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RpcClient;
using Azure.Deployments.Core.Definitions.Identifiers;
using FluentAssertions;

namespace Bicep.Testing.Framework.Tests;

[TestClass]
public class SnapshotTests
{
    private readonly IBicepClientFactory clientFactory = new BicepClientFactory();

    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [DataRow(BicepConnectionMode.NamedPipe)]
    [DataRow(BicepConnectionMode.Stdio)]
    public async Task Snapshot_evaluates_nested_module_parameters_and_deployment_context(BicepConnectionMode connectionMode)
    {
        var session = new BicepTestHelper(clientFactory);
        var snapshot = await session.SnapshotAsync(new()
        {
            FilePath = Fixture("main.bicepparam"),
            TenantId = "00000000-0000-0000-0000-000000000001",
            SubscriptionId = "00000000-0000-0000-0000-000000000002",
            ResourceGroup = "test-rg",
            Location = "eastus",
            DeploymentName = "offline-test",
        }, Configuration(connectionMode), TestContext.CancellationTokenSource.Token);

        snapshot.Diagnostics.Should().BeEmpty();
        var storage = snapshot.RequireResource("microsoft.storage/STORAGEACCOUNTS", "teststorage");
        snapshot.PredictedResources.Should().ContainSingle();
        storage.Properties.GetProperty("allowBlobPublicAccess").GetBoolean().Should().BeFalse();
        storage.Properties.GetProperty("minimumTlsVersion").GetString().Should().Be("TLS1_2");
        storage.Location.Should().Be("eastus");
        storage.TryParseId().Should().BeOfType<ResourceGroupLevelResourceId>()
            .Which.SubscriptionId.Should().Be("00000000-0000-0000-0000-000000000002");
        storage.TryParseId().Should().BeOfType<ResourceGroupLevelResourceId>()
            .Which.ResourceGroup.Should().Be("test-rg");
        storage.Sku!.Value.GetProperty("name").GetString().Should().Be("Standard_LRS");
        snapshot.Outputs["storageId"].GetString().Should().Be(storage.Id);
        snapshot.Outputs["tenantId"].GetString().Should().Be("00000000-0000-0000-0000-000000000001");
        snapshot.Outputs["deploymentName"].GetString().Should().Be("offline-test");
    }

    [TestMethod]
    public async Task Snapshot_accepts_omitted_context_and_reuses_the_session()
    {
        using var factory = new PooledBicepClientFactory();
        var session = new BicepTestHelper(factory);

        for (var i = 0; i < 2; i++)
        {
            var snapshot = await session.SnapshotAsync(new() { FilePath = Fixture("main.bicepparam") }, Configuration(), TestContext.CancellationTokenSource.Token);
            snapshot.Diagnostics.Should().BeEmpty();
            snapshot.ResourcesOfType("Microsoft.Storage/storageAccounts").Should().ContainSingle();
        }
    }

    [TestMethod]
    public async Task Snapshot_supports_management_group_context()
    {
        var session = new BicepTestHelper(clientFactory);
        var snapshot = await session.SnapshotAsync(new()
        {
            FilePath = Fixture("managementGroup.bicepparam"),
            ManagementGroupId = "platform",
            Location = "eastus",
        }, Configuration(), TestContext.CancellationTokenSource.Token);

        snapshot.Diagnostics.Should().BeEmpty();
        snapshot.Outputs["scopeId"].GetString().Should().Be("/providers/Microsoft.Management/managementGroups/platform");
    }

    private static BicepClientConfiguration Configuration(BicepConnectionMode connectionMode = BicepConnectionMode.NamedPipe)
        => new()
        {
            ExistingCliPath = Path.Combine(AppContext.BaseDirectory, OperatingSystem.IsWindows() ? "bicep.exe" : "bicep"),
            ConnectionMode = connectionMode,
        };

    private static string Fixture(string fileName) => Path.Combine(AppContext.BaseDirectory, "Files", fileName);
}
