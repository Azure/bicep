// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphTopologyTests
{
    [TestMethod]
    public void Matches_WithEmptyMeasuredGraph_ReturnsFalseForNonEmptyLiveGraph()
    {
        var live = new CanonicalGraph([Node("a", GraphNodeKind.Resource, parentId: null)], [], ErrorCount: 0);

        VisualGraphTopology.Matches(new MeasuredGraph([], []), live).Should().BeFalse();
    }

    [TestMethod]
    public void Matches_WithIdenticalTopology_ReturnsTrue()
    {
        var measured = new MeasuredGraph(
            Nodes:
            [
                Measured("a", GraphNodeKind.Resource, parentId: null),
                Measured("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("a->b", "a", "b")]);
        var live = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("a->b", "a", "b")],
            ErrorCount: 0);

        VisualGraphTopology.Matches(measured, live).Should().BeTrue();
    }

    [TestMethod]
    public void Matches_WithMetadataOnlyChange_ReturnsTrue()
    {
        var measured = new MeasuredGraph(Nodes: [Measured("a", GraphNodeKind.Resource, parentId: null)], Edges: []);

        // Same id, kind, and parent, but a different type and error state: a metadata-only change.
        var live = new CanonicalGraph(
            Nodes: [Node("a", GraphNodeKind.Resource, parentId: null, type: "Microsoft.Storage/storageAccounts", hasError: true)],
            Edges: [],
            ErrorCount: 1);

        VisualGraphTopology.Matches(measured, live).Should().BeTrue();
    }

    [TestMethod]
    public void Matches_WithAddedNode_ReturnsFalse()
    {
        var measured = new MeasuredGraph(Nodes: [Measured("a", GraphNodeKind.Resource, parentId: null)], Edges: []);
        var live = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [],
            ErrorCount: 0);

        VisualGraphTopology.Matches(measured, live).Should().BeFalse();
    }

    [TestMethod]
    public void Matches_WithReparentedNode_ReturnsFalse()
    {
        var measured = new MeasuredGraph(
            Nodes:
            [
                Measured("m", GraphNodeKind.Module, parentId: null),
                Measured("res", GraphNodeKind.Resource, parentId: "m"),
            ],
            Edges: []);
        var live = new CanonicalGraph(
            Nodes:
            [
                Node("m", GraphNodeKind.Module, parentId: null),
                Node("res", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [],
            ErrorCount: 0);

        VisualGraphTopology.Matches(measured, live).Should().BeFalse();
    }

    [TestMethod]
    public void Matches_WithChangedEdge_ReturnsFalse()
    {
        var measured = new MeasuredGraph(
            Nodes:
            [
                Measured("a", GraphNodeKind.Resource, parentId: null),
                Measured("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("b->a", "b", "a")]);
        var live = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("a->b", "a", "b")],
            ErrorCount: 0);

        VisualGraphTopology.Matches(measured, live).Should().BeFalse();
    }

    private static GraphNode Node(
        string id,
        string kind,
        string? parentId,
        string type = "Test.Rp/tests",
        bool hasError = false) =>
        new(
            Id: id,
            Kind: kind,
            ParentId: parentId,
            Type: type,
            SymbolName: id,
            IsCollection: false,
            HasChildren: false,
            HasError: hasError);

    private static GraphEdge Edge(string id, string sourceId, string targetId) => new(id, sourceId, targetId);

    private static MeasuredGraphNode Measured(string id, string kind, string? parentId) =>
        new(Id: id, Kind: kind, ParentId: parentId, Width: 100, Height: 50);
}