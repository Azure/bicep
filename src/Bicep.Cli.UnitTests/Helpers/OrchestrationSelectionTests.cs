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
    public async Task Looping_groups_select_inherited_labels_and_the_matching_sibling_instance()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            group apps = [for (region, i) in ['westus', 'eastus']: {
              labels: { region: region }
              stack backend './stack.bicepparam' = { inputs: { region: region } }
              stack frontend './stack.bicepparam' = {
                labels: { kind: 'frontend' }
                inputs: { region: region }
                requires: [backend]
              }
            }]
            stack global './stack.bicepparam' = {
              labels: { kind: 'global' }
              inputs: {}
              requires: [apps[1]::frontend]
            }
            """);

        var selected = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=frontend", "region=westus"]), false, CancellationToken.None);
        selected.Selected.Should().BeEquivalentTo("apps::frontend[0]", "apps::backend[0]");
        selected.Dependencies["apps::frontend[0]"].Should().BeEquivalentTo("apps::backend[0]");
        selected.Dependencies["apps::frontend[1]"].Should().BeEquivalentTo("apps::backend[1]");

        var global = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=global"]), false, CancellationToken.None);
        global.Selected.Should().BeEquivalentTo("global", "apps::frontend[1]", "apps::backend[1]");
    }

    [TestMethod]
    [DataRow("region")]
    [DataRow("Region")]
    public async Task Dynamic_group_label_conflicts_are_rejected_during_selection(string groupLabel)
    {
        var (template, parameters) = CompileGroups($$"""
            targetScope = 'orchestrator'
            param groupLabels object = { {{groupLabel}}: 'westus' }
            group apps = {
              labels: groupLabels
              stack backend './stack.bicepparam' = {
                labels: { region: 'eastus' }
                inputs: {}
              }
            }
            """);

        var select = async () => await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels([]), false, CancellationToken.None);
        await select.Should().ThrowAsync<Bicep.Core.Exceptions.BicepException>()
            .WithMessage("*region*group*apps::backend*");
    }

    [TestMethod]
    public async Task Dynamically_computed_group_label_keys_are_checked_for_conflicts()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            param labelName string = 'region'
            group apps = {
              labels: { '${labelName}': 'westus' }
              stack backend './stack.bicepparam' = {
                labels: { region: 'eastus' }
                inputs: {}
              }
            }
            """);

        var select = async () => await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels([]), false, CancellationToken.None);
        await select.Should().ThrowAsync<Bicep.Core.Exceptions.BicepException>()
            .WithMessage("*region*group*apps::backend*");
    }

    [TestMethod]
    public async Task Dynamic_stack_labels_that_differ_only_in_casing_are_rejected_before_merging()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            param stackLabels object = { region: 'westus', Region: 'eastus' }
            group apps = {
              labels: { kind: 'app' }
              stack backend './stack.bicepparam' = {
                labels: stackLabels
                inputs: {}
              }
            }
            """);

        var select = async () => await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels([]), false, CancellationToken.None);
        await select.Should().ThrowAsync<Bicep.Core.Exceptions.BicepException>()
            .WithMessage("*Region*differ*casing*apps::backend*");
    }

    [TestMethod]
    public async Task Dynamic_group_labels_that_differ_only_in_casing_remain_selectable()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            param groupLabels object = { region: 'westus', Region: 'eastus' }
            group apps = {
              labels: groupLabels
              stack backend './stack.bicepparam' = {
                labels: { kind: 'app' }
                inputs: {}
              }
            }
            """);

        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["region=westus", "Region=eastus"]), false, CancellationToken.None);

        selection.Selected.Should().BeEquivalentTo("apps::backend");
    }

    [TestMethod]
    public async Task Labels_can_use_spread_expressions_inside_a_group()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            var groupLabels = { environment: 'prod' }
            var stackLabels = { kind: 'app' }
            group apps = {
              labels: { ...groupLabels, region: 'westus' }
              stack backend './stack.bicepparam' = {
                labels: { ...stackLabels, stage: 'test' }
                inputs: {}
              }
            }
            """);

        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["environment=prod", "kind=app"]), false, CancellationToken.None);

        selection.Selected.Should().BeEquivalentTo("apps::backend");
    }

    [TestMethod]
    public async Task Cross_group_indexed_dependencies_follow_the_dependent_group_loop()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            var regions = ['westus', 'eastus']
            group foundation = [for region in regions: {
              stack backend './stack.bicepparam' = { inputs: {} }
            }]
            group apps = [for (region, i) in regions: {
              labels: { region: region }
              stack frontend './stack.bicepparam' = {
                inputs: {}
                requires: [foundation[i]::backend]
              }
            }]
            """);

        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["region=eastus"]), false, CancellationToken.None);
        selection.Selected.Should().BeEquivalentTo("apps::frontend[1]", "foundation::backend[1]");
        selection.Dependencies["apps::frontend[0]"].Should().BeEquivalentTo("foundation::backend[0]");
        selection.Dependencies["apps::frontend[1]"].Should().BeEquivalentTo("foundation::backend[1]");
    }

    [TestMethod]
    public async Task Group_and_stack_conditions_are_combined_and_empty_groups_emit_no_stacks()
    {
        var (template, parameters) = CompileGroups("""
            targetScope = 'orchestrator'
            group empty = {}
            group disabled = if (false) {
              stack backend './stack.bicepparam' = { inputs: {} }
            }
            group apps = [for region in ['westus', 'eastus']: if (region == 'westus') {
              labels: { region: region }
              stack backend './stack.bicepparam' = { inputs: {} }
              stack frontend './stack.bicepparam' = if (false) { inputs: {} }
            }]
            stack global './stack.bicepparam' = {
              labels: { kind: 'global' }
              inputs: {}
            }
            """);

        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels([]), false, CancellationToken.None);
        selection.Selected.Should().BeEquivalentTo("apps::backend[0]", "global");
    }

    [TestMethod]
    [DataRow("apps::backend[1]")]
    [DataRow("(apps::backend)[1]")]
    public async Task Non_looping_groups_support_stack_collections_and_indexed_dependencies(string dependency)
    {
        var (template, parameters) = CompileGroups($$"""
            targetScope = 'orchestrator'
            group apps = {
              labels: { kind: 'backend' }
              stack backend './stack.bicepparam' = [for i in range(0, 2): {
                labels: { instance: '${i}' }
                inputs: {}
              }]
            }
            stack global './stack.bicepparam' = {
              labels: { kind: 'global' }
              inputs: {}
              requires: [{{dependency}}]
            }
            """);

        var selection = await OrchestrationSelection.Create(template, parameters,
            OrchestrationSelection.ParseLabels(["kind=global"]), false, CancellationToken.None);
        selection.Selected.Should().BeEquivalentTo("global", "apps::backend[1]");
        selection.Dependencies["global"].Should().BeEquivalentTo("apps::backend[1]");
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

    private static (string Template, string Parameters) CompileGroups(string source)
    {
        var result = CompilationHelper.CompileParams(new ServiceBuilder()
            .WithEmptyAzResources().WithFeatureOverrides(new(OrchestrationEnabled: true)),
            ("main.bicep", source),
            ("parameters.bicepparam", "using './main.bicep'"),
            ("stack.bicep", "output value string = 'example'"),
            ("stack.bicepparam", "using './stack.bicep'"));
        result.Should().NotHaveAnyDiagnostics();
        var emitted = result.Compilation.Emitter.Parameters();
        return (emitted.Template!.Template!, emitted.Parameters!);
    }
}
