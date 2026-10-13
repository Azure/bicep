// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, createStore } from "jotai";
import type { Box } from "@/lib/math";

import { useLayoutEffect } from "react";

/**
 * Calls `applyBox` with the box now and synchronously whenever it changes. The first call happens
 * before the browser paints, so a newly mounted node never shows a frame at its default position.
 */
export function useBoxUpdate(store: ReturnType<typeof createStore>, boxAtom: Atom<Box>, applyBox: (box: Box) => void) {
  useLayoutEffect(() => {
    applyBox(store.get(boxAtom));
    return store.sub(boxAtom, () => applyBox(store.get(boxAtom)));
  }, [store, boxAtom, applyBox]);
}
