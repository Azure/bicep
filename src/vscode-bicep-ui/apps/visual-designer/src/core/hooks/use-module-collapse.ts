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
import { useGraphApi } from "../api";
import {
  collapsedModuleIdsAtom,
  focusedNodeIdAtom,
  focusNodeAtom,
  isGraphChangeInProgressAtom,
  pendingSubgraphLayoutsAtom,
  raiseNodeAtom,
  restoringModuleIdsAtom,
  shownGraphAtom,
  sourceGraphAtom,
  undoHistoryAtom,
} from "../atoms";
import { getDescendantIds, isCollapsible, isDescendant, isRestorable, projectGraph } from "../graph-model";
import { getNodePositions, getPositionedNodeIds, setNodePositions } from "../node-positions";
import { showGraph } from "../show-graph";
import { recordStep } from "../undo-history";

export interface ModuleCollapse {
  /** Collapse or expand a shown module for the user, as one undo step. */
  toggleModuleCollapsed: (moduleId: string) => void;
  /** Collapse or expand a module without recording it, for undo and redo. */
  setModuleCollapsed: (moduleId: string, collapsed: boolean) => void;
  /**
   * Expand a registry module that is not restored yet, or whose restore failed: restore it, then open it once its
   * children arrive and are laid out. The module is in `restoringModuleIdsAtom` meanwhile. Settles when that is
   * done, or when the module has nothing to show. Never throws.
   */
  restoreModule: (moduleId: string) => Promise<void>;
}

/**
 * Collapses a module, or expands it again. It changes only what the canvas shows: no Bicep source and no
 * camera.
 *
 * A collapsing module is centered where its frame was, and its descendants keep their positions while hidden.
 * An expanding module comes to the front, and shows its descendants where they were, moved with the module if it
 * was moved. Descendants
 * that were never shown, because they were added while the module was collapsed or the module was just
 * restored, are laid out as a subgraph around the module, leaving the other nodes where they are. They stay hidden
 * until then, so the module opens around them in one morph, as it does when they were laid out before.
 */
export function useModuleCollapse(
  coordinator: GraphUpdateCoordinator<GetGraphResult>,
  getViewportCenter: () => Point,
): ModuleCollapse {
  const store = useStore();
  const api = useGraphApi();

  /**
   * Collapse or expand a module. Returns null when nothing changed, or a promise that settles once any layout the
   * change needs is done.
   */
  const changeModuleCollapsed = useCallback(
    (moduleId: string, collapsed: boolean): Promise<void> | null => {
      const collapsedModuleIds = new Set(store.get(collapsedModuleIdsAtom));
      if (collapsedModuleIds.has(moduleId) === collapsed) {
        return null;
      }

      const source = store.get(sourceGraphAtom);

      if (collapsed) {
        // A module that is not shown has no frame to replace, so it stays where it was.
        if (store.get(shownGraphAtom).nodes.has(moduleId)) {
          const containerBox = getNodeTargetBox(store, moduleId);
          if (!containerBox) {
            return null;
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
        // An opened module comes to the front, so its frame and children are not hidden under the neighbors it
        // grew over.
        store.set(raiseNodeAtom, moduleId);
      }

      store.set(collapsedModuleIdsAtom, collapsedModuleIds);

      const graph = projectGraph(source, collapsedModuleIds);
      const positions = getNodePositions(store);
      const center = positions.get(moduleId);
      const unplacedNodeIds = getPositionedNodeIds(graph).filter((nodeId) => !positions.has(nodeId));

      // Descendants shown for the first time are laid out around the module, so nothing else moves. Until then they
      // stay hidden and the module as it was, so it opens with one morph.
      const anchor =
        !collapsed &&
        center &&
        (unplacedNodeIds.length > 0 || store.get(pendingSubgraphLayoutsAtom).has(moduleId)) &&
        unplacedNodeIds.every((nodeId) => isDescendant(graph, nodeId, moduleId))
          ? center
          : undefined;
      if (anchor) {
        store.set(pendingSubgraphLayoutsAtom, (subgraphs) => new Map(subgraphs).set(moduleId, anchor));
      }

      const origins = new Map(
        !collapsed && center ? getDescendantIds(graph, moduleId).map((nodeId) => [nodeId, center]) : [],
      );
      showGraph(store, graph, { origins, getDefaultOrigin: getViewportCenter });
      const awaitsLayout = !collapsed && store.get(pendingSubgraphLayoutsAtom).has(moduleId);

      if (unplacedNodeIds.length === 0 && !awaitsLayout) {
        return Promise.resolve();
      }

      const mode = unplacedNodeIds.length > 0 && !anchor ? "auto" : "subgraph";
      return coordinator.requestGraphLayout(mode).catch((error: unknown) => {
        console.error("Visual designer layout failed:", getErrorMessage(error, "Failed to lay out the graph."));
      });
    },
    [coordinator, getViewportCenter, store],
  );

  /** Returns whether the module changed. */
  const setModuleCollapsed = useCallback(
    (moduleId: string, collapsed: boolean): boolean => changeModuleCollapsed(moduleId, collapsed) !== null,
    [changeModuleCollapsed],
  );

  /** Toggle a module as one undo step. Returns null when it did not change, or a promise as `changeModuleCollapsed`. */
  const toggleModule = useCallback(
    (moduleId: string): Promise<void> | null => {
      // The toggle records an undo step, so like a drag it is ignored while a source edit or Reset Layout is
      // in flight: a step recorded meanwhile could land above one still being replayed.
      if (
        store.get(isGraphChangeInProgressAtom) ||
        !isCollapsible(store.get(sourceGraphAtom).nodes.get(moduleId)) ||
        !store.get(shownGraphAtom).nodes.has(moduleId)
      ) {
        return null;
      }

      const collapsed = !store.get(collapsedModuleIdsAtom).has(moduleId);
      const changed = changeModuleCollapsed(moduleId, collapsed);
      if (changed) {
        store.set(undoHistoryAtom, (history) => recordStep(history, { kind: "moduleCollapse", moduleId, collapsed }));
      }

      return changed;
    },
    [changeModuleCollapsed, store],
  );

  const toggleModuleCollapsed = useCallback(
    (moduleId: string) => {
      void toggleModule(moduleId);
    },
    [toggleModule],
  );

  const restoreModule = useCallback(
    async (moduleId: string) => {
      if (
        store.get(restoringModuleIdsAtom).has(moduleId) ||
        !isRestorable(store.get(sourceGraphAtom).nodes.get(moduleId))
      ) {
        return;
      }

      store.set(restoringModuleIdsAtom, (ids) => new Set(ids).add(moduleId));

      try {
        await api.restoreModule(moduleId);
      } catch (error) {
        console.error(
          "Visual designer module restore failed:",
          getErrorMessage(error, "Failed to restore the module."),
        );
      }

      try {
        // The next graph shows what the restore supplied. Arriving children start collapsed, so the module opens as
        // the user asked, once they are laid out; when there are none, the module says why.
        await coordinator.requestUpdate();

        if (store.get(collapsedModuleIdsAtom).has(moduleId)) {
          await toggleModule(moduleId);
        }
      } catch (error) {
        console.error("Visual designer graph update failed:", getErrorMessage(error, "Failed to update the graph."));
      } finally {
        store.set(restoringModuleIdsAtom, (ids) => {
          const next = new Set(ids);
          next.delete(moduleId);
          return next;
        });
      }
    },
    [api, coordinator, store, toggleModule],
  );

  return { toggleModuleCollapsed, setModuleCollapsed, restoreModule };
}
