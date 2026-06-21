// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/utils/math/geometry";

import { useGetPanZoomDimensions, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import { graphBoundsAtom } from "@/lib/graph/atoms";
import { getBoxCenter, getBoxHeight, getBoxWidth } from "@/lib/utils/math/geometry";

/**
 * Returns a callback that applies a pan-zoom transform to center the
 * given bounding box in the viewport.  Unlike {@link useFitView}, the
 * bounds are supplied explicitly, so callers can fit to a layout's
 * final positions before the nodes have animated there.
 */
export function useFitViewToBounds() {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const { transform } = usePanZoomControl();

  return useCallback(
    (bounds: Box) => {
      const graphWidth = getBoxWidth(bounds);
      const graphHeight = getBoxHeight(bounds);
      const { x: graphCenterX, y: graphCenterY } = getBoxCenter(bounds);

      const { width: viewportWidth, height: viewportHeight } = getPanZoomDimensions();

      // Calculate scale to fit content in viewport with some padding
      const padding = 100;
      const scaleX = (viewportWidth - padding * 2) / graphWidth;
      const scaleY = (viewportHeight - padding * 2) / graphHeight;
      const scale = Math.min(scaleX, scaleY, 1); // Don't zoom in beyond 1:1

      // Calculate translation to center the content
      const viewportCenterX = viewportWidth / 2;
      const viewportCenterY = viewportHeight / 2;
      const translateX = viewportCenterX - graphCenterX * scale;
      const translateY = viewportCenterY - graphCenterY * scale;

      transform(translateX, translateY, scale);
    },
    [getPanZoomDimensions, transform],
  );
}

/**
 * Returns a callback that reads the graph bounding box
 * and applies a pan-zoom transform to center the graph in the viewport.
 */
export function useFitView() {
  const fitViewToBounds = useFitViewToBounds();

  return useAtomCallback(
    useCallback(
      (get) => {
        const bounds = get(graphBoundsAtom);

        if (bounds === null) {
          return;
        }

        fitViewToBounds(bounds);
      },
      [fitViewToBounds],
    ),
  );
}
