// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using static Bicep.LanguageServer.Telemetry.BicepTelemetryEvent;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDecompileCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        #region Simple JSON

        private const string SimpleJson = @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""location"": {
      ""type"": ""string"",
      ""defaultValue"": ""[resourceGroup().location]""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-02-01"",
      ""name"": ""name"",
      ""location"": ""[parameters('location')]"",
      ""kind"": ""StorageV2"",
      ""sku"": {
        ""name"": ""Premium_LRS""
      }
    }
  ]
}";
        private const string SimpleExpectedBicep = @"param location string = resourceGroup().location

resource name 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}";

        #endregion

        #region Complex JSON (multi-file output)

        private const string ComplexJson = @"
{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""resources"": [
    {
      ""name"": ""nestedDeploymentInner"",
      ""type"": ""Microsoft.Resources/deployments"",
      ""apiVersion"": ""2021-04-01"",
      ""properties"": {
        ""expressionEvaluationOptions"": {
          ""scope"": ""inner""
        },
        ""mode"": ""Incremental"",
        ""parameters"": {},
        ""template"": {
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""parameters"": {},
          ""variables"": {},
          ""resources"": [
            {
              ""name"": ""storageaccount1"",
              ""type"": ""Microsoft.Storage/storageAccounts"",
              ""apiVersion"": ""2021-04-01"",
              ""tags"": {
                ""displayName"": ""storageaccount1""
              },
              ""location"": ""[resourceGroup().location]"",
              ""kind"": ""StorageV2"",
              ""sku"": {
                ""name"": ""Premium_LRS"",
                ""tier"": ""Premium""
              }
            }
          ],
          ""outputs"": {}
        }
      }
    },
    {
      ""name"": ""nestedDeploymentOuter"",
      ""type"": ""Microsoft.Resources/deployments"",
      ""apiVersion"": ""2021-04-01"",
      ""properties"": {
        ""mode"": ""Incremental"",
        ""template"": {
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""variables"": {},
          ""resources"": [
            {
              ""name"": ""storageaccount2"",
              ""type"": ""Microsoft.Storage/storageAccounts"",
              ""apiVersion"": ""2021-04-01"",
              ""tags"": {
                ""displayName"": ""storageaccount2""
              },
              ""location"": ""[resourceGroup().location]"",
              ""kind"": ""StorageV2"",
              ""sku"": {
                ""name"": ""Premium_LRS"",
                ""tier"": ""Premium""
              }
            }
          ],
          ""outputs"": {}
        }
      }
    },
    {
      ""name"": ""storageaccount"",
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-04-01"",
      ""tags"": {
        ""displayName"": ""storageaccount""
      },
      ""location"": ""[resourceGroup().location]"",
      ""kind"": ""StorageV2"",
      ""sku"": {
        ""name"": ""Premium_LRS"",
        ""tier"": ""Premium""
      }
    },
    {
      ""name"": ""nestedDeploymentInner2"",
      ""type"": ""Microsoft.Resources/deployments"",
      ""apiVersion"": ""2021-04-01"",
      ""properties"": {
        ""expressionEvaluationOptions"": {
          ""scope"": ""inner""
        },
        ""mode"": ""Incremental"",
        ""parameters"": {},
        ""template"": {
          ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
          ""contentVersion"": ""1.0.0.0"",
          ""parameters"": {},
          ""variables"": {},
          ""resources"": [],
          ""outputs"": {}
        }
      }
    }
  ]
}";

        private const string ComplexExpectedBicep_MainOutput = @"module nestedDeploymentInner './nested_nestedDeploymentInner.bicep' = {
  name: 'nestedDeploymentInner'
  params: {
  }
}

module nestedDeploymentOuter './nested_nestedDeploymentOuter.bicep' = {
  name: 'nestedDeploymentOuter'
  params: {
  }
}

resource storageaccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: 'storageaccount'
  tags: {
    displayName: 'storageaccount'
  }
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
    tier: 'Premium'
  }
}

module nestedDeploymentInner2 './nested_nestedDeploymentInner2.bicep' = {
  name: 'nestedDeploymentInner2'
  params: {
  }
}";

        private const string ComplexExpectedBicep_Filename2 = "nested_nestedDeploymentInner.bicep";
        private const string ComplexExpectedBicep_Output2Regex = @"resource storageaccount1 'Microsoft.Storage/storageAccounts@2021-04-01'";

        private const string ComplexExpectedBicep_Filename3 = "nested_nestedDeploymentInner2.bicep";
        private const string ComplexExpectedBicep_Output3Regex = "^$";

        private const string ComplexExpectedBicep_Filename4 = "nested_nestedDeploymentOuter.bicep";
        private const string ComplexExpectedBicep_Output4Regex = "resource storageaccount2 'Microsoft.Storage/storageAccounts@2021-04-01'";

        #endregion

        private BicepDecompileCommandHandler CreateCommandHandler(LanguageServerMock server)
        {
            return new BicepDecompileCommandHandler(
                Repository.Create<ISerializer>().Object,
                BicepTestConstants.Features,
                BicepTestConstants.NamespaceProvider,
                BicepTestConstants.ConfigurationManager,
                BicepTestConstants.RegistryProvider,
                server.ClientCapabilitiesProvider,
                server.Mock.Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object);
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            var server = new LanguageServerMock();

            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepDecompileCommandHandler handler = CreateCommandHandler(server);

#pragma warning disable VSTHRD002 // Throws synchronously
            var sut = () => handler.Handle(new(DocumentUri.File(path)), CancellationToken.None).Result;
#pragma warning restore VSTHRD002

            sut.Should().Throw<Exception>();
        }

        [TestMethod]
        public async Task SimpleJson_ShouldCreateSimpleBicepFile()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);

            var server = new LanguageServerMock();
            server.WindowMock.OnShowDocument();

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            var expectedBicepPath = Path.ChangeExtension(jsonPath, ".bicep");
            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().MatchRegex("Decompiling .*[\\\\/]main\\.json into Bicep\\.\\.\\.");
            result.output.Should().MatchRegex("Writing .*[\\\\/]main\\.bicep.");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main\\.bicep");
            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(SimpleExpectedBicep);
        }

        [TestMethod]
        public async Task SimpleJson_ShouldShowDisclaimerMessage()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);

            var server = new LanguageServerMock();
            server.WindowMock.OnShowDocument();

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            result.output.Should().Contain("WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.");
            result.output.Should().Contain("You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.");
        }

        [TestMethod]
        public async Task SimpleJson_ClientSupportsShowDocument_ShouldShowDocument()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);

            var server = new LanguageServerMock();
            DocumentUri? showDocUri = null;
            server.WindowMock.OnShowDocument(p => showDocUri = p.Uri);

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            showDocUri.Should().NotBeNull();
            showDocUri.Should().Be(DocumentUri.File(Path.ChangeExtension(jsonPath, ".bicep")));
        }

        [TestMethod]
        public async Task SimpleJson_ClientDoesntSupportShowDocument_ShouldNotShowDocument()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);

            var server = new LanguageServerMock();
            DocumentUri? showDocUri = null;
            server.WindowMock.OnShowDocument(
                p => showDocUri = p.Uri,
                enableClientCapability: false);

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            showDocUri.Should().BeNull();
        }

        [TestMethod]
        public async Task SimpleJson_PathDoesntExist_ShouldShowError()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            var server = new LanguageServerMock();

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(Path.Join(testOutputPath, "folder and file don't exist.json"))),
                CancellationToken.None);

            //result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Failed);
            result.output.Should().MatchRegex("Failed to read ");
        }

        [TestMethod]
        public async Task SimpleJson_InvalidSchema_ShouldShowError()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string json = SimpleJson.Replace("deploymentTemplate.json", "deploymentTemplate.whoops");
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", json, testOutputPath);

            var server = new LanguageServerMock();

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            result.bicepUri.Should().Be(null);
            result.status.Should().Be(BicepDecompileCommandStatus.Failed);
            result.output.Should().Contain("$schema value \"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.whoops#\" did not match any of the known ARM template deployment schemas.");
        }

        [TestMethod]
        public async Task SimpleJson_OutputFileAlreadyExists_AndUserSelectsOverwrite()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);
            var expectedBicepPath = Path.ChangeExtension(jsonPath, ".bicep");
            FileHelper.SaveResultFile(TestContext, "main.bicep", "existing bicep contents", testOutputPath);

            var server = new LanguageServerMock();
            string? message = null;
            server.WindowMock.OnShowMessageRequest(
                p =>
                {
                    message = p.Message;
                },
                new MessageActionItem() { Title = "Overwrite" });
            server.WindowMock.OnShowDocument();

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be("Output file already exists: \"main.bicep\"");
            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().MatchRegex("Overwriting .*[\\\\/]main\\.bicep.");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main\\.bicep");
            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(SimpleExpectedBicep);
        }

        [TestMethod]
        public async Task SimpleJson_OutputFileAlreadyExists_AndUserSelectsCancel()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);
            var expectedBicepPath = Path.ChangeExtension(jsonPath, ".bicep");
            FileHelper.SaveResultFile(TestContext, "main.bicep", "existing bicep contents", testOutputPath);

            var server = new LanguageServerMock();
            string? message = null;
            server.WindowMock.OnShowMessageRequest(
                p =>
                {
                    message = p.Message;
                },
                new MessageActionItem() { Title = "Cancel" });

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be("Output file already exists: \"main.bicep\"");
            result.bicepUri.Should().BeNull();
            result.status.Should().Be(BicepDecompileCommandStatus.Canceled);
            result.output.Should().NotMatchRegex("Overwriting");
            result.output.Should().NotMatchRegex("Writing");
            result.output.Should().MatchRegex("Decompile canceled.");
            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines("existing bicep contents");
        }

        [TestMethod]
        public async Task SimpleJson_OutputFileAlreadyExists_AndUserSelectsCopy()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main.bicep", "existing bicep contents", testOutputPath);

            var server = new LanguageServerMock();
            string? message = null;
            server.WindowMock.OnShowMessageRequest(
                p =>
                {
                    message = p.Message;
                },
                new MessageActionItem() { Title = "Create copy" });
            server.WindowMock.OnShowDocument();

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            var expectedBicepPath = Path.Join(testOutputPath, "main2.bicep");
            message.Should().Be("Output file already exists: \"main.bicep\"");
            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().NotMatchRegex("Overwriting");
            result.output.Should().MatchRegex("Writing .*[\\\\/]main2.bicep");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main2.bicep");
            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(SimpleExpectedBicep);
        }

        [TestMethod]
        public async Task SimpleJson_OutputFileAlreadyExists_AndLotsOfExistingCopies_AndUserSelectsCopy()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main.json", SimpleJson, testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main.bicep", "existing bicep contents", testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main2.bicep", "existing bicep2 contents", testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main3.bicep", "existing bicep3 contents", testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main4.bicep", "existing bicep4 contents", testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main5.bicep", "existing bicep5 contents", testOutputPath);
            Directory.CreateDirectory(Path.Join(testOutputPath, "main6.bicep"));
            var expectedBicepPath = Path.Join(testOutputPath, "main7.bicep");

            var server = new LanguageServerMock();
            string? message = null;
            string? displayedDoc = null;
            server.WindowMock.OnShowMessageRequest(
                p =>
                {
                    message = p.Message;
                },
                new MessageActionItem() { Title = "Create copy" });
            server.WindowMock.OnShowDocument(
                p =>
                {
                    displayedDoc = p.Uri.ToUri().LocalPath;
                });

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be("Output file already exists: \"main.bicep\"");
            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().NotMatchRegex("Overwriting");
            result.output.Should().MatchRegex("Writing .*[\\\\/]main7.bicep");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main7.bicep");
            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(SimpleExpectedBicep);
            displayedDoc!.ToLowerInvariant().Should().Be(expectedBicepPath.ToLowerInvariant(), "Should have displayed new file");
        }

        [TestMethod]
        public async Task MultiFileDecompilation_ShouldCreateOutputFiles()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main file.json", ComplexJson, testOutputPath);
            var expectedBicepPath = Path.Join(testOutputPath, "main file.bicep");

            var server = new LanguageServerMock();
            string? displayedDoc = null;
            server.WindowMock.OnShowDocument(p => displayedDoc = p.Uri.ToUri().LocalPath);

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().NotMatchRegex("Overwriting");
            result.output.Should().MatchRegex("Writing .*[\\\\/]main file.bicep");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename2}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename3}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename4}");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main file.bicep");

            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(ComplexExpectedBicep_MainOutput);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename2)).Should().MatchRegex(ComplexExpectedBicep_Output2Regex);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename3)).Should().MatchRegex(ComplexExpectedBicep_Output3Regex);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename4)).Should().MatchRegex(ComplexExpectedBicep_Output4Regex);

            displayedDoc!.ToLowerInvariant().Should().Be(expectedBicepPath.ToLowerInvariant(), "Should have displayed main file");
        }


        [TestMethod]
        public async Task MultiFileDecompilation_MainFileConflicts_UserSelectsOverwriteAll()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main file.json", ComplexJson, testOutputPath);
            var expectedBicepPath = Path.Join(testOutputPath, "main file.bicep");
            FileHelper.SaveResultFile(TestContext, "main file.bicep", "existing bicep", testOutputPath);

            var server = new LanguageServerMock();
            string? message = null;
            string? displayedDoc = null;
            server.WindowMock.OnShowDocument(p => displayedDoc = p.Uri.ToUri().LocalPath);
            server.WindowMock.OnShowMessageRequest(p => message = p.Message, new MessageActionItem() { Title = "Overwrite all" });

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be("There are multiple decompilation output files and the following already exist: \"main file.bicep\"");
            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().MatchRegex("Overwriting .*[\\\\/]main file.bicep");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename2}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename3}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename4}");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main file.bicep");

            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(ComplexExpectedBicep_MainOutput);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename2)).Should().MatchRegex(ComplexExpectedBicep_Output2Regex);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename3)).Should().MatchRegex(ComplexExpectedBicep_Output3Regex);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename4)).Should().MatchRegex(ComplexExpectedBicep_Output4Regex);

            displayedDoc!.ToLowerInvariant().Should().Be(expectedBicepPath.ToLowerInvariant(), "Should have displayed main file");
        }

        [TestMethod]
        public async Task MultiFileDecompilation_OtherFilesConflict_UserSelectsOverwriteAll()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main file.json", ComplexJson, testOutputPath);
            var expectedBicepPath = Path.Join(testOutputPath, "main file.bicep");
            FileHelper.SaveResultFile(TestContext, "main file.bicep", "existing bicep", testOutputPath);
            FileHelper.SaveResultFile(TestContext, ComplexExpectedBicep_Filename3, "existing bicep 3", testOutputPath);

            var server = new LanguageServerMock();
            string? message = null;
            string? displayedDoc = null;
            server.WindowMock.OnShowDocument(p => displayedDoc = p.Uri.ToUri().LocalPath);
            server.WindowMock.OnShowMessageRequest(p => message = p.Message, new MessageActionItem() { Title = "Overwrite all" });

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be($"There are multiple decompilation output files and the following already exist: \"main file.bicep\", \"{ComplexExpectedBicep_Filename3}\"");
            result.bicepUri.Should().Be(new Uri(expectedBicepPath));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().MatchRegex("Overwriting .*[\\\\/]main file.bicep");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename2}");
            result.output.Should().MatchRegex($"Overwriting .*[\\\\/]{ComplexExpectedBicep_Filename3}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]{ComplexExpectedBicep_Filename4}");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main file.bicep");

            File.ReadAllText(expectedBicepPath).Should().BeEquivalentToIgnoringNewlines(ComplexExpectedBicep_MainOutput);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename2)).Should().MatchRegex(ComplexExpectedBicep_Output2Regex);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename3)).Should().MatchRegex(ComplexExpectedBicep_Output3Regex);
            File.ReadAllText(Path.Join(testOutputPath, ComplexExpectedBicep_Filename4)).Should().MatchRegex(ComplexExpectedBicep_Output4Regex);

            displayedDoc!.ToLowerInvariant().Should().Be(expectedBicepPath.ToLowerInvariant(), "Should have displayed main file");
        }

        [TestMethod]
        public async Task MultiFileDecompilation_OtherFilesConflict_UserSelectsCancell()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main file.json", ComplexJson, testOutputPath);
            var expectedBicepPath = Path.Join(testOutputPath, "main file.bicep");
            FileHelper.SaveResultFile(TestContext, "main file.bicep", "existing bicep", testOutputPath);
            FileHelper.SaveResultFile(TestContext, ComplexExpectedBicep_Filename3, "existing bicep 3", testOutputPath);

            var server = new LanguageServerMock();
            string? message = null;
            server.WindowMock.OnShowDocumentThrow(enableClientCapability: true);
            server.WindowMock.OnShowMessageRequest(p => message = p.Message, new MessageActionItem() { Title = "Cancel" });

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be($"There are multiple decompilation output files and the following already exist: \"main file.bicep\", \"{ComplexExpectedBicep_Filename3}\"");
            result.bicepUri.Should().BeNull();
            result.status.Should().Be(BicepDecompileCommandStatus.Canceled);
            result.output.Should().NotMatchRegex("Overwriting");
            result.output.Should().NotMatchRegex($"Writing");
        }


        [TestMethod]
        public async Task MultiFileDecompilation_OtherFilesConflict_UserSelectsCopy()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            string jsonPath = FileHelper.SaveResultFile(TestContext, "main file.json", ComplexJson, testOutputPath);
            FileHelper.SaveResultFile(TestContext, "main file.bicep", "existing bicep", testOutputPath);
            FileHelper.SaveResultFile(TestContext, ComplexExpectedBicep_Filename3, "existing bicep 3", testOutputPath);
            Directory.CreateDirectory(Path.Join(testOutputPath, "main file_decompiled"));
            var expectedOutputPath = Path.Join(testOutputPath, $"main file_decompiled2");

            var server = new LanguageServerMock();
            string? message = null;
            string? displayedDoc = null;
            server.WindowMock.OnShowDocument(p => displayedDoc = p.Uri.ToUri().LocalPath);
            server.WindowMock.OnShowMessageRequest(p => message = p.Message, new MessageActionItem() { Title = "New subfolder" });

            var sut = CreateCommandHandler(server);
            var result = await sut.Handle(
                new BicepDecompileCommandParams(DocumentUri.File(jsonPath)),
                CancellationToken.None);

            message.Should().Be($"There are multiple decompilation output files and the following already exist: \"main file.bicep\", \"{ComplexExpectedBicep_Filename3}\"");
            result.bicepUri.Should().Be(new Uri(Path.Join(expectedOutputPath, "main file.bicep")));
            result.status.Should().Be(BicepDecompileCommandStatus.Success);
            result.output.Should().NotMatchRegex("Overwriting");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]main file_decompiled2[\\\\//]main file.bicep");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]main file_decompiled2[\\\\//]{ComplexExpectedBicep_Filename2}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]main file_decompiled2[\\\\//]{ComplexExpectedBicep_Filename3}");
            result.output.Should().MatchRegex($"Writing .*[\\\\/]main file_decompiled2[\\\\//]{ComplexExpectedBicep_Filename4}");
            result.output.Should().MatchRegex("Finished decompiling to .*[\\\\/]main file_decompiled2[\\\\//]main file.bicep");

            File.ReadAllText(Path.Join(expectedOutputPath, "main file.bicep")).Should().BeEquivalentToIgnoringNewlines(ComplexExpectedBicep_MainOutput);
            File.ReadAllText(Path.Join(expectedOutputPath, ComplexExpectedBicep_Filename2)).Should().MatchRegex(ComplexExpectedBicep_Output2Regex);
            File.ReadAllText(Path.Join(expectedOutputPath, ComplexExpectedBicep_Filename3)).Should().MatchRegex(ComplexExpectedBicep_Output3Regex);
            File.ReadAllText(Path.Join(expectedOutputPath, ComplexExpectedBicep_Filename4)).Should().MatchRegex(ComplexExpectedBicep_Output4Regex);

            displayedDoc!.ToLowerInvariant().Should().Be(Path.Join(expectedOutputPath, "main file.bicep").ToLowerInvariant(), "Should have displayed main file");
        }
    }
}
