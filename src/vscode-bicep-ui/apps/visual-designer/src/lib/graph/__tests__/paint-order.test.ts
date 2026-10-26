// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";
import type { PaintOrderNode } from "../paint-order";

import { describe, expect, it } from "vitest";
import { computePaintOrder, getEdgeZIndex, getNodeZIndex } from "../paint-order";

const box = (x: number, y: number, width = 10, height = 10): Box => ({
  min: { x, y },
  max: { x: x + width, y: y + height },
});

function node(id: string, parentId: string | null, targetBox: Box | null, isCompound = false): PaintOrderNode {
  return { id, parentId, isCompound, targetBox };
}

// a   m [x  n [y  z]]   b, laid out left to right without overlapping.
const nodes = [
  node("a", null, box(0, 0)),
  node("m", null, box(100, 0, 200, 100), true),
  node("x", "m", box(110, 10)),
  node("n", "m", box(150, 10, 100, 50), true),
  node("y", "n", box(160, 20)),
  node("z", "n", box(200, 20)),
  node("b", null, box(400, 0)),
];

function order(raisedNodeId: string | null, input: readonly PaintOrderNode[] = nodes) {
  const paintOrder = computePaintOrder(input, raisedNodeId);
  return [...paintOrder.nodeZIndices].sort(([, left], [, right]) => left - right).map(([id]) => id);
}

describe("computePaintOrder", () => {
  it("paints parents before their children, in sibling order", () => {
    expect(order(null)).toEqual(["a", "m", "x", "n", "y", "z", "b"]);
  });

  it("raises a node and each of its ancestors above their siblings", () => {
    expect(order("y")).toEqual(["a", "b", "m", "x", "n", "z", "y"]);
  });

  it("raises a compound node with its children", () => {
    expect(order("n")).toEqual(["a", "b", "m", "x", "n", "y", "z"]);
  });

  it("does not parse IDs and treats a node with an absent parent as a root", () => {
    const paintOrder = computePaintOrder([node("m::child", null, null), node("orphan", "missing", null)], "orphan");

    expect([...paintOrder.nodeZIndices.keys()]).toEqual(["m::child", "orphan"]);
  });

  it("ignores a raised node that is not in the graph", () => {
    expect(order("missing")).toEqual(order(null));
  });

  it("paints a compound node that a sibling sits on before the other siblings", () => {
    // b was dragged onto m.
    const moved = nodes.map((each) => (each.id === "b" ? node("b", null, box(120, 50)) : each));

    expect(order("b", moved)).toEqual(["m", "x", "n", "y", "z", "a", "b"]);
    // An atomic node sits on a compound node it overlaps, wherever it is in sibling order.
    expect(order(null, moved)).toEqual(["m", "x", "n", "y", "z", "a", "b"]);
  });

  it("puts the later of two overlapping compound nodes on the earlier one", () => {
    const overlapping = [
      node("p", null, box(0, 0, 100, 100), true),
      node("p1", "p", box(10, 10)),
      node("q", null, box(50, 50, 100, 100), true),
      node("q1", "q", box(60, 60)),
      node("c", null, box(500, 0)),
    ];

    expect(order(null, overlapping)).toEqual(["p", "p1", "q", "q1", "c"]);
    expect(order("p", overlapping)).toEqual(["q", "q1", "c", "p", "p1"]);
  });
});

describe("getEdgeZIndex", () => {
  const z = (paintOrder: ReturnType<typeof computePaintOrder>, id: string) => getNodeZIndex(paintOrder, id)!;

  it("paints an edge below every node, whatever the order of its endpoints", () => {
    const paintOrder = computePaintOrder(nodes, null);
    const zIndex = getEdgeZIndex(paintOrder, { parentId: null, endpointIds: ["a", "b"] });

    for (const id of ["a", "m", "x", "n", "y", "z", "b"]) {
      expect(zIndex).toBeLessThan(z(paintOrder, id));
    }
  });

  it("paints an edge between children above their parent and below every child", () => {
    const paintOrder = computePaintOrder(nodes, null);
    const zIndex = getEdgeZIndex(paintOrder, { parentId: "m", endpointIds: ["x", "n"] });

    expect(zIndex).toBeGreaterThan(z(paintOrder, "m"));
    for (const id of ["x", "n", "y", "z"]) {
      expect(zIndex).toBeLessThan(z(paintOrder, id));
    }
  });

  it("paints the edges of a node sitting on a compound node above it, and below every other node", () => {
    const moved = nodes.map((each) => (each.id === "b" ? node("b", null, box(120, 50)) : each));
    const paintOrder = computePaintOrder(moved, "b");
    const onModule = getEdgeZIndex(paintOrder, { parentId: null, endpointIds: ["a", "b"] });

    expect(paintOrder.nodesOnCompounds).toEqual(new Set(["b"]));
    for (const id of ["m", "x", "n", "y", "z"]) {
      expect(onModule).toBeGreaterThan(z(paintOrder, id));
    }
    expect(onModule).toBeLessThan(z(paintOrder, "a"));
    expect(onModule).toBeLessThan(z(paintOrder, "b"));
  });

  it("keeps an edge between nodes that sit on nothing below the compound node others sit on", () => {
    const withC = [...nodes, node("c", null, box(600, 0))].map((each) =>
      each.id === "b" ? node("b", null, box(120, 50)) : each,
    );
    const paintOrder = computePaintOrder(withC, null);

    expect(getEdgeZIndex(paintOrder, { parentId: null, endpointIds: ["a", "c"] })).toBeLessThan(z(paintOrder, "m"));
  });

  it("paints an edge whose parent has no children just above the parent, and one with no parent at the bottom", () => {
    const collapsed = [node("m", null, box(0, 0)), node("a", null, box(100, 0))];
    const paintOrder = computePaintOrder(collapsed, null);

    expect(getEdgeZIndex(paintOrder, { parentId: "m", endpointIds: ["m::x", "m::y"] })).toBe(z(paintOrder, "m") + 1);
    expect(getEdgeZIndex(paintOrder, { parentId: "missing", endpointIds: [] })).toBe(0);
  });
});
