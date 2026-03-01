// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useGetPanZoomDimensions } from "@vscode-bicep-ui/components";
import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import { computeLayout, type LayoutResult } from "../elk-layout";

export interface ComputeLayoutResult {
  layout: LayoutResult;
  viewport: { width: number; height: number };
}

/**
 * Returns an async callback that reads the current pan-zoom viewport
 * dimensions, computes an ELK layout for the graph, and returns both
 * the {@link LayoutResult} and the viewport that was used.
 *
 * This hook bridges the scoped pan-zoom store (viewport dimensions)
 * and the default Jotai store (graph atoms), so callers don't need
 * to thread viewport dimensions manually.
 *
 * Must be rendered inside a `PanZoomProvider`.
 */
export function useComputeLayout() {
  const getPanZoomDimensions = useGetPanZoomDimensions();

  return useAtomCallback(
    useCallback(
      async (get): Promise<ComputeLayoutResult> => {
        const viewport = getPanZoomDimensions();
        const layout = await computeLayout(get, viewport);
        return { layout, viewport };
      },
      [getPanZoomDimensions],
    ),
  );
}
