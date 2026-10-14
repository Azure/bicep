// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { RefObject } from "react";
import type { NodeKind } from "../atoms";

import { atom, useAtomValue } from "jotai";
import { useLayoutEffect, useMemo } from "react";
import { focusedNodeIdAtom, getNodeZIndex } from "../atoms";
import { cancelWrite, scheduleWrite } from "./batched-writes";

/**
 * Keeps a node's z-index in step with focus by writing it straight to the node's element.
 *
 * Focus restacks most of the graph, so reading it during render would re-render every node, and
 * all of its content, on every click. Writes are batched with the nodes' other style writes.
 */
export function useNodeZIndex(
  store: ReturnType<typeof createStore>,
  ref: RefObject<HTMLElement | null>,
  id: string,
  kind: NodeKind,
) {
  const zIndexAtom = useMemo(() => atom((get) => getNodeZIndex(id, kind, get(focusedNodeIdAtom))), [id, kind]);

  useLayoutEffect(() => {
    const key = {};
    const write = () => {
      if (ref.current) {
        ref.current.style.zIndex = String(store.get(zIndexAtom));
      }
    };

    scheduleWrite(key, write);
    const unsubscribe = store.sub(zIndexAtom, () => scheduleWrite(key, write));

    return () => {
      unsubscribe();
      cancelWrite(key);
    };
  }, [store, ref, zIndexAtom]);
}

/** Whether a node is focused. Re-renders only when this node gains or loses focus. */
export function useIsNodeFocused(id: string): boolean {
  const isFocusedAtom = useMemo(() => atom((get) => get(focusedNodeIdAtom) === id), [id]);
  return useAtomValue(isFocusedAtom);
}
