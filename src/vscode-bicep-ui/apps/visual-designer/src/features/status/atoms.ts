// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/**
 * The number of diagnostics errors in the file.
 * Updated by the deployment graph notification handler.
 */
export const errorCountAtom = atom(0);

/**
 * Whether the current deployment graph has any nodes.
 * Updated by the deployment graph notification handler.
 */
export const hasNodesAtom = atom(false);

export type GraphStatus = { kind: "errors"; errorCount: number } | { kind: "empty" } | { kind: "ready" };

/**
 * Semantic status of the current graph used by the status bar.
 * Keeping this decision in atoms avoids duplicating UI state logic.
 */
export const graphStatusAtom = atom<GraphStatus>((get) => {
  const errorCount = get(errorCountAtom);
  if (errorCount > 0) {
    return { kind: "errors", errorCount };
  }

  if (!get(hasNodesAtom)) {
    return { kind: "empty" };
  }

  return { kind: "ready" };
});
