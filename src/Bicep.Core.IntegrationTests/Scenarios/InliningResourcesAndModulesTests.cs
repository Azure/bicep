// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    /// <summary>
    ///  https://github.com/Azure/bicep/issues/1545
    /// </summary>
    [TestClass]
    public class InliningResourcesAndModulesTests
    {
        [TestMethod]
        public void AssigningResourceToVariable_ShouldNotGenerateVariables()
        {
            var result = CompilationHelper.Compile(@"
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var ref = storage

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: ref.properties.allowBlobPublicAccess
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                        "[resourceId('Microsoft.Storage/storageAccounts', 'test')]"
                    },
                    "Referenced resource should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[reference(resourceId('Microsoft.Storage/storageAccounts', 'test'), '2019-06-01', 'full').properties.allowBlobPublicAccess]",
                    "Resource access should be in-lined");
            }
        }


        [TestMethod]
        public void AssigningResourceToVariable_ShouldNotGenerateVariables_ChainedVariables()
        {
            var result = CompilationHelper.Compile(@"
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var ref4 = ref3
var ref3 = ref2
var ref2 = ref1
var ref1 = storage

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: ref4.properties.allowBlobPublicAccess
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                "[resourceId('Microsoft.Storage/storageAccounts', 'test')]"
                    },
                    "Referenced resource should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[reference(resourceId('Microsoft.Storage/storageAccounts', 'test'), '2019-06-01', 'full').properties.allowBlobPublicAccess]",
                    "Resource access should be in-lined");
            }
        }

        [TestMethod]
        public void AssigningResourceToVariable_ShouldNotGenerateVariables_Condition()
        {
            var result = CompilationHelper.Compile(@"
param mode int = 0

resource storage1_1 'Microsoft.Storage/storageAccounts@2019-06-01' = if (mode == 1) {
  name: 'test11'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
resource storage1_2 'Microsoft.Storage/storageAccounts@2019-06-01' = if (mode != 1) {
  name: 'test12'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var refResource = mode == 1 ? storage1_1 : storage1_2

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: refResource.properties.allowBlobPublicAccess
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                "[resourceId('Microsoft.Storage/storageAccounts', 'test11')]",
                "[resourceId('Microsoft.Storage/storageAccounts', 'test12')]"
                    },
                    "Referenced resource should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[if(equals(parameters('mode'), 1), reference(resourceId('Microsoft.Storage/storageAccounts', 'test11'), '2019-06-01', 'full'), reference(resourceId('Microsoft.Storage/storageAccounts', 'test12'), '2019-06-01', 'full')).properties.allowBlobPublicAccess]",
                    "Resource access should be in-lined");
            }
        }

        [TestMethod]
        public void AssigningResourceToVariable_ShouldNotGenerateVariables_ChainedCondition()
        {
            var result = CompilationHelper.Compile(@"
param mode int = 0

resource storage1_1 'Microsoft.Storage/storageAccounts@2019-06-01' = if (mode == 1) {
  name: 'test11'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
resource storage1_2 'Microsoft.Storage/storageAccounts@2019-06-01' = if (mode != 1) {
  name: 'test12'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var refResource = ref3
var ref1 = storage1_1
var ref2 = storage1_2
var ref3 = mode == 1 ? ref1 : ref2

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: refResource.properties.allowBlobPublicAccess
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                "[resourceId('Microsoft.Storage/storageAccounts', 'test11')]",
                "[resourceId('Microsoft.Storage/storageAccounts', 'test12')]"
                    },
                    "Referenced resource should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[if(equals(parameters('mode'), 1), reference(resourceId('Microsoft.Storage/storageAccounts', 'test11'), '2019-06-01', 'full'), reference(resourceId('Microsoft.Storage/storageAccounts', 'test12'), '2019-06-01', 'full')).properties.allowBlobPublicAccess]",
                    "Resource access should be in-lined");
            }
        }

        [TestMethod]
        public void AssigningModuleToVariable_ShouldNotGenerateVariables()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
module mod 'module.bicep' = {
  name: 'testmod'
}

var refMod = mod

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
   allowBlobPublicAccess: refMod.outputs.test
  }
}

"), ("module.bicep", @"
output test bool = true
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                        "[resourceId('Microsoft.Resources/deployments', 'testmod')]"
                    },
                    "Module should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[reference(resourceId('Microsoft.Resources/deployments', 'testmod'), '2019-10-01').outputs.test.value]",
                    "Module access should be in-lined correctly");
            }
        }

        [TestMethod]
        public void AssigningModuleToVariable_ShouldNotGenerateVariables_ChainedVariables()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
module mod 'module.bicep' = {
  name: 'testmod'
}

var refMod = mod

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
   allowBlobPublicAccess: refMod.outputs.test
  }
}

"), ("module.bicep", @"
output test bool = true
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                        "[resourceId('Microsoft.Resources/deployments', 'testmod')]"
                    },
                    "Module should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[reference(resourceId('Microsoft.Resources/deployments', 'testmod'), '2019-10-01').outputs.test.value]",
                    "Module access should be in-lined correctly");
            }
        }

        [TestMethod]
        public void AssigningModuleToVariable_ShouldNotGenerateVariables_Condition()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"

param mode int = 0
module mod1 'module.bicep' = if (mode == 1) {
  name: 'testmod1'
}

module mod2 'module.bicep' = if (mode != 1) {
  name: 'testmod2'
}

var refMod = mode == 1 ? mod1 : mod2

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
   allowBlobPublicAccess: refMod.outputs.test
  }
}

"), ("module.bicep", @"
output test bool = true
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                        "[resourceId('Microsoft.Resources/deployments', 'testmod1')]",
                        "[resourceId('Microsoft.Resources/deployments', 'testmod2')]"
                    },
                    "Module should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[if(equals(parameters('mode'), 1), reference(resourceId('Microsoft.Resources/deployments', 'testmod1'), '2019-10-01').outputs, reference(resourceId('Microsoft.Resources/deployments', 'testmod2'), '2019-10-01').outputs).test.value]",
                    "Module access should be in-lined correctly");
            }
        }

        [TestMethod]
        public void AssigningModuleToVariable_ShouldNotGenerateVariables_ConditionChained()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"

param mode int = 0
module mod1 'module.bicep' = if (mode == 1) {
  name: 'testmod1'
}

module mod2 'module.bicep' = if (mode != 1) {
  name: 'testmod2'
}

var refMod = ref3
var ref3 = mode == 1 ? ref1 : ref2
var ref1 = mod1
var ref2 = mod2

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
   allowBlobPublicAccess: refMod.outputs.test
  }
}

"), ("module.bicep", @"
output test bool = true
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().NotHaveValueAtPath("$.variables", "variable should not be generated");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    new JArray
                    {
                        "[resourceId('Microsoft.Resources/deployments', 'testmod1')]",
                        "[resourceId('Microsoft.Resources/deployments', 'testmod2')]"
                    },
                    "Module should be added to depends on section");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[if(equals(parameters('mode'), 1), reference(resourceId('Microsoft.Resources/deployments', 'testmod1'), '2019-10-01').outputs, reference(resourceId('Microsoft.Resources/deployments', 'testmod2'), '2019-10-01').outputs).test.value]",
                    "Module access should be in-lined correctly");
            }
        }


        [TestMethod]
        public void VariableThatLooksLikeModule_ShouldGenerateVariables()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"

param mode int = 0
var mod1 = {
  outputs: {
    test: false
  }
}

var mod2 = {
  outputs: {
    test: true
  }
}

var refMod = ref3
var ref3 = mode == 1 ? ref1 : ref2
var ref1 = mod1
var ref2 = mod2

resource storage2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test2'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
   allowBlobPublicAccess: refMod.outputs.test
  }
}

"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.variables", new JObject
                {
                    ["mod1"] = new JObject { ["outputs"] = new JObject { ["test"] = false } },
                    ["mod2"] = new JObject { ["outputs"] = new JObject { ["test"] = true } },
                    ["refMod"] = "[variables('ref3')]",
                    ["ref3"] = "[if(equals(parameters('mode'), 1), variables('ref1'), variables('ref2'))]",
                    ["ref1"] = "[variables('mod1')]",
                    ["ref2"] = "[variables('mod2')]"

                }, "variable should be generated");
                result.Template.Should().NotHaveValueAtPath("$.resources[?(@.name == 'test2')].dependsOn",
                    "DependsOn should be empty");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'test2')].properties.allowBlobPublicAccess",
                    "[variables('refMod').outputs.test]",
                    "Variable should be accessed correctly");
            }
        }

        [TestMethod]
        public void VariablesWithResourceId_ShouldNotBeInlined()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"

resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

var storageId = storage.id

output id string = storageId
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.variables", new JObject
                {
                    ["storageId"] = "[resourceId('Microsoft.Storage/storageAccounts', 'test')]"

                }, "variable should be generated");
                result.Template.Should().HaveValueAtPath("$.outputs.id.value",
                    "[variables('storageId')]",
                    "Variable should be accessed correctly");
            }
        }
    }
}
