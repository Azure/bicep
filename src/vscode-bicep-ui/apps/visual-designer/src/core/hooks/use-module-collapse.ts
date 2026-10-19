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
import {
  collapsedModuleIdsAtom,
  focusedNodeIdAtom,
  focusNodeAtom,
  isGraphChangeInProgressAtom,
  shownGraphAtom,
  sourceGraphAtom,
  undoHistoryAtom,
} from "../atoms";
import { getDescendantIds, isCollapsible, isDescendant, projectGraph } from "../graph-model";
import { getNodePositions, setNodePositions } from "../node-positions";
import { showGraph } from "../show-graph";
import { recordStep } from "../undo-history";

export interface ModuleCollapse {
  /** Collapse or expand a shown module for the user, as one undo step. */
  toggleModuleCollapsed: (moduleId: string) => void;
  /** Collapse or expand a module without recording it, for undo and redo. */
  setModuleCollapsed: (moduleId: string, collapsed: boolean) => void;
}

/**
 * Collapses a module into a card, or expands it again. It changes only what the canvas shows: no Bicep
 * source, no layout, and no camera.
 *
 * A collapsing module's card takes the place of its container, centered where the container was, and its
 * contents keep their positions while hidden. An expanding module shows its contents where they were,
 * moved with the card if the card was moved. Contents that were never shown, because they were added
 * while the module was collapsed, appear at the card and are laid out.
 */
export function useModuleCollapse(
  coordinator: GraphUpdateCoordinator<GetGraphResult>,
  getViewportCenter: () => Point,
): ModuleCollapse {
  const store = useStore();

  /** Returns whether the module changed. */
  const setModuleCollapsed = useCallback(
    (moduleId: string, collapsed: boolean): boolean => {
      const collapsedModuleIds = new Set(store.get(collapsedModuleIdsAtom));
      if (collapsedModuleIds.has(moduleId) === collapsed) {
        return false;
      }

      const source = store.get(sourceGraphAtom);

      if (collapsed) {
        // A module that is not shown has no container to replace, so its card stays where it was.
        if (store.get(shownGraphAtom).nodes.has(moduleId)) {
          const containerBox = getNodeTargetBox(store, moduleId);
          if (!containerBox) {
            return false;
          }

          setNodePositions(
            store,
            new Map(getNodePositions(store)).set(moduleId, getBoxCenter(containerBox)),
            "instant",
          );
        }

        collapsedModuleIds.add(moduleId);

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
        !collapsed && cardCenter ? getDescendantIds(graph, moduleId).map((nodeId) => [nodeId, cardCenter]) : [],
      );

      if (showGraph(store, graph, { origins, getDefaultOrigin: getViewportCenter }).length > 0) {
        coordinator.requestGraphLayout("auto").catch((error: unknown) => {
          console.error("Visual designer layout failed:", getErrorMessage(error, "Failed to lay out the graph."));
        });
      }

      return true;
    },
    [coordinator, getViewportCenter, store],
  );

  const toggleModuleCollapsed = useCallback(
    (moduleId: string) => {
      // The toggle records an undo step, so like a drag it is ignored while a source edit or Reset Layout is
      // in flight: a step recorded meanwhile could land above one still being replayed.
      if (
        store.get(isGraphChangeInProgressAtom) ||
        !isCollapsible(store.get(sourceGraphAtom).nodes.get(moduleId)) ||
        !store.get(shownGraphAtom).nodes.has(moduleId)
      ) {
        return;
      }

      const collapsed = !store.get(collapsedModuleIdsAtom).has(moduleId);
      if (setModuleCollapsed(moduleId, collapsed)) {
        store.set(undoHistoryAtom, (history) => recordStep(history, { kind: "moduleCollapse", moduleId, collapsed }));
      }
    },
    [setModuleCollapsed, store],
  );

  return { toggleModuleCollapsed, setModuleCollapsed };
}
