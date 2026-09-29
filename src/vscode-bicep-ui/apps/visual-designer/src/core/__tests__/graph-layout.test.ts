// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphEdge, GraphNode } from "../api";

import { describe, expect, it } from "vitest";
import { centerGraphLayout, graphChangeMayAffectLayout } from "../graph-layout";
import { indexGraph } from "../graph-model";

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

function graphOf(nodes: GraphNode[], edges: GraphEdge[] = []) {
  return indexGraph({ nodes, edges, errorCount: 0 });
}

function edge(sourceId: string, targetId: string): GraphEdge {
  return { id: `${sourceId}->${targetId}`, sourceId, targetId };
}

describe("graphChangeMayAffectLayout", () => {
  const a = node({ id: "a" });
  const b = node({ id: "b" });

  it("reflows when nodes or edges are added or removed", () => {
    const previous = graphOf([a, b], [edge("a", "b")]);

    expect(graphChangeMayAffectLayout(previous, graphOf([a, b, node({ id: "c" })], [edge("a", "b")]))).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([a], []))).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([a, b]))).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([a, b], [edge("b", "a")]))).toBe(true);
  });

  it("does not reflow an identical graph", () => {
    const previous = graphOf([a, b], [edge("a", "b")]);

    expect(graphChangeMayAffectLayout(previous, graphOf([b, a], [edge("a", "b")]))).toBe(false);
  });

  it("does not reflow for a node added where the user dropped it", () => {
    const next = graphOf([a, node({ id: "placed" })]);

    expect(graphChangeMayAffectLayout(graphOf([a]), next, { explicitlyPlacedNodeIds: new Set(["placed"]) })).toBe(
      false,
    );
    expect(graphChangeMayAffectLayout(graphOf([a]), next, { explicitlyPlacedNodeIds: new Set(["other"]) })).toBe(true);
  });

  it("preserves layout when undo removes only its independent resource", () => {
    const previous = graphOf([node({ id: "placed" }), a], [edge("placed", "a")]);
    const exemptions = { pendingRemovalNodeIds: new Set(["placed"]) };

    expect(graphChangeMayAffectLayout(previous, graphOf([a], [edge("placed", "a")]), exemptions)).toBe(false);
    expect(graphChangeMayAffectLayout(previous, graphOf([a], [edge("placed", "a")]))).toBe(true);
    expect(
      graphChangeMayAffectLayout(previous, graphOf([node({ id: "placed" })], [edge("placed", "a")]), exemptions),
    ).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([a]), exemptions)).toBe(true);
  });

  it("still reflows structural nodes even if an undo claims their ID", () => {
    const module = node({ id: "module", kind: "module", type: "<module>", hasChildren: true });
    const child = node({ id: "module::child", parentId: "module" });
    const exemptions = { pendingRemovalNodeIds: new Set(["module", "module::child"]) };

    expect(graphChangeMayAffectLayout(graphOf([module, child]), graphOf([module]), exemptions)).toBe(true);
    expect(graphChangeMayAffectLayout(graphOf([module, child]), graphOf([]), exemptions)).toBe(true);
  });

  it("reflows when a node changes kind or container", () => {
    const previous = graphOf([a]);

    expect(graphChangeMayAffectLayout(previous, graphOf([{ ...a, kind: "module" }]))).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([{ ...a, parentId: "module" }]))).toBe(true);
  });

  it("does not reflow when a node only toggles hasError", () => {
    expect(graphChangeMayAffectLayout(graphOf([a]), graphOf([{ ...a, hasError: true }]))).toBe(false);
  });

  it("reflows when a size-affecting field changes", () => {
    const previous = graphOf([a]);

    expect(graphChangeMayAffectLayout(previous, graphOf([{ ...a, type: "Microsoft.Web/sites" }]))).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([{ ...a, isCollection: true }]))).toBe(true);
    expect(graphChangeMayAffectLayout(previous, graphOf([{ ...a, hasChildren: true }]))).toBe(true);
  });
});

describe("centerGraphLayout", () => {
  it("passes positions through untouched when there are no bounds to centre against", () => {
    const result = centerGraphLayout([{ nodeId: "a", x: 5, y: 5 }], null, { x: 100, y: 100 });

    expect(result.bounds).toBeNull();
    expect([...result.positions]).toEqual([["a", { x: 5, y: 5 }]]);
  });

  it("shifts every node by the same offset and reports matching bounds", () => {
    const { positions, bounds } = centerGraphLayout(
      [
        { nodeId: "a", x: 0, y: 0 },
        { nodeId: "b", x: 100, y: 50 },
      ],
      { width: 100, height: 50 },
      { x: 500, y: 300 },
    );

    expect(positions.get("a")).toEqual({ x: 450, y: 275 });
    expect(positions.get("b")).toEqual({ x: 550, y: 325 });
    expect(bounds).toEqual({ min: { x: 450, y: 275 }, max: { x: 550, y: 325 } });
  });

  it("leaves the graph centred on the viewport centre", () => {
    const { bounds } = centerGraphLayout([], { width: 200, height: 100 }, { x: 640, y: 400 });

    expect((bounds!.min.x + bounds!.max.x) / 2).toBe(640);
    expect((bounds!.min.y + bounds!.max.y) / 2).toBe(400);
  });
});
