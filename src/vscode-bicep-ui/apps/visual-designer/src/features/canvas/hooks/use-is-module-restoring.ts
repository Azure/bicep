// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom, useAtomValue } from "jotai";
import { useMemo } from "react";
import { restoringModuleIdsAtom } from "@/core";

/** Whether a module is being restored. Re-renders only when this module starts or stops restoring. */
export function useIsModuleRestoring(id: string): boolean {
  const isRestoringAtom = useMemo(() => atom((get) => get(restoringModuleIdsAtom).has(id)), [id]);
  return useAtomValue(isRestoringAtom);
}
