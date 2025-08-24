// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DirectResourceCollectionTests
    {
        [TestMethod]
        public void DirectResourceCollectionAccess_Basic()
        {
            // Symbolic names aren't explicitly enabled, but using this feature will enable them
            var result = CompilationHelper.Compile(CreateReferencesBicepContent());

            // NOTE(kylealbert): it's important that this test asserts no diagnostics for CreateReferencesBicepContent()
            // in relation to other tests that extend this content
            result.WithErrorDiagnosticsOnly()
                .Should()
                .NotHaveAnyDiagnostics();

            var template = result.Template;

            // inside properties
            template.Should()
                .HaveValueAtPath(
                    "$.resources.containerController.properties.containers[0].properties.command[0]",
                    "[format('echo \"{0}\"', join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ','))]");

            // inside properties via inlined variable
            template.Should()
                .HaveValueAtPath(
                    "$.resources.containerController.properties.containers[0].properties.command[1]",
                    "[format('echo \"{0}\"', join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ','))]");

            // output value: no outer expression
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.outputWithoutOuterExpression.value",
                    "[references('containerWorkers', 'full')]");

            // output value: with outer expression
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.outputWithOuterExpression.value",
                    "[join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ',')]");

            // output value: no outer expression; inlined
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.outputInlinedWithoutOuterExpression.value",
                    "[references('containerWorkers', 'full')]");

            // output value: with outer expression; inlined
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.outputInlinedWithOuterExpression.value",
                    "[join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ',')]");
        }

        [TestMethod]
        public void Accessing_resource_ids_with_lambda_is_supported()
        {
            var result = CompilationHelper.Compile("""
            resource foo 'Microsoft.Storage/storageAccounts@2025-01-01' existing = [for i in range(0, 10): {
              name: 'foosa${i}'
            }]

            output ids string[] = map(foo, x => x.id)
            """);

            result.WithErrorDiagnosticsOnly().Should().NotHaveAnyDiagnostics();
            result.Template.Should()
                .HaveValueAtPath(
                    "$.outputs.ids.value",
                    "[map(references('foo', 'full'), lambda('x', lambdaVariables('x').id))]");
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_Modules()
        {
            var result = CompilationHelper.Compile(CreateReferencesBicepContentWithModules());

            result.WithErrorDiagnosticsOnly()
                .Should()
                .NotHaveAnyDiagnostics();

            var template = result.Template;

            // module params
            template.Should()
                .HaveValueAtPath(
                    "$.resources.singleModule.properties.parameters.modInput1.value",
                    "[join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ',')]");

            // module params via inlined variable
            template.Should()
                .HaveValueAtPath(
                    "$.resources.singleModule.properties.parameters.modInput2.value",
                    "[join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ',')]");

            // output
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.modOutputWithOuterExpression.value",
                    "[join(map(references('multiModules'), lambda('m', lambdaVariables('m').outputs.modOutput1.value)), ',')]");

            // output via inlined variable
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.modOutputInlinedWithOuterExpression.value",
                    "[join(map(references('multiModules'), lambda('m', lambdaVariables('m').outputs.modOutput1.value)), ',')]");
        }

        [DataTestMethod]
        [DataRow("""
var loopVar = [for i in range(0, 2): {
  prop: map(containerWorkers, (w) => w.properties.ipAddress.ip)
}]
""")]
        [DataRow("""
output loopOutput array = [for i in range(0, 2): {
  prop: map(containerWorkers, (w) => w.properties.ipAddress.ip)
}]
""")]
        [DataRow("""
resource propertyLoop 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
  name: 'gh9440-loop'
  location: 'westus'
  properties: {
    containers: [for i in range(0, 2): {
      name: 'gh9440-w1c-${i}'
      properties: {
        command: [
          'echo "${join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')}"'
        ]
      }
    }]
  }
}
""")]
        public void DirectResourceCollectionAccess_NotAllowedWithinLoops(string additionalContent)
        {
            var result = CompilationHelper.Compile($"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression."),
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_NotAllowedWithinLoops_InlinedVariable()
        {
            const string additionalContent = """
var containerWorkersAliased = containerWorkers
var loopVar = [for i in range(0, 2): {
  prop: map(containerWorkersAliased, (w) => w.properties.ipAddress.ip)
}]
""";
            var result = CompilationHelper.Compile($"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. The collection was accessed by the chain of \"containerWorkersAliased\" -> \"containerWorkers\". Apply an array indexer to the expression."),
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_NotAllowedWithinUnsupportedResourceProperties()
        {
            const string additionalContent = """
resource containerController2 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
  name: 'gh9440-c2'
  tags: {
    prop: join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
  }
}
""";
            var result = CompilationHelper.Compile($"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression."),
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_NotAllowedWithinUnsupportedResourceProperties_InlinedVariable()
        {
            const string additionalContent = """
var containerWorkerIps = join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
resource propertyLoop 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
  name: 'gh9440-inlined'
  location: 'westus'
  tags: {
    test: containerWorkerIps
  }
  properties: {}
}
""";
            var result = CompilationHelper.Compile($"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. The collection was accessed by the chain of \"containerWorkerIps\" -> \"containerWorkers\". Apply an array indexer to the expression."),
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_NotAllowedWithinUnsupportedResourceProperties_DoubleInlinedVariable()
        {
            const string additionalContent = """
var containerWorkersAliased = containerWorkers
var containerWorkerIps = join(map(containerWorkersAliased, (w) => w.properties.ipAddress.ip), ',')
resource propertyLoop 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
  name: 'gh9440-inlined'
  location: 'westus'
  tags: {
    test: containerWorkerIps
  }
  properties: {}
}
""";

            var result = CompilationHelper.Compile($"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. The collection was accessed by the chain of \"containerWorkerIps\" -> \"containerWorkersAliased\" -> \"containerWorkers\". Apply an array indexer to the expression."),
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_NotAllowedWithinResourceCollection()
        {
            const string bicepContents = """
resource containerWorkers 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
    ipAddress: {
      type: 'Public'
      ports: [
        {
          port: 80
          protocol: 'TCP'
        }
      ]
    }
  }
}]

resource containerWorkers2 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
    containers: [
      {
        name: 'gh9440-w1c-${i}'
        properties: {
          command: [
            'echo "${join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')}"'
          ]
        }
      }
    ]
  }
}]
""";

            var result = CompilationHelper.Compile(bicepContents);

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. Apply an array indexer to the expression.")
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_NotAllowedWithinResourceCollection_InlinedVariable()
        {
            const string bicepContents = """
resource containerWorkers 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
    ipAddress: {
      type: 'Public'
      ports: [
        {
          port: 80
          protocol: 'TCP'
        }
      ]
    }
  }
}]

var ipAddresses = map(containerWorkers, (w) => w.properties.ipAddress.ip)
resource containerWorkers2 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
    containers: [
      {
        name: 'gh9440-w1c-${i}'
        properties: {
          command: [
            'echo "${join(ipAddresses, ',')}"'
          ]
        }
      }
    ]
  }
}]
""";

            var result = CompilationHelper.Compile(bicepContents);

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported here. The collection was accessed by the chain of \"ipAddresses\" -> \"containerWorkers\". Apply an array indexer to the expression.")
                    });
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_AllowedForExistingResourceCollection()
        {
            const string additionalContent = """
resource directAccessOfExisting 'Providers.Test/statefulResources@2014-04-01' = {
  name: 'dacoe'
  properties: {
    test: join(map(existingCollection, (r) => r.properties.test), ',')
  }
}
""";

            var result = CompilationHelper.Compile(CreateReferencesBicepContent() + "\n" + additionalContent);

            result.WithErrorDiagnosticsOnly()
                .Should()
                .NotHaveAnyDiagnostics();
        }

        private static string CreateReferencesBicepContent()
        {
            return """
resource containerWorkers 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
  }
}]

var ipAddresses = join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
var containerWorkersAlias = containerWorkers

resource containerController 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
   name: 'gh9440-c'
   dependsOn: [containerWorkers]
   properties: {
    containers: [
      {
        name: 'gh9440-cc'
        properties: {
          command: [
            'echo "${join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')}"'
            'echo "${ipAddresses}"'
          ]
        }
      }
    ]
  }
}

resource existingCollection 'Providers.Test/statefulResources@2014-04-01' existing = [for i in range(0, 2): {
  name: 'existing'
}]

output outputWithoutOuterExpression array = containerWorkers
output outputWithOuterExpression string = join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
output outputInlinedWithoutOuterExpression array = containerWorkersAlias
output outputInlinedWithOuterExpression string = ipAddresses
""";
        }

        private static (string fileName, string fileContents)[] CreateReferencesBicepContentWithModules()
        {
            return
            [
                ("main.bicep", """
resource containerWorkers 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
  }
}]

var ipAddresses = join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
var containerWorkersAlias = containerWorkers

resource containerController 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
   name: 'gh9440-c'
   dependsOn: [containerWorkers]
   properties: {
    containers: [
      {
        name: 'gh9440-cc'
        properties: {
          command: [
            'echo "${join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')}"'
            'echo "${ipAddresses}"'
          ]
        }
      }
    ]
  }
}

module singleModule 'mod1.bicep' = {
  name: 'singleModule'
  params: {
    modInput1: join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
    modInput2: ipAddresses
  }
}

module multiModules 'mod1.bicep' = [for i in range(0, 4): {
  name: 'multiModule${i}'
  params: {
    modInput1: 'input-${i}'
  }
}]

var moduleOutput1s = join(map(multiModules, (m) => m.outputs.modOutput1), ',')

output outputWithoutOuterExpression array = containerWorkers
output outputWithOuterExpression string = join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
output outputInlinedWithoutOuterExpression array = containerWorkersAlias
output outputInlinedWithOuterExpression string = ipAddresses
output modOutputWithOuterExpression string = join(map(multiModules, (m) => m.outputs.modOutput1), ',')
output modOutputInlinedWithOuterExpression string = join(map(multiModules, (m) => m.outputs.modOutput1), ',')
"""),
                ("mod1.bicep", """
param modInput1 string = 'unspecified'
param modInput2 string = 'unspecified'

resource storage 'Providers.Test/statefulResources@2014-04-01' = {
  name: 'gh9440-m1'
  location: resourceGroup().location
  properties: {
    prop1: modInput1
    prop2: modInput2
  }
}

output modOutput1 string = storage.properties.modInput1
""")
            ];
        }
    }
}
