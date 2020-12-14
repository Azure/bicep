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

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
        [TestMethod]
        public void Test_Issue746()
        {
            var bicepContents = @"
var l = l
param l
";

            CompilationHelper.AssertFailureWithDiagnostics(
                bicepContents,
                new [] {
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP014", DiagnosticLevel.Error, "Expected a parameter type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
                });
        }

        [TestMethod]
        public void Test_Issue801()
        {
            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///path/to/main.bicep")] = @"
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
",
                [new Uri("file:///path/to/vnet.bicep")] = @"
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
",
            };

            var jsonOutput = CompilationHelper.AssertSuccessWithTemplateOutput(files, new Uri("file:///path/to/main.bicep"));

            // ensure we're generating the correct expression with 'subscriptionResourceId', and using the correct name for the module
            jsonOutput.Should().Contain("[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetId.value]");
            jsonOutput.Should().Contain("[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'vnet-rg'), 'Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetstate.value]");
        }

        [TestMethod]
        public void Test_Issue982()
        {
            var bicepContents = @"
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
";

            var jsonOutput = CompilationHelper.AssertSuccessWithTemplateOutput(bicepContents);
            jsonOutput.Should().Contain("[list(format('{0}/config/appsettings', resourceId('Microsoft.Web/sites', parameters('functionApp').name)), '2020-06-01')]");
        }

        [TestMethod]
        public void Test_Issue1093()
        {
            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///path/to/main.bicep")] = @"
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
",
                [new Uri("file:///path/to/resourceGroup.bicep")] = @"
param rgName string
param location string = 'westeurope'

targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: rgName
  location: location
}",
            };

            var jsonOutput = CompilationHelper.AssertSuccessWithTemplateOutput(files, new Uri("file:///path/to/main.bicep"));

            var template = JToken.Parse(jsonOutput);
            template.SelectToken("$.resources[?(@.name == 'rg30')].location")!.Should().DeepEqual("[deployment().location]");
            template.SelectToken("$.resources[?(@.name == 'rg31')].location")!.Should().DeepEqual("[deployment().location]");
        }
    }
}