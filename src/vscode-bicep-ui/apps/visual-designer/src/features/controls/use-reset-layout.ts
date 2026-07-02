// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useCallback, useRef } from "react";

/**
 * Returns a callback that requests a fresh graph layout. Concurrent invocations
 * are deduplicated; if a layout is already in flight the callback is a no-op.
 */
export function useResetLayout(requestLayout: () => Promise<void>) {
  const layoutInFlight = useRef(false);

  return useCallback(async () => {
    if (layoutInFlight.current) return;
    layoutInFlight.current = true;
    try {
      await requestLayout();
    } finally {
      layoutInFlight.current = false;
    }
  }, [requestLayout]);
}
