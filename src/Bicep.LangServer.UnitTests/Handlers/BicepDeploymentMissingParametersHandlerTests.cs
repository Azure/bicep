// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDeploymentMissingParametersHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

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
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer, BicepTestConstants.Features, BicepTestConstants.EmitterSettings, BicepTestConstants.NamespaceProvider, FileResolver, ModuleDispatcher, configurationManager);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().Be(@"Build succeeded. Created file input.json");
        }
    }
}
