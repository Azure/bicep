// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.RpcClient;
using Bicep.RpcClient.Models;
using FluentAssertions;
using Moq;

namespace Bicep.Testing.Framework.Tests;

[TestClass]
public class BicepTestHelperTests
{
    [TestMethod]
    public async Task Snapshot_forwards_inputs_and_preserves_resources_outputs_and_diagnostics()
    {
        using var cts = new CancellationTokenSource();
        var options = new SnapshotOptions
        {
            FilePath = Path.Combine("infra", "main.bicepparam"),
            TenantId = "tenant",
            ManagementGroupId = "management-group",
            SubscriptionId = "subscription",
            ResourceGroup = "resource-group",
            Location = "eastus",
            DeploymentName = "deployment",
            ConfigPath = Path.Combine("config", "bicepconfig.json"),
            ExternalInputs = [new("test.input", JsonValue.Create("config"), JsonValue.Create("value"))],
        };
        var client = new Mock<IBicepClient>(MockBehavior.Strict);
        client.Setup(x => x.GetVersion(cts.Token)).ReturnsAsync("0.49.2+abcdef");
        client.Setup(x => x.GetSnapshot(It.Is<GetSnapshotRequest>(request =>
            request.Path == Path.GetFullPath(options.FilePath) &&
            request.ConfigPath == Path.GetFullPath(options.ConfigPath) &&
            request.Metadata == new GetSnapshotRequest.MetadataDefinition("tenant", "management-group", "subscription", "resource-group", "eastus", "deployment") &&
            request.ExternalInputs == options.ExternalInputs), cts.Token))
            .ReturnsAsync(new GetSnapshotResponse("""
                {
                  "predictedResources": [
                    {
                      "id": "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/rg/providers/Microsoft.Storage/storageAccounts/storage",
                      "name": "storage",
                      "type": "Microsoft.Storage/storageAccounts",
                      "apiVersion": "2023-05-01",
                      "location": "eastus",
                      "kind": "StorageV2",
                      "managedBy": "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/owner",
                      "properties": { "allowBlobPublicAccess": false },
                      "sku": { "name": "Standard_LRS" },
                      "tags": { "environment": "test" }
                    },
                    {
                      "type": "Custom.Resource",
                      "identifiers": { "name": "custom" },
                      "properties": {}
                    }
                  ],
                  "diagnostics": ["Evaluation warning"],
                  "outputs": { "storageId": "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/rg/providers/Microsoft.Storage/storageAccounts/storage", "count": 2, "optional": null }
                }
                """));
        client.Setup(x => x.Dispose());
        using var session = new BicepTestHelper(client.Object);

        var snapshot = await session.SnapshotAsync(options, cts.Token);

        snapshot.Diagnostics.Should().Equal("Evaluation warning");
        snapshot.PredictedResources.Should().HaveCount(2);
        var storage = snapshot.RequireResource("/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/rg/providers/Microsoft.Storage/storageAccounts/storage");
        storage.ApiVersion.Should().Be("2023-05-01");
        storage.TryParseId().Should().BeOfType<ResourceGroupLevelResourceId>()
            .Which.SubscriptionId.Should().Be("00000000-0000-0000-0000-000000000001");
        storage.TryParseId()!.FormatName().Should().Be("storage");
        storage.Location.Should().Be("eastus");
        storage.Kind.Should().Be("StorageV2");
        storage.ManagedBy.Should().Be("/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/owner");
        storage.Properties.GetProperty("allowBlobPublicAccess").GetBoolean().Should().BeFalse();
        storage.Sku!.Value.GetProperty("name").GetString().Should().Be("Standard_LRS");
        storage.Tags!.Value.GetProperty("environment").GetString().Should().Be("test");
        snapshot.PredictedResources[1].Id.Should().BeNull();
        snapshot.PredictedResources[1].TryParseId().Should().BeNull();
        snapshot.PredictedResources[1].AdditionalProperties!["identifiers"].GetProperty("name").GetString().Should().Be("custom");
        snapshot.Outputs["storageId"].GetString().Should().Be(storage.Id);
        snapshot.Outputs["count"].GetInt32().Should().Be(2);
        snapshot.Outputs["optional"].ValueKind.Should().Be(JsonValueKind.Null);
        client.Verify(x => x.GetSnapshot(It.IsAny<GetSnapshotRequest>(), cts.Token), Times.Once);
    }

    [TestMethod]
    public async Task Unresolved_resource_values_expose_ARM_expressions()
    {
        var client = CreateClient("""
            {
              "predictedResources": [
                {
                  "id": "[resourceId('Microsoft.Storage/storageAccounts', 'storage')]",
                  "name": "[parameters('storageName')]",
                  "type": "Microsoft.Storage/storageAccounts",
                  "apiVersion": "2023-05-01",
                  "location": "[parameters('location')]",
                  "kind": "[parameters('kind')]",
                  "managedBy": "[parameters('owner')]",
                  "properties": {}
                }
              ],
              "diagnostics": [],
              "outputs": {}
            }
            """);
        using var session = new BicepTestHelper(client.Object);

        var snapshot = await session.SnapshotAsync(new() { FilePath = "main.bicepparam" });
        var resource = snapshot.PredictedResources.Single();

        resource.Id.Should().Be("[resourceId('Microsoft.Storage/storageAccounts', 'storage')]");
        resource.Name.Should().Be("[parameters('storageName')]");
        resource.Location.Should().Be("[parameters('location')]");
        resource.Kind.Should().Be("[parameters('kind')]");
        resource.ManagedBy.Should().Be("[parameters('owner')]");
        resource.TryParseId().Should().BeNull();
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("not json")]
    [DataRow("""{"predictedResources":[],"diagnostics":[],"outputs":null}""")]
    [DataRow("""{"predictedResources":[{}],"diagnostics":[],"outputs":{}}""")]
    public async Task Malformed_snapshots_fail_instead_of_returning_empty_results(string json)
    {
        var client = CreateClient(json);
        using var session = new BicepTestHelper(client.Object);

        var act = () => session.SnapshotAsync(new() { FilePath = "main.bicepparam" });

        await act.Should().ThrowAsync<JsonException>();
    }

    [TestMethod]
    public async Task Null_snapshot_fails_explicitly()
    {
        var client = CreateClient("null");
        using var session = new BicepTestHelper(client.Object);

        var act = () => session.SnapshotAsync(new() { FilePath = "main.bicepparam" });

        await act.Should().ThrowAsync<InvalidDataException>().WithMessage("*null snapshot*");
    }

    [TestMethod]
    public async Task Rpc_failures_and_cancellation_are_propagated()
    {
        using var cts = new CancellationTokenSource();
        var client = new Mock<IBicepClient>(MockBehavior.Strict);
        var failure = new IOException("RPC connection failed");
        client.SetupSequence(x => x.GetSnapshot(It.IsAny<GetSnapshotRequest>(), cts.Token))
            .ThrowsAsync(failure)
            .ThrowsAsync(new OperationCanceledException(cts.Token));
        client.Setup(x => x.Dispose());
        using var session = new BicepTestHelper(client.Object);
        var act = () => session.SnapshotAsync(new() { FilePath = "main.bicepparam" }, cts.Token);

        (await act.Should().ThrowAsync<IOException>()).Which.Should().BeSameAs(failure);
        await cts.CancelAsync();
        (await act.Should().ThrowAsync<OperationCanceledException>()).Which.CancellationToken.Should().Be(cts.Token);
    }

    [TestMethod]
    [DataRow("0.46.1")]
    [DataRow("unknown")]
    public async Task Management_group_context_is_not_silently_ignored_by_older_CLIs(string version)
    {
        var client = new Mock<IBicepClient>(MockBehavior.Strict);
        client.Setup(x => x.GetVersion(default)).ReturnsAsync(version);
        client.Setup(x => x.Dispose());
        using var session = new BicepTestHelper(client.Object);
        var act = () => session.SnapshotAsync(new() { FilePath = "main.bicepparam", ManagementGroupId = "platform" });

        await act.Should().ThrowAsync<NotSupportedException>().WithMessage("*0.47.16*");
        client.Verify(x => x.GetSnapshot(It.IsAny<GetSnapshotRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Disposal_releases_the_client_once_and_prevents_further_requests()
    {
        var client = new Mock<IBicepClient>(MockBehavior.Strict);
        client.Setup(x => x.Dispose());
        var session = new BicepTestHelper(client.Object);

        await session.DisposeAsync();
        session.Dispose();
        var act = () => session.SnapshotAsync(new() { FilePath = "main.bicepparam" });

        await act.Should().ThrowAsync<ObjectDisposedException>();
        client.Verify(x => x.Dispose(), Times.Once);
        client.VerifyNoOtherCalls();
    }

    private static Mock<IBicepClient> CreateClient(string json)
    {
        var client = new Mock<IBicepClient>(MockBehavior.Strict);
        client.Setup(x => x.GetSnapshot(It.IsAny<GetSnapshotRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetSnapshotResponse(json));
        client.Setup(x => x.Dispose());
        return client;
    }
}
