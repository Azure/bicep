// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode, GraphPatch, RenderedGraph, RenderedGraphNode } from "../messages";

import { describe, expect, it } from "vitest";
import { patchMayAffectLayout, renderedGraphsEqual } from "../layout-invalidation";

function makeNode(overrides: Partial<GraphNode> = {}): GraphNode {
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

function graphOf(...nodes: GraphNode[]) {
  return { nodes: new Map(nodes.map((node) => [node.id, node])) };
}

/** Mirror the server's `updateNode`: only the changed metadata fields are sent. */
function fullUpdate(node: GraphNode, changes: Partial<GraphNode> = {}): GraphPatch {
  const merged = { ...node, ...changes };
  return {
    op: "updateNode",
    nodeId: node.id,
    changes: {
      type: merged.type,
      isCollection: merged.isCollection,
      hasChildren: merged.hasChildren,
      hasError: merged.hasError,
    },
  };
}

describe("patchMayAffectLayout", () => {
  const node = makeNode({ id: "a" });
  const graph = graphOf(node);

  it("treats structural patches as layout-affecting", () => {
    const structural: GraphPatch[] = [
      { op: "clearGraph" },
      { op: "addNode", node: makeNode({ id: "b" }) },
      { op: "removeNode", nodeId: "a" },
      { op: "addEdge", edge: { id: "a>b", sourceId: "a", targetId: "b" } },
      { op: "removeEdge", edgeId: "a>b" },
    ];

    for (const patch of structural) {
      expect(patchMayAffectLayout(graph, patch)).toBe(true);
    }
  });

  it("treats setNodeLayout and setErrorCount as non-affecting", () => {
    expect(patchMayAffectLayout(graph, { op: "setNodeLayout", nodeId: "a", layout: { x: 1, y: 2 } })).toBe(false);
    expect(patchMayAffectLayout(graph, { op: "setErrorCount", errorCount: 3 })).toBe(false);
  });

  it("does not reflow when an updateNode only toggles hasError", () => {
    expect(patchMayAffectLayout(graph, fullUpdate(node, { hasError: true }))).toBe(false);
  });

  it("ignores null update fields as omitted metadata", () => {
    expect(
      patchMayAffectLayout(graph, {
        op: "updateNode",
        nodeId: "a",
        changes: { type: null, isCollection: null, hasChildren: null, hasError: true },
      }),
    ).toBe(false);
  });

  it("reflows when a size-affecting field actually changes", () => {
    expect(patchMayAffectLayout(graph, fullUpdate(node, { type: "Microsoft.Web/sites" }))).toBe(true);
    expect(patchMayAffectLayout(graph, fullUpdate(node, { isCollection: true }))).toBe(true);
    expect(patchMayAffectLayout(graph, fullUpdate(node, { hasChildren: true }))).toBe(true);
  });

  it("does not reflow for an updateNode targeting an unknown node", () => {
    expect(patchMayAffectLayout(graph, fullUpdate(makeNode({ id: "missing" })))).toBe(false);
  });
});

describe("renderedGraphsEqual", () => {
  function rnode(overrides: Partial<RenderedGraphNode> = {}): RenderedGraphNode {
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

  const base: RenderedGraph = {
    nodes: [rnode({ id: "a" }), rnode({ id: "b" })],
    edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
  };

  it("returns false when the previous input is null", () => {
    expect(renderedGraphsEqual(null, base)).toBe(false);
  });

  it("returns true for the same graph regardless of node and edge order", () => {
    const reordered: RenderedGraph = {
      nodes: [rnode({ id: "b" }), rnode({ id: "a" })],
      edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
    };
    expect(renderedGraphsEqual(base, reordered)).toBe(true);
  });

  it("returns false when a node count differs", () => {
    const extra: RenderedGraph = { nodes: [...base.nodes, rnode({ id: "c" })], edges: base.edges };
    expect(renderedGraphsEqual(base, extra)).toBe(false);
  });

  it("returns false when a measured size differs (the sub-pixel case)", () => {
    const widened: RenderedGraph = {
      nodes: [rnode({ id: "a", width: 221 }), rnode({ id: "b" })],
      edges: base.edges,
    };
    expect(renderedGraphsEqual(base, widened)).toBe(false);
  });

  it("returns false when containment (parentId) differs", () => {
    const reparented: RenderedGraph = {
      nodes: [rnode({ id: "a", parentId: "b" }), rnode({ id: "b" })],
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
