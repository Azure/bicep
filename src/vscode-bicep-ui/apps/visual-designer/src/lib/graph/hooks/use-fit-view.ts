// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";

import { useGetPanZoomDimensions, useGetPanZoomTransform, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomCallback } from "jotai/utils";
import { animate } from "motion";
import { useCallback, useEffect, useRef } from "react";
import { getBoxCenter, getBoxHeight, getBoxWidth } from "@/lib/math";
import { graphBoundsAtom } from "../atoms";
import { NODE_MOVE_TRANSITION } from "../motion";

/** The furthest the camera zooms out. */
export const MINIMUM_ZOOM = 1 / 4;

/**
 * How the camera moves to frame the bounds:
 * - `animate`: the camera's own transition.
 * - `instant`: at once.
 * - `follow-nodes`: in step with nodes that start moving to those bounds now, from the center of the view.
 */
export type FitViewMotion = "animate" | "instant" | "follow-nodes";

type CameraTransform = { x: number; y: number; scale: number };

function transformsEqual(a: CameraTransform, b: CameraTransform): boolean {
  return Math.abs(a.x - b.x) < 0.01 && Math.abs(a.y - b.y) < 0.01 && Math.abs(a.scale - b.scale) < 1e-6;
}

/**
 * Returns a callback that applies a pan-zoom transform to center the
 * given bounding box in the viewport.  Unlike {@link useFitView}, the
 * bounds are supplied explicitly, so callers can fit to a layout's
 * final positions before the nodes have animated there.
 */
export function useFitViewToBounds() {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const getPanZoomTransform = useGetPanZoomTransform();
  const { transform } = usePanZoomControl();
  const followRef = useRef<ReturnType<typeof animate> | null>(null);

  useEffect(() => () => followRef.current?.stop(), []);

  return useCallback(
    (bounds: Box, motion: FitViewMotion = "animate") => {
      followRef.current?.stop();
      followRef.current = null;

      const graphWidth = getBoxWidth(bounds);
      const graphHeight = getBoxHeight(bounds);
      const { x: graphCenterX, y: graphCenterY } = getBoxCenter(bounds);

      const { width: viewportWidth, height: viewportHeight } = getPanZoomDimensions();

      // Calculate scale to fit content in viewport with some padding
      const padding = 100;
      const scaleX = (viewportWidth - padding * 2) / graphWidth;
      const scaleY = (viewportHeight - padding * 2) / graphHeight;
      // Don't zoom in beyond 1:1
      const scale = Math.max(Math.min(scaleX, scaleY, 1), MINIMUM_ZOOM);

      // Calculate translation to center the content
      const viewportCenterX = viewportWidth / 2;
      const viewportCenterY = viewportHeight / 2;

      if (motion !== "follow-nodes") {
        transform(
          viewportCenterX - graphCenterX * scale,
          viewportCenterY - graphCenterY * scale,
          scale,
          motion === "animate",
        );
        return;
      }

      // Nodes spread from the center of the view, so the camera zooms around it in step with them. The zoom is
      // front-loaded: zoomed out at the pace the nodes spread, the graph would grow past its final size on screen and
      // then shrink back.
      const start = getPanZoomTransform();
      const startCenterX = (viewportCenterX - start.x) / start.scale;
      const startCenterY = (viewportCenterY - start.y) / start.scale;
      let applied: CameraTransform = start;

      followRef.current = animate(0, 1, {
        ...NODE_MOVE_TRANSITION,
        onUpdate: (progress) => {
          if (!transformsEqual(getPanZoomTransform(), applied)) {
            // The user, or another fit, moved the camera.
            followRef.current?.stop();
            return;
          }

          // The nodes bounce as they settle. The camera does not.
          const zoom = 1 - (1 - Math.min(progress, 1)) ** 2;
          const nextScale = start.scale * (scale / start.scale) ** zoom;
          const centerX = startCenterX + (graphCenterX - startCenterX) * zoom;
          const centerY = startCenterY + (graphCenterY - startCenterY) * zoom;
          applied = {
            x: viewportCenterX - centerX * nextScale,
            y: viewportCenterY - centerY * nextScale,
            scale: nextScale,
          };
          transform(applied.x, applied.y, applied.scale, false);
        },
      });
    },
    [getPanZoomDimensions, getPanZoomTransform, transform],
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
