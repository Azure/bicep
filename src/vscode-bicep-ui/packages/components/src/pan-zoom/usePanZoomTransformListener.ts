// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect } from "react";
import { panZoomTransformAtom, useStore } from "./atoms";

/**
 * Listens to changes in the pan-zoom transform and invokes the provided listener function, once with the current
 * transform and then on every change. It does not re-render the component.
 *
 * @param listener
 * The listener function to be invoked when the pan and zoom transform changes.
 * It receives the updated x-coordinate, y-coordinate, and scale as parameters.
 */
export function usePanZoomTransformListener(listener: (x: number, y: number, scale: number) => void) {
  const store = useStore();

  useEffect(() => {
    const notify = () => {
      const { x, y, scale } = store.get(panZoomTransformAtom);
      listener(x, y, scale);
    };

    notify();
    return store.sub(panZoomTransformAtom, notify);
  }, [listener, store]);
}
