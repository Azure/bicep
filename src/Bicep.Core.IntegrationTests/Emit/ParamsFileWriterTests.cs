// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Emit;
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
param myParam = 'test value'", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": ""test value""
    }
  }
}")]
        [DataRow(@"
param myParam = 1", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": 1
    }
  }
}")]
        [DataRow(@"
param myParam = true", @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""myParam"": {
      ""value"": true
    }
  }
}")]
        [DataRow(@"
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
}")]
        [DataRow(@"
param myParam = {
  property1 : 'value1',
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
}")]
        [DataRow(@"
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
}")]
        [DataRow(@"
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
}")]
        [DataRow(@"
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
}")]

        [DataRow(@"
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
}")]
        public void params_file_with_no_errors_should_compile_correctly(string paramsText, string jsonText)
        {
          var syntax = ParamsParserHelper.ParamsParse(paramsText);

          var paramsWriter = new ParamsFileTemplateWriter(syntax);

          var jsonOuput = paramsWriter.GenerateTemplate();

          var expectedJsonOuput = JToken.Parse(jsonText);

          jsonOuput.Should().DeepEqual(expectedJsonOuput);
        }
        
        [DataTestMethod]
        public void params_file_with_not_implemented_syntax_should_throw_expction()
        {
          var syntax = ParamsParserHelper.ParamsParse("param foo = 1 + 2");

          var paramsWriter = new ParamsFileTemplateWriter(syntax);

          Action act = () => paramsWriter.GenerateTemplate();
          act.Should().Throw<NotImplementedException>().WithMessage("Cannot emit unexpected expression of type BinaryOperationSyntax");
        }
    }
}

