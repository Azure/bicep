// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphResult, MeasuredGraph } from "../api";
import type { GraphActions } from "../context/GraphActionsContext";
import type { ClientGraph } from "../graph-model";
import type { GraphLayoutMode, GraphLayoutResult } from "../graph-update-coordinator";
import type { NodePositions } from "../node-positions";

import { useStore } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { nodesByIdAtom } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  commitPendingResourcesAtom,
  documentErrorCountAtom,
  pendingPlacementsAtom,
  pendingRemovalNodeIdsAtom,
  replayableSourceStepKeysAtom,
  targetScopeAtom,
  undoHistoryAtom,
} from "../atoms";
import { centerGraphLayout, graphChangeMayAffectLayout } from "../graph-layout";
import {
  clientGraphsRenderEqually,
  EMPTY_CLIENT_GRAPH,
  indexGraph,
  measuredGraphsEqual,
  measureGraph,
} from "../graph-model";
import { GraphUpdateCoordinator } from "../graph-update-coordinator";
import { captureNodePositions, getAtomicNodeIds } from "../node-positions";
import { forgetRemovedNodes, getSourceStepKey, getSourceStepReferences, recordLayoutChange } from "../undo-history";
import { useApplyGraph } from "./use-apply-graph";
import { useApplyGraphLayout } from "./use-apply-graph-layout";
import { useResourceCreation } from "./use-resource-creation";
import { useTrackGraphChange } from "./use-track-graph-change";
import { useUndoRedo } from "./use-undo-redo";

type Store = ReturnType<typeof createStore>;

/** How long document changes must pause before the graph is fetched again. */
const GRAPH_UPDATE_DELAY_MS = 200;

function waitForAnimationFrame(): Promise<void> {
  return new Promise((resolve) => requestAnimationFrame(() => resolve()));
}

/** Snapshot the measured box of every mounted node, keyed by id. */
function measureNodes(store: Store): Map<string, Box> {
  const renderedNodes = store.get(nodesByIdAtom);
  const boxes = new Map<string, Box>();

  for (const [nodeId, rendered] of Object.entries(renderedNodes)) {
    boxes.set(nodeId, store.get(rendered.boxAtom));
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
 * Resource creation and undo/redo live in their own hooks. They share state with this one only
 * through atoms: the undo history, the pending resources and removals, and the graph changes in
 * progress.
 *
 * Ordering lives in `GraphUpdateCoordinator`, which has no React dependency and is unit tested
 * directly — the rules there govern hazards that are impractical to force end to end.
 */
export function useGraphSync(getViewportCenter: () => Point, fitViewToBounds: (bounds: Box) => void): GraphSync {
  const store = useStore();
  const applyGraph = useApplyGraph(getViewportCenter);
  const { applyGraphLayout, animateNodePositions, stopNodeAnimations } = useApplyGraphLayout();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();
  const [coordinator] = useState(() => new GraphUpdateCoordinator<GetGraphResult>());
  const createResourceAt = useResourceCreation(coordinator);
  const { undo, redo } = useUndoRedo(coordinator, { animateNodePositions, stopNodeAnimations });

  /**
   * The two graphs the client holds: the latest graph from the host, and the last measured graph it
   * submitted for layout — kept so a pass can skip layout when measured sizes are unchanged.
   */
  const clientGraphsRef = useRef<{ graph: ClientGraph; measured: MeasuredGraph | null }>({
    graph: EMPTY_CLIENT_GRAPH,
    measured: null,
  });

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
    async (response: GetGraphResult): Promise<{ layoutRequired: boolean }> => {
      // Offer only the source steps the host confirmed for this update. The rest stay in the history,
      // disabled, since an edit in progress can make a step replayable again. The coordinator never
      // applies a response fetched before the latest document change, so these match the current file.
      store.set(replayableSourceStepKeysAtom, new Set((response.replayableSourceSteps ?? []).map(getSourceStepKey)));

      if (!response.graph) {
        return { layoutRequired: false };
      }

      store.set(targetScopeAtom, response.targetScope);
      store.set(documentErrorCountAtom, response.errorCount);
      const previous = clientGraphsRef.current.graph;
      const graph = indexGraph(response.graph);
      clientGraphsRef.current.graph = graph;

      const placements = store.get(pendingPlacementsAtom);
      const pendingRemovalNodeIds = store.get(pendingRemovalNodeIdsAtom);
      const layoutRequired =
        graph.nodes.size > 0 &&
        graphChangeMayAffectLayout(previous, graph, {
          explicitlyPlacedNodeIds: new Set(placements.keys()),
          pendingRemovalNodeIds,
        });

      // A removal the designer was expecting has happened once the node is gone.
      store.set(pendingRemovalNodeIdsAtom, new Set([...pendingRemovalNodeIds].filter((id) => graph.nodes.has(id))));

      const newNodeOrigins = new Map(
        [...placements].filter(([nodeId]) => graph.nodes.has(nodeId) && !previous.nodes.has(nodeId)),
      );

      if (graph.nodes.size === 0) {
        clientGraphsRef.current.measured = null;
      }

      // Nothing the canvas shows has changed, so leave the mounted nodes alone rather than tearing the
      // graph down and re-laying it out. Most keystrokes land here.
      if (clientGraphsRenderEqually(previous, graph)) {
        return { layoutRequired };
      }
      // Apply the new topology. Visibility is preserved for incremental edits (so nodes animate in
      // place) and gated for major changes; positions arrive with the layout.
      const previousNodesById = store.get(nodesByIdAtom);
      applyGraph(graph, newNodeOrigins);
      if (store.get(nodesByIdAtom) !== previousNodesById) {
        const liveNodeIds = new Set([...getAtomicNodeIds(store), ...placements.keys()]);
        store.set(undoHistoryAtom, (history) => forgetRemovedNodes(history, liveNodeIds));
      }

      if (newNodeOrigins.size > 0) {
        store.set(commitPendingResourcesAtom, new Set(newNodeOrigins.keys()));

        if (!layoutRequired) {
          // An explicitly placed node is already where the user dropped it, so no layout is owed --
          // but the graph may still be behind the visibility gate, so reveal it.
          await applyGraphLayout(new Map());
        }
      }

      return { layoutRequired };
    },
    [applyGraph, applyGraphLayout, store],
  );

  const runGraphLayout = useCallback(
    async (mode: GraphLayoutMode): Promise<GraphLayoutResult> => {
      const isReset = mode === "reset";
      const graph = clientGraphsRef.current.graph;

      if (graph.nodes.size === 0) {
        clientGraphsRef.current.measured = null;
        return "completed";
      }

      await waitForAnimationFrame();

      const measuredGraph = measureGraph(graph, measureNodes(store));

      if (!isReset && measuredGraphsEqual(clientGraphsRef.current.measured, measuredGraph)) {
        // Nothing was resized since the last layout, so the positions still hold. Reveal the graph in
        // case it is still behind the visibility gate. A reset skips this: the measurements are
        // unchanged when the user has only dragged nodes, which is exactly when it must still run.
        await applyGraphLayout(new Map());
        return "completed";
      }

      const layoutResponse = await api.layoutGraph(measuredGraph);

      if (layoutResponse.status === "graphChanged") {
        return "graphChanged";
      }

      if (layoutResponse.status === "layoutFailed") {
        // No usable layout — reveal the graph as-is so it isn't stuck hidden.
        await applyGraphLayout(new Map());
        return "completed";
      }

      const { positions: centeredPositions, bounds } = centerGraphLayout(
        layoutResponse.positions,
        layoutResponse.bounds,
        getViewportCenter(),
      );
      clientGraphsRef.current.measured = measuredGraph;

      if (!isReset) {
        // Fit the viewport to the server-computed graph bounds before the nodes settle there.
        if (bounds) {
          fitViewToBounds(bounds);
        }
        await applyGraphLayout(centeredPositions);
        return "completed";
      }

      // A reset re-arranges the graph the user is already looking at, so it keeps their camera. It is
      // one undoable step, from where the nodes are now (stopping any animation) to the new layout.
      stopNodeAnimations();
      const positionsBefore = captureNodePositions(store);
      await applyGraphLayout(centeredPositions);
      store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, centeredPositions));

      return "completed";
    },
    [api, applyGraphLayout, fitViewToBounds, getViewportCenter, stopNodeAnimations, store],
  );

  useEffect(() => {
    coordinator.setOperations({ fetchUpdate, applyUpdate, runGraphLayout });
  }, [applyUpdate, coordinator, fetchUpdate, runGraphLayout]);

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
      trackGraphChange(() => coordinator.requestResetGraphLayout()).catch((error: unknown) => {
        console.error(
          "Visual designer layout reset failed:",
          getErrorMessage(error, "Failed to reset the graph layout."),
        );
      }),
    [coordinator, trackGraphChange],
  );

  return {
    handleDocumentChange,
    resetGraphLayout,
    createResourceAt,
    handleNodeDragStart,
    handleNodeDragEnd,
    undo,
    redo,
  };
}
