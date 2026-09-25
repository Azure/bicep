// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useCallback, useMemo } from "react";
import { panZoomControlAtom, useStore } from "./atoms";

/**
 * A hook that provides access to the pan-zoom control, allowing for zooming in, zooming out, and resetting the pan-zoom transform values.
 *
 * @returns The pan-zoom control object, which includes functions for zooming in, zooming out, and resetting.
 */
export function usePanZoomControl() {
  const store = useStore();
  const zoomIn = useCallback(
    (scaleFactor?: number) => store.get(panZoomControlAtom).zoomIn(scaleFactor),
    [store],
  );
  const zoomOut = useCallback(
    (scaleFactor?: number) => store.get(panZoomControlAtom).zoomOut(scaleFactor),
    [store],
  );
  const reset = useCallback(() => store.get(panZoomControlAtom).reset(), [store]);
  const transform = useCallback(
    (x: number, y: number, scale: number) => store.get(panZoomControlAtom).transform(x, y, scale),
    [store],
  );

  return useMemo(
    () => ({
      zoomIn,
      zoomOut,
      reset,
      transform,
    }),
    [reset, transform, zoomIn, zoomOut],
  );
}
