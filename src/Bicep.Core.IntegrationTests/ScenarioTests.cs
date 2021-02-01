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
                template!.Should().BeNull();
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
                template!.Should().NotBeNull();
                diags.Should().BeEmpty();

                // ensure we're generating the correct expression with 'subscriptionResourceId', and using the correct name for the module
                template!.SelectToken("$.outputs['vnetid'].value")!.Should().DeepEqual("[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetId.value]");
                template.SelectToken("$.outputs['vnetstate'].value")!.Should().DeepEqual("[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetstate.value]");
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
                template!.Should().NotBeNull();
                diags.Should().BeEmpty();

                template!.SelectToken("$.outputs['config'].value")!.Should().DeepEqual("[list(format('{0}/config/appsettings', resourceId('Microsoft.Web/sites', parameters('functionApp').name)), '2020-06-01')]");
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
                template!.Should().NotBeNull();
                diags.Should().BeEmpty();

                template!.SelectToken("$.resources[?(@.name == 'rg30')].location")!.Should().DeepEqual("[deployment().location]");
                template.SelectToken("$.resources[?(@.name == 'rg31')].location")!.Should().DeepEqual("[deployment().location]");
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

            template!.Should().NotBeNull();

            // variable 'subnets' should have been inlined
            template!.SelectToken("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value")!.Type.Should().Be(JTokenType.Array);
            template.SelectToken("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value[0].name")!.Should().DeepEqual("GatewaySubnet");
            template.SelectToken("$.resources[?(@.name == '[variables(\\'vnetName\\')]')].properties.parameters.subnets.value[1].name")!.Should().DeepEqual("appsn01");
            // there should be no definition in the variables list for 'subnets'
            template.SelectToken("$.variables.subnets")!.Should().BeNull();
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
                template!.Should().NotBeNull();
                diags.Should().BeEmpty();

                // deploying a management group module at tenant scope requires an unqualified resource id
                template!.SelectToken("$.resources[?(@.name == 'allupmgdeploy')].scope")!.Should().DeepEqual("[format('Microsoft.Management/managementGroups/{0}', parameters('allUpMgName'))]");
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
                template!.Should().NotBeNull();
                diags.Should().BeEmpty();

                template!.SelectToken("$.variables.issue")!.Should().DeepEqual("[if(true(), createObject('prop1', createObject(variables('propname'), createObject())), createObject())]");
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

            template!.Should().NotBeNull();
            using (new AssertionScope())
            {
                template!.SelectToken("$.variables.myInt")!.Should().DeepEqual(5);
                template.SelectToken("$.variables.myBigInt")!.Should().DeepEqual(2199023255552);
                template.SelectToken("$.variables.myIntExpression")!.Should().DeepEqual("[mul(5, 5)]");
                template.SelectToken("$.variables.myBigIntExpression2")!.Should().DeepEqual("[mul(json('2199023255552'), json('2199023255552'))]");
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

            template!.Should().NotBeNull();
            using (new AssertionScope())
            {
                template!.SelectToken("$.resources[?(@.name == 'subDeploy')].subscriptionId")!.Should().DeepEqual("[subscription().subscriptionId]");
                template.SelectToken("$.resources[?(@.name == 'subDeploy')].location")!.Should().DeepEqual("[resourceGroup().location]");
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

            template!.Should().NotBeNull();
            using (new AssertionScope())
            {
                template!.SelectToken("$.resources[?(@.name == 'subDeploy')].subscriptionId")!.Should().DeepEqual("abcd-efgh");
                template.SelectToken("$.resources[?(@.name == 'subDeploy')].location")!.Should().DeepEqual("[resourceGroup().location]");
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

            template!.Should().NotBeNull();
            using (new AssertionScope())
            {
                template!.SelectToken("$.resources[?(@.name == 'subDeploy')].subscriptionId")!.Should().DeepEqual("abcd-efgh");
                template.SelectToken("$.resources[?(@.name == 'subDeploy')].resourceGroup")!.Should().DeepEqual("bicep-rg");
                template.SelectToken("$.resources[?(@.name == 'subDeploy')].location")!.Should().BeNull();
            }
        }
    }
}