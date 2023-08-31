// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepBuildCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static BicepBuildCommandHandler CreateHandler(ICompilationManager compilationManager)
        {
            var helper = ServiceBuilder.Create(services => services
                .AddSingleton(StrictMock.Of<ISerializer>().Object)
                .AddSingleton(compilationManager)
                .AddSingleton<BicepBuildCommandHandler>());

            return helper.Construct<BicepBuildCommandHandler>();
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task Handle_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            ICompilationManager bicepCompilationManager = StrictMock.Of<ICompilationManager>().Object;
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);

            Func<Task> sut = () => bicepBuildCommandHandler.Handle(path, CancellationToken.None);

            await sut.Should().ThrowAsync<ArgumentException>().WithMessage("Invalid input file path");
        }

        [TestMethod]
        public async Task Handle_WithNullContext_ShouldCreateCompilation()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            // Do not upsert compilation. This will cause CompilationContext to be null
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: false);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Bicep build succeeded. Created ARM template file: input.json");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndOnlyWarningsAndInfoInInputFile_ReturnsBuildSucceededMessage()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            FileHelper.SaveResultFile(TestContext, "encoding.txt", @"Π π Φ φ", testOutputPath, Encoding.UTF8);

            string bicepFileContents = @"var textLoadEncoding = loadTextContent('encoding.txt', 'us-ascii')
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            Uri bicepFileUri = new Uri(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Bicep build succeeded. Created ARM template file: input.json");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndErrorsAndWarningsInInputFile_ReturnsBuildFailedMessage()
        {
            DocumentUri documentUri = DocumentUri.From("input.bicep");
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, @"targetScope

 #completionTest(12) -> empty
targetScope

 #completionTest(13,14) -> targetScopes
targetScope =


targetScope = 'asdfds'

targetScope = { }

targetScope = true
param accountName string = 'testAccount'
", true);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string expected = await bicepBuildCommandHandler.Handle(documentUri.Path, CancellationToken.None);

            expected.Should().BeEquivalentToIgnoringNewlines("""
                Bicep build failed. Please fix below errors:
                /input.bicep(1,1) : Error BCP112: The "targetScope" cannot be declared multiple times in one file.
                /input.bicep(1,12) : Error BCP018: Expected the "=" character at this location.
                /input.bicep(1,12) : Error BCP009: Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.
                /input.bicep(3,2) : Error BCP001: The following token is not recognized: "#".
                /input.bicep(3,2) : Error BCP007: This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration.
                /input.bicep(4,1) : Error BCP112: The "targetScope" cannot be declared multiple times in one file.
                /input.bicep(4,12) : Error BCP018: Expected the "=" character at this location.
                /input.bicep(4,12) : Error BCP009: Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.
                /input.bicep(6,2) : Error BCP001: The following token is not recognized: "#".
                /input.bicep(6,2) : Error BCP007: This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration.
                /input.bicep(7,1) : Error BCP112: The "targetScope" cannot be declared multiple times in one file.
                /input.bicep(7,14) : Error BCP009: Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.
                /input.bicep(10,1) : Error BCP112: The "targetScope" cannot be declared multiple times in one file.
                /input.bicep(10,15) : Error BCP033: Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "'asdfds'".
                /input.bicep(12,1) : Error BCP112: The "targetScope" cannot be declared multiple times in one file.
                /input.bicep(12,15) : Error BCP033: Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "object".
                /input.bicep(14,1) : Error BCP112: The "targetScope" cannot be declared multiple times in one file.
                /input.bicep(14,15) : Error BCP033: Expected a value of type "'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'" but the provided value is of type "true".
                /input.bicep(15,7) : Warning no-unused-params: Parameter "accountName" is declared but never used. [https://aka.ms/bicep/linter/no-unused-params]

                """);
        }

        [TestMethod]
        public async Task Handle_WhenCompiledFileAlreadyExists_ReturnsBuildFailedMessage()
        {
            string outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, outputPath);
            FileHelper.SaveResultFile(TestContext, "input.json", string.Empty, outputPath);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, true);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string actual = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            actual.Should().Be(@"Bicep build failed. The output file ""input.json"" already exists and was not generated by Bicep. If overwriting the file is intended, delete it manually and retry the build command.");
        }

        [TestMethod]
        public async Task Handle_WhenCompiledFileAlreadyExistsAndIsMalformed_ReturnsBuildFailedMessage()
        {
            string outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, outputPath);
            FileHelper.SaveResultFile(TestContext, "input.json", "invalid json", outputPath);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, true);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string actual = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            actual.Should().Be(@"Bicep build failed. The output file ""input.json"" already exists and was not generated by Bicep. If overwriting the file is intended, delete it manually and retry the build command.");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndNoErrorsInInputFile_ReturnsBuildSucceededMessage()
        {
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}

");
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, true);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string actual = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            actual.Should().Be(@"Bicep build succeeded. Created ARM template file: input.json");
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void TemplateContainsBicepGeneratorMetadata_WithInvalidInput_ReturnsFalse(string template)
        {
            ICompilationManager bicepCompilationManager = StrictMock.Of<ICompilationManager>().Object;
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);

            bool actual = bicepBuildCommandHandler.TemplateContainsBicepGeneratorMetadata(template);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TemplateContainsBicepGeneratorMetadata_WithBicepGeneratorMetadataInInput_ReturnsTrue()
        {
            ICompilationManager bicepCompilationManager = StrictMock.Of<ICompilationManager>().Object;
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.4.491.37184"",
      ""templateHash"": ""583008187481737995""
    }
  },
  ""functions"": [],
  ""resources"": []
}";

            bool actual = bicepBuildCommandHandler.TemplateContainsBicepGeneratorMetadata(template);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TemplateContainsBicepGeneratorMetadata_WithoutBicepGeneratorMetadataInInput_ReturnsFalse()
        {
            ICompilationManager bicepCompilationManager = StrictMock.Of<ICompilationManager>().Object;
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);
            string template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""test"",
      ""version"": ""0.4.491.37184"",
      ""templateHash"": ""583008187481737995""
    }
  },
  ""functions"": [],
  ""resources"": []
}";

            bool actual = bicepBuildCommandHandler.TemplateContainsBicepGeneratorMetadata(template);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public async Task Handle_ShouldPickUp_LoadTextContent_Updates()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            string sqlFileContents = @"CREATE TABLE regions1 (
    region_id INT IDENTITY(1,1) PRIMARY KEY
);";
            FileHelper.SaveResultFile(TestContext, "test.sql", sqlFileContents, testOutputPath);

            string bicepFileContents = @"var textFromFile = loadTextContent('test.sql')";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);

            Uri bicepFileUri = new Uri(bicepFilePath);
            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepBuildCommandHandler = CreateHandler(bicepCompilationManager);

            string buildOutputMessage = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            string buildOutputFilePath = Path.Combine(testOutputPath, "input.json");

            VerifyBuildOutputMessageAndContents(buildOutputMessage, File.ReadAllText(buildOutputFilePath), @"""variables"": {
    ""textFromFile"": ""CREATE TABLE regions1 (\n    region_id INT IDENTITY(1,1) PRIMARY KEY\n);""
  }");

            // Update test.sql and execute build command
            sqlFileContents = @"CREATE TABLE regions2 (
    region_id INT IDENTITY(1,1) PRIMARY KEY
);";
            FileHelper.SaveResultFile(TestContext, "test.sql", sqlFileContents, testOutputPath);

            buildOutputMessage = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            VerifyBuildOutputMessageAndContents(buildOutputMessage, File.ReadAllText(buildOutputFilePath), @"""variables"": {
    ""textFromFile"": ""CREATE TABLE regions2 (\n    region_id INT IDENTITY(1,1) PRIMARY KEY\n);""
  }");
        }

        private void VerifyBuildOutputMessageAndContents(string actualBuildOutputMessage, string buildOutputContents, string expectedText)
        {
            actualBuildOutputMessage.Should().Be(@"Bicep build succeeded. Created ARM template file: input.json");
            buildOutputContents.Should().ContainIgnoringNewlines(expectedText);
        }
    }
}
