// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { allocateProgressiveRows } from "../progressive-rows";

describe("allocateProgressiveRows", () => {
  it("renders everything when the budget covers every header and row", () => {
    expect(
      allocateProgressiveRows(
        [
          { rowCount: 2, expanded: true },
          { rowCount: 5, expanded: false },
        ],
        10,
      ),
    ).toEqual({ rowsPerGroup: [2, 0], hasMore: false });
  });

  it("counts collapsed groups as their header only", () => {
    const groups = Array.from({ length: 3 }, () => ({ rowCount: 50, expanded: false }));

    expect(allocateProgressiveRows(groups, 3)).toEqual({ rowsPerGroup: [0, 0, 0], hasMore: false });
    expect(allocateProgressiveRows(groups, 2)).toEqual({ rowsPerGroup: [0, 0], hasMore: true });
  });

  it("truncates an expanded group and renders no later group", () => {
    expect(
      allocateProgressiveRows(
        [
          { rowCount: 2, expanded: true },
          { rowCount: 10, expanded: true },
          { rowCount: 1, expanded: true },
        ],
        8,
      ),
    ).toEqual({ rowsPerGroup: [2, 4], hasMore: true });
  });

  it("stops after a header when the budget runs out exactly there", () => {
    expect(allocateProgressiveRows([{ rowCount: 3, expanded: true }], 1)).toEqual({
      rowsPerGroup: [0],
      hasMore: true,
    });
  });

  it("reports nothing more for an exactly spent budget", () => {
    expect(allocateProgressiveRows([{ rowCount: 3, expanded: true }], 4)).toEqual({
      rowsPerGroup: [3],
      hasMore: false,
    });
    expect(allocateProgressiveRows([], 0)).toEqual({ rowsPerGroup: [], hasMore: false });
  });
});
