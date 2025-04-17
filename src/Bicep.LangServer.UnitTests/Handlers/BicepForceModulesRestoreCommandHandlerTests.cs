// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepForceModulesRestoreCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static BicepForceModulesRestoreCommandHandler CreateHandler(ICompilationManager compilationManager)
        {
            var helper = ServiceBuilder.Create(services => services
                .AddSingleton<ICompilationManager>(compilationManager)
                .AddSingleton(StrictMock.Of<ISerializer>().Object)
                .AddSingleton<BicepForceModulesRestoreCommandHandler>());

            return helper.Construct<BicepForceModulesRestoreCommandHandler>();
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task Handle_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            var compilationManager = StrictMock.Of<ICompilationManager>().Object;
            var handler = CreateHandler(compilationManager);

            Func<Task> sut = () => handler.Handle(path, CancellationToken.None);

            await sut.Should().ThrowAsync<UriFormatException>();
        }

        [TestMethod]
        public async Task Handle_WithValidPath_WithoutModules_ReturnsBuildSucceededMessage()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

            string bicepFileContents = @"
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            Uri bicepFileUri = new(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var handler = CreateHandler(compilationManager);
            string expected = await handler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Restore (force) skipped. No modules references in input file.");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndThreeLocalModulesInInputFile_ReturnsSummaryMessage()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

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
            Uri bicepFileUri = new(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var handler = CreateHandler(compilationManager);

            string expected = StringUtils.ReplaceNewlines(await handler.Handle(bicepFilePath, CancellationToken.None), "|");

            expected.Should().Be(@"Restore (force) summary: |  * ./localmodule1.bicep: Succeeded|  * ./localmodule2.bicep: Succeeded");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndTwoLocalModules_InInputFile_WhichOneDoNotExist_ReturnsSummaryMessage()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

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
            Uri bicepFileUri = new(bicepFilePath);

            DocumentUri documentUri = DocumentUri.From(bicepFileUri);
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var handler = CreateHandler(compilationManager);

            string expected = StringUtils.ReplaceNewlines(await handler.Handle(bicepFilePath, CancellationToken.None), "|");

            expected.Should().Be(@"Restore (force) summary: |  * ./localmodule1.bicep: Succeeded|  * ./localmodule2.bicep: Failed");
        }


        // One scenario not tested here is when we have an external module and another file than the module lock is locked, which prevent the directory delete. We don't have a test for the message
    }
}
