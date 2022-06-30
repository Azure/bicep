// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Emit;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
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
        public void params_file_with_no_errors_should_compile_correctly(string paramsText, string jsonText)
        {
          var syntax = ParamsParserHelper.ParamsParse(paramsText);

          var paramsWriter = new ParamsFileWriter(syntax);

          var jsonOuput = paramsWriter.GenerateTemplate();

          var expectedJsonOuput = JToken.Parse(jsonText);

          jsonOuput.Should().DeepEqual(expectedJsonOuput);
        }
    }
}

