// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";
import type { GraphEdge, GraphNode, MeasuredGraph, MeasuredGraphNode } from "../api";
import type { ClientGraph } from "../graph-model";

import { describe, expect, it } from "vitest";
import {
  clientGraphsRenderEqually,
  EMPTY_CLIENT_GRAPH,
  indexGraph,
  measuredGraphsEqual,
  measureGraph,
  projectGraph,
  subtreeGraph,
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
    requiresRestore: false,
    hasError: false,
    ...overrides,
  };
}

const box = (width: number, height: number): Box => ({ min: { x: 0, y: 0 }, max: { x: width, y: height } });

describe("indexGraph", () => {
  it("indexes nodes and edges by id", () => {
    const graph = indexGraph({
      nodes: [node({ id: "a" }), node({ id: "b" })],
      edges: [{ id: "a->b", sourceId: "a", targetId: "b" }],
    });

    expect([...graph.nodes.keys()]).toEqual(["a", "b"]);
    expect(graph.edges.get("a->b")).toEqual({ id: "a->b", sourceId: "a", targetId: "b" });
  });
});

describe("projectGraph", () => {
  const moduleNode = node({
    id: "module",
    kind: "module",
    type: "<module>",
    hasChildren: true,
  });
  const childA = node({ id: "module::a", parentId: "module" });
  const childB = node({ id: "module::b", parentId: "module" });
  const outside = node({ id: "outside" });

  it("returns the source graph when no module is collapsed", () => {
    const graph = makeClientGraph([moduleNode, childA]);

    expect(projectGraph(graph, new Set())).toBe(graph);
    expect(projectGraph(graph, new Set(["missing"]))).toBe(graph);
  });

  it("keeps the module, hides descendants, and marks it collapsed", () => {
    const projected = projectGraph(makeClientGraph([moduleNode, childA, childB, outside]), new Set(["module"]));

    expect([...projected.nodes.keys()]).toEqual(["module", "outside"]);
    expect(projected.nodes.get("module")).toMatchObject({ hasChildren: false, isCollapsed: true });
  });

  it("reroutes boundary edges, removes internal edges, and deduplicates visible relationships", () => {
    const projected = projectGraph(
      makeClientGraph(
        [moduleNode, childA, childB, outside],
        [
          { id: "internal", sourceId: "module::a", targetId: "module::b" },
          { id: "first", sourceId: "outside", targetId: "module::a" },
          { id: "duplicate", sourceId: "outside", targetId: "module::b" },
          { id: "outbound", sourceId: "module::b", targetId: "outside" },
        ],
      ),
      new Set(["module"]),
    );

    expect([...projected.edges.values()]).toEqual([
      { id: "outside->module", sourceId: "outside", targetId: "module" },
      { id: "module->outside", sourceId: "module", targetId: "outside" },
    ]);
  });

  it("projects nested collapsed modules to the outermost visible boundary", () => {
    const nested = node({
      id: "module::nested",
      kind: "module",
      parentId: "module",
      type: "<module>",
      hasChildren: true,
    });
    const nestedChild = node({ id: "module::nested::child", parentId: "module::nested" });
    const projected = projectGraph(
      makeClientGraph(
        [moduleNode, nested, nestedChild, outside],
        [{ id: "edge", sourceId: "module::nested::child", targetId: "outside" }],
      ),
      new Set(["module", "module::nested"]),
    );

    expect([...projected.nodes.keys()]).toEqual(["module", "outside"]);
    expect([...projected.edges.values()]).toEqual([{ id: "module->outside", sourceId: "module", targetId: "outside" }]);
  });
});

describe("subtreeGraph", () => {
  it("keeps a module, its descendants, and the dependencies among them", () => {
    const graph = makeClientGraph(
      [
        node({ id: "m", kind: "module", type: "<module>", hasChildren: true }),
        node({ id: "m::a", parentId: "m" }),
        node({ id: "m::b", parentId: "m" }),
        node({ id: "mPrefixed" }),
      ],
      [
        { id: "m::a->m::b", sourceId: "m::a", targetId: "m::b" },
        { id: "mPrefixed->m", sourceId: "mPrefixed", targetId: "m" },
      ],
    );

    const subtree = subtreeGraph(graph, "m");

    expect([...subtree.nodes.keys()]).toEqual(["m", "m::a", "m::b"]);
    expect([...subtree.edges.keys()]).toEqual(["m::a->m::b"]);
  });
});

describe("measureGraph", () => {
  it("reports measured sizes, and zero for a node not yet measured", () => {
    const graph = indexGraph({
      nodes: [node({ id: "measured" }), node({ id: "unmeasured" })],
      edges: [],
    });

    const measured = measureGraph(graph, new Map([["measured", box(220, 80)]]));

    expect(measured.nodes).toEqual([
      { id: "measured", kind: "resource", parentId: null, width: 220, height: 80 },
      { id: "unmeasured", kind: "resource", parentId: null, width: 0, height: 0 },
    ]);
  });

  it("carries edges through by id", () => {
    const graph = indexGraph({
      nodes: [node({ id: "a" })],
      edges: [{ id: "a->b", sourceId: "a", targetId: "b" }],
    });

    expect(measureGraph(graph, new Map()).edges).toEqual([{ id: "a->b", sourceId: "a", targetId: "b" }]);
  });

  it("rounds sizes to whole pixels, absorbing floating-point drift from moving a box", () => {
    const graph = indexGraph({ nodes: [node({ id: "a" })], edges: [] });

    expect(measureGraph(graph, new Map([["a", box(228.0000000001, 83.9999999999)]])).nodes).toEqual([
      { id: "a", kind: "resource", parentId: null, width: 228, height: 84 },
    ]);
  });
});

function measuredNode(overrides: Partial<MeasuredGraphNode> = {}): MeasuredGraphNode {
  return { id: "a", kind: "resource", parentId: null, width: 220, height: 80, ...overrides };
}

describe("measuredGraphsEqual", () => {
  const base: MeasuredGraph = {
    nodes: [measuredNode({ id: "a" }), measuredNode({ id: "b" })],
    edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
  };

  it("returns false when the previous input is null", () => {
    expect(measuredGraphsEqual(null, base)).toBe(false);
  });

  it("returns true for the same graph regardless of node and edge order", () => {
    const reordered: MeasuredGraph = {
      nodes: [measuredNode({ id: "b" }), measuredNode({ id: "a" })],
      edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
    };
    expect(measuredGraphsEqual(base, reordered)).toBe(true);
  });

  it("returns false when a node count differs", () => {
    const extra: MeasuredGraph = { nodes: [...base.nodes, measuredNode({ id: "c" })], edges: base.edges };
    expect(measuredGraphsEqual(base, extra)).toBe(false);
  });

  it("returns false when a measured size differs", () => {
    const widened: MeasuredGraph = {
      nodes: [measuredNode({ id: "a", width: 221 }), measuredNode({ id: "b" })],
      edges: base.edges,
    };
    expect(measuredGraphsEqual(base, widened)).toBe(false);
  });

  it("returns false when containment differs", () => {
    const reparented: MeasuredGraph = {
      nodes: [measuredNode({ id: "a", parentId: "b" }), measuredNode({ id: "b" })],
      edges: base.edges,
    };
    expect(measuredGraphsEqual(base, reparented)).toBe(false);
  });

  it("returns false when the edge set differs", () => {
    const rewired: MeasuredGraph = {
      nodes: base.nodes,
      edges: [{ id: "b>a", sourceId: "b", targetId: "a" }],
    };
    expect(measuredGraphsEqual(base, rewired)).toBe(false);
  });
});
function makeClientGraph(nodes: GraphNode[], edges: GraphEdge[] = []): ClientGraph {
  return indexGraph({ nodes, edges });
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

  it("reports a change from or to an empty graph", () => {
    expect(clientGraphsRenderEqually(EMPTY_CLIENT_GRAPH, EMPTY_CLIENT_GRAPH)).toBe(true);
    expect(clientGraphsRenderEqually(EMPTY_CLIENT_GRAPH, makeClientGraph([node()]))).toBe(false);
    expect(clientGraphsRenderEqually(makeClientGraph([node()]), EMPTY_CLIENT_GRAPH)).toBe(false);
  });

  it.each([
    ["id", { id: "other" }],
    ["type", { type: "Microsoft.Web/sites" }],
    ["isCollection", { isCollection: true }],
    ["hasChildren", { hasChildren: true }],
    ["hasError", { hasError: true }],
    ["requiresRestore", { requiresRestore: true }],
  ])("reports a change when %s differs", (_field, overrides) => {
    const before = makeClientGraph([node()]);
    const after = makeClientGraph([node(overrides as Partial<GraphNode>)]);

    expect(clientGraphsRenderEqually(before, after)).toBe(false);
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
