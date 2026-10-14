// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, createStore } from "jotai";
import type { Box } from "@/lib/math";

import { useEffectEvent, useLayoutEffect } from "react";
import { cancelWrite, scheduleWrite } from "./batched-writes";

/**
 * Calls `applyBox` with the node's latest box when it mounts and whenever the box changes.
 *
 * Writes are batched (see `scheduleWrite`): they land before the browser paints, so a new node is
 * never shown at its default position, without forcing a layout per node while a graph mounts.
 */
export function useBoxUpdate(store: ReturnType<typeof createStore>, boxAtom: Atom<Box>, applyBox: (box: Box) => void) {
  const onBoxUpdate = useEffectEvent(applyBox);

  useLayoutEffect(() => {
    const key = {};
    const write = () => onBoxUpdate(store.get(boxAtom));
    scheduleWrite(key, write);
    const unsubscribe = store.sub(boxAtom, () => scheduleWrite(key, write));

    return () => {
      unsubscribe();
      cancelWrite(key);
    };
  }, [store, boxAtom]);
}
