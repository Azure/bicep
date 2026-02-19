// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useSetAtom } from "jotai";
import { useCallback, useRef } from "react";
import { applyLayoutAtom } from "../elk-layout";
import { useComputeLayout } from "./use-compute-layout";

/**
 * Returns a callback that computes a fresh ELK layout and applies
 * it to the graph with spring animations.  Concurrent invocations
 * are deduplicated — if a layout is already in flight the callback
 * is a no-op.
 */
export function useResetLayout() {
  const computeLayout = useComputeLayout();
  const applyLayout = useSetAtom(applyLayoutAtom);
  const layoutInFlight = useRef(false);

  return useCallback(async () => {
    if (layoutInFlight.current) return;
    layoutInFlight.current = true;
    try {
      const { layout } = await computeLayout();
      await applyLayout({ result: layout });
    } finally {
      layoutInFlight.current = false;
    }
  }, [computeLayout, applyLayout]);
}
