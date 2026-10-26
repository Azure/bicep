// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphResult, MeasuredGraph } from "../api";
import type { GraphLayoutMode, GraphLayoutResult, GraphUpdateCoordinator } from "../graph-update-coordinator";

import { useStore } from "jotai";
import { useCallback, useEffect, useRef } from "react";
import { getNodeTargetBoxes, graphCommittedAtom, layoutReadyAtom } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  collapsedModuleIdsAtom,
  commitPendingResourcesAtom,
  documentErrorCountAtom,
  pendingPlacementsAtom,
  pendingRemovalNodeIdsAtom,
  replayableSourceStepKeysAtom,
  shownGraphAtom,
  sourceGraphAtom,
  sourceGraphRevisionAtom,
  targetScopeAtom,
  undoHistoryAtom,
} from "../atoms";
import { centerGraphLayout, getLayoutCenters, graphChangeMayAffectLayout } from "../graph-layout";
import { clientGraphsRenderEqually, indexGraph, measuredGraphsEqual, measureGraph, projectGraph } from "../graph-model";
import { forgetRemovedNodePositions, getNodePositions, moveNodes, setNodePositions } from "../node-positions";
import { isMostlyNewGraph, showGraph } from "../show-graph";
import { forgetRemovedNodes, getSourceStepKey, getSourceStepReferences, recordLayoutChange } from "../undo-history";
import { useTrackGraphChange } from "./use-track-graph-change";

type Store = ReturnType<typeof createStore>;

/** How long document changes must pause before the graph is fetched again. */
const GRAPH_UPDATE_DELAY_MS = 200;

/** Resolves once React has committed the graph last given to `lib/graph`. */
function whenGraphCommitted(store: Store): Promise<void> {
  if (store.get(graphCommittedAtom)) {
    return Promise.resolve();
  }

  return new Promise((resolve) => {
    const unsubscribe = store.sub(graphCommittedAtom, () => {
      if (store.get(graphCommittedAtom)) {
        unsubscribe();
        resolve();
      }
    });
  });
}

function waitForAnimationFrame(): Promise<void> {
  return new Promise((resolve) => requestAnimationFrame(() => resolve()));
}

export interface GraphSync {
  /** The Bicep file may have changed: withdraw what depended on it and fetch the graph once edits pause. */
  handleDocumentChange: () => void;
  resetGraphLayout: () => Promise<void>;
}

/**
 * Keeps the client graph in step with the Bicep file: graph updates, layout, and Reset Layout, which is
 * one undo step. Mounted once, by `GraphActionsProvider`.
 *
 * Ordering lives in `GraphUpdateCoordinator`, which has no React dependency and is unit tested
 * directly — the rules there govern hazards that are impractical to force end to end.
 */
export function useGraphSync(
  coordinator: GraphUpdateCoordinator<GetGraphResult>,
  getViewportCenter: () => Point,
  fitViewToBounds: (bounds: Box) => void,
): GraphSync {
  const store = useStore();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();

  /** The last graph submitted for layout, so a pass can skip layout when nothing was resized. */
  const lastLaidOutGraphRef = useRef<MeasuredGraph | null>(null);

  /** The pending graph update for a document change, which waits for a pause in the edits. */
  const updateTimerRef = useRef<number | undefined>(undefined);
  useEffect(() => () => window.clearTimeout(updateTimerRef.current), []);

  /** Show the graph if it is still hidden behind the visibility gate, once its nodes have mounted. */
  const revealGraph = useCallback(async () => {
    if (!store.get(layoutReadyAtom)) {
      await waitForAnimationFrame();
      store.set(layoutReadyAtom, true);
    }
  }, [store]);

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
      const previousSource = store.get(sourceGraphAtom);
      const previousShown = store.get(shownGraphAtom);
      const source = indexGraph(response.graph);
      const shown = projectGraph(source, store.get(collapsedModuleIdsAtom));
      store.set(sourceGraphAtom, source);
      store.set(sourceGraphRevisionAtom, response.revision ?? null);

      const placements = store.get(pendingPlacementsAtom);
      const pendingRemovalNodeIds = store.get(pendingRemovalNodeIdsAtom);
      const layoutRequired =
        shown.nodes.size > 0 &&
        graphChangeMayAffectLayout(previousShown, shown, {
          explicitlyPlacedNodeIds: new Set(placements.keys()),
          pendingRemovalNodeIds,
        });

      // A removal the designer was expecting has happened once the node is gone.
      store.set(pendingRemovalNodeIdsAtom, new Set([...pendingRemovalNodeIds].filter((id) => source.nodes.has(id))));

      // Forget where removed nodes were, so a later node that reuses an ID starts afresh. Hidden nodes are
      // still in the source graph, so they keep their positions and history.
      const positions = getNodePositions(store);
      const keptPositions = forgetRemovedNodePositions(positions, source);
      if (keptPositions !== positions) {
        setNodePositions(store, keptPositions, "instant");
        const liveNodeIds = new Set([...keptPositions.keys(), ...placements.keys()]);
        store.set(undoHistoryAtom, (history) => forgetRemovedNodes(history, liveNodeIds));
      }

      if (shown.nodes.size === 0) {
        lastLaidOutGraphRef.current = null;
      }

      // Nothing the canvas shows has changed, so leave the mounted nodes alone rather than tearing the
      // graph down and re-laying it out. Most keystrokes land here.
      if (clientGraphsRenderEqually(previousShown, shown)) {
        return { layoutRequired };
      }

      // When most of the graph is replaced, hide it until it is laid out rather than show new nodes piled at
      // their spawn point. Smaller changes stay visible, so nodes animate in place.
      if (shown.nodes.size > 0 && isMostlyNewGraph(previousShown, shown)) {
        store.set(layoutReadyAtom, false);
      }

      // Nodes dropped by the user appear where they were dropped; other new nodes wait for the layout.
      const newNodeOrigins = new Map(
        [...placements].filter(([nodeId]) => shown.nodes.has(nodeId) && !previousSource.nodes.has(nodeId)),
      );
      showGraph(store, shown, {
        origins: newNodeOrigins,
        getDefaultOrigin: getViewportCenter,
        placeholderNodeIds: new Set(newNodeOrigins.keys()),
      });

      if (newNodeOrigins.size > 0) {
        store.set(commitPendingResourcesAtom, new Set(newNodeOrigins.keys()));

        if (!layoutRequired) {
          // An explicitly placed node is already where the user dropped it, so no layout is owed --
          // but the graph may still be behind the visibility gate.
          await revealGraph();
        }
      }

      return { layoutRequired };
    },
    [getViewportCenter, revealGraph, store],
  );

  const runGraphLayout = useCallback(
    async (mode: GraphLayoutMode): Promise<GraphLayoutResult> => {
      const isReset = mode === "reset";
      const shown = store.get(shownGraphAtom);
      const revision = store.get(sourceGraphRevisionAtom);

      if (shown.nodes.size === 0) {
        lastLaidOutGraphRef.current = null;
        return "completed";
      }

      // Measure once React has committed the shown nodes, so each has measured what it now renders.
      await whenGraphCommitted(store);

      // A module opened or closed meanwhile, so lay out what is shown now.
      if (store.get(shownGraphAtom) !== shown) {
        return "graphChanged";
      }

      const measuredGraph = measureGraph(shown, getNodeTargetBoxes(store));

      if (!isReset && measuredGraphsEqual(lastLaidOutGraphRef.current, measuredGraph)) {
        // Nothing was resized since the last layout, so the positions still hold. A reset skips this: the
        // measurements are unchanged when the user has only dragged nodes, which is exactly when it must run.
        await revealGraph();
        return "completed";
      }

      const layoutResponse = await api.layoutGraph({ graph: measuredGraph, revision });

      if (layoutResponse.status === "graphChanged" || store.get(shownGraphAtom) !== shown) {
        return "graphChanged";
      }

      if (layoutResponse.status === "layoutFailed") {
        // No usable layout — reveal the graph as-is so it isn't stuck hidden.
        await revealGraph();
        return "completed";
      }

      const { positions: topLeftCorners, bounds } = centerGraphLayout(
        layoutResponse.positions,
        layoutResponse.bounds,
        getViewportCenter(),
      );
      lastLaidOutGraphRef.current = measuredGraph;

      // The contents of collapsed modules move with their cards, so they are in place when one opens.
      const positionsBefore = getNodePositions(store);
      const positionsAfter = moveNodes(
        positionsBefore,
        getLayoutCenters(topLeftCorners, measuredGraph),
        store.get(sourceGraphAtom),
        store.get(collapsedModuleIdsAtom),
      );

      // An automatic layout frames what arrived, before the nodes settle there. A reset re-arranges the
      // graph the user is already looking at, so it keeps their camera and is one undoable step.
      if (!isReset && bounds) {
        fitViewToBounds(bounds);
      }

      await revealGraph();
      setNodePositions(store, positionsAfter, "animate");

      if (isReset) {
        store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, positionsAfter));
      }

      return "completed";
    },
    [api, fitViewToBounds, getViewportCenter, revealGraph, store],
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
      trackGraphChange(() => coordinator.requestGraphLayout("reset")).catch((error: unknown) => {
        console.error(
          "Visual designer layout reset failed:",
          getErrorMessage(error, "Failed to reset the graph layout."),
        );
      }),
    [coordinator, trackGraphChange],
  );

  return { handleDocumentChange, resetGraphLayout };
}
