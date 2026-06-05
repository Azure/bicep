// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.InMemory;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using Bicep.TextFixtures.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class VisualGraphUpdateTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task Initial_request_with_null_current_returns_a_full_add_delta()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new VisualGraphUpdateParams(new TextDocumentIdentifier(helper.MainUri), Current: null),
                default);

            result.Should().NotBeNull();

            var addedNodes = result.Patches.OfType<GraphPatch.AddNode>().Select(patch => patch.Node).ToList();
            addedNodes.Select(node => node.Id).Should().BeEquivalentTo("res1", "res2", "mod1", "mod1::res3");

            var res2 = addedNodes.Single(node => node.Id == "res2");
            res2.Kind.Should().Be(GraphNodeKind.Resource);
            res2.Type.Should().Be("Test.Rp/readWriteTests");
            res2.ParentId.Should().BeNull();

            var mod1 = addedNodes.Single(node => node.Id == "mod1");
            mod1.Kind.Should().Be(GraphNodeKind.Module);
            mod1.Type.Should().Be("<module>");
            mod1.HasChildren.Should().BeTrue();

            var res3 = addedNodes.Single(node => node.Id == "mod1::res3");
            res3.Kind.Should().Be(GraphNodeKind.Resource);
            res3.ParentId.Should().Be("mod1");

            result.Patches.OfType<GraphPatch.AddEdge>().Select(patch => (patch.Edge.SourceId, patch.Edge.TargetId))
                .Should().Contain(("res2", "mod1"));

            result.Patches.OfType<GraphPatch.RemoveNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.RemoveEdge>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.UpdateNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.SetErrorCount>().Single().ErrorCount.Should().Be(0);
        }

        [TestMethod]
        public async Task Request_with_matching_current_returns_only_metadata_refresh()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            // First obtain the server's view, then submit it back as the rendered graph.
            var initial = await client.SendRequest(
                new VisualGraphUpdateParams(new TextDocumentIdentifier(helper.MainUri), Current: null),
                default);

            var renderedNodes = initial.Patches.OfType<GraphPatch.AddNode>()
                .Select(patch => new RenderedGraphNode(patch.Node.Id, patch.Node.Kind, patch.Node.ParentId, 100, 50))
                .ToList();
            var renderedEdges = initial.Patches.OfType<GraphPatch.AddEdge>()
                .Select(patch => new RenderedGraphEdge(patch.Edge.Id, patch.Edge.SourceId, patch.Edge.TargetId))
                .ToList();

            var result = await client.SendRequest(
                new VisualGraphUpdateParams(
                    new TextDocumentIdentifier(helper.MainUri),
                    new RenderedGraph(renderedNodes, renderedEdges)),
                default);

            // Topology is unchanged, so no structural patches, just the idempotent metadata refresh + error count.
            result.Patches.OfType<GraphPatch.AddNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.RemoveNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.AddEdge>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.RemoveEdge>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.UpdateNode>().Select(patch => patch.NodeId)
                .Should().BeEquivalentTo("res1", "res2", "mod1", "mod1::res3");
            result.Patches.OfType<GraphPatch.SetErrorCount>().Single().ErrorCount.Should().Be(0);
        }

        private async Task<TestServer> StartServerAndOpenAsync()
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var mainContent = """
                resource res1 'Test.Rp/basicTests@2020-01-01' = {
                  name: 'res1'
                }

                resource res2 'Test.Rp/readWriteTests@2020-01-01' = {
                  name: 'res2'
                  properties: {
                    readwrite: mod1.outputs.output1
                  }
                }

                module mod1 './modules/module1.bicep' = {
                  name: 'mod1'
                }
                """;
            var fileSet = InMemoryTestFileSet.Create(
                ("/main.bicep", mainContent),
                ("/modules/module1.bicep", """
                    resource res3 'Test.Rp/basicTests@2020-01-01' = {
                      name: 'res3'
                    }

                    output output1 int = 123
                    """));

            var mainUri = fileSet.GetUri("main.bicep");
            var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFileExplorer(fileSet.FileExplorer));

            helper.Client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri.ToDocumentUri(), mainContent, 1));
            await diagnosticsListener.WaitNext();

            return new TestServer(helper, mainUri.ToDocumentUri());
        }

        private sealed class TestServer : IDisposable
        {
            public TestServer(LanguageServerHelper helper, DocumentUri mainUri)
            {
                this.Helper = helper;
                this.MainUri = mainUri;
            }

            public LanguageServerHelper Helper { get; }

            public DocumentUri MainUri { get; }

            public void Dispose() => this.Helper.Dispose();
        }
    }
}
