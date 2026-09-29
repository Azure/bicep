// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";
import type { GraphEdge, GraphNode, GraphPatch, NodeLayout, RenderedGraph, RenderedGraphNode } from "../api";
import type { ClientGraph } from "../graph-model";

import { describe, expect, it } from "vitest";
import {
  applyGraphPatch,
  buildRenderedGraph,
  clientGraphsRenderEqually,
  createClientGraph,
  renderedGraphsEqual,
} from "../graph-model";

function node(overrides: Partial<GraphNode> = {}): GraphNode {
  return {
    id: "n",
    kind: "resource",
    parentId: null,
    type: "Microsoft.Storage/storageAccounts",
    symbolName: "n",
    isCollection: false,
    hasChildren: false,
    hasError: false,
    ...overrides,
  };
}

function apply(patches: GraphPatch[]) {
  const graph = createClientGraph();
  const nodeLayouts = new Map<string, NodeLayout>();

  for (const patch of patches) {
    applyGraphPatch(graph, nodeLayouts, patch);
  }

  return { graph, nodeLayouts };
}

const box = (width: number, height: number): Box => ({ min: { x: 0, y: 0 }, max: { x: width, y: height } });

describe("applyGraphPatch", () => {
  it("adds, updates and removes nodes", () => {
    const { graph } = apply([
      { op: "addNode", node: node({ id: "a" }) },
      { op: "addNode", node: node({ id: "b" }) },
      { op: "updateNode", nodeId: "a", changes: { hasError: true } },
      { op: "removeNode", nodeId: "b" },
    ]);

    expect([...graph.nodes.keys()]).toEqual(["a"]);
    expect(graph.nodes.get("a")?.hasError).toBe(true);
  });

  it("leaves fields the server omitted untouched", () => {
    // The server sends only changed metadata, and models "unchanged" as undefined or null.
    const { graph } = apply([
      { op: "addNode", node: node({ id: "a", type: "Microsoft.Web/sites", isCollection: true }) },
      { op: "updateNode", nodeId: "a", changes: { hasError: true, type: undefined, isCollection: null } },
    ]);

    const updated = graph.nodes.get("a");
    expect(updated?.type).toBe("Microsoft.Web/sites");
    expect(updated?.isCollection).toBe(true);
    expect(updated?.hasError).toBe(true);
  });

  it("ignores an update for a node it does not hold", () => {
    const { graph } = apply([{ op: "updateNode", nodeId: "ghost", changes: { hasError: true } }]);

    expect(graph.nodes.size).toBe(0);
  });

  it("collects setNodeLayout separately from the graph", () => {
    const { graph, nodeLayouts } = apply([
      { op: "addNode", node: node({ id: "a" }) },
      { op: "setNodeLayout", nodeId: "a", layout: { x: 10, y: 20 } },
    ]);

    expect(nodeLayouts.get("a")).toEqual({ x: 10, y: 20 });
    expect(graph.nodes.get("a")).not.toHaveProperty("x");
  });

  it("clears everything on clearGraph", () => {
    const { graph } = apply([
      { op: "addNode", node: node({ id: "a" }) },
      { op: "addEdge", edge: { id: "e", sourceId: "a", targetId: "a" } },
      { op: "setErrorCount", errorCount: 3 },
      { op: "clearGraph" },
    ]);

    expect(graph.nodes.size).toBe(0);
    expect(graph.edges.size).toBe(0);
    expect(graph.errorCount).toBe(0);
  });

  it("tracks the error count", () => {
    const { graph } = apply([{ op: "setErrorCount", errorCount: 2 }]);

    expect(graph.errorCount).toBe(2);
  });
});

describe("buildRenderedGraph", () => {
  it("reports measured sizes, and zero for a node not yet measured", () => {
    const { graph } = apply([
      { op: "addNode", node: node({ id: "measured" }) },
      { op: "addNode", node: node({ id: "unmeasured" }) },
    ]);

    const rendered = buildRenderedGraph(graph, new Map([["measured", box(220, 80)]]));

    expect(rendered.nodes).toEqual([
      expect.objectContaining({ id: "measured", width: 220, height: 80 }),
      expect.objectContaining({ id: "unmeasured", width: 0, height: 0 }),
    ]);
  });

  it("carries edges through by id", () => {
    const { graph } = apply([
      { op: "addNode", node: node({ id: "a" }) },
      { op: "addEdge", edge: { id: "a->b", sourceId: "a", targetId: "b" } },
    ]);

    expect(buildRenderedGraph(graph, new Map()).edges).toEqual([{ id: "a->b", sourceId: "a", targetId: "b" }]);
  });
});

function renderedNode(overrides: Partial<RenderedGraphNode> = {}): RenderedGraphNode {
  return {
    id: "a",
    kind: "resource",
    parentId: null,
    type: "Microsoft.Storage/storageAccounts",
    isCollection: false,
    hasChildren: false,
    hasError: false,
    width: 220,
    height: 80,
    ...overrides,
  };
}

describe("renderedGraphsEqual", () => {
  const base: RenderedGraph = {
    nodes: [renderedNode({ id: "a" }), renderedNode({ id: "b" })],
    edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
  };

  it("returns false when the previous input is null", () => {
    expect(renderedGraphsEqual(null, base)).toBe(false);
  });

  it("returns true for the same graph regardless of node and edge order", () => {
    const reordered: RenderedGraph = {
      nodes: [renderedNode({ id: "b" }), renderedNode({ id: "a" })],
      edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
    };
    expect(renderedGraphsEqual(base, reordered)).toBe(true);
  });

  it("returns false when a node count differs", () => {
    const extra: RenderedGraph = { nodes: [...base.nodes, renderedNode({ id: "c" })], edges: base.edges };
    expect(renderedGraphsEqual(base, extra)).toBe(false);
  });

  it("returns false when a measured size differs", () => {
    const widened: RenderedGraph = {
      nodes: [renderedNode({ id: "a", width: 221 }), renderedNode({ id: "b" })],
      edges: base.edges,
    };
    expect(renderedGraphsEqual(base, widened)).toBe(false);
  });

  it("returns false when containment differs", () => {
    const reparented: RenderedGraph = {
      nodes: [renderedNode({ id: "a", parentId: "b" }), renderedNode({ id: "b" })],
      edges: base.edges,
    };
    expect(renderedGraphsEqual(base, reparented)).toBe(false);
  });

  it("returns false when the edge set differs", () => {
    const rewired: RenderedGraph = {
      nodes: base.nodes,
      edges: [{ id: "b>a", sourceId: "b", targetId: "a" }],
    };
    expect(renderedGraphsEqual(base, rewired)).toBe(false);
  });
});

function makeClientGraph(nodes: GraphNode[], edges: GraphEdge[] = [], errorCount = 0): ClientGraph {
  return {
    nodes: new Map(nodes.map((graphNode) => [graphNode.id, graphNode])),
    edges: new Map(edges.map((edge) => [edge.id, edge])),
    errorCount,
  };
}

describe("clientGraphsRenderEqually", () => {
  it("reports equal for a graph rebuilt from identical parts", () => {
    expect(clientGraphsRenderEqually(makeClientGraph([node()]), makeClientGraph([node()]))).toBe(true);
  });

  it("ignores node and edge ordering", () => {
    const a = node({ id: "a" });
    const b = node({ id: "b" });

    expect(clientGraphsRenderEqually(makeClientGraph([a, b]), makeClientGraph([b, a]))).toBe(true);
  });

  it("ignores fields the canvas never reads", () => {
    const before = makeClientGraph([node({ symbolName: "before" })]);
    const after = makeClientGraph([node({ symbolName: "after" })]);

    expect(clientGraphsRenderEqually(before, after)).toBe(true);
  });

  it("treats two nulls as equal but a null on one side as a change", () => {
    expect(clientGraphsRenderEqually(null, null)).toBe(true);
    expect(clientGraphsRenderEqually(null, makeClientGraph([node()]))).toBe(false);
    expect(clientGraphsRenderEqually(makeClientGraph([node()]), null)).toBe(false);
  });

  it.each([
    ["id", { id: "other" }],
    ["type", { type: "Microsoft.Web/sites" }],
    ["isCollection", { isCollection: true }],
    ["hasChildren", { hasChildren: true }],
    ["hasError", { hasError: true }],
  ])("reports a change when %s differs", (_field, overrides) => {
    const before = makeClientGraph([node()]);
    const after = makeClientGraph([node(overrides as Partial<GraphNode>)]);

    expect(clientGraphsRenderEqually(before, after)).toBe(false);
  });

  it("reports a change when the error count differs", () => {
    expect(clientGraphsRenderEqually(makeClientGraph([node()], [], 0), makeClientGraph([node()], [], 1))).toBe(false);
  });

  it("reports a change when an edge is added or retargeted", () => {
    const nodes = [node({ id: "a" }), node({ id: "b" })];
    const none = makeClientGraph(nodes);
    const one = makeClientGraph(nodes, [{ id: "e", sourceId: "a", targetId: "b" }]);
    const other = makeClientGraph(nodes, [{ id: "e", sourceId: "b", targetId: "a" }]);

    expect(clientGraphsRenderEqually(none, one)).toBe(false);
    expect(clientGraphsRenderEqually(one, other)).toBe(false);
  });
});
