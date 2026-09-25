// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useMemo } from "react";
import { resetPanZoomAtom, transformPanZoomAtom, useSetAtom, zoomInAtom, zoomOutAtom } from "./atoms";

/**
 * Provides pan-zoom commands without subscribing to the controller that PanZoom registers on mount.
 * Commands require a mounted PanZoom surface.
 *
 * @returns The pan-zoom control object, which includes functions for zooming in, zooming out, and resetting.
 */
export function usePanZoomControl() {
  const zoomIn = useSetAtom(zoomInAtom);
  const zoomOut = useSetAtom(zoomOutAtom);
  const reset = useSetAtom(resetPanZoomAtom);
  const transform = useSetAtom(transformPanZoomAtom);

  return useMemo(() => ({ zoomIn, zoomOut, reset, transform }), [reset, transform, zoomIn, zoomOut]);
}
