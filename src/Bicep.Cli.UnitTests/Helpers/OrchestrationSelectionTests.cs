// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Helpers;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils.Snapshots;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.UnitTests.Helpers;

[TestClass]
public class OrchestrationSelectionTests
{
    [TestMethod]
    public async Task Labels_that_require_deployment_outputs_are_rejected()
    {
        var (template, parameters) = Compile();
        var json = JObject.Parse(template);
        json["resources"]!["apps"]!["properties"]!["body"]!["labels"]!["region"] =
            "[reference('foundation').outputs.value]";
        var select = async () => await OrchestrationSelection.Create(json.ToString(), parameters,
            OrchestrationSelection.ParseLabels(["kind=app"]), false, CancellationToken.None);
        await select.Should().ThrowAsync<CommandLineException>().WithMessage("*Labels for stack*must be resolvable*");
    }

    [TestMethod]
    public async Task Literal_brackets_in_label_values_are_supported()
    {
        var (template, parameters) = Compile();
        var json = JObject.Parse(template);
        json["resources"]!["apps"]!["properties"]!["body"]!["labels"]!["region"] = "[[westus]";
        var selection = await OrchestrationSelection.Create(json.ToString(), parameters,
            OrchestrationSelection.ParseLabels(["region=[westus]"]), false, CancellationToken.None);
        selection.Selected.Should().Contain("apps[0]");
    }

    [TestMethod]
    public async Task Selecting_a_collection_instance_includes_only_its_transitive_prerequisites()
    {
        var (template, parameters) = Compile();
        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=app", "region=westus"]), false, CancellationToken.None);

        selection.Selected.Should().BeEquivalentTo("apps[0]", "clusters[0]", "foundation");
        selection.Dependencies["apps[0]"].Should().BeEquivalentTo("clusters[0]");
        var filtered = await SnapshotHelper.GetSnapshot(ResourceScope.Orchestrator, selection.Template, parameters,
            null, null, null, null, null, null, [], true, CancellationToken.None);
        filtered.Diagnostics.Should().BeEmpty();
        filtered.PredictedResources.Select(resource => resource.GetProperty("symbolicName").GetString())
            .Should().BeEquivalentTo(selection.Selected);
        JObject.Parse(selection.Template)["resources"]!["apps"]!["properties"]!["__bicepOrchestrationDependencies"]
            .Should().BeNull();
    }

    [TestMethod]
    public async Task Whole_collection_dependencies_include_all_active_instances()
    {
        var (template, parameters) = Compile();
        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=global"]), false, CancellationToken.None);

        selection.Selected.Should().BeEquivalentTo("global", "apps[0]", "apps[1]", "clusters[0]", "clusters[1]", "foundation");
    }

    [TestMethod]
    public async Task Label_values_are_ored_and_keys_are_anded_with_case_sensitive_matching()
    {
        var (template, parameters) = Compile();
        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=app", "region=westus,eastus"]), false, CancellationToken.None);
        selection.Selected.Should().NotContain("global");
        selection.Selected.Should().Contain("apps[0]").And.Contain("apps[1]");

        var repeated = OrchestrationSelection.ParseLabels(["region=westus", "region=eastus"]);
        repeated["region"].Should().BeEquivalentTo("westus", "eastus");
        var noMatch = async () => await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["region=WestUS"]), false, CancellationToken.None);
        await noMatch.Should().ThrowAsync<CommandLineException>().WithMessage("*No stacks match*");
    }

    [TestMethod]
    public async Task Filtered_teardown_rejects_surviving_dependents_and_deletes_dependents_first()
    {
        var (template, parameters) = Compile();
        var unsafeSelection = async () => await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=cluster"]), true, CancellationToken.None);
        await unsafeSelection.Should().ThrowAsync<CommandLineException>().WithMessage("*unselected stack 'apps*");

        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=global"]), true, CancellationToken.None);
        selection.Selected.Should().BeEquivalentTo("global");

        var all = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels([]), true, CancellationToken.None);
        var batches = all.GetTeardownBatches();
        batches[0].Should().BeEquivalentTo("global", "unrelated");
        batches[1].Should().BeEquivalentTo("apps[0]", "apps[1]");
        batches[2].Should().BeEquivalentTo("clusters[0]", "clusters[1]");
        batches[3].Should().BeEquivalentTo("foundation");
    }

    [TestMethod]
    [DataRow(false, 2)]
    [DataRow(true, 0)]
    public async Task Conditions_and_empty_collections_do_not_add_inactive_stacks(bool enabled, int count)
    {
        var (template, parameters) = Compile(enabled, count);
        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=global"]), false, CancellationToken.None);
        selection.Selected.Should().BeEquivalentTo("global");
        selection.Dependencies["global"].Should().BeEmpty();
    }

    [TestMethod]
    [DataRow("region")]
    [DataRow("=westus")]
    [DataRow("region=")]
    [DataRow("region=westus,")]
    [DataRow("region=,westus")]
    public void Malformed_selectors_are_rejected(string selector)
    {
        var act = () => OrchestrationSelection.ParseLabels([selector]);
        act.Should().Throw<CommandLineException>().WithMessage("*Invalid label selector*");
    }

    private static (string Template, string Parameters) Compile(bool enabled = true, int count = 2)
    {
        var result = CompilationHelper.CompileParams(new ServiceBuilder()
            .WithEmptyAzResources().WithFeatureOverrides(new(OrchestrationEnabled: true)),
            ("main.bicep", $$"""
                targetScope = 'orchestrator'
                param enabled bool = {{enabled.ToString().ToLowerInvariant()}}
                var regions = ['westus', 'eastus']
                stack foundation './stack.bicepparam' = { inputs: {} }
                stack clusters './stack.bicepparam' = [for i in range(0, {{count}}): {
                  labels: {
                    kind: 'cluster'
                    region: regions[i]
                  }
                  inputs: {}
                  requires: [foundation]
                }]
                stack apps './stack.bicepparam' = [for i in range(0, {{count}}): if (enabled) {
                  labels: {
                    kind: 'app'
                    region: regions[i]
                  }
                  inputs: {}
                  requires: [clusters[i]]
                }]
                stack global './stack.bicepparam' = {
                  labels: { kind: 'global' }
                  inputs: {}
                  requires: [apps]
                }
                stack unrelated './stack.bicepparam' = {
                  labels: { kind: 'other' }
                  inputs: {}
                }
                """),
            ("parameters.bicepparam", "using './main.bicep'"),
            ("stack.bicep", "output value string = 'example'"),
            ("stack.bicepparam", "using './stack.bicep'"));
        result.Should().NotHaveAnyDiagnostics();
        var emitted = result.Compilation.Emitter.Parameters();
        return (emitted.Template!.Template!, emitted.Parameters!);
    }
}
