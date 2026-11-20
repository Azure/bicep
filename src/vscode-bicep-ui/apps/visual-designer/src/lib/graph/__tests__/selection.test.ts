// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { SelectableNode } from "../selection";

import { describe, expect, it } from "vitest";
import { getNodeIdsInMarquee } from "../selection";

function node(id: string, parentId: string | null, [x0, y0, x1, y1]: number[], isContainer = false): SelectableNode {
  return { id, parentId, box: { min: { x: x0!, y: y0! }, max: { x: x1!, y: y1! } }, isContainer };
}

// `module` frames `a` and `b`; `c` is outside it.
const nodes = [
  node("module", null, [0, 0, 100, 100], true),
  node("a", "module", [10, 10, 40, 40]),
  node("b", "module", [60, 60, 90, 90]),
  node("c", null, [200, 0, 240, 40]),
];

const box = (x0: number, y0: number, x1: number, y1: number) => ({ min: { x: x0, y: y0 }, max: { x: x1, y: y1 } });

describe("getNodeIdsInMarquee", () => {
  it("selects the nodes a marquee touches", () => {
    expect(getNodeIdsInMarquee(nodes, box(30, 30, 210, 50))).toEqual(["a", "c"]);
  });

  it("selects a container only when the marquee encloses it, together with what is inside it", () => {
    expect(getNodeIdsInMarquee(nodes, box(5, 5, 95, 95))).toEqual(["a", "b"]);
    expect(getNodeIdsInMarquee(nodes, box(-1, -1, 101, 101))).toEqual(["module", "a", "b"]);
  });

  it("selects nothing in empty space", () => {
    expect(getNodeIdsInMarquee(nodes, box(110, 110, 150, 150))).toEqual([]);
  });
});
