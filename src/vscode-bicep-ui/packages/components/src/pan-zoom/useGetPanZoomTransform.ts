// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useCallback } from "react";
import { panZoomTransformAtom, useStore } from "./atoms";

/**
 * A hook that returns a function to get the current pan-zoom transform object.
 * @returns The function to get the current pan-zoom transform object.
 */
export function useGetPanZoomTransform() {
  const store = useStore();

  return useCallback(() => {
    return store.get(panZoomTransformAtom);
  }, [store]);
}
