// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    [TestClass]
    public class ScopeTests
    {
        [TestMethod]
        public void UsingIndexValueInModuleScope_ShouldInlineTheResourceGroupName()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
targetScope = 'subscription'

// parameters
param tags object
param location string

// variables
var resourceGroups = [
  'test-uat-rg'
  'test-uat-blue-rg'
  'test-uat-green-rg'
]

// base resource group deployment
resource rgs 'Microsoft.Resources/resourceGroups@2021-04-01' = [for rgname in resourceGroups: {
  name: rgname
  location: location
  tags: tags
}]

module storage 'storage.bicep' = {
  name: 'str'
  scope: rgs[0]
  params: {
    environment: 'uat'
  }
}
"), ("storage.bicep", @"
param environment string
output env string = environment
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'str')].resourceGroup", "[variables('resourceGroups')[0]]");
            }
        }

        [TestMethod]
        public void UsingIndexValueInModuleScope_ShouldInlineTheManagementGroupName_1()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
targetScope = 'tenant'

// variables
var managementGroups = [
  'mg1'
  'mg2'
]

resource mgs 'Microsoft.Management/managementGroups@2020-02-01' = [for mgname in managementGroups: {
  name: mgname  
}]


module mod 'mod.bicep' = {
  name: 'mod'
  scope: mgs[0]
  params: {
    environment: 'uat'
  }
}
"), ("mod.bicep", @"
targetScope = 'managementGroup'
param environment string
output env string = environment
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'mod')].scope", "[format('Microsoft.Management/managementGroups/{0}', variables('managementGroups')[0])]");
            }
        }

        [TestMethod]
        public void UsingIndexValueInModuleScope_ShouldInlineTheManagementGroupName_2()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
targetScope = 'tenant'

// variables
var managementGroups = [
  'mg1'
  'mg2'
]

resource mgs 'Microsoft.Management/managementGroups@2020-02-01' existing = [for mgname in managementGroups: {
  name: mgname  
}]


module mod 'mod.bicep' = {
  name: 'mod'
  scope: mgs[0]
  params: {
    environment: 'uat'
  }
}
"), ("mod.bicep", @"
targetScope = 'managementGroup'
param environment string
output env string = environment
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'mod')].scope", "[format('Microsoft.Management/managementGroups/{0}', variables('managementGroups')[0])]");
            }
        }

        [TestMethod]
        public void UsingIndexValueInModuleScope_ShouldInlineTheResourceGroupAndSubscription()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
targetScope = 'subscription'

// variables
var resourceGroups = [
  {
    name: 'test-uat-rg'
    sub: '00000000-0000-0000-0000-000000000000'
  } 
]

// base resource group deployment
resource rgs 'Microsoft.Resources/resourceGroups@2021-04-01' existing = [for rg in resourceGroups: {
  name: rg.name
  scope: subscription(rg.sub)  
}]

module storage 'storage.bicep' = {
  name: 'str'
  scope: rgs[0]
  params: {
    environment: 'uat'
  }
}
"), ("storage.bicep", @"
param environment string
output env string = environment
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'str')].resourceGroup", "[variables('resourceGroups')[0].name]");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'str')].subscriptionId", "[variables('resourceGroups')[0].sub]");
            }
        }
        [TestMethod]
        public void ScopeToNestedChildResource_ShouldGenerateScopeIdCorrectly()
        {
            var result = CompilationHelper.Compile(("main.bicep", @"
param postgreSqlServerId string
var PSQL_DATABASES = [
{
 database: {
  name: 'db1'
 }
}
{
 database: {
  name: 'db2'
 }
}
]

resource postgreSQL 'Microsoft.DBForPostgreSQL/servers@2017-12-01' existing = {
  name: last(split(postgreSqlServerId, '/'))
  resource database 'databases' = [for (item, index) in PSQL_DATABASES: {
    name: item.database.name
    properties: {
      charset: contains(item.database, 'charset') ? item.database.charset : 'utf8'
      collation: contains(item.database, 'collation') ? item.database.charset : 'English_United States.1252'
    }
  }]
}

resource dbLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (item, index) in PSQL_DATABASES: {
  name: 'dbLock'
  scope: postgreSQL::database[index]
  properties: {
    level: 'CanNotDelete'
    notes: 'Database cannot be deleted'
  }
}]
"));

            result.Should().NotHaveAnyDiagnostics();
            using (new AssertionScope())
            {
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'dbLock')].scope", "[format('Microsoft.DBforPostgreSQL/servers/{0}/databases/{1}', last(split(parameters('postgreSqlServerId'), '/')), variables('PSQL_DATABASES')[copyIndex()].database.name)]");
                result.Template.Should().HaveValueAtPath("$.resources[?(@.name == 'dbLock')].dependsOn", new JArray
                {
                    "[resourceId('Microsoft.DBforPostgreSQL/servers/databases', last(split(parameters('postgreSqlServerId'), '/')), variables('PSQL_DATABASES')[copyIndex()].database.name)]"
                });
            }
        }
    }
}
