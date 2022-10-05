// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        [NotNull] public TestContext? TestContext { get; set; }

        [TestMethod]
        // https://github.com/azure/bicep/issues/3636
        public void Test_Issue3636()
        {
            var lineCount = 100; // increase this number to 10,000 for more intense test

            // use this crypto random number gen to avoid CI warning
            int generateRandomInt(int minVal = 0, int maxVal = 50)
            {
                var rnd = new byte[4];
                RandomNumberGenerator.Fill(rnd);
                var i = Math.Abs(BitConverter.ToInt32(rnd, 0));
                return Convert.ToInt32(i % (maxVal - minVal + 1) + minVal);
            }

            string randomString()
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, generateRandomInt())
                    .Select(s => s[generateRandomInt(0, s.Length - 1)]).ToArray());
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            // ensure we're generating the correct expression with 'subscriptionResourceId', and using the correct name for the module
            result.Template.Should().HaveValueAtPath("$.outputs['vnetid'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module')).outputs.vnetId.value]");
            result.Template.Should().HaveValueAtPath("$.outputs['vnetstate'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module')).outputs.vnetstate.value]");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/982
        public void Test_Issue982()
        {
            var result = CompilationHelper.Compile(@"
param functionApp object
param serverFarmId string

#disable-next-line outputs-should-not-contain-secrets
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

resource rgOwner 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: '${guid(rg.name, 'owner')}'
  scope: rg
  properties: {
    roleDefinitionId: '${subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '8e3af657-a8ff-443c-a75c-2fe8c4bcb635')}'
    principalId: groupOwnerId
    principalType: 'Group'
  }
}

resource rgContributor 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: '${guid(rg.name, 'contributor')}'
  scope: rg
  properties: {
    roleDefinitionId: '${subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')}'
    principalId: groupContributorId
    principalType: 'Group'
  }
}

resource rgReader 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP139", DiagnosticLevel.Error, "A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope."),
                ("BCP139", DiagnosticLevel.Error, "A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope."),
                ("BCP139", DiagnosticLevel.Error, "A resource's scope must match the scope of the Bicep file for it to be deployable. You must use modules to deploy resources to a different scope."),
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['fooName'].value", "[format('{0}-test', parameters('someParam'))]");
            result.Template.Should().HaveValueAtPath("$.outputs['fooOutput'].value", "[reference(resourceId('Microsoft.Resources/deployments', format('{0}-test', parameters('someParam')))).outputs.test.value]");
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP032", DiagnosticLevel.Error, "The value must be a compile-time constant."),
                ("BCP057", DiagnosticLevel.Error, "The name \"w\" does not exist in the current context."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/1630
        public void Test_Issue1630()
        {
            var result = CompilationHelper.Compile(@"
#disable-next-line BCP241
var singleResource = providers('Microsoft.Insights', 'components')
#disable-next-line BCP241
var allResources = providers('Microsoft.Insights')

// singleResource is an object!
var firstApiVersion = singleResource.apiVersions[0]

// allResources is an array of objects!
var firstResourceFirstApiVersion = allResources.resourceTypes[0].apiVersions[0]

output singleResource object = singleResource
output allResources array = allResources.resourceTypes
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP029", DiagnosticLevel.Error, "The resource type is not valid. Specify a valid resource type of format \"<types>@<apiVersion>\"."),
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

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Authorization/roleAssignments')].dependsOn", new JArray
            {
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

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Authorization/roleAssignments')].dependsOn", new JArray
            {
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

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for (vnet, i) in vnets: {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Authorization/roleAssignments')].dependsOn", new JArray
            {
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
            var expectedOutput = new JArray
            {
                new JObject {["element"] = "one"},
                new JObject {["element"] = "two"},
                new JObject {["element"] = "three"},
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
#disable-next-line BCP241
  thing: providers(providerNamespace)
#disable-next-line BCP241
  otherThing: providers(providerNamespace, 'sites')
}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var providersMetadata = new[]
            {
                new
                {
                    @namespace = "Microsoft.Web",
                    resourceTypes = new[]
                    {
                        new
                        {
                            resourceType = "sites",
                            locations = new[] {"West US", "East US",},
                            apiVersions = new[] {"2019-01-01", "2020-01-01",},
                        }
                    }
                }
            };

            var evaluated = TemplateEvaluator.Evaluate(result.Template, configBuilder: config => config with
            {
                Metadata = new()
                {
                    ["providers"] = JToken.FromObject(providersMetadata),
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providerOutput'].value.thing", new JObject
            {
                ["namespace"] = "Microsoft.Web",
                ["resourceTypes"] = new JArray
                {
                    new JObject
                    {
                        ["resourceType"] = "sites",
                        ["locations"] = new JArray {"West US", "East US"},
                        ["apiVersions"] = new JArray {"2019-01-01", "2020-01-01"},
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
#disable-next-line BCP241
output providersNamespace string = providers('Test.Rp').namespace
#disable-next-line BCP241
output providersResources array = providers('Test.Rp').resourceTypes

#disable-next-line BCP241
output providersResourceType string = providers('Test.Rp', 'fakeResource').resourceType
#disable-next-line BCP241
output providersApiVersionFirst string = providers('Test.Rp', 'fakeResource').apiVersions[0]
#disable-next-line BCP241
output providersLocationFirst string = providers('Test.Rp', 'fakeResource').locations[0]
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var providersMetadata = new[]
            {
                new
                {
                    @namespace = "Test.Rp",
                    resourceTypes = new[]
                    {
                        new
                        {
                            resourceType = "fakeResource",
                            locations = new[] {"Earth", "Mars"},
                            apiVersions = new[] {"3024-01-01", "4100-01-01",},
                        }
                    }
                }
            };

            var evaluated = TemplateEvaluator.Evaluate(result.Template, configBuilder: config => config with
            {
                Metadata = new()
                {
                    ["providers"] = JToken.FromObject(providersMetadata),
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providersNamespace'].value", "Test.Rp");
            evaluated.Should().HaveValueAtPath("$.outputs['providersResources'].value", new JArray
            {
                new JObject
                {
                    ["resourceType"] = "fakeResource",
                    ["locations"] = new JArray {"Earth", "Mars"},
                    ["apiVersions"] = new JArray {"3024-01-01", "4100-01-01"},
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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


            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP037", DiagnosticLevel.Warning, "The property \"valThatDoesNotExist\" from source declaration \"vmNotWorkingProps\" is not allowed on objects of type \"VirtualMachineProperties\". Permissible properties include \"additionalCapabilities\", \"availabilitySet\", \"billingProfile\", \"diagnosticsProfile\", \"evictionPolicy\", \"extensionsTimeBudget\", \"hardwareProfile\", \"host\", \"hostGroup\", \"licenseType\", \"networkProfile\", \"osProfile\", \"priority\", \"proximityPlacementGroup\", \"securityProfile\", \"storageProfile\", \"virtualMachineScaleSet\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/2535
        public void Test_Issue2535()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'managementGroup'

resource mg 'Microsoft.Management/managementGroups@2020-05-01' = {
  name: 'MyChildMG'
  scope: tenant()
  properties: {
    displayName: 'This should be a child of MyParentMG'
    details: {
      parent: managementGroup()
    }
  }
}
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[0].properties.details.parent", "[managementGroup()]");

            var evaluated = TemplateEvaluator.Evaluate(result.Template);
            evaluated.Should().HaveValueAtPath("$.resources[0].properties.details.parent.id", "/providers/Microsoft.Management/managementGroups/3fc9f36e-8699-43af-b038-1c103980942f");
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

resource my_interface 'Microsoft.Network/networkInterfaces@2021-02-01' = {
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

resource sqlDb 'Microsoft.Sql/servers/databases@2021-02-01-preview' existing = {
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
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP179", DiagnosticLevel.Warning, "Unique resource or deployment name is required when looping. The loop item variable \"thing\" must be referenced in at least one of the value expressions of the following properties: \"name\", \"parent\""),
                ("BCP170", DiagnosticLevel.Error, "Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name."),
                ("BCP179", DiagnosticLevel.Warning, "Unique resource or deployment name is required when looping. The loop item variable \"thing2\" must be referenced in at least one of the value expressions of the following properties: \"name\""),
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP179", DiagnosticLevel.Warning, "Unique resource or deployment name is required when looping. The loop item variable \"item\" must be referenced in at least one of the value expressions of the following properties: \"name\""),
                ("BCP178", DiagnosticLevel.Error, "This expression is being used in the for-expression, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"test\" -> \"rg\"). Properties of rg which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP177", DiagnosticLevel.Error, "This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment. Properties of rg2 which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\".")
            });
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"string\" but the provided value is of type \"null\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"scope\" expected a value of type \"resource | tenant\" but the provided value is of type \"null\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"string\" but the provided value is of type \"null\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"parent\" expected a value of type \"Microsoft.Network/dnsZones\" but the provided value is of type \"null\"."),
                ("BCP240", DiagnosticLevel.Error, "The \"parent\" property only permits direct references to resources. Expressions are not supported."),
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
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2248
        public void Test_Issue2248_UnionTypeInArrayAccessBaseExpression_NegativeCase()
        {
            var result = CompilationHelper.Compile(@"
var foos = true ? true : []
var primaryFoo = foos[0]
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP076", DiagnosticLevel.Error, "Cannot index over expression of type \"array | bool\". Arrays or objects are required.")
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
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Network/publicIPAddresses\" type, which requires a value that can be calculated at the start of the deployment.")
            });
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/2291
        public void Test_Issue2291()
        {
            var result = CompilationHelper.Compile(@"
resource registry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' existing = {
  name: 'foo'
  resource importPipeline 'importPipelines' existing = {
    name: 'import'
  }
}

resource pipelineRun 'Microsoft.ContainerRegistry/registries/pipelineRuns@2021-06-01-preview' = [for index in range(0, 3): if(registry::importPipeline.properties.trigger.sourceTrigger.status == 'Disabled') {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP177", DiagnosticLevel.Error, "This expression is being used in the if-condition expression, which requires a value that can be calculated at the start of the deployment. Properties of importPipeline which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\".")
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP009", DiagnosticLevel.Error, "Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.")
            });

            result.ExcludingLinterDiagnostics().Should().NotHaveDiagnosticsWithCodes(new[] { "BCP183" });
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

resource eventSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
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
                ("BCP240", DiagnosticLevel.Error, "The \"parent\" property only permits direct references to resources. Expressions are not supported."),
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"res2\" is declared but never used."),
                ("BCP036", DiagnosticLevel.Error, "The property \"parent\" expected a value of type \"Microsoft.Network/virtualNetworks\" but the provided value is of type \"tenant\"."),
                ("BCP240", DiagnosticLevel.Error, "The \"parent\" property only permits direct references to resources. Expressions are not supported."),
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

resource registry 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
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
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
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

        // https://github.com/Azure/bicep/issues/1228
        [TestMethod]
        public void Test_Issue1228()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'managementGroup'

resource policy01 'Microsoft.Authorization/policyDefinitions@2020-09-01' = {
  name: 'Allowed locations'
  properties: {
    policyType: 'Custom'
    mode: 'All'
    policyRule: {
      if: {
         field: 'location'
         notIn: [
           'westeurope'
        ]
      }
      then: {
         effect: 'Deny'
      }
   }
  }
}

resource initiative 'Microsoft.Authorization/policySetDefinitions@2020-09-01' = {
  name: 'Default initiative'
  properties: {
    policyDefinitions: [
      {
        policyDefinitionId: policy01.id
//        policyDefinitionId: '/providers/Microsoft.Management/managementGroups/MYMANAGEMENTGROUP/providers/${policy01.id}'
      }
    ]
  }
}
");

            result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'Default initiative')].properties.policyDefinitions[0].policyDefinitionId", "[extensionResourceId(managementGroup().id, 'Microsoft.Authorization/policyDefinitions', 'Allowed locations')]");

            var evaluated = TemplateEvaluator.Evaluate(result.Template);
            evaluated.Should().HaveValueAtPath("$.resources[?(@.name == 'Default initiative')].properties.policyDefinitions[0].policyDefinitionId", "/providers/Microsoft.Management/managementGroups/3fc9f36e-8699-43af-b038-1c103980942f/providers/Microsoft.Authorization/policyDefinitions/Allowed locations");
        }

        // https://github.com/Azure/bicep/issues/4850
        [TestMethod]
        public void Test_Issue4850()
        {
            // missing new line at the start and end of the object
            var result = CompilationHelper.Compile(@"
resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: 'foo'
  resource eventHubConnectionString 'secrets' existing = {
    name: 'eh-connectionstring'
  }
}

resource ehConn 'Microsoft.KeyVault/vaults/secrets@2021-06-01-preview' existing = {
  parent: keyVault
  name: 'eh-connectionstring'
}

var settings = [
  {
    name: 'ThisFails'
    value: '@Microsoft.KeyVault(SecretUri=${keyVault::eventHubConnectionString.properties.secretUriWithVersion})'
  }
]");

            result.Template.Should().NotHaveValueAtPath("$.variables");
            result.Should().OnlyContainDiagnostic("no-unused-vars", DiagnosticLevel.Warning, "Variable \"settings\" is declared but never used.");
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3934
        /// </summary>
        [TestMethod]
        public void Test_Issue3934()
        {
            var result = CompilationHelper.Compile(@"
param paramString string

#disable-next-line no-loc-expr-outside-params
output out1 string = paramString + resourceGroup().location
output out2 string = paramString + 'world'
output out3 string = paramString + paramString
output out4 string = 'hello' + 'world'
");

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP045", DiagnosticLevel.Error, "Cannot apply operator \"+\" to operands of type \"string\" and \"string\". Use string interpolation instead."),
                ("BCP045", DiagnosticLevel.Error, "Cannot apply operator \"+\" to operands of type \"string\" and \"'world'\". Use string interpolation instead."),
                ("BCP045", DiagnosticLevel.Error, "Cannot apply operator \"+\" to operands of type \"string\" and \"string\". Use string interpolation instead."),
                ("BCP045", DiagnosticLevel.Error, "Cannot apply operator \"+\" to operands of type \"'hello'\" and \"'world'\". Use string interpolation instead.")
            });
        }

        // https://github.com/Azure/bicep/issues/3749
        [TestMethod]
        public void Test_Issue3749()
        {
            // missing new line at the start and end of the object
            var result = CompilationHelper.Compile(@"
param foo string
param bar string

output out1 string = foo
");

            result.Template.Should().NotHaveValueAtPath("$.functions");
            result.Should().OnlyContainDiagnostic("no-unused-params", DiagnosticLevel.Warning, "Parameter \"bar\" is declared but never used.");
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5099
        /// </summary>
        [TestMethod]
        public void Test_Issue5099()
        {
            var result = CompilationHelper.Compile(("foo.bicep", @"param input array
output out array = input"), ("main.bicep", @"targetScope = 'subscription'

@description('rgNames param')
param rgNames array = [
  'hello'
  'world'
]

@description('resource group in for loop')
resource rgs 'Microsoft.Resources/resourceGroups@2019-10-01' = [for rgName in rgNames: {
  name: rgName
  location: deployment().location
}]

@description('module loop')
module m 'foo.bicep' = [for (rgName, i) in rgNames: {
  name: 'foo${rgName}'
  scope: rgs[i]
  params: {
    input: rgName
  }
}]

@description('The Resources Ids of the API management service product groups')
output productGroupsResourceIds array = [for rgName in rgNames: resourceId('Microsoft.Resources/resourceGroups', rgName)]
"));
            result.Template.Should().NotBeNull();
            var templateContent = result.Template!.ToString();

            result.Template!.Should().HaveValueAtPath("$.parameters.rgNames.metadata.description", "rgNames param");
            result.Template!.Should().HaveValueAtPath("$.resources[?(@.name == '[parameters(\\'rgNames\\')[copyIndex()]]')].metadata.description", "resource group in for loop");
            result.Template!.Should().HaveValueAtPath("$.resources[?(@.name == '[format(\\'foo{0}\\', parameters(\\'rgNames\\')[copyIndex()])]')].metadata.description", "module loop");
            result.Template!.Should().HaveValueAtPath("$.outputs.productGroupsResourceIds.metadata.description", "The Resources Ids of the API management service product groups");
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/5371
        public void Test_Issue5371_positive_test()
        {
            var result = CompilationHelper.Compile(@"
var myValue = -9223372036854775808
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables.myValue", "[json('-9223372036854775808')]");
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/5371
        public void Test_Issue5371_positive_test_2()
        {
            var result = CompilationHelper.Compile(@"
var myValue = 9223372036854775807
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables.myValue", 9223372036854775807);
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/5371
        public void Test_Issue5371_positive_test_3()
        {
            var result = CompilationHelper.Compile(@"
var myValue = -2147483648
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables.myValue", -2147483648);
        }

        [TestMethod]
        // https://github.com/Azure/bicep/issues/5371
        public void Test_Issue5371_positive_test_4()
        {
            var result = CompilationHelper.Compile(@"
var myValue = 2147483647
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables.myValue", 2147483647);
        }

        [DataTestMethod]
        [DataRow("var myValue = -9223372036854775809")]
        [DataRow("var myValue = 9223372036854775808")]
        // https://github.com/Azure/bicep/issues/5371
        public void Test_Issue5371_negative_tests(string fileContents)
        {
            var result = CompilationHelper.Compile(fileContents);

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP010", DiagnosticLevel.Error, "Expected a valid 64-bit signed integer.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5456
        /// </summary>
        [TestMethod]
        public void Test_Issue5456_1()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[]
            {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new[]
                {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant, "name property"),
                    new TypeProperty("tags", LanguageConstants.Array, TypePropertyFlags.ReadOnly, "tags property"),
                    new TypeProperty("properties", new ObjectType("properties", TypeSymbolValidationFlags.Default, new[]
                    {
                        new TypeProperty("prop1", LanguageConstants.String, TypePropertyFlags.ReadOnly, "prop1")
                    }, null), TypePropertyFlags.ReadOnly, "properties property"),
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
  tags: [
    'tag1'
  ]
  properties: {
    prop1: 'value'
  }
}
"));
            result.Should().GenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP073", DiagnosticLevel.Warning, "The property \"tags\" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP073", DiagnosticLevel.Warning, "The property \"properties\" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5456
        /// </summary>
        [TestMethod]
        public void Test_Issue5456_2()
        {

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(("module.bicep", @""), ("main.bicep", @"
module mod 'module.bicep' = {
  name: 'module'
  outputs: {}
}
"));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP073", DiagnosticLevel.Error, "The property \"outputs\" is read-only. Expressions cannot be assigned to read-only properties.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3114
        /// </summary>
        [TestMethod]
        public void Test_Issue3114()
        {
            var result = CompilationHelper.Compile(@"
output contentVersion string = deployment().properties.template.contentVersion
");
            result.Template.Should().NotBeNull();
            result.Template.Should().HaveValueAtPath("$.outputs['contentVersion'].value", "[deployment().properties.template.contentVersion]");
        }

        // https://github.com/Azure/bicep/issues/6044
        [TestMethod]
        public void Test_Issue6044()
        {
            var services = new ServiceBuilder().WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, symbolicNameCodegenEnabled: true));

            var result = CompilationHelper.Compile(services, @"
var adminUsername = 'cooluser'

resource server 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: 'sql-${uniqueString(resourceGroup().id)}'
  location: resourceGroup().location
  properties: {
    administratorLogin: adminUsername
  }

  resource db 'databases' = {
    name: 'cool-database'
    location: resourceGroup().location
  }

  resource firewall 'firewallRules' = {
    name: 'allow'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
}

resource server2 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: 'sql'
  location: resourceGroup().location
  properties: {
    administratorLogin: adminUsername
  }

  resource db 'databases' = {
    name: 'cool-database2'
    location: resourceGroup().location
  }

  resource firewall 'firewallRules' = {
    name: 'test'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
}

output foo string = server2::firewall.name
output bar string = server2::firewall.properties.startIpAddress
");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().NotHaveValueAtPath("$.resources.db");
            result.Template.Should().NotHaveValueAtPath("$.resources.firewall");

            result.Template.Should().HaveValueAtPath("$.resources['server::db'].name", "[format('{0}/{1}', format('sql-{0}', uniqueString(resourceGroup().id)), 'cool-database')]");
            result.Template.Should().HaveValueAtPath("$.resources['server::firewall'].name", "[format('{0}/{1}', format('sql-{0}', uniqueString(resourceGroup().id)), 'allow')]");
            result.Template.Should().HaveValueAtPath("$.resources['server2::db'].name", "[format('{0}/{1}', 'sql', 'cool-database2')]");
            result.Template.Should().HaveValueAtPath("$.resources['server2::firewall'].name", "[format('{0}/{1}', 'sql', 'test')]");

            result.Template.Should().HaveValueAtPath("$.outputs['foo'].value", "[resourceInfo('server2::firewall').name]");
            result.Template.Should().HaveValueAtPath("$.outputs['bar'].value", "[reference('server2::firewall').startIpAddress]");
        }

        // https://github.com/Azure/bicep/issues/4833
        [TestMethod]
        public void Test_Issue4833()
        {
            var result = CompilationHelper.Compile(@"
param storageName string

resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: storageName
}

var storage = stg

output badResult object = {
  value: storage.listAnything().keys[0].value
}");

            result.Template.Should().HaveValueAtPath("$.outputs['badResult'].value", new JObject
            {
                ["value"] = "[listAnything(resourceId('Microsoft.Storage/storageAccounts', parameters('storageName')), '2021-04-01').keys[0].value]",
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5530
        /// </summary>
        [TestMethod]
        public void Test_Issue_5530()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'subscription'

resource foo 'Microsoft.AAD/domainServices@2021-05-01' existing = {
  scope: resourceGroup
  name: 'foo'
}

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01'
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5530
        /// </summary>
        [TestMethod]
        public void Test_Issue_5530_2()
        {
            var result = CompilationHelper.Compile(@"
targetScope = 'tenant'

resource foo 'Microsoft.Authorization/policyAssignments@2021-06-01' existing = {
  scope: managementGroup
  name: 'foo'
}

resource managementGroup 'Microsoft.Management/managementGroups@2021-04-01'
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/6224
        /// </summary>
        [TestMethod]
        public void Test_Issue_6224()
        {
            var inputFile = FileHelper.SaveResultFile(TestContext, "main.bicep", @"
var text = loadTextContent('./con')
var text2 = loadTextContent('./con.txt')
var base64 = loadFileAsBase64('./con')
var base64_2 = loadFileAsBase64('./con.txt')

module test './con'

module test './con.txt'

");

            // the bug was that the compilation would not complete
            var compilation = Services.BuildCompilation(ImmutableDictionary.Create<Uri, string>(), new Uri(inputFile));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().NotBeEmpty();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3169
        /// </summary>
        [TestMethod]
        public void Test_Issue_3169()
        {
            var result = CompilationHelper.Compile(@"
resource newStg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}

resource existingStg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: newStg.properties.accessTier
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3169
        /// </summary>
        [TestMethod]
        public void Test_Issue_3169_2()
        {
            var result = CompilationHelper.Compile(@"
resource newStg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}

resource existingStg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: newStg.properties.accessTier
}

resource foo 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: existingStg.name
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"name\" property of the \"Microsoft.Storage/storageAccounts\" type, which requires a value that can be calculated at the start of the deployment. Properties of existingStg which can be calculated at the start include \"apiVersion\", \"id\", \"type\".")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3169
        /// </summary>
        [TestMethod]
        public void Test_Issue_3169_3()
        {
            var result = CompilationHelper.Compile(@"
resource newStg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}

resource existingStg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: newStg.name
}

resource newStg2 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: existingStg.name
  kind: 'BlobStorage'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[?(@.kind == 'BlobStorage')].name", "test");
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3169
        /// </summary>
        [TestMethod]
        public void Test_Issue_3169_4()
        {
            var result = CompilationHelper.Compile(@"
resource newStg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}

resource existingStg1 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: existingStg2.name
}

resource existingStg2 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: newStg.properties.accessTier
}

resource existingStg3 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: existingStg1.name
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3169
        /// </summary>
        [TestMethod]
        public void Test_Issue_3169_5()
        {
            var result = CompilationHelper.Compile(@"
resource newStg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
}

resource existingStg1 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: existingStg2.name
}

resource existingStg2 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: newStg.properties.accessTier
}

resource existingStg3 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: existingStg2.name
}

resource existingStg4 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: existingStg1.name
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/6423
        /// </summary>
        [TestMethod]
        public void Test_Issue6423()
        {
            var result = CompilationHelper.Compile(@"
var configs = [
  {
    name: 'name1'
  }
  {
    name: 'name2'
  }
]

resource webApp 'Microsoft.Web/sites@2021-03-01' = [for c in configs: {
  name: c.name
  location: 'West US'
}]

resource auth 'Microsoft.Web/sites/config@2021-03-01' = [for (c, i) in configs: {
  name: 'authsettingsV2'
  parent: webApp[i] // webApp[0] doesn't work either
  properties: {
    madeUpProperty: 'blah'
    // No IntelliSense or type checking
  }
}]
");

            // verify we have diagnostics for 'properties'
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP037", DiagnosticLevel.Warning, "The property \"madeUpProperty\" is not allowed on objects of type \"SiteAuthSettingsV2Properties\". Permissible properties include \"globalValidation\", \"httpSettings\", \"identityProviders\", \"login\", \"platform\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });

            result.Template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', variables('configs')[copyIndex()].name, 'authsettingsV2')]");
            result.Template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray
            {
                "[resourceId('Microsoft.Web/sites', variables('configs')[copyIndex()].name)]",
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3356
        /// </summary>
        [TestMethod]
        public void Test_Issue_3356_Accept_Correct_Type_Definitions()
        {
            var result = CompilationHelper.Compile(@"
resource msi 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'myIdentity'
  location: resourceGroup().location
}

#disable-next-line BCP081
resource foo 'Microsoft.Storage/storageAccounts@2021-09-00' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
    capacity: 1
  }
  extendedLocation: {
    type: 'NotSpecified'
  }
  scale: {
    capacity: 2
    minimum: 1
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${msi.id}': {}
    }
  }
}

output fooIdProps object = {
  clientId: foo.identity.userAssignedIdentities[msi.id].clientId
  principalId: foo.identity.userAssignedIdentities[msi.id].principalId
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3356
        /// </summary>
        [TestMethod]
        public void Test_Issue_3356_Warn_On_Bad_Type_Definitions()
        {
            var result = CompilationHelper.Compile(@"
resource foo 'Microsoft.Storage/storageAccounts@2021-09-00' = {
  name: 'test'
  kind: 'StorageV2'
  location: resourceGroup().location
  sku: {
    name: 123
    capacity: '1'

  }
  extendedLocation: {
    type: 1
  }
  scale: {
    capacity: '2'
    minimum: 1
  }
  identity: {
    type: 'noType'
    tenantId: 3
    userAssignedIdentities: {
      'blah': {
        clientId: 1
        principalId: 2
      }
    }
  }
}

output fooBadIdProps object = {
  clientId: foo.identity.userAssignedIdentities['blah'].hello
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Microsoft.Storage/storageAccounts@2021-09-00\" does not have types available."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"name\" expected a value of type \"string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"capacity\" expected a value of type \"int\" but the provided value is of type \"'1'\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"type\" expected a value of type \"'ArcZone' | 'CustomLocation' | 'EdgeZone' | 'NotSpecified' | string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"capacity\" expected a value of type \"int\" but the provided value is of type \"'2'\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"tenantId\" expected a value of type \"string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"clientId\" expected a value of type \"string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"principalId\" expected a value of type \"string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                ("BCP053", DiagnosticLevel.Error, "The type \"userAssignedIdentityProperties\" does not contain property \"hello\". Available properties include \"clientId\", \"principalId\"."),
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/4600
        /// </summary>
        public void Test_Issue_4600()
        {
            var result = CompilationHelper.Compile(@"
param keyVaultRoleIds array = [
  //https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
  //Key Vault Secrets Officer
  'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  //Key Vault Crypto Officer
  '14b46e9e-c2b7-41b4-b07b-48a6ebf60603'
  //Key Vault Certificates Officer
  'a4417e6f-fecd-4de8-b567-7b0420556985'
]
param managedIdentityKeyVaultRoleId string = newGuid()
param userAssignedManagedIdentityId string
param userAssignedManagedIdentityName string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name: userAssignedManagedIdentityName
}

//Assign the managed identity access to the Key Vault
resource vaultAssignments 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for roleId in keyVaultRoleIds: {
  name: managedIdentityKeyVaultRoleId
  scope: managedIdentity
  dependsOn: [
    managedIdentity
  ]
  properties: {
   roleDefinitionId: roleId
   principalId: 'principalId-123'
  }
}]
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP179", DiagnosticLevel.Warning, "Unique resource or deployment name is required when looping. The loop item variable \"roleId\" must be referenced in at least one of the value expressions of the following properties: \"name\", \"scope\"")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/4600
        /// </summary>
        [TestMethod]
        public void Test_Issue_4600_2()
        {
            var result = CompilationHelper.Compile(@"
param keyVaultRoleIds array = [
  //https://docs.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
  //Key Vault Secrets Officer
  'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  //Key Vault Crypto Officer
  '14b46e9e-c2b7-41b4-b07b-48a6ebf60603'
  //Key Vault Certificates Officer
  'a4417e6f-fecd-4de8-b567-7b0420556985'
]
param managedIdentityKeyVaultRoleId string = newGuid()
param userAssignedManagedIdentityId string
param userAssignedManagedIdentityName string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' existing = {
  name: userAssignedManagedIdentityName
}

//Assign the managed identity access to the Key Vault
resource vaultAssignments 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = [for (roleId,index) in keyVaultRoleIds: {
  name: managedIdentityKeyVaultRoleId
  scope: managedIdentity
  dependsOn: [
    managedIdentity
  ]
  properties: {
   roleDefinitionId: roleId
   principalId: 'principalId-123'
  }
}]
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP179", DiagnosticLevel.Warning, "Unique resource or deployment name is required when looping. The loop item variable \"roleId\" or the index variable \"index\" must be referenced in at least one of the value expressions of the following properties in the loop body: \"name\", \"scope\"")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7241
        /// </summary>
        [TestMethod]
        public void Test_Issue_7241_1()
        {
            var result = CompilationHelper.Compile(@"
var foo = {
  copy: [
    {
      name: 'blah'
      count: '[notAFunction()]'
      input: {}
    }
  ]
}
var bar = {
  'copy': 'copy'
}
");

            // verify we have diagnostics for 'properties'
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().NotBeNull();

            using (new AssertionScope())
            {
                result.Template!.SelectToken("$.variables.foo").Should().NotBeNull()
                    .And.Subject.As<JObject>().Properties().ElementAt(0).Name.Should().Be("[string('copy')]");
                result.Template!.SelectToken("$.variables.bar").Should().NotBeNull()
                    .And.Subject.As<JObject>().Properties().ElementAt(0).Name.Should().Be("[string('copy')]");
                result.Template!.SelectToken("$.variables.bar").Should().NotBeNull()
                    .And.Subject.As<JObject>().Properties().ElementAt(0).Value.Should().DeepEqual("copy");
            }
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7241
        /// </summary>
        [DataTestMethod]
        [DataRow("copy")]
        [DataRow("COPY")]
        [DataRow("Copy")]
        [DataRow("CoPy")]
        public void Test_Issue_7241_2(string copy)
        {
            var result = CompilationHelper.Compile(@"
var " + copy + @" = {}
");


            using (new AssertionScope())
            {
                result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP239", DiagnosticLevel.Error, "Identifier \"copy\" is a reserved Bicep symbol name and cannot be used in this context.")
                });
                result.Template.Should().BeNull();
            }
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7154
        /// </summary>
        [TestMethod]
        public void Test_Issue_7154_Ternary_Syntax_Produces_Error()
        {
            var result = CompilationHelper.Compile(@"
var deployServerlessCosmosDb = true

resource cosmosDbServer 'Microsoft.DocumentDB/databaseAccounts@2021-07-01-preview' = {
  kind: 'GlobalDocumentDB'
  name: 'cosmosdbname'
  location: resourceGroup().location
  properties: {
    createMode: 'Default'
    locations: [
      {
        locationName: resourceGroup().location
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    diagnosticLogSettings: {
      enableFullTextQuery: 'None'
    }
  }
}

resource PassDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-07-01-preview' = {
  parent: cosmosDbServer
  name: 'PassDb'
  properties: {
    resource: {
      id: 'PassDb'
    }
  }
}

resource QPDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-07-01-preview' = if(!deployServerlessCosmosDb) {
  parent: cosmosDbServer
  name: 'QPDB'
  properties: {
    resource: {
      id: 'QPDB'
    }
  }
}

resource container_ActorColdStorage 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-07-01-preview' = {
  parent: deployServerlessCosmosDb? PassDb: QPDB
  name: 'ActorColdStorage'
  properties: {
    resource: {
      id: 'ActorColdStorage'
      partitionKey: {
        paths: [
          '/Type'
        ]
        kind: 'Hash'
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
    }
  }
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP240", DiagnosticLevel.Error, "The \"parent\" property only permits direct references to resources. Expressions are not supported.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7154
        /// </summary>
        [TestMethod]
        public void Test_Issue_7154_2_Ternary_Syntax_With_Parentheses_Produces_Error()
        {
            var result = CompilationHelper.Compile(@"
var deployServerlessCosmosDb = true

resource cosmosDbServer 'Microsoft.DocumentDB/databaseAccounts@2021-07-01-preview' = {
  kind: 'GlobalDocumentDB'
  name: 'cosmosdbname'
  location: resourceGroup().location
  properties: {
    createMode: 'Default'
    locations: [
      {
        locationName: resourceGroup().location
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
      maxIntervalInSeconds: 5
      maxStalenessPrefix: 100
    }
    diagnosticLogSettings: {
      enableFullTextQuery: 'None'
    }
  }
}

resource PassDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-07-01-preview' = {
  parent: cosmosDbServer
  name: 'PassDb'
  properties: {
    resource: {
      id: 'PassDb'
    }
  }
}

resource QPDB 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-07-01-preview' = if(!deployServerlessCosmosDb) {
  parent: cosmosDbServer
  name: 'QPDB'
  properties: {
    resource: {
      id: 'QPDB'
    }
  }
}

resource container_ActorColdStorage 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-07-01-preview' = {
  parent: (deployServerlessCosmosDb)? PassDb: QPDB
  name: 'ActorColdStorage'
  properties: {
    resource: {
      id: 'ActorColdStorage'
      partitionKey: {
        paths: [
          '/Type'
        ]
        kind: 'Hash'
      }
      conflictResolutionPolicy: {
        mode: 'LastWriterWins'
        conflictResolutionPath: '/_ts'
      }
    }
  }
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP240", DiagnosticLevel.Error, "The \"parent\" property only permits direct references to resources. Expressions are not supported.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7271
        /// </summary>
        [TestMethod]
        public void Test_Issue7271()
        {
            var result = CompilationHelper.Compile(@"
var less           = any(1) < any(2)
var lessOrEqual    = any(1) <= any(2)
var greater        = any(1) > any(2)
var greaterOrEqual = any(1) >= any(2)");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/6951
        /// </summary>
        [TestMethod]
        public void Test_Issue6951()
        {
            const string Main = @"
param SfAppCertificateSubjectNames array

module SfAppCertificates './certificate-generation.bicep' = [for cert in SfAppCertificateSubjectNames: {
  name: 'sfdsf'
  params: {

  }
}]

var nodeTypes = []

resource SFNodeTypes 'Microsoft.ServiceFabric/managedClusters/nodeTypes@2022-02-01-preview' = [for node in nodeTypes: if (node.instanceCount > 0) {
  name: node.name
  parent: SF
  properties: {
    //...
    vmSecrets: [for (subjectName, i) in SfAppCertificateSubjectNames if (contains(SfAppCertificateSubjectNames[i].targetNodeTypes, node.name)): {
      sourceVault: {
        id: resourceId('Microsoft.KeyVault/vaults', SfAppCertificates[i].outputs.KeyVaultName)
      }
      vaultCertificates: [
        {
          certificateStore: 'My'
          certificateUrl: SfAppCertificates[i].outputs.PublicCertificateUrl
        }
      ]
    }]
  }
}]
";
            var result = CompilationHelper.Compile(("main.bicep", Main), ("certificate-generation.bicep", string.Empty));

            // the above snippet is malformed but should not throw
            result.Diagnostics.Should().NotBeEmpty();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/2017
        /// </summary>
        [TestMethod]
        public void Test_Issue2017()
        {
            var result = CompilationHelper.Compile(@"
var providersTest = providers('Microsoft.Resources').namespace
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
").ExcludingLinterDiagnostics();

            result.Should().HaveDiagnostics(new [] {
                ("BCP241", DiagnosticLevel.Warning, "The \"providers\" function is deprecated and will be removed in a future release of Bicep. Please add a comment to https://github.com/Azure/bicep/issues/2017 if you believe this will impact your workflow."),
                ("BCP241", DiagnosticLevel.Warning, "The \"providers\" function is deprecated and will be removed in a future release of Bicep. Please add a comment to https://github.com/Azure/bicep/issues/2017 if you believe this will impact your workflow."),
            });

            result.Diagnostics.Should().OnlyContain(x => x.Styling == DiagnosticStyling.ShowCodeDeprecated);
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7482
        /// </summary>
        [TestMethod]
        public void Test_Issue7482()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
module optionModuleLoop 'module.bicep' = [for item in ['option:a','option:b']: {
  name: 'myOptionModule-${uniqueString(item)}'
  params: {
    option: item
  }
}]
"),
                ("module.bicep", @"
@allowed(['option:a','option:b', 'option:c', 'option:d'])
param option string

var optionsLUT = {
  'option:a': {
    text: 'Option A'
    value: 'a'
  }
  'option:b': {
    text: 'Option B'
    value: 'b'
  }
  'option:c': {
    text: 'Option C'
    value: 'c'
  }
}

var optionType = optionsLUT[option]

output optionTypeText string = optionType.text
output optionTypeValue string = optionType.value
")).ExcludingLinterDiagnostics();

            result.Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7482
        /// </summary>
        [TestMethod]
        public void Test_Issue7482_alternative()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
var options = ['option:a','option:b']
module optionModuleLoop 'module.bicep' = [for item in options: {
  name: 'myOptionModule-${uniqueString(item)}'
  params: {
    option: item
  }
}]
"),
                ("module.bicep", @"
@allowed(['option:a','option:b', 'option:c', 'option:d'])
param option string

var optionsLUT = {
  'option:a': {
    text: 'Option A'
    value: 'a'
  }
  'option:b': {
    text: 'Option B'
    value: 'b'
  }
  'option:c': {
    text: 'Option C'
    value: 'c'
  }
}

var optionType = optionsLUT[option]

output optionTypeText string = optionType.text
output optionTypeValue string = optionType.value
")).ExcludingLinterDiagnostics();

            result.Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7861
        /// </summary>
        [TestMethod]
        public void Test_Issue7861()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
param objectId string
param keyvaultName string

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyvaultName

  resource accessPolicy 'accessPolicies' = {
    name: 'add'
    properties: {
      accessPolicies: [
        {
          tenantId: subscription().tenantId
          objectId: objectId
          permissions: {
            certificates: [
              'get'
            ]
            secrets: [
              'get'
            ]
            keys:[
              'get'
            ]
          }
        }
      ]
    }
  }
}
"));

            result.Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/6477
        /// </summary>
        [TestMethod]
        public void Test_Issue6477()
        {
            var result = CompilationHelper.Compile(@"
param storageAccountName string

@allowed([
  'Standard_GRS'
  'Standard_ZRS'
])
@description('Storage account SKU.  Standard_ZRS should be used if region supports, else Standard_GRS.')
param storageAccountSku string

resource storageAccountName_resource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  #disable-next-line no-loc-expr-outside-params
  location: resourceGroup().location
  sku: {
    name: storageAccountSku
    #disable-next-line BCP073
    tier: 'Standard'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: true // Ibiza requires anonymous access to the container
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    encryption: {
      services: {
        file: {
          enabled: true
        }
        blob: {
          enabled: true
        }
      }
      keySource: 'Microsoft.Storage'
    }
    accessTier: 'Hot'
  }

  resource blobs 'blobServices' existing = {
    name: 'default'

    resource containers 'containers' = {
      name: 'extension'
      properties: {
        publicAccess: 'Container'
      }
    }
  }
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.resources[0].type", "Microsoft.Storage/storageAccounts/blobServices/containers");
            result.Template.Should().HaveValueAtPath("$.resources[0].dependsOn", new JArray("[resourceId('Microsoft.Storage/storageAccounts', parameters('storageAccountName'))]"));

            result.Template.Should().HaveValueAtPath("$.resources[1].type", "Microsoft.Storage/storageAccounts");
            result.Template.Should().NotHaveValueAtPath("$.resources[1].dependsOn");
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/7455
        /// </summary>
        [TestMethod]
        public void Test_Issue7455()
        {
            var result = CompilationHelper.Compile(@"
var test1  = {
  'tata':'loco'
}

var test2 = {
  'tata':'cola'
}

param useFirst bool = true

var value = (useFirst ? test1 : test2).tata
").ExcludingLinterDiagnostics();

            result.Should().NotHaveAnyDiagnostics();
       }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/6863
        /// </summary>
        [TestMethod]
        public void Test_Issue6863()
        {
            var result = CompilationHelper.Compile(@"
@description('Region to deploy to')
param Location string = resourceGroup().location

var Names = [
  'fruit-primary'
  'fruit-secondary'
]

var Service_Bus_Queues = [
  'apples'
  'oranges'
]

resource serviceBuses 'Microsoft.ServiceBus/namespaces@2021-11-01' = [for name in Names: {
  name: name
  location: Location
  sku: {
    name: 'Premium'
    tier: 'Premium'
  }
  properties: {
    zoneRedundant: false
  }
}]

resource queues 'Microsoft.ServiceBus/namespaces/queues@2021-11-01' = [for item in Service_Bus_Queues: {
  parent: serviceBuses[0]
  name: item
}]

resource queueAuthorizationRules 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2021-11-01' = [for (item, index) in Service_Bus_Queues: {
  parent: queues[index]
  name: 'Listen'
  properties: {
    rights: [
      'Listen'
    ]
  }
}]
");

            result.Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.resources[0].copy.name", "serviceBuses");
            result.Template.Should().HaveValueAtPath("$.resources[0].name", "[variables('Names')[copyIndex()]]");
            result.Template.Should().NotHaveValueAtPath("$.resources[0].dependsOn");

            result.Template.Should().HaveValueAtPath("$.resources[1].copy.name", "queues");
            result.Template.Should().HaveValueAtPath("$.resources[1].name", "[format('{0}/{1}', variables('Names')[0], variables('Service_Bus_Queues')[copyIndex()])]");
            result.Template.Should().HaveValueAtPath("$.resources[1].dependsOn", new JArray("[resourceId('Microsoft.ServiceBus/namespaces', variables('Names')[0])]"));

            result.Template.Should().HaveValueAtPath("$.resources[2].copy.name", "queueAuthorizationRules");
            result.Template.Should().HaveValueAtPath("$.resources[2].name", "[format('{0}/{1}/{2}', variables('Names')[0], variables('Service_Bus_Queues')[copyIndex()], 'Listen')]");
            result.Template.Should().HaveValueAtPath("$.resources[2].dependsOn", new JArray("[resourceId('Microsoft.ServiceBus/namespaces/queues', variables('Names')[0], variables('Service_Bus_Queues')[copyIndex()])]"));
        }
    }
}
