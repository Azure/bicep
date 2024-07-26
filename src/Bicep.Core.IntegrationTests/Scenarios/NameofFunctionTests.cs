// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    [TestClass]
    public class NameofFunctionTests
    {
        [DataRow("prop",".prop")]
        [DataRow("'complex-prop'","['complex-prop']")]
        [DataTestMethod]
        public void NameofFunction_OnObjectProperty_ReturnsPropertyName(string propertyName, string propertyAccess)
        {
            var result = CompilationHelper.Compile($$"""
var obj = {
  {{propertyName}}: 'value'
}
output name string = nameof(obj{{propertyAccess}})
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", propertyName);
            }
        }

        [TestMethod]
        public void NameofFunction_OnVariable_ReturnsVariableName()
        {
            var result = CompilationHelper.Compile("""
var obj = {
  prop: 'value'
}
output name string = nameof(obj)
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "obj");
            }
        }

        [TestMethod]
        public void NameofFunction_OnParameter_ReturnsParameterName()
        {
            var result = CompilationHelper.Compile("""
                                                   param test string
                                                   output name string = nameof(test)
                                                   """);

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "test");
            }
        }

        [TestMethod]
        public void NameofFunction_OnResource_ReturnsResourceSymbolicName()
        {
            var result = CompilationHelper.Compile("""
resource myStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'storage123'
}

output name string = nameof(myStorage)
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "myStorage");
            }
        }

        [TestMethod]
        public void NameofFunction_OnChildResource_ReturnsResourceSymbolicName()
        {
            var result = CompilationHelper.Compile("""
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'

  resource primaryDb 'databases' = {
    name: 'primary'
  }
}

output name string = nameof(sqlServer::primaryDb)
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "primaryDb");
            }
        }

        [TestMethod]
        public void NameofFunction_OnLoopedResource_ReturnsResourceSymbolicName()
        {
            var result = CompilationHelper.Compile("""

var dbs = [
  'primary'
  'secondary'
  'tertiary'
]
   
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'

  @batchSize(1)
  resource databases 'databases' = [for db in dbs: {
    name: db
  }]
}

output name string = nameof(sqlServer::databases)
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "databases");
            }
        }

        [TestMethod]
        public void NameofFunction_OnLoopedResourceProperty_ReturnsResourcePropertyName()
        {
            var result = CompilationHelper.Compile("""

var dbs = [
  'primary'
  'secondary'
  'tertiary'
]
   
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'

  @batchSize(1)
  resource databases 'databases' = [for db in dbs: {
    name: db
  }]
}

output name string = nameof(sqlServer::databases[100].properties.collation)
""");


            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "collation");
            }
        }

        [DataTestMethod]
        [DataRow("name", "name")]
        [DataRow("type", "type")]
        [DataRow("location", "location")]
        [DataRow("properties", "properties")]
        [DataRow("properties.sku", "sku")]
        public void NameofFunction_OnResourceProperty_ReturnsResourcePropertyName(string resourceProperty, string expectedValue)
        {
            var result = CompilationHelper.Compile($$"""
 resource myStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
   name: 'storage123'
 }

 var name = nameof(myStorage.{{resourceProperty}})

 """);

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.variables['name']", expectedValue);
            }
        }

        [TestMethod]
        public void UsingNameofFunction_ShouldNotGenerateDependsOnEntries()
        {
            var result = CompilationHelper.Compile("""
resource myStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'storage123'
}

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: nameof(myStorage)
}
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Sql/servers')].name", "myStorage", "Nameof function should emit static string during compilation");
                result.Template.Should().NotHaveValueAtPath("$.resources[?(@.type == 'Microsoft.Sql/servers')].dependsOn", "Depends on should not be generated for nameof function usage");
            }
        }
    }
}
