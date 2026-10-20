// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface ProgressiveGroup {
  rowCount: number;
  expanded: boolean;
}

export interface ProgressiveAllocation {
  /** Rows to render for each leading group; groups past the end of this array are not rendered. */
  rowsPerGroup: number[];
  /** Whether anything was left out, so the list should offer to render more. */
  hasMore: boolean;
  /** First expanded group with rows still to render, if any. */
  truncatedGroupIndex?: number;
}

/**
 * Spends a render budget over groups in display order. Each rendered header and each rendered row costs one,
 * collapsed groups cost only their header. `minHeaders` keeps already-visible browse headers mounted when
 * opening a large group consumes the budget; the remaining rows are rendered before any further rows.
 */
export function allocateProgressiveRows(
  groups: readonly ProgressiveGroup[],
  budget: number,
  minHeaders = 0,
): ProgressiveAllocation {
  const rowsPerGroup: number[] = [];
  let remaining = budget;
  let truncatedGroupIndex: number | undefined;

  for (const group of groups) {
    if (remaining <= 0 && rowsPerGroup.length >= minHeaders) {
      return truncatedGroupIndex === undefined
        ? { rowsPerGroup, hasMore: true }
        : { rowsPerGroup, hasMore: true, truncatedGroupIndex };
    }

    remaining = Math.max(0, remaining - 1);
    const rows = group.expanded && truncatedGroupIndex === undefined ? Math.min(group.rowCount, remaining) : 0;
    if (group.expanded && rows < group.rowCount && truncatedGroupIndex === undefined) {
      truncatedGroupIndex = rowsPerGroup.length;
    }
    rowsPerGroup.push(rows);
    remaining -= rows;
  }

  return truncatedGroupIndex === undefined
    ? { rowsPerGroup, hasMore: false }
    : { rowsPerGroup, hasMore: true, truncatedGroupIndex };
}
