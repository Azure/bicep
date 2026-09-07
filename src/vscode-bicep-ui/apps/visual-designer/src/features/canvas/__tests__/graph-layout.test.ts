// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode, GraphPatch } from "../api";

import { describe, expect, it } from "vitest";
import { centerGraphLayout, extractGraphLayout, patchMayAffectLayout } from "../graph-layout";

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

function graphOf(...nodes: GraphNode[]) {
  return { nodes: new Map(nodes.map((graphNode) => [graphNode.id, graphNode])) };
}

/** Mirror the server's `updateNode`: only the changed metadata fields are sent. */
function fullUpdate(graphNode: GraphNode, changes: Partial<GraphNode> = {}): GraphPatch {
  const merged = { ...graphNode, ...changes };
  return {
    op: "updateNode",
    nodeId: graphNode.id,
    changes: {
      type: merged.type,
      isCollection: merged.isCollection,
      hasChildren: merged.hasChildren,
      hasError: merged.hasError,
    },
  };
}

describe("patchMayAffectLayout", () => {
  const graphNode = node({ id: "a" });
  const graph = graphOf(graphNode);

  it("treats structural patches as layout-affecting", () => {
    const structural: GraphPatch[] = [
      { op: "clearGraph" },
      { op: "addNode", node: node({ id: "b" }) },
      { op: "removeNode", nodeId: "a" },
      { op: "addEdge", edge: { id: "a>b", sourceId: "a", targetId: "b" } },
      { op: "removeEdge", edgeId: "a>b" },
    ];

    for (const patch of structural) {
      expect(patchMayAffectLayout(graph, patch)).toBe(true);
    }
  });

  it("does not reflow an addNode patch with an explicit placement", () => {
    const patch: GraphPatch = { op: "addNode", node: node({ id: "placed" }) };

    expect(patchMayAffectLayout(graph, patch, new Set(["placed"]))).toBe(false);
    expect(patchMayAffectLayout(graph, patch, new Set(["other"]))).toBe(true);
  });

  it("treats layout and error-count patches as non-affecting", () => {
    expect(patchMayAffectLayout(graph, { op: "setNodeLayout", nodeId: "a", layout: { x: 1, y: 2 } })).toBe(false);
    expect(patchMayAffectLayout(graph, { op: "setErrorCount", errorCount: 3 })).toBe(false);
  });

  it("does not reflow when an updateNode only toggles hasError", () => {
    expect(patchMayAffectLayout(graph, fullUpdate(graphNode, { hasError: true }))).toBe(false);
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
    expect(patchMayAffectLayout(graph, fullUpdate(graphNode, { type: "Microsoft.Web/sites" }))).toBe(true);
    expect(patchMayAffectLayout(graph, fullUpdate(graphNode, { isCollection: true }))).toBe(true);
    expect(patchMayAffectLayout(graph, fullUpdate(graphNode, { hasChildren: true }))).toBe(true);
  });

  it("does not reflow for an updateNode targeting an unknown node", () => {
    expect(patchMayAffectLayout(graph, fullUpdate(node({ id: "missing" })))).toBe(false);
  });
});

describe("extractGraphLayout", () => {
  it("extracts layout patches and ignores unrelated patches", () => {
    const patches: GraphPatch[] = [
      { op: "setNodeLayout", nodeId: "a", layout: { x: 1, y: 2 } },
      { op: "setErrorCount", errorCount: 1 },
      { op: "setNodeLayout", nodeId: "b", layout: { x: 3, y: 4 } },
    ];

    const { nodeLayouts, graphBounds } = extractGraphLayout(patches);

    expect([...nodeLayouts]).toEqual([
      ["a", { x: 1, y: 2 }],
      ["b", { x: 3, y: 4 }],
    ]);
    expect(graphBounds).toBeNull();
  });

  it("takes the last bounds when several are present", () => {
    const patches: GraphPatch[] = [
      { op: "setGraphBounds", bounds: { width: 10, height: 10 } },
      { op: "setGraphBounds", bounds: { width: 20, height: 30 } },
    ];

    expect(extractGraphLayout(patches).graphBounds).toEqual({ width: 20, height: 30 });
  });
});

describe("centerGraphLayout", () => {
  it("passes layouts through untouched when there are no bounds to centre against", () => {
    const layouts = new Map([["a", { x: 5, y: 5 }]]);
    const result = centerGraphLayout(layouts, null, { x: 100, y: 100 });

    expect(result.bounds).toBeNull();
    expect(result.nodeLayouts).toBe(layouts);
  });

  it("shifts every node by the same offset and reports matching bounds", () => {
    const layouts = new Map([
      ["a", { x: 0, y: 0 }],
      ["b", { x: 100, y: 50 }],
    ]);

    const { nodeLayouts, bounds } = centerGraphLayout(layouts, { width: 100, height: 50 }, { x: 500, y: 300 });

    expect(nodeLayouts.get("a")).toEqual({ x: 450, y: 275 });
    expect(nodeLayouts.get("b")).toEqual({ x: 550, y: 325 });
    expect(bounds).toEqual({ min: { x: 450, y: 275 }, max: { x: 550, y: 325 } });
  });

  it("leaves the graph centred on the viewport centre", () => {
    const { bounds } = centerGraphLayout(new Map(), { width: 200, height: 100 }, { x: 640, y: 400 });

    expect((bounds!.min.x + bounds!.max.x) / 2).toBe(640);
    expect((bounds!.min.y + bounds!.max.y) / 2).toBe(400);
  });
});
