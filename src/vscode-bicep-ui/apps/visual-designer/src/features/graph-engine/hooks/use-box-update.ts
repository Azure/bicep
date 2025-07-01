// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, createStore } from "jotai";
import type { Box } from "../../../utils/math";

import { useEffect } from "react";

export function useBoxUpdate(
  store: ReturnType<typeof createStore>,
  boxAtom: Atom<Box>,
  onBoxUpdate: (box: Box) => void,
) {
  useEffect(() => {
    onBoxUpdate(store.get(boxAtom));
    return store.sub(boxAtom, () => onBoxUpdate(store.get(boxAtom)));
  }, [store, boxAtom, onBoxUpdate]);
}
