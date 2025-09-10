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

        template.FromJson<JToken>().Should().HaveJsonAtPath("$.resources", """
{
  "global": {
    "import": "az0synthesized",
    "type": "OrchestrationStack",
    "properties": {
      "template": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\"contentVersion\":\"1.0.0.0\",\"metadata\":{\"_generator\":{\"name\":\"bicep\",\"version\":\"dev\",\"templateHash\":\"12740863314443779634\"}},\"resources\":[]}",
      "parameters": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\"contentVersion\":\"1.0.0.0\",\"parameters\":{}}"
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
      "template": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\"contentVersion\":\"1.0.0.0\",\"metadata\":{\"_generator\":{\"name\":\"bicep\",\"version\":\"dev\",\"templateHash\":\"12740863314443779634\"}},\"resources\":[]}",
      "parameters": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\"contentVersion\":\"1.0.0.0\",\"parameters\":{}}"
    }
  },
  "clusterApp": {
    "import": "az0synthesized",
    "copy": {
      "name": "clusterApp",
      "count": "[length(parameters('config').regions)]"
    },
    "type": "OrchestrationStack",
    "properties": {
      "template": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\"contentVersion\":\"1.0.0.0\",\"metadata\":{\"_generator\":{\"name\":\"bicep\",\"version\":\"dev\",\"templateHash\":\"12740863314443779634\"}},\"resources\":[]}",
      "parameters": "{\"$schema\":\"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\",\"contentVersion\":\"1.0.0.0\",\"parameters\":{}}"
    }
  }
}
""");

        parameters.FromJson<JToken>().Should().HaveJsonAtPath("$.parameters", """
{
  "config": {
    "value": {
      "global": {
        "serviceTreeId": "..."
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