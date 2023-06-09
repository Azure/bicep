// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ReferencesFunctionTests
    {
        private static ServiceBuilder NewServiceBuilder(bool isSymbolicNameCodegenEnabled) => new ServiceBuilder()
            .WithFeatureOverrides(new FeatureProviderOverrides { SymbolicNameCodegenEnabled = isSymbolicNameCodegenEnabled });

        private static readonly string referencesBicepContents1 = """
resource containerWorkers 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = [for i in range(0, 4): {
  name: 'gh9440-w1-${i}'
  location: 'westus'
  properties: {
    containers: [
      {
        name: 'gh9440-w1c-${i}'
        properties: {
          image: 'mcr.microsoft.com/azuredocs/aci-helloworld'
          ports: [
            {
              port: 80
              protocol: 'TCP'
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 2
            }
          }
        }
      }
    ]
    osType: 'Linux'
    restartPolicy: 'Always'
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
          ]
          image: 'mcr.microsoft.com/azuredocs/aci-helloworld'
          ports: [
            {
              port: 80
              protocol: 'TCP'
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 2
            }
          }
        }
      }
    ]
    osType: 'Linux'
    restartPolicy: 'Always'
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
}
""";

        [TestMethod]
        public void ReferencesFunction_NonSymbolic_Basic()
        {
            var result = CompilationHelper.Compile(referencesBicepContents1);

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var template = result.Template;

            template.Should().HaveValueAtPath(
                "$.resources[?(@.name == 'gh9440-c')].properties.containers[0].properties.command[0]",
                "[format('echo \"{0}\"', join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ','))]");
        }

        [TestMethod]
        public void ReferencesFunction_Basic()
        {
            var result = CompilationHelper.Compile(NewServiceBuilder(isSymbolicNameCodegenEnabled: true), referencesBicepContents1);

            result.ExcludingLinterDiagnostics()
                .Should()
                .NotHaveAnyDiagnostics();

            var template = result.Template;

            template.Should()
                .HaveValueAtPath(
                    "$.resources.containerController.properties.containers[0].properties.command[0]",
                    "[format('echo \"{0}\"', join(map(references('containerWorkers', 'full'), lambda('w', lambdaVariables('w').properties.ipAddress.ip)), ','))]");
        }
    }
}
