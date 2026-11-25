// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CanvasNodeData } from "@/core";

import { atom, useAtomValue } from "jotai";
import { useMemo } from "react";
import { nodesByIdAtom } from "@/lib/graph";

/** What the canvas shows for a node, or undefined when it is not shown. Re-renders only when that changes. */
export function useCanvasNodeData(id: string | null): CanvasNodeData | undefined {
  const dataAtom = useMemo(
    () =>
      atom((get) => {
        const node = id === null ? undefined : get(nodesByIdAtom)[id];
        return node ? (get(node.dataAtom) as CanvasNodeData) : undefined;
      }),
    [id],
  );

  return useAtomValue(dataAtom);
}
