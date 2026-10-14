// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { GetGraphResult, MeasuredGraph } from "../api";
import type { GraphActions } from "../context/GraphActionsContext";
import type { ClientGraph } from "../graph-model";
import type { GraphLayoutMode, GraphLayoutResult } from "../graph-update-coordinator";
import type { NodePositions } from "../node-positions";
import type { NodeEntrance } from "./use-apply-graph";

import { useStore } from "jotai";
import { animate } from "motion";
import { useCallback, useEffect, useRef, useState } from "react";
import { fadedNodeIdsAtom, focusedNodeIdAtom, nodesByIdAtom } from "@/lib/graph";
import { getErrorMessage } from "@/utils";
import { useGraphApi } from "../api";
import {
  commitPendingResourcesAtom,
  documentErrorCountAtom,
  isGraphChangeInProgressAtom,
  motionPolicyAtom,
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
  projectGraph,
} from "../graph-model";
import { GraphUpdateCoordinator } from "../graph-update-coordinator";
import { MODULE_CONTENTS_FADE_MS, MODULE_MORPH_TRANSITION, shouldReduceMotion } from "../motion";
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

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => window.setTimeout(resolve, ms));
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

function getNodeCenter(box: Box): Point {
  return {
    x: (box.min.x + box.max.x) / 2,
    y: (box.min.y + box.max.y) / 2,
  };
}

function captureAtomicNodeCenters(store: Store, rootNodeId: string): Map<string, Point> {
  const centers = new Map<string, Point>();

  for (const [nodeId, node] of Object.entries(store.get(nodesByIdAtom))) {
    if (node.kind === "atomic" && nodeId.startsWith(`${rootNodeId}::`)) {
      centers.set(nodeId, getNodeCenter(store.get(node.boxAtom)));
    }
  }

  return centers;
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
  const { applyGraphLayout, animateNodePositions, stopNodeAnimations, captureSettledNodePositions } =
    useApplyGraphLayout();
  const api = useGraphApi();
  const trackGraphChange = useTrackGraphChange();
  const [coordinator] = useState(() => new GraphUpdateCoordinator<GetGraphResult>());
  const createResourceAt = useResourceCreation(coordinator);
  const { undo, redo } = useUndoRedo(coordinator, { animateNodePositions, stopNodeAnimations });

  /**
   * The two graphs the client holds: the latest graph from the host, and the last measured graph it
   * submitted for layout — kept so a pass can skip layout when measured sizes are unchanged.
   */
  const clientGraphsRef = useRef<{ source: ClientGraph; rendered: ClientGraph; measured: MeasuredGraph | null }>({
    source: EMPTY_CLIENT_GRAPH,
    rendered: EMPTY_CLIENT_GRAPH,
    measured: null,
  });
  /** Module IDs the user collapsed. A local view choice: the host's graph never changes. */
  const collapsedNodeIdsRef = useRef(new Set<string>());
  /** Where the contents of collapsed modules were, so expanding puts them back. */
  const hiddenNodeCentersRef = useRef(new Map<string, Point>());
  /** Each collapsed module's top-left corner when it collapsed, to tell how far its card has moved since. */
  const collapsedModuleAnchorsRef = useRef(new Map<string, Point>());

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
      const previousSource = clientGraphsRef.current.source;
      const previousRendered = clientGraphsRef.current.rendered;
      const source = indexGraph(response.graph);
      collapsedNodeIdsRef.current = new Set(
        [...collapsedNodeIdsRef.current].filter((nodeId) => {
          const node = source.nodes.get(nodeId);
          return node?.kind === "module" && node.hasChildren;
        }),
      );
      const graph = projectGraph(source, collapsedNodeIdsRef.current);
      clientGraphsRef.current.source = source;
      clientGraphsRef.current.rendered = graph;

      const placements = store.get(pendingPlacementsAtom);
      const pendingRemovalNodeIds = store.get(pendingRemovalNodeIdsAtom);
      const layoutRequired =
        graph.nodes.size > 0 &&
        graphChangeMayAffectLayout(previousRendered, graph, {
          explicitlyPlacedNodeIds: new Set(placements.keys()),
          pendingRemovalNodeIds,
        });

      // A removal the designer was expecting has happened once the node is gone.
      store.set(pendingRemovalNodeIdsAtom, new Set([...pendingRemovalNodeIds].filter((id) => source.nodes.has(id))));

      const newNodeOrigins = new Map(
        [...placements].filter(([nodeId]) => graph.nodes.has(nodeId) && !previousSource.nodes.has(nodeId)),
      );

      if (graph.nodes.size === 0) {
        clientGraphsRef.current.measured = null;
      }

      // Nothing the canvas shows has changed, so leave the mounted nodes alone rather than tearing the
      // graph down and re-laying it out. Most keystrokes land here.
      if (clientGraphsRenderEqually(previousRendered, graph)) {
        return { layoutRequired };
      }
      // Apply the new topology. Visibility is preserved for incremental edits (so nodes animate in
      // place) and gated for major changes; positions arrive with the layout.
      const previousNodesById = store.get(nodesByIdAtom);
      applyGraph(graph, newNodeOrigins);
      if (store.get(nodesByIdAtom) !== previousNodesById) {
        const liveNodeIds = new Set([
          ...[...source.nodes.values()].filter((node) => !node.hasChildren).map((node) => node.id),
          ...collapsedNodeIdsRef.current,
          ...placements.keys(),
        ]);
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
      const graph = clientGraphsRef.current.rendered;

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

      const layoutResponse = await api.layoutGraph({
        graph: measuredGraph,
        collapsedNodeIds: [...collapsedNodeIdsRef.current],
      });

      // A local collapse/expand can replace the rendered projection while this request is in flight.
      // Its response describes the old projection, so it must not move nodes in the new one.
      if (clientGraphsRef.current.rendered !== graph) {
        return "completed";
      }

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
      // one undoable step, from where the nodes were settling to the new layout: undoing it must not
      // stop partway through an animation that the reset interrupted.
      const positionsBefore = captureSettledNodePositions();
      stopNodeAnimations();
      await applyGraphLayout(centeredPositions);
      store.set(undoHistoryAtom, (history) => recordLayoutChange(history, positionsBefore, centeredPositions));

      return "completed";
    },
    [api, applyGraphLayout, captureSettledNodePositions, fitViewToBounds, getViewportCenter, stopNodeAnimations, store],
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

  const toggleModuleCollapsed = useCallback(
    (nodeId: string) => {
      if (store.get(isGraphChangeInProgressAtom)) {
        return;
      }

      const sourceNode = clientGraphsRef.current.source.nodes.get(nodeId);
      const renderedNode = store.get(nodesByIdAtom)[nodeId];
      const isCollapsing = !collapsedNodeIdsRef.current.has(nodeId);

      if (
        sourceNode?.kind !== "module" ||
        !sourceNode.hasChildren ||
        !renderedNode ||
        renderedNode.kind !== (isCollapsing ? "compound" : "atomic")
      ) {
        return;
      }

      const isDescendant = (id: string) => id.startsWith(`${nodeId}::`);
      const fade = (ids: readonly string[]) => store.set(fadedNodeIdsAtom, (faded) => new Set([...faded, ...ids]));
      const unfade = (ids: readonly string[]) =>
        store.set(fadedNodeIdsAtom, (faded) => new Set([...faded].filter((id) => !ids.includes(id))));

      const toggle = async () => {
        stopNodeAnimations();
        const reduceMotion = shouldReduceMotion(store.get(motionPolicyAtom));
        const moduleBox = store.get(renderedNode.boxAtom);
        const collapsedNodeIds = new Set(collapsedNodeIdsRef.current);
        const entrances = new Map<string, NodeEntrance>();
        let hiddenNodeIds: string[] = [];

        if (isCollapsing) {
          for (const [hiddenNodeId, center] of captureAtomicNodeCenters(store, nodeId)) {
            hiddenNodeCentersRef.current.set(hiddenNodeId, center);
          }
          // The card shrinks to the container's center, so restoring the contents is relative to it.
          collapsedModuleAnchorsRef.current.set(nodeId, getNodeCenter(moduleBox));

          if (!reduceMotion) {
            hiddenNodeIds = Object.keys(store.get(nodesByIdAtom)).filter(isDescendant);
            fade(hiddenNodeIds);
            await delay(MODULE_CONTENTS_FADE_MS);

            // A graph update while the contents faded may have removed the module.
            if (!clientGraphsRef.current.source.nodes.get(nodeId)?.hasChildren) {
              unfade(hiddenNodeIds);
              return;
            }
          }

          collapsedNodeIds.add(nodeId);
          entrances.set(nodeId, {
            data: {
              morphFrom: reduceMotion
                ? undefined
                : { width: moduleBox.max.x - moduleBox.min.x, height: moduleBox.max.y - moduleBox.min.y },
            },
          });
        } else {
          // Restore the contents relative to wherever the card was moved while collapsed.
          const cardCenter = getNodeCenter(moduleBox);
          const anchor = collapsedModuleAnchorsRef.current.get(nodeId) ?? cardCenter;
          const dx = cardCenter.x - anchor.x;
          const dy = cardCenter.y - anchor.y;

          for (const [hiddenNodeId, center] of hiddenNodeCentersRef.current) {
            if (isDescendant(hiddenNodeId)) {
              hiddenNodeCentersRef.current.set(hiddenNodeId, { x: center.x + dx, y: center.y + dy });
            }
          }

          collapsedModuleAnchorsRef.current.delete(nodeId);
          collapsedNodeIds.delete(nodeId);
          if (!reduceMotion) {
            entrances.set(nodeId, { enterFrom: moduleBox, data: { morphFrom: "card" } });
          }
        }

        collapsedNodeIdsRef.current = collapsedNodeIds;
        const graph = projectGraph(clientGraphsRef.current.source, collapsedNodeIds);
        clientGraphsRef.current.rendered = graph;
        clientGraphsRef.current.measured = null;

        const revealedNodeIds = isCollapsing ? [] : [...graph.nodes.keys()].filter(isDescendant);
        const moduleCenter = getNodeCenter(moduleBox);
        for (const revealedNodeId of revealedNodeIds) {
          entrances.set(revealedNodeId, {
            origin: hiddenNodeCentersRef.current.get(revealedNodeId) ?? moduleCenter,
          });
        }

        if (!reduceMotion) {
          // Mount the contents transparent, so they fade in once the frame has grown around them.
          fade(revealedNodeIds);
        }

        applyGraph(graph, new Map(), entrances);
        // The hidden contents are unmounted now, so they no longer need fading.
        unfade(hiddenNodeIds);

        const focusedNodeId = store.get(focusedNodeIdAtom);
        if (isCollapsing && focusedNodeId && isDescendant(focusedNodeId)) {
          store.set(focusedNodeIdAtom, nodeId);
        }

        if (isCollapsing) {
          if (!reduceMotion) {
            // The card centers itself as it shrinks. Keep the change in progress until it settles,
            // so a drag cannot fight the morph for the card's position.
            await delay(MODULE_MORPH_TRANSITION.duration * 1000);
          }
          return;
        }

        store.set(undoHistoryAtom, (history) => forgetRemovedNodes(history, getAtomicNodeIds(store)));

        const expandedNode = store.get(nodesByIdAtom)[nodeId];
        if (reduceMotion || expandedNode?.kind !== "compound") {
          unfade(revealedNodeIds);
          return;
        }

        const growth = animate(0, 1, {
          ...MODULE_MORPH_TRANSITION,
          onUpdate: (progress) => store.set(expandedNode.enterProgressAtom, progress),
        });
        // Bring the contents in while the frame is still settling around them.
        await delay(MODULE_MORPH_TRANSITION.duration * 500);
        unfade(revealedNodeIds);
        await growth;
        store.set(expandedNode.enterProgressAtom, 1);
      };

      trackGraphChange(toggle).catch((error: unknown) => {
        console.error("Visual designer module toggle failed:", getErrorMessage(error, "Failed to toggle the module."));
      });
    },
    [applyGraph, stopNodeAnimations, store, trackGraphChange],
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
