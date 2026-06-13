// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphDifferTests
{
    [TestMethod]
    public void Diff_against_null_current_adds_every_node_and_edge_parents_first()
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
    public void Diff_removes_nodes_absent_from_target_deepest_first()
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
    public void Diff_treats_reparented_node_as_remove_then_add()
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
    public void Diff_treats_kind_change_as_remove_then_add()
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
    public void Diff_refreshes_surviving_nodes_with_metadata()
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
    }

    [TestMethod]
    public void Diff_adds_and_removes_edges_by_id()
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
    public void Diff_removes_edges_before_removing_nodes()
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
    public void Diff_always_emits_set_error_count_last()
    {
        var target = new CanonicalGraph(
            Nodes: [Node("res", GraphNodeKind.Resource, parentId: null)],
            Edges: [],
            ErrorCount: 4);

        var patches = VisualGraphDiffer.Diff(current: null, target);

        patches.Last().Should().BeOfType<GraphPatch.SetErrorCount>()
            .Which.ErrorCount.Should().Be(4);
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
            HasError: hasError,
            FilePath: "/main.bicep",
            Range: new Range(0, 0, 0, 0));

    private static GraphEdge Edge(string id, string sourceId, string targetId) => new(id, sourceId, targetId);

    private static RenderedGraphNode Rendered(string id, string kind, string? parentId) =>
        new(Id: id, Kind: kind, ParentId: parentId, Width: 100, Height: 50);

    private static RenderedGraphEdge RenderedEdge(string id, string sourceId, string targetId) => new(id, sourceId, targetId);
}
