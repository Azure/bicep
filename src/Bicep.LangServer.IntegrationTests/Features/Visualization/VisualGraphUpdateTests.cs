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
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        public async Task VisualGraphUpdate_ForNullCurrent_ReturnsFullAddDelta()
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
            result.Patches.OfType<GraphPatch.SetNodeLayout>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.SetErrorCount>().Single().ErrorCount.Should().Be(0);
        }

        [TestMethod]
        public async Task VisualGraphUpdate_ForMatchingCurrent_EmitsNoMetadataPatches()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            // First obtain the server's view, then submit it back as the rendered graph.
            var initial = await client.SendRequest(
                new VisualGraphUpdateParams(new TextDocumentIdentifier(helper.MainUri), Current: null),
                default);

            var renderedNodes = initial.Patches.OfType<GraphPatch.AddNode>()
                .Select(patch => Rendered(patch.Node, 100, 50))
                .ToList();
            var renderedEdges = initial.Patches.OfType<GraphPatch.AddEdge>()
                .Select(patch => new RenderedGraphEdge(patch.Edge.Id, patch.Edge.SourceId, patch.Edge.TargetId))
                .ToList();

            var result = await client.SendRequest(
                new VisualGraphUpdateParams(
                    new TextDocumentIdentifier(helper.MainUri),
                    new RenderedGraph(renderedNodes, renderedEdges)),
                default);

            // Topology and metadata are unchanged, so the server emits no node patches at all, only the
            // error-count refresh. A whitespace-only edit therefore costs O(0) node updates.
            result.Patches.OfType<GraphPatch.AddNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.RemoveNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.AddEdge>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.RemoveEdge>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.SetNodeLayout>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.UpdateNode>().Should().BeEmpty();
            result.Patches.OfType<GraphPatch.SetErrorCount>().Single().ErrorCount.Should().Be(0);
        }

        [TestMethod]
        public async Task VisualGraphLayout_ForMeasuredMatchingCurrent_ReturnsLayoutPatches()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var initial = await client.SendRequest(
                new VisualGraphUpdateParams(new TextDocumentIdentifier(helper.MainUri), Current: null),
                default);

            var renderedNodes = initial.Patches.OfType<GraphPatch.AddNode>()
                .Select(patch => Rendered(patch.Node, 180, 72))
                .ToList();
            var renderedEdges = initial.Patches.OfType<GraphPatch.AddEdge>()
                .Select(patch => new RenderedGraphEdge(patch.Edge.Id, patch.Edge.SourceId, patch.Edge.TargetId))
                .ToList();

            var result = await client.SendRequest(
                new VisualGraphLayoutParams(
                    new TextDocumentIdentifier(helper.MainUri),
                    new RenderedGraph(renderedNodes, renderedEdges),
                    Options: null),
                default);

            result.Status.Should().Be(VisualGraphLayoutStatus.Ok);
            result.Patches.OfType<GraphPatch.SetNodeLayout>().Select(patch => patch.NodeId)
                .Should().BeEquivalentTo("res1", "res2", "mod1", "mod1::res3");
            result.Patches.OfType<GraphPatch.SetNodeLayout>().Select(patch => patch.Layout)
                .Should().OnlyContain(layout => double.IsFinite(layout.X) && double.IsFinite(layout.Y));

            // The engine returns the whole-graph bounds alongside the positions so the webview can fit the
            // viewport without re-deriving module box extents.
            result.Patches.OfType<GraphPatch.SetGraphBounds>().Should().ContainSingle()
                .Which.Bounds.Should().Match<GraphBounds>(bounds => bounds.Width > 0 && bounds.Height > 0);
            result.Patches.Should().AllSatisfy(patch =>
                patch.Should().Match(p => p is GraphPatch.SetNodeLayout || p is GraphPatch.SetGraphBounds));
        }

        [TestMethod]
        public async Task VisualGraphLayout_ForStaleMeasuredCurrent_ReturnsGraphChanged()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;
            var stale = new RenderedGraph(
                Nodes: [new RenderedGraphNode("missing", GraphNodeKind.Resource, ParentId: null, Type: "Test.Rp/basicTests", IsCollection: false, HasChildren: false, HasError: false, Width: 180, Height: 72)],
                Edges: []);

            var result = await client.SendRequest(
                new VisualGraphLayoutParams(new TextDocumentIdentifier(helper.MainUri), stale, Options: null),
                default);

            result.Status.Should().Be(VisualGraphLayoutStatus.GraphChanged);
            result.Patches.Should().BeEmpty();
        }

        [TestMethod]
        public async Task VisualGraphLayout_WhenEngineReturnsPositionsForSomeNodes_ReturnsOkAndLeavesOthersUnpositioned()
        {
            // The engine yields a position for only one node; the handler must report `ok` and emit a
            // setNodeLayout only for that node, so unpositioned nodes keep their existing client positions.
            // This pins the partial-layout contract so it is not silently changed.
            var partialEngine = new PartialLayoutEngine(positionedNodeId: "res1");

            using var helper = await StartServerAndOpenAsync(
                services => services.AddSingleton<IVisualGraphLayoutEngine>(partialEngine));
            var client = helper.Helper.Client;

            var initial = await client.SendRequest(
                new VisualGraphUpdateParams(new TextDocumentIdentifier(helper.MainUri), Current: null),
                default);

            var renderedNodes = initial.Patches.OfType<GraphPatch.AddNode>()
                .Select(patch => Rendered(patch.Node, 180, 72))
                .ToList();
            var renderedEdges = initial.Patches.OfType<GraphPatch.AddEdge>()
                .Select(patch => new RenderedGraphEdge(patch.Edge.Id, patch.Edge.SourceId, patch.Edge.TargetId))
                .ToList();

            var result = await client.SendRequest(
                new VisualGraphLayoutParams(
                    new TextDocumentIdentifier(helper.MainUri),
                    new RenderedGraph(renderedNodes, renderedEdges),
                    Options: null),
                default);

            result.Status.Should().Be(VisualGraphLayoutStatus.Ok);
            result.Patches.OfType<GraphPatch.SetNodeLayout>().Select(patch => patch.NodeId)
                .Should().BeEquivalentTo("res1");
        }

        private async Task<TestServer> StartServerAndOpenAsync(Action<IServiceCollection>? configureServices = null)
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
                services =>
                {
                    services.WithNamespaceProvider(BuiltInTestTypes.Create()).WithFileExplorer(fileSet.FileExplorer);
                    configureServices?.Invoke(services);
                });

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

        // Echoes a server-built node back as the client would: same identity and metadata, plus a measured size.
        private static RenderedGraphNode Rendered(GraphNode node, double width, double height) =>
            new(
                Id: node.Id,
                Kind: node.Kind,
                ParentId: node.ParentId,
                Type: node.Type,
                IsCollection: node.IsCollection,
                HasChildren: node.HasChildren,
                HasError: node.HasError,
                Width: width,
                Height: height);

        /// <summary>
        /// A layout engine that positions only a single node, used to pin the handler's partial-layout
        /// contract: nodes the engine does not position must keep their existing client positions.
        /// </summary>
        private sealed class PartialLayoutEngine : IVisualGraphLayoutEngine
        {
            private readonly string positionedNodeId;

            public PartialLayoutEngine(string positionedNodeId)
            {
                this.positionedNodeId = positionedNodeId;
            }

            public VisualGraphLayout Layout(
                CanonicalGraph graph,
                IReadOnlyDictionary<string, NodeSize> nodeSizes,
                VisualGraphLayoutOptions options,
                CancellationToken cancellationToken) =>
                graph.Nodes.Any(node => node.Id == this.positionedNodeId)
                    ? new VisualGraphLayout(new Dictionary<string, NodeLayout> { [this.positionedNodeId] = new NodeLayout(0, 0) }, new GraphBounds(0, 0))
                    : new VisualGraphLayout(new Dictionary<string, NodeLayout>(), null);
        }
    }
}
