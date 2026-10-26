// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom, useAtomValue } from "jotai";
import { useMemo } from "react";
import { focusedNodeIdAtom } from "@/core";

/** Whether a node is focused. Re-renders only when this node gains or loses focus. */
export function useIsNodeFocused(id: string): boolean {
  const isFocusedAtom = useMemo(() => atom((get) => get(focusedNodeIdAtom) === id), [id]);
  return useAtomValue(isFocusedAtom);
}
