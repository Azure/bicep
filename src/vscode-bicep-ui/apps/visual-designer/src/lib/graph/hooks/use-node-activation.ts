// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom } from "jotai";
import type { RefObject } from "react";

import { useStore } from "jotai";
import { useEffect } from "react";
import { nodeConfigAtom } from "../atoms";

/**
 * Calls the configured `onNodeActivate` when a node is double-clicked.
 *
 * What activation *means* belongs to the product, not the engine: `lib/graph` reports that a node was
 * activated and lets `nodeConfigAtom` decide what happens. This is the same injection seam
 * `renderContent` uses, and it is what keeps this module free of any host-protocol knowledge.
 *
 * Uses a native listener rather than an `onDoubleClick` prop so it can `stopPropagation()` before
 * d3-zoom's handler on the PanZoom ancestor sees the event.
 */
export function useNodeActivation(ref: RefObject<HTMLElement | null>, id: string, dataAtom: Atom<unknown>) {
  const store = useStore();

  useEffect(() => {
    const element = ref.current;

    if (!element) {
      return;
    }

    const handler = (event: MouseEvent) => {
      event.stopPropagation();
      store.get(nodeConfigAtom).onNodeActivate?.(id, store.get(dataAtom));
    };

    element.addEventListener("dblclick", handler);

    return () => element.removeEventListener("dblclick", handler);
  }, [dataAtom, id, ref, store]);
}
