// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    /// <summary>
    /// Tests below will not test actual loading file just the fact that bicep function accepts parameters and produces expected values in ARM template output. 
    /// Loading file is done via IFileResolver - here we use InMemoryFileResolver, therefore no actual file reading can be tested.
    /// Testing FileResolver is covered in a separate place. E2E Testing is done in baseline tests
    /// </summary>
    [TestClass]
    public class LoadFunctionsTests
    {
        private const string TEXT_CONTENT = @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
  Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
  Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
";
        private static readonly string B64_TEXT_CONTENT = Convert.ToBase64String(Encoding.UTF8.GetBytes(TEXT_CONTENT));
        public enum FunctionCase { loadTextContent, loadFileAsBase64 }
        private static string ExpectedResult(FunctionCase function) => function switch
        {
            FunctionCase.loadTextContent => TEXT_CONTENT,
            FunctionCase.loadFileAsBase64 => B64_TEXT_CONTENT,
            _ => throw new NotSupportedException()
        };

        [DataTestMethod]
        [DataRow(FunctionCase.loadTextContent)]
        [DataRow(FunctionCase.loadFileAsBase64)]
        public void LoadFunction_inVariable(FunctionCase function)
        {
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var script = " + function.ToString() + @"('script.sh')

output out string = script
"),
    ("script.sh", TEXT_CONTENT));

            diags.ExcludingLinterDiagnostics().Should().BeEmpty();
            template!.Should().NotBeNull();
            var testToken = template!.SelectToken("$.variables.script");
            using (new AssertionScope())
            {
                testToken.Should().NotBeNull().And.DeepEqual(ExpectedResult(function));
            }
        }

        [DataTestMethod]
        [DataRow(FunctionCase.loadTextContent)]
        [DataRow(FunctionCase.loadFileAsBase64)]
        public void LoadFunction_asPartOfObject_inVariables(FunctionCase function)
        {
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var script = {
  name: 'script'
  content: " + function.ToString() + @"('script.sh')
}

output out object = script
"),
    ("script.sh", TEXT_CONTENT));

            diags.ExcludingLinterDiagnostics().Should().BeEmpty();
            template!.Should().NotBeNull();
            var testToken = template!.SelectToken("$.variables.script.content");
            using (new AssertionScope())
            {
                testToken.Should().NotBeNull().And.DeepEqual(ExpectedResult(function));
            }
        }

        [DataTestMethod]
        [DataRow(FunctionCase.loadTextContent)]
        [DataRow(FunctionCase.loadFileAsBase64)]
        public void LoadFunction_InInterpolation_inVariable(FunctionCase function)
        {
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var message = 'Body: ${" + function.ToString() + @"('message.txt')}'

output out string = message
"),
    ("message.txt", TEXT_CONTENT));

            diags.ExcludingLinterDiagnostics().Should().BeEmpty();
            template!.Should().NotBeNull();
            var testToken = template!.SelectToken("$.variables.message");
            using (new AssertionScope())
            {
                testToken.Should().NotBeNull().And.DeepEqual("[format('Body: {0}', '" + ExpectedResult(function) + "')]");
            }
        }


        [DataTestMethod]
        [DataRow("utf-8")]
        [DataRow("utf-16BE")]
        [DataRow("utf-16")]
        [DataRow("us-ascii")]
        [DataRow("iso-8859-1")]
        public void LoadTextContent_AcceptsAvailableEncoding(string encoding)
        {
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var message = 'Body: ${loadTextContent('message.txt', '" + encoding + @"')}'

output out string = message
"),
    ("message.txt", TEXT_CONTENT));

            diags.ExcludingLinterDiagnostics().Should().BeEmpty();
            template!.Should().NotBeNull();
            var testToken = template!.SelectToken("$.variables.message");
            using (new AssertionScope())
            {
                testToken.Should().NotBeNull().And.DeepEqual("[format('Body: {0}', '" + TEXT_CONTENT + "')]");
            }
        }

        [DataTestMethod]
        [DataRow("utf")]
        [DataRow("utf-32be")]
        [DataRow("utf-32le")]
        [DataRow("utf-7")]
        [DataRow("iso-8859-2")]
        [DataRow("en-us")]
        [DataRow("")]
        public void LoadTextContent_DisallowsUnknownEncoding(string encoding)
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var message = 'Body: ${loadTextContent('message.txt', '" + encoding + @"')}'
"),
    ("message.txt", TEXT_CONTENT));

            template!.Should().BeNull();
            diags.ExcludingLinterDiagnostics().Should().ContainSingleDiagnostic("BCP070", Diagnostics.DiagnosticLevel.Error, $"Argument of type \"'{encoding}'\" is not assignable to parameter of type \"{LanguageConstants.LoadTextContentEncodings}\".");
        }

        [DataTestMethod]
        [DataRow("utf")]
        [DataRow("iso-8859-2")]
        [DataRow("en-us")]
        [DataRow("")]
        public void LoadTextContent_DisallowsUnknownEncoding_passedFromVariable(string encoding)
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var encoding = '" + encoding + @"'
var message = 'Body: ${loadTextContent('message.txt', encoding)}'
"),
    ("message.txt", TEXT_CONTENT));

            template!.Should().BeNull();
            diags.ExcludingLinterDiagnostics().Should().ContainSingleDiagnostic("BCP070", Diagnostics.DiagnosticLevel.Error, $"Argument of type \"'{encoding}'\" is not assignable to parameter of type \"{LanguageConstants.LoadTextContentEncodings}\".");
        }

        [DataTestMethod]
        [DataRow(FunctionCase.loadTextContent, "var fileName = 'message.txt'", "fileName", DisplayName = "loadTextContent: variable")]
        [DataRow(FunctionCase.loadFileAsBase64, "var fileName = 'message.txt'", "fileName", DisplayName = "loadFileAsBase64: variable")]
        [DataRow(FunctionCase.loadTextContent, @"var fileNames = [
'message.txt'
]", "fileNames[0]", DisplayName = "loadTextContent: array value")]
        [DataRow(FunctionCase.loadFileAsBase64, @"var fileNames = [
'message.txt'
]", "fileNames[0]", DisplayName = "loadFileAsBase64: array value")]
        [DataRow(FunctionCase.loadTextContent, @"var files = [
 {
  name: 'message.txt'
 }
]", "files[0].name", DisplayName = "loadTextContent: object property from an array")]
        [DataRow(FunctionCase.loadFileAsBase64, @"var files = [
 {
  name: 'message.txt'
 }
]", "files[0].name", DisplayName = "loadFileAsBase64: object property from an array")]
        [DataRow(FunctionCase.loadTextContent, @"var files = [
 {
  name: 'message.txt'
  encoding: 'utf-8'
 }
]", "files[0].name", "files[0].encoding", DisplayName = "loadTextContent: object property and encoding from an array")]
        [DataRow(FunctionCase.loadTextContent, @"var encoding = 'us-ascii'", "'message.txt'", "encoding", DisplayName = "loadTextContent: encoding as variable")]
        public void LoadFunction_RequiresCompileTimeConstantArguments_Valid(FunctionCase function, string declaration, string filePath, string? encoding = null)
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
" + declaration + @"
var message = 'Body: ${" + function.ToString() + @"(" + filePath + ((encoding is null) ? string.Empty : (", " + encoding)) + @")}'
output out string = message
"),
    ("message.txt", TEXT_CONTENT));

            diags.ExcludingLinterDiagnostics().Should().BeEmpty();
            template!.Should().NotBeNull();
            var testToken = template!.SelectToken("$.variables.message");
            using (new AssertionScope())
            {
                testToken.Should().NotBeNull().And.DeepEqual("[format('Body: {0}', '" + ExpectedResult(function) + "')]");
            }
        }

        [DataTestMethod]
        [DataRow(FunctionCase.loadTextContent, "var fileName = 'message.txt'", "'${fileName}'", DisplayName = "loadTextContent: variable in interpolation")]
        [DataRow(FunctionCase.loadFileAsBase64, "var fileName = 'message.txt'", "'${fileName}'", DisplayName = "loadFileAsBase64: variable in interpolation")]
        [DataRow(FunctionCase.loadTextContent, "", "'${'fileName'}'", DisplayName = "loadTextContent: string literal in interpolation")]
        [DataRow(FunctionCase.loadFileAsBase64, "", "'${'fileName'}'", DisplayName = "loadFileAsBase64: string literal in interpolation")]
        [DataRow(FunctionCase.loadTextContent, "param fileName string = 'message.txt'", "fileName", DisplayName = "loadTextContent: parameter")]
        [DataRow(FunctionCase.loadFileAsBase64, "param fileName string = 'message.txt'", "fileName", DisplayName = "loadFileAsBase64: parameter")]
        [DataRow(FunctionCase.loadTextContent, @"param fileName string = 'message.txt'
var _fileName = fileName", "_fileName", DisplayName = "loadTextContent: variable from parameter")]
        [DataRow(FunctionCase.loadFileAsBase64, @"param fileName string = 'message.txt'
var _fileName = fileName", "_fileName", DisplayName = "loadFileAsBase64: variable from parameter")]
        [DataRow(FunctionCase.loadTextContent, @"param fileName string = 'message.txt'
var fileNames = [
fileName
]", "fileNames[0]", DisplayName = "loadTextContent: param as array value")]
        [DataRow(FunctionCase.loadFileAsBase64, @"param fileName string = 'message.txt'
var fileNames = [
fileName
]", "fileNames[0]", DisplayName = "loadFileAsBase64: param as array value")]
        [DataRow(FunctionCase.loadTextContent, @"param fileName string = 'message.txt'
var files = [
 {
  name: fileName
 }
]", "files[0].name", DisplayName = "loadTextContent: param as object property in array")]
        [DataRow(FunctionCase.loadFileAsBase64, @"param fileName string = 'message.txt'
var files = [
 {
  name: fileName
 }
]", "files[0].name", DisplayName = "loadFileAsBase64: param as object property in array")]
        [DataRow(FunctionCase.loadTextContent, @"param encoding string = 'us-ascii'
var files = [
 {
  name: 'message.txt'
  encoding: encoding
 }
]", "files[0].name", "files[0].encoding", DisplayName = "loadTextContent: encoding param as object property in array")]
        public void LoadFunction_RequiresCompileTimeConstantArguments_Invalid(FunctionCase function, string declaration, string filePath, string? encoding = null)
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
" + declaration + @"
var message = 'Body: ${" + function.ToString() + @"(" + filePath + ((encoding is null) ? string.Empty : (", " + encoding)) + @")}'
"),
    ("message.txt", TEXT_CONTENT));

            template!.Should().BeNull();
            diags.ExcludingLinterDiagnostics().Should().ContainSingleDiagnostic("BCP032", Diagnostics.DiagnosticLevel.Error, "The value must be a compile-time constant.");
        }

        private const string LOGIC_APP = @"
{
  ""$schema"": ""https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {},
  ""triggers"": {
      ""manual"": {
          ""type"": ""Request"",
          ""kind"": ""Http"",
          ""inputs"": {
              ""schema"": {}
          }
      }
  },
  ""actions"": {
      ""Delay"": {
          ""runAfter"": {},
          ""type"": ""Wait"",
          ""inputs"": {
              ""interval"": {
                  ""count"": 10,
                  ""unit"": ""Second""
              }
          }
      }
  },
  ""outputs"": {}
}";
        [TestMethod]
        public void LoadTextContent_RequiresCompileTimeConstantArguments_Loop_Invalid_Interpolation()
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var apps = [
  'logicApp1'
  'logicApp2'
]
resource logicApps 'Microsoft.Logic/workflows@2019-05-01' = [ for app in apps :{
  name: app
  location: resourceGroup().location
  properties: {
    definition: json(loadTextContent('${app}.json'))
  }
}]"),
    ("logicApp1.json", LOGIC_APP), ("logicApp2.json", LOGIC_APP));

            template!.Should().BeNull();
            diags.ExcludingLinterDiagnostics().Should().ContainSingleDiagnostic("BCP032", Diagnostics.DiagnosticLevel.Error, "The value must be a compile-time constant.");
        }

        [TestMethod]
        public void LoadTextContent_RequiresCompileTimeConstantArguments_Loop_Invalid_indexAccess()
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var apps = [
  {
    name: 'logicApp1'
    file: 'logicApp1.json'
  }
  {
    name: 'logicApp2'
    file: 'logicApp2.json'
  }
]
resource logicApps 'Microsoft.Logic/workflows@2019-05-01' = [ for (app, i) in apps :{
  name: app.name
  location: resourceGroup().location
  properties: {
    definition: json(loadTextContent(app.file))
  }
}]"),
    ("logicApp1.json", LOGIC_APP), ("logicApp2.json", LOGIC_APP));

            template!.Should().BeNull();
            diags.ExcludingLinterDiagnostics().Should().ContainSingleDiagnostic("BCP032", Diagnostics.DiagnosticLevel.Error, "The value must be a compile-time constant.");
        }

        [TestMethod]
        public void LoadTextContent_RequiresCompileTimeConstantArguments_Loop_Valid()
        {
            //notice - here we will not test actual loading file with given encoding - just the fact that bicep function accepts all .NET available encodings
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var apps = [
  {
    name: 'logicApp1'
    file: loadTextContent('logicApp1.json')
  }
  {
    name: 'logicApp2'
    file: loadTextContent('logicApp2.json')
  }
]
resource logicApps 'Microsoft.Logic/workflows@2019-05-01' = [ for (app, i) in apps :{
  name: app.name
  location: resourceGroup().location
  properties: {
    definition: json(app.file)
  }
}]"),
    ("logicApp1.json", LOGIC_APP), ("logicApp2.json", LOGIC_APP));

            template!.Should().NotBeNull();
            diags.ExcludingLinterDiagnostics().Should().BeEmpty();
        }

        public static IEnumerable<object[]> LoadFunction_InvalidPath_Data
        {
            get
            {
                foreach (var function in new[] { FunctionCase.loadTextContent, FunctionCase.loadFileAsBase64 })
                {
                    yield return new object[] { function, "" };
                    yield return new object[] { function, " " };
                    yield return new object[] { function, "\t" };
                    yield return new object[] { function, "/script.sh" };
                    yield return new object[] { function, ".\\script.sh" };
                    yield return new object[] { function, "https://storage.azure.com/script.sh" };
                    yield return new object[] { function, "github://azure/bicep/@main/samples/script.sh" };
                    yield return new object[] { function, "ssh://root@host:/data/script.sh" };
                    yield return new object[] { function, "../" };
                    yield return new object[] { function, "dir\\..\\script.sh" };
                }
            }
        }
        [DataTestMethod]
        [DynamicData(nameof(LoadFunction_InvalidPath_Data), DynamicDataSourceType.Property)]
        public void LoadFunction_InvalidPath(FunctionCase function, string invalidPath)
        {
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var script = " + function.ToString() + @"('" + invalidPath + @"')

output out string = script
"),
    ("script.sh", TEXT_CONTENT));

            template!.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(FunctionCase.loadTextContent)]
        [DataRow(FunctionCase.loadFileAsBase64)]
        public void LoadFunction_FileDoesNotExist(FunctionCase function)
        {
            var (template, diags, _) = CompilationHelper.Compile(
    ("main.bicep", @"
var script = " + function.ToString() + @"('script.cmd')

output out string = script
"),
    ("script.sh", TEXT_CONTENT));

            template!.Should().BeNull();
            diags.Should().ContainDiagnostic("BCP091", Diagnostics.DiagnosticLevel.Error, "An error occurred reading file. Could not find file \"/path/to/script.cmd\"");
        }
    }
}
