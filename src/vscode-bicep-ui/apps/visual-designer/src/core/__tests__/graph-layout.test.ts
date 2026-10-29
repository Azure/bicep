// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphEdge, GraphNode } from "../api";

import { describe, expect, it } from "vitest";
import { centerGraphLayout, getLayoutCenters, getSubgraph, graphChangeMayAffectLayout } from "../graph-layout";
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
    sourceStatus: null,
    sourceError: null,
    ...overrides,
  };
}

function graphOf(nodes: GraphNode[], edges: GraphEdge[] = []) {
  return indexGraph({ nodes, edges });
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

  it("does not reflow a module that stays shown without children", () => {
    const module = node({ id: "m", kind: "module", type: "<module>", sourceStatus: "unrestored" });
    const previous = graphOf([module, a]);

    // Its children arrived while it is collapsed, or it explains why it has none: it keeps its size.
    expect(
      graphChangeMayAffectLayout(
        previous,
        graphOf([{ ...module, sourceStatus: null, hasChildren: true, isCollapsed: true }, a]),
      ),
    ).toBe(false);
    expect(
      graphChangeMayAffectLayout(previous, graphOf([{ ...module, sourceStatus: "error", sourceError: "Failed." }, a])),
    ).toBe(false);
    // Shown open, it becomes a container.
    expect(
      graphChangeMayAffectLayout(previous, graphOf([{ ...module, sourceStatus: null, hasChildren: true }, a])),
    ).toBe(true);
  });
});

describe("getSubgraph", () => {
  const measured = (id: string, parentId: string | null) => ({
    id,
    kind: "resource" as const,
    parentId,
    width: 1,
    height: 1,
  });

  it("keeps the module as the only top-level node, with its descendants and the dependencies between them", () => {
    const graph = {
      nodes: [
        measured("m", null),
        measured("m::a", "m"),
        measured("m::n", "m"),
        measured("m::n::b", "m::n"),
        measured("x", null),
      ],
      edges: [edge("m::a", "m::n"), edge("x", "m::a"), edge("x", "m")],
    };

    expect(getSubgraph(graph, "m")).toEqual({
      nodes: [measured("m", null), measured("m::a", "m"), measured("m::n", "m"), measured("m::n::b", "m::n")],
      edges: [edge("m::a", "m::n")],
    });

    expect(getSubgraph(graph, "m::n")).toEqual({
      nodes: [measured("m::n", null), measured("m::n::b", "m::n")],
      edges: [],
    });
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

describe("getLayoutCenters", () => {
  it("converts the corners of nodes that are not containers to centers", () => {
    const centers = getLayoutCenters(
      new Map([
        ["module", { x: 0, y: 0 }],
        ["module::a", { x: 40, y: 50 }],
        ["collapsed", { x: 300, y: 0 }],
      ]),
      {
        nodes: [
          { id: "module", kind: "module", parentId: null, width: 300, height: 200 },
          { id: "module::a", kind: "resource", parentId: "module", width: 220, height: 80 },
          { id: "collapsed", kind: "module", parentId: null, width: 220, height: 84 },
        ],
        edges: [],
      },
    );

    expect(centers).toEqual(
      new Map([
        ["module::a", { x: 150, y: 90 }],
        ["collapsed", { x: 410, y: 42 }],
      ]),
    );
  });
});
