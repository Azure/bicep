// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDeploymentScopeRequestHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly FileResolver FileResolver = BicepTestConstants.FileResolver;
        private static readonly IConfigurationManager configurationManager = new ConfigurationManager(new IOFileSystem());
        private readonly ModuleDispatcher ModuleDispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider, configurationManager);
        private readonly ISerializer Serializer = StrictMock.Of<ISerializer>().Object;

        [TestMethod]
        public async Task Handle_WithInvalidInputFile_ReturnsBicepDeploymentScopeResponseWithErrorMessage()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);

            BicepDeploymentScopeRequestHandler bicepDeploymentScopeRequestHandler = new BicepDeploymentScopeRequestHandler(
                bicepCompilationManager,
                configurationManager,
                new DeploymentFileCompilationCache(),
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider,
                Serializer,
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer);

            var textDocumentIdentifier = new TextDocumentIdentifier(documentUri);
            BicepDeploymentScopeParams bicepDeploymentScopeParams = new BicepDeploymentScopeParams(textDocumentIdentifier);

            var result = await bicepDeploymentScopeRequestHandler.Handle(bicepDeploymentScopeParams, CancellationToken.None);

            result.scope.Should().Be(ResourceScope.None.ToString());
            result.template.Should().BeNull();
            result.errorMessage.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Handle_WithInvalidConfigurationFile_ReturnsBicepDeploymentScopeResponseWithErrorMessage()
        {
            string outputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents, outputPath);
            FileHelper.SaveResultFile(TestContext, "bicepconfig.json", "invalid json", outputPath);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);

            BicepDeploymentScopeRequestHandler bicepDeploymentScopeRequestHandler = new BicepDeploymentScopeRequestHandler(
                bicepCompilationManager,
                configurationManager,
                new DeploymentFileCompilationCache(),
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider,
                Serializer,
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer);

            var textDocumentIdentifier = new TextDocumentIdentifier(documentUri);
            BicepDeploymentScopeParams bicepDeploymentScopeParams = new BicepDeploymentScopeParams(textDocumentIdentifier);

            var result = await bicepDeploymentScopeRequestHandler.Handle(bicepDeploymentScopeParams, CancellationToken.None);

            result.scope.Should().Be(ResourceScope.None.ToString());
            result.template.Should().BeNull();
            result.errorMessage.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Handle_WithValidInputFile_ReturnsBicepDeploymentScopeResponse()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);

            BicepDeploymentScopeRequestHandler bicepDeploymentScopeRequestHandler = new BicepDeploymentScopeRequestHandler(
                bicepCompilationManager,
                configurationManager,
                new DeploymentFileCompilationCache(),
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider,
                Serializer,
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer);

            var textDocumentIdentifier = new TextDocumentIdentifier(documentUri);
            BicepDeploymentScopeParams bicepDeploymentScopeParams = new BicepDeploymentScopeParams(textDocumentIdentifier);

            var result = await bicepDeploymentScopeRequestHandler.Handle(bicepDeploymentScopeParams, CancellationToken.None);

            result.scope.Should().Be(LanguageConstants.TargetScopeTypeResourceGroup);
            result.template.Should().BeEquivalentToIgnoringNewlines(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""dev"",
      ""templateHash"": ""9722914539529618239""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""name"",
      ""location"": ""global""
    }
  ]
}");
            result.errorMessage.Should().BeNull();
        }

        [DataRow(LanguageConstants.TargetScopeTypeManagementGroup, LanguageConstants.TargetScopeTypeManagementGroup)]
        [DataRow(LanguageConstants.TargetScopeTypeResourceGroup, LanguageConstants.TargetScopeTypeResourceGroup)]
        [DataRow(LanguageConstants.TargetScopeTypeSubscription, LanguageConstants.TargetScopeTypeSubscription)]
        [DataRow(LanguageConstants.TargetScopeTypeTenant, LanguageConstants.TargetScopeTypeTenant)]
        [DataRow("Invalid_Scope", "None")]
        [DataTestMethod]
        public async Task Handle_WithValidInputFile_VerifyDeploymentScope(string scope, string result)
        {
            string bicepFileContents = @"targetScope = '" + scope + "\'" + "\n" +
@"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);

            BicepDeploymentScopeRequestHandler bicepDeploymentScopeRequestHandler = new BicepDeploymentScopeRequestHandler(
                bicepCompilationManager,
                configurationManager,
                new DeploymentFileCompilationCache(),
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider,
                Serializer,
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer);

            var textDocumentIdentifier = new TextDocumentIdentifier(documentUri);
            BicepDeploymentScopeParams bicepDeploymentScopeParams = new BicepDeploymentScopeParams(textDocumentIdentifier);

            var bicepDeploymentScopeResponse = await bicepDeploymentScopeRequestHandler.Handle(bicepDeploymentScopeParams, CancellationToken.None);

            bicepDeploymentScopeResponse.scope.Should().Be(result);
        }

        [TestMethod]
        public async Task Handle_WithValidInputFile_VerifyCompilationEntryIsAddedToDeploymentFileCompilationCache()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var deploymentFileCompilationCache = new DeploymentFileCompilationCache();

            BicepDeploymentScopeRequestHandler bicepDeploymentScopeRequestHandler = new BicepDeploymentScopeRequestHandler(
                bicepCompilationManager,
                configurationManager,
                deploymentFileCompilationCache,
                BicepTestConstants.FeatureProviderFactory,
                FileResolver,
                ModuleDispatcher,
                BicepTestConstants.NamespaceProvider,
                Serializer,
                BicepTestConstants.ApiVersionProviderFactory,
                BicepTestConstants.LinterAnalyzer);

            var textDocumentIdentifier = new TextDocumentIdentifier(documentUri);
            BicepDeploymentScopeParams bicepDeploymentScopeParams = new BicepDeploymentScopeParams(textDocumentIdentifier);

            await bicepDeploymentScopeRequestHandler.Handle(bicepDeploymentScopeParams, CancellationToken.None);

            var compilationFromDeploymentFileCompilationCache = deploymentFileCompilationCache.FindAndRemoveCompilation(documentUri);
            var compilationFromCompilationManager = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;

            compilationFromCompilationManager.Should().NotBeNull();
            compilationFromDeploymentFileCompilationCache?.Should().NotBeNull();
            compilationFromCompilationManager.Should().BeSameAs(compilationFromDeploymentFileCompilationCache!);
        }
    }
}
