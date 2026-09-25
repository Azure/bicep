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
}

/**
 * Spends a render budget over groups in display order. Each rendered header and each rendered row costs one,
 * collapsed groups cost only their header, and rendering stops where the budget runs out so the list never
 * shows a later group while an earlier one is still truncated.
 */
export function allocateProgressiveRows(groups: readonly ProgressiveGroup[], budget: number): ProgressiveAllocation {
  const rowsPerGroup: number[] = [];
  let remaining = budget;

  for (const group of groups) {
    if (remaining <= 0) {
      return { rowsPerGroup, hasMore: true };
    }

    remaining -= 1;
    const rows = group.expanded ? Math.min(group.rowCount, remaining) : 0;
    rowsPerGroup.push(rows);
    remaining -= rows;

    if (group.expanded && rows < group.rowCount) {
      return { rowsPerGroup, hasMore: true };
    }
  }

  return { rowsPerGroup, hasMore: false };
}
