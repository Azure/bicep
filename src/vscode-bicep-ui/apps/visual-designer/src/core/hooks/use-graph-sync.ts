// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphUpdateResult, GraphPatch, NodeLayout, RenderedGraph } from "../api";
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
  pendingPlacementsAtom,
  pendingRemovalNodeIdsAtom,
  resourceNodeIsCommittingAtomFamily,
  targetScopeAtom,
  undoHistoryAtom,
} from "../atoms";
import { centerGraphLayout, extractGraphLayout, patchMayAffectLayout } from "../graph-layout";
import { applyGraphPatch, buildRenderedGraph, createClientGraph, renderedGraphsEqual } from "../graph-model";
import { GraphUpdateCoordinator } from "../graph-update-coordinator";
import { captureNodePositions, getAtomicNodeIds } from "../node-positions";
import { forgetRemovedNodes, recordLayoutChange } from "../undo-history";
import { useApplyGraph } from "./use-apply-graph";
import { useApplyGraphLayout } from "./use-apply-graph-layout";
import { useResourceCreation } from "./use-resource-creation";
import { useTrackGraphChange } from "./use-track-graph-change";
import { useUndoRedo } from "./use-undo-redo";

type Store = ReturnType<typeof createStore>;

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

/** The node a patch adds, removes, or updates, if any. */
function getPatchedNodeId(patch: GraphPatch): string | null {
  switch (patch.op) {
    case "addNode":
      return patch.node.id;
    case "removeNode":
    case "updateNode":
      return patch.nodeId;
    default:
      return null;
  }
}

export interface GraphSync extends GraphActions {
  /** The Bicep file may have changed: fetch and apply the graph delta. */
  requestGraphUpdate: () => Promise<void>;
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
  const [coordinator] = useState(() => new GraphUpdateCoordinator<GetGraphUpdateResult>());
  const createResourceAt = useResourceCreation(coordinator);
  const { undo, redo } = useUndoRedo(coordinator, { animateNodePositions, stopNodeAnimations });

  /**
   * The two graphs the client holds: its copy of the server's canonical graph, and the last
   * `RenderedGraph` it submitted for layout — kept so a pass can skip layout when measured sizes are
   * unchanged.
   */
  const clientGraphsRef = useRef<{ graph: ClientGraph; rendered: RenderedGraph | null }>({
    graph: createClientGraph(),
    rendered: null,
  });

  /** For each node drag in progress, the positions of the dragged subtree when it started moving. */
  const dragStartPositionsRef = useRef(new Map<string, NodePositions>());

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

  const fetchUpdate = useCallback(() => {
    const graph = clientGraphsRef.current.graph;
    const current: RenderedGraph | null =
      graph.nodes.size === 0 ? null : buildRenderedGraph(graph, measureNodes(store));

    return api.fetchUpdate(current);
  }, [api, store]);

  const applyUpdate = useCallback(
    async (response: GetGraphUpdateResult): Promise<{ layoutRequired: boolean }> => {
      store.set(targetScopeAtom, response.targetScope);
      const graph = clientGraphsRef.current.graph;
      const placements = store.get(pendingPlacementsAtom);
      const pendingRemovalNodeIds = new Set(store.get(pendingRemovalNodeIdsAtom));
      const nodeLayouts = new Map<string, NodeLayout>();
      const newNodeOrigins = new Map<string, Point>();
      const explicitlyPlacedNodeIds = new Set(placements.keys());
      let layoutMayBeStale = false;

      for (const patch of response.patches) {
        layoutMayBeStale ||= patchMayAffectLayout(graph, patch, { explicitlyPlacedNodeIds, pendingRemovalNodeIds });

        // Once a graph update touches a node, any removal the designer was expecting has happened.
        const patchedNodeId = getPatchedNodeId(patch);
        if (patchedNodeId !== null) {
          pendingRemovalNodeIds.delete(patchedNodeId);
        }

        if (patch.op === "addNode") {
          const origin = placements.get(patch.node.id);
          if (origin) {
            newNodeOrigins.set(patch.node.id, origin);
          }
        }

        applyGraphPatch(graph, nodeLayouts, patch);
      }

      // A `clearGraph` patch removes nodes without a `removeNode` patch for each of them.
      for (const nodeId of pendingRemovalNodeIds) {
        if (!graph.nodes.has(nodeId)) {
          pendingRemovalNodeIds.delete(nodeId);
        }
      }
      store.set(pendingRemovalNodeIdsAtom, pendingRemovalNodeIds);

      const layoutRequired = layoutMayBeStale && graph.nodes.size > 0;

      if (graph.nodes.size === 0) {
        clientGraphsRef.current.rendered = null;
      }

      for (const nodeId of newNodeOrigins.keys()) {
        // Set before applyGraph mounts the node so Motion sees the compact initial state.
        store.set(resourceNodeIsCommittingAtomFamily(nodeId), true);
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
        clientGraphsRef.current.rendered = null;
        return "completed";
      }

      await waitForAnimationFrame();

      const measuredGraph = buildRenderedGraph(graph, measureNodes(store));

      if (!isReset && renderedGraphsEqual(clientGraphsRef.current.rendered, measuredGraph)) {
        // Nothing was resized since the last layout, so the positions still hold. Reveal the graph in
        // case it is still behind the visibility gate. A reset skips this: the measurements are
        // unchanged when the user has only dragged nodes, which is exactly when it must still run.
        await applyGraphLayout(new Map());
        return "completed";
      }

      const layoutResponse = await api.fetchGraphLayout(measuredGraph);

      if (layoutResponse.status === "graphChanged") {
        return "graphChanged";
      }

      if (layoutResponse.status === "layoutFailed") {
        // No usable layout — reveal the graph as-is so it isn't stuck hidden.
        await applyGraphLayout(new Map());
        return "completed";
      }

      const { nodeLayouts, graphBounds } = extractGraphLayout(layoutResponse.patches);
      const { nodeLayouts: centeredNodeLayouts, bounds } = centerGraphLayout(
        nodeLayouts,
        graphBounds,
        getViewportCenter(),
      );
      clientGraphsRef.current.rendered = measuredGraph;

      if (!isReset) {
        // Fit the viewport to the server-computed graph bounds before the nodes settle there.
        if (bounds) {
          fitViewToBounds(bounds);
        }
        await applyGraphLayout(centeredNodeLayouts);
        return "completed";
      }

      // A reset re-arranges the graph the user is already looking at, so it keeps their camera. It is
      // one undoable step, from where the nodes are now (stopping any animation) to the new layout.
      stopNodeAnimations();
      const positionsBefore = captureNodePositions(store);
      await applyGraphLayout(centeredNodeLayouts);
      store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, centeredNodeLayouts));

      return "completed";
    },
    [api, applyGraphLayout, fitViewToBounds, getViewportCenter, stopNodeAnimations, store],
  );

  useEffect(() => {
    coordinator.setOperations({ fetchUpdate, applyUpdate, runGraphLayout });
  }, [applyUpdate, coordinator, fetchUpdate, runGraphLayout]);

  const requestGraphUpdate = useCallback(() => coordinator.requestUpdate(), [coordinator]);

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
    requestGraphUpdate,
    resetGraphLayout,
    createResourceAt,
    handleNodeDragStart,
    handleNodeDragEnd,
    undo,
    redo,
  };
}
