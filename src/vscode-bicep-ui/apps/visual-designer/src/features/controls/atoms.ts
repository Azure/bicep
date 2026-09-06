// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";
import { hasNodesAtom } from "@/features/status";

export interface GraphControlAvailability {
  canFitView: boolean;
  canResetGraphLayout: boolean;
  canExportGraph: boolean;
}

/**
 * Availability model for graph controls that require graph content.
 */
export const graphControlAvailabilityAtom = atom<GraphControlAvailability>((get) => {
  const hasNodes = get(hasNodesAtom);

  return {
    canFitView: hasNodes,
    canResetGraphLayout: hasNodes,
    canExportGraph: hasNodes,
  };
});
