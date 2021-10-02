// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Xml.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
        [TestMethod]
        // https://github.com/azure/bicep/issues/3636
        public void Test_Issue3636()
        {
            var lineCount = 100; // increase this number to 10,000 for more intense test

            // use this crypto random number gen to avoid CI warning
            int generateRandomInt(int minVal = 0, int maxVal = 50)
            {
                var rnd = new byte[4];
                using var rng = new RNGCryptoServiceProvider();
                rng.GetBytes(rnd);
                var i = Math.Abs(BitConverter.ToInt32(rnd, 0));
                return Convert.ToInt32(i % (maxVal - minVal + 1) + minVal);
            }
            Random random = new Random();

            string randomString()
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, generateRandomInt())
                  .Select(s => s[generateRandomInt(0, s.Length-1)]).ToArray());
            }

            var file = "param adminuser string\nvar adminstring = 'xyx ${adminuser} 123'\n";
            file += "output values object = {\n";
            for (var i = 0; i < lineCount; i++)
            {
                file += $"  testa{i}: '{randomString()} ${{adminuser}} {randomString()}'\n";
                file += $"  testb{i}: '{randomString()} ${{adminstring}} {randomString()}'\n";
            }
            file += "}\n";

            // not a true test for existing diagnostics
            // this is a trigger to allow timing within the
            // linter rules - timing must be readded to
            // initially added to time NoHardcodedEnvironmentUrlsRule
            CompilationHelper.Compile(file).Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/746
        public void Test_Issue746()
        {
            var result = CompilationHelper.Compile(@"
var l = l
param l
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                (NoUnusedParametersRule.Code, DiagnosticLevel.Warning, new NoUnusedParametersRule().GetMessage("l")),
                ("BCP014", DiagnosticLevel.Error, "Expected a parameter type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/801
        public void Test_Issue801()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
    location: 'eastus'
    name: 'vnet-rg'
}

module vnet './vnet.bicep' = {
    scope: resourceGroup('vnet-rg')
    name: 'network-module'
    params: {
        location: 'eastus'
        name: 'myVnet'
    }
    dependsOn: [
        rg
    ]
}

output vnetid string = vnet.outputs.vnetId
output vnetstate string = vnet.outputs.vnetstate
"),
                ("vnet.bicep", @"
param location string
param name string

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
    location: location
    name: name
    properties:{
        addressSpace: {
            addressPrefixes: [
                '10.0.0.0/20'
            ]
        }
        subnets: [
            {
                name: 'snet-apim'
                properties: {
                    addressPrefix: '10.0.0.0/24'
                }
            }
        ]
    }
}

output vnetId string = vnet.id
output vnetstate string = vnet.properties.provisioningState
"));

            result.Should().NotHaveAnyDiagnostics();
            // ensure we're generating the correct expression with 'subscriptionResourceId', and using the correct name for the module
            result.Template.Should().HaveValueAtPath("$.outputs['vnetid'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2020-06-01').outputs.vnetId.value]");
            result.Template.Should().HaveValueAtPath("$.outputs['vnetstate'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2020-06-01').outputs.vnetstate.value]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/982
        public void Test_Issue982()
        {
            var result = CompilationHelper.Compile(@"
param functionApp object
param serverFarmId string

output config object = list(format('{0}/config/appsettings', functionAppResource.id), functionAppResource.apiVersion)

resource functionAppResource 'Microsoft.Web/sites@2020-06-01' = {
  name: functionApp.name
  location: resourceGroup().location
  kind: 'functionApp'
  properties: {
    httpsOnly: true
    serverFarmId: serverFarmId
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['config'].value", "[list(format('{0}/config/appsettings', resourceId('Microsoft.Web/sites', parameters('functionApp').name)), '2020-06-01')]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1093
        public void Test_Issue1093()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'managementGroup'

module bicep3rg 'resourceGroup.bicep' = {
  name: 'rg30'
  params: {
    rgName: 'bicep3-rg'
  }
  scope: subscription('DEV1')
}
module bicep4rg 'resourceGroup.bicep' = {
  name: 'rg31'
  params: {
    rgName: 'bicep4-rg'
  }
  scope: subscription('DEV2')
}
"),
                ("resourceGroup.bicep", @"
param rgName string
param location string = 'westeurope'

targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: rgName
  location: location
}
"));

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'rg30')].location", "[deployment().location]");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'rg31')].location", "[deployment().location]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1173
        public void Test_Issue1173()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'subscription'

param azRegion string = 'southcentralus'
param vNetAddressPrefix string = '10.1.0.0/24'
param GatewayAddressPrefix string = '10.1.0.0/27'
param AppAddressPrefix string = '10.1.0.128/26'

var rgName = 'testRG'
var vnetName = 'testvnet'
var appNSGName = 'testvnet-appsn01nsg'
var appRTName = 'testvnet-appsn01routetable'

var subnets = [
  {
    name: 'GatewaySubnet'
    properties: {
      addressPrefix: GatewayAddressPrefix
    }
  }

  {
    name: 'appsn01'
    properties: {
      addressPrefix: AppAddressPrefix
      networkSecurityGroup: {
        id: appNSG.outputs.id
      }
      routeTable: {
        id: appRT.outputs.id
      }
    }
  }
]

resource rgVNet 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: rgName
  location: azRegion
}

module vnet './vnet.bicep' = {
  name: vnetName
  scope: resourceGroup(rgVNet.name)
  params: {
    vnetName: vnetName
    vNetAddressPrefix: vNetAddressPrefix
    subnets: subnets
  }
}

module appNSG './nsg.bicep' = {
  name: appNSGName
  scope: resourceGroup(rgVNet.name)
  params: {
    nsgName: appNSGName
    secRules: [
      {
        name: 'default-allow-rdp'
        properties: {
          priority: 1010
          access: 'Allow'
          direction: 'Inbound'
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: 'VirtualNetwork'
          destinationAddressPrefix: '*'
          destinationPortRange: '3389'
        }
      }
    ]
  }
}

module appRT './rt.bicep' = {
  name: appRTName
  scope: resourceGroup(rgVNet.name)
  params: {
    rtName: appRTName
  }
}
"),
                ("vnet.bicep", @"
param vnetName string
param vNetAddressPrefix string
param subnets array

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: resourceGroup().location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vNetAddressPrefix
      ]
    }
    enableVmProtection: false
    enableDdosProtection: false
    subnets: subnets
  }
}
output id string = vnet.id
"),
                ("nsg.bicep", @"
param nsgName string
param secRules array

resource nsg  'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: nsgName
  location: resourceGroup().location
  properties: {
    securityRules: secRules
  }
}
output id string = nsg.id
"),
                ("rt.bicep", @"
param rtName string
//param azFwlIp string

resource routetable 'Microsoft.Network/routeTables@2020-06-01' = {
  name: rtName
  location: resourceGroup().location
  properties: {
    disableBgpRoutePropagation: false
    routes: [
      // {
      //   name: 'DefaultRoute'
      //   properties: {
      //     addressPrefix: '0.0.0.0/0'
      //     nextHopType: 'VirtualAppliance'
      //     nextHopIpAddress: azFwlIp
      //   }
      // }
    ]
  }
}

output id string = routetable.id
"));

            // variable 'subnets' should have been inlined
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value[0].name", "GatewaySubnet");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value[1].name", "appsn01");
            // there should be no definition in the variables list for 'subnets'
            result.Template.Should().NotHaveValueAtPath("$.variables.subnets");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1185
        public void Test_Issue1185()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'tenant'

param allUpMgName string

module allup_mg './modules/rblab-allup-mg-policies.bicep' = {
  name: 'allupmgdeploy'
  scope: managementGroup(allUpMgName)
  params: {
    mgName: allUpMgName
  }
}
"),
                ("modules/rblab-allup-mg-policies.bicep", @"
targetScope = 'managementGroup'

param mgName string
"));

            result.Should().NotHaveAnyDiagnostics();
            // deploying a management group module at tenant scope requires an unqualified resource id
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'allupmgdeploy')].scope", "[format('Microsoft.Management/managementGroups/{0}', parameters('allUpMgName'))]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1332
        public void Test_Issue1332()
        {
            var result = CompilationHelper.Compile(@"
var propname = 'ptest'
var issue = true ? {
    prop1: {
        '${propname}': {}
    }
} : {}
");

            result.Should().HaveDiagnostics(new[] {
                    (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("issue"))
                });
            result.Template.Should().HaveValueAtPath("$.variables.issue", "[if(true(), createObject('prop1', createObject(format('{0}', variables('propname')), createObject())), createObject())]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/486
        public void Test_Issue486()
        {
            var result = CompilationHelper.Compile(@"
var myInt = 5
var myBigInt = 2199023255552
var myIntExpression = 5 * 5
var myBigIntExpression = 2199023255552 * 2
var myBigIntExpression2 = 2199023255552 * 2199023255552
");

            result.Template.Should().HaveValueAtPath("$.variables.myInt", 5);
            result.Template.Should().HaveValueAtPath("$.variables.myBigInt", 2199023255552);
            result.Template.Should().HaveValueAtPath("$.variables.myIntExpression", "[mul(5, 5)]");
            result.Template.Should().HaveValueAtPath("$.variables.myBigIntExpression2", "[mul(json('2199023255552'), json('2199023255552'))]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1362
        public void Test_Issue1362_1()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'resourceGroup'

module sub './modules/subscription.bicep' = {
  name: 'subDeploy'
  scope: subscription()
}"),
                ("modules/subscription.bicep", @"
targetScope = 'subscription'
"));

            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].subscriptionId", "[subscription().subscriptionId]");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].location", "[resourceGroup().location]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1362
        public void Test_Issue1362_2()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'resourceGroup'

module sub './modules/subscription.bicep' = {
  name: 'subDeploy'
  scope: subscription('abcd-efgh')
}"),
                ("modules/subscription.bicep", @"
targetScope = 'subscription'
"));

            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].subscriptionId", "abcd-efgh");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].location", "[resourceGroup().location]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1402
        public void Test_Issue1402()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'subscription'

module sub './modules/resourceGroup.bicep' = {
  name: 'subDeploy'
  scope: resourceGroup('abcd-efgh','bicep-rg')
}"),
                ("modules/resourceGroup.bicep", @"
targetScope = 'resourceGroup'
"));

            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].subscriptionId", "abcd-efgh");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].resourceGroup", "bicep-rg");
            result.Template.Should().NotHaveValueAtPath("$.resources[?(@.name == 'subDeploy')].location");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1391
        public void Test_Issue1391()
        {
            var result = CompilationHelper.Compile(@"
resource dep 'Microsoft.Resources/deployments@2020-06-01' = {
  name: 'nestedDeployment'
  resourceGroup: 'bicep-rg'
  subscriptionId: 'abcd-efgh'
  location: 'westus'
  properties: {
    mode: 'Incremental'
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'nestedDeployment')].subscriptionId", "abcd-efgh");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'nestedDeployment')].resourceGroup", "bicep-rg");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1454
        public void Test_Issue1454()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'subscription'
param name string = 'name'
param location string = 'westus'
param subscriptionId string = newGuid()

module rg './resourcegroup.template.bicep' = {
    name: '${uniqueString(deployment().name)}-1'
    scope: subscription(subscriptionId)
    params: {
        name: name
        location: location
    }
}

var appResGrp = resourceGroup(rg.outputs.resourceGroupName)

module redis './redis.template.bicep' = {
    name: '${uniqueString(deployment().name)}-2'
    scope: appResGrp
}
"),
                ("resourcegroup.template.bicep", @"
targetScope = 'subscription'
param name string
param location string

resource rg 'Microsoft.Resources/resourceGroups@2018-05-01' = {
    name: name
    location: location
    tags: {
        'owner': 'me'
    }
}

output resourceGroupName string = rg.name
"),
                ("redis.template.bicep", @"
param redis_name string = 'redis'
param redis_location string = 'westus'

resource redis 'Microsoft.Cache/Redis@2019-07-01' = {
    name: redis_name
    location: redis_location
    properties: {
        sku: {
            name: 'Standard'
            family: 'C'
            capacity: 2
        }
    }
}
"));

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"scope\" property of the \"module\" type, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"appResGrp\" -> \"rg\"). Properties of rg which can be calculated at the start include \"name\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1465
        public void Test_Issue1465()
        {
            var result = CompilationHelper.Compile(@"
resource foo 'Microsoft.foo/bar@2020-01-01' existing = {
  name: 'name'
}
output prop1 string = foo.properties.prop1
");

            result.Template.Should().HaveValueAtPath("$.outputs['prop1'].value", "[reference(resourceId('Microsoft.foo/bar', 'name'), '2020-01-01').prop1]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/822
        public void Test_Issue822()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'subscription'

resource myRg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  location: 'eastus'
  name: 'rg'
}

module vnetmodule './vnet.bicep' = {
  scope: myRg
  name: 'vnet'
  params: {
    location: 'eastus'
    name: 'myVnet'
  }
}
"),
                ("vnet.bicep", @"
param location string
param name string
"));

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().NotHaveValueAtPath("$.resources[?(@.name == 'vnet')].subscriptionId");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].resourceGroup", "rg");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].dependsOn[0]", "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'rg')]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/822
        public void Test_Issue822_scoped()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource myRg 'Microsoft.Resources/resourceGroups@2020-06-01' existing = {
  scope: subscription('abcdef')
  name: 'rg'
}

module vnetmodule './vnet.bicep' = {
  scope: myRg
  name: 'vnet'
  params: {
    location: 'eastus'
    name: 'myVnet'
  }
}
"),
                ("vnet.bicep", @"
param location string
param name string
"));

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].subscriptionId", "abcdef");
            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].resourceGroup", "rg");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1388
        public void Test_Issue1388()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'subscription'

param rgName string
param location string = deployment().location

param groupOwnerId string
param groupContributorId string
param groupReaderId string

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: rgName
  location: location
}

resource rgOwner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: '${guid(rg.name, 'owner')}'
  scope: rg
  properties: {
    roleDefinitionId: '${subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '8e3af657-a8ff-443c-a75c-2fe8c4bcb635')}'
    principalId: groupOwnerId
    principalType: 'Group'
  }
}

resource rgContributor 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: '${guid(rg.name, 'contributor')}'
  scope: rg
  properties: {
    roleDefinitionId: '${subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')}'
    principalId: groupContributorId
    principalType: 'Group'
  }
}

resource rgReader 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: '${guid(rg.name, 'reader')}'
  scope: rg
  properties: {
    roleDefinitionId: '${subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'acdd72a7-3385-48ef-bd42-f606fba81ae7')}'
    principalId: groupReaderId
    principalType: 'Group'
  }
}
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
                ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
                ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1364
        public void Test_Issue1364()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'blablah'
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'\" but the provided value is of type \"'blablah'\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/569
        public void Test_Issue569_success()
        {
            var result = CompilationHelper.Compile(@"
param myparam string
var myvar = 'hello'

output myparam string = myparam
output myvar string = myvar
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['myparam'].value", "[parameters('myparam')]");
            result.Template.Should().HaveValueAtPath("$.outputs['myvar'].value", "[variables('myvar')]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/569
        public void Test_Issue569_duplicates()
        {
            var result = CompilationHelper.Compile(@"
output duplicate string = 'hello'
output duplicate string = 'hello'
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP145", DiagnosticLevel.Error, "Output \"duplicate\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP145", DiagnosticLevel.Error, "Output \"duplicate\" is declared multiple times. Remove or rename the duplicates."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/569
        public void Test_Issue569_outputs_cannot_be_referenced()
        {
            var result = CompilationHelper.Compile(@"
output output1 string = 'hello'
output output2 string = output1
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP058", DiagnosticLevel.Error, "The name \"output1\" is an output. Outputs cannot be referenced in expressions."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1599
        public void Test_Issue1599()
        {
            var result = CompilationHelper.Compile(@"
param x string = 't'
output xx = x
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP146", DiagnosticLevel.Error, "Expected an output type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1661
        public void Test_Issue1661()
        {
            // Issue 1661 only repros if global-resources.bicep exists and kevault-secrets.bicep does not
            var result = CompilationHelper.Compile(("main.bicep", @"
targetScope = 'subscription'

param prefix string
param deploymentId string
param tags object

param stampLocations array = [
  'northeurope'
  'eastus2'
]

resource rg_stamps 'Microsoft.Resources/resourceGroups@2020-06-01' = [for stamp in stampLocations: {
  name: '${prefix}-${stamp}-rg'
  location: stamp
  tags: tags
}]

//... more modules

module global_resources './global-resources.bicep' = {
  name: 'global_resources-${deploymentId}'
  scope: rg_global
  params: {
    location: rg_global.location
    prefix: prefix
    tags: tags
    stamps: [for index in range(0, length(stampLocations)): {
      location: stampLocations[index]
      aksKubletIdentityPrincipalId: stamps[index].outputs.aksKubletIdentityPrincipalId
      aksSubnetId: stamps[index].outputs.aksSubnetId
      backend_fqdn: stamps[index].outputs.ingressFqdn
    }]
  }
}

var secrets = [
  {
    name: 'CosmosDb-Endpoint'
    value: global_resources.outputs.cosmosDbEndpoint
  }
  {
    name: 'CosmosDb-PrimaryKey'
    value: global_resources.outputs.cosmosDbKey
  }
]

module stamp_0_secrets './kevault-secrets.bicep' = [for secret in secrets: {
  name: 'stamp_0_secrets-${deploymentId}'
  scope: resourceGroup(rg_stamps[0].name)
  dependsOn: [
    stamps
  ]
  params: {
    keyVaultName: '${prefix}${rg_stamps[0].location}kv'
    secretName: secret.name
    secretValue: secret.value
  }
}]

module stamp_1_secrets './kevault-secrets.bicep' = [for secret in secrets: {
  name: 'stamp_1_secrets-${deploymentId}'
  scope: resourceGroup(rg_stamps[1].name)
  dependsOn: [
    stamps
  ]
  params: {
    keyVaultName: '${prefix}${rg_stamps[1].location}kv'
    secretName: secret.name
    secretValue: secret.value
  }
}]
"), ("global-resources.bicep", string.Empty));

            result.Template.Should().NotHaveValue();
            result.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"rg_global\" does not exist in the current context.");
            result.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"rg_global\" does not exist in the current context.");
            result.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"stamps\" does not exist in the current context.");
            result.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"stamps\" does not exist in the current context.");
            result.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"stamps\" does not exist in the current context.");
            result.Should().ContainDiagnostic("BCP052", DiagnosticLevel.Error, "The type \"outputs\" does not contain property \"cosmosDbEndpoint\".");
            result.Should().ContainDiagnostic("BCP052", DiagnosticLevel.Error, "The type \"outputs\" does not contain property \"cosmosDbKey\".");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1592
        public void Test_Issue1592()
        {
            var result = CompilationHelper.Compile(
              ("main.bicep", @"
module foo 'test.bicep' = {
  name: 'foo'
}

output fooName string = foo.name
    "),
              ("test.bicep", @""));

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['fooName'].value", "foo");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1592
        public void Test_Issue1592_special_cases()
        {
            var result = CompilationHelper.Compile(
              ("main.bicep", @"
param someParam string

module foo 'test.bicep' = {
  name: '${someParam}-test'
}

output fooName string = foo.name
output fooOutput string = foo.outputs.test
    "),
              ("test.bicep", @"
output test string = 'hello'
"));

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['fooName'].value", "[format('{0}-test', parameters('someParam'))]");
            result.Template.Should().HaveValueAtPath("$.outputs['fooOutput'].value", "[reference(resourceId('Microsoft.Resources/deployments', format('{0}-test', parameters('someParam'))), '2020-06-01').outputs.test.value]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1432
        public void Test_Issue1432()
        {
            var result = CompilationHelper.Compile(@"
resource foo 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'myVM'
  name: 'myVm'
}
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP025", DiagnosticLevel.Error, "The property \"name\" is declared multiple times in this object. Remove or rename the duplicate properties."),
                ("BCP025", DiagnosticLevel.Error, "The property \"name\" is declared multiple times in this object. Remove or rename the duplicate properties."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1817
        public void Test_Issue1817()
        {
            var result = CompilationHelper.Compile(@"
targetScope = w

var foo = 42
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP032", DiagnosticLevel.Error, "The value must be a compile-time constant."),
                ("BCP057", DiagnosticLevel.Error, "The name \"w\" does not exist in the current context."),
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("foo")),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1630
        public void Test_Issue1630()
        {
            var result = CompilationHelper.Compile(@"
var singleResource = providers('Microsoft.Insights', 'components')
var allResources = providers('Microsoft.Insights')

// singleResource is an object!
var firstApiVersion = singleResource.apiVersions[0]

// allResources is an array of objects!
var firstResourceFirstApiVersion = allResources.resourceTypes[0].apiVersions[0]

output singleResource object = singleResource
output allResources array = allResources.resourceTypes
");

            result.Should().HaveDiagnostics(new[] {
                    (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("firstApiVersion")),
                    (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("firstResourceFirstApiVersion")),
                });
            result.Template.Should().HaveValueAtPath("$.variables['singleResource']", "[providers('Microsoft.Insights', 'components')]");
            result.Template.Should().HaveValueAtPath("$.variables['firstApiVersion']", "[variables('singleResource').apiVersions[0]]");
            result.Template.Should().HaveValueAtPath("$.variables['allResources']", "[providers('Microsoft.Insights')]");
            result.Template.Should().HaveValueAtPath("$.variables['firstResourceFirstApiVersion']", "[variables('allResources').resourceTypes[0].apiVersions[0]]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1627
        public void Test_Issue1627()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    foo: 'test'
  }
}

var bar = modulea.outputs.bar
"),
                ("modulea.bicep", @"
// duplicate parameter symbols
param foo string
param foo int

// duplicate output symbols
output bar bool = true
output bar int = 42
"));

            result.Should().HaveDiagnostics(new[] {
                ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("bar")),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1941
        public void Test_Issue1941()
        {
            var result = CompilationHelper.Compile(@"
param eventGridTopicName string
param eventGridSubscriptionName string
param location string

resource eventGridTopic 'Microsoft.EventGrid/topics@2020-06-01' = {
  name: eventGridTopicName
  location: location
}

resource eventGridSubscription 'Microsoft.EventGrid/topics/providers/eventSubscriptions@2020-06-01' = {
  name: '${eventGridTopic.name}/Microsoft.EventGrid/${eventGridSubscriptionName}'
}
");

            // verify the template still compiles
            result.Template.Should().NotBeNull();
            result.Should().HaveDiagnostics(new[] {
                ("BCP174", DiagnosticLevel.Warning, "Type validation is not available for resource types declared containing a \"/providers/\" segment. Please instead use the \"scope\" property."),
            });

            result = CompilationHelper.Compile(@"
resource resA 'Rp.A/providers@2020-06-01' = {
  name: 'resA'
}
resource resB 'Rp.A/providers/a/b@2020-06-01' = {
  name: 'resB/child/grandchild'
}
resource resC 'Rp.A/a/b/providers@2020-06-01' = {
  name: 'resC/child/grandchild'
}
");

            result.Template.Should().NotBeNull();
            result.Should().HaveDiagnostics(new[] {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Rp.A/providers@2020-06-01\" does not have types available."),
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Rp.A/providers/a/b@2020-06-01\" does not have types available."),
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Rp.A/a/b/providers@2020-06-01\" does not have types available."),
            });

            result = CompilationHelper.Compile(@"
param eventGridTopicName string
param eventGridSubscriptionName string
param location string

resource eventGridTopic 'Microsoft.EventGrid/topics@2020-06-01' = {
  name: eventGridTopicName
  location: location
}

resource eventGridSubscription 'Microsoft.EventGrid/eventSubscriptions@2020-06-01' = {
  name: eventGridSubscriptionName
  scope: eventGridTopic
}
");

            // verify the 'fixed' version compiles without diagnostics
            result.Template.Should().NotBeNull();
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/657
        public void Test_Issue657_discriminators()
        {
            var customTypes = new[] {
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Rp.A/parent@2020-10-01",
                        ("name", LanguageConstants.String))),
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent/child@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateDiscriminatedObjectType(
                        "Rp.A/parent/child@2020-10-01",
                        "name",
                        TestTypeHelper.CreateObjectType(
                            "Val1Type",
                            ("name", new StringLiteralType("val1")),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal1", LanguageConstants.Bool)))),
                        TestTypeHelper.CreateObjectType(
                            "Val2Type",
                            ("name", new StringLiteralType("val2")),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal2", LanguageConstants.Bool)))))),
            };

            var result = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// top-level resource
resource test2 'Rp.A/parent/child@2020-10-01' = {
  name: 'test/test2'
  properties: {
    anythingGoesHere: true
  }
}

// 'existing' top-level resource
resource test3 'Rp.A/parent/child@2020-10-01' existing = {
  name: 'test/test3'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'val1'
  properties: {
    onlyOnVal1: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'val2'
}
"));

            result.Should().NotHaveAnyDiagnostics();

            var failedResult = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'notAValidVal'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'notAValidVal'
}
"));

            failedResult.Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/657
        public void Test_Issue657_enum()
        {
            var customTypes = new[] {
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Rp.A/parent@2020-10-01",
                        ("name", LanguageConstants.String))),
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent/child@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Rp.A/parent/child@2020-10-01",
                        ("name", UnionType.Create(new StringLiteralType("val1"), new StringLiteralType("val2"))),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnEnum", LanguageConstants.Bool))))),
            };

            var result = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// top-level resource
resource test2 'Rp.A/parent/child@2020-10-01' = {
  name: 'test/test2'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' top-level resource
resource test3 'Rp.A/parent/child@2020-10-01' existing = {
  name: 'test/test3'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'val1'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'val2'
}
"));

            result.Should().NotHaveAnyDiagnostics();

            var failedResult = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'notAValidVal'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'notAValidVal'
}
"));

            failedResult.Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1985
        public void Test_Issue1985()
        {
            var result = CompilationHelper.Compile(@"
resource aksDefaultPoolSubnet 'Microsoft.Network/virtualNetworks/subnets' existing = {
  parent: virtualNetwork
  name: aksDefaultPoolSubnetName
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(aksDefaultPoolSubnet.id, 'Network Contributor')
  scope: aksDefaultPoolSubnet
  properties: {
    principalId: aksServicePrincipalObjectId
    roleDefinitionId: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  }
  dependsOn: [
    virtualNetwork
    userAssignedIdentities
  ]
}
");

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<provider>/<types>@<apiVersion>\"."),
                ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"aksDefaultPoolSubnet\" is not valid."),
                ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"aksDefaultPoolSubnet\" is not valid."),
                ("BCP057", DiagnosticLevel.Error, "The name \"aksServicePrincipalObjectId\" does not exist in the current context."),
                ("BCP057", DiagnosticLevel.Error, "The name \"virtualNetwork\" does not exist in the current context."),
                ("BCP057", DiagnosticLevel.Error, "The name \"userAssignedIdentities\" does not exist in the current context."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1986
        public void Test_Issue1986()
        {
            var result = CompilationHelper.Compile(@"
var aksServicePrincipalObjectId = 'aksServicePrincipalObjectId'
var aksDefaultPoolSubnetName = 'asdf'
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-08-01' = {
  name: 'asdfasdf'
}

resource userAssignedIdentities 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'asdfsdf'
  location: 'West US'
}

resource aksDefaultPoolSubnet 'Microsoft.Network/virtualNetworks/subnets@2020-08-01' existing = {
  parent: virtualNetwork
  name: aksDefaultPoolSubnetName
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(aksDefaultPoolSubnet.id, 'Network Contributor')
  scope: aksDefaultPoolSubnet
  properties: {
    principalId: aksServicePrincipalObjectId
    roleDefinitionId: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  }
  dependsOn: [
    userAssignedIdentities
  ]
}
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Authorization/roleAssignments')].dependsOn", new JArray {
                "[resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', 'asdfsdf')]",
                "[resourceId('Microsoft.Network/virtualNetworks', 'asdfasdf')]", // dependsOn should include the virtualNetwork parent resource
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1986
        public void Test_Issue1986_nested()
        {
            var result = CompilationHelper.Compile(@"
var aksServicePrincipalObjectId = 'aksServicePrincipalObjectId'
var aksDefaultPoolSubnetName = 'asdf'
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-08-01' = {
  name: 'asdfasdf'

  resource aksDefaultPoolSubnet 'subnets' existing = {
    name: aksDefaultPoolSubnetName
  }
}

resource userAssignedIdentities 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'asdfsdf'
  location: 'West US'
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(virtualNetwork::aksDefaultPoolSubnet.id, 'Network Contributor')
  scope: virtualNetwork::aksDefaultPoolSubnet
  properties: {
    principalId: aksServicePrincipalObjectId
    roleDefinitionId: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  }
  dependsOn: [
    userAssignedIdentities
  ]
}
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Authorization/roleAssignments')].dependsOn", new JArray {
                "[resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', 'asdfsdf')]",
                "[resourceId('Microsoft.Network/virtualNetworks', 'asdfasdf')]", // dependsOn should include the virtualNetwork parent resource
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1986
        public void Test_Issue1986_loops()
        {
            var result = CompilationHelper.Compile(@"
var aksServicePrincipalObjectId = 'aksServicePrincipalObjectId'
var aksDefaultPoolSubnetName = 'asdf'
var vnets = [
  'vnet1'
  'vnet2'
]
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-08-01' = [for vnet in vnets: {
  name: vnet
}]

resource userAssignedIdentities 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'asdfsdf'
  location: 'West US'
}

resource aksDefaultPoolSubnet 'Microsoft.Network/virtualNetworks/subnets@2020-08-01' existing = [for (vnet, i) in vnets: {
  parent: virtualNetwork[i]
  name: aksDefaultPoolSubnetName
}]

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for (vnet, i) in vnets: {
  name: guid(aksDefaultPoolSubnet[i].id, 'Network Contributor')
  scope: aksDefaultPoolSubnet[i]
  properties: {
    principalId: aksServicePrincipalObjectId
    roleDefinitionId: '4d97b98b-1d4f-4787-a291-c67834d212e7'
  }
  dependsOn: [
    userAssignedIdentities
  ]
}]
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Authorization/roleAssignments')].dependsOn", new JArray {
                "[resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', 'asdfsdf')]",
                "[resourceId('Microsoft.Network/virtualNetworks', variables('vnets')[copyIndex()])]", // dependsOn should include the virtualNetwork parent resource
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1993
        public void Test_Issue1993()
        {
            var result = CompilationHelper.Compile(@"
//""flat"" string
var jsonStringFlat  = '[""one"",""two"",""three"" ]'

//Good Array
var jsonStringGood  = '''
[
  ""one"",
  ""two"",
  ""three""
]'''

//Bad Array
var jsonStringBad  = '''
[
  ""one"",
  ""two"",
  ""three""
]
'''
var jsonArrayFlat = json(jsonStringFlat)
var jsonArrayGood = json(jsonStringGood)
var jsonArrayBad = json(jsonStringBad)

output flatArray array = [for (name, i) in jsonArrayFlat: {
  element: name
}]

output goodArray array = [for (name, i) in jsonArrayGood: {
  element: name
}]

output badArray array = [for (name, i) in jsonArrayBad : {
  element: name
}]
");


            var evaluated = TemplateEvaluator.Evaluate(result.Template);
            var expectedOutput = new JArray {
                new JObject { ["element"] = "one" },
                new JObject { ["element"] = "two" },
                new JObject { ["element"] = "three" },
            };
            evaluated.Should().HaveValueAtPath("$.outputs['flatArray'].value", expectedOutput);
            evaluated.Should().HaveValueAtPath("$.outputs['goodArray'].value", expectedOutput);
            evaluated.Should().HaveValueAtPath("$.outputs['badArray'].value", expectedOutput);
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/2009
        public void Test_Issue2009()
        {
            var result = CompilationHelper.Compile(@"
param providerNamespace string = 'Microsoft.Web'

output providerOutput object = {
  thing: providers(providerNamespace)

  otherThing: providers(providerNamespace, 'sites')
}
");

            result.Should().NotHaveAnyDiagnostics();

            var providersMetadata = new[] {
                new {
                    @namespace = "Microsoft.Web",
                    resourceTypes = new[] {
                        new {
                            resourceType = "sites",
                            locations = new[] { "West US", "East US", },
                            apiVersions = new[] { "2019-01-01", "2020-01-01", },
                        }
                    }
                }
            };

            var evaluated = TemplateEvaluator.Evaluate(result.Template, config => config with
            {
                Metadata = new()
                {
                    ["providers"] = JToken.FromObject(providersMetadata),
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providerOutput'].value.thing", new JObject
            {
                ["namespace"] = "Microsoft.Web",
                ["resourceTypes"] = new JArray {
                    new JObject {
                        ["resourceType"] = "sites",
                        ["locations"] = new JArray { "West US", "East US" },
                        ["apiVersions"] = new JArray { "2019-01-01", "2020-01-01" },
                    }
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providerOutput'].value.otherThing", new JObject
            {
                ["resourceType"] = "sites",
                ["locations"] = new JArray { "West US", "East US" },
                ["apiVersions"] = new JArray { "2019-01-01", "2020-01-01" },
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/2009
        public void Test_Issue2009_expanded()
        {
            var result = CompilationHelper.Compile(@"
output providersNamespace string = providers('Test.Rp').namespace
output providersResources array = providers('Test.Rp').resourceTypes

output providersResourceType string = providers('Test.Rp', 'fakeResource').resourceType
output providersApiVersionFirst string = providers('Test.Rp', 'fakeResource').apiVersions[0]
output providersLocationFirst string = providers('Test.Rp', 'fakeResource').locations[0]
");

            result.Should().NotHaveAnyDiagnostics();

            var providersMetadata = new[] {
                new {
                    @namespace = "Test.Rp",
                    resourceTypes = new[] {
                        new {
                            resourceType = "fakeResource",
                            locations = new[] { "Earth", "Mars" },
                            apiVersions = new[] { "3024-01-01", "4100-01-01", },
                        }
                    }
                }
            };

            var evaluated = TemplateEvaluator.Evaluate(result.Template, config => config with
            {
                Metadata = new()
                {
                    ["providers"] = JToken.FromObject(providersMetadata),
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providersNamespace'].value", "Test.Rp");
            evaluated.Should().HaveValueAtPath("$.outputs['providersResources'].value", new JArray {
                new JObject {
                    ["resourceType"] = "fakeResource",
                    ["locations"] = new JArray { "Earth", "Mars" },
                    ["apiVersions"] = new JArray { "3024-01-01", "4100-01-01" },
                }
            });


            evaluated.Should().HaveValueAtPath("$.outputs['providersResourceType'].value", "fakeResource");
            evaluated.Should().HaveValueAtPath("$.outputs['providersApiVersionFirst'].value", "3024-01-01");
            evaluated.Should().HaveValueAtPath("$.outputs['providersLocationFirst'].value", "Earth");
        }

        [TestMethod]
        public void Variable_loops_should_not_cause_infinite_recursion()
        {
            var result = CompilationHelper.Compile(@"
var loopInput = [
  'one'
  'two'
]
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
  index: i
  name: name
  value: 'prefix-${i}-${name}-suffix'
}]
");

            result.Should().HaveDiagnostics(new[]{
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("arrayOfObjectsViaLoop")),
            });
            result.Template.Should().NotBeNull();
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1883
        public void Test_Issue1883()
        {
            var result = CompilationHelper.Compile(@"
resource vm 'Microsoft.Compute/virtualMachines@2020-06-01' = if (true) {
  name: 'myVM'
  location: 'westus'

  resource vmExt 'extensions' = {
    name: 'myVMExtension'
    location: 'westus'
  }
}

output vmExtName string = vm::vmExt.name
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().NotBeNull();
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/691
        public void Test_Issue691()
        {
            var result = CompilationHelper.Compile(@"
var vmNotWorkingProps = {
  valThatDoesNotExist: ''
}

resource vmNotWorking 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'notWorking'
  location: 'west us'
  // no diagnostics raised here even though the type is invalid!
  properties: vmNotWorkingProps
//@           ~~~~~~~~~~~~~~~~~ $0
}
");


            result.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, "The property \"valThatDoesNotExist\" from source declaration \"vmNotWorkingProps\" is not allowed on objects of type \"VirtualMachineProperties\". Permissible properties include \"additionalCapabilities\", \"availabilitySet\", \"billingProfile\", \"diagnosticsProfile\", \"evictionPolicy\", \"extensionsTimeBudget\", \"hardwareProfile\", \"host\", \"hostGroup\", \"licenseType\", \"networkProfile\", \"osProfile\", \"priority\", \"proximityPlacementGroup\", \"securityProfile\", \"storageProfile\", \"virtualMachineScaleSet\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1988
        public void Test_Issue1988()
        {
            var result = CompilationHelper.Compile(@"
var subnet1Name = 'foobarsubnet-blueprint'
var virtualNetworkResourceGroup = 'alex-test-feb'

resource vnet 'Microsoft.Network/virtualNetworks@2020-08-01' existing = {
  name: 'foobarvnet-blueprint'
  scope: resourceGroup(virtualNetworkResourceGroup)
}

resource my_subnet 'Microsoft.Network/virtualNetworks/subnets@2020-08-01' existing = {
  name: subnet1Name
  parent: vnet
}

resource my_interface 'Microsoft.Network/networkInterfaces@2015-05-01-preview' = {
  name: 'nic-test01'
  location: vnet.location // this is not valid because it requires reference() if resource is 'existing'
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig1'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: my_subnet.id
          }
        }
      }
    ]
  }
}
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"location\" property of the \"Microsoft.Network/networkInterfaces\" type, which requires a value that can be calculated at the start of the deployment. Properties of vnet which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/2268
        public void Test_Issue2268()
        {
            var result = CompilationHelper.Compile(@"
param sqlServerName string = 'myServer'
param sqlDbName string = 'myDb'
var sqlDatabase = {
  name: sqlDbName
  dataEncryption: 'Enabled'
}

resource sqlDb 'Microsoft.Sql/servers/databases@2020-02-02-preview' existing = {
  name: '${sqlServerName}/${sqlDatabase.name}'
}

resource transparentDataEncryption 'Microsoft.Sql/servers/databases/transparentDataEncryption@2014-04-01' = {
  name: 'current'
  parent: sqlDb
  properties: {
    status: sqlDatabase.dataEncryption
  }
}

output tdeId string = transparentDataEncryption.id
");
            result.Should().NotHaveAnyDiagnostics();

            var evaluated = TemplateEvaluator.Evaluate(result.Template);

            evaluated.Should().HaveValueAtPath("$.resources[0].name", "myServer/myDb/current");
            evaluated.Should().HaveValueAtPath("$.outputs['tdeId'].value", "/subscriptions/f91a30fd-f403-4999-ae9f-ec37a6d81e13/resourceGroups/testResourceGroup/providers/Microsoft.Sql/servers/myServer/databases/myDb/transparentDataEncryption/current");
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2289
        public void Test_Issue2289()
        {
            var result = CompilationHelper.Compile(@"

resource p 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'sss'
  location: ''
}

resource c 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
  parent: p
  name: 'sss/'
}]

resource p2 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'sss2'
  location: ''

  resource c2 'CNAME' = [for thing2 in []: {
    name: 'sss2/'
  }]
}
");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP179", DiagnosticLevel.Warning, "The loop item variable \"thing\" must be referenced in at least one of the value expressions of the following properties: \"name\", \"parent\""),
                ("BCP170", DiagnosticLevel.Error, "Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name."),
                ("BCP179", DiagnosticLevel.Warning, "The loop item variable \"thing2\" must be referenced in at least one of the value expressions of the following properties: \"name\""),
                ("BCP170", DiagnosticLevel.Error, "Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name."),
            });
            result.Template.Should().BeNull();
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1809
        public void Test_Issue1809()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
module tags './tags.bicep' = {
  name: 'tags'
}

resource vwan 'Microsoft.Network/virtualWans@2020-05-01' = {
  location: 'westus'
  name: 'vwan'
  properties: {
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    type: 'foo'
  }
  tags: tags.outputs.tagsoutput
}

resource vwan2 'Microsoft.Network/virtualWans@2020-05-01' = {
  location: 'westus'
  name: 'vwan2'
  properties: {
    disableVpnEncryption: false
    allowBranchToBranchTraffic: true
    allowVnetToVnetTraffic: true
    type: 'foo'
  }
  tags: {
    // Should run deploy-time constant checking for myTag1.
    myTag1: tags.outputs.tagsoutput.tag1
  }
}

resource nsgs 'Microsoft.Network/networkSecurityGroups@2019-04-01' = [for i in range(0, 2): {
  name: 'nsg-${i}'
  location: 'westus'
  properties: {}
  tags: tags.outputs.tagsoutput
}]

resource nsgs2 'Microsoft.Network/networkSecurityGroups@2019-04-01' = [for i in range(0, 2): {
  name: 'nsg2-${i}'
  location: 'westus'
  properties: {}
  tags: {
    // Should run deploy-time constant checking for myTag1.
    myTag1: tags.outputs.tagsoutput.tag1
  }
}]

resource publicIP 'Microsoft.Network/publicIpAddresses@2019-04-01' = {
  name: 'publicIP'
  location: 'westus'
  zones: [
    // Should run deploy-time constant checking inside zones.
    vwan.properties.type
  ]
  sku: {
    name: 'Basic'
  }
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
    }
  }
}
"),
                ("tags.bicep", @"
output tagsoutput object = {
  tag1: 'tag1Value'
}
"));

            result.Should().HaveDiagnostics(new[] {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"tags\" property of the \"Microsoft.Network/virtualWans\" type, which requires a value that can be calculated at the start of the deployment. Properties of tags which can be calculated at the start include \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"tags\" property of the \"Microsoft.Network/virtualWans\" type, which requires a value that can be calculated at the start of the deployment. Properties of tags which can be calculated at the start include \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"tags\" property of the \"Microsoft.Network/networkSecurityGroups\" type, which requires a value that can be calculated at the start of the deployment. Properties of tags which can be calculated at the start include \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"tags\" property of the \"Microsoft.Network/networkSecurityGroups\" type, which requires a value that can be calculated at the start of the deployment. Properties of tags which can be calculated at the start include \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"zones\" property of the \"Microsoft.Network/publicIPAddresses\" type, which requires a value that can be calculated at the start of the deployment. Properties of vwan which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2391
        public void Test_Issue2391()
        {
            var result = CompilationHelper.Compile(@"
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'myVM'
  location: 'westus'

  resource vmExts 'extensions' = [for vmExtName in []: {
    name: vmExtName
    location: 'westus'
  }]
}

output vmExtNames array = [for vmExtName in vm::vmExts: {
  name: vmExtName
}]
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP144", DiagnosticLevel.Error, "Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression.")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2090
        public void Test_Issue2090()
        {
            var result = CompilationHelper.Compile(@"
resource vnet 'Microsoft.Network/virtualNetworks@2020-11-01' = {
  name: 'myVnet'
}

output snetIds array = [for subnet in vnet.properties.subnets: {
  subName: subnet.name
  subId: subnet.id
}]
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. Properties of vnet which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\".")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/1699
        public void Test_Issue1699()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-10-01' = {
  name: 'rg'
  location: 'West US'
}

var test = [
  {
    name: 'test'
    value: rg.properties.provisioningState
  }
]

resource rg2 'Microsoft.Resources/resourceGroups@2020-10-01' = [for item in test: {
  name: 'rg2'
  location: 'West US'
}]

resource rg3 'Microsoft.Resources/resourceGroups@2020-10-01' = if (rg2[0].tags.foo == 'bar') {
  name: 'rg3'
  location: 'West US'
}
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP179", DiagnosticLevel.Warning, "The loop item variable \"item\" must be referenced in at least one of the value expressions of the following properties: \"name\""),
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"test\" -> \"rg\"). Properties of rg which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP177", DiagnosticLevel.Error, "This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment. Properties of rg2 which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\".")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2262
        public void Test_Issue2262()
        {
            // Wrong discriminated key: PartitionScheme.
            var result = CompilationHelper.Compile(@"
resource service 'Microsoft.ServiceFabric/clusters/applications/services@2020-12-01-preview' = {
  name: 'myCluster/myApp/myService'
  properties: {
    serviceKind: 'Stateful'
    partitionDescription: {
      PartitionScheme: 'Named'
      names: [
        'foo'
      ]
      count: 1
    }
  }
}
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP078", DiagnosticLevel.Warning, "The property \"partitionScheme\" requires a value of type \"'Named' | 'Singleton' | 'UniformInt64Range'\", but none was supplied."),
                ("BCP089", DiagnosticLevel.Warning, "The property \"PartitionScheme\" is not allowed on objects of type \"'Named' | 'Singleton' | 'UniformInt64Range'\". Did you mean \"partitionScheme\"?"),
            });

            var diagnosticWithCodeFix = result.Diagnostics.OfType<FixableDiagnostic>().Single();
            var codeFix = diagnosticWithCodeFix.Fixes.Single();
            var codeReplacement = codeFix.Replacements.Single();

            codeReplacement.Span.Should().Be(new TextSpan(212, 15));
            codeReplacement.Text.Should().Be("partitionScheme");
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2484
        public void Test_Issue2484()
        {
            var result = CompilationHelper.Compile(@"
@sys.allowed([
  'apple'
  'banana'
])
param foo string = 'peach'
");

            result.Should().HaveDiagnostics(new[] {
                (NoUnusedParametersRule.Code, DiagnosticLevel.Warning, new NoUnusedParametersRule().GetMessage("foo")),
                ("BCP027", DiagnosticLevel.Error, "The parameter expects a default value of type \"'apple' | 'banana'\" but provided value is of type \"'peach'\"."),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2547
        public void Test_Issue2547()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
module stgModule './stg.bicep' = {
  name: 'stgModule'
}

resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: 'pubIP'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: az.listSecrets(stgModule.outputs.storageAccount.id, stgModule.outputs.storageAccount.apiVersion).keys[0].value
    dnsSettings: {
      domainNameLabel: listKeys(stgModule.outputs.storageAccount.id, stgModule.outputs.storageAccount.apiVersion).keys[0].value
    }
  }
}
"),
                ("stg.bicep", @"
resource stg 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'mystorage1234567'
  location: 'westus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

output storageAccount object = {
  id: stg.id
  apiVersion: stg.apiVersion
}
"));

            result.Should().HaveDiagnostics(new[] {
                ("BCP181", DiagnosticLevel.Error, "This expression is being used in an argument of the function \"listSecrets\", which requires a value that can be calculated at the start of the deployment. Properties of stgModule which can be calculated at the start include \"name\"."),
                ("BCP181", DiagnosticLevel.Error, "This expression is being used in an argument of the function \"listSecrets\", which requires a value that can be calculated at the start of the deployment. Properties of stgModule which can be calculated at the start include \"name\"."),
                ("BCP181", DiagnosticLevel.Error, "This expression is being used in an argument of the function \"listKeys\", which requires a value that can be calculated at the start of the deployment. Properties of stgModule which can be calculated at the start include \"name\"."),
                ("BCP181", DiagnosticLevel.Error, "This expression is being used in an argument of the function \"listKeys\", which requires a value that can be calculated at the start of the deployment. Properties of stgModule which can be calculated at the start include \"name\"."),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2494
        public void Test_Issue2494()
        {
            var result = CompilationHelper.Compile(@"
var name = nameCopy
var nameCopy = name

resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: name
  location: resourceGroup().location
  sku: {
    name: 'F1'
    capacity: 1
  }
}
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"nameCopy\" -> \"name\")."),
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"name\" -> \"nameCopy\")."),
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"name\" -> \"nameCopy\").")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2624
        public void Test_Issue2624()
        {
            var result = CompilationHelper.Compile(@"
var foo = az.listKeys('foo', '2012-02-01')[0].value
");

            result.Should().HaveDiagnostics(new[] {
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("foo")),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/449
        public void Test_Issue449_PositiveCase()
        {
            var result = CompilationHelper.Compile(@"
param zonesEnabled bool

resource pubipv4 'Microsoft.Network/publicIpAddresses@2020-05-01' = {
  name: 'pip'
  zones: zonesEnabled ? [
    'a'
  ] : null
}");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/449
        public void Test_Issue449_NegativeCase()
        {
            var result = CompilationHelper.Compile(@"
param zonesEnabled bool

resource pubipv4 'Microsoft.Network/publicIpAddresses@2020-05-01' = {
  name: null
  zones: zonesEnabled ? [
    'a'
  ] : null
}

resource lock 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'lock'
  properties: {
    level: 'CanNotDelete'
  }
  scope: null
}

resource cname 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
  name: null
  parent: null
}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"string\" but the provided value is of type \"null\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"scope\" expected a value of type \"resource | tenant\" but the provided value is of type \"null\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"string\" but the provided value is of type \"null\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"parent\" expected a value of type \"Microsoft.Network/dnsZones\" but the provided value is of type \"null\"."),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2248
        public void Test_Issue2248_UnionTypeInArrayAccessBaseExpression()
        {
            var result = CompilationHelper.Compile(@"
param isProdLike bool

var testLocations = [
  'northeurope'
]
var prodLocations = [
  'northeurope'
  'westeurope'
]
var locations = isProdLike ? prodLocations : testLocations
var primaryLocation = locations[0]
");
            result.Should().HaveDiagnostics(new[] {
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("primaryLocation")),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2248
        public void Test_Issue2248_UnionTypeInArrayAccessBaseExpression_NegativeCase()
        {
            var result = CompilationHelper.Compile(@"
var foos = true ? true : []
var primaryFoo = foos[0]
");
            result.Should().HaveDiagnostics(new[]
            {
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("primaryFoo")),
                ("BCP076",DiagnosticLevel.Error,"Cannot index over expression of type \"array | bool\". Arrays or objects are required.")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2248
        public void Test_Issue2248_UnionTypeInPropertyAccessBaseExpression()
        {
            var result = CompilationHelper.Compile(@"
param input object
param which bool

var default = {

}

var chosenOne = which ? input : default

var p = chosenOne.foo
");
            result.Should().HaveDiagnostics(new[] {
                (NoUnusedVariablesRule.Code, DiagnosticLevel.Warning, new NoUnusedVariablesRule().GetMessage("p")),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/2695
        public void Test_Issue2695()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'managementGroup'

module mgDeploy 'managementGroup.bicep' = {
  name: 'mgDeploy'
  params: {
  }
  scope: managementGroup('test')
}
"),
                ("managementGroup.bicep", @"
targetScope = 'managementGroup'

resource policyAssignment 'Microsoft.Authorization/policyAssignments@2020-09-01' = {
  name: 'policy-assignment-01'
  properties: {
    policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/10ee2ea2-fb4d-45b8-a7e9-a2e770044cd9'
    displayName: 'Sample policy assignment'
    description: 'Sample policy'
    enforcementMode: 'Default'
  }
}
"));

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2622
        public void Test_Issue2622()
        {
            var result = CompilationHelper.Compile(@"
resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  // Runtime error. This should be blocked.
  name: listKeys('foo', '2012-01-01')[0].value
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: 'dnsname'
    }
  }
}
");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP120",DiagnosticLevel.Error,"This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Network/publicIPAddresses\" type, which requires a value that can be calculated at the start of the deployment.")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2291
        public void Test_Issue2291()
        {
            var result = CompilationHelper.Compile(@"
resource registry 'Microsoft.ContainerRegistry/registries@2019-12-01-preview' existing = {
  name: 'foo'
  resource importPipeline 'importPipelines' existing = {
    name: 'import'
  }
}

resource pipelineRun 'Microsoft.ContainerRegistry/registries/pipelineRuns@2019-12-01-preview' = [for index in range(0, 3): if(registry::importPipeline.properties.trigger.sourceTrigger.status == 'Disabled') {
  parent: registry
  name: 'bar${index}'
  properties: {
    request: {
      pipelineResourceId: registry::importPipeline.id
      artifacts: []
      source: {
        type: 'AzureStorageBlob'
        name: 'blobBaseName_${index}'
      }
      catalogDigest: ''
    }
    forceUpdateTag: ''
  }
}]
");
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP177",DiagnosticLevel.Error,"This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment. Properties of importPipeline which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\".")
            });
        }

        [TestMethod]
        public void Test_Issue2578()
        {
            var result = CompilationHelper.Compile(
                ("simple.bicep", @"
param hello string
output hello string = hello
"),
                ("main.bicep", @"
var v = {
  hello: 's'
}

module simple 'simple.bicep' = {
  name: 's2'
  params: v
}
"));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP183", DiagnosticLevel.Error, "The value of the module \"params\" property must be an object literal.")
            });
        }

        [TestMethod]
        public void Test_Issue2578_ParseError()
        {
            var result = CompilationHelper.Compile(
                ("simple.bicep", @"
param hello string
output hello string = hello
"),
                ("main.bicep", @"
var v = {
  hello: 's'
}

module simple 'simple.bicep' = {
  name: 's2'
  params:
}

output v object = v
"));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.")
            });

            result.Should().NotHaveDiagnosticsWithCodes(new[] { "BCP183" });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2895
        public void Test_Issue2895()
        {
            var result = CompilationHelper.Compile(@"
param vnetName string
param subnetName string
param vnetResourceGroupName string

resource subnetRef 'Microsoft.Network/virtualNetworks/subnets@2020-11-01' existing = {
  name: '${vnetName}/subnets/${subnetName}'
  scope: resourceGroup(vnetResourceGroupName)
}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP169", DiagnosticLevel.Error, "Expected resource name to contain 1 \"/\" character(s). The number of name segments must match the number of segments in the resource type."),
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/3566
        public void Test_Issue3566()
        {
            var result = CompilationHelper.Compile(@"
resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: uniqueString(resourceGroup().id, 'alfran')
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}

var secret = storageaccount.listKeys().keys[0].value
output secret string = secret
");

            result.Template.Should().NotHaveValueAtPath("$.variables['secret']", "the listKeys() output should be in-lined and not generate a variable");
            result.Template.Should().HaveValueAtPath("$.outputs['secret'].value", "[listKeys(resourceId('Microsoft.Storage/storageAccounts', uniqueString(resourceGroup().id, 'alfran')), '2021-02-01').keys[0].value]", "the listKeys() output should be in-lined");

            result.Should().NotHaveAnyDiagnostics();
        }

        // https://github.com/Azure/bicep/issues/3558
        [TestMethod]
        public void Test_Issue3558()
        {
            var result = CompilationHelper.Compile(@"
param dataCollectionRule object
param tags object

var defaultLogAnalyticsWorkspace = {
  subscriptionId: subscription().subscriptionId
}

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
  name: logAnalyticsWorkspace.name
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
}]

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
  name: dataCollectionRule.name
  location: dataCollectionRule.location
  tags: tags
  kind: dataCollectionRule.kind
  properties: {
    description: dataCollectionRule.description
    destinations: union(empty(dataCollectionRule.destinations.azureMonitorMetrics.name) ? {} : {
      azureMonitorMetrics: {
        name: dataCollectionRule.destinations.azureMonitorMetrics.name
      }
    },{
      logAnalytics: [for (logAnalyticsWorkspace, i) in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
        name: logAnalyticsWorkspace.destinationName
        workspaceResourceId: logAnalyticsWorkspaces[i].id
      }]
    })
    dataSources: dataCollectionRule.dataSources
    dataFlows: dataCollectionRule.dataFlows
  }
}
");

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP138", DiagnosticLevel.Error, "For-expressions are not supported in this context. For-expressions may be used as values of resource, module, variable, and output declarations, or values of resource and module properties.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/1833
        /// </summary>
        [TestMethod]
        public void Test_Issue1833()
        {
            var result = CompilationHelper.Compile(
                ("managementGroup.bicep", @"
targetScope = 'managementGroup'
"),
                ("main.bicep", @"
targetScope = 'tenant'

param mainMgName string
param managementGroups array

resource mainMg 'Microsoft.Management/managementGroups@2020-05-01' = {
  name: mainMgName
}

resource mgs 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, i) in managementGroups: {
  name: mg
}]

module singleMgModule 'managementGroup.bicep' = {
  name: 'single-mg'
  scope: mainMg
}
"));

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/3617
        public void Test_Issue3617()
        {
            var result = CompilationHelper.Compile(@"
param eventGridSystemTopicName string
param subscription object
param endPointPropertiesWithIdentity object
param endPointProperties object
param defaultAdvancedFilterObject object

resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2020-10-15-preview' = {
  name: '${eventGridSystemTopicName}/${subscription.name}'
  properties: {
    deliveryWithResourceIdentity: subscription.destination.useIdentity ? endPointPropertiesWithIdentity[toLower(subscription.destination.type)] : null
    destination: subscription.destination.useIdentity ? null : endPointProperties[toLower(subscription.destination.type)]
    filter: {
      subjectBeginsWith: subscription.filter.beginsWith
      subjectEndsWith: subscription.filter.endsWith
      includedEventTypes: subscription.filter.eventTypes
      isSubjectCaseSensitive: subscription.filter.caseSensitive
      enableAdvancedFilteringOnArrays: subscription.filter.enableAdvancedFilteringOnArrays
      advancedFilters: [for advancedFilter in subscription.filter.advancedFilters: {
        key: advancedFilter.key
        operatorType: advancedFilter.operator
        value: union(defaultAdvancedFilterObject, advancedFilter).value
        values: union(defaultAdvancedFilterObject, advancedFilter).values
      }]
    }
  }
}
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2990
        public void Test_Issue2990()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'managementGroup'

param managementGroupName string
param subscriptionId string

resource myManagementGroup 'Microsoft.Management/managementGroups@2021-04-01' existing = {
  scope: tenant()
  name: managementGroupName
}

resource subscriptionAssociation 'Microsoft.Management/managementGroups/subscriptions@2021-04-01' = {
  parent: myManagementGroup
  name: subscriptionId
}
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/4007
        public void Test_Issue4007()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'subscription'

var map = {
    '1': 'hello'
}

output one string = map['1']
");

            result.Template.Should().HaveValueAtPath("$.outputs.one.value", "[variables('map')['1']]");

            var evaluated = TemplateEvaluator.Evaluate(result.Template);
            evaluated.Should().HaveValueAtPath("$.outputs.one.value", "hello");
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/4156
        public void Test_Issue4156()
        {
            var result = CompilationHelper.Compile(@"
var location = resourceGroup().location
var topics = [
  'topicA'
  'topicB'
]

resource eventGridTopics 'Microsoft.EventGrid/topics@2021-06-01-preview' = [for topic in topics: {
  name: '${topic}-ZZZ'
  location: location
  sku: {
    name: 'Basic'
  }
  kind: 'Azure'
  identity: {
    type: 'SystemAssigned'
  }
}]

resource testR 'Microsoft.EventGrid/topics@2021-06-01-preview' existing = {
  name: 'myExistingEventGridTopic'
}

output deployedTopics array = [for (topicName, i) in topics: {
  name: topicName
  accessKey1: testR.listKeys().key1
  accessKey2: eventGridTopics[i].listKeys().key1
}]
");

            result.Template!.Should().HaveValueAtPath("$.outputs.deployedTopics.copy.input", new JObject
            {
                ["name"] = "[variables('topics')[copyIndex()]]",
                ["accessKey1"] = "[listKeys(resourceId('Microsoft.EventGrid/topics', 'myExistingEventGridTopic'), '2021-06-01-preview').key1]",
                ["accessKey2"] = "[listKeys(resourceId('Microsoft.EventGrid/topics', format('{0}-ZZZ', variables('topics')[copyIndex()])), '2021-06-01-preview').key1]"
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/4212
        public void Test_Issue4212()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
module mod 'mod.bicep' = {
  name: 'mod'
}

resource res 'Microsoft.Network/virtualNetworks/subnets@2020-11-01' existing = {
  name: 'abc/def'
  parent: mod
}

resource res2 'Microsoft.Network/virtualNetworks/subnets@2020-11-01' existing = {
  name: 'res2'
  parent: tenant()
}

output test string = res.id
"),
                ("mod.bicep", ""));

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Error, "The property \"parent\" expected a value of type \"Microsoft.Network/virtualNetworks\" but the provided value is of type \"module\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"parent\" expected a value of type \"Microsoft.Network/virtualNetworks\" but the provided value is of type \"tenant\"."),
            });
        }
        
        // https://github.com/Azure/bicep/issues/4542
        [TestMethod]
        public void Test_Issue4542()
        {
            var result = CompilationHelper.Compile(@"
param sasTokenBaseTime string = utcNow('u')
param permissions string = 'adlrwu'

var sasTokenParams = {
  signedPermission: permissions
  signedExpiry: dateTimeAdd(sasTokenBaseTime, 'PT96H')
  signedProtocol: 'https'
  signedResourceTypes: 'sco'
  signedServices: 'b'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-04-01' = {
  name: 'foo'
  sku: {
    name: 'Standard_RAGRS'
  }
  kind: 'StorageV2'
  location: 'westus'

  resource blob 'blobServices' = {
    name: 'default'
    resource container 'containers' = {
      name: 'foo'
      properties: {
        publicAccess: 'None'
      }
    }
  }
}

resource registry 'Microsoft.ContainerRegistry/registries@2019-12-01-preview' = {
  name: 'foo'
  location: 'westus'
  sku: {
    name: 'Premium'
  }

  resource importPipeline 'importPipelines' = {
    name: 'foo'
    location: 'westus'
    identity: {
      type: 'SystemAssigned'
    }
    properties: {
      source: {
        type: 'AzureStorageBlobContainer'
        uri: uri(storageAccount.properties.primaryEndpoints.blob, storageAccount::blob::container.name)
        keyVaultUri: kv::secret.properties.secretUri
      }
    }
  }
}

resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: 'foo'

  resource ap 'accessPolicies' = {
    name: 'add'
    properties: {
      accessPolicies: [
        {
          tenantId: registry::importPipeline.identity.tenantId
          objectId: registry::importPipeline.identity.principalId
          permissions: {
            secrets: [
              'get'
            ]
          }
        }
      ]
    }
  }

  resource secret 'secrets' = {
    name: 'secretname'
    properties: {
      value: storageAccount.listAccountSas(storageAccount.apiVersion, sasTokenParams).accountSasToken
    }
    dependsOn: [
      // the below dependency gets a stack overflow
      ap
    ]
  }
}
");

            result.Template.Should().NotHaveValue();
            result.Should().HaveDiagnostics(new[] {
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"secret\" -> \"ap\" -> \"importPipeline\")."),
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"importPipeline\" -> \"secret\" -> \"ap\")."),
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"importPipeline\" -> \"secret\" -> \"ap\")."),
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"ap\" -> \"importPipeline\" -> \"secret\")."),
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/2703
        /// </summary>
        [TestMethod]
        public void Test_Issue2703()
        {
            var result = CompilationHelper.Compile(@"
resource test 'Microsoft.Resources/deploymentScripts@2020-10-01' existing = {
  name: 'test'
}

output expTime string = test.properties.status.expirationTime
");

            result.Should().NotHaveAnyDiagnostics();
        }

        // https://github.com/Azure/bicep/issues/4565
        [TestMethod]
        public void Test_Issue4565()
        {
            var result = CompilationHelper.Compile(@"
var port = 1234

output test string = '${port}'
");

            result.Template.Should().HaveValueAtPath("$.outputs['test'].value", "[format('{0}', variables('port'))]");

            var evaluated = TemplateEvaluator.Evaluate(result.Template);
            evaluated.Should().HaveValueAtPath("$.outputs['test'].value", "1234", "the evaluated output should be of type string");
        }
    }
}
