// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { computePaintOrder, getEdgeZIndex, getNodeZIndex } from "../paint-order";

const nodes = [
  { id: "a", parentId: null },
  { id: "m", parentId: null },
  { id: "x", parentId: "m" },
  { id: "n", parentId: "m" },
  { id: "y", parentId: "n" },
  { id: "z", parentId: "n" },
  { id: "b", parentId: null },
];

function order(raisedNodeId: string | null) {
  return [...computePaintOrder(nodes, raisedNodeId)].sort(([, left], [, right]) => left - right).map(([id]) => id);
}

describe("computePaintOrder", () => {
  it("paints containers before their contents, in sibling order", () => {
    expect(order(null)).toEqual(["a", "m", "x", "n", "y", "z", "b"]);
  });

  it("raises a node and each of its ancestors above their siblings", () => {
    expect(order("y")).toEqual(["a", "b", "m", "x", "n", "z", "y"]);
  });

  it("raises a container with its contents", () => {
    expect(order("n")).toEqual(["a", "b", "m", "x", "n", "y", "z"]);
  });

  it("does not parse IDs and treats a node with an absent parent as a root", () => {
    const paintOrder = computePaintOrder(
      [
        { id: "m::child", parentId: null },
        { id: "orphan", parentId: "missing" },
      ],
      "orphan",
    );

    expect([...paintOrder.keys()]).toEqual(["m::child", "orphan"]);
  });

  it("ignores a raised node that is not in the graph", () => {
    expect(order("missing")).toEqual(order(null));
  });
});

describe("getEdgeZIndex", () => {
  const paintOrder = computePaintOrder(nodes, null);
  const below = (nodeId: string) => ({ nodeId, placement: "below" as const });

  it("paints an edge just below its higher endpoint", () => {
    const zIndex = getEdgeZIndex(paintOrder, [below("a"), below("b")]);

    expect(zIndex).toBe(getNodeZIndex(paintOrder, "b") - 1);
    expect(zIndex).toBeGreaterThan(getNodeZIndex(paintOrder, "m"));
    expect(zIndex).toBeGreaterThan(getNodeZIndex(paintOrder, "z"));
  });

  it("paints an edge inside a container over its frame and under the higher endpoint", () => {
    const zIndex = getEdgeZIndex(paintOrder, [below("y"), below("z")]);

    expect(zIndex).toBeGreaterThan(getNodeZIndex(paintOrder, "n"));
    expect(zIndex).toBeGreaterThan(getNodeZIndex(paintOrder, "y"));
    expect(zIndex).toBeLessThan(getNodeZIndex(paintOrder, "z"));
  });

  it("raises an edge with a raised endpoint", () => {
    const raised = computePaintOrder(nodes, "a");

    expect(getEdgeZIndex(raised, [below("a"), below("b")])).toBeGreaterThan(getNodeZIndex(raised, "m"));
  });

  it("paints an edge leaving a container above it, and one with no anchors at the bottom", () => {
    expect(getEdgeZIndex(paintOrder, [{ nodeId: "n", placement: "above" }])).toBe(getNodeZIndex(paintOrder, "n") + 1);
    expect(getEdgeZIndex(paintOrder, [below("missing")])).toBe(0);
  });
});
