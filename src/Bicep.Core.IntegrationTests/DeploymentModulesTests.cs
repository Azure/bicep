// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Testing.Extensions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DeploymentModulesTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private const string ApiVersion = "2026-10-05-preview";

        private ServiceBuilder ServicesOn => new ServiceBuilder().WithFeatureOverrides(new(TestContext, DeploymentModulesEnabled: true));

        private static JToken Res(CompilationHelper.CompilationResult result, string symbol)
            => result.Template!.SelectToken($"$.resources.{symbol}")!;

        private static void AssertDeploymentModule(JToken module)
        {
            module.Should().NotBeNull();
            module["type"]!.Value<string>().Should().Be("Microsoft.Resources/deployments/modules");
            module["apiVersion"]!.Value<string>().Should().Be(ApiVersion);
            module["mode"].Should().BeNull();
            module["location"].Should().BeNull();
            module["scope"].Should().BeNull();
            module["subscriptionId"].Should().BeNull();
            module["resourceGroup"].Should().BeNull();
            module.SelectToken("$.properties.mode").Should().BeNull();
            module.SelectToken("$.properties.expressionEvaluationOptions").Should().BeNull();
        }

        private static void AssertScopeId(CompilationHelper.CompilationResult result, string symbol, string expected)
        {
            AssertDeploymentModule(Res(result, symbol));
            result.Template.Should().HaveValueAtPath($"$.resources.{symbol}.properties.resourceScope.id", expected);
        }

        private const string SimpleModule = """
            param p string = 'x'
            output o string = p
            @secure()
            output s string = 'secret'
            """;

        [TestMethod]
        public void Flag_off_emits_regular_deployments()
        {
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'mod'
                    }
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var arm = Res(result, "mod");
            arm["type"]!.Value<string>().Should().Be("Microsoft.Resources/deployments");
            arm["properties"]!["mode"]!.Value<string>().Should().Be("Incremental");
            arm["properties"]!["expressionEvaluationOptions"].Should().NotBeNull();
        }

        [TestMethod]
        public void Same_scope_module_emits_deployment_module()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'myMod'
                      params: {
                        p: 'v'
                      }
                    }

                    output o string = mod.outputs.o
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertScopeId(result, "mod", "[resourceGroup().id]");
            result.Template.Should().HaveValueAtPath("$.resources.mod.name", "[format('{0}/{1}', split(deployment().name, '/')[0], 'myMod')]");
            result.Template.Should().HaveValueAtPath("$.resources.mod.properties.parameters.p.value", "v");
            result.Template!.SelectToken("$.languageVersion")!.Value<string>().Should().EndWith("-experimental");
            result.Template.Should().HaveValueAtPath("$.outputs.o.value", "[reference('mod').outputs.o.value]");
        }

        [TestMethod]
        public void Module_without_name_gets_wrapped_generated_name()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                    }
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "mod"));
            Res(result, "mod")["name"]!.Value<string>().Should().StartWith("[format('{0}/{1}', split(deployment().name, '/')[0], ");
        }

        [TestMethod]
        public void Explicit_resource_group_scopes_with_loops()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    param subId string
                    param names array

                    module a 'mod.bicep' = [for n in names: {
                      name: 'a${n}'
                      scope: resourceGroup('rg-${n}')
                    }]

                    module b 'mod.bicep' = [for n in names: {
                      name: 'b${n}'
                      scope: resourceGroup(subId, 'rg-${n}')
                    }]

                    module c 'mod.bicep' = {
                      name: 'c'
                      scope: resourceGroup('other')
                    }

                    module d 'mod.bicep' = {
                      name: 'd'
                      scope: resourceGroup(subId, 'other')
                    }
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            foreach (var s in new[] { "a", "b", "c", "d" })
            {
                AssertDeploymentModule(Res(result, s));
            }

            result.Template.Should().HaveValueAtPath("$.resources.a.properties.resourceScope.id", "[format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, format('rg-{0}', parameters('names')[copyIndex()]))]");
            result.Template.Should().HaveValueAtPath("$.resources.b.properties.resourceScope.id", "[format('/subscriptions/{0}/resourceGroups/{1}', parameters('subId'), format('rg-{0}', parameters('names')[copyIndex()]))]");
            result.Template.Should().HaveValueAtPath("$.resources.c.properties.resourceScope.id", "[format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'other')]");
            result.Template.Should().HaveValueAtPath("$.resources.d.properties.resourceScope.id", "[format('/subscriptions/{0}/resourceGroups/{1}', parameters('subId'), 'other')]");
            Res(result, "a")["copy"].Should().NotBeNull();
        }

        [TestMethod]
        public void Subscription_scope_parent_scopes()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    targetScope = 'subscription'

                    module implicitSub 'sub.bicep' = {
                      name: 'implicitSub'
                    }

                    module explicitSub 'sub.bicep' = {
                      name: 'explicitSub'
                      scope: subscription('xyz')
                    }

                    module rgMod 'mod.bicep' = {
                      name: 'rgMod'
                      scope: resourceGroup('rg')
                    }

                    module tenantMod 'tenant.bicep' = {
                      name: 'tenantMod'
                      scope: tenant()
                    }
                    """),
                ("mod.bicep", SimpleModule),
                ("sub.bicep", "targetScope = 'subscription'\noutput o string = 'x'"),
                ("tenant.bicep", "targetScope = 'tenant'\noutput o string = 'x'"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertScopeId(result, "implicitSub", "[subscription().id]");
            AssertDeploymentModule(Res(result, "explicitSub"));
            Res(result, "explicitSub")["properties"]!["resourceScope"]!["id"]!.Value<string>().Should().Contain("/subscriptions/{0}").And.Contain("'xyz'");
            AssertDeploymentModule(Res(result, "rgMod"));
            Res(result, "rgMod")["properties"]!["resourceScope"]!["id"]!.Value<string>().Should().Contain("/subscriptions/{0}/resourceGroups/{1}").And.Contain("subscription().subscriptionId").And.Contain("'rg'");
            AssertScopeId(result, "tenantMod", "/");
        }

        [TestMethod]
        public void Management_group_scope_parent_scopes()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    targetScope = 'managementGroup'

                    module implicitMg 'mg.bicep' = {
                      name: 'implicitMg'
                    }

                    module explicitMg 'mg.bicep' = {
                      name: 'explicitMg'
                      scope: managementGroup('mg')
                    }
                    """),
                ("mg.bicep", "targetScope = 'managementGroup'\noutput o string = 'x'"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertScopeId(result, "implicitMg", "[managementGroup().id]");
            AssertDeploymentModule(Res(result, "explicitMg"));
            Res(result, "explicitMg")["properties"]!["resourceScope"]!["id"]!.Value<string>().Should().Contain("tenantResourceId('Microsoft.Management/managementGroups'").And.Contain("'mg'");
        }

        [TestMethod]
        public void Tenant_scope_parent_with_management_group_module()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    targetScope = 'tenant'

                    module mg 'mg.bicep' = {
                      name: 'mg'
                      scope: managementGroup('mg')
                    }
                    """),
                ("mg.bicep", "targetScope = 'managementGroup'\noutput o string = 'x'"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "mg"));
            Res(result, "mg")["properties"]!["resourceScope"]!["id"]!.Value<string>().Should().Contain("tenantResourceId('Microsoft.Management/managementGroups'");
        }

        [TestMethod]
        public void Conditional_module_and_depends_on()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    param deploy bool

                    module mod 'mod.bicep' = if (deploy) {
                      name: 'mod'
                    }

                    resource sa 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                      name: 'sastorage1'
                      location: 'westus'
                      kind: 'StorageV2'
                      sku: {
                        name: 'Standard_LRS'
                      }
                      dependsOn: [
                        mod
                      ]
                    }
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "mod"));
            result.Template.Should().HaveValueAtPath("$.resources.mod.condition", "[parameters('deploy')]");
            result.Template.Should().HaveJsonAtPath("$.resources.sa.dependsOn", """["mod"]""");
        }

        [TestMethod]
        public void Secure_output_uses_list_outputs_with_secure_values()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'mod'
                    }

                    output s string = mod.outputs.s
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs.s.value", $"[listOutputsWithSecureValues('mod', '{ApiVersion}').s]");
        }

        [TestMethod]
        public void Nested_modules_also_emit_deployment_modules()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    module outer 'outer.bicep' = {
                      name: 'outer'
                    }
                    """),
                ("outer.bicep", """
                    module inner 'inner.bicep' = {
                      name: 'inner'
                    }
                    """),
                ("inner.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "outer"));
            var inner = result.Template!.SelectToken("$.resources.outer.properties.template.resources.inner")!;
            AssertDeploymentModule(inner);
            inner["name"]!.Value<string>().Should().Be("[format('{0}/{1}', split(deployment().name, '/')[0], 'inner')]");
        }

        [TestMethod]
        public void Module_name_access_returns_user_facing_name()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'myMod'
                    }

                    output n string = mod.name
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var value = result.Template!.SelectToken("$.outputs.n.value")!.Value<string>()!;
            value.Should().NotContain("split(deployment().name");
            value.Should().Contain("myMod");
        }

        private const string TwoRgSameNameMain = """
            module a 'mod.bicep' = {
              name: 'deploy'
              scope: resourceGroup('a')
            }

            module b 'mod.bicep' = {
              name: 'deploy'
              scope: resourceGroup('b')
            }
            """;

        [TestMethod]
        public void Same_name_in_different_resource_groups_is_duplicate_when_flag_on()
        {
            var result = CompilationHelper.Compile(ServicesOn, ("main.bicep", TwoRgSameNameMain), ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Diagnostics.Where(d => d.Code == "BCP122").Should().HaveCount(2);
        }

        [TestMethod]
        public void Same_name_in_different_resource_groups_is_fine_when_flag_off()
        {
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
                ("main.bicep", TwoRgSameNameMain),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Diagnostics.Should().NotContain(d => d.Code == "BCP122");
        }

        private const string ScopeOnlyLoopMain = """
            param rgs array

            module mods 'mod.bicep' = [for rg in rgs: {
              name: 'deploy'
              scope: resourceGroup(rg)
            }]
            """;

        [TestMethod]
        public void Loop_varying_only_scope_with_constant_name_is_invariant_when_flag_on()
        {
            var result = CompilationHelper.Compile(ServicesOn, ("main.bicep", ScopeOnlyLoopMain), ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Diagnostics.Should().Contain(d => d.Code == "BCP179");
        }

        [TestMethod]
        public void Loop_varying_only_scope_with_constant_name_is_fine_when_flag_off()
        {
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
                ("main.bicep", ScopeOnlyLoopMain),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Diagnostics.Should().NotContain(d => d.Code == "BCP179");
        }

        [TestMethod]
        public void Loop_with_generated_name_and_varying_scope_is_fine()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    param rgs array

                    module mods 'mod.bicep' = [for rg in rgs: {
                      scope: resourceGroup(rg)
                    }]
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Existing_resource_group_symbols_as_scopes()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    targetScope = 'subscription'

                    param names array

                    resource rgs 'Microsoft.Resources/resourceGroups@2022-09-01' existing = [for n in names: {
                      name: n
                    }]

                    resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' existing = {
                      name: 'single'
                    }

                    module viaCollection 'mod.bicep' = [for (n, i) in names: {
                      name: 'viaCollection${n}'
                      scope: rgs[i]
                    }]

                    module viaSingle 'mod.bicep' = {
                      name: 'viaSingle'
                      scope: rg
                    }
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertScopeId(result, "viaCollection", "[format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, parameters('names')[copyIndex()])]");
            AssertScopeId(result, "viaSingle", "[format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'single')]");
        }

        [TestMethod]
        public void Existing_management_group_symbol_as_scope()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    targetScope = 'tenant'

                    resource mg 'Microsoft.Management/managementGroups@2023-04-01' existing = {
                      scope: tenant()
                      name: 'myMg'
                    }

                    module mod 'mg.bicep' = {
                      name: 'mod'
                      scope: mg
                    }
                    """),
                ("mg.bicep", "targetScope = 'managementGroup'\noutput o string = 'x'"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertScopeId(result, "mod", "[tenantResourceId('Microsoft.Management/managementGroups', 'myMg')]");
        }

        [TestMethod]
        public void Module_extension_configs_are_emitted_under_properties()
        {
            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(TestContext, DeploymentModulesEnabled: true, ModuleExtensionConfigsEnabled: true))
                .WithConfigurationPatch(c => c.WithExtensions("""
                    {
                      "az": "builtin:",
                      "kubernetes": "builtin:"
                    }
                    """))
                .WithNamespaceProvider(TestExtensionsNamespaceProvider.CreateWithDefaults());

            var result = CompilationHelper.Compile(
                services,
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'mod'
                      extensionConfigs: {
                        kubernetes: {
                          kubeConfig: 'cfg'
                          namespace: 'ns'
                        }
                      }
                    }
                    """),
                ("mod.bicep", """
                    extension kubernetes
                    output o string = 'x'
                    """));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "mod"));
            result.Template.Should().HaveValueAtPath("$.resources.mod.properties.extensionConfigs.kubernetes.kubeConfig.value", "cfg");
            result.Template.Should().HaveValueAtPath("$.resources.mod.properties.extensionConfigs.kubernetes.namespace.value", "ns");
        }

        [TestMethod]
        public void Local_target_scope_with_flag_on_emits_local_deploy_shape()
        {
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, DeploymentModulesEnabled: true, LocalDeployEnabled: true)),
                ("main.bicep", """
                    targetScope = 'local'

                    module mod 'mod.bicep' = {
                      name: 'mod'
                      scope: resourceGroup('00000000-0000-0000-0000-000000000000', 'rg')
                    }
                    """),
                ("mod.bicep", "output o string = 'x'"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            var arm = Res(result, "mod");
            arm["type"]!.Value<string>().Should().Be("Microsoft.Resources/deployments");
            result.Template.Should().NotHaveValueAtPath("$.resources.mod.properties.resourceScope");
        }

        [TestMethod]
        public void Module_collection_references_compile()
        {
            var result = CompilationHelper.Compile(
                ServicesOn,
                ("main.bicep", """
                    module mods 'mod.bicep' = [for i in range(0, 2): {
                      name: 'mod${i}'
                    }]

                    module dependent 'mod.bicep' = {
                      name: 'dependent'
                      dependsOn: [
                        mods[0]
                      ]
                    }

                    output first string = mods[0].outputs.o
                    output all array = [for i in range(0, 2): mods[i].outputs.o]
                    output names array = [for i in range(0, 2): mods[i].name]
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "mods"));
            result.Template.Should().HaveValueAtPath("$.outputs.first.value", "[reference(format('mods[{0}]', 0)).outputs.o.value]");
            result.Template.Should().HaveValueAtPath("$.outputs.all.copy.input", "[reference(format('mods[{0}]', range(0, 2)[copyIndex()])).outputs.o.value]");
            result.Template.Should().HaveJsonAtPath("$.resources.dependent.dependsOn", """["[format('mods[{0}]', 0)]"]""");
        }

        [TestMethod]
        public void Template_evaluator_skips_nested_deployment_module_templates()
        {
            // the nested template references a parameter that only the child template declares
            var template = JToken.Parse("""
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                  "contentVersion": "1.0.0.0",
                  "resources": [
                    {
                      "type": "Microsoft.Resources/deployments/modules",
                      "apiVersion": "2026-10-05-preview",
                      "name": "root/mod",
                      "properties": {
                        "template": {
                          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                          "contentVersion": "1.0.0.0",
                          "parameters": {
                            "childOnly": { "type": "string", "defaultValue": "x" }
                          },
                          "resources": [],
                          "outputs": {
                            "o": { "type": "string", "value": "[parameters('childOnly')]" }
                          }
                        }
                      }
                    }
                  ]
                }
                """);

            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();
            evaluated.SelectToken("$.resources[0].properties.template.outputs.o.value")!.Value<string>().Should().Be("[parameters('childOnly')]");
        }

        [TestMethod]
        public void Module_identity_passes_through()
        {
            var result = CompilationHelper.Compile(
                new ServiceBuilder().WithFeatureOverrides(new(TestContext, DeploymentModulesEnabled: true, ModuleExtensionConfigsEnabled: true)),
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'mod'
                      identity: {
                        type: 'UserAssigned'
                        userAssignedIdentities: {
                          id: {}
                        }
                      }
                    }
                    """),
                ("mod.bicep", SimpleModule));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            AssertDeploymentModule(Res(result, "mod"));
            result.Template.Should().HaveJsonAtPath("$.resources.mod.identity", """{"type":"UserAssigned","userAssignedIdentities":{"id":{}}}""");
            result.Template!.SelectToken("$.languageVersion")!.Value<string>().Should().Be("2.2-experimental");
        }
    }
}
