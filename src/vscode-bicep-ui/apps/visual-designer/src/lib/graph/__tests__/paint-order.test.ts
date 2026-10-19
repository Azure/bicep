// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { computePaintOrder } from "../paint-order";

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
