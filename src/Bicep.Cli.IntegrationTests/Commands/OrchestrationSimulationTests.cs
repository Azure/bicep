// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.AzureApi;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils.Snapshots;
using Bicep.Local.Deploy.Azure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests.Commands;

[TestClass]
public class OrchestrationSimulationTests : TestBase
{
    private const string Scope = "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/test";
    private const string StorageId = Scope + "/providers/Microsoft.Storage/storageAccounts/example";
    private const string StackId = Scope + "/providers/Microsoft.Resources/deploymentStacks/app";

    private Task<Bicep.Cli.UnitTests.CliResult> Run(string path, string state, string command, params string[] arguments)
    {
        var armClient = StrictMock.Of<IArmClientProvider>();
        var deployment = StrictMock.Of<IArmDeploymentProvider>();
        var processor = StrictMock.Of<IDeploymentProcessor>();
        var settings = CreateDefaultSettings() with
        {
            FeatureOverrides = CreateDefaultFeatureProviderOverrides() with { OrchestrationEnabled = true, DeployCommandsEnabled = true },
            Environment = TestEnvironment.Default.WithVariables((SimulatedArmDeploymentProvider.EnvironmentVariable, state)),
        };
        return Bicep(settings, services => services.AddSingleton(armClient.Object)
            .AddSingleton(deployment.Object).AddSingleton(processor.Object),
            TestContext.CancellationTokenSource.Token,
            ["orchestrate", command, path, .. arguments]);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public async Task Empty_simulation_state_path_fails_without_falling_back_to_Azure(string state)
    {
        var (path, _) = CreateFiles("delete");
        var result = await Run(path, state, "deploy");
        result.Should().Fail();
        result.Stderr.Should().Contain($"{SimulatedArmDeploymentProvider.EnvironmentVariable} must contain a JSON state file path.");
    }

    [TestMethod]
    public async Task What_if_reads_local_resources_and_formats_property_and_array_diffs()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'example'
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
              properties: {
                supportsHttpsTrafficOnly: true
                networkAcls: {
                  defaultAction: 'Deny'
                  ipRules: [{ value: '1.2.3.4' }, { value: '5.6.7.8' }]
                }
              }
            }
            """, Path.GetDirectoryName(path));
        var deployed = await Run(path, state, "deploy");
        deployed.Should().Succeed("stdout: {0}; stderr: {1}", deployed.Stdout, deployed.Stderr);
        var resources = ParseResources(await File.ReadAllTextAsync(state));
        resources[StorageId]!["properties"]!["supportsHttpsTrafficOnly"] = false;
        resources[StorageId]!["properties"]!["networkAcls"]!["ipRules"]![1]!["value"] = "9.10.11.12";
        resources[StorageId]!["sku"]!["name"] = "Standard_GRS";
        resources[StorageId]!["tags"] = new JObject { ["obsolete"] = "remove-me" };
        await File.WriteAllTextAsync(state, new JArray(resources.Properties().Select(resource => resource.Value)).ToString());
        var before = await File.ReadAllTextAsync(state);
        var result = await Run(path, state, "what-if");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var output = result.WithVisibleAnsi().Stdout;
        output.Should().Contain("~ Microsoft.Storage/storageAccounts/example")
            .And.Contain("properties.supportsHttpsTrafficOnly").And.Contain("false").And.Contain("true")
            .And.Contain("sku.name").And.Contain("Standard_GRS").And.Contain("Standard_LRS")
            .And.Contain("ipRules").And.Contain("9.10.11.12").And.Contain("5.6.7.8")
            .And.Contain("remove-me").And.NotContain("Simulated what-if").And.NotContain("\"changeType\"");
        (await File.ReadAllTextAsync(state)).Should().Be(before);
    }

    [TestMethod]
    public async Task Stack_parameter_errors_are_reported_without_an_internal_server_error()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        FileHelper.SaveResultFile(TestContext, "app.bicepparam", $$"""
            using './app.bicep' with {
              mode: 'stack'
              name: string(externalInput('unsupported.setting', 'name'))
              scope: '{{Scope}}'
              actionOnUnmanage: { resources: 'delete' }
              denySettings: { mode: 'none' }
            }
            """, Path.GetDirectoryName(path));
        var result = await Run(path, state, "deploy");
        result.Should().Fail();
        result.Stdout.Should().Contain("unsupported.setting").And.NotContain("internal server error");
        (await File.ReadAllTextAsync(state)).Should().Be("[]");
    }

    [TestMethod]
    public async Task Object_external_inputs_preserve_property_access()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "main.bicep", """
            targetScope = 'orchestrator'
            stack app './app.bicepparam' = {
              region: 'westus'
              deploy: 'always'
              inputs: { name: 'app', config: { name: 'example', enabled: false } }
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "app.bicepparam", $$"""
            using './app.bicep' with {
              mode: 'stack'
              name: string(externalInput('stack.setting', 'name'))
              scope: '{{Scope}}'
              actionOnUnmanage: { resources: 'delete' }
              denySettings: { mode: 'none' }
            }
            var config = externalInput('stack.setting', 'config')
            param name = config.name
            param enabled = config.enabled
            """, directory);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            param name string
            param enabled bool
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: name
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
              properties: { supportsHttpsTrafficOnly: enabled }
            }
            output enabled bool = enabled
            """, directory);
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var resources = ParseResources(await File.ReadAllTextAsync(state));
        resources[StorageId]?["properties"]?["supportsHttpsTrafficOnly"]?.Value<bool>().Should().BeFalse();
        resources[StackId]?["properties"]?["outputs"]?["enabled"]?["value"]?.Value<bool>().Should().BeFalse();
    }

    [TestMethod]
    public async Task Simulation_persists_resource_ids_and_outputs_and_what_if_is_read_only()
    {
        var (path, state) = CreateFiles("delete");
        var result = await Run(path, state, "deploy", "--format", "Json");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var resources = ParseResources(await File.ReadAllTextAsync(state));
        resources[StorageId]?["properties"]?["primaryEndpoints"].Should().BeNull();
        resources[StackId]?["properties"]?["outputs"]?["endpoint"]?["value"]?.Value<string>()
            .Should().Contain("reference(").And.Contain("primaryEndpoints.blob");
        resources[Scope + "/providers/Microsoft.Resources/deployments/child"].Should().BeNull();
        resources[Scope + "/providers/Microsoft.Resources/tags/default"]?["properties"]?["tags"]?["endpoint"]?.Value<string>()
            .Should().Contain("reference(").And.Contain("primaryEndpoints.blob");

        var stack = resources[StackId]!["properties"]!;
        var snapshot = await SnapshotHelper.GetSnapshot(
            ResourceScope.ResourceGroup,
            stack["template"]!.ToString(),
            new JObject
            {
                ["$schema"] = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                ["contentVersion"] = "1.0.0.0",
                ["parameters"] = stack["parameters"]!.DeepClone(),
            }.ToString(),
            null, null, "00000000-0000-0000-0000-000000000001", "test", "app", "westus",
            [], false, TestContext.CancellationTokenSource.Token);
        snapshot.Diagnostics.Should().BeEmpty();
        var predictions = snapshot.PredictedResources.Select(resource => JObject.Parse(resource.GetRawText())).ToArray();
        predictions.Should().HaveCount(2);
        foreach (var prediction in predictions)
        {
            JToken.DeepEquals(resources[prediction["id"]!.Value<string>()!], prediction).Should().BeTrue();
        }

        var previous = await File.ReadAllTextAsync(state);
        var whatIf = await Run(path, state, "what-if", "--label", "kind=app");
        whatIf.Should().Succeed("stdout: {0}; stderr: {1}", whatIf.Stdout, whatIf.Stderr);
        whatIf.Stdout.Should().Contain("= Microsoft.Storage/storageAccounts/example")
            .And.NotContain("Simulated what-if").And.NotContain("\"changeType\"");
        (await File.ReadAllTextAsync(state)).Should().Be(previous);

        var teardown = await Run(path, state, "teardown", "--label", "kind=app");
        teardown.Should().Succeed("stdout: {0}; stderr: {1}", teardown.Stdout, teardown.Stderr);
        var remaining = ParseResources(await File.ReadAllTextAsync(state));
        remaining[StorageId].Should().BeNull();
        remaining[StackId].Should().BeNull();
        remaining[Scope].Should().NotBeNull();
    }

    [TestMethod]
    public async Task What_if_evaluates_prerequisites_before_dependents_without_writing_state()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "main.bicep", """
            targetScope = 'orchestrator'
            stack app './app.bicepparam' = {
              region: 'westus'
              deploy: 'always'
              labels: { kind: 'app' }
              inputs: { name: 'app' }
              requires: [foundation]
            }
            stack foundation './foundation.bicepparam' = {
              region: 'westus'
              deploy: 'always'
              inputs: {}
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "foundation.bicepparam", $$"""
            using './foundation.bicep' with {
              mode: 'stack'
              name: 'foundation'
              scope: '{{Scope}}'
              actionOnUnmanage: { resources: 'delete' }
              denySettings: { mode: 'none' }
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "foundation.bicep", """
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'example'
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
              properties: { supportsHttpsTrafficOnly: true }
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: 'example'
            }
            resource tags 'Microsoft.Resources/tags@2021-04-01' = {
              name: 'default'
              properties: {
                tags: { value: string(storage.properties.supportsHttpsTrafficOnly) }
              }
            }
            """, directory);
        var before = await File.ReadAllTextAsync(state);
        var result = await Run(path, state, "what-if", "--label", "kind=app");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        result.Stdout.IndexOf("Microsoft.Storage/storageAccounts/example", StringComparison.Ordinal).Should()
            .BeLessThan(result.Stdout.IndexOf("Microsoft.Resources/tags/default", StringComparison.Ordinal));
        result.Stdout.Should().Contain("+ Microsoft.Storage/storageAccounts/example").And.Contain($"Scope: {Scope}");
        (await File.ReadAllTextAsync(state)).Should().Be(before);
        var deployed = await Run(path, state, "deploy", "--label", "kind=app");
        deployed.Should().Succeed("stdout: {0}; stderr: {1}", deployed.Stdout, deployed.Stderr);
        ParseResources(await File.ReadAllTextAsync(state))[StackId].Should().NotBeNull();
    }

    [TestMethod]
    public async Task Detach_keeps_resources_but_removes_the_stack()
    {
        var (path, state) = CreateFiles("detach");
        var deployment = await Run(path, state, "deploy");
        deployment.Should().Succeed("stdout: {0}; stderr: {1}", deployment.Stdout, deployment.Stderr);
        FileHelper.SaveResultFile(TestContext, "app.bicep", "output value string = 'empty'", Path.GetDirectoryName(path));
        var before = await File.ReadAllTextAsync(state);
        var whatIf = await Run(path, state, "what-if");
        whatIf.Should().Succeed("stdout: {0}; stderr: {1}", whatIf.Stdout, whatIf.Stderr);
        whatIf.Stdout.Should().NotContain("Microsoft.Storage/storageAccounts/example").And.NotContain("Delete");
        (await File.ReadAllTextAsync(state)).Should().Be(before);
        var teardown = await Run(path, state, "teardown");
        teardown.Should().Succeed("stdout: {0}; stderr: {1}", teardown.Stdout, teardown.Stderr);
        var remaining = ParseResources(await File.ReadAllTextAsync(state));
        remaining[StorageId].Should().NotBeNull();
        remaining[StackId].Should().BeNull();
    }

    [TestMethod]
    public async Task Missing_generated_properties_are_persisted_as_unresolved_expressions()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var remaining = ParseResources(await File.ReadAllTextAsync(state));
        remaining[StackId]?["properties"]?["outputs"]?["endpoint"]?["value"]?.Value<string>()
            .Should().Contain("reference(").And.Contain("primaryEndpoints.blob");
        remaining[StorageId].Should().NotBeNull();
    }

    [TestMethod]
    public async Task Snapshot_expands_nested_scopes_parameters_and_conditions()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            module child './child.bicep' = {
              name: 'child'
              scope: resourceGroup('other')
              params: { name: 'nested' }
            }
            resource skipped 'Microsoft.Resources/tags@2021-04-01' = if (false) {
              name: 'default'
              properties: { tags: {} }
            }
            output id string = child.outputs.id
            """, directory);
        FileHelper.SaveResultFile(TestContext, "child.bicep", """
            param name string
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: name
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            output id string = storage.id
            """, directory);
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var resources = JArray.Parse(await File.ReadAllTextAsync(state));
        var nestedId = "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/other/providers/Microsoft.Storage/storageAccounts/nested";
        resources.OfType<JObject>().Select(resource => resource["id"]?.Value<string>())
            .Should().BeEquivalentTo(Scope, StackId, nestedId);
        ParseResources(resources.ToString())[StackId]?["properties"]?["outputs"]?["id"]?["value"]?.Value<string>()
            .Should().Be(nestedId);
    }

    [TestMethod]
    public async Task Unresolved_resource_ids_fail_without_committing_stack_changes()
    {
        var (path, state) = CreateFiles("delete", seed: false);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: string(resourceGroup().tags.name)
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }
            """, Path.GetDirectoryName(path));
        var result = await Run(path, state, "deploy");
        result.Should().Fail();
        (result.Stdout + result.Stderr).Should().Contain("Simulation");
        var resources = JArray.Parse(await File.ReadAllTextAsync(state));
        resources.OfType<JObject>().Select(resource => resource["id"]?.Value<string>())
            .Should().BeEquivalentTo(Scope);
    }

    [TestMethod]
    public async Task Corrupt_state_fails_without_overwriting_it()
    {
        var (path, state) = CreateFiles("delete");
        await File.WriteAllTextAsync(state, "{broken");
        var result = await Run(path, state, "deploy");
        result.Should().Fail();
        (await File.ReadAllTextAsync(state)).Should().Be("{broken");
    }

    [TestMethod]
    public async Task Existing_resources_and_list_responses_remain_unresolved_even_with_seeded_state()
    {
        var (path, state) = CreateFiles("delete");
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
              name: 'example'
            }
            resource tags 'Microsoft.Resources/tags@2021-04-01' = {
              name: 'default'
              properties: {
                tags: {
                  endpoint: storage.properties.primaryEndpoints.blob
                  key: storage.listKeys().keys[0].value
                }
              }
            }
            """, directory);
        var resources = ParseResources(await File.ReadAllTextAsync(state));
        resources[StorageId]!["simulation"] = JObject.Parse("""{"listResponses":{"listKeys":{"keys":[{"value":"simulated-key"}]}}}""");
        await File.WriteAllTextAsync(state, new JArray(resources.Properties().Select(resource => resource.Value)).ToString());
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var deployed = ParseResources(await File.ReadAllTextAsync(state));
        deployed[Scope + "/providers/Microsoft.Resources/tags/default"]?["properties"]?["tags"]?["key"]?.Value<string>()
            .Should().Contain("listKeys(");
        deployed[Scope + "/providers/Microsoft.Resources/tags/default"]?["properties"]?["tags"]?["endpoint"]?.Value<string>()
            .Should().Contain("reference(").And.Contain("primaryEndpoints.blob");
        var teardown = await Run(path, state, "teardown");
        teardown.Should().Succeed();
        ParseResources(await File.ReadAllTextAsync(state))[StorageId].Should().NotBeNull();
    }

    [TestMethod]
    public async Task Existing_collections_predict_references_by_resource_id()
    {
        var (path, state) = CreateFiles("delete");
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            var names = ['example', 'another']
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = [for name in names: {
              name: name
            }]
            resource tags 'Microsoft.Resources/tags@2021-04-01' = {
              name: 'default'
              properties: {
                tags: {
                  endpoint: storage[1].properties.primaryEndpoints.blob
                }
              }
            }
            """, directory);
        var resources = ParseResources(await File.ReadAllTextAsync(state));
        var second = (JObject)resources[StorageId]!.DeepClone();
        var id = Scope + "/providers/Microsoft.Storage/storageAccounts/another";
        second["id"] = id;
        second["name"] = "another";
        second["properties"]!["primaryEndpoints"]!["blob"] = "https://another.example/";
        resources[id] = second;
        await File.WriteAllTextAsync(state, new JArray(resources.Properties().Select(resource => resource.Value)).ToString());
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        ParseResources(await File.ReadAllTextAsync(state))[Scope + "/providers/Microsoft.Resources/tags/default"]?["properties"]?["tags"]?["endpoint"]?.Value<string>()
            .Should().Contain(id).And.Contain("primaryEndpoints.blob");
    }

    [TestMethod]
    public async Task Loop_instances_are_persisted_and_removed_from_stack_management()
    {
        var (path, state) = CreateFiles("delete");
        var directory = Path.GetDirectoryName(path);
        string Template(int count) => $$"""
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = [for i in range(0, {{count}}): {
              name: 'example${i}'
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
            }]
            """;
        FileHelper.SaveResultFile(TestContext, "app.bicep", Template(2), directory);
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var deployed = ParseResources(await File.ReadAllTextAsync(state));
        deployed[Scope + "/providers/Microsoft.Storage/storageAccounts/example0"].Should().NotBeNull();
        deployed[Scope + "/providers/Microsoft.Storage/storageAccounts/example1"].Should().NotBeNull();
        FileHelper.SaveResultFile(TestContext, "app.bicep", Template(1), directory);
        var beforeWhatIf = await File.ReadAllTextAsync(state);
        var whatIf = await Run(path, state, "what-if");
        whatIf.Should().Succeed("stdout: {0}; stderr: {1}", whatIf.Stdout, whatIf.Stderr);
        whatIf.Stdout.Should().Contain("- Microsoft.Storage/storageAccounts/example1");
        (await File.ReadAllTextAsync(state)).Should().Be(beforeWhatIf);
        var updated = await Run(path, state, "deploy");
        updated.Should().Succeed("stdout: {0}; stderr: {1}", updated.Stdout, updated.Stderr);
        ParseResources(await File.ReadAllTextAsync(state))[Scope + "/providers/Microsoft.Storage/storageAccounts/example1"].Should().BeNull();
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("[{}]")]
    [DataRow("[1]")]
    [DataRow("[{\"id\":1}]")]
    [DataRow("[{\"id\":\"relative\"}]")]
    [DataRow("[{\"id\":\"/resource\"},{\"id\":\"/RESOURCE\"}]")]
    public async Task Invalid_array_state_fails_without_overwriting_it(string contents)
    {
        var (path, state) = CreateFiles("delete");
        await File.WriteAllTextAsync(state, contents);
        var result = await Run(path, state, "deploy");
        result.Should().Fail();
        (await File.ReadAllTextAsync(state)).Should().Be(contents);
    }

    [TestMethod]
    public async Task Missing_state_file_is_created_as_a_flat_array()
    {
        var (path, state) = CreateFiles("delete");
        File.Delete(state);
        FileHelper.SaveResultFile(TestContext, "app.bicep", "output value string = 'example'", Path.GetDirectoryName(path));
        var result = await Run(path, state, "deploy");
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        var resources = JArray.Parse(await File.ReadAllTextAsync(state));
        resources.Count.Should().Be(2);
        resources.OfType<JObject>().Select(resource => resource["id"]?.Value<string>())
            .Should().BeEquivalentTo(Scope, StackId);
    }

    [TestMethod]
    public async Task Concurrent_invocations_cannot_open_the_same_state()
    {
        var (_, state) = CreateFiles("delete");
        await using var first = new SimulatedArmDeploymentProvider(state);
        var open = () => new SimulatedArmDeploymentProvider(state);
        open.Should().Throw<IOException>();
    }

    private (string Path, string State) CreateFiles(string action, bool seed = true)
    {
        var path = FileHelper.SaveResultFile(TestContext, "main.bicepparam", "using './main.bicep'");
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "main.bicep", """
            targetScope = 'orchestrator'
            stack app './app.bicepparam' = {
              region: 'westus'
              deploy: 'always'
              labels: { kind: 'app' }
              inputs: { name: 'app' }
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "app.bicepparam", $$"""
            using './app.bicep' with {
              mode: 'stack'
              name: string(externalInput('stack.setting', 'name'))
              scope: '{{Scope}}'
              actionOnUnmanage: { resources: '{{action}}' }
              denySettings: { mode: 'none' }
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "app.bicep", """
            resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
              name: 'example'
              location: 'westus'
              kind: 'StorageV2'
              sku: { name: 'Standard_LRS' }
              properties: { supportsHttpsTrafficOnly: true }
            }
            module child './child.bicep' = {
              name: 'child'
              params: { endpoint: storage.properties.primaryEndpoints.blob }
            }
            resource tags 'Microsoft.Resources/tags@2021-04-01' = {
              name: 'default'
              properties: { tags: { endpoint: child.outputs.endpoint } }
            }
            output endpoint string = child.outputs.endpoint
            """, directory);
        FileHelper.SaveResultFile(TestContext, "child.bicep", """
            param endpoint string
            output endpoint string = endpoint
            """, directory);
        var state = FileHelper.SaveResultFile(TestContext, "state.json", seed ? new JArray
        {
            new JObject
            {
                ["id"] = StorageId,
                ["type"] = "Microsoft.Storage/storageAccounts",
                ["name"] = "example",
                ["properties"] = new JObject
                {
                    ["primaryEndpoints"] = new JObject { ["blob"] = "https://seeded.example/" },
                },
            },
        }.ToString() : "[]", directory);
        return (path, state);
    }

    private static JObject ParseResources(string json) =>
        new(JArray.Parse(json).OfType<JObject>().Select(resource => new JProperty(resource["id"]!.Value<string>()!, resource)));
}
