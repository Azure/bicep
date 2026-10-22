// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Emit
{
    [TestClass]
    public class EmitterSettingsForcedSymbolicNamesTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private const string VanillaResourceBicep = """
            resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
              name: 'test123'
              location: 'eastus'
              sku: { name: 'Standard_LRS' }
              kind: 'StorageV2'
            }
            """;

        private const string LegacyOuterScopedDeploymentBicep = """
            param tags object

            resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
              name: 'nested'
              properties: {
                mode: 'Incremental'
                template: {
                  '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                  contentVersion: '1.0.0.0'
                  resources: []
                  outputs: {
                    tags: {
                      type: 'object'
                      value: tags
                    }
                  }
                }
              }
            }
            """;

        private const string LegacyDeploymentFunctionBicep = """
            resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
              name: 'nested'
              properties: {
                mode: 'Incremental'
                template: {
                  '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                  contentVersion: '1.0.0.0'
                  resources: []
                  outputs: {
                    deploymentName: {
                      type: 'string'
                      value: deployment().name
                    }
                  }
                }
              }
            }
            """;

        private const string LegacyEnclosingLoopLocalBicep = """
            resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = [for item in ['nested']: {
              name: item
              properties: {
                mode: 'Incremental'
                template: {
                  '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                  contentVersion: '1.0.0.0'
                  resources: []
                  outputs: {
                    loopItem: {
                      type: 'string'
                      value: item
                    }
                  }
                }
              }
            }]
            """;

        private const string LegacyScopeRelativeResourceIdBicep = """
            resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
              name: 'nested'
              properties: {
                mode: 'Incremental'
                template: {
                  '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                  contentVersion: '1.0.0.0'
                  resources: []
                  outputs: {
                    resourceId: {
                      type: 'string'
                      value: resourceId('Microsoft.Storage/storageAccounts', 'stg')
                    }
                  }
                }
              }
            }
            """;

        private const string LegacyNameBasedReferenceBicep = """
            resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
              name: 'nested'
              properties: {
                mode: 'Incremental'
                template: {
                  '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                  contentVersion: '1.0.0.0'
                  resources: []
                  outputs: {
                    resource: {
                      type: 'object'
                      value: reference('stg', '2023-01-01')
                    }
                  }
                }
              }
            }
            """;

        [TestMethod]
        public void EmitterSettings_Default_LeavesEnableSymbolicNamesOff()
        {
            var model = Services.BuildCompilation(VanillaResourceBicep).GetEntrypointSemanticModel();

            new EmitterSettings(model).EnableSymbolicNames.Should().BeFalse();
        }

        [TestMethod]
        public void EmitterSettings_ForceEnableSymbolicNames_TurnsFlagOn()
        {
            var model = Services.BuildCompilation(VanillaResourceBicep).GetEntrypointSemanticModel();

            new EmitterSettings(model, forceEnableSymbolicNames: true).EnableSymbolicNames.Should().BeTrue();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedAndNormalPathsKeepIndependentResourceShapes()
        {
            var model = Services.BuildCompilation(VanillaResourceBicep).GetEntrypointSemanticModel();
            var forcedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();
            var normalJson = EmitNormally(model);

            forcedJson.Should().NotBeNullOrEmpty();
            JObject.Parse(forcedJson!)["resources"]!.Type.Should().Be(JTokenType.Object);
            JObject.Parse(normalJson)["resources"]!.Type.Should().Be(JTokenType.Array);
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsInvalidModels()
        {
            var model = Services.BuildCompilation("var broken = missingSymbol").GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsBinderErrors()
        {
            var model = Services.BuildCompilation("""
                var duplicate = 'first'
                var duplicate = 'second'
                """).GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().Contain(diagnostic => diagnostic.Code == "BCP028");
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsEmitLimitationErrors()
        {
            var model = Services.BuildCompilation("""
                resource first 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'duplicate'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                resource second 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'duplicate'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }
                """).GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().Contain(diagnostic => diagnostic.Code == "BCP121");
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsCompilerVersionErrors()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("bicepconfig.json", """{"bicep":{"version":">=999.0.0"}}"""),
                ("main.bicep", VanillaResourceBicep));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .WithBicepVersion("1.0.0")
                .WithCompilerVersionCheckOptions(new(DiagnosticLevel.Error))
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().Contain(diagnostic => diagnostic.Code == "BCP456");
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathIgnoresLinterErrors()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("bicepconfig.json", """
                    {
                      "analyzers": {
                        "core": {
                          "rules": {
                            "no-unused-params": { "level": "error" }
                          }
                        }
                      }
                    }
                    """),
                ("main.bicep", $"""
                    param unused string

                    {VanillaResourceBicep}
                    """));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().Contain(diagnostic => diagnostic.Code == "no-unused-params" && diagnostic.IsError());
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsLegacyOuterScopedNestedDeploymentReferences()
        {
            var model = Services.BuildCompilation(LegacyOuterScopedDeploymentBicep).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsExplicitDeploymentResources()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            var normalTemplate = JObject.Parse(EmitNormally(model));

            normalTemplate["resources"].Should().BeOfType<JArray>().Which
                .OfType<JObject>().Should().ContainSingle(resource =>
                    resource["type"]!.ToString() == TemplateWriter.NestedDeploymentResourceType);
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsLegacyDeploymentFunction()
        {
            var model = Services.BuildCompilation(LegacyDeploymentFunctionBicep).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsLegacyEnclosingLoopLocal()
        {
            var model = Services.BuildCompilation(LegacyEnclosingLoopLocalBicep).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsExplicitOuterScopeWithoutReferences()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    expressionEvaluationOptions: {
                      scope: 'outer'
                    }
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsIndirectEvaluationOptions()
        {
            var model = Services.BuildCompilation("""
                var evaluationOptions = {
                  scope: 'outer'
                }

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    expressionEvaluationOptions: evaluationOptions
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsSpreadDeploymentProperties()
        {
            var model = Services.BuildCompilation("""
                var sharedProperties = {
                  expressionEvaluationOptions: {
                    scope: 'outer'
                  }
                  mode: 'Incremental'
                }

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    ...sharedProperties
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsSpreadOnlyDeploymentProperties()
        {
            var model = Services.BuildCompilation("""
                var sharedProperties = {
                  expressionEvaluationOptions: {
                    scope: 'outer'
                  }
                  mode: 'Incremental'
                  template: {
                    '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                    contentVersion: '1.0.0.0'
                    resources: []
                  }
                }

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    ...sharedProperties
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsDynamicTemplateKey()
        {
            var model = Services.BuildCompilation("""
                var templateKey = 'template'

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    '${templateKey}': {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsCaseVariantTemplateKeys()
        {
            var model = Services.BuildCompilation("""
                param tags object

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                    Template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                      outputs: {
                        tags: {
                          type: 'object'
                          value: tags
                        }
                      }
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsSpreadEvaluationOptions()
        {
            var model = Services.BuildCompilation("""
                var outerScope = {
                  scope: 'outer'
                }

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    expressionEvaluationOptions: {
                      ...outerScope
                    }
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsDynamicEvaluationScopeKey()
        {
            var model = Services.BuildCompilation("""
                var scopeKey = 'scope'

                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    expressionEvaluationOptions: {
                      '${scopeKey}': 'outer'
                    }
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsCaseVariantEvaluationScopeKeys()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    expressionEvaluationOptions: {
                      scope: 'inner'
                      Scope: 'outer'
                    }
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsOpaqueTemplatePayload()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: json('{"$schema":"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#","contentVersion":"1.0.0.0","resources":[],"outputs":{"name":{"type":"string","value":"[deployment().name]"}}}')
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsOpaqueTemplateSpread()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      ...json('{"$schema":"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#","contentVersion":"1.0.0.0","resources":[],"outputs":{"name":{"type":"string","value":"[deployment().name]"}}}')
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsNestedOpaqueTemplatePayload()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                      outputs: json('{"name":{"type":"string","value":"[deployment().name]"}}')
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsBase64EncodedOpaqueTemplatePayload()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                      outputs: base64ToJson(base64('{"name":{"type":"string","value":"[deployment().name]"}}'))
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsComposedOpaqueTemplateStructure()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                      outputs: union({
                        name: {
                          type: 'string'
                          value: '[deployment().name]'
                        }
                      }, {})
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsOpaqueOutputDefinition()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                      outputs: {
                        name: json('{"type":"string","value":"[deployment().name]"}')
                      }
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsOpaqueResourceDefinition()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: [
                        json('{"type":"Microsoft.Storage/storageAccounts","apiVersion":"2023-01-01","name":"[deployment().name]","location":"eastus","sku":{"name":"Standard_LRS"},"kind":"StorageV2"}')
                      ]
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsOpaqueFunctionDefinition()
        {
            var model = Services.BuildCompilation("""
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                      functions: [
                        base64ToJson(base64('{"namespace":"hidden","members":{}}'))
                      ]
                    }
                  }
                }
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("(", ")")]
        [DataRow("any(", ")")]
        public void TemplateEmitter_ForcedPathRejectsWrappedOuterReferences(string wrapperStart, string wrapperEnd)
        {
            var model = Services.BuildCompilation(GetWrappedLegacyOuterScopeBicep(wrapperStart, wrapperEnd))
                .GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsScopeRelativeResourceId()
        {
            var model = Services.BuildCompilation(LegacyScopeRelativeResourceIdBicep).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsLiteralNameBasedReference()
        {
            var model = Services.BuildCompilation(LegacyNameBasedReferenceBicep).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathEmitsExistingResourcesAsExisting()
        {
            var model = Services.BuildCompilation("""
                resource existingStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
                  name: 'existing'
                }

                resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'deployed'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }
                """).GetEntrypointSemanticModel();

            var forcedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

            var resources = JObject.Parse(forcedJson!)["resources"]!.Should().BeOfType<JObject>().Subject;
            resources.ContainsKey("storage").Should().BeTrue();
            resources["existingStorage"]!["existing"]!.Value<bool>().Should().BeTrue();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathAllowsReferencedExistingResources()
        {
            var model = Services.BuildCompilation("""
                resource existingStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
                  name: 'existing'
                }

                resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'deployed'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                  properties: {
                    accessTier: existingStorage.properties.accessTier
                  }
                }
                """).GetEntrypointSemanticModel();

            var forcedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

            var resources = JObject.Parse(forcedJson!)["resources"]!.Should().BeOfType<JObject>().Subject;
            resources.ContainsKey("existingStorage").Should().BeTrue();
            resources["storage"]!["properties"]!["accessTier"]!.Value<string>()
                .Should().Contain("reference('existingStorage')");
        }

        [DataTestMethod]
        [DataRow("reference('first', '2023-01-01')")]
        [DataRow("reference(('FIRST'), '2023-01-01')")]
        [DataRow("reference(any('first'), '2023-01-01')")]
        [DataRow("listKeys('first', '2023-01-01')")]
        public void TemplateEmitter_ForcedPathRejectsLiteralTargetMatchingSymbolicName(string call)
        {
            var model = Services.BuildCompilation($$"""
                resource first 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'alpha'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                output referenced object = {{call}}
                """).GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().NotContain(diagnostic => diagnostic.IsError());
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsLiteralTargetMatchingModuleSymbolicName()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    module child './child.bicep' = {
                      name: 'child-deployment'
                    }

                    output referenced object = reference('child', '2022-09-01')
                    """),
                ("child.bicep", VanillaResourceBicep));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var model = (await compiler.CreateCompilation(fileSet.GetUri("main.bicep"))).GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().NotContain(diagnostic => diagnostic.IsError());
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("reference(storageId, '2023-01-01')")]
        [DataRow("listKeys(storageId, '2023-01-01')")]
        [DataRow("reference('alpha', '2023-01-01')")]
        public void TemplateEmitter_ForcedPathAllowsTargetsThatCannotMatchSymbolicNames(string call)
        {
            var model = Services.BuildCompilation($$"""
                param storageId string

                resource first 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'alpha'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                output referenced object = {{call}}
                """).GetEntrypointSemanticModel();

            model.GetAllDiagnostics().Should().NotContain(diagnostic => diagnostic.IsError());
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathRejectsRootNameBasedReference()
        {
            var model = Services.BuildCompilation("""
                resource first 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'alpha'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                resource second 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                  name: 'first'
                  location: 'eastus'
                  sku: { name: 'Standard_LRS' }
                  kind: 'StorageV2'
                }

                output referenced object = reference('first', '2023-01-01')
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathAllowsRootFullResourceIdReference()
        {
            var model = Services.BuildCompilation("""
                output referenced object = reference(
                  '/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/rg/providers/Microsoft.Storage/storageAccounts/stg',
                  '2023-01-01')
                """).GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void TemplateEmitter_ForcedPathIsSafeDuringAggregateDiagnosticAssembly()
        {
            var analyzer = new ForcedEmissionAnalyzer();
            var compilation = Services.WithBicepAnalyzer(analyzer).BuildCompilation(VanillaResourceBicep);

            var action = () => compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            action.Should().NotThrow();
            analyzer.WasInvoked.Should().BeTrue("otherwise the re-entry assertion is vacuous");
            analyzer.EmittedJson.Should().NotBeNullOrEmpty();
            JObject.Parse(analyzer.EmittedJson!)["resources"]!.Type.Should().Be(JTokenType.Object);
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathPropagatesToPlainNestedModules()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    module child './child.bicep' = {
                      name: 'child'
                    }
                    """),
                ("child.bicep", VanillaResourceBicep));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();

            var forcedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();
            var normalJson = EmitNormally(model);

            forcedJson.Should().NotBeNullOrEmpty();
            var forcedTemplate = JObject.Parse(forcedJson!);
            forcedTemplate["resources"]!["child"]!["properties"]!["template"]!["resources"]!.Type
                .Should().Be(JTokenType.Object);
            var normalTemplate = JObject.Parse(normalJson);
            normalTemplate["resources"]![0]!["properties"]!["template"]!["resources"]!.Type
                .Should().Be(JTokenType.Array);
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsLegacyOuterScopedNestedDeploymentReferencesInModules()
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    module child './child.bicep' = {
                      name: 'child'
                      params: {
                        tags: {}
                      }
                    }
                    """),
                ("child.bicep", LegacyOuterScopedDeploymentBicep));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            var model = compilation.GetEntrypointSemanticModel();

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsLegacyDeploymentFunctionInModules()
        {
            var model = await BuildModuleCompilation(LegacyDeploymentFunctionBicep);

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsLegacyEnclosingLoopLocalInModules()
        {
            var model = await BuildModuleCompilation(LegacyEnclosingLoopLocalBicep);

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsExplicitDeploymentResourcesInModules()
        {
            var childBicep = """
                resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
                  name: 'nested'
                  properties: {
                    expressionEvaluationOptions: {
                      scope: 'outer'
                    }
                    mode: 'Incremental'
                    template: {
                      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                      contentVersion: '1.0.0.0'
                      resources: []
                    }
                  }
                }
                """;
            var model = await BuildModuleCompilation(childBicep);

            var normalTemplate = JObject.Parse(EmitNormally(model));
            var moduleDeployment = normalTemplate["resources"].Should().BeOfType<JArray>().Which
                .OfType<JObject>().Should().ContainSingle(resource =>
                    resource["type"]!.ToString() == TemplateWriter.NestedDeploymentResourceType).Which;
            moduleDeployment["properties"]!["template"]!["resources"].Should().BeOfType<JArray>().Which
                .OfType<JObject>().Should().ContainSingle(resource =>
                    resource["type"]!.ToString() == TemplateWriter.NestedDeploymentResourceType);
            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsWrappedOuterReferencesInModules()
        {
            var model = await BuildModuleCompilation(GetWrappedLegacyOuterScopeBicep("any(", ")"));

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsScopeRelativeResourceIdInModules()
        {
            var model = await BuildModuleCompilation(LegacyScopeRelativeResourceIdBicep);

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        [TestMethod]
        public async Task TemplateEmitter_ForcedPathRejectsLiteralNameBasedReferenceInModules()
        {
            var model = await BuildModuleCompilation(LegacyNameBasedReferenceBicep);

            new TemplateEmitter(model).EmitWithForcedSymbolicNames().Should().BeNull();
        }

        private static string GetWrappedLegacyOuterScopeBicep(string wrapperStart, string wrapperEnd) => $$"""
            param tags object

            resource nestedDeployment 'Microsoft.Resources/deployments@2020-10-01' = {
              name: 'nested'
              properties: {{wrapperStart}}{
                mode: 'Incremental'
                template: {
                  '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
                  contentVersion: '1.0.0.0'
                  resources: []
                  outputs: {
                    tags: {
                      type: 'object'
                      value: tags
                    }
                  }
                }
              }{{wrapperEnd}}
            }
            """;

        private static async Task<SemanticModel> BuildModuleCompilation(string childBicep)
        {
            var fileSet = MockFileSystemTestFileSet.Create(
                ("main.bicep", """
                    module child './child.bicep' = {
                      name: 'child'
                    }
                    """),
                ("child.bicep", childBicep));
            var compiler = Services
                .WithFileExplorer(fileSet.FileExplorer)
                .WithFileSystem(fileSet.FileSystem)
                .Build()
                .GetCompiler();
            var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
            return compilation.GetEntrypointSemanticModel();
        }

        private static string EmitNormally(SemanticModel model)
        {
            using var writer = new StringWriter { NewLine = "\n" };
            var result = new TemplateEmitter(model).Emit(writer);
            result.Status.Should().Be(EmitStatus.Succeeded);
            return writer.ToString();
        }

        private sealed class ForcedEmissionAnalyzer : IBicepAnalyzer
        {
            public bool WasInvoked { get; private set; }

            public string? EmittedJson { get; private set; }

            public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => [];

            public IEnumerable<IDiagnostic> Analyze(SemanticModel model)
            {
                this.WasInvoked = true;
                this.EmittedJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();
                return [];
            }
        }
    }
}