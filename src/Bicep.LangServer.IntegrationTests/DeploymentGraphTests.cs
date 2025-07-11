// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Handlers;
using Bicep.TextFixtures.IO;
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
            var mainContent = """
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
                """;
            var fileSet = InMemoryTestFileSet.Create(
                ("/main.bicep", mainContent),
                ("/modules/module1.bicep", """
                    resource res3 'Test.Rp/basicTests@2020-01-01' = {
                      name: 'res3'
                    }

                    output output1 int = 123
                    """),
                ("/modules/module2.bicep", """
                    resource res4 'Test.Rp/basicTests@2020-01-01' = {
                      name: 'res4'
                    }

                    module nestedMod './nestedModules/nestedModule.bicep' = [for x in []: {
                      name: 'nestedMod'
                      dependsOn: [
                        res4
                      ]
                    }]
                    """),
                ("/modules/nestedModules/nestedModule.bicep", """
                    resource res5 'Test.Rp/basicTests@2020-01-01' = {
                      name: 'res5'
                    }
                    """));

            var mainUri = fileSet.GetUri("main.bicep");
            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFileExplorer(fileSet.FileExplorer));
            var client = helper.Client;

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri.ToDocumentUri(), mainContent, 1));
            await diagnosticsListener.WaitNext();

            var deploymentGraph = await client.SendRequest(new BicepDeploymentGraphParams(new TextDocumentIdentifier(mainUri.ToDocumentUri())), default);

            deploymentGraph.Should().NotBeNull();
            deploymentGraph!.Nodes.Should().Equal(
                new BicepDeploymentGraphNode("mod1", "<module>", false, CreateTextRange(14, 0, 16, 1), true, false, "/path/to/main.bicep"),
                new BicepDeploymentGraphNode("mod1::res3", "Test.Rp/basicTests", false, CreateTextRange(0, 0, 2, 1), false, false, "/path/to/modules/module1.bicep"),
                new BicepDeploymentGraphNode("mod2", "<module>", false, CreateTextRange(18, 0, 20, 1), true, false, "/path/to/main.bicep"),
                new BicepDeploymentGraphNode("mod2::nestedMod", "<module>", true, CreateTextRange(4, 0, 9, 2), true, false, "/path/to/modules/module2.bicep"),
                new BicepDeploymentGraphNode("mod2::nestedMod::res5", "Test.Rp/basicTests", false, CreateTextRange(0, 0, 2, 1), false, false, "/path/to/modules/nestedModules/nestedModule.bicep"),
                new BicepDeploymentGraphNode("mod2::res4", "Test.Rp/basicTests", false, CreateTextRange(0, 0, 2, 1), false, false, "/path/to/modules/module2.bicep"),
                new BicepDeploymentGraphNode("nonExistingMod", "<module>", false, CreateTextRange(22, 0, 23, 1), false, true, "/path/to/main.bicep"),
                new BicepDeploymentGraphNode("res1", "Test.Rp/basicTests", false, CreateTextRange(0, 0, 2, 1), false, false, "/path/to/main.bicep"),
                new BicepDeploymentGraphNode("res2", "Test.Rp/readWriteTests", false, CreateTextRange(4, 0, 9, 1), false, true, "/path/to/main.bicep"));
            deploymentGraph!.Edges.Should().Equal(
                new BicepDeploymentGraphEdge("mod2::nestedMod", "mod2::res4"),
                new BicepDeploymentGraphEdge("res2", "mod1"));
            deploymentGraph!.ErrorCount.Should().Be(6);
        }

        private static TextRange CreateTextRange(int startLine, int startCharacter, int endLine, int endCharacter) =>
            new(new(startLine, startCharacter), new(endLine, endCharacter));
    }
}
