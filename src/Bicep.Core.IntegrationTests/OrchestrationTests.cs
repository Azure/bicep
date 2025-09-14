// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.TextFixtures.Assertions;
using Bicep.TextFixtures.IO;
using Bicep.TextFixtures.Mocks;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
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
    [EmbeddedFilesTestData(@"orchestration_samples/basic/main\.bicepparam")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public void Basic_example_can_be_compiled_successfully(EmbeddedFile file)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, file);

        var compiler = Services.Build().GetCompiler();
        var compilation = compiler.CreateCompilationWithoutRestore(IOUri.FromFilePath(baselineFolder.EntryFile.OutputFilePath));

        var result = CompilationHelper.CompileParams(compilation);

        result.Should().NotHaveAnyDiagnostics();

        var paramsResult = result.Compilation.Emitter.Parameters();
        var template = paramsResult.Template?.Template;
        var parameters = paramsResult.Parameters;

        var outputBaseUri = IOUri.FromFilePath(baselineFolder.OutputFolderPath).ToUriString();

        template!.Replace(outputBaseUri, "${TEST_OUTPUT_DIR}").FromJson<JToken>().Should().HaveJsonAtPath("$.resources", """
{
  "global": {
    "import": "az0synthesized",
    "type": "OrchestrationStack",
    "properties": {
      "template": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\"contentVersion\":\"1.0.0.0\",\"metadata\":{\"_generator\":{\"name\":\"bicep\",\"version\":\"dev\",\"templateHash\":\"12225932258748713702\"}},\"parameters\":{\"foo\":{\"type\":\"string\"}},\"resources\":[]}",
      "parameters": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\"contentVersion\":\"1.0.0.0\",\"parameters\":{\"foo\":{\"expression\":\"[externalInputs('stack_setting_0').foo]\"},\"$usingConfig\":{\"expression\":\"[createObject('mode', 'stack', 'scope', format('/subscriptions/{0}/resourceGroups/{1}', externalInputs('stack_setting_1'), externalInputs('stack_setting_2')), 'name', string(externalInputs('stack_setting_3')), 'actionOnUnmanage', createObject('resources', 'delete'), 'denySettings', createObject('mode', 'denyDelete'))]\"}},\"externalInputDefinitions\":{\"stack_setting_0\":{\"kind\":\"stack.setting\",\"config\":\"config\"},\"stack_setting_1\":{\"kind\":\"stack.setting\",\"config\":\"subscriptionId\"},\"stack_setting_2\":{\"kind\":\"stack.setting\",\"config\":\"resourceGroup\"},\"stack_setting_3\":{\"kind\":\"stack.setting\",\"config\":\"name\"}}}",
      "sourceUri": "${TEST_OUTPUT_DIR}/global/main.bicepparam",
      "body": {
        "region": "[parameters('config').global.infraRegion]",
        "deploy": "onChange",
        "inputs": {
          "subscriptionId": "[__bicep.lookupSubscription('global')]",
          "resourceGroup": "[format('{0}-global', variables('prefix'))]",
          "name": "[format('{0}-global', variables('prefix'))]",
          "config": {
            "foo": "bar"
          }
        }
      }
    }
  },
  "cluster": {
    "import": "az0synthesized",
    "copy": {
      "name": "cluster",
      "count": "[length(parameters('config').regions)]"
    },
    "type": "OrchestrationStack",
    "properties": {
      "template": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\"contentVersion\":\"1.0.0.0\",\"metadata\":{\"_generator\":{\"name\":\"bicep\",\"version\":\"dev\",\"templateHash\":\"12225932258748713702\"}},\"parameters\":{\"foo\":{\"type\":\"string\"}},\"resources\":[]}",
      "parameters": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\"contentVersion\":\"1.0.0.0\",\"parameters\":{\"foo\":{\"expression\":\"[externalInputs('stack_setting_0').foo]\"},\"$usingConfig\":{\"expression\":\"[createObject('mode', 'stack', 'scope', format('/subscriptions/{0}/resourceGroups/{1}', externalInputs('stack_setting_1'), externalInputs('stack_setting_2')), 'name', string(externalInputs('stack_setting_3')), 'actionOnUnmanage', createObject('resources', 'delete'), 'denySettings', createObject('mode', 'denyDelete'))]\"}},\"externalInputDefinitions\":{\"stack_setting_0\":{\"kind\":\"stack.setting\",\"config\":\"config\"},\"stack_setting_1\":{\"kind\":\"stack.setting\",\"config\":\"subscriptionId\"},\"stack_setting_2\":{\"kind\":\"stack.setting\",\"config\":\"resourceGroup\"},\"stack_setting_3\":{\"kind\":\"stack.setting\",\"config\":\"name\"}}}",
      "sourceUri": "${TEST_OUTPUT_DIR}/cluster/main.bicepparam",
      "body": {
        "region": "[parameters('config').regions[copyIndex()]]",
        "deploy": "onChange",
        "inputs": {
          "subscriptionId": "[__bicep.lookupSubscription(parameters('config').regions[copyIndex()])]",
          "resourceGroup": "[format('{0}-cluster-{1}', variables('prefix'), parameters('config').regions[copyIndex()])]",
          "name": "[format('{0}-cluster-{1}', variables('prefix'), parameters('config').regions[copyIndex()])]",
          "config": {
            "foo": "bar"
          }
        }
      }
    },
    "dependsOn": [
      "global"
    ]
  },
  "clusterApp": {
    "import": "az0synthesized",
    "copy": {
      "name": "clusterApp",
      "count": "[length(parameters('config').regions)]"
    },
    "type": "OrchestrationStack",
    "properties": {
      "template": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\"contentVersion\":\"1.0.0.0\",\"metadata\":{\"_generator\":{\"name\":\"bicep\",\"version\":\"dev\",\"templateHash\":\"12225932258748713702\"}},\"parameters\":{\"foo\":{\"type\":\"string\"}},\"resources\":[]}",
      "parameters": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\"contentVersion\":\"1.0.0.0\",\"parameters\":{\"foo\":{\"expression\":\"[externalInputs('stack_setting_0').foo]\"},\"$usingConfig\":{\"expression\":\"[createObject('mode', 'stack', 'scope', format('/subscriptions/{0}/resourceGroups/{1}', externalInputs('stack_setting_1'), externalInputs('stack_setting_2')), 'name', string(externalInputs('stack_setting_3')), 'actionOnUnmanage', createObject('resources', 'delete'), 'denySettings', createObject('mode', 'denyDelete'))]\"}},\"externalInputDefinitions\":{\"stack_setting_0\":{\"kind\":\"stack.setting\",\"config\":\"config\"},\"stack_setting_1\":{\"kind\":\"stack.setting\",\"config\":\"subscriptionId\"},\"stack_setting_2\":{\"kind\":\"stack.setting\",\"config\":\"resourceGroup\"},\"stack_setting_3\":{\"kind\":\"stack.setting\",\"config\":\"name\"}}}",
      "sourceUri": "${TEST_OUTPUT_DIR}/clusterApp/main.bicepparam",
      "body": {
        "region": "[parameters('config').regions[copyIndex()]]",
        "deploy": "always",
        "inputs": {
          "subscriptionId": "[__bicep.lookupSubscription(parameters('config').regions[copyIndex()])]",
          "resourceGroup": "[format('{0}-cluster-{1}', variables('prefix'), parameters('config').regions[copyIndex()])]",
          "name": "[format('{0}-cluster-app-{1}', variables('prefix'), parameters('config').regions[copyIndex()])]",
          "config": {
            "foo": "bar"
          }
        }
      }
    },
    "dependsOn": [
      "[format('cluster[{0}]', copyIndex())]"
    ]
  }
}
""");

        parameters.FromJson<JToken>().Should().HaveJsonAtPath("$.parameters", """
{
  "config": {
    "value": {
      "global": {
        "serviceTreeId": "67fa35d2-495a-4f32-aee6-9dac46f7ecce",
        "environment": "test",
        "infraRegion": "eastus",
        "infraPrefix": "orch"
      },
      "regional": {
        "eastus": {
          "shortName": "eus"
        },
        "westus": {
          "shortName": "wus"
        }
      },
      "regions": [
        "eastus",
        "westus"
      ],
      "stageMappings": [
        {
          "name": "stage0",
          "regions": [
            "eastus"
          ]
        },
        {
          "name": "stage1",
          "regions": [
            "westus"
          ]
        }
      ]
    }
  },
  "mode": {
    "value": "hotfix"
  }
}
""");

        // TODO needs some work to support baseline paths that differ from the source file paths
        // var paramsFile = baselineFolder.GetFileOrEnsureCheckedIn("main.parameters.json");
        // var templateFile = baselineFolder.GetFileOrEnsureCheckedIn("main.json");

        // paramsFile.WriteToOutputFolder(parameters!);
        // templateFile.WriteToOutputFolder(template!);

        // paramsFile.ShouldHaveExpectedJsonValue();
        // templateFile.ShouldHaveExpectedJsonValue();
    }
}