// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { RefObject } from "react";

import { atom, useStore } from "jotai";
import { useContext, useLayoutEffect, useMemo } from "react";
import { GraphContext } from "../context/GraphContext";
import { getNodeZIndex } from "../paint-order";
import { cancelWrite, scheduleWrite } from "./batched-writes";

/**
 * Keeps a node's z-index in step with the paint order by writing it straight to the node's element.
 *
 * Raising a node restacks most of the graph, so reading the order during render would re-render every
 * node, and all of its content, on every click. Writes are batched with the nodes' other style writes.
 */
export function useNodeZIndex(ref: RefObject<HTMLElement | null>, id: string) {
  const store = useStore();
  const { paintOrderAtom } = useContext(GraphContext);
  const zIndexAtom = useMemo(() => atom((get) => getNodeZIndex(get(paintOrderAtom), id)), [paintOrderAtom, id]);

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
