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
    public class VisualGraphTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task VisualGraph_ReturnsWholeGraphFromLiveCompilation()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var result = await client.SendRequest(new VisualGraphParams(new TextDocumentIdentifier(helper.MainUri)), default);

            var graph = result.Graph!;
            graph.Nodes.Select(node => node.Id).Should().BeEquivalentTo("res1", "res2", "mod1", "mod1::res3");

            var res2 = graph.Nodes.Single(node => node.Id == "res2");
            res2.Kind.Should().Be(GraphNodeKind.Resource);
            res2.Type.Should().Be("Test.Rp/readWriteTests");
            res2.ParentId.Should().BeNull();

            var mod1 = graph.Nodes.Single(node => node.Id == "mod1");
            mod1.Kind.Should().Be(GraphNodeKind.Module);
            mod1.Type.Should().Be("<module>");
            mod1.HasChildren.Should().BeTrue();

            var res3 = graph.Nodes.Single(node => node.Id == "mod1::res3");
            res3.Kind.Should().Be(GraphNodeKind.Resource);
            res3.ParentId.Should().Be("mod1");

            graph.Edges.Select(edge => (edge.SourceId, edge.TargetId)).Should().Contain(("res2", "mod1"));
            result.ErrorCount.Should().Be(0);
            result.TargetScope.Should().Be("resourceGroup");
        }

        [TestMethod]
        public async Task VisualGraphLayout_ForMeasuredMatchingGraph_ReturnsPositionsAndBounds()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new VisualGraphLayoutParams(new TextDocumentIdentifier(helper.MainUri), await MeasureGraph(helper), Options: null),
                default);

            result.Status.Should().Be(VisualGraphLayoutStatus.Ok);
            result.Positions.Select(position => position.NodeId).Should().BeEquivalentTo("res1", "res2", "mod1", "mod1::res3");
            result.Positions.Should().OnlyContain(position => double.IsFinite(position.X) && double.IsFinite(position.Y));

            // The engine returns the whole-graph bounds alongside the positions so the webview can fit the
            // viewport without re-deriving module box extents.
            result.Bounds.Should().Match<GraphBounds>(bounds => bounds.Width > 0 && bounds.Height > 0);
        }

        [TestMethod]
        public async Task VisualGraphLayout_ForStaleMeasuredGraph_ReturnsGraphChanged()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;
            var stale = new MeasuredGraph(
                Nodes: [new MeasuredGraphNode("missing", GraphNodeKind.Resource, ParentId: null, Width: 180, Height: 72)],
                Edges: []);

            var result = await client.SendRequest(
                new VisualGraphLayoutParams(new TextDocumentIdentifier(helper.MainUri), stale, Options: null),
                default);

            result.Status.Should().Be(VisualGraphLayoutStatus.GraphChanged);
            result.Positions.Should().BeEmpty();
            result.Bounds.Should().BeNull();
        }

        [TestMethod]
        public async Task VisualGraphLayout_WhenEngineReturnsPositionsForSomeNodes_ReturnsOkAndLeavesOthersUnpositioned()
        {
            // The engine yields a position for only one node; the handler must report `ok` and return a position
            // only for that node, so unpositioned nodes keep their existing client positions. This pins the
            // partial-layout contract so it is not silently changed.
            var partialEngine = new PartialLayoutEngine(positionedNodeId: "res1");

            using var helper = await StartServerAndOpenAsync(
                services => services.AddSingleton<IVisualGraphLayoutEngine>(partialEngine));
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new VisualGraphLayoutParams(new TextDocumentIdentifier(helper.MainUri), await MeasureGraph(helper), Options: null),
                default);

            result.Status.Should().Be(VisualGraphLayoutStatus.Ok);
            result.Positions.Select(position => position.NodeId).Should().BeEquivalentTo("res1");
        }

        /// <summary>Fetches the server's graph and measures it as the webview would.</summary>
        private static async Task<MeasuredGraph> MeasureGraph(TestServer helper)
        {
            var result = await helper.Helper.Client.SendRequest(new VisualGraphParams(new TextDocumentIdentifier(helper.MainUri)), default);
            var graph = result.Graph!;

            return new MeasuredGraph(
                [.. graph.Nodes.Select(node => new MeasuredGraphNode(node.Id, node.Kind, node.ParentId, Width: 180, Height: 72))],
                graph.Edges);
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
