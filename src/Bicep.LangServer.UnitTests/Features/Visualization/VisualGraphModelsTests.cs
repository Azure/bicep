// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphModelsTests
{
    private static readonly CanonicalGraph Graph = new(
        Nodes:
        [
            Node("a", GraphNodeKind.Resource, parentId: null),
            Node("m", GraphNodeKind.Module, parentId: null, hasChildren: true),
            Node("m::b", GraphNodeKind.Resource, parentId: "m"),
        ],
        Edges: [Edge("a->m", "a", "m")]);

    [TestMethod]
    public void CanonicalGraph_GetRevision_WithMetadataOnlyChangeOrReordering_IsUnchanged()
    {
        var changed = new CanonicalGraph(
            Nodes:
            [
                Node("m::b", GraphNodeKind.Resource, parentId: "m", type: "Microsoft.Storage/storageAccounts", hasError: true),
                Node("m", GraphNodeKind.Module, parentId: null, hasChildren: true, sourceStatus: GraphNodeSourceStatus.Unrestored),
                Node("a", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("a->m", "a", "m")]);

        changed.GetRevision().Should().Be(Graph.GetRevision());
    }

    [TestMethod]
    public void CanonicalGraph_GetRevision_WithTopologyChange_Changes()
    {
        var revision = Graph.GetRevision();

        (Graph with { Nodes = [.. Graph.Nodes, Node("c", GraphNodeKind.Resource, parentId: null)] }).GetRevision()
            .Should().NotBe(revision);
        (Graph with { Nodes = [Graph.Nodes[0], Graph.Nodes[1], Node("m::b", GraphNodeKind.Resource, parentId: null)] }).GetRevision()
            .Should().NotBe(revision);
        (Graph with { Edges = [Edge("a->m", "m", "a")] }).GetRevision()
            .Should().NotBe(revision);
    }

    [TestMethod]
    public void MeasuredGraph_ToLayoutGraph_WithWellFormedGraph_DerivesContainers()
    {
        var measured = new MeasuredGraph(
            Nodes:
            [
                Measured("a", GraphNodeKind.Resource, parentId: null),
                Measured("m", GraphNodeKind.Module, parentId: null),
                Measured("m::b", GraphNodeKind.Resource, parentId: "m"),
            ],
            Edges: [Edge("a->m", "a", "m")]);

        var layoutGraph = measured.ToLayoutGraph();

        layoutGraph.Should().NotBeNull();
        layoutGraph!.Nodes.Select(node => (node.Id, node.ParentId, node.HasChildren)).Should().Equal(
            ("a", null, false),
            ("m", null, true),
            ("m::b", "m", false));
        layoutGraph.Edges.Should().Equal(measured.Edges);
    }

    [TestMethod]
    public void MeasuredGraph_ToLayoutGraph_WithModuleWhoseChildrenAreHidden_LaysItOutAsALeaf()
    {
        // A collapsed module is rendered without its children, so it is laid out like a resource.
        var measured = new MeasuredGraph(
            Nodes: [Measured("m", GraphNodeKind.Module, parentId: null), Measured("a", GraphNodeKind.Resource, parentId: null)],
            Edges: [Edge("a->m", "a", "m")]);

        measured.ToLayoutGraph()!.Nodes.Should().OnlyContain(node => !node.HasChildren);
    }

    [TestMethod]
    public void MeasuredGraph_ToLayoutGraph_WithMalformedGraph_ReturnsNull()
    {
        var a = Measured("a", GraphNodeKind.Resource, parentId: null);
        var b = Measured("b", GraphNodeKind.Resource, parentId: null);

        new MeasuredGraph([a, a], []).ToLayoutGraph().Should().BeNull();
        new MeasuredGraph([a with { ParentId = "missing" }], []).ToLayoutGraph().Should().BeNull();
        new MeasuredGraph([a with { ParentId = "b" }, b with { ParentId = "a" }], []).ToLayoutGraph().Should().BeNull();
        new MeasuredGraph([a], [Edge("a->missing", "a", "missing")]).ToLayoutGraph().Should().BeNull();
        new MeasuredGraph([a, b], [Edge("edge", "a", "b"), Edge("edge", "b", "a")]).ToLayoutGraph().Should().BeNull();
    }

    private static GraphNode Node(
        string id,
        string kind,
        string? parentId,
        string type = "Test.Rp/tests",
        bool hasError = false,
        bool hasChildren = false,
        string? sourceStatus = null) =>
        new(
            Id: id,
            Kind: kind,
            ParentId: parentId,
            Type: type,
            SymbolName: id,
            IsCollection: false,
            HasChildren: hasChildren,
            HasError: hasError,
            SourceStatus: sourceStatus);

    private static GraphEdge Edge(string id, string sourceId, string targetId) => new(id, sourceId, targetId);

    private static MeasuredGraphNode Measured(string id, string kind, string? parentId) =>
        new(Id: id, Kind: kind, ParentId: parentId, Width: 100, Height: 50);
}
