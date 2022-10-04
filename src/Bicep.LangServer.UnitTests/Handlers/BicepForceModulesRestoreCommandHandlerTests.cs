// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
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
    public class BicepForceModulesRestoreCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly FileResolver FileResolver = BicepTestConstants.FileResolver;
        private static readonly MockRepository Repository = new(MockBehavior.Strict);
        private static readonly ISerializer Serializer = Repository.Create<ISerializer>().Object;
        private static readonly IConfigurationManager configurationManager = new ConfigurationManager(new IOFileSystem());
        private readonly ModuleDispatcher ModuleDispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider, configurationManager);

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void Handle_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepForceModulesRestoreCommandHandler bicepforceModulesRestoreCommandHandler = new BicepForceModulesRestoreCommandHandler(Serializer, FileResolver, ModuleDispatcher);

            Action sut = () => bicepforceModulesRestoreCommandHandler.Handle(path, CancellationToken.None);

            sut.Should().Throw<ArgumentException>().WithMessage("Invalid input file path");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_WithoutModules_ReturnsBuildSucceededMessage()
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            string bicepFileContents = @"
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            Uri bicepFileUri = new Uri(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            BicepForceModulesRestoreCommandHandler bicepForceModulesRestoreCommandHandler = new BicepForceModulesRestoreCommandHandler(Repository.Create<ISerializer>().Object, FileResolver, ModuleDispatcher);
            string expected = await bicepForceModulesRestoreCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Restore (force) skipped. No modules references in input file.");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndThreeLocalModulesInInputFile_ReturnsSummaryMessage()
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            string bicepLocalModuleFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}";

            string bicepFileContents = @"
module localModule './localmodule1.bicep' = {
  name: 'localModuleDeploy1'
}

module localModule './localmodule2.bicep' = {
  name: 'localModuleDeploy2'
}

// same module, different reference
module localModule3 './localmodule2.bicep' = {
  name: 'localModuleDeploy3'
}

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            // save 2 copies of the module
            string bicepLocalModule1FilePath = FileHelper.SaveResultFile(TestContext, "localmodule1.bicep", bicepLocalModuleFileContents, testOutputPath);
            string bicepLocalModule2FilePath = FileHelper.SaveResultFile(TestContext, "localmodule2.bicep", bicepLocalModuleFileContents, testOutputPath);
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            Uri bicepFileUri = new Uri(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            BicepForceModulesRestoreCommandHandler bicepForceModulesRestoreCommandHandler = new BicepForceModulesRestoreCommandHandler(Repository.Create<ISerializer>().Object, FileResolver, ModuleDispatcher);

            string expected = StringUtils.ReplaceNewlines(await bicepForceModulesRestoreCommandHandler.Handle(bicepFilePath, CancellationToken.None), "|");

            expected.Should().Be(@"Restore (force) summary: |  * ./localmodule1.bicep: Succeeded|  * ./localmodule2.bicep: Succeeded");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndTwoLocalModules_InInputFile_WhichOneDoNotExist_ReturnsSummaryMessage()
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            string bicepLocalModuleFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: 'global'
}";

            string bicepFileContents = @"
module localModule './localmodule1.bicep' = {
  name: 'localModuleDeploy1'
}

module localModule './localmodule2.bicep' = {
  name: 'localModuleDeploy2'
}

resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepLocalModule1FilePath = FileHelper.SaveResultFile(TestContext, "localmodule1.bicep", bicepLocalModuleFileContents, testOutputPath);
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            Uri bicepFileUri = new Uri(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);

            BicepForceModulesRestoreCommandHandler bicepForceModulesRestoreCommandHandler = new BicepForceModulesRestoreCommandHandler(Repository.Create<ISerializer>().Object, FileResolver, ModuleDispatcher);

            string expected = StringUtils.ReplaceNewlines(await bicepForceModulesRestoreCommandHandler.Handle(bicepFilePath, CancellationToken.None), "|");

            expected.Should().Be(@"Restore (force) summary: |  * ./localmodule1.bicep: Succeeded|  * ./localmodule2.bicep: Succeeded");
        }


        // One scenario not tested here is when we have an external module and another file than the module lock is locked, which prevent the directory delete. We don't have a test for the message
     }
}
