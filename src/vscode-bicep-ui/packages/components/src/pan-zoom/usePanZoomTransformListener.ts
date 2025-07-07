// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect } from "react";
import { panZoomTransformAtom, useAtomValue, useStore } from "./atoms";

/**
 * Listens to changes in the pan-zoom transform and invokes the provided listener function.
 *
 * @param listener
 * The listener function to be invoked when the pan and zoom transform changes.
 * It receives the updated x-coordinate, y-coordinate, and scale as parameters.
 */
export function usePanZoomTransformListener(listener: (x: number, y: number, scale: number) => void) {
  const store = useStore();
  const panZoomTransform = useAtomValue(panZoomTransformAtom);

  useEffect(() => {
    return store.sub(panZoomTransformAtom, () => {
      const { x, y, scale } = store.get(panZoomTransformAtom);
      listener(x, y, scale);
    });
  }, [listener, panZoomTransform, store]);
}
