// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using Bicep.Core.UnitTests.Utils;
using Newtonsoft.Json.Linq;
using FluentAssertions.Execution;
using System.ComponentModel.DataAnnotations;
using Bicep.Core.Semantics;
using System.Linq;
using Bicep.Core.TypeSystem;
using Bicep.Core.Resources;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
        [TestMethod]
        public void Test_Issue746()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
var l = l
param l
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP014", DiagnosticLevel.Error, "Expected a parameter type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue801()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                // ensure we're generating the correct expression with 'subscriptionResourceId', and using the correct name for the module
                template.Should().HaveValueAtPath("$.outputs['vnetid'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetId.value]");
                template.Should().HaveValueAtPath("$.outputs['vnetstate'].value", "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetstate.value]");
            }
        }

        [TestMethod]
        public void Test_Issue982()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.outputs['config'].value", "[list(format('{0}/config/appsettings', resourceId('Microsoft.Web/sites', parameters('functionApp').name)), '2020-06-01')]");
            }
        }

        [TestMethod]
        public void Test_Issue1093()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[?(@.name == 'rg30')].location", "[deployment().location]");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'rg31')].location", "[deployment().location]");
            }
        }

        [TestMethod]
        public void Test_Issue1173()
        {
            var (template, _, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                // variable 'subnets' should have been inlined
                template.Should().HaveValueAtPath("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value[0].name", "GatewaySubnet");
                template.Should().HaveValueAtPath("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value[1].name", "appsn01");
                // there should be no definition in the variables list for 'subnets'
                template.Should().NotHaveValueAtPath("$.variables.subnets");
            }
        }

        [TestMethod]
        public void Test_Issue1185()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                // deploying a management group module at tenant scope requires an unqualified resource id
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'allupmgdeploy')].scope", "[format('Microsoft.Management/managementGroups/{0}', parameters('allUpMgName'))]");
            }
        }

        [TestMethod]
        public void Test_Issue1332()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
var propname = 'ptest'
var issue = true ? {
    prop1: {
        '${propname}': {}
    }
} : {}
");

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.variables.issue", "[if(true(), createObject('prop1', createObject(variables('propname'), createObject())), createObject())]");
            }
        }

        [TestMethod]
        public void Test_Issue486()
        {
            var (template, _, _) = CompilationHelper.Compile(@"
var myInt = 5
var myBigInt = 2199023255552
var myIntExpression = 5 * 5
var myBigIntExpression = 2199023255552 * 2
var myBigIntExpression2 = 2199023255552 * 2199023255552
");

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.variables.myInt", 5);
                template.Should().HaveValueAtPath("$.variables.myBigInt", 2199023255552);
                template.Should().HaveValueAtPath("$.variables.myIntExpression", "[mul(5, 5)]");
                template.Should().HaveValueAtPath("$.variables.myBigIntExpression2", "[mul(json('2199023255552'), json('2199023255552'))]");
            }
        }

        [TestMethod]
        public void Test_Issue1362_1()
        {
            var (template, _, _) = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'resourceGroup'

module sub './modules/subscription.bicep' = {
  name: 'subDeploy'
  scope: subscription()
}"),
                ("modules/subscription.bicep", @"
targetScope = 'subscription'
"));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].subscriptionId", "[subscription().subscriptionId]");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].location", "[resourceGroup().location]");
            }
        }

        [TestMethod]
        public void Test_Issue1362_2()
        {
            var (template, _, _) = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'resourceGroup'

module sub './modules/subscription.bicep' = {
  name: 'subDeploy'
  scope: subscription('abcd-efgh')
}"),
                ("modules/subscription.bicep", @"
targetScope = 'subscription'
"));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].subscriptionId", "abcd-efgh");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].location", "[resourceGroup().location]");
            }
        }

        [TestMethod]
        public void Test_Issue1402()
        {
            var (template, _, _) = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = 'subscription'

module sub './modules/resourceGroup.bicep' = {
  name: 'subDeploy'
  scope: resourceGroup('abcd-efgh','bicep-rg')
}"),
                ("modules/resourceGroup.bicep", @"
targetScope = 'resourceGroup'
"));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].subscriptionId", "abcd-efgh");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'subDeploy')].resourceGroup", "bicep-rg");
                template.Should().NotHaveValueAtPath("$.resources[?(@.name == 'subDeploy')].location");
            }
        }

        [TestMethod]
        public void Test_Issue1391()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[?(@.name == 'nestedDeployment')].subscriptionId", "abcd-efgh");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'nestedDeployment')].resourceGroup", "bicep-rg");
            }
        }

        [TestMethod]
        public void Test_Issue1454()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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
                
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP120", DiagnosticLevel.Error, "The property \"scope\" must be evaluable at the start of the deployment, and cannot depend on any values that have not yet been calculated. You are referencing a variable which cannot be calculated in time (\"appResGrp\" -> \"rg\"). Accessible properties of rg are \"name\", \"scope\"."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue1465()
        {
            var (template, _, _) = CompilationHelper.Compile(@"
resource foo 'Microsoft.foo/bar@2020-01-01' existing = {
  name: 'name'
}
output prop1 string = foo.properties.prop1
");

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.outputs['prop1'].value", "[reference(resourceId('Microsoft.foo/bar', 'name'), '2020-01-01').prop1]");
            }
        }

        [TestMethod]
        public void Test_Issue822()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().NotHaveValueAtPath("$.resources[?(@.name == 'vnet')].subscriptionId");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].resourceGroup", "rg");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].dependsOn[0]", "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'rg')]");
            }
        }

        [TestMethod]
        public void Test_Issue822_scoped()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].subscriptionId", "abcdef");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'vnet')].resourceGroup", "rg");
            }
        }

        [TestMethod]
        public void Test_Issue1388()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
                    ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
                    ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
                });
            }
        }
        
        [TestMethod]
        public void Test_Issue1364()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
targetScope = 'blablah'
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'\" but the provided value is of type \"'blablah'\"."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue569_success()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
param myparam string
var myvar = 'hello'
        
output myparam string = myparam
output myvar string = myvar
");

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.outputs['myparam'].value", "[parameters('myparam')]");
                template.Should().HaveValueAtPath("$.outputs['myvar'].value", "[variables('myvar')]");
            }
        }

        [TestMethod]
        public void Test_Issue569_duplicates()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
output duplicate string = 'hello'
output duplicate string = 'hello'
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP145", DiagnosticLevel.Error, "Output \"duplicate\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP145", DiagnosticLevel.Error, "Output \"duplicate\" is declared multiple times. Remove or rename the duplicates."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue569_outputs_cannot_be_referenced()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
output output1 string = 'hello'
output output2 string = output1
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP058", DiagnosticLevel.Error, "The name \"output1\" is an output. Outputs cannot be referenced in expressions."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue1599()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
param x string = 't'
output xx = x
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP146", DiagnosticLevel.Error, "Expected an output type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue1661()
        {
            // Issue 1661 only repros if global-resources.bicep exists and kevault-secrets.bicep does not
            var (template, diags, _) = CompilationHelper.Compile(("main.bicep", @"
targetScope = 'subscription'

param prefix string
param deploymentId string
param tags object

param stampLocations array {
  default: [
    'northeurope'
    'eastus2'
  ]
}

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

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diags.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"rg_global\" does not exist in the current context.");
                diags.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"rg_global\" does not exist in the current context.");
                diags.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"stamps\" does not exist in the current context.");
                diags.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"stamps\" does not exist in the current context.");
                diags.Should().ContainDiagnostic("BCP057", DiagnosticLevel.Error, "The name \"stamps\" does not exist in the current context.");
                diags.Should().ContainDiagnostic("BCP052", DiagnosticLevel.Error, "The type \"outputs\" does not contain property \"cosmosDbEndpoint\".");
                diags.Should().ContainDiagnostic("BCP052", DiagnosticLevel.Error, "The type \"outputs\" does not contain property \"cosmosDbKey\".");
            }
        }

        [TestMethod]
        public void Test_Issue1592()
        {
            var (template, diags, _) = CompilationHelper.Compile(
              ("main.bicep", @"
module foo 'test.bicep' = {
  name: 'foo'
}

output fooName string = foo.name
    "),
              ("test.bicep", @""));

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.outputs['fooName'].value", "foo");
            }
        }

        [TestMethod]
        public void Test_Issue1592_special_cases()
        {
            var (template, diags, _) = CompilationHelper.Compile(
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

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.outputs['fooName'].value", "[format('{0}-test', parameters('someParam'))]");
                template.Should().HaveValueAtPath("$.outputs['fooOutput'].value", "[reference(resourceId('Microsoft.Resources/deployments', format('{0}-test', parameters('someParam'))), '2019-10-01').outputs.test.value]");
            }
        }

        [TestMethod]
        public void Test_Issue1432()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
resource foo 'Microsoft.Compute/virtualMachines@2020-06-01' = {
  name: 'myVM'
  name: 'myVm'
}
");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP025", DiagnosticLevel.Error, "The property \"name\" is declared multiple times in this object. Remove or rename the duplicate properties."),
                    ("BCP025", DiagnosticLevel.Error, "The property \"name\" is declared multiple times in this object. Remove or rename the duplicate properties."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue1817()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
targetScope = w

var foo = 42
");

            using (new AssertionScope())
            {
                template.Should().BeNull();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP032", DiagnosticLevel.Error, "The value must be a compile-time constant."),
                    ("BCP057", DiagnosticLevel.Error, "The name \"w\" does not exist in the current context."),
                });
            }
        }

        [TestMethod]
        public void Test_Issue1630()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
var singleResource = providers('Microsoft.Insights', 'components')
var allResources = providers('Microsoft.Insights')

// singleResource is an object!
var firstApiVersion = singleResource.apiVersions[0]

// allResources is an array of objects!
var firstResourceFirstApiVersion = allResources[0].apiVersions[0]

output singleResource object = singleResource
output allResources array = allResources
");

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.variables['singleResource']", "[providers('Microsoft.Insights', 'components')]");
                template.Should().HaveValueAtPath("$.variables['firstApiVersion']", "[variables('singleResource').apiVersions[0]]");
                template.Should().HaveValueAtPath("$.variables['allResources']", "[providers('Microsoft.Insights')]");
                template.Should().HaveValueAtPath("$.variables['firstResourceFirstApiVersion']", "[variables('allResources')[0].apiVersions[0]]");
            }
        }

        [TestMethod]
        public void Test_Issue1627()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    foo: 'test'
  }
}

var bar = modulea.outputs.bar
",
                [moduleAUri] = @"
// duplicate parameter symbols
param foo string
param foo int

// duplicate output symbols
output bar bool = true
output bar int = 42
",
            };


            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxTreeGroupingFactory.CreateForFiles(files, mainUri));

            var diagnosticsByFile = compilation.GetAllDiagnosticsBySyntaxTree().ToDictionary(kvp => kvp.Key.FileUri, kvp => kvp.Value);
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
            });
        }

        [TestMethod]
        public void Test_Issue1941()
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                // verify the template still compiles
                template.Should().NotBeNull();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP174", DiagnosticLevel.Warning, "Type validation is not available for resource types declared containing a \"/providers/\" segment. Please instead use the \"scope\" property. See https://aka.ms/BicepScopes for more information."),
                });
            }

            (template, diags, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                // we show the regular missing type warning for "providers" at the beginning or end of type segment
                template.Should().NotBeNull();
                diags.Should().HaveDiagnostics(new[] {
                    ("BCP081", DiagnosticLevel.Warning, "Resource type \"Rp.A/providers@2020-06-01\" does not have types available."),
                    ("BCP081", DiagnosticLevel.Warning, "Resource type \"Rp.A/providers/a/b@2020-06-01\" does not have types available."),
                    ("BCP081", DiagnosticLevel.Warning, "Resource type \"Rp.A/a/b/providers@2020-06-01\" does not have types available."),
                });
            }

            (template, diags, _) = CompilationHelper.Compile(@"
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

            using (new AssertionScope())
            {
                // verify the 'fixed' version compiles without diagnostics
                template.Should().NotBeNull();
                diags.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Test_Issue657_discriminators()
        {
            var customTypes = new [] {
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    ResourceTypeProviderHelper.CreateObjectType(
                        "Rp.A/parent@2020-10-01",
                        ("name", LanguageConstants.String))),
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent/child@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    ResourceTypeProviderHelper.CreateDiscriminatedObjectType(
                        "Rp.A/parent/child@2020-10-01",
                        "name",
                        ResourceTypeProviderHelper.CreateObjectType(
                            "Val1Type",
                            ("name", new StringLiteralType("val1")),
                            ("properties", ResourceTypeProviderHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal1", LanguageConstants.Bool)))),
                        ResourceTypeProviderHelper.CreateObjectType(
                            "Val2Type",
                            ("name", new StringLiteralType("val2")),
                            ("properties", ResourceTypeProviderHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal2", LanguageConstants.Bool)))))),
            };

            var result = CompilationHelper.Compile(
                ResourceTypeProviderHelper.CreateMockTypeProvider(customTypes),
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

            result.Should().NotHaveDiagnostics();

            var failedResult = CompilationHelper.Compile(
                ResourceTypeProviderHelper.CreateMockTypeProvider(customTypes),
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
        public void Test_Issue657_enum()
        {
            var customTypes = new [] {
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    ResourceTypeProviderHelper.CreateObjectType(
                        "Rp.A/parent@2020-10-01",
                        ("name", LanguageConstants.String))),
                new ResourceType(
                    ResourceTypeReference.Parse("Rp.A/parent/child@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    ResourceTypeProviderHelper.CreateObjectType(
                        "Rp.A/parent/child@2020-10-01",
                        ("name", UnionType.Create(new StringLiteralType("val1"), new StringLiteralType("val2"))),
                            ("properties", ResourceTypeProviderHelper.CreateObjectType(
                                "properties",
                                ("onlyOnEnum", LanguageConstants.Bool))))),
            };

            var result = CompilationHelper.Compile(
                ResourceTypeProviderHelper.CreateMockTypeProvider(customTypes),
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

            result.Should().NotHaveDiagnostics();

            var failedResult = CompilationHelper.Compile(
                ResourceTypeProviderHelper.CreateMockTypeProvider(customTypes),
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
    }
}