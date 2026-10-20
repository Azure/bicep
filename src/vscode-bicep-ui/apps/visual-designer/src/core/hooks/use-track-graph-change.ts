// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useStore } from "jotai";
import { useCallback } from "react";
import { graphChangesInProgressCountAtom } from "../atoms";

/**
 * Returns a function that counts an operation as a graph change in progress until it settles.
 * While any is in progress, Undo, Redo, Reset Layout, and node gestures are blocked.
 */
export function useTrackGraphChange() {
  const store = useStore();

  return useCallback(
    async (operation: () => Promise<void>): Promise<void> => {
      store.set(graphChangesInProgressCountAtom, (count) => count + 1);
      try {
        await operation();
      } finally {
        store.set(graphChangesInProgressCountAtom, (count) => count - 1);
      }
    },
    [store],
  );
}
