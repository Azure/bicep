// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepBuildCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly FileResolver FileResolver = new();
        private static readonly MockRepository Repository = new(MockBehavior.Strict);
        private static readonly ISerializer Serializer = Repository.Create<ISerializer>().Object;
        private static readonly IConfigurationManager configurationManager = new ConfigurationManager(new IOFileSystem());
        private readonly ModuleDispatcher ModuleDispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void Handle_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);

            Action sut = () => bicepBuildCommandHandler.Handle(path, CancellationToken.None);

            sut.Should().Throw<ArgumentException>().WithMessage("Invalid input file");
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
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Build succeeded. Created file input.json");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndOnlyWarningsAndInfoInInputFile_ReturnsBuildSucceededMessage()
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

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
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Build succeeded. Created file input.json");
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
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(documentUri.Path, CancellationToken.None);

            expected.Should().BeEquivalentToIgnoringNewlines(@"Build failed. Please fix below errors:
/input.bicep(1,1) : Error BCP112: The ""targetScope"" cannot be declared multiple times in one file.
/input.bicep(1,12) : Error BCP018: Expected the ""="" character at this location.
/input.bicep(1,12) : Error BCP009: Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.
/input.bicep(3,2) : Error BCP007: This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.
/input.bicep(3,2) : Error BCP001: The following token is not recognized: ""#"".
/input.bicep(4,1) : Error BCP112: The ""targetScope"" cannot be declared multiple times in one file.
/input.bicep(4,13) : Error BCP018: Expected the ""="" character at this location.
/input.bicep(4,13) : Error BCP009: Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.
/input.bicep(6,2) : Error BCP007: This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration.
/input.bicep(6,2) : Error BCP001: The following token is not recognized: ""#"".
/input.bicep(7,1) : Error BCP112: The ""targetScope"" cannot be declared multiple times in one file.
/input.bicep(7,15) : Error BCP009: Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location.
/input.bicep(10,1) : Error BCP112: The ""targetScope"" cannot be declared multiple times in one file.
/input.bicep(10,15) : Error BCP033: Expected a value of type ""'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'"" but the provided value is of type ""'asdfds'"".
/input.bicep(12,1) : Error BCP112: The ""targetScope"" cannot be declared multiple times in one file.
/input.bicep(12,15) : Error BCP033: Expected a value of type ""'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'"" but the provided value is of type ""object"".
/input.bicep(14,1) : Error BCP112: The ""targetScope"" cannot be declared multiple times in one file.
/input.bicep(14,15) : Error BCP033: Expected a value of type ""'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'"" but the provided value is of type ""bool"".
/input.bicep(15,7) : Warning no-unused-params: Parameter ""accountName"" is declared but never used. [https://aka.ms/bicep/linter/no-unused-params]
");
        }

        [TestMethod]
        public async Task Handle_WhenCompiledFileAlreadyExists_ReturnsBuildFailedMessage()
        {
            string outputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, outputPath);
            FileHelper.SaveResultFile(TestContext, "input.json", string.Empty, outputPath);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, true);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Build failed. The file ""input.json"" already exists and was not generated by Bicep. If overwriting the file is intended, delete it manually and retry the Build command.");
        }

        [TestMethod]
        public async Task Handle_WhenCompiledFileAlreadyExistsAndIsMalformed_ReturnsBuildFailedMessage()
        {
            string outputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty, outputPath);
            FileHelper.SaveResultFile(TestContext, "input.json", "invalid json", outputPath);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, true);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Build failed. The file ""input.json"" already exists and was not generated by Bicep. If overwriting the file is intended, delete it manually and retry the Build command.");
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
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Build succeeded. Created file input.json");
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void TemplateContainsBicepGeneratorMetadata_WithInvalidInput_ReturnsFalse(string template)
        {
            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);

            bool actual = bicepBuildCommandHandler.TemplateContainsBicepGeneratorMetadata(template);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TemplateContainsBicepGeneratorMetadata_WithBicepGeneratorMetadataInInput_ReturnsTrue()
        {
            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
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
            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
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
    }
}
