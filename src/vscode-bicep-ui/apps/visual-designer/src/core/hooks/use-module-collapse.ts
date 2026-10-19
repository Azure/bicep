// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { GetGraphResult } from "../api";
import type { GraphUpdateCoordinator } from "../graph-update-coordinator";

import { useStore } from "jotai";
import { useCallback } from "react";
import { getNodeTargetBox } from "@/lib/graph";
import { getBoxCenter } from "@/lib/math";
import { getErrorMessage } from "@/utils";
import { collapsedModuleIdsAtom, focusedNodeIdAtom, focusNodeAtom, shownGraphAtom, sourceGraphAtom } from "../atoms";
import { getDescendantIds, isCollapsible, isDescendant, projectGraph } from "../graph-model";
import { getNodePositions, setNodePositions } from "../node-positions";
import { showGraph } from "../show-graph";

/**
 * Returns a function that collapses a module into a card, or expands it again. It changes only what the
 * canvas shows: no Bicep source, no layout, no camera, and no undo step.
 *
 * A collapsing module's card takes the place of its container, centered where the container was, and its
 * contents keep their positions while hidden. An expanding module shows its contents where they were,
 * moved with the card if the card was moved. Contents that were never shown, because they were added
 * while the module was collapsed, appear at the card and are laid out.
 */
export function useModuleCollapse(
  coordinator: GraphUpdateCoordinator<GetGraphResult>,
  getViewportCenter: () => Point,
): (moduleId: string) => void {
  const store = useStore();

  return useCallback(
    (moduleId: string) => {
      const source = store.get(sourceGraphAtom);
      const shown = store.get(shownGraphAtom);

      if (!isCollapsible(source.nodes.get(moduleId)) || !shown.nodes.has(moduleId)) {
        return;
      }

      const collapsedModuleIds = new Set(store.get(collapsedModuleIdsAtom));
      const isCollapsing = !collapsedModuleIds.has(moduleId);

      if (isCollapsing) {
        const containerBox = getNodeTargetBox(store, moduleId);
        if (!containerBox) {
          return;
        }

        collapsedModuleIds.add(moduleId);
        setNodePositions(store, new Map(getNodePositions(store)).set(moduleId, getBoxCenter(containerBox)), "instant");

        const focusedNodeId = store.get(focusedNodeIdAtom);
        if (focusedNodeId !== null && isDescendant(source, focusedNodeId, moduleId)) {
          store.set(focusNodeAtom, moduleId);
        }
      } else {
        collapsedModuleIds.delete(moduleId);
      }

      store.set(collapsedModuleIdsAtom, collapsedModuleIds);

      const graph = projectGraph(source, collapsedModuleIds);
      const cardCenter = getNodePositions(store).get(moduleId);
      const origins = new Map(
        !isCollapsing && cardCenter ? getDescendantIds(graph, moduleId).map((nodeId) => [nodeId, cardCenter]) : [],
      );

      if (showGraph(store, graph, { origins, getDefaultOrigin: getViewportCenter }).length > 0) {
        coordinator.requestGraphLayout("auto").catch((error: unknown) => {
          console.error("Visual designer layout failed:", getErrorMessage(error, "Failed to lay out the graph."));
        });
      }
    },
    [coordinator, getViewportCenter, store],
  );
}
