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
        var session = CreateHelper(client);

        var snapshot = await session.GetSnapshot(new(), options, cts.Token);

        snapshot.Diagnostics.Should().Equal("Evaluation warning");
        snapshot.PredictedResources.Should().HaveCount(2);
        var storage = snapshot.GetArmResourceOfType("Microsoft.Storage/storageAccounts").Should().ContainSingle().Subject;
        storage.GetRequiredStringProperty("/apiVersion").Should().Be("2023-05-01");
        storage.Id.Should().BeOfType<ResourceGroupLevelResourceId>()
            .Which.SubscriptionId.Should().Be("00000000-0000-0000-0000-000000000001");
        storage.Name.Should().Be("storage");
        storage.GetRequiredStringProperty("/location").Should().Be("eastus");
        storage.GetRequiredStringProperty("/kind").Should().Be("StorageV2");
        storage.GetRequiredStringProperty("/managedBy").Should().Be("/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/owner");
        storage.GetRequiredBoolProperty("/properties/allowBlobPublicAccess").Should().BeFalse();
        storage.GetRequiredStringProperty("/sku/name").Should().Be("Standard_LRS");
        storage.GetRequiredStringProperty("/tags/environment").Should().Be("test");
        snapshot.PredictedResources[1].Should().BeOfType<SnapshotResource>()
            .Which.GetRequiredStringProperty("/identifiers/name").Should().Be("custom");
        snapshot.Outputs["storageId"].GetString().Should().Be(storage.GetRequiredStringProperty("/id"));
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
        var session = CreateHelper(client);

        var snapshot = await session.GetSnapshot(new(), new() { FilePath = "main.bicepparam" });
        var resource = snapshot.PredictedResources.Single();

        resource.GetRequiredStringProperty("/id").Should().Be("[resourceId('Microsoft.Storage/storageAccounts', 'storage')]");
        resource.GetRequiredStringProperty("/name").Should().Be("[parameters('storageName')]");
        resource.GetRequiredStringProperty("/location").Should().Be("[parameters('location')]");
        resource.GetRequiredStringProperty("/kind").Should().Be("[parameters('kind')]");
        resource.GetRequiredStringProperty("/managedBy").Should().Be("[parameters('owner')]");
        snapshot.GetArmResources().Should().BeEmpty();
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("not json")]
    [DataRow("""{"predictedResources":[],"diagnostics":[],"outputs":null}""")]
    [DataRow("""{"predictedResources":[],"diagnostics":[]}""")]
    [DataRow("""{"predictedResources":[],"diagnostics":null,"outputs":{}}""")]
    public async Task Malformed_snapshots_fail_instead_of_returning_empty_results(string json)
    {
        var client = CreateClient(json);
        var session = CreateHelper(client);

        var act = () => session.GetSnapshot(new(), new() { FilePath = "main.bicepparam" });

        await act.Should().ThrowAsync<JsonException>();
        client.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task Null_snapshot_fails_explicitly()
    {
        var client = CreateClient("null");
        var session = CreateHelper(client);

        var act = () => session.GetSnapshot(new(), new() { FilePath = "main.bicepparam" });

        await act.Should().ThrowAsync<InvalidDataException>().WithMessage("*null snapshot*");
        client.Verify(x => x.Dispose(), Times.Once);
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
        var session = CreateHelper(client);
        var act = () => session.GetSnapshot(new(), new() { FilePath = "main.bicepparam" }, cts.Token);

        (await act.Should().ThrowAsync<IOException>()).Which.Should().BeSameAs(failure);
        await cts.CancelAsync();
        (await act.Should().ThrowAsync<OperationCanceledException>()).Which.CancellationToken.Should().Be(cts.Token);
        client.Verify(x => x.Dispose(), Times.Exactly(2));
    }

    [TestMethod]
    public async Task Snapshot_uses_the_factory_for_each_configuration_and_releases_each_client()
    {
        using var cts = new CancellationTokenSource();
        var firstConfiguration = new BicepClientConfiguration { BicepVersion = "0.48.0" };
        var secondConfiguration = new BicepClientConfiguration { BicepVersion = "0.49.2" };
        var firstClient = CreateClient("""{"predictedResources":[],"diagnostics":[],"outputs":{"version":"first"}}""");
        var secondClient = CreateClient("""{"predictedResources":[],"diagnostics":[],"outputs":{"version":"second"}}""");
        var factory = new Mock<IBicepClientFactory>(MockBehavior.Strict);
        factory.Setup(x => x.Initialize(firstConfiguration, cts.Token)).ReturnsAsync(firstClient.Object);
        factory.Setup(x => x.Initialize(secondConfiguration, cts.Token)).ReturnsAsync(secondClient.Object);
        var session = new BicepTestHelper(factory.Object);

        var snapshots = await Task.WhenAll(
            session.GetSnapshot(firstConfiguration, new() { FilePath = "main.bicepparam" }, cts.Token),
            session.GetSnapshot(secondConfiguration, new() { FilePath = "main.bicepparam" }, cts.Token));

        snapshots[0].Outputs["version"].GetString().Should().Be("first");
        snapshots[1].Outputs["version"].GetString().Should().Be("second");
        factory.Verify(x => x.Initialize(firstConfiguration, cts.Token), Times.Once);
        factory.Verify(x => x.Initialize(secondConfiguration, cts.Token), Times.Once);
        firstClient.Verify(x => x.Dispose(), Times.Once);
        secondClient.Verify(x => x.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task Factory_failures_and_cancellation_are_propagated()
    {
        using var cts = new CancellationTokenSource();
        var factory = new Mock<IBicepClientFactory>(MockBehavior.Strict);
        var failure = new IOException("CLI initialization failed");
        factory.SetupSequence(x => x.Initialize(It.IsAny<BicepClientConfiguration>(), cts.Token))
            .ThrowsAsync(failure)
            .ThrowsAsync(new OperationCanceledException(cts.Token));
        var session = new BicepTestHelper(factory.Object);
        var act = () => session.GetSnapshot(new(), new() { FilePath = "main.bicepparam" }, cts.Token);

        (await act.Should().ThrowAsync<IOException>()).Which.Should().BeSameAs(failure);
        await cts.CancelAsync();
        (await act.Should().ThrowAsync<OperationCanceledException>()).Which.CancellationToken.Should().Be(cts.Token);
    }

    [TestMethod]
    [DataRow("""{"id":null}""")]
    [DataRow("""{"id":42}""")]
    [DataRow("""{"id":"invalid"}""")]
    [DataRow("""{"id":"[parameters('id')]"}""")]
    [DataRow("""{"identifiers":{"name":"custom"}}""")]
    [DataRow("{}")]
    public async Task Resources_without_parseable_ARM_ids_preserve_their_original_bodies(string json)
    {
        var client = CreateClient($$$"""{"predictedResources":[{{{json}}}],"diagnostics":[],"outputs":{}}""");
        var session = CreateHelper(client);

        var snapshot = await session.GetSnapshot(new(), new() { FilePath = "main.bicepparam" });

        var resource = snapshot.PredictedResources.Should().ContainSingle().Subject;
        resource.Should().BeOfType<SnapshotResource>();
        using var expected = JsonDocument.Parse(json);
        JsonElement.DeepEquals(resource.Body, expected.RootElement).Should().BeTrue();
        snapshot.GetArmResources().Should().BeEmpty();
    }

    private static BicepTestHelper CreateHelper(Mock<IBicepClient> client)
    {
        var factory = new Mock<IBicepClientFactory>(MockBehavior.Strict);
        factory.Setup(x => x.Initialize(It.IsAny<BicepClientConfiguration>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(client.Object);
        return new(factory.Object);
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
