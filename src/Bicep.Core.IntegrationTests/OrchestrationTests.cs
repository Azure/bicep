// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils.Snapshots;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class OrchestrationTests
{
    private static ServiceBuilder Services => new ServiceBuilder()
        .WithEmptyAzResources()
        .WithFeatureOverrides(new(OrchestrationEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    [DataRow("{ region: 'westus' }", true)]
    [DataRow("{ region: 1 }", false)]
    [DataRow("{ region: true }", false)]
    [DataRow("{ region: ['westus'] }", false)]
    [DataRow("{ region: { name: 'westus' } }", false)]
    [DataRow("{ region: null }", false)]
    [DataRow("'westus'", false)]
    public void Stack_labels_require_string_values(string labels, bool valid)
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", $$"""
                targetScope = 'orchestrator'
                stack example './stack.bicepparam' = {
                  labels: {{labels}}
                  inputs: {}
                }
                """),
            ("stack.bicepparam", "using './stack.bicep'"),
            ("stack.bicep", "output value string = 'example'"));
        if (valid)
        {
            result.Should().NotHaveAnyDiagnostics();
        }
        else
        {
            result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
        }
    }

    [TestMethod]
    public void Groups_emit_nested_stacks_with_inherited_labels_and_qualified_dependencies()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = {
              labels: {
                region: 'westus'
                stamp: '0'
              }
              stack backendApp 'stack.bicepparam' = {}
              stack frontendApp 'stack.bicepparam' = {
                labels: { kind: 'frontend' }
                requires: [backendApp]
              }
            }
            stack global './stack.bicepparam' = {
              requires: [apps::frontendApp]
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
        var resources = result.Template!["resources"]!;
        resources["apps::backendApp"]!["properties"]!["body"]!["labels"]!["region"]!.Value<string>().Should().Be("westus");
        resources["apps::frontendApp"]!["dependsOn"]!.Values<string>().Should().Equal("apps::backendApp");
        resources["global"]!["dependsOn"]!.Values<string>().Should().Equal("apps::frontendApp");
        resources["apps::frontendApp"]!["properties"]!["body"]!["labels"]!.Value<string>()
            .Should().Contain("union(");
    }

    [TestMethod]
    public void Stack_descriptions_are_supported_inside_and_outside_groups()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            @description('Global stack')
            stack global './stack.bicepparam' = {}
            group apps = {
              @sys.description('Grouped stack')
              stack backend './stack.bicepparam' = {}
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template!["resources"]!["global"]!["metadata"]!["description"]!.Value<string>().Should().Be("Global stack");
        result.Template!["resources"]!["apps::backend"]!["metadata"]!["description"]!.Value<string>().Should().Be("Grouped stack");
    }

    [TestMethod]
    public async Task Configurations_can_be_referenced_by_grouped_stacks()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            param regions array = ['westus', 'eastus']

            configuration global = {
              values: { environment: 'production' }
            }
            configuration regional = [for region in regions: {
              labels: { region: region }
              values: { location: region }
            }]
            group apps = [for (region, i) in regions: {
              stack backend './stack.bicepparam' = {
                labels: {
                  environment: global.values.environment
                  region: regional[i].values.location
                }
              }
            }]
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template!["variables"]!["global"]!["values"]!["environment"]!.Value<string>().Should().Be("production");
        result.Template!["variables"]!["copy"]!.Children().Should().Contain(item => item["name"]!.Value<string>() == "regional");
        var labels = result.Template!["resources"]!["apps::backend"]!["properties"]!["body"]!["labels"]!;
        labels["environment"]!.Value<string>().Should().Contain("variables('global')");
        labels["region"]!.Value<string>().Should().Contain("variables('regional')");

        var snapshot = await SnapshotHelper.GetSnapshot(
            ResourceScope.Orchestrator, result.Template.ToString(),
            """{"$schema":"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#","contentVersion":"1.0.0.0","parameters":{}}""",
            null, null, null, null, null, null, [], true, CancellationToken.None);
        snapshot.Diagnostics.Should().BeEmpty();
        var resolvedLabels = snapshot.PredictedResources
            .Select(resource => resource.GetProperty("properties").GetProperty("body").GetProperty("labels"))
            .ToArray();
        resolvedLabels.Select(label => label.GetProperty("region").GetString()).Should().Equal("westus", "eastus");
        resolvedLabels.Select(label => label.GetProperty("environment").GetString()).Should().OnlyContain(value => value == "production");
    }

    [TestMethod]
    public void Configurations_require_orchestrator_scope()
    {
        var result = CompileGroup("""
            configuration global = { values: {} }
            output value object = global.values
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP601");
    }

    [TestMethod]
    [DataRow("{}")]
    [DataRow("{ values: 'invalid' }")]
    [DataRow("{ values: {}, labels: { region: 1 } }")]
    [DataRow("[for region in ['westus']: { labels: { region: region } }]")]
    public void Configurations_require_values_and_string_labels(string value)
    {
        var result = CompileGroup($$"""
            targetScope = 'orchestrator'
            configuration global = {{value}}
            stack example './stack.bicepparam' = { inputs: global }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    public void Rules_support_conditional_declarations()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            param isDogfood bool
            rule changeApproval 'Approval' = if (!isDogfood) {
              provider: 'SafeFly'
              before: 'rollout'
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Conditional_rule_condition_must_be_boolean()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            param isDogfood string
            rule changeApproval 'Approval' = if (isDogfood) {
              provider: 'SafeFly'
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow("region")]
    [DataRow("Region")]
    public void Duplicate_group_and_stack_labels_are_rejected(string stackLabel)
    {
        var result = CompileGroup($$"""
            targetScope = 'orchestrator'
            group apps = {
              labels: { region: 'westus' }
              stack backend './stack.bicepparam' = {
                labels: { {{stackLabel}}: 'eastus' }
              }
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP604" && diagnostic.Message.Contains(stackLabel));
    }

    [TestMethod]
    public void Stack_labels_combined_with_group_labels_cannot_differ_only_in_casing()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = {
              labels: { kind: 'app' }
              stack backend './stack.bicepparam' = {
                labels: { region: 'westus', Region: 'eastus' }
              }
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP607" && diagnostic.Message.Contains("Region"));
    }

    [TestMethod]
    public void Looping_stacks_in_looping_groups_are_rejected()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = [for region in ['westus']: {
              stack backend './stack.bicepparam' = [for i in range(0, 2): {}]
            }]
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP603");
    }

    [TestMethod]
    public void Unknown_group_members_produce_a_diagnostic()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = {}
            stack global './stack.bicepparam' = {
              requires: [apps::missing]
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP602");
    }

    [TestMethod]
    [DataRow("group")]
    [DataRow("group apps =")]
    [DataRow("group apps = {\n  stack")]
    [DataRow("group apps = [for item in ['a']: {\n  stack app './stack.bicepparam' = { requires: [")]
    [DataRow("group apps = {\n  stack app './stack.bicepparam' = {}\n}\nstack other './stack.bicepparam' = { requires: [apps::")]
    public void Incomplete_group_syntax_reports_diagnostics(string declaration)
    {
        var result = CompileGroup($"targetScope = 'orchestrator'\n{declaration}");

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow("...settings\n  stack app './stack.bicepparam' = {}")]
    [DataRow("labels: { region: 'westus' }\n  stack app './stack.bicepparam' = { ...settings }")]
    public void Spreads_in_group_and_grouped_stack_bodies_are_rejected(string body)
    {
        var result = CompileGroup($$"""
            targetScope = 'orchestrator'
            var settings = { labels: { kind: 'app' } }
            group apps = {
              {{body}}
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP401");
    }

    [TestMethod]
    public void Computed_stack_body_keys_cannot_hide_group_label_conflicts()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            param propertyName string = 'labels'
            group apps = {
              labels: { region: 'westus' }
              stack app './stack.bicepparam' = {
                '${propertyName}': { region: 'eastus' }
              }
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error &&
            diagnostic.Message.Contains("property") && diagnostic.Message.Contains("labels"));
    }

    [TestMethod]
    public void Computed_stack_body_keys_remain_available_without_group_labels()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            param propertyName string = 'custom'
            group apps = {
              stack app './stack.bicepparam' = {
                '${propertyName}': 'value'
              }
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    [DataRow("{ region: 'westus' }", true)]
    [DataRow("{ region: 1 }", false)]
    [DataRow("{ region: null }", false)]
    [DataRow("'westus'", false)]
    public void Group_labels_require_string_values_even_without_stacks(string labels, bool valid)
    {
        var result = CompileGroup($$"""
            targetScope = 'orchestrator'
            group apps = {
              labels: {{labels}}
            }
            """);
        if (valid)
        {
            result.Should().NotHaveAnyDiagnostics();
        }
        else
        {
            result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
        }
    }

    [TestMethod]
    public void Identical_stack_names_in_different_groups_are_independent()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group one = {
              stack backend './stack.bicepparam' = {}
            }
            group two = {
              stack backend './stack.bicepparam' = {}
            }
            stack global './stack.bicepparam' = {
              requires: [one::backend, two::backend]
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template!["resources"]!["global"]!["dependsOn"]!.Values<string>()
            .Should().BeEquivalentTo("one::backend", "two::backend");
    }

    [TestMethod]
    public void Duplicate_stack_names_in_the_same_group_are_rejected()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = {
              stack backend './stack.bicepparam' = {}
              stack backend './stack.bicepparam' = {}
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP028");
    }

    [TestMethod]
    [DataRow("frontend")]
    [DataRow("apps::frontend")]
    public void Cyclic_grouped_stack_dependencies_are_rejected(string dependency)
    {
        var result = CompileGroup($$"""
            targetScope = 'orchestrator'
            group apps = {
              stack backend './stack.bicepparam' = { requires: [{{dependency}}] }
              stack frontend './stack.bicepparam' = { requires: [backend] }
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP080");
    }

    [TestMethod]
    public void Groups_require_orchestration_scope()
    {
        var result = CompileGroup("""
            targetScope = 'resourceGroup'
            group apps = {
              stack backend './stack.bicepparam' = {}
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP601");
    }

    [TestMethod]
    public void Missing_group_stack_artifact_reports_a_diagnostic()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = {
              stack backend './missing.bicepparam' = {}
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
    }

    [TestMethod]
    [DataRow("apps::backend")]
    [DataRow("apps::backend.labels")]
    public void Group_stack_references_outside_requires_produce_a_diagnostic(string reference)
    {
        var result = CompileGroup($$"""
            targetScope = 'orchestrator'
            group apps = {
              stack backend './stack.bicepparam' = {}
            }
            var unsupported = {{reference}}
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP606");
    }

    [TestMethod]
    public void Sibling_stack_values_outside_requires_produce_a_diagnostic()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            group apps = {
              stack backend './stack.bicepparam' = {}
              stack frontend './stack.bicepparam' = {
                inputs: { value: backend }
              }
            }
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Code == "BCP606");
    }

    [TestMethod]
    public void Unsupported_group_decorators_produce_a_diagnostic()
    {
        var result = CompileGroup("""
            targetScope = 'orchestrator'
            @batchSize(1)
            group apps = [for i in range(0, 2): {
              stack backend './stack.bicepparam' = {}
            }]
            """);

        result.Diagnostics.Should().Contain(diagnostic => diagnostic.Level == DiagnosticLevel.Error);
    }

    private static CompilationHelper.CompilationResult CompileGroup(string source) =>
        CompilationHelper.Compile(Services,
            ("main.bicep", source),
            ("stack.bicepparam", "using './stack.bicep'"),
            ("stack.bicep", "output value string = 'example'"));

    [TestMethod]
    [EmbeddedFilesTestData(@"orchestration_samples/basic/main\.bicepparam")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task Basic_example_can_be_compiled_successfully(EmbeddedFile file)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, file);

        var compiler = Services.Build().GetCompiler();
        var compilation = await compiler.CreateCompilation(IOUri.FromFilePath(baselineFolder.EntryFile.OutputFilePath));

        var result = CompilationHelper.CompileParams(compilation);

        result.Should().NotHaveAnyDiagnostics();

        // TODO needs some work to support baseline paths that differ from the source file paths
        // var paramsResult = result.Compilation.Emitter.Parameters();
        // var template = paramsResult.Template?.Template;
        // var parameters = paramsResult.Parameters;

        // var outputBaseUri = IOUri.FromFilePath(baselineFolder.OutputFolderPath).ToUriString();

        // var paramsFile = baselineFolder.GetFileOrEnsureCheckedIn("main.parameters.json");
        // var templateFile = baselineFolder.GetFileOrEnsureCheckedIn("main.json");

        // paramsFile.WriteToOutputFolder(parameters!);
        // templateFile.WriteToOutputFolder(template!);

        // paramsFile.ShouldHaveExpectedJsonValue();
        // templateFile.ShouldHaveExpectedJsonValue();
    }
}
