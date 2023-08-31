// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Emit
{
    [TestClass]
    public class ParamsFileWriterTests
    {
        [DataTestMethod]
        [DataRow(@"
using 'main.bicep'

param myParam = 'test value'", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": ""test value""
    }
  }
}", @"
param myParam string
")]
        [DataRow(@"
using 'main.bicep'
param myParam = getSecret('<subscriptionId>', '<resourceGroupName>', '<keyVaultName>', '<secretName>')", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""reference"": {
        ""keyVault"": {
          ""id"": ""/subscriptions/<subscriptionId>/resourceGroups/<resourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyVaultName>""
        },
        ""secretName"": ""<secretName>""
      }
    }
  }
}", @"
param myParam string
")]
        [DataRow(@"
using 'main.bicep'
param myParam = getSecret('<subscriptionId>', '<resourceGroupName>', '<keyVaultName>', '<secretName>', '<secretVersion>')", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""reference"": {
        ""keyVault"": {
          ""id"": ""/subscriptions/<subscriptionId>/resourceGroups/<resourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyVaultName>""
        },
        ""secretName"": ""<secretName>"",
        ""secretVersion"": ""<secretVersion>""
      }
    }
  }
}", @"
param myParam string
")]
        [DataRow("""
          using 'main.bicep'
          param myParam = getSecret('<subscription${toUpper('i')}d>', '<resourceGroup${toUpper('n')}ame>', '<keyVault${toUpper('n')}ame>', '<secret${toUpper('n')}ame>', '<secret${toUpper('v')}ersion>')
          """,
          """
          {
            "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
            "contentVersion": "1.0.0.0",
            "parameters": {
              "myParam": {
                "reference": {
                  "keyVault": {
                    "id": "/subscriptions/<subscriptionId>/resourceGroups/<resourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyVaultName>"
                  },
                  "secretName": "<secretName>",
                  "secretVersion": "<secretVersion>"
                }
              }
            }
          }
          """,
          """
          param myParam string
          """)]
        [DataRow(@"
using 'main.bicep'
param myParam = 1", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": 1
    }
  }
}", @"
param myParam int
")]
        [DataRow(@"
using 'main.bicep'

param myParam = true", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": true
    }
  }
}", @"
param myParam bool
")]
        [DataRow(@"
using 'main.bicep'

param myParam = [
  1
  2
]", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": [
        1,
        2
        ]
    }
  }
}", @"
param myParam array
")]
        [DataRow(@"
using 'main.bicep'

param myParam = {
  property1 : 'value1'
  property2 : 'value2'
}", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": {
        ""property1"" : ""value1"",
        ""property2"" : ""value2""
      }
    }
  }
}", @"
param myParam object
")]
        [DataRow(@"
using 'main.bicep'

param myParam = {
  property1 : null
}", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": {
        ""property1"" : null
      }
    }
  }
}", @"
param myParam object
")]
        [DataRow(@"
using 'main.bicep'

param myParam = {
  // basic case
  'foo': 'val1'
  // special characters in a key value
  '#$%^&*': 'val1'
}", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": {
        ""foo"" : ""val1"",
        ""#$%^&*"" : ""val1""
      }
    }
  }
}", @"
param myParam object
")]
        [DataRow(@"
using 'main.bicep'

// involves all syntax
param myParam = {
  arr: [
    {
      a : 'b'
    }
    {
      c : true
    }
  ]
  name: 'complex object!'
  priority: 3
  val: null
  obj: {
      a: 'b'
      c: [
          'd'
           1
      ]
  }
}", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": {
        ""arr"" : [
          {
            ""a"" : ""b""
          },
          {
            ""c"" : true
          }
        ],
        ""name"" : ""complex object!"",
        ""priority"" : 3,
        ""val"" : null,
        ""obj"" : {
          ""a"" : ""b"",
          ""c"" : [
            ""d"",
            1
          ]
        }
      }
    }
  }
}", @"
param myParam object
")]

        [DataRow(@"
using 'main.bicep'

//multiple parameters
param myStr = 'foo'
param myBool = false
param myInt = 1", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myStr"": {
      ""value"": ""foo""
    },
    ""myBool"": {
      ""value"": false
    },
    ""myInt"": {
      ""value"": 1
    }
  }
}", @"
param myStr string
param myBool bool
param myInt int
")]
        public void Params_file_with_no_errors_should_compile_correctly(string paramsText, string paramsJsonText, string bicepText)
        {
            var result = CompilationHelper.CompileParams(("parameters.bicepparam", paramsText), ("main.bicep", bicepText));

            // Exclude the "No using declaration is present in this parameters file" diagnostic
            result.ExcludingLinterDiagnostics().WithFilteredDiagnostics(x => x.Code != "BCP261").Should().NotHaveAnyDiagnostics();
            result.Parameters.Should().DeepEqual(JToken.Parse(paramsJsonText));
        }
    }
}
