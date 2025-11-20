// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
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
        [DataRow("prop", ".prop", "prop")]
        [DataRow("'complex-prop'", "['complex-prop']", "complex-prop")]
        [DataTestMethod]
        public void NameofFunction_OnObjectProperty_ReturnsPropertyName(string propertyName, string propertyAccess, string expectedResult)
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
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", expectedResult);
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
        public void NameofFunction_OnVariablePropertyWithArrayAccessSyntax_ReturnsPropertyName()
        {
            var result = CompilationHelper.Compile("""
var obj = {
  '1prop-x': 'value'
}
output name string = nameof(obj['1prop-x'])
""");

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", "1prop-x");
            }
        }

        [TestMethod]
        public void NameofFunction_OnArrayAccessSyntaxThatIsNotLiteral_ReturnsError()
        {
            var result = CompilationHelper.Compile("""
var obj = {
 '1prop-x': 'value'
}
var prop = '1prop-x'
output name string = nameof(obj[prop])
""");

            using (new AssertionScope())
            {
                result.Should()
                    .NotGenerateATemplate().And
                    .HaveDiagnostics([("BCP408", DiagnosticLevel.Error, "The \"nameof\" function can only be used with an expression which has a name.")]);
            }
        }

        [TestMethod]
        public void NameofFunction_OnArrayAccessSyntaxThatIsIndex_ReturnsError()
        {
            var result = CompilationHelper.Compile("""
var arr = ['1prop-x', '2prop-y']
output name string = nameof(arr[0])
""");

            using (new AssertionScope())
            {
                result.Should()
                    .NotGenerateATemplate().And
                    .HaveDiagnostics([("BCP408", DiagnosticLevel.Error, "The \"nameof\" function can only be used with an expression which has a name.")]);
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

 output name string = nameof(myStorage.{{resourceProperty}})
 """);

            using (new AssertionScope())
            {
                result.Should().NotHaveAnyCompilationBlockingDiagnostics();
                result.Template.Should().HaveValueAtPath("$.outputs['name'].value", expectedValue);
            }
        }

        [TestMethod]
        public void UsingNameofFunction_ShouldGenerateDependsOnEntries()
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
                result.Template.Should().HaveValueAtPath("$.resources[?(@.type == 'Microsoft.Sql/servers')].dependsOn[0]", "[resourceId('Microsoft.Storage/storageAccounts', 'storage123')]");
            }
        }

        [DataTestMethod]
        [DataRow("'abc'")]
        [DataRow("123")]
        [DataRow("1+2-3")]
        [DataRow("true ? 'ok' : 'notOk'")]
        [DataRow("any('abc')")]
        [DataRow("true")]
        [DataRow("{ x: 'y'}")]
        public void UsingNameofFunction_InInvalidWay_ThrowsError(string expression)
        {
            var result = CompilationHelper.Compile($"output name string = nameof({expression})");

            using (new AssertionScope())
            {
                result.Should()
                    .NotGenerateATemplate().And
                    .HaveDiagnostics([("BCP408", DiagnosticLevel.Error, "The \"nameof\" function can only be used with an expression which has a name.")]);
            }
        }
    }
}
