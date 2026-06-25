// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class MsaglVisualGraphLayoutEngineTests
{
    [TestMethod]
    public void Layout_ForEmptyGraph_ReturnsNoPositions()
    {
        var engine = CreateEngine();

        var layout = engine.Layout(new CanonicalGraph([], [], ErrorCount: 0), NoSizes, DefaultOptions, CancellationToken.None);

        layout.Positions.Should().BeEmpty();
        layout.Bounds.Should().BeNull();
    }

    [TestMethod]
    public void Layout_WhenCancelled_ThrowsOperationCanceled()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b")],
            Edges: [Edge("a", "b")],
            ErrorCount: 0);
        var cancelled = new CancellationToken(canceled: true);

        var act = () => engine.Layout(graph, NoSizes, DefaultOptions, cancelled);

        act.Should().Throw<OperationCanceledException>();
    }

    [TestMethod]
    public void Layout_ForConnectedGraph_PositionsEveryNode()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b"), Node("c")],
            Edges: [Edge("a", "b"), Edge("b", "c")],
            ErrorCount: 0);

        var layout = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;

        layout.Keys.Should().BeEquivalentTo("a", "b", "c");
        layout.Values.Should().OnlyContain(position => IsFinite(position));
    }

    [TestMethod]
    public void Layout_ForGraph_PlacesTopLeftCornerAtOrigin()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b"), Node("c")],
            Edges: [Edge("a", "b"), Edge("a", "c")],
            ErrorCount: 0);

        var layout = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;

        // The graph's left-most and top-most edges sit at the origin; pan/zoom and centering stay on the client.
        layout.Values.Min(position => position.X).Should().BeApproximately(0, Tolerance);
        layout.Values.Min(position => position.Y).Should().BeApproximately(0, Tolerance);
    }

    [TestMethod]
    public void Layout_ForGraph_ReturnsBoundsEnclosingEveryNode()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b"), Node("c")],
            Edges: [Edge("a", "b"), Edge("a", "c")],
            ErrorCount: 0);

        var layout = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None);

        // The bounds wrap the whole graph from the origin, so every node's top-left position falls inside.
        layout.Bounds.Should().NotBeNull();
        layout.Bounds!.Width.Should().BeGreaterThan(0);
        layout.Bounds.Height.Should().BeGreaterThan(0);
        layout.Positions.Values.Should().OnlyContain(position =>
            position.X >= -Tolerance && position.X <= layout.Bounds.Width + Tolerance &&
            position.Y >= -Tolerance && position.Y <= layout.Bounds.Height + Tolerance);
    }

    [TestMethod]
    public void Layout_ForDependencyEdge_PlacesEndpointsOnDifferentLayers()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b")],
            Edges: [Edge("a", "b")],
            ErrorCount: 0);

        var layout = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;

        // A layered top-to-bottom layout separates the two endpoints vertically.
        layout["a"].Y.Should().NotBe(layout["b"].Y);
    }

    [TestMethod]
    public void Layout_ForSameInput_ProducesIdenticalPositions()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b"), Node("c"), Node("d")],
            Edges: [Edge("a", "b"), Edge("a", "c"), Edge("b", "d"), Edge("c", "d")],
            ErrorCount: 0);

        var first = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;
        var second = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;

        second.Should().BeEquivalentTo(first);
    }

    [TestMethod]
    public void Layout_ForNestedModule_PositionsModuleAndChildren()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes:
            [
                Node("root"),
                Node("mod", GraphNodeKind.Module, hasChildren: true),
                Node("mod::child1", parentId: "mod"),
                Node("mod::child2", parentId: "mod"),
            ],
            Edges: [Edge("root", "mod"), Edge("mod::child1", "mod::child2")],
            ErrorCount: 0);

        var layout = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;

        layout.Keys.Should().BeEquivalentTo("root", "mod", "mod::child1", "mod::child2");
        layout.Values.Should().OnlyContain(position => IsFinite(position));
    }

    [TestMethod]
    public void Layout_WithClientMeasuredSizes_PositionsEveryNode()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes: [Node("a"), Node("b")],
            Edges: [Edge("a", "b")],
            ErrorCount: 0);
        var sizes = Sizes(("a", 400, 300), ("b", 400, 300));

        var layout = engine.Layout(graph, sizes, DefaultOptions, CancellationToken.None).Positions;

        // Larger nodes simply produce a valid, fully-populated layout; sizing is an input, not an output.
        layout.Keys.Should().BeEquivalentTo("a", "b");
        layout.Values.Should().OnlyContain(position => IsFinite(position));
    }

    [TestMethod]
    public void Layout_WithCancelledToken_ThrowsOperationCancelled()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(Nodes: [Node("a")], Edges: [], ErrorCount: 0);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var act = () => engine.Layout(graph, NoSizes, DefaultOptions, cancellation.Token);

        act.Should().Throw<OperationCanceledException>();
    }

    [TestMethod]
    public void Layout_WithCustomModulePadding_OffsetsChildrenByPadding()
    {
        var engine = CreateEngine();
        var graph = new CanonicalGraph(
            Nodes:
            [
                Node("mod", GraphNodeKind.Module, hasChildren: true),
                Node("mod::child", parentId: "mod"),
            ],
            Edges: [],
            ErrorCount: 0);
        var options = new VisualGraphLayoutOptions(
            defaultNodeSize: new NodeSize(100, 80),
            nodeSeparation: 10,
            layerSeparation: 20,
            modulePadding: new ModulePadding(top: 17, right: 31, bottom: 23, left: 29));

        var layout = engine.Layout(graph, NoSizes, options, CancellationToken.None);

        layout.Positions["mod"].Should().Be(new NodeLayout(0, 0));
        layout.Positions["mod::child"].Should().Be(new NodeLayout(29, 17));

        // The module box wraps its single default-sized child (100x80) plus padding on every side, and is the
        // only top-level node, so the whole-graph bounds equal the module box size.
        layout.Bounds.Should().Be(new GraphBounds(100 + 29 + 31, 80 + 17 + 23));
    }

    [TestMethod]
    public void Layout_ForSimpleGraph_CompletesWithinSmokeBudget()
    {
        var engine = CreateEngine();
        var graph = BuildRepresentativeGraph(moduleCount: 4, resourcesPerModule: 4, topLevelResourceCount: 8);
        var stopwatch = Stopwatch.StartNew();

        var layout = engine.Layout(graph, NoSizes, DefaultOptions, CancellationToken.None).Positions;

        stopwatch.Stop();
        layout.Keys.Should().BeEquivalentTo(graph.Nodes.Select(node => node.Id));
        layout.Values.Should().OnlyContain(position => IsFinite(position));
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromMilliseconds(500));
    }

    private const double Tolerance = 1e-6;

    private static readonly VisualGraphLayoutOptions DefaultOptions = VisualGraphLayoutOptions.Default;

    private static readonly IReadOnlyDictionary<string, NodeSize> NoSizes =
        new Dictionary<string, NodeSize>(StringComparer.Ordinal);

    private static MsaglVisualGraphLayoutEngine CreateEngine() =>
        new(NullLogger<MsaglVisualGraphLayoutEngine>.Instance);

    private static bool IsFinite(NodeLayout position) =>
        double.IsFinite(position.X) && double.IsFinite(position.Y);

    private static IReadOnlyDictionary<string, NodeSize> Sizes(params (string Id, double Width, double Height)[] entries) =>
        entries.ToDictionary(entry => entry.Id, entry => new NodeSize(entry.Width, entry.Height), StringComparer.Ordinal);

    private static CanonicalGraph BuildRepresentativeGraph(
        int moduleCount,
        int resourcesPerModule,
        int topLevelResourceCount)
    {
        var nodes = new List<GraphNode>();
        var edges = new List<GraphEdge>();

        for (var index = 0; index < topLevelResourceCount; index++)
        {
            nodes.Add(Node($"top{index}"));

            if (index > 0)
            {
                edges.Add(Edge($"top{index - 1}", $"top{index}"));
            }
        }

        for (var moduleIndex = 0; moduleIndex < moduleCount; moduleIndex++)
        {
            var moduleId = $"mod{moduleIndex}";
            nodes.Add(Node(moduleId, GraphNodeKind.Module, hasChildren: true));

            if (topLevelResourceCount > 0)
            {
                edges.Add(Edge($"top{moduleIndex % topLevelResourceCount}", moduleId));
            }

            for (var resourceIndex = 0; resourceIndex < resourcesPerModule; resourceIndex++)
            {
                var resourceId = $"{moduleId}::res{resourceIndex}";
                nodes.Add(Node(resourceId, parentId: moduleId));

                if (resourceIndex > 0)
                {
                    edges.Add(Edge($"{moduleId}::res{resourceIndex - 1}", resourceId));
                }
            }
        }

        return new CanonicalGraph(nodes, edges, ErrorCount: 0);
    }

    private static GraphNode Node(
        string id,
        string kind = GraphNodeKind.Resource,
        string? parentId = null,
        bool hasChildren = false) =>
        new(
            Id: id,
            Kind: kind,
            ParentId: parentId,
            Type: "Test.Rp/tests",
            SymbolName: id,
            IsCollection: false,
            HasChildren: hasChildren,
            HasError: false);

    private static GraphEdge Edge(string sourceId, string targetId) =>
        new(Id: $"{sourceId}->{targetId}", SourceId: sourceId, TargetId: targetId);
}
