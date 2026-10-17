// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";
import { documentErrorCountAtom, graphHasNodesAtom, isGraphLoadedAtom } from "@/core";

export type GraphStatus =
  { kind: "loading" } | { kind: "errors"; errorCount: number } | { kind: "empty" } | { kind: "ready" };

/** What the status bar shows, derived from the latest graph. */
export const graphStatusAtom = atom<GraphStatus>((get) => {
  if (!get(isGraphLoadedAtom)) {
    return { kind: "loading" };
  }

  const errorCount = get(documentErrorCountAtom);
  if (errorCount > 0) {
    return { kind: "errors", errorCount };
  }

  if (!get(graphHasNodesAtom)) {
    return { kind: "empty" };
  }

  return { kind: "ready" };
});
