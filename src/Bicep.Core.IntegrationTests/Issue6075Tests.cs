// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.IntegrationTests
{
    /// <summary>
    /// Fixing https://github.com/Azure/bicep/issues/6075 required major changes to how name expressions and classic dependencies
    /// are emitted, so that complexity warrants adding a special test class for the entire issue.
    /// </summary>
    [TestClass]
    public class Issue6075Tests
    {
        private const string OriginalReproBicep = @"
var numberOfAccounts = 2
var blobsPerAccount = 3
var saprefix = uniqueString(resourceGroup().id)

resource sa 'Microsoft.Storage/storageAccounts@2021-08-01' = [for i in range(0, numberOfAccounts): {
  name: '${saprefix}${i}'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]

resource blobSvc 'Microsoft.Storage/storageAccounts/blobServices@2021-08-01' = [for j in range(0, numberOfAccounts): {
  parent: sa[j]
  name: 'default'
}]

resource containers 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-08-01' = [for k in range(0, (numberOfAccounts * blobsPerAccount)): {
  parent: blobSvc[k % numberOfAccounts]
  name: 'container${k % blobsPerAccount}'
}]
";

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void OriginalReproTemplate_Classic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(OriginalReproBicep, false);
            result.Should().GenerateATemplate();

            /*
             The generated template was manually verified to be deployable.
             */
            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[0].copy.name", "sa");
                template.Should().HaveValueAtPath("$.resources[0].name", "[format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[copyIndex()])]");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].copy.name", "blobSvc");
                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[range(0, variables('numberOfAccounts'))[copyIndex()]]), 'default')]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray("[resourceId('Microsoft.Storage/storageAccounts', format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[range(0, variables('numberOfAccounts'))[copyIndex()]]))]"));

                template.Should().HaveValueAtPath("$.resources[2].copy.name", "containers");
                template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}/{2}', format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[range(0, variables('numberOfAccounts'))[mod(range(0, mul(variables('numberOfAccounts'), variables('blobsPerAccount')))[copyIndex()], variables('numberOfAccounts'))]]), 'default', format('container{0}', mod(range(0, mul(variables('numberOfAccounts'), variables('blobsPerAccount')))[copyIndex()], variables('blobsPerAccount'))))]");
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray("[resourceId('Microsoft.Storage/storageAccounts/blobServices', format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[range(0, variables('numberOfAccounts'))[mod(range(0, mul(variables('numberOfAccounts'), variables('blobsPerAccount')))[copyIndex()], variables('numberOfAccounts'))]]), 'default')]"));
            }
        }

        [TestMethod]
        public void OriginalReproTemplate_Symbolic_ShouldProduceExpectedExpressions()
        {
            CompilationHelper.CompilationResult result = Compile(OriginalReproBicep, true);
            result.Should().GenerateATemplate();

            /*
             The generated template was manually verified to be deployable.
             */
            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources.sa.copy.name", "sa");
                template.Should().HaveValueAtPath("$.resources.sa.name", "[format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[copyIndex()])]");
                template.Should().NotHaveValueAtPath("$.resources.sa.dependsOn");

                template.Should().HaveValueAtPath("$.resources.blobSvc.copy.name", "blobSvc");
                template.Should().HaveValueAtPath("$.resources.blobSvc.name", "[format('{0}/{1}', format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[range(0, variables('numberOfAccounts'))[copyIndex()]]), 'default')]");
                template.Should().HaveValueAtPath("$.resources.blobSvc.dependsOn", new JArray("[format('sa[{0}]', range(0, variables('numberOfAccounts'))[copyIndex()])]"));

                template.Should().HaveValueAtPath("$.resources.containers.copy.name", "containers");
                template.Should().HaveValueAtPath("$.resources.containers.name", "[format('{0}/{1}/{2}', format('{0}{1}', variables('saprefix'), range(0, variables('numberOfAccounts'))[range(0, variables('numberOfAccounts'))[mod(range(0, mul(variables('numberOfAccounts'), variables('blobsPerAccount')))[copyIndex()], variables('numberOfAccounts'))]]), 'default', format('container{0}', mod(range(0, mul(variables('numberOfAccounts'), variables('blobsPerAccount')))[copyIndex()], variables('blobsPerAccount'))))]");
                template.Should().HaveValueAtPath("$.resources.containers.dependsOn", new JArray("[format('blobSvc[{0}]', mod(range(0, mul(variables('numberOfAccounts'), variables('blobsPerAccount')))[copyIndex()], variables('numberOfAccounts')))]"));
            }
        }

        private const string AllIndexVariablesBicep = @"
resource vnet 'Microsoft.Network/virtualNetworks@2021-05-01' = [for (ii, i) in range(0, 2): {
  name: string(i)
}]

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-05-01' = [for (jj, j) in range(0, 6): {
  parent: vnet[j % 2]
  name: string(j)
}]

resource thing 'Microsoft.Network/virtualNetworks/subnets/things@2021-05-01' = [for (kk, k) in range(0, 24): {
  parent: subnet[k % 6]
  name: string(k)
}]
";

        [TestMethod]
        public void ThreeNestedResources_Classic_AllIndexVariables_ShouldProduceExpectedExpressions()
        {
            var result = Compile(AllIndexVariablesBicep, false);
            result.Should().GenerateATemplate();

            /*
             The generated template was manually verified to be deployable.
             */
            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[0].copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources[0].name", "[string(copyIndex())]");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', string(mod(copyIndex(), 2)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks', string(mod(copyIndex(), 2)))]"));

                template.Should().HaveValueAtPath("$.resources[2].copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}/{2}', string(mod(mod(copyIndex(), 6), 2)), string(mod(copyIndex(), 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks/subnets', string(mod(mod(copyIndex(), 6), 2)), string(mod(copyIndex(), 6)))]"));
            }
        }

        [TestMethod]
        public void ThreeNestedResources_Symbolic_AllIndexVariables_ShouldProduceExpectedExpressions()
        {
            var result = Compile(AllIndexVariablesBicep, true);
            result.Should().GenerateATemplate();

            /*
             The generated template was manually verified to be deployable.
             */
            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources.vnet.copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources.vnet.name", "[string(copyIndex())]");
                template.Should().NotHaveValueAtPath("$.resources.vnet.dependsOn");

                template.Should().HaveValueAtPath("$.resources.subnet.copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources.subnet.name", "[format('{0}/{1}', string(mod(copyIndex(), 2)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.subnet.dependsOn", new JArray("[format('vnet[{0}]', mod(copyIndex(), 2))]"));

                template.Should().HaveValueAtPath("$.resources.thing.copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources.thing.name", "[format('{0}/{1}/{2}', string(mod(mod(copyIndex(), 6), 2)), string(mod(copyIndex(), 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.thing.dependsOn", new JArray("[format('subnet[{0}]', mod(copyIndex(), 6))]"));
            }
        }

        private const string TopItemVariableBicep = @"
resource vnet 'Microsoft.Network/virtualNetworks@2021-05-01' = [for (ii, i) in range(0, 2): {
  name: string(ii)
}]

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-05-01' = [for (jj, j) in range(0, 6): {
  parent: vnet[j % 2]
  name: string(j)
}]

resource thing 'Microsoft.Network/virtualNetworks/subnets/things@2021-05-01' = [for (kk, k) in range(0, 24): {
  parent: subnet[k % 6]
  name: string(k)
}]
";

        [TestMethod]
        public void ThreeNestedResources_TopItemVariable_Classic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(TopItemVariableBicep, false);
            result.Should().GenerateATemplate();

            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[0].copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources[0].name", "[string(range(0, 2)[copyIndex()])]");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', string(range(0, 2)[mod(copyIndex(), 2)]), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks', string(range(0, 2)[mod(copyIndex(), 2)]))]"));

                template.Should().HaveValueAtPath("$.resources[2].copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}/{2}', string(range(0, 2)[mod(mod(copyIndex(), 6), 2)]), string(mod(copyIndex(), 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks/subnets', string(range(0, 2)[mod(mod(copyIndex(), 6), 2)]), string(mod(copyIndex(), 6)))]"));
            }
        }

        [TestMethod]
        public void ThreeNestedResources_TopItemVariable_Symbolic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(TopItemVariableBicep, true);
            result.Should().GenerateATemplate();

            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources.vnet.copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources.vnet.name", "[string(range(0, 2)[copyIndex()])]");
                template.Should().NotHaveValueAtPath("$.resources.vnet.dependsOn");

                template.Should().HaveValueAtPath("$.resources.subnet.copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources.subnet.name", "[format('{0}/{1}', string(range(0, 2)[mod(copyIndex(), 2)]), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.subnet.dependsOn", new JArray("[format('vnet[{0}]', mod(copyIndex(), 2))]"));

                template.Should().HaveValueAtPath("$.resources.thing.copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources.thing.name", "[format('{0}/{1}/{2}', string(range(0, 2)[mod(mod(copyIndex(), 6), 2)]), string(mod(copyIndex(), 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.thing.dependsOn", new JArray("[format('subnet[{0}]', mod(copyIndex(), 6))]"));
            }
        }

        private const string MiddleItemVariableBicep = @"
resource vnet 'Microsoft.Network/virtualNetworks@2021-05-01' = [for (ii, i) in range(0, 2): {
  name: string(i)
}]

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-05-01' = [for (jj, j) in range(0, 6): {
  parent: vnet[jj % 2]
  name: string(j)
}]

resource thing 'Microsoft.Network/virtualNetworks/subnets/things@2021-05-01' = [for (kk, k) in range(0, 24): {
  parent: subnet[k % 6]
  name: string(k)
}]
";

        [TestMethod]
        public void ThreeNestedResources_MiddleItemVariable_Classic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(MiddleItemVariableBicep, false);
            result.Should().GenerateATemplate();

            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[0].copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources[0].name", "[string(copyIndex())]");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', string(mod(range(0, 6)[copyIndex()], 2)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks', string(mod(range(0, 6)[copyIndex()], 2)))]"));

                template.Should().HaveValueAtPath("$.resources[2].copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}/{2}', string(mod(range(0, 6)[mod(copyIndex(), 6)], 2)), string(mod(copyIndex(), 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks/subnets', string(mod(range(0, 6)[mod(copyIndex(), 6)], 2)), string(mod(copyIndex(), 6)))]"));
            }
        }

        [TestMethod]
        public void ThreeNestedResources_MiddleItemVariable_Symbolic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(MiddleItemVariableBicep, true);
            result.Should().GenerateATemplate();

            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources.vnet.copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources.vnet.name", "[string(copyIndex())]");
                template.Should().NotHaveValueAtPath("$.resources.vnet.dependsOn");

                template.Should().HaveValueAtPath("$.resources.subnet.copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources.subnet.name", "[format('{0}/{1}', string(mod(range(0, 6)[copyIndex()], 2)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.subnet.dependsOn", new JArray("[format('vnet[{0}]', mod(range(0, 6)[copyIndex()], 2))]"));

                template.Should().HaveValueAtPath("$.resources.thing.copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources.thing.name", "[format('{0}/{1}/{2}', string(mod(range(0, 6)[mod(copyIndex(), 6)], 2)), string(mod(copyIndex(), 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.thing.dependsOn", new JArray("[format('subnet[{0}]', mod(copyIndex(), 6))]"));
            }
        }

        private const string BottomItemVariableBicep = @"
resource vnet 'Microsoft.Network/virtualNetworks@2021-05-01' = [for (ii, i) in range(0, 2): {
  name: string(i)
}]

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-05-01' = [for (jj, j) in range(0, 6): {
  parent: vnet[j % 2]
  name: string(j)
}]

resource thing 'Microsoft.Network/virtualNetworks/subnets/things@2021-05-01' = [for (kk, k) in range(0, 24): {
  parent: subnet[kk % 6]
  name: string(k)
}]
";

        [TestMethod]
        public void ThreeNestedResources_BottomItemVariable_Classic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(BottomItemVariableBicep, false);
            result.Should().GenerateATemplate();

            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[0].copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources[0].name", "[string(copyIndex())]");
                template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

                template.Should().HaveValueAtPath("$.resources[1].copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', string(mod(copyIndex(), 2)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks', string(mod(copyIndex(), 2)))]"));

                template.Should().HaveValueAtPath("$.resources[2].copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}/{2}', string(mod(mod(range(0, 24)[copyIndex()], 6), 2)), string(mod(range(0, 24)[copyIndex()], 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray("[resourceId('Microsoft.Network/virtualNetworks/subnets', string(mod(mod(range(0, 24)[copyIndex()], 6), 2)), string(mod(range(0, 24)[copyIndex()], 6)))]"));
            }
        }

        [TestMethod]
        public void ThreeNestedResources_BottomItemVariable_Symbolic_ShouldProduceExpectedExpressions()
        {
            var result = Compile(BottomItemVariableBicep, true);
            result.Should().GenerateATemplate();

            var template = result.Template;
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources.vnet.copy.name", "vnet");
                template.Should().HaveValueAtPath("$.resources.vnet.name", "[string(copyIndex())]");
                template.Should().NotHaveValueAtPath("$.resources.vnet.dependsOn");

                template.Should().HaveValueAtPath("$.resources.subnet.copy.name", "subnet");
                template.Should().HaveValueAtPath("$.resources.subnet.name", "[format('{0}/{1}', string(mod(copyIndex(), 2)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.subnet.dependsOn", new JArray("[format('vnet[{0}]', mod(copyIndex(), 2))]"));

                template.Should().HaveValueAtPath("$.resources.thing.copy.name", "thing");
                template.Should().HaveValueAtPath("$.resources.thing.name", "[format('{0}/{1}/{2}', string(mod(mod(range(0, 24)[copyIndex()], 6), 2)), string(mod(range(0, 24)[copyIndex()], 6)), string(copyIndex()))]");
                template.Should().HaveValueAtPath("$.resources.thing.dependsOn", new JArray("[format('subnet[{0}]', mod(range(0, 24)[copyIndex()], 6))]"));
            }
        }

        private CompilationHelper.CompilationResult Compile(string bicep, bool symbolicNameCodegenEnabled)
        {
            var services = new ServiceBuilder().WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(this.TestContext, symbolicNameCodegenEnabled: symbolicNameCodegenEnabled));

            return CompilationHelper.Compile(services, bicep);
        }
    }
}
