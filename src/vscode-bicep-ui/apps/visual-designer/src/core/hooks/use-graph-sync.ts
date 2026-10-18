// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphResult, MeasuredGraph } from "../api";
import type { GraphActions } from "../context/GraphActionsContext";
import type { ClientGraph } from "../graph-model";
import type { GraphLayoutMode, GraphLayoutRequest, GraphLayoutResult } from "../graph-update-coordinator";
import type { NodePositions } from "../node-positions";

import { useStore } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { nodesByIdAtom, waitForNodeSizes } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  collapsedModuleIdsAtom,
  commitGraphViewAtom,
  commitPendingResourcesAtom,
  documentErrorCountAtom,
  graphViewAtom,
  isGraphChangeInProgressAtom,
  isGraphLoadedAtom,
  pendingPlacementsAtom,
  pendingRemovalNodeIdsAtom,
  reconcileModuleRestoresAtom,
  replayableSourceStepKeysAtom,
  targetScopeAtom,
  trackGraphChangeAtom,
  undoHistoryAtom,
} from "../atoms";
import { centerGraphLayout, findRestoredModules, graphChangeMayAffectLayout } from "../graph-layout";
import { clientGraphsRenderEqually, indexGraph, measuredGraphsEqual, measureGraph, subtreeGraph } from "../graph-model";
import { AUTO_LAYOUT, GraphUpdateCoordinator } from "../graph-update-coordinator";
import { updateGraphSource } from "../graph-view-state";
import { applyNodePositions, captureNodePositions } from "../node-positions";
import { getSourceStepKey, getSourceStepReferences, recordLayoutChange } from "../undo-history";
import { useApplyGraphLayout } from "./use-apply-graph-layout";
import { useModuleExpansion } from "./use-module-expansion";
import { useModuleRestore } from "./use-module-restore";
import { useResourceCreation } from "./use-resource-creation";
import { useUndoRedo } from "./use-undo-redo";

type Store = ReturnType<typeof createStore>;

/** How long document changes must pause before the graph is fetched again. */
const GRAPH_UPDATE_DELAY_MS = 200;

interface LayoutPolicy {
  /** Whether the layout is skipped when the measured graph is unchanged since the last layout of the whole graph. */
  readonly skipsUnchangedInput: boolean;
  /** `fit` frames what was laid out; `keep` leaves the camera where the user put it. */
  readonly viewport: "fit" | "keep";
  /** Whether the layout is one undoable step. */
  readonly recordsUndoStep: boolean;
}

/**
 * What each kind of layout may disturb. Where it puts its result follows from what it covers: the whole graph is
 * centered in the viewport, and a module where it was before it expanded.
 *
 * - `auto` follows a change to the graph. It skips when nothing was resized, and frames what arrived.
 * - `reset` is the user re-tidying the graph they are looking at, so it always runs: the measurements are unchanged
 *   when the user has only dragged nodes, which is exactly when it must still run. It keeps their camera and is one
 *   undoable step.
 * - `module` places the contents of a restored module, leaving the camera and the rest of the graph where they are.
 */
const LAYOUT_POLICIES: Readonly<Record<GraphLayoutMode | "module", LayoutPolicy>> = {
  auto: { skipsUnchangedInput: true, viewport: "fit", recordsUndoStep: false },
  reset: { skipsUnchangedInput: false, viewport: "keep", recordsUndoStep: true },
  module: { skipsUnchangedInput: false, viewport: "keep", recordsUndoStep: false },
};

/**
 * The box of every mounted atomic node, keyed by id. A container is sized from its contents, so it has no size of its
 * own to lay out -- and the box it draws is in motion while it grows or shrinks.
 */
function measureNodes(store: Store): Map<string, Box> {
  const renderedNodes = store.get(nodesByIdAtom);
  const boxes = new Map<string, Box>();

  for (const [nodeId, rendered] of Object.entries(renderedNodes)) {
    if (rendered.kind === "atomic") {
      boxes.set(nodeId, store.get(rendered.boxAtom));
    }
  }

  return boxes;
}

export interface GraphSync extends GraphActions {
  /** The Bicep file may have changed: withdraw what depended on it and fetch the graph once edits pause. */
  handleDocumentChange: () => void;
}

/**
 * Keeps the client graph in step with the Bicep file: graph updates, layout, and recording node
 * drags and Reset Layout in the undo history. Mounted once, by `GraphActionsProvider`.
 *
 * `graphViewAtom` holds the accepted source and local projection. Both host updates here and local module toggles
 * commit through `commitGraphViewAtom`, which mounts the projection and prunes history in one transaction.
 * Source restoration, creation, and undo/redo request updates through the coordinator.
 *
 * Ordering lives in `GraphUpdateCoordinator`, which has no React dependency and is unit tested
 * directly — the rules there govern hazards that are impractical to force end to end.
 */
export function useGraphSync(getViewportCenter: () => Point, fitViewToBounds: (bounds: Box) => void): GraphSync {
  const store = useStore();
  const { revealGraph, applyGraphLayout, animateNodePositions, stopNodeAnimations, captureSettledNodePositions } =
    useApplyGraphLayout();
  const api = useGraphApi();
  const [coordinator] = useState(() => new GraphUpdateCoordinator<GetGraphResult>());
  const createResourceAt = useResourceCreation(coordinator);
  const { undo, redo } = useUndoRedo(coordinator, { animateNodePositions, stopNodeAnimations });

  const lastWholeGraphLayoutInputRef = useRef<MeasuredGraph | null>(null);

  const restoreModuleSource = useModuleRestore(coordinator);
  const {
    reconcileExpansions,
    prepareExpansions,
    getExpansionAnchor,
    revealExpansions,
    toggleModuleCollapsed: toggleCachedModule,
  } = useModuleExpansion(coordinator, { getViewportCenter, stopNodeAnimations });

  const toggleModuleCollapsed = useCallback(
    (nodeId: string) => {
      if (store.get(isGraphChangeInProgressAtom)) {
        return;
      }
      if (store.get(graphViewAtom).source.nodes.get(nodeId)?.requiresRestore) {
        restoreModuleSource(nodeId);
      } else {
        toggleCachedModule(nodeId);
      }
    },
    [restoreModuleSource, store, toggleCachedModule],
  );

  /** For each node drag in progress, the positions of the dragged subtree when it started moving. */
  const dragStartPositionsRef = useRef(new Map<string, NodePositions>());

  /** The pending graph update for a document change, which waits for a pause in the edits. */
  const updateTimerRef = useRef<number | undefined>(undefined);
  useEffect(() => () => window.clearTimeout(updateTimerRef.current), []);

  const handleNodeDragStart = useCallback(
    (nodeId: string) => {
      stopNodeAnimations();
      dragStartPositionsRef.current.set(nodeId, captureNodePositions(store, nodeId));
    },
    [stopNodeAnimations, store],
  );

  const handleNodeDragEnd = useCallback(
    (nodeId: string) => {
      const positionsBefore = dragStartPositionsRef.current.get(nodeId);
      dragStartPositionsRef.current.delete(nodeId);

      if (positionsBefore) {
        const positionsAfter = captureNodePositions(store, nodeId);
        store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, positionsAfter));
      }
    },
    [store],
  );

  const fetchUpdate = useCallback(
    () => api.getGraph(getSourceStepReferences(store.get(undoHistoryAtom))),
    [api, store],
  );

  const applyUpdate = useCallback(
    async (response: GetGraphResult): Promise<{ layouts: GraphLayoutRequest[] }> => {
      // Offer only the source steps the host confirmed for this update. The rest stay in the history,
      // disabled, since an edit in progress can make a step replayable again. The coordinator never
      // applies a response fetched before the latest document change, so these match the current file.
      store.set(replayableSourceStepKeysAtom, new Set((response.replayableSourceSteps ?? []).map(getSourceStepKey)));

      if (!response.graph) {
        return { layouts: [] };
      }

      store.set(isGraphLoadedAtom, true);
      store.set(targetScopeAtom, response.targetScope);
      store.set(documentErrorCountAtom, response.errorCount);
      const previousView = store.get(graphViewAtom);
      const previousSource = previousView.source;
      const previousRendered = previousView.rendered;
      const view = updateGraphSource(previousView, indexGraph(response.graph));
      const { source, rendered: graph } = view;
      store.set(reconcileModuleRestoresAtom, source);
      reconcileExpansions(previousSource, source);

      const placements = store.get(pendingPlacementsAtom);
      const pendingRemovalNodeIds = store.get(pendingRemovalNodeIdsAtom);
      const restoredModuleIds = findRestoredModules(previousRendered, graph);
      const layoutRequired =
        graph.nodes.size > 0 &&
        graphChangeMayAffectLayout(previousRendered, graph, {
          explicitlyPlacedNodeIds: new Set(placements.keys()),
          pendingRemovalNodeIds,
          restoredModuleIds,
        });
      // A restored module expands in place, laid out on its own, unless the whole graph needs laying out anyway.
      const layouts: GraphLayoutRequest[] = layoutRequired
        ? [AUTO_LAYOUT]
        : [...restoredModuleIds].map((nodeId) => ({ scope: "module", nodeId }));

      // A removal the designer was expecting has happened once the node is gone.
      store.set(pendingRemovalNodeIdsAtom, new Set([...pendingRemovalNodeIds].filter((id) => source.nodes.has(id))));

      const newNodeOrigins = new Map(
        [...placements].filter(([nodeId]) => graph.nodes.has(nodeId) && !previousSource.nodes.has(nodeId)),
      );

      // Apply the new topology. Visibility is preserved for incremental edits (so nodes animate in
      // place) and gated for major changes; positions arrive with the layout. A restored module's contents start
      // hidden where the module was until its own layout places them.
      const entrances = prepareExpansions(graph);
      store.set(commitGraphViewAtom, {
        view,
        viewportCenter: getViewportCenter(),
        options: { newNodeOrigins, entrances, awaitsLayout: layoutRequired },
      });
      if (graph.nodes.size === 0) {
        lastWholeGraphLayoutInputRef.current = null;
      }

      if (newNodeOrigins.size > 0) {
        store.set(commitPendingResourcesAtom, new Set(newNodeOrigins.keys()));

        if (!layoutRequired) {
          // An explicitly placed node is already where the user dropped it, so no layout is owed --
          // but the graph may still be behind the visibility gate, so reveal it.
          await revealGraph();
        }
      }

      return { layouts };
    },
    [getViewportCenter, prepareExpansions, reconcileExpansions, revealGraph, store],
  );

  /**
   * Lay out what a request covers: measure it, have the host lay it out, and place the result as its policy says.
   * What to retry or fall back to afterwards is the coordinator's decision, made from the result.
   */
  const layOut = useCallback(
    async (request: GraphLayoutRequest): Promise<GraphLayoutResult> => {
      const policy = LAYOUT_POLICIES[request.scope === "graph" ? request.mode : "module"];
      const cover = (shown: ClientGraph) => (request.scope === "graph" ? shown : subtreeGraph(shown, request.nodeId));

      // Read what is shown once its sizes have settled: a collapse or expand can change it meanwhile.
      await waitForNodeSizes(store);

      const graph = store.get(graphViewAtom).rendered;
      // A module is laid out on its own only while its contents wait to be placed in its container.
      const expansionAnchor = request.scope === "module" ? getExpansionAnchor(request.nodeId) : undefined;

      if (graph.nodes.size === 0 || (request.scope === "module" && !expansionAnchor)) {
        return "completed";
      }

      const target = cover(graph);
      const measuredGraph = measureGraph(target, measureNodes(store));

      if (policy.skipsUnchangedInput && measuredGraphsEqual(lastWholeGraphLayoutInputRef.current, measuredGraph)) {
        // Nothing was resized since the last layout, so the positions still hold. Reveal the graph in
        // case it is still behind the visibility gate.
        await revealGraph();
        return "completed";
      }

      const response = await api.layoutGraph({
        graph: measuredGraph,
        collapsedNodeIds: [...store.get(collapsedModuleIdsAtom)],
        rootNodeId: request.scope === "module" ? request.nodeId : undefined,
      });

      // A local collapse or expand can change what is shown while the request is in flight. The response describes
      // what was measured, so it must not move nodes in what is shown now -- but the layout is still owed, so it runs
      // again, against what is shown then.
      if (!clientGraphsRenderEqually(cover(store.get(graphViewAtom).rendered), target)) {
        return "graphChanged";
      }

      if (response.status === "graphChanged") {
        return "graphChanged";
      }

      if (response.status === "layoutFailed") {
        // No usable layout — reveal the graph as-is so it isn't stuck hidden.
        await revealGraph();
        return "layoutFailed";
      }

      // An undoable layout is one step from where the nodes were settling: undoing it must not stop partway through
      // an animation that it interrupted.
      const positionsBefore = policy.recordsUndoStep ? captureSettledNodePositions() : null;
      if (positionsBefore) {
        stopNodeAnimations();
      }

      // The layout puts its top-left corner at the origin and reports its size, so centering it on the anchor -- the
      // viewport's center, or where a module was before it expanded -- needs no measuring.
      const { positions, bounds } = centerGraphLayout(
        response.positions,
        response.bounds,
        expansionAnchor ?? getViewportCenter(),
      );

      if (policy.viewport === "fit" && bounds) {
        // Fit the viewport to the laid-out bounds before the nodes settle there.
        fitViewToBounds(bounds);
      }

      if (expansionAnchor) {
        // The module's contents stay hidden until it is revealed, so they jump into place.
        applyNodePositions(store, positions);
      } else {
        lastWholeGraphLayoutInputRef.current = measuredGraph;
        await applyGraphLayout(positions);
      }

      if (positionsBefore) {
        store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, positions));
      }

      return "completed";
    },
    [
      api,
      applyGraphLayout,
      captureSettledNodePositions,
      fitViewToBounds,
      getExpansionAnchor,
      getViewportCenter,
      revealGraph,
      stopNodeAnimations,
      store,
    ],
  );

  useEffect(() => {
    coordinator.setOperations({ fetchUpdate, applyUpdate, runGraphLayout: layOut, completeLayouts: revealExpansions });
  }, [applyUpdate, coordinator, fetchUpdate, layOut, revealExpansions]);

  const handleDocumentChange = useCallback(() => {
    // Replay confirmations describe the previous content, so none holds until the update for this change.
    store.set(replayableSourceStepKeysAtom, new Set<string>());
    coordinator.invalidateUpdate();

    // Edits arrive in bursts, so fetch once they pause rather than showing every intermediate graph.
    window.clearTimeout(updateTimerRef.current);
    updateTimerRef.current = window.setTimeout(() => {
      coordinator.requestUpdate().catch((error: unknown) => {
        console.error("Visual designer graph update failed:", getErrorMessage(error, "Failed to update the graph."));
      });
    }, GRAPH_UPDATE_DELAY_MS);
  }, [coordinator, store]);

  const resetGraphLayout = useCallback(
    () =>
      store
        .set(trackGraphChangeAtom, () => coordinator.requestResetGraphLayout())
        .catch((error: unknown) => {
          console.error(
            "Visual designer layout reset failed:",
            getErrorMessage(error, "Failed to reset the graph layout."),
          );
        }),
    [coordinator, store],
  );

  return {
    handleDocumentChange,
    resetGraphLayout,
    createResourceAt,
    toggleModuleCollapsed,
    handleNodeDragStart,
    handleNodeDragEnd,
    undo,
    redo,
  };
}
