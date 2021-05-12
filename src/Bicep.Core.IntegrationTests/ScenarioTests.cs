// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Bicep.Core.UnitTests.Utils;
using Newtonsoft.Json.Linq;
using Bicep.Core.TypeSystem;
using Bicep.Core.Resources;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
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
            result.Template.Should().HaveValueAtPath("$.outputs['vnetid'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetId.value]");
            result.Template.Should().HaveValueAtPath("$.outputs['vnetstate'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetstate.value]");
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

param azRegion string {
  default: 'southcentralus'
}

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

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.variables.issue", "[if(true(), createObject('prop1', createObject(variables('propname'), createObject())), createObject())]");
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
                ("BCP120", DiagnosticLevel.Error, "The property \"scope\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. You are referencing a variable which cannot be calculated in time (\"appResGrp\" -> \"rg\"). Accessible properties of rg are \"name\"."),
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
            result.Template.Should().HaveValueAtPath("$.outputs['fooOutput'].value", "[reference(resourceId('Microsoft.Resources/deployments', format('{0}-test', parameters('someParam'))), '2019-10-01').outputs.test.value]");
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

            result.Should().NotHaveAnyDiagnostics();
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
                ("BCP174", DiagnosticLevel.Warning, "Type validation is not available for resource types declared containing a \"/providers/\" segment. Please instead use the \"scope\" property. See https://aka.ms/BicepScopes for more information."),
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
            var customTypes = new [] {
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
                TestTypeHelper.CreateProviderWithTypes(customTypes),
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
                TestTypeHelper.CreateProviderWithTypes(customTypes),
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
            var customTypes = new [] {
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
                TestTypeHelper.CreateProviderWithTypes(customTypes),
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
                TestTypeHelper.CreateProviderWithTypes(customTypes),
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

            var evaluated = TemplateEvaluator.Evaluate(result.Template, config => config with {
                Metadata = new() {
                    ["providers"] = JToken.FromObject(providersMetadata),
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providerOutput'].value.thing", new JObject {
                ["namespace"] = "Microsoft.Web",
                ["resourceTypes"] = new JArray {
                    new JObject {
                        ["resourceType"] = "sites",
                        ["locations"] = new JArray { "West US", "East US" },
                        ["apiVersions"] = new JArray { "2019-01-01", "2020-01-01" },
                    }
                }
            });

            evaluated.Should().HaveValueAtPath("$.outputs['providerOutput'].value.otherThing", new JObject {
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

            var evaluated = TemplateEvaluator.Evaluate(result.Template, config => config with {
                Metadata = new() {
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

            result.Should().NotHaveAnyDiagnostics();
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
                ("BCP037", DiagnosticLevel.Warning, "The property \"valThatDoesNotExist\" from source declaration \"vmNotWorkingProps\" is not allowed on objects of type \"VirtualMachineProperties\". Permissible properties include \"additionalCapabilities\", \"availabilitySet\", \"billingProfile\", \"diagnosticsProfile\", \"evictionPolicy\", \"extensionsTimeBudget\", \"hardwareProfile\", \"host\", \"hostGroup\", \"licenseType\", \"networkProfile\", \"osProfile\", \"priority\", \"proximityPlacementGroup\", \"securityProfile\", \"storageProfile\", \"virtualMachineScaleSet\"."),
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
                ("BCP120", DiagnosticLevel.Error, "The property \"location\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of vnet are \"apiVersion\", \"id\", \"name\", \"type\"."),
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
  name: 'myTde'
  parent: sqlDb
  properties: {
    status: sqlDatabase.dataEncryption
  }
}

output tdeId string = transparentDataEncryption.id
");

            var evaluated = TemplateEvaluator.Evaluate(result.Template);

            evaluated.Should().HaveValueAtPath("$.resources[0].name", "myServer/myDb/myTde");
            evaluated.Should().HaveValueAtPath("$.outputs['tdeId'].value", "/subscriptions/f91a30fd-f403-4999-ae9f-ec37a6d81e13/resourceGroups/testResourceGroup/providers/Microsoft.Sql/servers/myServer/databases/myDb/transparentDataEncryption/myTde");
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
                ("BCP170", DiagnosticLevel.Error, "Expected resource name to not contain any \"/\" characters. Child resources with a parent resource reference (via the parent property or via nesting) must not contain a fully-qualified name.")
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
                ("BCP120", DiagnosticLevel.Error, "The property \"tags\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of tags are \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "The property \"myTag1\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of tags are \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "The property \"tags\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of tags are \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "The property \"myTag1\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of tags are \"name\"."),
                ("BCP120", DiagnosticLevel.Error, "The property \"zones\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of vwan are \"apiVersion\", \"id\", \"name\", \"type\"."),
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
                ("BCP178", DiagnosticLevel.Error, "The for-expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of vnet are \"apiVersion\", \"id\", \"name\", \"type\".")
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
                ("BCP178", DiagnosticLevel.Error, "The for-expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. You are referencing a variable which cannot be calculated in time (\"test\" -> \"rg\"). Accessible properties of rg are \"apiVersion\", \"id\", \"name\", \"type\"."),
                ("BCP177", DiagnosticLevel.Error, "The if-condition expression must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of rg2 are \"apiVersion\", \"id\", \"name\", \"type\".")
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
                // TODO: change the first diagnostic once https://github.com/Azure/bicep/issues/2624 is fixed.
                ("BCP066", DiagnosticLevel.Error, "Function \"listSecrets\" is not valid at this location. It can only be used in resource declarations."),
                ("BCP181", DiagnosticLevel.Error, "The arguments of function \"listKeys\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of stgModule are \"name\"."),
                ("BCP181", DiagnosticLevel.Error, "The arguments of function \"listKeys\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. Accessible properties of stgModule are \"name\"."),
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
                ("BCP080", DiagnosticLevel.Error, "The expression is involved in a cycle (\"name\" -> \"nameCopy\")."),
            });
        }
    } }
