// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useCallback } from "react";
import { panZoomDimensionsAtom, useStore } from "./atoms";


export interface PanZoomDimensions {
  width: number;
  height: number;
}

/**
 * A hook that provides the current width and height of the PanZoom container.
 *
 * @returns The current dimensions of the PanZoom container.
 */
export function useGetPanZoomDimensions() {
  const store = useStore();

  return useCallback(() => {
    return store.get(panZoomDimensionsAtom);
  }, [store]);
}
