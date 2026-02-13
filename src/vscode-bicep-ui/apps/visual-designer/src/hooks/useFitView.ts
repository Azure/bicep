// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useGetPanZoomDimensions, usePanZoomControl } from "@vscode-bicep-ui/components";
import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import { nodesAtom } from "../features/graph-engine/atoms";

/**
 * Returns a callback that computes the bounding box of all nodes
 * and applies a pan-zoom transform to center the graph in the viewport.
 */
export function useFitView() {
  const getPanZoomDimensions = useGetPanZoomDimensions();
  const { transform } = usePanZoomControl();

  return useAtomCallback(
    useCallback(
      (get) => {
        const nodes = get(nodesAtom);
        const nodeList = Object.values(nodes);

        if (nodeList.length === 0) {
          return;
        }

        // Calculate the bounding box of all nodes
        const boxes = nodeList.map((node) => get(node.boxAtom));
        const graphMinX = Math.min(...boxes.map((box) => box.min.x));
        const graphMinY = Math.min(...boxes.map((box) => box.min.y));
        const graphMaxX = Math.max(...boxes.map((box) => box.max.x));
        const graphMaxY = Math.max(...boxes.map((box) => box.max.y));

        const graphWidth = graphMaxX - graphMinX;
        const graphHeight = graphMaxY - graphMinY;
        const graphCenterX = graphMinX + graphWidth / 2;
        const graphCenterY = graphMinY + graphHeight / 2;

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
    ),
  );
}
