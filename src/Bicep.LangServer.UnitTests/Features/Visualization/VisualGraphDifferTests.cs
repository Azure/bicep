// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphDifferTests
{
    [TestMethod]
    public void Diff_AgainstNullCurrent_AddsEveryNodeAndEdgeParentsFirst()
    {
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("mod1", GraphNodeKind.Module, parentId: null),
                Node("mod1::res1", GraphNodeKind.Resource, parentId: "mod1"),
                Node("res0", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("res0->mod1", "res0", "mod1")],
            ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current: null, target);

        // Parents are added before their children.
        patches.OfType<GraphPatch.AddNode>().Select(patch => patch.Node.Id)
            .Should().ContainInOrder("mod1", "mod1::res1")
            .And.Contain("res0");
        patches.OfType<GraphPatch.AddEdge>().Select(patch => patch.Edge.Id).Should().Equal("res0->mod1");
        patches.OfType<GraphPatch.RemoveNode>().Should().BeEmpty();
        patches.OfType<GraphPatch.RemoveEdge>().Should().BeEmpty();
        patches.OfType<GraphPatch.UpdateNode>().Should().BeEmpty();
        patches.OfType<GraphPatch.SetErrorCount>().Single().ErrorCount.Should().Be(0);
    }

    [TestMethod]
    public void Diff_WhenNodesAbsentFromTarget_RemovesDeepestFirst()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("mod1", GraphNodeKind.Module, parentId: null),
                Rendered("mod1::sub", GraphNodeKind.Module, parentId: "mod1"),
                Rendered("mod1::sub::res", GraphNodeKind.Resource, parentId: "mod1::sub"),
            ],
            Edges: []);
        var target = new CanonicalGraph([], [], ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        // Children removed before their parents.
        patches.OfType<GraphPatch.RemoveNode>().Select(patch => patch.NodeId)
            .Should().Equal("mod1::sub::res", "mod1::sub", "mod1");
    }

    [TestMethod]
    public void Diff_ForReparentedNode_EmitsRemoveThenAdd()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("a", GraphNodeKind.Module, parentId: null),
                Rendered("res", GraphNodeKind.Resource, parentId: "a"),
            ],
            Edges: []);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Module, parentId: null),
                Node("res", GraphNodeKind.Resource, parentId: "b"),
            ],
            Edges: [],
            ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        patches.OfType<GraphPatch.RemoveNode>().Select(patch => patch.NodeId).Should().Contain("res");
        patches.OfType<GraphPatch.AddNode>().Select(patch => patch.Node.Id).Should().Contain("res");
        patches.OfType<GraphPatch.UpdateNode>().Select(patch => patch.NodeId).Should().NotContain("res");
    }

    [TestMethod]
    public void Diff_ForKindChange_EmitsRemoveThenAdd()
    {
        var current = new RenderedGraph(
            Nodes: [Rendered("x", GraphNodeKind.Resource, parentId: null)],
            Edges: []);
        var target = new CanonicalGraph(
            Nodes: [Node("x", GraphNodeKind.Module, parentId: null)],
            Edges: [],
            ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        patches.OfType<GraphPatch.RemoveNode>().Select(patch => patch.NodeId).Should().Equal("x");
        patches.OfType<GraphPatch.AddNode>().Select(patch => patch.Node.Id).Should().Equal("x");
        patches.OfType<GraphPatch.UpdateNode>().Should().BeEmpty();
    }

    [TestMethod]
    public void Diff_ForSurvivingNodes_RefreshesMetadata()
    {
        var current = new RenderedGraph(
            Nodes: [Rendered("res", GraphNodeKind.Resource, parentId: null)],
            Edges: []);
        var target = new CanonicalGraph(
            Nodes: [Node("res", GraphNodeKind.Resource, parentId: null, type: "Microsoft.Storage/storageAccounts", hasError: true)],
            Edges: [],
            ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        patches.OfType<GraphPatch.RemoveNode>().Should().BeEmpty();
        patches.OfType<GraphPatch.AddNode>().Should().BeEmpty();
        var update = patches.OfType<GraphPatch.UpdateNode>().Single();
        update.NodeId.Should().Be("res");
        update.Changes.Type.Should().Be("Microsoft.Storage/storageAccounts");
        update.Changes.HasError.Should().Be(true);
        // Unchanged fields are omitted so the client applies the minimal delta.
        update.Changes.IsCollection.Should().BeNull();
        update.Changes.HasChildren.Should().BeNull();
    }

    [TestMethod]
    public void Diff_ForSurvivingNodes_WhenMetadataUnchanged_EmitsNoUpdate()
    {
        // A whitespace-only edit re-builds an identical canonical graph. Because source ranges are no longer
        // part of node metadata, the differ must emit zero patches (O(0) chatter) rather than one update per node.
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("res", GraphNodeKind.Resource, parentId: null),
                Rendered("mod", GraphNodeKind.Module, parentId: null),
            ],
            Edges: []);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("res", GraphNodeKind.Resource, parentId: null),
                Node("mod", GraphNodeKind.Module, parentId: null),
            ],
            Edges: [],
            ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        // Only the error-count refresh remains; no per-node patches.
        patches.OfType<GraphPatch.UpdateNode>().Should().BeEmpty();
        patches.OfType<GraphPatch.AddNode>().Should().BeEmpty();
        patches.OfType<GraphPatch.RemoveNode>().Should().BeEmpty();
        patches.OfType<GraphPatch.SetNodeLayout>().Should().BeEmpty();
    }

    [TestMethod]
    public void Diff_WhenEdgesChange_AddsAndRemovesEdgesById()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("a", GraphNodeKind.Resource, parentId: null),
                Rendered("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [RenderedEdge("a->b", "a", "b")]);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("b->a", "b", "a")],
            ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        patches.OfType<GraphPatch.RemoveEdge>().Select(patch => patch.EdgeId).Should().Equal("a->b");
        patches.OfType<GraphPatch.AddEdge>().Select(patch => patch.Edge.Id).Should().Equal("b->a");
    }

    [TestMethod]
    public void Diff_WhenRemovingNodes_RemovesEdgesFirst()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("a", GraphNodeKind.Resource, parentId: null),
                Rendered("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [RenderedEdge("a->b", "a", "b")]);
        var target = new CanonicalGraph([], [], ErrorCount: 0);

        var patches = VisualGraphDiffer.Diff(current, target);

        var firstRemoveEdge = patches.ToList().FindIndex(patch => patch is GraphPatch.RemoveEdge);
        var firstRemoveNode = patches.ToList().FindIndex(patch => patch is GraphPatch.RemoveNode);
        firstRemoveEdge.Should().BeLessThan(firstRemoveNode);
    }

    [TestMethod]
    public void Diff_ForAnyGraph_EmitsSetErrorCountLast()
    {
        var target = new CanonicalGraph(
            Nodes: [Node("res", GraphNodeKind.Resource, parentId: null)],
            Edges: [],
            ErrorCount: 4);

        var patches = VisualGraphDiffer.Diff(current: null, target);

        patches.Last().Should().BeOfType<GraphPatch.SetErrorCount>()
            .Which.ErrorCount.Should().Be(4);
    }

    [TestMethod]
    public void HasTopologyChange_WithNullCurrent_ReturnsTrue()
    {
        var target = new CanonicalGraph([Node("a", GraphNodeKind.Resource, parentId: null)], [], ErrorCount: 0);

        VisualGraphDiffer.HasTopologyChange(current: null, target).Should().BeTrue();
    }

    [TestMethod]
    public void HasTopologyChange_WithIdenticalTopology_ReturnsFalse()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("a", GraphNodeKind.Resource, parentId: null),
                Rendered("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [RenderedEdge("a->b", "a", "b")]);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("a->b", "a", "b")],
            ErrorCount: 0);

        VisualGraphDiffer.HasTopologyChange(current, target).Should().BeFalse();
    }

    [TestMethod]
    public void HasTopologyChange_WithMetadataOnlyChange_ReturnsFalse()
    {
        var current = new RenderedGraph(
            Nodes: [Rendered("a", GraphNodeKind.Resource, parentId: null)],
            Edges: []);

        // Same id, kind, and parent, but a different type and error state: a metadata-only change.
        var target = new CanonicalGraph(
            Nodes: [Node("a", GraphNodeKind.Resource, parentId: null, type: "Microsoft.Storage/storageAccounts", hasError: true)],
            Edges: [],
            ErrorCount: 1);

        VisualGraphDiffer.HasTopologyChange(current, target).Should().BeFalse();
    }

    [TestMethod]
    public void HasTopologyChange_WithAddedNode_ReturnsTrue()
    {
        var current = new RenderedGraph(Nodes: [Rendered("a", GraphNodeKind.Resource, parentId: null)], Edges: []);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [],
            ErrorCount: 0);

        VisualGraphDiffer.HasTopologyChange(current, target).Should().BeTrue();
    }

    [TestMethod]
    public void HasTopologyChange_WithReparentedNode_ReturnsTrue()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("m", GraphNodeKind.Module, parentId: null),
                Rendered("res", GraphNodeKind.Resource, parentId: "m"),
            ],
            Edges: []);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("m", GraphNodeKind.Module, parentId: null),
                Node("res", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [],
            ErrorCount: 0);

        VisualGraphDiffer.HasTopologyChange(current, target).Should().BeTrue();
    }

    [TestMethod]
    public void HasTopologyChange_WithChangedEdge_ReturnsTrue()
    {
        var current = new RenderedGraph(
            Nodes:
            [
                Rendered("a", GraphNodeKind.Resource, parentId: null),
                Rendered("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: []);
        var target = new CanonicalGraph(
            Nodes:
            [
                Node("a", GraphNodeKind.Resource, parentId: null),
                Node("b", GraphNodeKind.Resource, parentId: null),
            ],
            Edges: [Edge("a->b", "a", "b")],
            ErrorCount: 0);

        VisualGraphDiffer.HasTopologyChange(current, target).Should().BeTrue();
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

    private static RenderedGraphNode Rendered(
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
            IsCollection: false,
            HasChildren: false,
            HasError: hasError,
            Width: 100,
            Height: 50);

    private static RenderedGraphEdge RenderedEdge(string id, string sourceId, string targetId) => new(id, sourceId, targetId);
}
