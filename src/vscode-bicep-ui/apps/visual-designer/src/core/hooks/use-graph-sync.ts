// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphResult, MeasuredGraph } from "../api";
import type { ClientGraph } from "../graph-model";
import type { GraphLayoutMode, GraphLayoutResult, GraphUpdateCoordinator } from "../graph-update-coordinator";

import { useStore } from "jotai";
import { useCallback, useEffect, useRef } from "react";
import { getNodeSizes, getNodeTargetBoxes, graphCommittedAtom, layoutReadyAtom } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  collapsedModuleIdsAtom,
  commitPendingResourcesAtom,
  documentErrorCountAtom,
  pendingPlacementsAtom,
  pendingRemovalNodeIdsAtom,
  pendingSubgraphLayoutsAtom,
  replayableSourceStepKeysAtom,
  shownGraphAtom,
  sourceGraphAtom,
  sourceGraphRevisionAtom,
  targetScopeAtom,
  undoHistoryAtom,
} from "../atoms";
import { centerGraphLayout, getLayoutCenters, getSubgraph, graphChangeMayAffectLayout } from "../graph-layout";
import {
  clientGraphsRenderEqually,
  getModulesWithNewChildren,
  indexGraph,
  isCollapsible,
  isDescendant,
  measuredGraphsEqual,
  measureGraph,
  projectGraph,
} from "../graph-model";
import {
  forgetRemovedNodePositions,
  getNodePositions,
  getPositionedNodeIds,
  moveNodes,
  setNodePositions,
} from "../node-positions";
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

/**
 * Each shown node's box, for measuring the graph. A node waiting to be placed, such as a child of a module that just
 * opened, has no target box yet, so its measured size stands in.
 */
function getMeasuredBoxes(store: Store): Map<string, Box> {
  const boxes = getNodeTargetBoxes(store);

  for (const [nodeId, { width, height }] of getNodeSizes(store)) {
    if (!boxes.has(nodeId)) {
      boxes.set(nodeId, { min: { x: 0, y: 0 }, max: { x: width, y: height } });
    }
  }

  return boxes;
}

/**
 * Place each subgraph's nodes that are still waiting for a layout at its anchor, so none stays hidden when no layout
 * comes. A later layout moves them from there.
 */
function placeAtAnchors(store: Store, shown: ClientGraph, subgraphs: Iterable<readonly [string, Point]>) {
  const positions = getNodePositions(store);
  const waitingNodeIds = getPositionedNodeIds(shown).filter((nodeId) => !positions.has(nodeId));
  const next = new Map(positions);

  for (const [rootId, anchor] of subgraphs) {
    for (const nodeId of waitingNodeIds) {
      if (!next.has(nodeId) && isDescendant(shown, nodeId, rootId)) {
        next.set(nodeId, anchor);
      }
    }
  }

  if (next.size !== positions.size) {
    setNodePositions(store, next, "instant");
  }
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

      // Children that arrive for a remote module stay hidden until the user opens it, so nothing moves.
      const arrivedModuleIds = getModulesWithNewChildren(previousSource, source);
      if (arrivedModuleIds.length > 0) {
        store.set(collapsedModuleIdsAtom, (ids) => new Set([...ids, ...arrivedModuleIds]));
      }

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

  /**
   * Lay out each subgraph in `pendingSubgraphLayoutsAtom` whose root is shown open, on its own, centered on its
   * anchor. Other nodes and the camera stay where they are, so the subgraph may overlap its neighbors.
   */
  const layOutSubgraphs = useCallback(
    async (shown: ClientGraph, measuredGraph: MeasuredGraph, revision: string | null): Promise<GraphLayoutResult> => {
      const pending = store.get(pendingSubgraphLayoutsAtom);
      const isOpen = (rootId: string) => {
        const node = shown.nodes.get(rootId);
        return isCollapsible(node) && node?.isCollapsed !== true;
      };
      // A subgraph inside another that is laid out is placed with it.
      const outermost = [...pending].filter(
        ([rootId]) =>
          isOpen(rootId) &&
          ![...pending.keys()].some((otherId) => otherId !== rootId && isDescendant(shown, rootId, otherId)),
      );
      const centers = new Map<string, Point>();
      const unplacedSubgraphs: [string, Point][] = [];

      for (const [rootId, anchor] of outermost) {
        const graph = getSubgraph(measuredGraph, rootId);
        let response: Awaited<ReturnType<typeof api.layoutGraph>> | null = null;

        try {
          response = await api.layoutGraph({ graph, revision });
        } catch (error) {
          console.error("Visual designer layout failed:", getErrorMessage(error, "Failed to lay out the graph."));
        }

        if (response?.status === "graphChanged" || store.get(shownGraphAtom) !== shown) {
          return "graphChanged";
        }

        if (response?.status === "ok") {
          const { positions } = centerGraphLayout(response.positions, response.bounds, anchor);
          for (const [nodeId, center] of getLayoutCenters(positions, graph)) {
            centers.set(nodeId, center);
          }
        } else {
          unplacedSubgraphs.push([rootId, anchor]);
        }
      }

      const source = store.get(sourceGraphAtom);
      if (centers.size > 0) {
        // Nodes waiting for this layout are hidden, so they appear where it puts them as their module opens around
        // them; nodes already shown move there.
        setNodePositions(
          store,
          moveNodes(getNodePositions(store), centers, source, store.get(collapsedModuleIdsAtom)),
          "animate",
        );
      }
      placeAtAnchors(store, shown, unplacedSubgraphs);
      // Subgraphs whose root closed again stay pending, so they are laid out when it opens.
      store.set(
        pendingSubgraphLayoutsAtom,
        new Map(
          [...pending].filter(
            ([rootId]) => source.nodes.has(rootId) && !outermost.some(([laidOutId]) => laidOutId === rootId),
          ),
        ),
      );
      await revealGraph();

      return "completed";
    },
    [api, revealGraph, store],
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

      const measuredGraph = measureGraph(shown, getMeasuredBoxes(store));

      if (mode === "subgraph") {
        return layOutSubgraphs(shown, measuredGraph, revision);
      }

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
        // No usable layout — reveal the graph as-is so it isn't stuck hidden, nor are nodes waiting for a layout.
        placeAtAnchors(store, shown, store.get(pendingSubgraphLayoutsAtom));
        await revealGraph();
        return "completed";
      }

      const { positions: topLeftCorners, bounds } = centerGraphLayout(
        layoutResponse.positions,
        layoutResponse.bounds,
        getViewportCenter(),
      );
      lastLaidOutGraphRef.current = measuredGraph;

      // The descendants of collapsed modules move with them, so they are in place when one opens.
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
      // The whole graph was placed, including any pending subgraph.
      store.set(pendingSubgraphLayoutsAtom, new Map());

      if (isReset) {
        store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, positionsAfter));
      }

      return "completed";
    },
    [api, fitViewToBounds, getViewportCenter, layOutSubgraphs, revealGraph, store],
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
