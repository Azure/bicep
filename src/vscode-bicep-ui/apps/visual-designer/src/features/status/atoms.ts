// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";
import { graphErrorCountAtom, graphHasNodesAtom } from "@/core";

export type GraphStatus = { kind: "errors"; errorCount: number } | { kind: "empty" } | { kind: "ready" };

/** What the status bar shows, derived from the latest graph. */
export const graphStatusAtom = atom<GraphStatus>((get) => {
  const errorCount = get(graphErrorCountAtom);
  if (errorCount > 0) {
    return { kind: "errors", errorCount };
  }

  if (!get(graphHasNodesAtom)) {
    return { kind: "empty" };
  }

  return { kind: "ready" };
});
