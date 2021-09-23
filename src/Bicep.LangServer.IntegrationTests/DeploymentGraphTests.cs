// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using TextRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class DeploymentGraphTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task RequestDeploymentGraphShouldReturnDeploymentGraph()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var fileSystemDict = new Dictionary<Uri, string>();

            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsParams => diagnosticsListener.AddMessage(diagnosticsParams)),
                new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: new InMemoryFileResolver(fileSystemDict)));

            var mainUri = DocumentUri.FromFileSystemPath("/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"
resource res1 'Test.Rp/basicTests@2020-01-01' = {
  name: 'res1'
}

resource res2 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'res2'
  properites: {
    readwrite: mod1.outputs.output1
  }
}

resource unknownRes = {
}

module mod1 './modules/module1.bicep' = {
  name: 'mod1'
}

module mod2 './modules/module2.bicep' = {
  name: 'mod2'
}

module nonExistingMod './path/to/nonExistingModule.bicep' = {
}
";

            var module1Uri = DocumentUri.FromFileSystemPath("/modules/module1.bicep");
            fileSystemDict[module1Uri.ToUri()] = @"
resource res3 'Test.Rp/basicTests@2020-01-01' = {
  name: 'res3'
}

output output1 int = 123
";

            var module2Uri = DocumentUri.FromFileSystemPath("/modules/module2.bicep");
            fileSystemDict[module2Uri.ToUri()] = @"
resource res4 'Test.Rp/basicTests@2020-01-01' = {
  name: 'res4'
}

module nestedMod './nestedModules/nestedModule.bicep' = [for x in []: {
  name: 'nestedMod'
  dependsOn: [
    res4
  ]
}]
";

            var nestedModuleUri = DocumentUri.FromFileSystemPath("/modules/nestedModules/nestedModule.bicep");
            fileSystemDict[nestedModuleUri.ToUri()] = @"
resource res5 'Test.Rp/basicTests@2020-01-01' = {
  name: 'res5'
}
";

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));
            await diagnosticsListener.WaitNext();

            var deploymentGraph = await client.SendRequest(new BicepDeploymentGraphParams(new TextDocumentIdentifier(mainUri)), default);

            deploymentGraph.Should().NotBeNull();
            deploymentGraph!.Nodes.Should().Equal(
                new BicepDeploymentGraphNode("mod1", "<module>", false, CreateTextRange(15, 0, 17, 1), true, false, Path.GetFullPath(module1Uri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("mod1::res3", "Test.Rp/basicTests", false, CreateTextRange(1, 0, 3, 1), false, false, Path.GetFullPath(module1Uri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("mod2", "<module>", false, CreateTextRange(19, 0, 21, 1), true, false, Path.GetFullPath(module2Uri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("mod2::nestedMod", "<module>", true, CreateTextRange(5, 0, 9, 1), true, false, Path.GetFullPath(nestedModuleUri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("mod2::nestedMod::res5", "Test.Rp/basicTests", false, CreateTextRange(1, 0, 3, 1), false, false, Path.GetFullPath(nestedModuleUri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("mod2::res4", "Test.Rp/basicTests", false, CreateTextRange(1, 0, 3, 1), false, false, Path.GetFullPath(module2Uri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("nonExistingMod", "<module>", false, CreateTextRange(23, 0, 24, 1), false, true, Path.GetFullPath("/path/to/nonExistingModule.bicep")),
                new BicepDeploymentGraphNode("res1", "Test.Rp/basicTests", false, CreateTextRange(1, 0, 3, 1), false, false, Path.GetFullPath(mainUri.GetFileSystemPath())),
                new BicepDeploymentGraphNode("res2", "Test.Rp/readWriteTests", false, CreateTextRange(5, 0, 10, 1), false, true, Path.GetFullPath(mainUri.GetFileSystemPath())));
            deploymentGraph!.Edges.Should().Equal(
                new BicepDeploymentGraphEdge("mod2::nestedMod", "mod2::res4"),
                new BicepDeploymentGraphEdge("res2", "mod1"));
            deploymentGraph!.ErrorCount.Should().Be(7);
        }

        private static TextRange CreateTextRange(int startLine, int startCharacter, int endLine, int endCharacter) =>
            new(new (startLine, startCharacter), new (endLine, endCharacter));
    }
}
