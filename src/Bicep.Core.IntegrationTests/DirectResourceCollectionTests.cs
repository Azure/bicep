// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DirectResourceCollectionTests
    {
        [TestMethod]
        public void DirectResourceCollectionAccess_NonSymbolic_Basic()
        {
            var result = CompilationHelper.Compile(CreateReferencesBicepContent());

            result.WithErrorDiagnosticsOnly()
                .Should()
                .NotHaveAnyDiagnostics();

            var template = result.Template;

            template.Should()
                .HaveValueAtPath(
                    "$.resources[?(@.name == 'gh9440-c')].properties.containers[0].properties.command[0]",
                    "[format('echo \"{0}\"', join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ','))]");

            // inlined variable
            template.Should()
                .HaveValueAtPath(
                    "$.resources[?(@.name == 'gh9440-c')].properties.containers[0].properties.command[1]",
                    "[format('echo \"{0}\"', join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ','))]");
        }

        [TestMethod]
        public void DirectResourceCollectionAccess_Basic()
        {
            var result = CompilationHelper.Compile(NewServiceBuilder(isSymbolicNameCodegenEnabled: true), CreateReferencesBicepContent());

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

            // output value
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.o1.value",
                    "[join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ',')]");

            // output value via inlined variable
            template.Should()
                .HaveValueAtPath(
                    "$.outputs.o2.value",
                    "[join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ',')]");
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
            var result = CompilationHelper.Compile(NewServiceBuilder(isSymbolicNameCodegenEnabled: true), $"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression."),
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
            var result = CompilationHelper.Compile(NewServiceBuilder(isSymbolicNameCodegenEnabled: true), $"{CreateReferencesBicepContent()}\n{additionalContent}");

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression."),
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

            var result = CompilationHelper.Compile(NewServiceBuilder(isSymbolicNameCodegenEnabled: true), bicepContents);

            result.WithErrorDiagnosticsOnly()
                .Should()
                .HaveDiagnostics(
                    new[]
                    {
                        ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression.")
                    });
        }

        private static ServiceBuilder NewServiceBuilder(bool isSymbolicNameCodegenEnabled) => new ServiceBuilder()
            .WithFeatureOverrides(new FeatureProviderOverrides { SymbolicNameCodegenEnabled = isSymbolicNameCodegenEnabled });

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

output o1 string = join(map(containerWorkers, (w) => w.properties.ipAddress.ip), ',')
output o2 string = ipAddresses
""";
        }
    }
}
